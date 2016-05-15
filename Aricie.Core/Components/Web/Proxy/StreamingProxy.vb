Imports System
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Web
Imports System.Web.Caching
Imports Aricie.IO

Namespace Web.Proxy


    Public Class StreamingProxy
        Implements IHttpAsyncHandler
        Implements IHttpHandler


        Const BUFFER_SIZE As Integer = 8 * 1024
        Private Const STR_ContentEncoding As String = "Content-Encoding"
        Private Const STR_ContentLength As String = "Content-Length"
        Private Const STR_ContentDisposition As String = "Content-Disposition"
        Public Const STR_METHOD As String = "m"
        Private Const STR_GET As String = "GET"
        Public Const STR_URL As String = "u"
        Public Const STR_CACHE_DURATION As String = "c"
        Public Const STR_CONTENTTYPE As String = "t"
        Private Const STR_DEFAULT_CACHE_DURATION As String = "0"

        Private _PipeStream As PipeStream
        Private _ResponseStream As Stream

#Region "Http Sync Handler ProcessRequest"




        Public Overridable Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            Dim method As String = Nothing
            Dim url As String = Nothing
            Dim cacheDuration As Integer
            Dim contentType As String = Nothing
            InitParams(context, url, method, cacheDuration, contentType)
            If String.IsNullOrEmpty(url) Then
                context.Response.[End]()
                Exit Sub
            End If


            Log.WriteLine(Convert.ToString((Convert.ToString((method & Convert.ToString(" ")) + cacheDuration.ToString() + " ") & contentType) + " ") & url)
            ' If cache duration specified, emit response header so that browser caches the response for specified duration.
            If cacheDuration > 0 Then
                If context.Cache(url) IsNot Nothing Then
                    Dim content As CachedContent = TryCast(context.Cache(url), CachedContent)

                    If Not String.IsNullOrEmpty(content.ContentEncoding) Then
                        context.Response.AppendHeader(STR_ContentEncoding, content.ContentEncoding)
                    End If
                    If Not String.IsNullOrEmpty(content.ContentLength) Then
                        context.Response.AppendHeader(STR_ContentLength, content.ContentLength)
                    End If

                    context.Response.ContentType = content.ContentType

                    content.Content.Position = 0
                    content.Content.WriteTo(context.Response.OutputStream)
                End If
            End If

            Using New TimedLog(Convert.ToString("StreamingProxy" & vbTab) & url)
                Dim outRequest As HttpWebRequest = ProxyHelper.CreateScalableHttpWebRequest(url)
                ' As we will stream the response, don't want to automatically decompress the content
                ' when source sends compressed content
                outRequest.AutomaticDecompression = DecompressionMethods.None

                CopyCookies(context.Request, outRequest)

                If Not String.IsNullOrEmpty(contentType) Then
                    outRequest.ContentType = contentType
                End If

                outRequest.Method = method
                outRequest.UserAgent = context.Request.UserAgent

                ' If there's a body provided then upload that            
                If DoesRequestHaveBody(method) Then
                    If context.Request.ContentLength > 0 Then
                        outRequest.ContentLength = context.Request.ContentLength
                    End If

                    Using outStream As Stream = outRequest.GetRequestStream()
                        CopyRequestStream(context.Request.InputStream, outStream)
                    End Using
                End If

                Using New TimedLog("StreamingProxy" & vbTab & "Total GetResponse and transmit data")
                    Using response As HttpWebResponse = TryCast(outRequest.GetResponse(), HttpWebResponse)
                        Me.DownloadData(outRequest, response, context, cacheDuration)
                    End Using
                End Using
            End Using
        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

#End Region

