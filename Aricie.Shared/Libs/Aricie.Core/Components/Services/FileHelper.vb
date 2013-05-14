Imports System.IO
Imports System.Text
Imports System.Web

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
        ''' <param name="filePath"></param>
        ''' <param name="response"></param>
        ''' <remarks></remarks>
        Public Sub DownloadFile(ByVal filePath As String, ByRef response As HttpResponse)
            Dim myFile As New FileInfo(filePath)
            If myFile IsNot Nothing Then
                response.Clear()
                response.ClearHeaders()
                response.Charset = String.Empty
                response.CacheControl = "Private"
                response.AppendHeader("Expires", "0")
                response.AppendHeader("Pragma", "cache")
                response.AppendHeader("Cache-Control", "private")
                response.AppendHeader("Content-Disposition", "attachment; filename=" + myFile.Name.TrimEnd.Replace(" ", "_"))
                response.ContentType = GetMimeTypeFromExtention(myFile.Extension)

                response.WriteFile(myFile.FullName)

                response.End()
            End If
        End Sub

        ''' <summary>
        ''' Propose to download file to the user 
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <param name="response"></param>
        ''' <param name="server"></param>
        ''' <remarks></remarks>
        Public Sub DownloadFile(ByVal filePath As String, ByVal response As HttpResponse, ByVal server As HttpServerUtility)
            Dim myFile As New FileInfo(filePath)

            Dim myFileStream As New System.IO.FileStream(myFile.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)
            Try
                If myFileStream.CanRead Then
                    response.Clear()
                    response.ContentType = GetMimeTypeFromExtention(myFile.Extension)

                    response.AddHeader("Content-Disposition", "attachment; filename=" + myFile.Name)

                    ' Set the buffer that will receive the data from the file
                    Dim myBuffer As Byte() = New Byte(4096) {}

                    ' Setup the Time out in seconds according to file size
                    server.ScriptTimeout = 3600

                    While True
                        ' Check if the client is still connected
                        If Not response.IsClientConnected Then
                            ' TODO: might not be correct. Was : Exit While
                            Exit While
                        End If

                        ' read data from the file
                        Dim myNumberOfBytesRead As Integer = myFileStream.Read(myBuffer, 0, 4096)
                        If myNumberOfBytesRead = 0 Then
                            ' TODO: might not be correct. Was : Exit While
                            Exit While
                        End If

                        ' Write the data to the current output stream.
                        response.OutputStream.Write(myBuffer, 0, myNumberOfBytesRead)

                        ' Flush the data to the HTML output.

                        response.Flush()
                    End While

                End If
            Catch ex As Exception
                Throw ex
            Finally
                myFileStream.Close()
                myFileStream.Dispose()
                response.End()
            End Try
        End Sub


        Public Function GetMimeTypeFromExtention(ByVal extension As String) As String

            extension = extension.Trim("."c)

            Dim contentType As String = "application/octet-stream"

            Select Case extension
                Case "pdf"
                    contentType = "application/pdf"
                Case "htm", "html"
                    contentType = "text/html"
                Case "txt"
                    contentType = "text/plain"
                Case Else
                    contentType = "application/octet-stream"
            End Select

            Return contentType

        End Function

    End Module

End Namespace

