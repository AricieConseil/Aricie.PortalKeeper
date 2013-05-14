Imports System
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Web
Imports System.Web.Caching
Imports Aricie.IO
Imports System.Web.SessionState

Namespace Web.Proxy
    Public Class StreamingProxy
        Implements IHttpAsyncHandler, IHttpHandler
        Implements IRequiresSessionState


        ' Fields
        Private _PipeStream As PipeStream
        Private _ResponseStream As Stream

        Private url As String
        Private cacheDuration As Integer
        Private contentType As String

        Private Const BUFFER_SIZE As Integer = 8192


#Region "Implementations"

        ' Properties
        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property



        Public Sub ProcessRequest(ByVal context As HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            Me.PreProcessRequest(context)

            If ((cacheDuration > 0) AndAlso (Not context.Cache.Item(url) Is Nothing)) Then
                Dim content As CachedContent = TryCast(context.Cache.Item(url), CachedContent)
                Me.ProcessCacheContent(context.Response, content)

            End If
            Dim request As HttpWebRequest = BuildHttpRequest(context)

            Using response As HttpWebResponse = TryCast(request.GetResponse, HttpWebResponse)
                Me.DownloadData(request, response, context, cacheDuration)
            End Using
        End Sub




        ' Methods
        Public Function BeginProcessRequest(ByVal context As HttpContext, ByVal cb As AsyncCallback, ByVal extraData As Object) As IAsyncResult Implements System.Web.IHttpAsyncHandler.BeginProcessRequest
            Me.PreProcessRequest(context)

            If ((cacheDuration > 0) AndAlso (Not context.Cache.Item(url) Is Nothing)) Then
                Dim toReturn As New SyncResult()
                toReturn.Context = context
                toReturn.Content = TryCast(context.Cache.Item(url), CachedContent)
            End If
            Dim request As HttpWebRequest = Me.BuildHttpRequest(context)
            Dim state As New AsyncState()
            state.Context = context
            state.Url = url
            state.CacheDuration = cacheDuration
            state.Request = request
            Return request.BeginGetResponse(cb, state)
        End Function



        Public Sub EndProcessRequest(ByVal result As IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
            Dim syncResult As SyncResult = TryCast(result, SyncResult)
            If (result.CompletedSynchronously AndAlso (Not syncResult Is Nothing)) Then
                Me.ProcessCacheContent(syncResult.Context.Response, syncResult.Content)
            Else
                Dim state As AsyncState = TryCast(result.AsyncState, AsyncState)
                state.Context.Response.Buffer = False
                Dim request As HttpWebRequest = state.Request
                Using response As HttpWebResponse = TryCast(request.EndGetResponse(result), HttpWebResponse)
                    Me.DownloadData(request, response, state.Context, state.CacheDuration)
                End Using
            End If
        End Sub




#End Region

#Region "virtual methods"

        Protected Overridable Function ProcessUrl(ByVal url As String) As String
            Return url
        End Function

#End Region

#Region "Private methods"

        Private Sub DownloadData(ByVal request As HttpWebRequest, ByVal response As HttpWebResponse, ByVal context As HttpContext, ByVal cacheDuration As Integer)
            Dim responseBuffer As New MemoryStream
            context.Response.Buffer = False
            Try
                If (response.StatusCode <> HttpStatusCode.OK) Then
                    context.Response.StatusCode = CInt(response.StatusCode)
                Else
                    Using readStream As Stream = response.GetResponseStream
                        If context.Response.IsClientConnected Then
                            Dim contentLength As String = String.Empty
                            Dim contentEncoding As String = String.Empty
                            Me.ProduceResponseHeader(response, context, cacheDuration, contentLength, contentEncoding)
                            Dim totalBytesWritten As Integer = Me.TransmitDataAsyncOptimized(context, readStream, responseBuffer)
                            If (cacheDuration > 0) Then
                                Dim objCache As New CachedContent()
                                objCache.Content = responseBuffer
                                objCache.ContentEncoding = contentEncoding
                                objCache.ContentLength = contentLength
                                objCache.ContentType = response.ContentType
                                context.Cache.Insert(request.RequestUri.ToString, objCache, Nothing, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(CDbl(cacheDuration)), CacheItemPriority.Normal, Nothing)
                            End If
                        End If
                        context.Response.Flush()
                    End Using
                End If
            Catch x As Exception
                request.Abort()
            End Try
        End Sub



        Private Sub ProduceResponseHeader(ByVal response As HttpWebResponse, ByVal context As HttpContext, ByVal cacheDuration As Integer, <Out()> ByRef contentLength As String, <Out()> ByRef contentEncoding As String)
            If (cacheDuration > 0) Then
                HttpHelper.CacheResponse(context, cacheDuration)
            Else
                HttpHelper.DoNotCacheResponse(context)
            End If
            contentLength = response.GetResponseHeader("Content-Length")
            If Not String.IsNullOrEmpty(contentLength) Then
                context.Response.AppendHeader("Content-Length", contentLength)
            End If
            contentEncoding = response.GetResponseHeader("Content-Encoding")
            If Not String.IsNullOrEmpty(contentEncoding) Then
                context.Response.AppendHeader("Content-Encoding", contentEncoding)
            End If
            context.Response.ContentType = response.ContentType
        End Sub

        Private Sub ReadData()
            Dim buffer As Byte() = New Byte(&H2000 - 1) {}
            Dim totalBytesFromSocket As Integer = 0
            Try
                Try
                    Dim dataReceived As Integer = Me._ResponseStream.Read(buffer, 0, &H2000)
                    Do While (dataReceived > 0)
                        dataReceived = Me._ResponseStream.Read(buffer, 0, &H2000)
                        Me._PipeStream.Write(buffer, 0, dataReceived)
                        totalBytesFromSocket = (totalBytesFromSocket + dataReceived)
                    Loop
                Catch x As Exception
                End Try
            Finally
                Me._ResponseStream.Dispose()
                Me._PipeStream.Flush()
            End Try
        End Sub

        Private Function TransmitDataAsync(ByVal context As HttpContext, ByVal readStream As Stream, ByVal responseBuffer As MemoryStream) As Integer
            Dim dataReceived As Integer
            Me._ResponseStream = readStream
            Me._PipeStream = New PipeStreamBlock(&H1388)
            Dim buffer As Byte() = New Byte(&H2000 - 1) {}
            Dim objThread As New Thread(New ThreadStart(AddressOf Me.ReadData))
            objThread.Start()
            Dim totalBytesWritten As Integer = 0
            dataReceived = Me._PipeStream.Read(buffer, 0, &H2000)
            Do While (dataReceived > 0)
                dataReceived = Me._PipeStream.Read(buffer, 0, &H2000)
                context.Response.OutputStream.Write(buffer, 0, dataReceived)
                responseBuffer.Write(buffer, 0, dataReceived)
                totalBytesWritten = (totalBytesWritten + dataReceived)
            Loop
            Me._PipeStream.Dispose()
            Return totalBytesWritten
        End Function

        Private Function TransmitDataAsyncOptimized(ByVal context As HttpContext, ByVal readStream As Stream, ByVal responseBuffer As MemoryStream) As Integer
            Dim dataReceived As Integer
            Me._ResponseStream = readStream
            Me._PipeStream = New PipeStreamBlock(&H2710)
            Dim objBuffer As Byte() = New Byte(&H2000 - 1) {}
            Dim objThread As New Thread(New ThreadStart(AddressOf Me.ReadData))
            objThread.Start()
            Dim totalBytesWritten As Integer = 0
            Dim outputBuffer As Byte() = New Byte(&H2000 - 1) {}
            Dim responseBufferPos As Integer = 0
            dataReceived = Me._PipeStream.Read(objBuffer, 0, &H2000)
            Do While (dataReceived > 0)
                dataReceived = Me._PipeStream.Read(objBuffer, 0, &H2000)
                Dim bufferSpaceLeft As Integer = (&H2000 - responseBufferPos)
                If (bufferSpaceLeft < dataReceived) Then
                    Buffer.BlockCopy(objBuffer, 0, outputBuffer, responseBufferPos, bufferSpaceLeft)
                    context.Response.OutputStream.Write(outputBuffer, 0, &H2000)
                    responseBuffer.Write(outputBuffer, 0, &H2000)
                    totalBytesWritten = (totalBytesWritten + &H2000)
                    responseBufferPos = 0
                    Dim bytesLeftOver As Integer = (dataReceived - bufferSpaceLeft)
                    Buffer.BlockCopy(objBuffer, bufferSpaceLeft, outputBuffer, 0, bytesLeftOver)
                    responseBufferPos = bytesLeftOver
                Else
                    Buffer.BlockCopy(objBuffer, 0, outputBuffer, responseBufferPos, dataReceived)
                    responseBufferPos = (responseBufferPos + dataReceived)
                End If
            Loop
            If (responseBufferPos > 0) Then
                context.Response.OutputStream.Write(outputBuffer, 0, responseBufferPos)
                responseBuffer.Write(outputBuffer, 0, responseBufferPos)
                totalBytesWritten = (totalBytesWritten + responseBufferPos)
            End If
            Me._PipeStream.Dispose()
            Return totalBytesWritten
        End Function

        Private Function TransmitDataInChunks(ByVal context As HttpContext, ByVal readStream As Stream, ByVal responseBuffer As MemoryStream) As Integer
            Dim bytesRead As Integer
            Dim buffer As Byte() = New Byte(&H2000 - 1) {}
            Dim totalBytesWritten As Integer = 0
            bytesRead = readStream.Read(buffer, 0, &H2000)
            Do While (bytesRead > 0)
                bytesRead = readStream.Read(buffer, 0, &H2000)
                context.Response.OutputStream.Write(buffer, 0, bytesRead)
                responseBuffer.Write(buffer, 0, bytesRead)
                totalBytesWritten = (totalBytesWritten + bytesRead)
            Loop
            Return totalBytesWritten
        End Function


        Private Sub ProcessCacheContent(ByRef objResponse As HttpResponse, ByVal objCachedContent As CachedContent)

            If Not String.IsNullOrEmpty(objCachedContent.ContentEncoding) Then
                objResponse.AppendHeader("Content-Encoding", objCachedContent.ContentEncoding)
            End If
            If Not String.IsNullOrEmpty(objCachedContent.ContentLength) Then
                objResponse.AppendHeader("Content-Length", objCachedContent.ContentLength)
            End If
            objResponse.ContentType = objCachedContent.ContentType
            objCachedContent.Content.Seek(0, SeekOrigin.Begin)
            objCachedContent.Content.WriteTo(objResponse.OutputStream)

        End Sub

        Private Sub PreProcessRequest(ByVal context As HttpContext)
            Me.url = context.Request.Item("proxy-distant-url")
            Me.cacheDuration = Convert.ToInt32(IIf(context.Request.Item("cache") <> Nothing, context.Request.Item("cache"), "0"))
            Me.contentType = context.Request.Item("type")
            Dim item As String
            For Each item In context.Request.QueryString.Keys
                If (item <> "proxy-distant-url") Then
                    If url.Contains("?") Then
                        url = (url & String.Format("&{0}={1}", item, context.Request.QueryString.Item(item)))
                    Else
                        url = (url & String.Format("?{0}={1}", item, context.Request.QueryString.Item(item)))
                    End If
                End If
            Next
            url = Me.ProcessUrl(url)
        End Sub

        Private Function BuildHttpRequest(ByVal context As HttpContext) As HttpWebRequest
            Dim request As HttpWebRequest = HttpHelper.CreateScalableHttpWebRequest(url)
            request.AutomaticDecompression = DecompressionMethods.None
            request.Method = context.Request.RequestType
            Dim cookieContainer As New CookieContainer
            request.CookieContainer = cookieContainer
            If (request.Method = "POST") Then
                Dim encodedData As New UTF8Encoding
                Dim post As String = context.Request.Form.ToString
                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = post.Length
                Dim stOut As New StreamWriter(request.GetRequestStream, Encoding.ASCII)
                stOut.Write(post)
                stOut.Close()
            End If
            If Not String.IsNullOrEmpty(contentType) Then
                request.ContentType = contentType
            End If
            Return request
        End Function

#End Region









    End Class
End Namespace

