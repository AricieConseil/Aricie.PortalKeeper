Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.Web.Configuration
Imports Aricie.ComponentModel
Imports Aricie.Cryptography
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Services.FileSystem
Imports Aricie.Services
Imports System.Text
Imports System.Globalization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Users
Imports System.Xml
Imports DotNetNuke.Services.Localization
Imports System.IO
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Security

Namespace Services.Files

    Public Enum PayLoadFormat
        None
        [String]
        Bytes
        Base64String
    End Enum

    <Serializable()> _
    Public Class SmartFile
        'Inherits SmartFileInfo

        Private _encrypter As IEncrypter
        Protected _SaltBytes As Byte()
        Private _PayLoad As New CData("")
        Private _DNNFile As DotNetNuke.Services.FileSystem.FileInfo


        Public Sub New()
        End Sub

        Public Sub New(key As EntityKey)
            Me.Key = key
        End Sub

        Public Sub New(key As EntityKey, encrypter As IEncrypter)
            Me.New(key)
            Me._encrypter = encrypter
        End Sub

        <Browsable(False)>
        Public ReadOnly Property DNNFile As DotNetNuke.Services.FileSystem.FileInfo
            Get
                Return _DNNFile
            End Get
        End Property


        <ExtendedCategory("Key")> _
        <IsReadOnly(True)> _
        Public Property Key As EntityKey

        <ExtendedCategory("Content")> _
        <IsReadOnly(True)> _
        Public Property Signed As Boolean

        <ExtendedCategory("Content")> _
        <IsReadOnly(True)> _
        Public Property Compressed As Boolean



        <ExtendedCategory("Content")> _
        Public ReadOnly Property HasEncrypter As Boolean
            Get
                Return _encrypter IsNot Nothing
            End Get
        End Property

        Public Sub SetEncrypter(ByVal encrypter As IEncrypter)
            _encrypter = encrypter
        End Sub

        <ExtendedCategory("Content")> _
        <IsReadOnly(True)> _
        Public Property Encrypted As Boolean

        <ExtendedCategory("Content")> _
        <ConditionalVisible("Encrypted", False, True)> _
        <IsReadOnly(True)> _
        Public Property HasCustomEncryption As Boolean

        <Browsable(False)> _
        <XmlIgnore()>
        Public ReadOnly Property SaltBytes As Byte()
            Get
                Return _SaltBytes
            End Get
        End Property

        <ExtendedCategory("Content")> _
        <ConditionalVisible("Encrypted", False, True)> _
        <IsReadOnly(True)> _
        Public Property Salt As String
            Get
                If Me.Encrypted Then
                    Return Convert.ToBase64String(_SaltBytes)
                End If
                Return ""
            End Get
            Set(value As String)
                _SaltBytes = Convert.FromBase64String(value)
            End Set
        End Property


        <LineCount(20)> _
      <Width(500)> _
      <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
       <ExtendedCategory("Content")> _
     <ConditionalVisible("EditPayLoadFormat", True, True, PayLoadFormat.None)> _
       <XmlIgnore()> _
        Public Property EditPayLoad As String
            Get
                Return EditPayLoad(EditPayLoadFormat)
            End Get
            Set(value As String)
                EditPayLoad(EditPayLoadFormat) = value
            End Set
        End Property

        <ExtendedCategory("Content")> _
               <XmlIgnore()> _
        Public Property EditPayLoadFormat As PayLoadFormat = PayLoadFormat.None
        '<ExtendedCategory("Content")> _
        'Public Property ShowPayLoad As Boolean

        ' <AutoPostBack()> _
        '<ExtendedCategory("Content")> _
        '       <XmlIgnore()> _
        ' Public Property ShowLineBreaks As Boolean


        <Browsable(False)>
        Public Property EditPayload(format As PayLoadFormat) As String
            Get
                Select Case format
                    Case PayLoadFormat.String
                        Return _PayLoad.Value
                    Case PayLoadFormat.Bytes
                        Return BitConverter.ToString(Encoding.UTF8.GetBytes(_PayLoad.Value))
                    Case PayLoadFormat.Base64String
                        Return Common.GetBase64FromUtf8(_PayLoad.Value)
                    Case Else
                        Return String.Empty
                End Select
            End Get
            Set(value As String)
                If format <> PayLoadFormat.None Then
                    Dim newValue As String
                    Select Case format
                        Case PayLoadFormat.String
                            newValue = value
                        Case PayLoadFormat.Bytes
                            'newValue = Encoding.UTF8.GetString(Common.StringToByteArray(value))
                            newValue = Encoding.UTF8.GetString(Common.BitConverterStringToByteArray(value))
                        Case PayLoadFormat.Base64String
                            newValue = Common.GetUtf8FromBase64(value)
                    End Select
                    _PayLoad.Value = newValue
                End If
            End Set
        End Property

        <Browsable(False)>
        Public Property PayLoad As CData
            Get
                Return _PayLoad
            End Get
            Set(value As CData)
                _PayLoad = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property PayLoadBytes As Byte()
            Get
                Return Encoding.UTF8.GetBytes(Me._PayLoad.Value)
            End Get
        End Property


        <ExtendedCategory("Content")> _
        Public ReadOnly Property Size As Integer
            Get
                Return Me.PayLoadBytes.Length
            End Get
        End Property

        <ExtendedCategory("Content")> _
        Public ReadOnly Property MD5Checksum As String
            Get
                Return Common.Hash(Me.PayLoad, HashProvider.MD5)
            End Get
        End Property

        <ExtendedCategory("Content")> _
        Public ReadOnly Property Sha256Checksum As String
            Get
                Return Common.Hash(Me.PayLoad, HashProvider.SHA256)
            End Get
        End Property

        <ExtendedCategory("Content")> _
               <XmlIgnore()> _
        Public Property UseCustomEncryption As Boolean

        <ExtendedCategory("Content")> _
        <ConditionalVisible("UseCustomEncryption", False, True)> _
               <XmlIgnore()> _
        Public Property CustomEncryption As New EncryptionInfo()


