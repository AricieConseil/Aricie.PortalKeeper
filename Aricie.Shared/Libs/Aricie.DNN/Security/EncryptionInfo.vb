Imports Aricie.ComponentModel
Imports System.Security.Cryptography
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.Security.Cryptography
Imports Aricie.Cryptography
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Xml
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Text
Imports Newtonsoft.Json

Namespace Security.Cryptography
    Public Class EncryptionInfo
        Implements IEncrypter

        Public Sub New()
            Me.New(False)
        End Sub

        Public Sub New(initialize As Boolean)
            If initialize Then
                Me.ResetEncryptionKeys()
            End If
        End Sub


        Private _EncryptionKey As Byte() = {}

        Private _EncryptedPrivateKey As Byte() = {}

        Private _SaltBytes As Byte() = {}

        'Private _EncryptionPrivateKey As String = String.Empty
        Private _AsymmetricAlgo As AsymmetricAlgorithm

        'Private _SymmetricAlgo As SymmetricAlgorithm

        Private _DnnEncrypter As ICryptoTransform
        Private _DnnDecrypter As ICryptoTransform
        Private _ProtectInMemory As Boolean
        Private _SealType As KeyProtectionMode
        Private _EncryptionTypes As New List(Of EncryptionType)()


        'Public ReadOnly Property SymmetricAlgo As SymmetricAlgorithm
        '    Get
        '        If _SymmetricAlgo Is Nothing Then
        '            ResetEncryptionKeys()
        '        End If
        '        Return _SymmetricAlgo
        '    End Get
        'End Property

        '<IsReadOnly(True)> _
        'Public Property Id As String = Guid.NewGuid().ToString




        <IsReadOnly(True)>
        Public Property SealType As KeyProtectionMode
            Get
                Return _SealType
            End Get
            Set(value As KeyProtectionMode)
                _SealType = value
            End Set
        End Property

        <Browsable(False)>
        Public ReadOnly Property IsSealApplicationEncrypted As Boolean
            Get
                Return (Me.SealType And KeyProtectionMode.Application) = KeyProtectionMode.Application
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property IsSealInKeyContainer As Boolean
            Get
                Return (Me.SealType And KeyProtectionMode.KeyContainer) = KeyProtectionMode.KeyContainer
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property IsSealDataProtected As Boolean
            Get
                Return (Me.SealType And KeyProtectionMode.ProtectData) = KeyProtectionMode.ProtectData
            End Get
        End Property

        Public Property AsymmetricKeySize As RsaKeySize = RsaKeySize.Key2048

        Public Property SymmetricKeySize As RijndelKeySizes = RijndelKeySizes.Key256

        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List)>
        Public Property EncryptionTypes As List(Of EncryptionType)
            Get
                Return _EncryptionTypes
            End Get
            Set(value As List(Of EncryptionType))
                If value.Count = 0 Then
                    value.Add(EncryptionType.Symmetric)
                End If
                _EncryptionTypes = value
            End Set
        End Property

        <AutoPostBack()>
        Public Property ProtectInMemory As Boolean
            Get
                Return _ProtectInMemory
            End Get
            Set(value As Boolean)
                If value <> Me._ProtectInMemory AndAlso Me._EncryptionKey.Length > 0 Then
                    If value Then
                        ProtectedMemory.Protect(Me._EncryptionKey, MemoryProtectionScope.SameProcess)
                    Else
                        ProtectedMemory.Unprotect(Me._EncryptionKey, MemoryProtectionScope.SameProcess)
                    End If
                End If
                _ProtectInMemory = value
            End Set
        End Property


        <Browsable(False)>
        Public Property InitVector As Byte() = New Byte() {}

        <JsonIgnore()>
        <XmlIgnore()> _
          <Width(500)> _
          <LineCount(2)> _
          <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property EditInitVector() As String
            Get
                If InitVector.Length = 0 Then
                    Me.ResetEncryptionKeys()
                End If
                Return Convert.ToBase64String(Me.InitVector)
            End Get
            Set(ByVal value As String)
                Dim byteVal As Byte() = Convert.FromBase64String(value)
                InitVector = byteVal
            End Set
        End Property

        <Browsable(False)> _
      <XmlIgnore()>
        Public ReadOnly Property DNNSaltBytes As Byte()
            Get
                Return _SaltBytes
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property IsSalted As Boolean
            Get
                Return Me._SaltBytes.Length > 0
            End Get
        End Property

        '<XmlAttribute()> _
        <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("IsSalted", False, False)> _
       <IsReadOnly(True)> _
        Public Property DNNSalt As String
            Get
                If _SaltBytes.Length > 0 Then
                    Return Convert.ToBase64String(_SaltBytes)
                End If
                Return Nothing
            End Get
            Set(value As String)
                _SaltBytes = Convert.FromBase64String(value)
            End Set
        End Property

        <ExtendedCategory("PublicKeys")> _
        Public Property PublicKeyDisplay As PublicKeyDisplay

        <ExtendedCategory("PublicKeys")> _
        <Width(500)> _
        <LineCount(10)> _
     <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
     <ConditionalVisible("PublicKeyDisplay", False, True, PublicKeyDisplay.Xml)> _
        Public ReadOnly Property PublicKeyAsXml As String
            Get
                Return Me.AsymmetricAlgo.ToXmlString(False)
            End Get
        End Property

        <ExtendedCategory("PublicKeys")> _
        <Width(500)> _
        <LineCount(10)> _
     <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
     <ConditionalVisible("PublicKeyDisplay", False, True, PublicKeyDisplay.CSPBlob)> _
        Public ReadOnly Property PublicKeyAsCSPBlob As String
            Get
                Return Convert.ToBase64String(DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).ExportCspBlob(False))
            End Get
        End Property

        <ExtendedCategory("PublicKeys")>
        <ConditionalVisible("PublicKeyDisplay", False, True, PublicKeyDisplay.RSAParameters)>
        <LabelMode(LabelMode.Top)>
        Public ReadOnly Property PublicKeyAsRSAParameters As RSAParametersInfo
            Get
                Return New RSAParametersInfo(DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).ExportParameters(False))
            End Get
        End Property

        <Browsable(False)> _
        Public Property EncryptionPrivateKey As CData
            Get
                Try
                    If Me.SealType = KeyProtectionMode.None AndAlso Me._AsymmetricAlgo IsNot Nothing Then
                        Return Me._AsymmetricAlgo.ToXmlString(True)
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try
                Return Nothing
            End Get
            Set(value As CData)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        'Dim ephemeral As CspParameters = CryptoHelper.CSPCreateNewKey(String.Empty, Me.AsymmetricKeySize, False, True)
                        _AsymmetricAlgo = New RSACryptoServiceProvider(CInt(Me.AsymmetricKeySize))
                        _AsymmetricAlgo.FromXmlString(value)
                        Me.SealType = KeyProtectionMode.None
                    Catch ex As Exception
                        LogException(ex)
                    End Try
                End If
            End Set
        End Property


        <XmlIgnore()> _
        <JsonIgnore()> _
        <ExtendedCategory("PrivateKeys")> _
        <ConditionalVisible("SealType", False, True, KeyProtectionMode.None)> _
       <LineCount(36)> _
      <Width(500)> _
      <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property EncryptionPrivateKeyDisplay As CData
            Get
                Try
                    If Me.SealType = KeyProtectionMode.None Then
                        Return Me.AsymmetricAlgo.ToXmlString(True)
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try
                Return Nothing
            End Get
            Set(value As CData)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        _AsymmetricAlgo = New RSACryptoServiceProvider(CInt(Me.AsymmetricKeySize))
                        _AsymmetricAlgo.FromXmlString(value)
                        Me.SealType = KeyProtectionMode.None
                    Catch ex As Exception
                        LogException(ex)
                    End Try
                End If
            End Set
        End Property




        <ExtendedCategory("PrivateKeys")> _
        <LineCount(4)> _
      <Width(500)> _
       <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("SealType", False, True, KeyProtectionMode.ProtectData)> _
        Public ReadOnly Property EditProtectedDataEntropy As String
            Get
                Return Me.ProtectedDataEntropy.ToBase64()
            End Get
        End Property

        <Browsable(False)> _
        Public Property ProtectedDataEntropy As Byte() = New Byte() {}


        <ExtendedCategory("PrivateKeys")> _
        <LineCount(36)> _
      <Width(500)> _
       <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("SealType", True, True, KeyProtectionMode.None)> _
       <IsReadOnly(True)> _
        Public Property EncryptedPrivateKey() As String
            Get
                If Not Me.SealType = KeyProtectionMode.None Then
                    Try
                        Dim objBytes As Byte()

                        If Me.SealType <> KeyProtectionMode.None Then

                            objBytes = _EncryptedPrivateKey
                            Return objBytes.ToBase64()

                        Else
                            Return Nothing
                        End If
                    Catch ex As Exception
                        LogException(ex)
                    End Try
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        Me._EncryptedPrivateKey = value.FromBase64()
                    Catch ex As Exception
                        LogException(ex)
                    End Try
                End If
            End Set
        End Property

        Private ReadOnly Property AsymmetricAlgo As AsymmetricAlgorithm
            Get
                If _AsymmetricAlgo Is Nothing Then
                    If Me._EncryptedPrivateKey.Length > 0 Then

                        Dim objBytes As Byte() = Me._EncryptedPrivateKey

                        If (Me.SealType And KeyProtectionMode.ProtectData) = KeyProtectionMode.ProtectData Then
                            objBytes = ProtectedData.Unprotect(objBytes, Me.GetProtectedDataEntropy(), DataProtectionScope.CurrentUser)
                        End If
                        If (Me.SealType And KeyProtectionMode.KeyContainer) = KeyProtectionMode.KeyContainer Then
                            Dim containerName As String = GetCspContainerName(objBytes.ToBase64())
                            objBytes = objBytes.CSPDecrypt(containerName, AsymmetricEncryptionType.ByBlock)
                        End If
                        If (Me.SealType And KeyProtectionMode.Application) = KeyProtectionMode.Application Then
                            objBytes = objBytes.Decrypt(Me.DnnDecrypter)
                        End If

                        Dim keyContainer As CspParameters = CryptoHelper.CSPCreateNewKey(String.Empty, AsymmetricKeySize, True, True)
                        Dim rsaAlgo As New RSACryptoServiceProvider(keyContainer)
                        rsaAlgo.ImportCspBlob(objBytes)
                        'delete sensitive information
                        objBytes.ClearBytes(True)
                        _AsymmetricAlgo = rsaAlgo
                    Else
                        ResetEncryptionKeys()
                    End If
                End If
                Return _AsymmetricAlgo
            End Get
        End Property


        Private ReadOnly Property DnnEncrypter() As ICryptoTransform
            Get
                If _DnnEncrypter Is Nothing Then
                    Dim objBytes As Byte() = NukeHelper.DecryptionKey.ReadSecureStringToBytes(Encoding.UTF8)
                    _DnnEncrypter = CryptoHelper.GetRijndaelManaged(CryptoTransformDirection.Encrypt, objBytes, Me.InitVector, Me._SaltBytes)
                End If
                Return _DnnEncrypter
            End Get
        End Property


        Private ReadOnly Property DnnDecrypter() As ICryptoTransform
            Get
                If _DnnDecrypter Is Nothing Then
                    _DnnDecrypter = CryptoHelper.GetRijndaelManaged(CryptoTransformDirection.Decrypt, NukeHelper.DecryptionKey.ReadSecureStringToBytes(Encoding.UTF8), Me.InitVector, Me._SaltBytes)
                End If
                Return _DnnDecrypter
            End Get
        End Property


        <ExtendedCategory("PrivateKeys")> _
        <ConditionalVisible("SealType", False, True, KeyProtectionMode.None)> _
        <LineCount(3)> _
       <Width(500)> _
       <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property EncryptionKey As String
            Get
                If Me.SealType = KeyProtectionMode.None Then
                    Dim objBytes As Byte() = Me._EncryptionKey
                    If Me.ProtectInMemory Then
                        objBytes = DirectCast(Me._EncryptionKey.Clone(), Byte())
                        ProtectedMemory.Unprotect(objBytes, MemoryProtectionScope.SameProcess)
                    End If
                    Return Convert.ToBase64String(objBytes)
                End If
                Return Nothing
            End Get
            Set(value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Dim objBytes As Byte() = Convert.FromBase64String(value)
                    If Me.ProtectInMemory Then
                        ProtectedMemory.Protect(objBytes, MemoryProtectionScope.SameProcess)
                    End If
                    Me._EncryptionKey = objBytes
                End If
            End Set
        End Property

        <ExtendedCategory("PrivateKeys")> _
        <LineCount(6)> _
      <Width(500)> _
       <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("SealType", True, True, KeyProtectionMode.None)> _
       <IsReadOnly(True)> _
        Public Property EncryptedKey() As String
            Get
                If Me.SealType <> KeyProtectionMode.None Then
                    Try
                        Dim objBytes As Byte() = Me._EncryptionKey
                        If Me._EncryptionKey.Length > 0 Then
                            If Me.ProtectInMemory Then
                                objBytes = DirectCast(Me._EncryptionKey.Clone(), Byte())
                                ProtectedMemory.Unprotect(objBytes, MemoryProtectionScope.SameProcess)
                            End If
                        End If
                        objBytes = DirectCast(AsymmetricAlgo, RSACryptoServiceProvider).Encrypt(objBytes, True)
                        Return objBytes.ToBase64()
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        Dim objBytes As Byte() = DirectCast(AsymmetricAlgo, RSACryptoServiceProvider).Decrypt(Convert.FromBase64String(value), True)
                        If Me.ProtectInMemory Then
                            ProtectedMemory.Protect(objBytes, MemoryProtectionScope.SameProcess)
                        End If
                        Me._EncryptionKey = objBytes
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
            End Set
        End Property







        <ExtendedCategory("PrivateKeys")> _
        <ConditionalVisible("SealType", False, True, KeyProtectionMode.None)> _
        <ActionButton(IconName.Lock, IconOptions.Normal, "SealEncryptionKey.Alert")>
        Public Sub SealInApplication(ape As AriciePropertyEditorControl)
            If Me.SealType = KeyProtectionMode.None Then
                SyncLock Me
                    Dim objBytes As Byte()

                    objBytes = DirectCast(AsymmetricAlgo, RSACryptoServiceProvider).ExportCspBlob(True)

                    objBytes = objBytes.Encrypt(Me.DnnEncrypter)
                    Me._EncryptedPrivateKey = objBytes

                    Me.SealType = Me.SealType Or KeyProtectionMode.Application
                End SyncLock
                ape.ItemChanged = True
                ape.DisplayLocalizedMessage("SealInApplication.Completed", ModuleMessage.ModuleMessageType.GreenSuccess)
            Else
                Throw New ApplicationException("Can only seal clear params into Application")
            End If
        End Sub

        <ExtendedCategory("PrivateKeys")> _
        <ConditionalVisible("SealType", True, True, KeyProtectionMode.ProtectData)> _
      <ConditionalVisible("SealType", True, True, KeyProtectionMode.KeyContainer)> _
      <ActionButton(IconName.Lock, IconOptions.Normal, "SealInKeyContainer.Alert")>
        Public Sub SealInCSP(ape As AriciePropertyEditorControl)
            SyncLock Me
                Dim objBytes As Byte()
                If Me.SealType = KeyProtectionMode.None Then
                    objBytes = DirectCast(AsymmetricAlgo, RSACryptoServiceProvider).ExportCspBlob(True)
                Else
                    objBytes = _EncryptedPrivateKey
                End If
                'Dim ephemeral As CspParameters = CryptoHelper.CSPCreateNewKey(String.Empty, AsymmetricKeySize, False, True)
                Dim tmpRsa As New RSACryptoServiceProvider(CInt(AsymmetricKeySize))
                objBytes = tmpRsa.EncryptByBlocks(objBytes)
                Dim containerName = GetCspContainerName(objBytes.ToBase64())
                CSPImportFromXml(CStr(containerName), Me.AsymmetricKeySize, tmpRsa.ToXmlString(True), True)
                tmpRsa.Clear()
                Me._EncryptedPrivateKey = objBytes
                Me.SealType = Me.SealType Or KeyProtectionMode.KeyContainer
            End SyncLock
            ape.ItemChanged = True
            ape.DisplayLocalizedMessage("SealInKeyContainer.Completed", ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ExtendedCategory("PrivateKeys")> _
        <ConditionalVisible("SealType", True, True, KeyProtectionMode.ProtectData)> _
        <ActionButton(IconName.Lock, IconOptions.Normal, "SealProtectData.Alert")>
        Public Sub SealProtectData(ape As AriciePropertyEditorControl)
            SyncLock Me
                Dim objBytes As Byte()
                If Me.SealType = KeyProtectionMode.None Then
                    objBytes = DirectCast(AsymmetricAlgo, RSACryptoServiceProvider).ExportCspBlob(True)
                Else
                    objBytes = _EncryptedPrivateKey
                End If
                objBytes = ProtectedData.Protect(objBytes, Me.GetProtectedDataEntropy(), DataProtectionScope.CurrentUser)
                Me._EncryptedPrivateKey = objBytes

                Me.SealType = Me.SealType Or KeyProtectionMode.ProtectData
            End SyncLock
            ape.ItemChanged = True
            ape.DisplayLocalizedMessage("SealProtectData.Completed", ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub


        <ExtendedCategory("PrivateKeys")> _
        <ActionButton(IconName.Refresh, IconOptions.Normal, "ResetEncryptionKey.Alert")>
        Public Sub ResetEncryptionKeys(ape As AriciePropertyEditorControl)
            Me.ResetEncryptionKeys()
            ape.ItemChanged = True
            ape.DisplayLocalizedMessage("ResetEncryption.Completed", ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        Public Function DoEncrypt(clearText As String) As EncryptionResult

            Dim clearTextBytes As Byte() = clearText.ToUTF8()
            Dim saltBytes As Byte() = Nothing
            Dim cypherBytes As Byte() = Me.DoEncrypt(clearTextBytes, saltBytes)
            Dim toReturn As New EncryptionResult() With {.CypherText = cypherBytes.ToBase64(), _
                                                         .Salt = DirectCast(IIf(saltBytes IsNot Nothing, saltBytes.ToBase64(), String.Empty), String)}
            Return toReturn
        End Function

        Public Function Decrypt(encrypted As EncryptionResult) As String

            Dim cypherTextBytes As Byte() = encrypted.CypherText.FromBase64()
            Dim saltBytes As Byte() = encrypted.Salt.FromBase64()
            Dim clearTextBytes As Byte() = Decrypt(cypherTextBytes, saltBytes)
            Return clearTextBytes.FromUTF8()
        End Function

        Public Function DoEncrypt(payload As Byte(), ByRef salt() As Byte) As Byte() Implements IEncrypter.Encrypt
            Dim objBytes As Byte() = payload
            For Each objEncryptionType As EncryptionType In Me.EncryptionTypes
                If (objEncryptionType And EncryptionType.Symmetric) = EncryptionType.Symmetric Then
                    Dim keyBytes As Byte() = Me._EncryptionKey
                    If keyBytes.Length > 0 Then
                        If Me.ProtectInMemory Then
                            keyBytes = DirectCast(Me._EncryptionKey.Clone(), Byte())
                            ProtectedMemory.Unprotect(keyBytes, MemoryProtectionScope.SameProcess)
                        End If
                        objBytes = objBytes.Encrypt(keyBytes, Me.InitVector, salt)
                        If Me.ProtectInMemory Then
                            keyBytes.FillRandom()
                        End If
                    End If
                End If
                If (objEncryptionType And EncryptionType.Asymmetric) = EncryptionType.Asymmetric Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).Encrypt(objBytes, True)
                End If
                If (objEncryptionType And EncryptionType.AsymmetricByBlock) = EncryptionType.AsymmetricByBlock Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).EncryptByBlocks(objBytes)
                End If
                If (objEncryptionType And EncryptionType.AsymmetricKeyExchange) = EncryptionType.AsymmetricKeyExchange Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).EncryptByKeyExchange(objBytes)
                End If
            Next

            Return objBytes
        End Function

        Public Function Decrypt(payload As Byte(), salt() As Byte) As Byte() Implements IEncrypter.Decrypt
            Dim objBytes As Byte() = payload

            For i As Integer = Me.EncryptionTypes.Count - 1 To 0 Step -1
                Dim objEncryptionType As EncryptionType = Me.EncryptionTypes(i)
                If (objEncryptionType And EncryptionType.AsymmetricKeyExchange) = EncryptionType.AsymmetricKeyExchange Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).DecryptFromKeyExchange(objBytes)
                End If
                If (objEncryptionType And EncryptionType.AsymmetricByBlock) = EncryptionType.AsymmetricByBlock Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).DecryptBlocks(objBytes)
                End If
                If (objEncryptionType And EncryptionType.Asymmetric) = EncryptionType.Asymmetric Then
                    objBytes = DirectCast(Me.AsymmetricAlgo, RSACryptoServiceProvider).Decrypt(objBytes, True)
                End If
                If (objEncryptionType And EncryptionType.Symmetric) = EncryptionType.Symmetric Then
                    Dim keyBytes As Byte() = Me._EncryptionKey
                    If keyBytes.Length > 0 Then
                        If Me.ProtectInMemory Then
                            keyBytes = DirectCast(Me._EncryptionKey.Clone(), Byte())
                            ProtectedMemory.Unprotect(keyBytes, MemoryProtectionScope.SameProcess)
                        End If
                        objBytes = objBytes.Decrypt(keyBytes, Me.InitVector, salt)
                        If Me.ProtectInMemory Then
                            keyBytes.FillRandom()
                        End If
                    End If
                End If
            Next
            Return objBytes
        End Function

        Public Overridable Sub DoSign(ByRef doc As XmlDocument, ParamArray paths As String()) Implements IEncrypter.Sign
            SignXml(doc, AsymmetricAlgo, paths)
        End Sub

        Public Overridable Function Verify(ByVal signedDoc As XmlDocument) As Boolean Implements IEncrypter.Verify
            Return VerifyXml(signedDoc, AsymmetricAlgo)
        End Function



        Private Sub ResetEncryptionKeys()
            Me.SealType = KeyProtectionMode.None
            Me._SaltBytes = {}
            CryptoHelper.GetNewRijndaelManaged(Me.SymmetricKeySize, Me._EncryptionKey, Me.InitVector)
            'Dim ephemeral As CspParameters = CryptoHelper.CSPCreateNewKey(String.Empty, AsymmetricKeySize, False, True)
            Me._AsymmetricAlgo = New RSACryptoServiceProvider(CInt(AsymmetricKeySize))
            Me.ProtectedDataEntropy = {}
            If Me.EncryptionTypes.Count = 0 Then
                Me.EncryptionTypes.Add(EncryptionType.Symmetric)
            End If
        End Sub

        'Private Function GetEncryptedKey() As String
        '    Try
        '        Dim encrypted As String = Common.Encrypt(_EncryptionKey, DnnCryptoTransform)
        '        Select Case Me.SealType
        '            Case Security.EncryptionDepth.Machine
        '                Dim cp As New CspParameters()
        '                cp.KeyContainerName = Me.GetCspContainerName()
        '                Dim rsa As New RSACryptoServiceProvider(cp)
        '                encrypted = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(encrypted), True))
        '        End Select
        '        Return encrypted
        '    Catch ex As Exception
        '        ExceptionHelper.LogException(ex)
        '    End Try
        '    Return String.Empty
        'End Function

        Private Function GetCspContainerName(key As String) As String
            Return (Me.DNNSaltBytes.Hash(HashProvider.SHA256) & Me.InitVector.Hash(HashProvider.SHA256) & key.Hash(HashProvider.SHA256)).Hash(HashProvider.SHA256)
        End Function


        Private Function GetProtectedDataEntropy() As Byte()
            If ProtectedDataEntropy.Length = 0 Then
                Me.ProtectedDataEntropy = ProtectedData.Protect(CryptoHelper.GetNewSalt(30), Me.InitVector, DataProtectionScope.CurrentUser)
            End If
            Dim toReturn As Byte() = DirectCast(Me.ProtectedDataEntropy.Clone(), Byte())
            ProtectedData.Unprotect(toReturn, Me.InitVector, DataProtectionScope.CurrentUser)
            Return toReturn
        End Function







    End Class
End Namespace