Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Globalization

Namespace Services

    ''' <summary>
    ''' Helper class for file operations
    ''' </summary>
    Public Module FileHelper
        Public Sub CopyDirectory(ByVal src As String, ByVal dst As String)

            Dim files As String()
            If dst(dst.Length - 1) <> Path.DirectorySeparatorChar Then
                dst = dst + Path.DirectorySeparatorChar
            End If

            If Not Directory.Exists(dst) Then
                Directory.CreateDirectory(dst)
            End If
            files = Directory.GetFileSystemEntries(src)
            For Each element As String In files
                If (Directory.Exists(element)) Then
                    CopyDirectory(element, dst + Path.GetFileName(element))
                Else
                    File.Copy(element, dst + Path.GetFileName(element), True)
                End If
            Next

        End Sub


        ''' <summary>
        ''' Read Text File using file Encoding
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadFileText(ByVal filePath As String) As String
            Dim toReturn As String = String.Empty
            If File.Exists(filePath) Then
                Dim streamRead As New StreamReader(filePath, GetFileEncoding(filePath))
                toReturn = streamRead.ReadToEnd
                streamRead.Close()
            End If

            Return toReturn
        End Function


        ''' <summary>
        ''' Return File Encoding (UTF8, ANSI, etc)
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFileEncoding(ByVal filePath As String) As Encoding
            Dim toReturn As Encoding = Encoding.Default

            Dim file As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
            If file.CanSeek Then
                Dim bom As Byte() = New Byte(5) {}
                file.Read(bom, 0, 4)
                If bom(0) = 239 AndAlso bom(1) = 187 AndAlso bom(2) = 191 Then
                    toReturn = Encoding.UTF8
                ElseIf bom(0) = 254 AndAlso bom(1) = 255 Then
                    toReturn = Encoding.Unicode
                ElseIf bom(0) = 0 AndAlso bom(1) = 0 AndAlso bom(2) = 254 AndAlso bom(3) = 255 Then
                    toReturn = Encoding.UTF32
                ElseIf bom(0) = 43 AndAlso bom(1) = 47 AndAlso bom(2) = 118 Then
                    toReturn = Encoding.UTF7
                End If
            End If
            file.Close()
            Return toReturn
        End Function


        ''' <summary>
        ''' Propose to download file to the user 
        ''' </summary>
        ''' <param name="filePath">absolute path of file to download</param>
        ''' <param name="response">http response to write the file to</param>
        ''' <remarks></remarks>
        <Obsolete("user the overload with httpserverutility")> _
        Public Sub DownloadFile(ByVal filePath As String, ByRef response As HttpResponse)
            DownloadFile(filePath, response, Nothing)
        End Sub



        ''' <summary>
        ''' Propose to download file to the user 
        ''' </summary>
        ''' <param name="filePath">absolute path of file to download</param>
        ''' <param name="response">http response to write the file to</param>
        ''' <param name="server">http server to control the timeout if needed</param>
        ''' <remarks></remarks>
        Public Sub DownloadFile(ByVal filePath As String, ByRef response As HttpResponse, ByVal server As HttpServerUtility)
            Dim myFile As New FileInfo(filePath)
            If myFile.Exists Then
                response.ClearContent()
                response.ClearHeaders()
                response.Charset = String.Empty
                response.CacheControl = "Private"
                response.AppendHeader("Expires", "0")
                response.AppendHeader("Pragma", "cache")
                response.AppendHeader("Cache-Control", "private")
                response.AppendHeader("Content-Disposition", "attachment; filename=" + myFile.Name.TrimEnd().Replace(" ", "_"))
                response.AppendHeader("Content-Length", myFile.Length.ToString(CultureInfo.InvariantCulture))
                response.ContentType = GetMimeTypeFromExtention(myFile.Extension)

                'response.WriteFile(myFile.FullName)
                WriteFile(filePath, response, server)
                response.Flush()
                response.End()
            End If
        End Sub






        Public Sub WriteFile(filePath As String, objResponse As HttpResponse, ByVal objServer As HttpServerUtility)


            Using myFileStream As New System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)
                If myFileStream.CanRead Then
                    Dim minBytesPerSec As Integer = 5000
                    Dim bigTimeOut As Integer = 3600
                    Dim bufferSize As Integer = 8192
                    Dim myBuffer As Byte() = New Byte(bufferSize) {}
                    Dim lngDataToRead = myFileStream.Length
                    ' Setup the Time out in seconds according to file size
                    Dim tempScriptTimeout As Integer = -1
                    If objServer IsNot Nothing AndAlso objServer.ScriptTimeout < bigTimeOut AndAlso objServer.ScriptTimeout < lngDataToRead / minBytesPerSec Then
                        tempScriptTimeout = objServer.ScriptTimeout
                        objServer.ScriptTimeout = bigTimeOut
                    End If

                    Try

                        While lngDataToRead > 0
                            ' Check if the client is still connected
                            If Not objResponse.IsClientConnected Then
                                lngDataToRead = -1
                            Else
                                ' read data from the file
                                Dim myNumberOfBytesRead As Integer = myFileStream.Read(myBuffer, 0, bufferSize)
                                If myNumberOfBytesRead > 0 Then
                                    ' Write the data to the current output stream.
                                    objResponse.OutputStream.Write(myBuffer, 0, myNumberOfBytesRead)

                                    ' Flush the data to the HTML output.

                                    objResponse.Flush()
                                    lngDataToRead -= myNumberOfBytesRead
                                End If
                            End If
                        End While
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                        objResponse.Write(String.Concat("Error : ", ex.Message))
                    Finally
                        If tempScriptTimeout > 0 Then
                            objServer.ScriptTimeout = tempScriptTimeout
                        End If
                    End Try
                End If
            End Using
        End Sub


        Public Function GetMimeTypeFromExtention(ByVal extension As String) As String

            extension = extension.ToLowerInvariant().Trim("."c)

            Dim contentType As String
            Select Case extension
                Case "pdf"
                    contentType = "application/pdf"
                Case "htm", "html"
                    contentType = "text/html"
                Case "txt"
                    contentType = "text/plain"
                Case "rtf"
                    contentType = "text/richtext"
                Case "jpg", "jpeg"
                    contentType = "image/jpeg"
                Case "gif"
                    contentType = "image/gif"
                Case "bmp"
                    contentType = "image/bmp"
                Case "png"
                    contentType = "image/png"
                Case "ico"
                    contentType = "image/x-icon"
                Case "mp3"
                    contentType = "audio/mpeg"
                Case "wma"
                    contentType = "audio/x-ms-wma"
                Case "mpg", "mpeg"
                    contentType = "video/mpeg"
                Case "avi"
                    contentType = "video/avi"
                Case "mp4"
                    contentType = "video/mp4"
                Case "wmv"
                    contentType = "video/x-ms-wmv"
                Case "doc", "dot"
                    contentType = "application/msword"
                Case "docx", "dotx"
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                Case "csv"
                    contentType = "text/csv"
                Case "xls", "xlt"
                    contentType = "application/x-msexcel"
                Case "xlsx", "xltx"
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template"
                Case "ppt", "pps"
                    contentType = "application/vnd.ms-powerpoint"
                Case "pptx", "ppsx"
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"
                Case Else
                    contentType = "application/octet-stream"
            End Select

            Return contentType

        End Function

    End Module

End Namespace