#Region "Public Methods"


        <ExtendedCategory("Content")> _
        <ConditionalVisible("Signed", True, True)> _
      <ConditionalVisible("Encrypted", True, True)> _
      <ConditionalVisible("Compressed", True, True)> _
    <ActionButton(IconName.Key, IconOptions.Normal)> _
        Public Overloads Sub Sign(ape As AriciePropertyEditorControl)
            Me.Sign()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileSigned.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub


        '<ConditionalVisible("HasEncrypter", False, True)> _
        <ExtendedCategory("Content")> _
        <ConditionalVisible("Signed", False, True)> _
         <ConditionalVisible("Encrypted", True, True)> _
      <ConditionalVisible("Compressed", True, True)> _
     <ActionButton(IconName.CheckSquareO, IconOptions.Normal)> _
        Public Overloads Sub Verify(ape As AriciePropertyEditorControl)
            Dim message As String
            Dim messageType As ModuleMessage.ModuleMessageType
            If Me.Verify() Then
                message = Localization.GetString("SignatureVerified.Message", ape.LocalResourceFile)
                messageType = ModuleMessage.ModuleMessageType.GreenSuccess
            Else
                message = Localization.GetString("SignatureFailedToVerify.Message", ape.LocalResourceFile)
                messageType = ModuleMessage.ModuleMessageType.RedError
            End If
            ape.DisplayMessage(message, messageType)
        End Sub

        <ExtendedCategory("Content")> _
      <ConditionalVisible("Signed", False, True)> _
       <ConditionalVisible("Encrypted", True, True)> _
      <ConditionalVisible("Compressed", True, True)> _
   <ActionButton(IconName.Eraser, IconOptions.Normal)> _
        Public Overloads Sub RemoveSignature(ape As AriciePropertyEditorControl)
            Me.RemoveSignature()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SignatureRemoved.Message", ape.LocalResourceFile)
            Dim messageType As ModuleMessage.ModuleMessageType = ModuleMessage.ModuleMessageType.GreenSuccess
            ape.DisplayMessage(message, messageType)
        End Sub

        <ExtendedCategory("Content")> _
        <ConditionalVisible("Compressed", True, True)> _
         <ConditionalVisible("Encrypted", True, True)> _
      <ActionButton(IconName.Compress, IconOptions.Normal)> _
        Public Sub Compress(ape As AriciePropertyEditorControl)
            Me.Compress()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileCompressed.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ExtendedCategory("Content")> _
        <ConditionalVisible("Compressed", False, True)> _
         <ConditionalVisible("Encrypted", True, True)> _
       <ActionButton(IconName.Expand, IconOptions.Normal)> _
        Public Sub Decompress(ape As AriciePropertyEditorControl)
            Me.Decompress()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileDecompressed.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ExtendedCategory("Content")> _
       <ConditionalVisible("Encrypted", False, True)> _
          <ActionButton(IconName.Unlock, IconOptions.Normal)> _
        Public Sub Decrypt(ape As AriciePropertyEditorControl)
            Me.Decrypt()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileDecrypted.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ExtendedCategory("Content")> _
       <ConditionalVisible("Encrypted", True, True)> _
       <ActionButton(IconName.Lock, IconOptions.Normal)> _
        Public Sub Encrypt(ape As AriciePropertyEditorControl)
            Me.Encrypt()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileEncrypted.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub



        Public Sub Decrypt()
            Try
                If Me.Encrypted Then
                    Dim newPayLoad As String
                    If UseCustomEncryption Then
                        newPayLoad = Me.CustomEncryption.Decrypt(Me.PayLoad, Me._SaltBytes)
                    ElseIf _encrypter IsNot Nothing Then
                        newPayLoad = _encrypter.Decrypt(Me.PayLoad, Me._SaltBytes)
                    Else
                        newPayLoad = Common.Decrypt(Me.PayLoad, GetDefaultKey(), "", Me._SaltBytes)
                    End If
                    Me.DecryptInternal(newPayLoad)
                End If
            Catch ex As Exception
                Throw New ApplicationException("Value was encrypted with a distinct key or encrypted bytes were corrupted.")
            End Try
        End Sub


        Public Sub Encrypt()
            If Not Me.Encrypted Then
                Dim objSalt As Byte() = Nothing
                Dim newPayload As String
                If UseCustomEncryption Then
                    newPayload = Me.CustomEncryption.DoEncrypt(Me.PayLoad, objSalt)
                    Me.Encrypt(newPayload, objSalt)
                ElseIf _encrypter IsNot Nothing Then
                    newPayload = _encrypter.Encrypt(Me.PayLoad, objSalt)
                    Me.Encrypt(newPayload, objSalt)
                Else
                    newPayload = Common.Encrypt(Me.PayLoad, GetDefaultKey(), "", objSalt)
                    Me.EncryptInternal(newPayload, objSalt)
                End If
            End If
        End Sub



        Public Overloads Sub Sign()
            If Me.Encrypted OrElse Me.Compressed Then
                Throw New ApplicationException("Encrypted or compressed Content cannot be Signed")
            End If
            If Not Me.Signed Then
                SyncLock Me
                    If UseCustomEncryption Then
                        PayLoad = Aricie.DNN.Security.EncryptionHelper.SignXmlString(Me.PayLoad, Me.CustomEncryption)
                        Me.Signed = True
                    ElseIf Me._encrypter IsNot Nothing Then
                        PayLoad = Aricie.DNN.Security.EncryptionHelper.SignXmlString(Me.PayLoad, _encrypter)
                        Me.Signed = True
                    Else
                        Throw New ApplicationException("Cannot sign a smart file without an ecrypter")
                    End If
                End SyncLock
            End If
        End Sub

        Public Overloads Function Verify() As Boolean
            If Not Me.Signed Then
                Throw New ApplicationException("Unsigned document cannot be verified")
            Else
                Dim doc As New XmlDocument()
                SyncLock Me
                    doc.LoadXml(Me.PayLoad)
                    If UseCustomEncryption Then
                        Return Me.CustomEncryption.Verify(doc)
                    ElseIf Me._encrypter IsNot Nothing Then
                        Return _encrypter.Verify(doc)
                    Else
                        Throw New ApplicationException("Cannot verify a smart file without an ecrypter")
                    End If
                End SyncLock
            End If
        End Function



        Public Overloads Sub RemoveSignature()
            If Not Me.Signed Then
                Throw New ApplicationException("Unsigned document cannot have signature removed")
            Else
                SyncLock Me
                    PayLoad = Aricie.DNN.Security.EncryptionHelper.RemoveSignatureFromXmlString(Me.PayLoad)
                    Me.Signed = False
                End SyncLock
            End If
        End Sub

        Public Sub Compress()
            If Me.Encrypted Then
                Throw New ApplicationException("Encrypted Content cannot be compressed")
            End If
            If Not Me.Compressed Then
                SyncLock Me
                    PayLoad = Common.DoCompress(PayLoad, CompressionMethod.Gzip)
                    Me.Compressed = True
                End SyncLock
            End If
        End Sub

        Public Sub Decompress()
            If Me.Encrypted Then
                Throw New ApplicationException("Encrypted Content cannot be decompressed")
            End If
            If Me.Compressed Then
                SyncLock Me
                    PayLoad = Common.DoDeCompress(PayLoad, CompressionMethod.Gzip)
                    Me.Compressed = False
                End SyncLock
            End If
        End Sub


        Public Function GetDefaultKey() As String
            Return New MachineKeySection().DecryptionKey
        End Function


        'Public Function GetKey() As String
        '    If UseCustomKey Then
        '        Return Me.CustomKey
        '    Else
        '        Return New MachineKeySection().DecryptionKey
        '    End If
        'End Function

        Public Sub Decrypt(newPayload As String)
            SyncLock Me
                Me.DecryptInternal(newPayload)
                Me.HasCustomEncryption = True
            End SyncLock
        End Sub

        Public Sub Encrypt(ByVal newPayLoad As String, ByVal salt As Byte())
            SyncLock Me
                Me.EncryptInternal(newPayLoad, salt)
                Me.HasCustomEncryption = True
            End SyncLock
        End Sub

        Public Overridable Sub UnWrap()
            Me.Decrypt()
            Me.Decompress()
            If Me.Signed Then
                If Not Me.Verify() Then
                    Throw New ApplicationException("Smart file integrity signature does not verify against the provided encryption")
                End If
            End If
        End Sub

        Public Sub Wrap(settings As SmartFileInfo)
            If settings.Sign AndAlso Me._encrypter IsNot Nothing Then
                Me.Sign()
            End If
            If settings.Compress Then
                Me.Compress()
            End If
            If settings.Encrypt Then
                Me.Encrypt()
            End If
        End Sub



