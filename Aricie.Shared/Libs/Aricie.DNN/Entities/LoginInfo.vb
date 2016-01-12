Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.Security.Cryptography
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports System.Security
Imports System.Text
Imports Aricie.DNN.UI.WebControls

Namespace Entities

    <ActionButton(IconName.User, IconOptions.Normal)> _
    <DefaultProperty("UserName")> _
    Public Class LoginInfo


        Private _UserName As String = ""
        Private _Password As New SecureString()

        Public Sub New()

        End Sub

        Sub New(username As String, password As String)
            Me._UserName = username
            Me._Password = password.WriteSecureString(True)
        End Sub


        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <Width(500)> _
        <DotNetNuke.UI.WebControls.MaxLength(256)> _
        Public Property UserName() As String
            Get
                Return _UserName
            End Get
            Set(ByVal value As String)
                _UserName = value
            End Set
        End Property


        Public Property DisplayPasswordChars As Boolean

        <Browsable(False)> _
        Public Property PasswordSalt As Byte() = CryptoHelper.GetNewSalt(20)

        <Browsable(False)> _
        Public Property EncryptedPassword() As Byte()
            Get
                If Me._Password.Length > 0 Then
                    Return Me._Password.ReadSecureStringToBytes(Encoding.UTF8).Encrypt(Encoding.UTF8.GetBytes(Me._UserName), Nothing, Me.PasswordSalt)

                End If
                Return New Byte() {}
            End Get
            Set(ByVal value As Byte())
                If value.Length <> 0 Then
                    Try
                        _Password = value.Decrypt(Encoding.UTF8.GetBytes(Me._UserName), Nothing, Me.PasswordSalt).WriteSecureString(True, Encoding.UTF8, True)
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                    End Try
                Else
                    _Password.Clear()
                End If
            End Set
        End Property

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <MaxLength(256)> _
        <ConditionalVisible("DisplayPasswordChars", False, True)> _
         <Required(True)> _
       <Width(500)> _
          <XmlIgnore()> _
        Public Property Password() As String
            Get
                Return _Password.ReadSecureString()
            End Get
            Set(ByVal value As String)
                If value <> _Password.ReadSecureString() Then
                    Disabled = False
                    _Password = value.WriteSecureString(True)
                End If
            End Set
        End Property


        <ConditionalVisible("DisplayPasswordChars", True, True)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <Width(500)> _
        <MaxLength(256)> _
        <PasswordMode()> _
        <XmlIgnore()> _
        Public Property HiddenPassword() As String
            Get
                Return New String("x"c, _Password.Length)
            End Get
            Set(ByVal value As String)
                If value IsNot Nothing AndAlso value.Replace("x"c, String.Empty).Length > 0 Then
                    Me.Password = value
                End If
            End Set
        End Property


        <IsReadOnly(True)>
        Public Property Disabled As Boolean


        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property Enabled() As Boolean
            Get
                Return Not Disabled AndAlso Not String.IsNullOrEmpty(Me._UserName) AndAlso Not Me._Password.Length = 0
            End Get
        End Property

        Public Function GetPasswordAsSecureString() As SecureString
            Return _Password
        End Function


    End Class


End Namespace

