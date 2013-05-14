Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization

Namespace Entities

    ''' <summary>
    ''' Entity class for common secure API credentials authentication systems
    ''' </summary>
    ''' <remarks>The last authentication date helps managing locks.</remarks>
    <Serializable()> _
    <DefaultProperty("Key")> _
    Public Class APICredentials

        Private _Key As String = ""
        Private _Secret As String = ""
        Private _LastAuthenticationFailure As DateTime = DateTime.MinValue
        Private _Disabled As Boolean

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <Width(500)> _
        <DotNetNuke.UI.WebControls.MaxLength(256)> _
        Public Property Key() As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                If Me._Disabled AndAlso Not String.IsNullOrEmpty(_Key) AndAlso value <> _Key Then
                    Me._Disabled = False
                End If
                _Key = value.Trim
            End Set
        End Property


        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
         <LineCount(2)> _
        <Width(500)> _
        <DotNetNuke.UI.WebControls.MaxLength(256)> _
        Public Property Secret() As String
            Get
                Return _Secret
            End Get
            Set(ByVal value As String)
                If Me._Disabled AndAlso Not String.IsNullOrEmpty(_Secret) AndAlso value <> _Secret Then
                    Me._Disabled = False
                End If
                _Secret = value.Trim
            End Set
        End Property



        <IsReadOnly(True)> _
        Public Property LastAuthenticationFailure() As DateTime
            Get
                Return _LastAuthenticationFailure
            End Get
            Set(ByVal value As DateTime)
                _LastAuthenticationFailure = value
            End Set
        End Property



        <IsReadOnly(True)> _
        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    If Now.Subtract(_LastAuthenticationFailure) < TimeSpan.FromHours(1) Then
                        _Disabled = True
                    End If
                    _LastAuthenticationFailure = Now
                Else
                    _Disabled = False
                End If
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property Enabled() As Boolean
            Get
                Return Not _Disabled AndAlso Not String.IsNullOrEmpty(Me._Key) AndAlso Not String.IsNullOrEmpty(Me._Secret) AndAlso Not Me._Secret.Contains(" "c)
            End Get
        End Property

    End Class

    <Serializable()> _
    <DefaultProperty("UserName")> _
    Public Class LoginInfo


        Private _UserName As String = ""
        Private _Password As String = ""
        'Private _DisplayPasswordChars As Boolean
        Private _Disabled As Boolean


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

        'Public Property DisplayPasswordChars() As Boolean
        '    Get
        '        Return _DisplayPasswordChars
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _DisplayPasswordChars = value
        '    End Set
        'End Property

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

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Property Password() As String
            Get
                Return _Password
            End Get
            Set(ByVal value As String)
                If value <> _Password Then
                    _Disabled = False
                    _Password = value
                End If
            End Set
        End Property

        <XmlIgnore()> _
       <ConditionalVisible("DisplayPasswordChars", True, True)> _
       <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
       <Required(True)> _
       <Width(500)> _
       <DotNetNuke.UI.WebControls.MaxLength(256)> _
       <PasswordMode()> _
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




        ' <XmlIgnore()> _
        '<ConditionalVisible("DisplayPasswordChars", False, True)> _
        '<Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        '<Required(True)> _
        '<Width(500)> _
        '<DotNetNuke.UI.WebControls.MaxLength(256)> _
        'Public Property ClearPassword() As String
        '     Get
        '         Return _Password
        '     End Get
        '     Set(ByVal value As String)
        '         If Not String.IsNullOrEmpty(value) Then
        '             Me.Password = value
        '         End If
        '     End Set
        ' End Property




        <IsReadOnly(True)> _
        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal value As Boolean)
                _Disabled = value
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property Enabled() As Boolean
            Get
                Return Not _Disabled AndAlso Not String.IsNullOrEmpty(Me._UserName) AndAlso Not String.IsNullOrEmpty(Me._Password)
            End Get
        End Property

    End Class


End Namespace