#Region "Http Async Handler Begin/End ProcessRequest"

        Public Overridable Function BeginProcessRequest(context As HttpContext, cb As AsyncCallback, extraData As Object) As IAsyncResult Implements IHttpAsyncHandler.BeginProcessRequest
            Dim method As String = Nothing
            Dim url As String = Nothing
            Dim cacheDuration As Integer
            Dim contentType As String = Nothing
            InitParams(context, url, method, cacheDuration, contentType)

            Try
                If String.IsNullOrEmpty(url) Then
                    context.Response.[End]()
                    Return Nothing
                End If

                If cacheDuration > 0 Then
                    Dim cachedResult As CachedContent = TryCast(context.Cache(url), CachedContent)
                    If cachedResult IsNot Nothing Then
                        ' We have response to this URL already cached
                        Dim result As New SyncResult()
                        result.Context = context
                        result.Content = cachedResult
                        Return result
                    End If
                End If

                Dim outRequest As HttpWebRequest = ProxyHelper.CreateScalableHttpWebRequest(url)
                ' As we will stream the response, don't want to automatically decompress the content
                ' when source sends compressed content
                outRequest.AutomaticDecompression = DecompressionMethods.None
                outRequest.Method = method

                CopyCookies(context.Request, outRequest)

                If Not String.IsNullOrEmpty(contentType) Then
                    outRequest.ContentType = contentType
                End If

                Dim state As New AsyncState()
                state.Method = method
                state.Context = context
                state.Url = url
                state.CacheDuration = cacheDuration
                state.OutboundRequest = outRequest

                ' If there's a body provided then upload that            
                If DoesRequestHaveBody(method) Then
                    If context.Request.ContentLength > 0 Then
                        outRequest.ContentLength = context.Request.ContentLength
                    End If

                    Return outRequest.BeginGetRequestStream(cb, state)
                Else
                    Return outRequest.BeginGetResponse(cb, state)
                End If

            Catch ex As Exception
                Dim message As String = String.Format("Error in streaming proxy while processing url {0} from original url {1}", url, HttpContext.Current.Request.Url.ToString())
                Dim appEx As New ApplicationException(message, ex)
                Throw appEx
            End Try
            Return Nothing
           
        End Function

        Public Overridable Sub EndProcessRequest(result As IAsyncResult) Implements IHttpAsyncHandler.EndProcessRequest
            If result.CompletedSynchronously Then
                ' Content is already available in the cache and can be delivered from cache
                Dim syncResult As SyncResult = TryCast(result, SyncResult)
                If syncResult IsNot Nothing Then
                    syncResult.Context.Response.ContentType = syncResult.Content.ContentType
                    If Not syncResult.Content.ContentEncoding.IsNullOrEmpty() Then
                        syncResult.Context.Response.AppendHeader(STR_ContentEncoding, syncResult.Content.ContentEncoding)
                    End If
                    syncResult.Context.Response.AppendHeader(STR_ContentLength, syncResult.Content.ContentLength)
                    If Not syncResult.Content.ContentDisposition.IsNullOrEmpty() Then
                        syncResult.Context.Response.AppendHeader(STR_ContentDisposition, syncResult.Content.ContentDisposition)
                    End If

                    syncResult.Content.Content.Seek(0, SeekOrigin.Begin)
                    syncResult.Content.Content.WriteTo(syncResult.Context.Response.OutputStream)
                End If
            Else
                ' Content is not available in cache and needs to be downloaded from external source
                Dim state As AsyncState = TryCast(result.AsyncState, AsyncState)
                If state IsNot Nothing Then
                    'state.Context.Response.Buffer = False
                    state.Context.Response.Buffer = True
                    Dim outRequest As HttpWebRequest = state.OutboundRequest
                    Try
                        If DoesRequestHaveBody(state.Method) Then
                            Using outStream As Stream = outRequest.EndGetRequestStream(result)
                                Me.CopyRequestStream(state.Context.Request.InputStream, outStream)

                                Using outResponse As HttpWebResponse = TryCast(outRequest.GetResponse(), HttpWebResponse)
                                    Me.DownloadData(outRequest, outResponse, state.Context, state.CacheDuration)
                                End Using
                            End Using
                        Else
                            Using outResponse As HttpWebResponse = TryCast(outRequest.EndGetResponse(result), HttpWebResponse)
                                Me.DownloadData(outRequest, outResponse, state.Context, state.CacheDuration)
                            End Using
                        End If
                    Catch ex As Exception

                        Dim message As String = String.Format("Error in streaming proxy while processing url {0} from original url {1}", state.Url, state.Context.Request.Url.ToString())
                        Dim appEx As New ApplicationException(message, ex)
                        Throw appEx
                    End Try

                End If
            End If
        End Sub