#End Region




#Region "Shared Methods"


        Public Shared Function GetFileInfo(key As EntityKey, settings As SmartFileInfo) As DotNetNuke.Services.FileSystem.FileInfo
            Dim objPath As String = settings.GetPath(key)
            Dim folderPath As String = settings.GetFolderPath(key)
            Dim objFolderInfo As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(key.PortalId, folderPath)
            If objFolderInfo IsNot Nothing Then
                Dim fileName As String = System.IO.Path.GetFileName(objPath)
                Return ObsoleteDNNProvider.Instance.GetFile(objFolderInfo, fileName)
            End If
            Return Nothing
        End Function

        'Public Shared Function LoadAndRead(Of T As New)(key As EntityKey, settings As SmartFileInfo) As T
        '    Dim toReturn As T
        '    Dim objSmartFile As SmartFile(Of T) = LoadSmartFile(Of T)(key, settings)
        '    If objSmartFile IsNot Nothing Then
        '        toReturn = objSmartFile.Value
        '    End If
        '    Return toReturn
        'End Function


        Public Shared Function LoadSmartFile(Of T As New)(key As EntityKey, settings As SmartFileInfo) As SmartFile(Of T)
            Dim objFileInfo As DotNetNuke.Services.FileSystem.FileInfo = GetFileInfo(key, settings)
            Dim toReturn As SmartFile(Of T) = LoadSmartFile(Of T)(objFileInfo)
            If Not settings.CheckSmartFile(toReturn) Then
                Throw New ApplicationException(String.Format("smart file for key {0} at path {1} didn't match security settings", key.ToString(), objFileInfo.PhysicalPath))
            End If
            toReturn.SetEncrypter(settings.Encryption)
            Return toReturn
        End Function

        Public Shared Function LoadSmartFile(Of T As New)(objFileInfo As DotNetNuke.Services.FileSystem.FileInfo) As SmartFile(Of T)
            If objFileInfo IsNot Nothing Then
                Dim content As Byte() = ObsoleteDNNProvider.Instance.GetFileContent(objFileInfo)
                Using ms As New MemoryStream(content)
                    Using reader As XmlReader = XmlReader.Create(ms)
                        Dim toReturn As SmartFile(Of T) = ReflectionHelper.Deserialize(Of SmartFile(Of T))(reader)
                        toReturn._DNNFile = objFileInfo
                        Return toReturn
                    End Using
                End Using
            End If
            Return Nothing
        End Function


        Public Shared Function SaveSmartFile(value As SmartFile, settings As SmartFileInfo) As Boolean
            If value IsNot Nothing Then
                value.Wrap(settings)
                Dim objFolderPäth As String = settings.GetFolderPath(value.Key)
                Dim objFolderInfo As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(value.Key.PortalId, objFolderPäth)
                If objFolderInfo Is Nothing Then
                    Dim permissionUserId As Integer = -1
                    If (settings.GrantUserView OrElse settings.GrantUserEdit) AndAlso value.Key.UserName <> "" Then
                        Dim objUser As UserInfo = DotNetNuke.Entities.Users.UserController.GetUserByName(value.Key.PortalId, value.Key.UserName)
                        If objUser IsNot Nothing Then
                            permissionUserId = objUser.UserID
                        End If
                    End If
                    CreateSecureFoldersRecursive(value.Key.PortalId, objFolderPäth, permissionUserId, settings)
                    objFolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(value.Key.PortalId, objFolderPäth)
                End If
                If objFolderInfo IsNot Nothing Then
                    Dim objFileInfo As DotNetNuke.Services.FileSystem.FileInfo = GetFileInfo(value.Key, settings)
                    If objFileInfo Is Nothing Then
                        Dim objPath As String = settings.GetPath(value.Key)
                        objFileInfo = New DotNetNuke.Services.FileSystem.FileInfo() With {.FileName = System.IO.Path.GetFileName(objPath), .ContentType = "text/xml", .FolderId = objFolderInfo.FolderID, .PortalId = value.Key.PortalId, .StorageLocation = 2}
                    End If
                    Dim content As Byte()
                    Using ms As New MemoryStream()
                        Dim objXmlSettings As XmlWriterSettings = ReflectionHelper.GetStandardXmlWriterSettings()
                        Using writer As XmlWriter = XmlWriter.Create(ms, objXmlSettings)
                            ReflectionHelper.Serialize(value, writer)
                        End Using
                        content = ms.ToArray()
                    End Using
                    Dim result As Integer = ObsoleteDNNProvider.Instance.AddOrUpdateFile(objFileInfo, content, False)
                    If result < 0 Then
                        Throw New ApplicationException("Save failed, DNN returned " & result.ToString(CultureInfo.InvariantCulture))
                    End If
                    'End If
                Else
                    Throw New ApplicationException("Could not access Smart File Storage")
                End If
            Else
                Throw New ApplicationException("Cannot save null smartfile")
            End If

            Return True
        End Function

        Protected Shared Sub CreateSecureFoldersRecursive(portalId As Integer, path As String, permissionUserId As Integer, settings As SmartFileInfo)
            Dim objFolderInfo As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(portalId, path)
            If objFolderInfo Is Nothing Then
                Dim parentPath As String = path.TrimEnd("/"c)
                If parentPath.Contains("/"c) Then
                    parentPath = parentPath.Substring(0, parentPath.LastIndexOf("/"c))
                    CreateSecureFoldersRecursive(portalId, parentPath, -1, settings)
                End If
                Dim folder As New FolderInfo() With { _
                    .PortalID = portalId, _
                    .FolderPath = path, _
                    .StorageLocation = 2, _
                    .IsProtected = False, _
                    .IsCached = False _
                }
                ObsoleteDNNProvider.Instance.AddFolder(folder)
                folder = ObsoleteDNNProvider.Instance.GetFolderFromPath(folder.PortalID, folder.FolderPath)
                If folder IsNot Nothing Then
                    If permissionUserId > 0 Then
                        Dim fpc As New FolderPermissionController()
                        If settings.GrantUserView Then
                            Dim fp As New FolderPermissionInfo()
                            With fp
                                .UserID = permissionUserId
                                .FolderID = folder.FolderID
                                .FolderPath = folder.FolderPath
                                .PermissionCode = "VIEW"
                                .PermissionKey = "VIEW"
                            End With
                            fpc.AddFolderPermission(fp)
                        End If
                        If settings.GrantUserEdit Then
                            Dim fp As New FolderPermissionInfo()
                            With fp
                                .UserID = permissionUserId
                                .FolderID = folder.FolderID
                                .FolderPath = folder.FolderPath
                                .PermissionCode = "EDIT"
                                .PermissionKey = "EDIT"
                            End With
                            fpc.AddFolderPermission(fp)
                        End If
                    End If
                End If
            End If
        End Sub


