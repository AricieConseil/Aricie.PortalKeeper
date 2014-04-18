Imports Aricie.ComponentModel
Imports System.Security.Cryptography
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Xml
Imports DotNetNuke.Entities.Users
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Text
Imports System.IO

Namespace Security
    <Serializable()> _
    Public Class EncryptionInfo
        Implements IEncrypter

        Private _InitVector As String = String.Empty

        Private _EncryptionKey As String = String.Empty

        Private _SaltBytes As Byte() = {}


        Private _EncryptionPrivateKey As String = String.Empty
        Private _CryptoServiceProvider As RSACryptoServiceProvider

        Private _DnnDecryptionKey As String = String.Empty


        Private ReadOnly Property DnnDecryptionKey() As String
            Get
                If String.IsNullOrEmpty(_DnnDecryptionKey) Then
                    _DnnDecryptionKey = NukeHelper.WebConfigDocument.SelectSingleNode("configuration/system.web/machineKey").Attributes("decryptionKey").Value
                End If
                Return _DnnDecryptionKey
            End Get
        End Property

        '<IsReadOnly(True)> _
        <LineCount(3)> _
      <Width(500)> _
      <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property InitVector() As String
            Get
                If String.IsNullOrEmpty(_InitVector) Then
                    _InitVector = UserController.GeneratePassword(64)
                End If
                Return Common.GetBase64FromUtf8(Me._InitVector)
            End Get
            Set(ByVal value As String)
                _InitVector = Common.GetUtf8FromBase64(value)
            End Set
        End Property



        <Browsable(False)> _
      <XmlIgnore()>
        Public ReadOnly Property SaltBytes As Byte()
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


        <ConditionalVisible("IsSalted", False, False)> _
        <IsReadOnly(True)> _
        Public Property Salt As String
            Get
                Return Convert.ToBase64String(_SaltBytes)
            End Get
            Set(value As String)
                Me._EncryptionKey = ""
                _SaltBytes = Convert.FromBase64String(value)
            End Set
        End Property



        <LineCount(10)> _
       <Width(500)> _
       <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("IsSalted", True, False)> _
        Public Property EncryptionKey As String
            Get
                If Not Me.IsSalted Then
                    If String.IsNullOrEmpty(_EncryptionKey) Then
                        ResetEncryptionKeys()
                    End If
                    Return Common.GetBase64FromUtf8(Me._EncryptionKey)
                End If
                Return Nothing
            End Get
            Set(value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._EncryptionKey = Common.GetUtf8FromBase64(value)
                    Me._SaltBytes = {}
                End If
            End Set
        End Property

        <IsReadOnly(True)> _
        <ConditionalVisible("IsSalted", False, False)> _
        Public Property EncryptedKey() As String
            Get
                If Me.IsSalted Then
                    Try
                        'If String.IsNullOrEmpty(_EncryptionKey) Then
                        '    ResetEncryptionKeys()
                        'End If
                        Return Common.Encrypt(_EncryptionKey, Me.DnnDecryptionKey, Me._InitVector, Me._SaltBytes)
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) AndAlso Me.IsSalted Then
                    Try
                        _EncryptionKey = Common.Decrypt(value, Me.DnnDecryptionKey, Me._InitVector, Me._SaltBytes)
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
            End Set
        End Property

        <LineCount(36)> _
       <Width(500)> _
       <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("IsSalted", True, False)> _
        Public Property EncryptionPrivateKey As String
            Get
                If Not Me.IsSalted Then
                    If String.IsNullOrEmpty(_EncryptionPrivateKey) Then
                        ResetEncryptionKeys()
                    End If
                    Return Me._EncryptionPrivateKey
                End If
                Return Nothing
            End Get
            Set(value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Me._EncryptionPrivateKey = value
                    Me._SaltBytes = {}
                End If
            End Set
        End Property

        <IsReadOnly(True)> _
       <ConditionalVisible("IsSalted", False, False)> _
        Public Property EncryptedPrivateKey() As String
            Get
                If Me.IsSalted Then
                    Try
                        Return Common.Encrypt(_EncryptionPrivateKey, Me._EncryptionKey, Me._InitVector, Me._SaltBytes)
                    Catch ex As Exception
                        LogException(ex)

                    End Try
                End If
                Return Nothing
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) AndAlso Me.IsSalted Then
                    Try
                        _EncryptionPrivateKey = Common.Decrypt(value, Me._EncryptionKey, Me._InitVector, Me._SaltBytes)
                    Catch ex As Exception
                        LogException(ex)
                    End Try
                End If
            End Set
        End Property


        Private ReadOnly Property CryptoServiceProvider As RSACryptoServiceProvider
            Get
                If _CryptoServiceProvider Is Nothing Then
                    If String.IsNullOrEmpty(_EncryptionPrivateKey) Then
                        ResetEncryptionKeys()
                    End If
                    _CryptoServiceProvider = New RSACryptoServiceProvider
                    _CryptoServiceProvider.FromXmlString(Me._EncryptionPrivateKey)
                End If
                Return _CryptoServiceProvider
            End Get
        End Property



        <ActionButton(IconName.Key, IconOptions.Normal, "SealEncryptionKey.Alert")>
        Public Sub Seal(ape As AriciePropertyEditorControl)
            SyncLock Me
                Common.Encrypt(_EncryptionKey, Me.DnnDecryptionKey, Me._InitVector, Me._SaltBytes)
            End SyncLock
            ape.ItemChanged = True
            ape.DisplayMessage("SealEncryption.Completed", ModuleMessage.ModuleMessageType.YellowWarning)
        End Sub



        <ActionButton(IconName.Key, IconOptions.Normal, "ResetEncryptionKey.Alert")>
        Public Sub ResetEncryptionKey(ape As AriciePropertyEditorControl)
            Me.ResetEncryptionKeys()
            ape.ItemChanged = True
            ape.DisplayMessage("ResetEncryption.Completed", ModuleMessage.ModuleMessageType.YellowWarning)
        End Sub

       



        Public Function Decrypt(payload As String, salt() As Byte) As String Implements IEncrypter.Decrypt
            Return Common.Decrypt(payload, Me._EncryptionKey, Me._InitVector, salt)
        End Function

        Public Overridable Sub DoSign(ByVal doc As XmlDocument, ParamArray paths As String()) Implements IEncrypter.Sign
            SignXml(doc, CryptoServiceProvider, paths)
        End Sub

        Public Overridable Function Verify(ByVal signedDoc As XmlDocument) As Boolean Implements IEncrypter.Verify
            Return VerifyXml(signedDoc, CryptoServiceProvider)
        End Function

        Public Function DoEncrypt(payload As String, ByRef salt() As Byte) As String Implements IEncrypter.Encrypt
            Return Common.Encrypt(payload, Me._EncryptionKey, Me._InitVector, salt)
        End Function

        Private Sub ResetEncryptionKeys()
            Me._EncryptionKey = UserController.GeneratePassword(128) & UserController.GeneratePassword(128) & UserController.GeneratePassword(128)
            Me._EncryptionPrivateKey = New RSACryptoServiceProvider(2048).ToXmlString(True)
            Me._SaltBytes = {}
        End Sub


    End Class
End Namespace