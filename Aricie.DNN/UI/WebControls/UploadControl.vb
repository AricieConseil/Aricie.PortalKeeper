
Imports Aricie.DNN.Services
Imports DotNetNuke.Common
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Globalization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities

Namespace UI.WebControls
    Public Class UploadControl
        Inherits WebControl
        ' Methods

        Private Const OverrideMessage As String = "Ecraser le fichier éxistant"
        Private Const UploadHelpMessage As String = "Sélectionnez votre fichier puis cliquez sur la flèche verte pour l'enregistrer."
        Private Const AllowedExtensionsMessage As String = "Fichiers autorisés : "
        Private Const MaximumSizeAllowedMessage As String = "Taille maximum autorisée : "
        Private Const FileUnit As String = "ko"
        Private Const AltTextUpload As String = "Charger le fichier"
        Private Const AllowedExtensionsErrorMessage As String = "Les extensions autorisées sont {0}."
        Private Const MaximumSizeAllowedErrorMessage As String = "La taille maximum est de {0} " & FileUnit & "."
        Private Const SameFileNameAlreadyExistMessage As String = "Un fichier de m" & ChrW(234) & "me nom existe déj" & ChrW(224) & "."


        Public Property AutoErase As Boolean
            Get
                Return _autoerase
            End Get
            Set(ByVal value As Boolean)
                _autoErase = value
            End Set
        End Property

        ''' <summary>
        ''' Chemin de destination du fichier. Le chemin est relatif au portail courant (ex: Upload)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Path As String
            Get
                If String.IsNullOrEmpty(_path) Then
                    _path = CStr(ViewState("Path"))
                    If String.IsNullOrEmpty(_path) Then
                        'valeur par defaut
                        _path = "Upload"
                    End If
                End If
                Return _path
            End Get
            Set(ByVal value As String)
                _path = value
                Me.ViewState("Path") = value
            End Set
        End Property

        ''' <summary>
        ''' Liste des extensions autorisées (ex: *.png;*.jpg)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Extensions As String
            Get
                Return _extensions
            End Get
            Set(ByVal value As String)
                _extensions = value
            End Set
        End Property

        Public Property Size As Integer
            Get
                Return _Size
            End Get
            Set(ByVal value As Integer)
                _Size = value
            End Set
        End Property

        Public Property FileId As Integer
            Get
                Return CInt(Me.ViewState("FileId"))
            End Get
            Set(ByVal value As Integer)
                Me.ViewState("FileId") = value
            End Set
        End Property

        Public Property DNNAccountExist As Boolean
            Get
                Return _DNNAccountExist
            End Get
            Set(ByVal value As Boolean)
                _DNNAccountExist = value
            End Set
        End Property

        Public Property customUploadMessage As String
            Get
                Return _customUploadMessage
            End Get
            Set(ByVal value As String)
                _customUploadMessage = value
            End Set
        End Property

        Public Property Show_Image As Boolean
            Get
                Return _showImage
            End Get
            Set(ByVal value As Boolean)
                _showImage = value
            End Set
        End Property

        Public Property customMaximumHeight As Integer
            Get
                Return _customMaximumHeight
            End Get
            Set(ByVal value As Integer)
                _customMaximumHeight = value
            End Set
        End Property

        Public Property customMaximumWidth As Integer
            Get
                Return _customMaximumWidth
            End Get
            Set(ByVal value As Integer)
                _customMaximumWidth = value
            End Set
        End Property



        ''' <summary>
        ''' Permet de charger un nouveau fichier une fois le chargement terminé.
        ''' </summary>
        ''' <value>False par défaut</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AllowNewUpload() As Boolean
            Get
                Return _AllowNewUpload
            End Get
            Set(ByVal value As Boolean)
                _AllowNewUpload = value
            End Set
        End Property

        ' ajouter propriétés CustomMessage string, ShowImage boolean, CustomWidth, CustomHeight (garder les proportions)
        ' ajouter constantes message par défaut


        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()


            Me._uploadLink = New ImageButton
            Me._fileUpload = New FileUpload
            Me._ckOverride = New CheckBox
            Me._deleteLink = New ImageButton
            Me._errorLabel = New Label
            Me._InfoLabel = New Label
            Me._InfoLabel.CssClass = "Normal"
            Me._downloadLink = New LinkButton
            Me._downloadLink.CssClass = "Normal"
            Me._downloadLink.CausesValidation = False
            Me._fileImage = New Image
            Me._fileImage.Visible = False

            Me._fileUpload.ID = (Me.ID & "upload")
            Me._ckOverride.Text = OverrideMessage
            Me._ckOverride.CssClass = "Normal"
            Me._uploadLink.ID = Me.ID & "uploadLink"
            Me._uploadLink.AlternateText = AltTextUpload
            Me._uploadLink.ImageUrl = "~/images/up.gif"
            Me._deleteLink.ID = Me.ID & "deleteLink"
            Me._deleteLink.ImageUrl = "~/images/delete.gif"
            Me._errorLabel.Text = String.Empty
            Me._errorLabel.CssClass = "NormalRed"

            If Me.FileId <> 0 Then
                Dim controller As New FileController
                Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim fileById As FileInfo = controller.GetFileById(FileId, currentPortalSettings.PortalId)
                If (Not fileById Is Nothing) Then
                    Me._downloadLink.Text = fileById.FileName
                    If _showImage = True Then
                        Me.ShowImage()
                    End If
                    If AllowNewUpload Then
                        Me._fileUpload.Visible = True
                        Me._uploadLink.Visible = True
                        Me._InfoLabel.Visible = True
                        Me._deleteLink.Visible = True
                    Else
                        Me._fileUpload.Visible = False
                        Me._uploadLink.Visible = False
                        Me._InfoLabel.Visible = False
                        Me._deleteLink.Visible = True
                    End If
                    
                End If
            Else
                Me._deleteLink.Visible = False
            End If

            If Me.customUploadMessage <> String.Empty Then
                Me._InfoLabel.Text = customUploadMessage & "<br>"
            Else
                Me._InfoLabel.Text = UploadHelpMessage & "<br>"
            End If

            If (Me.Extensions <> String.Empty) Then
                Me._InfoLabel.Text = (Me._InfoLabel.Text & "<br>" & AllowedExtensionsMessage & Me.Extensions & ".<br>")
            End If
            If (Me.Size <> -1) Then
                Me._InfoLabel.Text = _
                    (Me._InfoLabel.Text & "<br>" & MaximumSizeAllowedMessage & Me.Size.ToString(CultureInfo.InvariantCulture) & FileUnit & ".<br>")
            End If

            Me.Controls.Add(Me._InfoLabel)

            Me.Controls.Add(Me._fileImage)
            Me.Controls.Add(Me._fileUpload)
            If Not AutoErase Then
                Me.Controls.Add(Me._ckOverride)
            End If
            Me.Controls.Add(Me._uploadLink)
            Me.Controls.Add(Me._downloadLink)
            Me.Controls.Add(Me._deleteLink)
            Me.Controls.Add(Me._errorLabel)

        End Sub

        Private Sub downloadLink_Click(ByVal sender As Object, ByVal e As EventArgs)
            If Me.FileId <> -1 Then
                FileSystemUtils.DownloadFile(PortalController.GetCurrentPortalSettings.PortalId, Me.FileId, True, True)
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

        'Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) _
        '    As Boolean
        '    Return False
        'End Function

        'Protected Sub OnDataChanged(ByVal e As EventArgs)
        '    Dim args As New EventArgs()
        '    args.Value = _value
        '    args.OldValue = MyBase.OldValue
        '    args.StringValue = Me.StringValue
        '    MyBase.OnValueChanged(args)
        '    Dim _
        '        validator As RequiredFieldValidator = _
        '            DirectCast(MyBase.Parent.Parent.FindControl((MyBase.Name & "_Req")), RequiredFieldValidator)
        '    If (Not validator Is Nothing) Then
        '        validator.Enabled = False
        '    End If
        'End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            Me.EnsureChildControls()

            If (Not Me._downloadLink Is Nothing) Then
                AddHandler Me._downloadLink.Click, New EventHandler(AddressOf Me.downloadLink_Click)
            End If
            If (Not Me._uploadLink Is Nothing) Then
                AddHandler Me._uploadLink.Click, New ImageClickEventHandler(AddressOf Me.uploadLink_Click)
            End If
            If (Not Me._deleteLink Is Nothing) Then
                AddHandler Me._deleteLink.Click, New ImageClickEventHandler(AddressOf Me.deleteLink_Click)
            End If
        End Sub

        'Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        '    MyBase.OnPreRender(e)
        '    If ((Not Me.Page Is Nothing)) Then 'AndAlso (MyBase.EditMode = PropertyEditorMode.Edit)) Then
        '        Me.Page.RegisterRequiresPostBack(Me)
        '    End If

        'End Sub

        'Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
        '    MyBase.RenderChildren(writer)
        'End Sub

        Protected Sub ShowImage()
            If Me.FileId <> -1 Then 'If _value <> -1 Then
                Dim controller As New FileController
                Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim fileById As FileInfo = controller.GetFileById(Me.FileId, currentPortalSettings.PortalId)
                'Dim list As New List(Of String)
                'list.Add("jpg")
                'list.Add("gif")
                'list.Add("png")
                'If ((Not fileById Is Nothing) AndAlso list.Contains(fileById.Extension.TrimStart("."c))) Then
                If ((Not fileById Is Nothing) AndAlso FileHelper.IsImageByExtension(fileById)) Then
                    Me._fileImage.Visible = True
                    Me._fileImage.AlternateText = fileById.FileName
                    Me._fileImage.ImageUrl = _
                        (currentPortalSettings.HomeDirectory & "/" & fileById.Folder & fileById.FileName)

                    ' Redimensionnement optionnel
                    If Me._customMaximumHeight <> -1 Or Me._customMaximumWidth <> -1 Then
                        If Me._customMaximumHeight <> -1 And Me._customMaximumWidth <> -1 Then
                            Dim widthRatio As Double = fileById.Width / customMaximumWidth
                            Dim heightRatio As Double = fileById.Height / customMaximumHeight
                            If widthRatio > heightRatio Then
                                _fileImage.Width = customMaximumWidth
                            Else
                                _fileImage.Height = customMaximumHeight
                            End If
                        Else
                            If Me._customMaximumHeight <> -1 Then
                                _fileImage.Height = customMaximumHeight
                            Else
                                _fileImage.Width = customMaximumWidth
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Protected Function UploadFile() As Boolean
            Dim str As String = String.Empty
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
                    String.Format(CultureInfo.InvariantCulture, AllowedExtensionsErrorMessage, Me.extensions)
                Return False
            End If
            If ((Me.Size <> -1) AndAlso (Me._fileUpload.FileContent.Length > (Me.Size * &H400))) Then
                Me._errorLabel.Text = _
                    String.Format(CultureInfo.InvariantCulture, MaximumSizeAllowedErrorMessage, _
                                   Me.Size.ToString(CultureInfo.InvariantCulture))
                Return False
            End If
            Dim fileName As String = Me._fileUpload.FileName
            Dim str3 As String = System.IO.Path.Combine(currentPortalSettings.HomeDirectoryMapPath, Replace(Me.Path, "/", "\")) '(currentPortalSettings.HomeDirectoryMapPath & Replace(Me.path, "/", "\"))
            Dim folderId As Integer = -1


            If (FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.Path) Is Nothing) Then
                FileSystemUtils.AddFolder(currentPortalSettings, currentPortalSettings.HomeDirectoryMapPath, Me.Path)

                ' Ajout des permissions en écriture pour l'utilisateur courant
                folderId = FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.Path).FolderID
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
            Else
                folderId = FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.Path).FolderID
            End If
            Dim fileFind As Boolean = False
            For Each myFile As FileItem In GetFileList(Me.Path)
                If myFile.Text = fileName Then
                    fileFind = True
                End If
            Next

            If (fileFind) Then
                If Not Me._ckOverride.Checked And Not AutoErase Then
                    Me._errorLabel.Text = SameFileNameAlreadyExistMessage
                    Return False
                End If

                If DNNAccountExist Then
                    str = FileSystemUtils.DeleteFile((str3 & fileName), currentPortalSettings)
                Else

                End If

                If Not String.IsNullOrEmpty(str) Then
                    Me._errorLabel.Text = str
                    Return False
                End If
            End If

            Dim fi As FileInfo
            Dim fc As New FileController

            If DNNAccountExist Then
                str = FileSystemUtils.UploadFile(str3, Me._fileUpload.PostedFile, False)
            Else
                Me._fileUpload.PostedFile.SaveAs(str3 & Me._fileUpload.FileName)
                FileSystemUtils.SynchronizeFolder(currentPortalSettings.PortalId, str3, Me.Path, True)

                fi = fc.GetFile(Me._fileUpload.FileName, currentPortalSettings.PortalId, folderId)
                FileId = fi.FileId
            End If

            If Not String.IsNullOrEmpty(str) Then
                Me._errorLabel.Text = str
                Return False
            End If

            If String.IsNullOrEmpty(str) Then
                Dim fileController As New FileController
                If folderId = -1 Then
                    Dim folderController As New FolderController
                    folderId = folderController.AddFolder(PortalController.GetCurrentPortalSettings.PortalId, Me.Path)
                End If

                fi = fc.GetFile(Me._fileUpload.FileName, currentPortalSettings.PortalId, folderId)
                If fi Is Nothing Then
                    Return False
                End If
                If FileHelper.IsImageByExtension(fi) Then
                    FileId = fileController.AddFile(PortalController.GetCurrentPortalSettings.PortalId, _
                    fileName, _
                    extension, _
                    Me._fileUpload.FileContent.Length, _
                    fi.Width, fi.Height, "", Me.Path, folderId, False)
                Else
                    FileId = fileController.AddFile(PortalController.GetCurrentPortalSettings.PortalId, _
                        fileName, _
                        extension, _
                        Me._fileUpload.FileContent.Length, _
                         0, 0, "", Me.Path, folderId, False)
                End If
                If Not AllowNewUpload Then
                    'Cache l'upload de fichier jusqu'à la suppression du dernier fichier chargé (qui permettra d'uploader a nouveau)
                    Me._fileUpload.Visible = False
                    Me._uploadLink.Visible = False
                Else
                    Me._fileUpload.Visible = True
                    Me._uploadLink.Visible = True
                End If
              
            Else
                Me._errorLabel.Text = str
                Return False
            End If

            Me._downloadLink.Text = fileName

            If _showImage Then
                Me.ShowImage()
            End If
            RaiseEvent FileUploaded(Me, EventArgs.Empty)
            Return True
        End Function

        Private Sub uploadLink_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
            If Me.UploadFile Then
                'Me.OnDataChanged(EventArgs.Empty)
                If Not Me.AllowNewUpload Then
                    Me._InfoLabel.Visible = False
                End If
                Me._deleteLink.Visible = True
            End If
        End Sub

        Private Sub deleteLink_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
            Dim currentPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim myfolder As FolderInfo = FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.Path)
            Dim str3 As String = (currentPortalSettings.HomeDirectoryMapPath & Replace(Me.Path, "/", "\"))
            Dim str As String = String.Empty

            If Me._downloadLink.Text <> String.Empty AndAlso myfolder IsNot Nothing Then
                Me.FileId = 0

                If DNNAccountExist Then
                    str = FileSystemUtils.DeleteFile((str3 & Me._downloadLink.Text), currentPortalSettings)
                Else
                    System.IO.File.Delete(str3 & Me._downloadLink.Text)
                    FileSystemUtils.SynchronizeFolder(currentPortalSettings.PortalId, str3, Me.Path, True)
                End If

                Me._fileImage.Visible = False
                Me._downloadLink.Text = String.Empty

                'Affiche à nouveau l'uploader de fichiers
                Me._fileUpload.Visible = True
                Me._uploadLink.Visible = True
                Me._InfoLabel.Visible = True
                Me._deleteLink.Visible = False

                'FileSystemUtils.DeleteFiles(New String() {FileSystemUtils.GetFolder(currentPortalSettings.PortalId, Me.Path).FolderPath})
            End If
        End Sub

        Public Event FileUploaded(ByVal sender As Object, ByVal e As EventArgs)


        ' Properties
        'Protected Overrides Property StringValue() As String
        '    Get
        '        If (_value Is Nothing) Then
        '            Return String.Empty
        '        End If
        '        Return _value.ToString
        '    End Get
        '    Set(ByVal value As String)
        '        _value = value
        '    End Set
        'End Property


        ' Fields
        Private _ckOverride As CheckBox
        Private _downloadLink As LinkButton
        Private _deleteLink As ImageButton
        Private _errorLabel As Label
        Private _fileImage As Image
        Private _fileUpload As FileUpload
        Private _InfoLabel As Label
        Private _uploadLink As ImageButton
        Private _extensions As String = String.Empty
        Private _path As String = String.Empty
        Private _Size As Integer = -1
        Private _autoErase As Boolean = False
        Private _DNNAccountExist As Boolean = True
        Private _customUploadMessage As String = String.Empty
        Private _showImage As Boolean = False
        Private _customMaximumWidth As Integer = -1
        Private _customMaximumHeight As Integer = -1
        Private _AllowNewUpload As Boolean = False

    End Class
End Namespace
