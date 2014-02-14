Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports System.Security

Namespace Entities
    <Serializable()> _
    <DefaultProperty("UserName")> _
    Public Class LoginInfo


        Private _UserName As String = ""
        Private _Password As String = ""

        Public Sub New()

        End Sub

        Sub New(username As String, password As String)
            Me._UserName = username
            Me._Password = password
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
        Public Property XmlPassword() As String
            Get
                If Not String.IsNullOrEmpty(Me._Password) Then
                    Return Aricie.Common.Encrypt(Me._Password, Me._UserName)
                End If
                Return ""
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        _Password = Aricie.Common.Decrypt(value, Me._UserName)
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                    End Try
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
                Return _Password
            End Get
            Set(ByVal value As String)
                If value <> _Password Then
                    Disabled = False
                    _Password = value
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
                Return Not Disabled AndAlso Not String.IsNullOrEmpty(Me._UserName) AndAlso Not String.IsNullOrEmpty(Me._Password)
            End Get
        End Property

        Public Function GetPasswordAsSecureString() As SecureString
            Dim toReturn As New SecureString
            For Each objChar As Char In _Password
                toReturn.AppendChar(objChar)
            Next
            Return toReturn
        End Function


    End Class


End Namespace

