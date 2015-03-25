Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services
Imports DotNetNuke.Common
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Globalization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities

Namespace UI.WebControls.EditControls

    'todo: most of this is redundant with Aricie.DNN.Web.UI.UploadControl, which should be relied upon
    Public Class UploadEditControl
        Inherits EditControl

        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()

            Dim attribute As Attribute
            For Each attribute In MyBase.CustomAttributes
                If (attribute.GetType Is GetType(FileExtensionsAttribute)) Then
                    Dim attribute2 As FileExtensionsAttribute = DirectCast(attribute, FileExtensionsAttribute)
                    Me.extensions = attribute2.Extensions
                ElseIf (attribute.GetType Is GetType(PathAttribute)) Then
                    Dim attribute3 As PathAttribute = DirectCast(attribute, PathAttribute)
                    Me.path = attribute3.Path
                ElseIf (attribute.GetType Is GetType(AutoEraseAttribute)) Then
                    Me.autoErase = True
                ElseIf (attribute.GetType Is GetType(SizeAttribute)) Then
                    Dim attribute4 As SizeAttribute = DirectCast(attribute, SizeAttribute)
                    Me.Size = attribute4.Size
                End If
            Next

        End Sub


        ' Methods
        Protected Overrides Sub CreateChildControls()
            Try
                Me._downloadLink = New LinkButton
                Me._downloadLink.CssClass = "Normal"
                Me._downloadLink.CausesValidation = False
                Me.Controls.Add(Me._downloadLink)
                Me._fileImage = New Image
                Me._fileImage.Visible = False
                Me.Controls.Add(Me._fileImage)
                Dim fileId As Integer
                If (Not MyBase.Value Is Nothing AndAlso Not String.IsNullOrEmpty(MyBase.Value.ToString)) AndAlso Integer.TryParse(MyBase.Value.ToString, fileId) Then
                    Dim controller As New FileController
                    Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                    Dim fileById As FileInfo = controller.GetFileById(fileId, currentPortalSettings.PortalId)
                    If (Not fileById Is Nothing) Then
                        Me._downloadLink.Text = fileById.FileName
                        Me.ShowImage(MyBase.Value.ToString)
                    End If
                End If
                If (MyBase.EditMode = PropertyEditorMode.Edit) Then
                    Me._fileUpload = New FileUpload
                    Me._ckOverride = New CheckBox
                    Me._uploadLink = New ImageButton
                    Me._errorLabel = New Label
                    Me._InfoLabel = New Label
                    Me._fileUpload.ID = (Me.ID & "upload")
                    Me._ckOverride.Text = "Ecraser le fichier éxistant"
                    Me._ckOverride.CssClass = "Normal"
                    Me._uploadLink.ID = Me.ID & "uploadLink"
                    Me._uploadLink.ImageUrl = "~/images/up.gif"
                    Me._errorLabel.Text = String.Empty
                    Me._errorLabel.CssClass = "NormalRed"
                    Me._InfoLabel.CssClass = "Normal"
                    If (Me.extensions <> String.Empty) Then
                        Me._InfoLabel.Text = (Me._InfoLabel.Text & "<br>Fichiers autorisés : " & Me.extensions & ". ")
                    End If
                    If (Me.Size <> -1) Then
                        Me._InfoLabel.Text = _
                            (Me._InfoLabel.Text & "<br>Taille maximum autorisée : " & _
                             Me.Size.ToString(CultureInfo.InvariantCulture) & "ko. ")
                    End If
                    Me.Controls.Add(Me._fileUpload)
                    If Not autoErase Then
                        Me.Controls.Add(Me._ckOverride)
                    End If
                    Me.Controls.Add(Me._uploadLink)
                    Me.Controls.Add(Me._InfoLabel)
                    Me.Controls.Add(Me._errorLabel)
                End If
            Finally
                Me.ChildControlsCreated = True
            End Try
        End Sub

        Private Sub downloadLink_Click(ByVal sender As Object, ByVal e As EventArgs)
            If (Not MyBase.Value Is Nothing AndAlso Not String.IsNullOrEmpty(MyBase.Value.ToString)) Then
                Dim fileId As Integer = Integer.Parse(MyBase.Value.ToString(), CultureInfo.InvariantCulture)
                FileSystemUtils.DownloadFile(PortalController.GetCurrentPortalSettings.PortalId, fileId, True, True)
            End If
        End Sub

        Protected Shared Function GetFileList(ByVal path As String) As List(Of FileItem)
            Dim toReturn As New List(Of FileItem)
            For Each item As FileItem In _
                Globals.GetFileList(PortalController.GetCurrentPortalSettings.PortalId, String.Empty, False, path, _
                                     False)
                toReturn.Add(item)
            Next
            Return toReturn
        End Function

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) _
            As Boolean
            Return False
        End Function

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(MyBase.Name)
            args.Value = MyBase.Value
            args.OldValue = MyBase.OldValue
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
            Dim _
                validator As RequiredFieldValidator = _
                    DirectCast(MyBase.Parent.Parent.FindControl((MyBase.Name & "_Req")), RequiredFieldValidator)
            If (Not validator Is Nothing) Then
                validator.Enabled = False
            End If
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            Me.EnsureChildControls()

            If (Not Me._downloadLink Is Nothing) Then
                AddHandler Me._downloadLink.Click, New EventHandler(AddressOf Me.downloadLink_Click)
            End If
            If (Not Me._uploadLink Is Nothing) Then
                AddHandler Me._uploadLink.Click, New ImageClickEventHandler(AddressOf Me.uploadLink_Click)
            End If
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)
            If ((Not Me.Page Is Nothing) AndAlso (MyBase.EditMode = PropertyEditorMode.Edit)) Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If

        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            MyBase.RenderChildren(writer)
        End Sub

        Protected Sub ShowImage(ByVal valueFileId As String)
            If Not String.IsNullOrEmpty(valueFileId) Then
                Dim fileId As Integer = Integer.Parse(valueFileId, CultureInfo.InvariantCulture)
                Dim controller As New FileController
                Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim fileById As FileInfo = controller.GetFileById(fileId, currentPortalSettings.PortalId)
                Dim list As New List(Of String)
                list.Add("jpg")
                list.Add("gif")
                list.Add("png")
                If ((Not fileById Is Nothing) AndAlso list.Contains(fileById.Extension.TrimStart("."c))) Then
                    Me._fileImage.Visible = True
                    Me._fileImage.AlternateText = fileById.FileName
                    Me._fileImage.ImageUrl = _
                        (currentPortalSettings.HomeDirectory & "/" & fileById.Folder & fileById.FileName)
                End If
            End If
        End Sub

        Protected Function UploadFile() As Boolean
            Dim str As String
            If Not Me._fileUpload.HasFile Then
                Return False
            End If
            'Dim predicate As Func(Of FileItem, Boolean) = Nothing
            'Dim currentUserInfo As UserInfo = UserController.GetCurrentUserInfo
            Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            'Dim controller As New FileController
            Dim extension As String = System.IO.Path.GetExtension(Me._fileUpload.FileName).TrimStart(New Char() {"."c})
            If _
                Not _
                (String.IsNullOrEmpty(Me.extensions) OrElse _
                 Me.extensions.Contains(extension.TrimStart(New Char() {"."c}))) Then
                Me._errorLabel.Text = _
                    String.Format(CultureInfo.InvariantCulture, "Les extensions autorisées sont {0}.", Me.extensions)
                Return False
            End If
            If ((Me.Size <> -1) AndAlso (Me._fileUpload.FileContent.Length > (Me.Size * &H400))) Then
                Me._errorLabel.Text = _
                    String.Format(CultureInfo.InvariantCulture, "La taille maximun est de {0} Ko.", _
                                   Me.Size.ToString(CultureInfo.InvariantCulture))
                Return False
            End If
            Dim fileName As String = Me._fileUpload.FileName
            Dim str3 As String = (currentPortalSettings.HomeDirectoryMapPath & Replace(Me.path, "/", "\"))
            Dim folderId As Integer = -1
            If (FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.path) Is Nothing) Then
                FileSystemUtils.AddFolder(currentPortalSettings, currentPortalSettings.HomeDirectoryMapPath, Me.path)
                ' Ajout des permissions en écriture pour l'utilisateur courant
                folderId = FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.path).FolderID
                Dim permissionToAdd As New DotNetNuke.Security.Permissions.FolderPermissionInfo
                permissionToAdd.FolderID = folderId
                Dim objRoleController As New DotNetNuke.Security.Roles.RoleController
                permissionToAdd.RoleID = objRoleController.GetRoleByName(currentPortalSettings.PortalId, "Registered Users").RoleID
                Dim objPermissionController As New DotNetNuke.Security.Permissions.PermissionController
                permissionToAdd.PermissionID = CType(objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "WRITE")(0), DotNetNuke.Security.Permissions.PermissionInfo).PermissionID
                permissionToAdd.PermissionKey = "WRITE"
                permissionToAdd.AllowAccess = True
                Dim obj As New DotNetNuke.Security.Permissions.FolderPermissionController
                obj.AddFolderPermission(permissionToAdd)
                ''''''''''''''''''''''''''''''''''''''''''''''
            Else
                Dim FdI As FolderInfo = FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.path)
                folderId = FdI.FolderID
                '''''''''''''''''''''''''''''''''''''''''''''''
            End If
            Dim fileFind As Boolean = False
            For Each myFile As FileItem In GetFileList(Me.path)
                If myFile.Text = fileName Then
                    fileFind = True
                End If
            Next

            If (fileFind) Then
                If Not Me._ckOverride.Checked And Not autoErase Then
                    Me._errorLabel.Text = "Un fichier de m" & ChrW(234) & "me nom existe déj" & ChrW(224) & "."
                    Return False
                End If
                str = FileSystemUtils.DeleteFile((System.IO.Path.Combine(str3, fileName)), currentPortalSettings)
                If Not String.IsNullOrEmpty(str) Then
                    Me._errorLabel.Text = str
                    Return False
                End If
            End If
            str = _
                FileSystemUtils.UploadFile((currentPortalSettings.HomeDirectoryMapPath & Replace(Me.path, "/", "\") & "\"), _
                                            Me._fileUpload.PostedFile, False)
            If String.IsNullOrEmpty(str) Then
                Dim fileController As New FileController
                If folderId = -1 Then
                    Dim folderController As New FolderController
                    folderId = folderController.AddFolder(PortalController.GetCurrentPortalSettings.PortalId, Me.path)
                End If

                'MyBase.Value = fileController.AddFile(PortalController.GetCurrentPortalSettings.PortalId, _
                '    fileName, _
                '    extension, _
                '    Me._fileUpload.FileContent.Length, _
                '     0, 0, "", Me.path, folderId, False)

                Dim fi As FileInfo
                Dim fc As New FileController

                fi = fc.GetFile(Me._fileUpload.FileName, currentPortalSettings.PortalId, folderId)
                If FileHelper.IsImageByExtension(fi) Then
                    MyBase.Value = fileController.AddFile(PortalController.GetCurrentPortalSettings.PortalId, _
                    fileName, _
                    extension, _
                    fi.Size, _
                    fi.Width, fi.Height, "", Me.path, folderId, False)
                Else
                    MyBase.Value = fileController.AddFile(PortalController.GetCurrentPortalSettings.PortalId, _
                        fileName, _
                        extension, _
                        fi.Size, _
                         0, 0, "", Me.path, folderId, False)
                End If
                If MyBase.Value.Equals(0) Then
                    'fix dnn6
                    MyBase.Value = fi.FileId
                End If

                'Dim myitems As List(Of FileItem) = GetFileList(Me.path)
                'For Each item As FileItem In myitems
                '    If item.Text = fileName Then
                '        MyBase.Value = item.Value
                '    End If
                'Next
                '= .Find(Function(f) f.Text = fileName).Value
            Else
                Me._errorLabel.Text = str
                Return False
            End If
            Me._downloadLink.Text = fileName
            Me.ShowImage(MyBase.Value.ToString)
            Return True
        End Function

        Private Sub uploadLink_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
            If Me.UploadFile Then
                Me.OnDataChanged(EventArgs.Empty)
            End If
        End Sub


        ' Properties
        Protected Overrides Property StringValue() As String
            Get
                If (MyBase.Value Is Nothing) Then
                    Return String.Empty
                End If
                Return MyBase.Value.ToString
            End Get
            Set(ByVal value As String)
                MyBase.Value = value
            End Set
        End Property


        ' Fields
        Private _ckOverride As CheckBox
        Private _downloadLink As LinkButton
        Private _errorLabel As Label
        Private _fileImage As Image
        Private _fileUpload As FileUpload
        Private _InfoLabel As Label
        Private _uploadLink As ImageButton
        Private extensions As String = String.Empty
        Private path As String = "Upload"
        Private Size As Integer = -1
        Private autoErase As Boolean = False


    End Class
End Namespace
