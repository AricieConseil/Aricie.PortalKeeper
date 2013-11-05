Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports System.IO
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.FileSystem
Imports System.Drawing
Imports System.Globalization
Imports Aricie.Services
Imports System.Reflection

Namespace Services

    Public Module FileHelper

        ''' <summary>
        ''' Upload un fichier
        ''' </summary>
        ''' <param name="Path">Chemin ou le fichier doit être copié</param>
        ''' <param name="objHtmlInputFile">Fichier posté</param>
        ''' <returns>FileId du fichier copié</returns>
        ''' <remarks></remarks>
        Public Function UploadFile(ByVal Path As String, ByVal objHtmlInputFile As HttpPostedFile) As Integer
            If (String.IsNullOrEmpty(Path)) Then
                Path = "Images"
            End If

            Dim PortalSetts As PortalSettings = PortalController.GetCurrentPortalSettings()
            If (Not Directory.Exists(PortalSetts.HomeDirectoryMapPath + Path + "\")) Then
                Directory.CreateDirectory(PortalSetts.HomeDirectoryMapPath + Path + "\")
                FileSystemUtils.AddFolder(PortalSetts, PortalSetts.HomeDirectoryMapPath, Path)
            End If
            Dim strUploadResult As String = FileSystemUtils.UploadFile(PortalSetts.HomeDirectoryMapPath + Path + "\", objHtmlInputFile, False)
            If (strUploadResult <> String.Empty) Then

                Dim EventLogCtrl As New EventLogController()
                Dim LogInf As New LogInfo(strUploadResult)
                LogInf.LogTypeKey = "DEBUG"
                LogInf.LogProperties.Add(New LogDetailInfo("Info:", strUploadResult))
                EventLogCtrl.AddLog(LogInf)
                Return -1
            End If
            Return GetFileId(Path, objHtmlInputFile.FileName)
        End Function

        ''' <summary>
        ''' Retourne le FileId à partir d'un répertoire et d'un nom de fichier
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="FileName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFileId(ByVal Path As String, ByVal FileName As String) As Integer
            Dim FileCtrl As New FileController()
            If (FileName.Contains("\\")) Then
                Dim PosBackSlash As Integer = FileName.LastIndexOf("\")
                FileName = FileName.Substring(PosBackSlash + 1, FileName.Length - PosBackSlash - 1)
            End If
            Return FileCtrl.ConvertFilePathToFileId(Path + "/" + FileName, PortalController.GetCurrentPortalSettings().PortalId)
        End Function

        ''' <summary>
        ''' Retourne l'url à partir d'un FileId
        ''' </summary>
        ''' <param name="FileId"></param>
        ''' <returns>Url correspondant au FileId</returns>
        ''' <remarks></remarks>
        Public Function GetFileUrl(ByVal fileId As Integer, Optional ByVal portalId As Integer = -1) As String
            Dim toReturn As String = String.Empty
            Dim FileCtrl As New FileController()
            If portalId = -1 Then
                portalId = PortalController.GetCurrentPortalSettings().PortalId
            End If
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = FileCtrl.GetFileById(fileId, portalId)
            If (objFile IsNot Nothing) Then
                If objFile.StorageLocation = 0 Then
                    toReturn = DotNetNuke.Common.ApplicationPath & "/"c & NukeHelper.PortalInfo(portalId).HomeDirectory & "/"c & objFile.Folder & objFile.FileName
                Else
                    toReturn = DotNetNuke.Common.Globals.LinkClick("fileid=" & fileId.ToString(CultureInfo.InvariantCulture), -1, -1)
                End If
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Returns path to icon matching a file extension
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFileIconPath(ByVal file As DotNetNuke.Services.FileSystem.FileInfo) As String
            Dim iconPath As String = String.Empty

            Dim type As String = file.Extension
            If type.Length > 1 Then
                type = type.Substring(1, type.Length - 1)
            End If
            Dim imageDirectory As String = "~/images/FileManager/Icons/"
            If type <> "" AndAlso System.IO.File.Exists(HttpContext.Current.Server.MapPath(imageDirectory & type & ".gif")) Then
                iconPath = imageDirectory & type & ".gif"
            Else
                Select Case type
                    Case "jpeg"
                        type = "jpg"
                    Case "docx"
                        type = "doc"
                    Case "pptx"
                        type = "ppt"
                    Case "xlsx"
                        type = "xls"
                    Case Else
                        iconPath = imageDirectory & "File.gif"
                End Select
                If iconPath = "" Then
                    iconPath = imageDirectory & type & ".gif"
                End If
            End If
            Return iconPath
        End Function

        ''' <summary>
        ''' returns mime type according to file extension
        ''' </summary>
        ''' <param name="extension"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContentType(ByVal extension As String) As String
            Dim contentType As String = String.Empty

            Select Case extension.TrimStart("."c).ToLower()
                Case "txt"
                    contentType = "text/plain"
                Case "htm"
                Case "html"
                    contentType = "text/html"
                Case "rtf"
                    contentType = "text/richtext"
                Case "jpg"
                Case "jpeg"
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
                Case "mpg"
                Case "mpeg"
                    contentType = "video/mpeg"
                Case "avi"
                    contentType = "video/avi"
                Case "mp4"
                    contentType = "video/mp4"
                Case "wmv"
                    contentType = "video/x-ms-wmv"
                Case "pdf"
                    contentType = "application/pdf"
                Case "doc"
                Case "dot"
                Case "docx"
                Case "dotx"
                    contentType = "application/msword"
                Case "csv"
                    contentType = "text/csv"
                Case "xls"
                Case "xlt"
                Case "xlsx"
                Case "xltx"
                    contentType = "application/x-msexcel"
                Case Else
                    contentType = "application/octet-stream"
            End Select

            Return contentType
        End Function

        ''' <summary>
        ''' Redimensionne une image en gardant les proportions
        ''' </summary>
        ''' <param name="FileId">FileId de l'image</param>
        ''' <param name="Height">Hauteur de l'image voulue</param>
        ''' <param name="Width">Largeur de l'image voulue</param>
        ''' <returns>Le FileId de l'image</returns>
        ''' <remarks></remarks>
        Public Function ResizeImage(ByVal FileId As Integer, ByVal Height As Integer, ByVal Width As Integer) As Integer
            Dim PortId As Integer = Nothing
            Return ResizeImage(FileId, Height, Width, PortId)
        End Function
        ''' <summary>
        ''' Redimensionne une image en gardant les proportions
        ''' </summary>
        ''' <param name="FileId">FileId de l'image</param>
        ''' <param name="Height">Hauteur de l'image voulue</param>
        ''' <param name="Width">Largeur de l'image voulue</param>
        ''' <param name="PortId">L'Id du portail</param>
        ''' <returns>Le FileId de l'image</returns>
        ''' <remarks></remarks>
        Public Function ResizeImage(ByVal FileId As Integer, ByVal Height As Integer, ByVal Width As Integer, ByVal PortId As Integer) As Integer
            Try
                Dim FileCtrl As FileController = New FileController()
                Dim RealPortalId As Integer = PortId

                Dim FichierImage As New DotNetNuke.Services.FileSystem.FileInfo
                FichierImage = FileCtrl.GetFileById(FileId, RealPortalId)
                If (FichierImage IsNot Nothing AndAlso FichierImage.Width <> Width AndAlso FichierImage.Height <> Height) Then

                    'Récupération de l'image à redimmensionner
                    Dim InPutImage As New Bitmap(FichierImage.PhysicalPath)

                    Dim lWidth As Integer = InPutImage.Width
                    Dim lHeight As Integer = InPutImage.Height
                    Dim zoomH As Double = Width / lWidth
                    Dim zoomW As Double = Height / lHeight
                    Dim zoom As Double = CDbl(IIf(zoomH < zoomW, zoomH, zoomW))
                    lWidth = CInt(lWidth * zoom)
                    lHeight = CInt(lHeight * zoom)

                    'Création de l'image de la taille souhaitée vide
                    Dim OutPutImage As New Bitmap(lWidth, lHeight)
                    'Création du graphique vide
                    Dim g As Graphics = Graphics.FromImage(OutPutImage)

                    'Copie de l'image
                    g.DrawImage(InPutImage, New Rectangle(0, 0, lWidth, lHeight), 0, 0, InPutImage.Width, InPutImage.Height, GraphicsUnit.Pixel)

                    Dim tmpFilePath As String = FichierImage.PhysicalPath.Replace("." + FichierImage.Extension, "tmp." + FichierImage.Extension)
                    For index As Integer = 0 To 10
                        If (File.Exists(tmpFilePath)) Then
                            tmpFilePath = tmpFilePath.Replace("tmp", "tmp" + index.ToString())
                        Else
                            Exit For
                        End If
                    Next

                    'Enregistrement de l'image
                    OutPutImage.Save(tmpFilePath, InPutImage.RawFormat)

                    'Libération des ressources
                    InPutImage.Dispose()
                    OutPutImage.Dispose()
                    g.Dispose()

                    FileCtrl.DeleteFile(RealPortalId, FichierImage.FileName, FichierImage.FolderId, True)
                    File.Delete(FichierImage.PhysicalPath)
                    File.Move(tmpFilePath, FichierImage.PhysicalPath)
                    Dim NewFileContent As Byte() = File.ReadAllBytes(FichierImage.PhysicalPath)
                    Return FileCtrl.AddFile(RealPortalId, FichierImage.FileName, FichierImage.Extension, NewFileContent.Length, lWidth, lHeight, FichierImage.ContentType, FichierImage.Folder, FichierImage.FolderId, True)

                ElseIf (FichierImage IsNot Nothing) Then
                    Return FileId
                Else
                    Return -1
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Function


        ''' <summary>
        ''' Coupe une image en fonction de sa hauteur ou de sa largeur
        ''' </summary>
        ''' <param name="FileId">FileId de l'image</param>
        ''' <param name="Height">Hauteur de l'image voulue</param>
        ''' <param name="Width">Largeur de l'image voulue</param>
        ''' <param name="PortId">L'Id du portail</param>
        ''' <returns>Le FileId de l'image</returns>
        ''' <remarks></remarks>
        Public Function CropImage(ByVal FileId As Integer, ByVal Height As Integer, ByVal Width As Integer, ByVal PortId As Integer) As Integer
            Try
                Dim FileCtrl As FileController = New FileController()
                Dim RealPortalId As Integer = PortId

                ' If PortId = Nothing Then
                '   RealPortalId = PortalController.GetCurrentPortalSettings().PortalId
                ' Else
                ' End If

                Dim FichierImage As New DotNetNuke.Services.FileSystem.FileInfo
                FichierImage = FileCtrl.GetFileById(FileId, RealPortalId)
                If FichierImage IsNot Nothing AndAlso FichierImage.Width > Width Then

                    'Récupération de l'image à redimmensionner
                    Dim InPutImage As New Bitmap(FichierImage.PhysicalPath)

                    Dim lWidth As Integer = InPutImage.Width
                    Dim lHeight As Integer = InPutImage.Height

                    'Création de l'image de la taille souhaitée vide
                    Dim OutPutImage As New Bitmap(Width, Height)
                    'Création du graphique vide
                    Dim g As Graphics = Graphics.FromImage(OutPutImage)

                    'Copie de l'image
                    g.DrawImage(InPutImage, New Rectangle(0, 0, Width, Height), lWidth - Width, 0, Width, Height, GraphicsUnit.Pixel)


                    Dim tmpFilePath As String = FichierImage.PhysicalPath.Replace("." + FichierImage.Extension, "tmp." + FichierImage.Extension)
                    For index As Integer = 0 To 10
                        If (File.Exists(tmpFilePath)) Then
                            tmpFilePath = tmpFilePath.Replace("tmp", "tmp" + index.ToString())
                        Else
                            Exit For
                        End If
                    Next

                    'Enregistrement de l'image
                    OutPutImage.Save(tmpFilePath, InPutImage.RawFormat)

                    'Libération des ressources
                    InPutImage.Dispose()
                    OutPutImage.Dispose()
                    g.Dispose()

                    FileCtrl.DeleteFile(RealPortalId, FichierImage.FileName, FichierImage.FolderId, True)
                    File.Delete(FichierImage.PhysicalPath)
                    File.Move(tmpFilePath, FichierImage.PhysicalPath)
                    Dim NewFileContent As Byte() = File.ReadAllBytes(FichierImage.PhysicalPath)
                    Return FileCtrl.AddFile(RealPortalId, FichierImage.FileName, FichierImage.Extension, NewFileContent.Length, lWidth, lHeight, FichierImage.ContentType, FichierImage.Folder, FichierImage.FolderId, True)

                ElseIf (FichierImage IsNot Nothing) Then
                    Return FileId
                Else
                    Return -1
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        ''' <summary>
        ''' Checks if extension matches any of the common images extension
        ''' </summary>
        ''' <param name="fi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsImageByExtension(ByVal fi As DotNetNuke.Services.FileSystem.FileInfo) As Boolean

            Dim ext As String
            Dim list As New List(Of String)
            list.Add("jpg")
            list.Add("gif")
            list.Add("png")
            list.Add("bmp")

            If fi.Extension.StartsWith("."c) Then
                ext = fi.Extension.TrimStart("."c)
            Else
                ext = fi.Extension
            End If

            For Each extension As String In list
                If ext = extension Then Return True
            Next

            Return False
        End Function

        ''' <summary>
        ''' returns the mapped path of a relative path
        ''' </summary>
        ''' <param name="relativePath"></param>
        ''' <param name="hostRoot"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAbsoluteMapPath(ByVal relativePath As String, ByVal hostRoot As Boolean) As String
            Dim root As String = DotNetNuke.Common.Globals.HostMapPath
            If Not hostRoot Then
                root = NukeHelper.PortalSettings.HomeDirectoryMapPath
            End If
            If String.IsNullOrEmpty(root) Then
                root = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath.TrimEnd("\"c) & "\Portals\_Default\"
            End If
            Return String.Format("{0}\{1}", root.TrimEnd("\"c), relativePath.Replace("/"c, "\"c).TrimStart("\"c))
        End Function

        ''' <summary>
        ''' Delete a file from current portal
        ''' </summary>
        ''' <param name="fileId">fileId to delete</param>
        ''' <returns>True if the file is delete, false if there is any error</returns>
        ''' <remarks></remarks>
        Public Function DeleteFile(ByVal fileId As Integer) As Boolean
            Dim toReturn As Boolean = True
            Dim fc As New DotNetNuke.Services.FileSystem.FileController
            Try
                Dim fi As DotNetNuke.Services.FileSystem.FileInfo = fc.GetFileById(fileId, PortalId)
                DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFile(fi.PhysicalPath, PortalSettings, True)
            Catch ex As Exception
                toReturn = False
            End Try

            Return toReturn
        End Function


        'Public Sub DownloadFile(path As String)

        '    If NukeHelper.DnnVersion.Major < 6 Then
        '        DotNetNuke.Common.Utilities.FileSystemUtils.DownloadFile(path)
        '    Else

        '    End If
        'End Sub


        'Private _FileManagerDNN6 As Object
        'Private ReadOnly Property FileManagerDNN6 As Object
        '    Get
        '        If _FileManagerDNN6 Is Nothing Then
        '            Dim fileManagerType As Type = ReflectionHelper.CreateType("DotNetNuke.Services.FileSystem.FileManager, DotNetNuke")
        '            _FileManagerDNN6 = fileManagerType.GetProperty("Instance").GetValue(Nothing, Nothing)
        '        End If
        '        Return _FileManagerDNN6
        '    End Get
        'End Property

        'Private _FileManagerDNN6WriteFileMethod As MethodInfo





    End Module

End Namespace
