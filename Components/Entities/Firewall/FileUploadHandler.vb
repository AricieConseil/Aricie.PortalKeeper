Imports Aricie.DNN.Entities
Imports System.IO
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.FileSystem
Imports OpenRasta.IO
Imports OpenRasta.Web
Imports OpenRasta.Security
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.Services
Imports DotNetNuke.Security.Permissions

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class FileUploadHandler

        ' will be injected by the IoC built in to openrasta
        Public Property Context As ICommunicationContext


        Public Function [Get]() As OperationResult
            Return New OperationResult.OK()
        End Function

        'Public Function [Get](fileId As Integer) As OperationResult
        '    Dim dnnFile As DotNetNuke.Services.FileSystem.FileInfo = Aricie.DNN.Services.NukeHelper.FileController.GetFileById(fileId, -1)
        '    If dnnFile IsNot Nothing Then
        '        Dim objFile As New FileUpload(dnnFile.FileName, DotNetNuke.Common.Utilities.FileSystemUtils.GetFileContent(dnnFile))
        '        Return New OperationResult.OK(objFile)
        '    End If
        '    Return New OperationResult.NotFound()
        'End Function



        'Public Function Post(entities As IEnumerable(Of IMultipartHttpEntity)) As OperationResult
        '    Dim toReturn As OperationResult
        '    For Each entity As IMultipartHttpEntity In entities
        '        If Not entity.Headers.ContentDisposition.Disposition.ToUpperInvariant = "form-Data".ToUpperInvariant Then
        '            toReturn = New OperationResult.BadRequest()
        '            toReturn.ResponseResource = "Sent a field that is not declared as form-data, cannot process"
        '            Return toReturn
        '        End If
        '        toReturn = New OperationResult.SeeOther()
        '        Dim fileId As Integer = ReceiveStream(entity.ContentType, entity.Stream)
        '        toReturn.RedirectLocation= GetType(FileUpload).CreateUri(new { id = ReceiveStream(entity.ContentType, entity.Stream) })
        '        Return toReturn


        '    Next
        'End Function

        '<RequiresAuthentication()> _
        'Public Function Post(file As IFile) As OperationResult
        '    Me.Post(file.FileName, file.OpenStream.ReadToEnd())
        '    Return New OperationResult.Created
        'End Function

        '<RequiresAuthentication()> _
        Public Function Post(objFile As FileUploadInfo) As OperationResult

            Return Post(objFile.Login.UserName, objFile.Login.Password, objFile.FileName, objFile.FileContent)
        End Function

        '<RequiresAuthentication()> _
        Private Function Post(userName As String, password As String, filePath As String, fileContent As Byte()) As OperationResult

            'If Context.User.Identity.Name IsNot Nothing Then
            Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_SUCCESS

            Dim objUser As UserInfo = UserController.UserLogin(PortalId, userName, password, "", "", "", loginStatus, True)
            If objUser IsNot Nothing Then
                HttpContext.Current.Items("UserInfo") = objUser

                Dim folderPath = FileSystemUtils.FormatFolderPath(Path.GetDirectoryName(filePath).Replace("\"c, "/"c))
                Dim fileMapPath As String = Path.Combine(DnnContext.Current.Portal.HomeDirectoryMapPath, filePath)
                'Dim folderPath As String = Path.GetDirectoryName(objFile.FileName)

                Dim objFolder As FolderInfo = NukeHelper.FolderController.GetFolder(PortalId, folderPath, True)
                If objFolder Is Nothing Then
                    Dim folderDir As DirectoryInfo = Directory.GetParent(fileMapPath)
                    Dim parentDir As DirectoryInfo = folderDir.Parent
                    If parentDir.Exists Then
                        Dim parentPath As String = folderPath.TrimEnd("/"c)
                        parentPath = parentPath.Substring(0, parentPath.LastIndexOf("/"c) + 1)
                        Dim parentFolder As FolderInfo = NukeHelper.FolderController.GetFolder(PortalId, parentPath, True)
                        If Not parentFolder Is Nothing Then
                            If Not folderDir.Exists Then
                                folderDir.Create()
                            End If
                            objFolder = New FolderInfo()
                            objFolder.FolderPath = folderPath
                            objFolder.StorageLocation = 0
                            Dim folderId As Integer = NukeHelper.FolderController.AddFolder(PortalId, folderPath)
                            objFolder = NukeHelper.FolderController.GetFolderInfo(PortalId, folderId)

                            Dim permc As New PermissionController
                            Dim perm As PermissionInfo = DirectCast(permc.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "WRITE")(0), PermissionInfo)
                            Dim fp As New FolderPermissionInfo()
                            fp.UserID = objUser.UserID
                            'fp.Username = objUser.Username
                            fp.AllowAccess = True
                            fp.PermissionID = perm.PermissionID
                            fp.FolderID = objFolder.FolderID
                            Dim fpc As New FolderPermissionController
                            fpc.AddFolderPermission(fp)
                            DataCache.ClearFolderPermissionsCache(PortalId)
                            'If FolderPermissionController.HasFolderPermission(NukeHelper.PortalId, parentPath, "WRITE") Then



                            'End If
                        End If
                    End If
                End If
                If FolderPermissionController.HasFolderPermission(PortalId, folderPath, "WRITE") Then
                    'FileSystemUtils.SaveFile(fileMapPath, fileContent)
                    'FileSystemUtils.FormatFolderPath(fileMapPath)

                    FileSystemUtils.SaveFile(fileMapPath, fileContent)



                    Return New OperationResult.Created
                End If
            Else
                Return New OperationResult.Forbidden


            End If
            Return New OperationResult.Forbidden
        End Function

    End Class
End Namespace