#End Region


#Region "Private Methods"


        Private Sub EncryptInternal(ByVal newPayLoad As String, ByVal salt As Byte())
            SyncLock Me
                Me.PayLoad = newPayLoad
                Me._SaltBytes = salt
                Me.Encrypted = True
            End SyncLock
        End Sub

        Private Sub DecryptInternal(newPayload As String)
            SyncLock Me
                Me.PayLoad = newPayload
                Me.Encrypted = False
                Me.Salt = ""
            End SyncLock
        End Sub


#End Region

    End Class


    <Serializable()> _
    Public Class SmartFile(Of T)
        Inherits SmartFile

        Private _Value As T

        Public Sub New()
        End Sub


        Public Sub New(key As EntityKey, value As T, settings As SmartFileInfo)
            MyBase.New(key, settings.Encryption)
            Me.Value = value
            Me.Wrap(settings)
        End Sub

        <XmlIgnore()> _
        <ExtendedCategory("Value")> _
        Public Property ShowValue As Boolean

        <ExtendedCategory("Value")> _
        <XmlIgnore()> _
        <ConditionalVisible("ShowValue", False, True)> _
        Public Property Value As T
            Get
                If _Value Is Nothing Then
                    Me.UnWrap()
                    If Not String.IsNullOrEmpty(Me.PayLoad) Then
                        _Value = Aricie.Services.ReflectionHelper.Deserialize(Of T)(Me.PayLoad)
                    End If
                End If
                Return _Value
            End Get
            Set(value As T)
                _Value = value
                Me.UpdatePayload()
            End Set
        End Property

        <ExtendedCategory("Value")> _
        <ActionButton(IconName.Refresh, IconOptions.Normal)> _
        Public Sub UpdatePayload(ape As AriciePropertyEditorControl)
            Me.UpdatePayload()
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("SmartFileDecompressed.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        Public Sub UpdatePayload()
            If _Value IsNot Nothing Then
                Dim sb As New StringBuilder()
                Using sw As New StringWriter(sb)
                    Dim objXmlSettings As XmlWriterSettings = ReflectionHelper.GetStandardXmlWriterSettings()
                    Using writer As XmlWriter = XmlWriter.Create(sw, objXmlSettings)
                        ReflectionHelper.Serialize(_Value, writer)
                    End Using
                End Using
                Me.PayLoad = sb.ToString()
            Else
                Me.PayLoad = ""
            End If
        End Sub

    End Class

End Namespace