#End Region

#Region "Private stuff"



        Private Sub CopyCookies(request As HttpRequest, outRequest As HttpWebRequest)
            Dim cookieContainer As New CookieContainer()
            outRequest.CookieContainer = cookieContainer
            For Each cookieName As String In request.Cookies
                Dim cookie As HttpCookie = request.Cookies(cookieName)
                If cookie.Domain.IsNullOrEmpty() Then
                    cookieContainer.Add(New Cookie(cookie.Name, cookie.Value, cookie.Path, outRequest.RequestUri.Host))
                Else
                    cookieContainer.Add(New Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain))
                End If
            Next
        End Sub
        Private Sub DownloadData(request As HttpWebRequest, response As HttpWebResponse, context As HttpContext, cacheDuration As Integer)
            Dim responseBuffer As New MemoryStream()
            'context.Response.Buffer = False
            context.Response.Buffer = True
            Try
                If response.StatusCode <> HttpStatusCode.OK Then
                    context.Response.StatusCode = CInt(response.StatusCode)
                    Return
                End If
                Using readStream As Stream = response.GetResponseStream()
                    If context.Response.IsClientConnected Then
                        Dim contentLength As String = String.Empty
                        Dim contentEncoding As String = String.Empty
                        Dim contentDisposition As String = String.Empty
                        ProduceResponseHeader(response, context, cacheDuration, contentLength, contentEncoding, contentDisposition)

                        'int totalBytesWritten = TransmitDataInChunks(context, readStream, responseBuffer);
                        'int totalBytesWritten = TransmitDataAsync(context, readStream, responseBuffer);
                        Dim totalBytesWritten As Integer = TransmitDataAsyncOptimized(context, readStream, responseBuffer)

                        Log.WriteLine("Response generated: " + DateTime.Now.ToString())
                        Log.WriteLine(String.Format("Content Length vs Bytes Written: {0} vs {1} ", contentLength, totalBytesWritten))

                        If cacheDuration > 0 Then
                            '#Region "Cache Response in memory"
                            ' Cache the content on server for specified duration
                            Dim objCache As New CachedContent()
                            objCache.Content = responseBuffer
                            objCache.ContentEncoding = contentEncoding
                            objCache.ContentDisposition = contentDisposition
                            objCache.ContentLength = contentLength
                            objCache.ContentType = response.ContentType

                            '#End Region
                            context.Cache.Insert(request.RequestUri.ToString(), objCache, Nothing, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheDuration), CacheItemPriority.Normal, _
                                Nothing)
                        End If
                    End If

                    'Using New TimedLog("StreamingProxy" & vbTab & "Response Flush")
                    '    context.Response.Flush()
                    'End Using
                End Using
            Catch x As Exception
                'Log.WriteLine(x.ToString())
                Aricie.Services.ExceptionHelper.LogException(x)
                request.Abort()
            End Try

        End Sub

        Private Function TransmitDataInChunks(context As HttpContext, readStream As Stream, responseBuffer As MemoryStream) As Integer
            Dim buffer As Byte() = New Byte(BUFFER_SIZE - 1) {}
            Dim bytesRead As Integer
            Dim totalBytesWritten As Integer = 0

            Using New TimedLog("StreamingProxy" & vbTab & "Total read from socket and write to response")
                While (bytesRead.InlineAssignHelper(readStream.Read(buffer, 0, BUFFER_SIZE))) > 0
                    Using New TimedLog("StreamingProxy" & vbTab & "Write " & bytesRead.ToString(CultureInfo.InvariantCulture) & " to response")
                        context.Response.OutputStream.Write(buffer, 0, bytesRead)
                    End Using

                    responseBuffer.Write(buffer, 0, bytesRead)

                    totalBytesWritten += bytesRead
                End While
            End Using

            Return totalBytesWritten
        End Function

        Private Function TransmitDataAsync(context As HttpContext, readStream As Stream, responseBuffer As MemoryStream) As Integer
            Me._ResponseStream = readStream

            _PipeStream = New PipeStreamBlock(5000)

            Dim objBuffer As Byte() = New Byte(BUFFER_SIZE - 1) {}

            Dim readerThread As New Thread(New ThreadStart(AddressOf Me.ReadData))
            readerThread.Start()
            'ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReadData));

            Dim totalBytesWritten As Integer = 0
            Dim dataReceived As Integer

            Using New TimedLog("StreamingProxy" & vbTab & "Total read and write")
                While (dataReceived.InlineAssignHelper(Me._PipeStream.Read(objBuffer, 0, BUFFER_SIZE))) > 0
                    Using New TimedLog("StreamingProxy" & vbTab & "Write " & dataReceived.ToString(CultureInfo.InvariantCulture) & " to response")
                        context.Response.OutputStream.Write(objBuffer, 0, dataReceived)
                        responseBuffer.Write(objBuffer, 0, dataReceived)
                        totalBytesWritten += dataReceived
                    End Using
                End While
            End Using

            _PipeStream.Dispose()

            Return totalBytesWritten

        End Function

        Private Function TransmitDataAsyncOptimized(context As HttpContext, readStream As Stream, responseBuffer As MemoryStream) As Integer
            Me._ResponseStream = readStream

            _PipeStream = New PipeStreamBlock(10000)
            '_PipeStream = new Utility.PipeStream(10000);

            Dim objBuffer As Byte() = New Byte(BUFFER_SIZE - 1) {}

            ' Asynchronously read content form response stream
            Dim readerThread As New Thread(New ThreadStart(AddressOf Me.ReadData))
            readerThread.Start()
            'ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReadData));

            ' Write to response 
            Dim totalBytesWritten As Integer = 0
            Dim dataReceived As Integer

            Dim outputBuffer As Byte() = New Byte(BUFFER_SIZE - 1) {}
            Dim responseBufferPos As Integer = 0

            Using New TimedLog("StreamingProxy" & vbTab & "Total read and write")
                While (dataReceived.InlineAssignHelper(Me._PipeStream.Read(objBuffer, 0, BUFFER_SIZE))) > 0
                    ' if about to overflow, transmit the response buffer and restart
                    Dim bufferSpaceLeft As Integer = BUFFER_SIZE - responseBufferPos

                    If bufferSpaceLeft < dataReceived Then
                        If Not context.Response.Buffer Then
                            context.Response.Buffer = True
                        End If
                        Buffer.BlockCopy(objBuffer, 0, outputBuffer, responseBufferPos, bufferSpaceLeft)

                        Using New TimedLog("StreamingProxy" & vbTab & "Write " & BUFFER_SIZE.ToString(CultureInfo.InvariantCulture) & " to response")
                            context.Response.OutputStream.Write(outputBuffer, 0, BUFFER_SIZE)
                            responseBuffer.Write(outputBuffer, 0, BUFFER_SIZE)
                            totalBytesWritten += BUFFER_SIZE
                        End Using

                        ' Initialize response buffer and copy the bytes that were not sent
                        responseBufferPos = 0
                        Dim bytesLeftOver As Integer = dataReceived - bufferSpaceLeft
                        Buffer.BlockCopy(objBuffer, bufferSpaceLeft, outputBuffer, 0, bytesLeftOver)
                        responseBufferPos = bytesLeftOver
                    Else
                        Buffer.BlockCopy(objBuffer, 0, outputBuffer, responseBufferPos, dataReceived)
                        responseBufferPos += dataReceived
                    End If
                End While

                ' If some data left in the response buffer, send it
                If responseBufferPos > 0 Then
                    Using New TimedLog("StreamingProxy" & vbTab & "Write " & responseBufferPos.ToString(CultureInfo.InvariantCulture) & " to response")
                        context.Response.OutputStream.Write(outputBuffer, 0, responseBufferPos)
                        responseBuffer.Write(outputBuffer, 0, responseBufferPos)
                        totalBytesWritten += responseBufferPos
                    End Using
                End If
            End Using

            Log.WriteLine("StreamingProxy" & vbTab & "Socket read " & Me._PipeStream.TotalWrite.ToString(CultureInfo.InvariantCulture) & " bytes and response written " & totalBytesWritten.ToString(CultureInfo.InvariantCulture) & " bytes")

            _PipeStream.Dispose()

            Return totalBytesWritten

        End Function

        Private Sub ProduceResponseHeader(response As HttpWebResponse, context As HttpContext, cacheDuration As Integer, _
                                          ByRef contentLength As String, ByRef contentEncoding As String, ByRef contentDisposition As String)
            ' produce cache headers for response caching
            If cacheDuration > 0 Then
                ProxyHelper.CacheResponse(context, cacheDuration)
            Else
                ProxyHelper.DoNotCacheResponse(context)
            End If

            ' If content length is not specified, this the response will be sent as Transfer-Encoding: chunked
            contentLength = response.GetResponseHeader(STR_ContentLength)
            If Not String.IsNullOrEmpty(contentLength) Then
                context.Response.AppendHeader(STR_ContentLength, contentLength)
            End If

            ' If downloaded data is compressed, Content-Encoding will have either gzip or deflate
            contentEncoding = response.GetResponseHeader(STR_ContentEncoding)
            If Not String.IsNullOrEmpty(contentEncoding) Then
                context.Response.AppendHeader(STR_ContentEncoding, contentEncoding)
            End If

            contentDisposition = response.GetResponseHeader(STR_ContentDisposition)
            If contentDisposition.IsNullOrEmpty() Then
                Dim fileName As String = response.ResponseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Trim("/"c)
                If Not fileName.IsNullOrEmpty() Then
                    contentDisposition = "filename=" + System.IO.Path.GetFileName(fileName)
                End If

            End If
            If Not String.IsNullOrEmpty(contentDisposition) Then
                context.Response.AppendHeader(STR_ContentDisposition, contentDisposition)
            End If

            context.Response.ContentType = response.ContentType

            Dim charSet As String = response.CharacterSet
            If Not String.IsNullOrEmpty(charSet) Then
                context.Response.ContentEncoding = Encoding.GetEncoding(charSet)
            Else
                context.Response.ContentEncoding = Encoding.Default
            End If


        End Sub

        Private Sub ReadData()
            Dim buffer As Byte() = New Byte(BUFFER_SIZE - 1) {}
            Dim dataReceived As Integer
            Dim totalBytesFromSocket As Integer = 0

            Using New TimedLog("StreamingProxy" & vbTab & "Total Read from socket")
                Try
                    While (dataReceived.InlineAssignHelper(Me._ResponseStream.Read(buffer, 0, BUFFER_SIZE))) > 0
                        Me._PipeStream.Write(buffer, 0, dataReceived)
                        totalBytesFromSocket += dataReceived
                    End While
                Catch x As Exception
                    'Log.WriteLine(x.ToString())
                    Aricie.Services.ExceptionHelper.LogException(x)
                Finally
                    Log.WriteLine("Total bytes read from socket " & totalBytesFromSocket.ToString(CultureInfo.InvariantCulture) & " bytes")
                    Me._ResponseStream.Dispose()
                    Me._PipeStream.Flush()
                End Try
            End Using
        End Sub

        Private Function DoesRequestHaveBody(method As String) As Boolean
            Return (method = "POST" OrElse method = "PUT")
        End Function

        Private Sub CopyRequestStream(inputStream As Stream, outStream As Stream)

            Dim bytesRead As Integer = 0
            Dim buffer As Byte() = New Byte(BUFFER_SIZE - 1) {}
            While (bytesRead.InlineAssignHelper(inputStream.Read(buffer, 0, BUFFER_SIZE))) > 0
                outStream.Write(buffer, 0, bytesRead)
            End While
            outStream.Flush()
            outStream.Close()
        End Sub
        'Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        '    target = value
        '    Return value
        'End Function


        Private Sub InitParams(context As HttpContext, ByRef url As String, ByRef method As String, ByRef cacheDuration As Integer, ByRef contentType As String)
            method = If(context.Request(STR_METHOD), context.Request.HttpMethod)
            url = ProxyHelper.GetOriginalUrl(context.Request)
            'Dim tempProxyUrl As String = ProxyHelper.GetProxyBaseUrl(context.Request)
            cacheDuration = Convert.ToInt32(If(context.Request(STR_CACHE_DURATION), STR_DEFAULT_CACHE_DURATION))
            contentType = If(context.Request(STR_CONTENTTYPE), String.Empty)
        End Sub

#End Region




    End Class
End Namespace

