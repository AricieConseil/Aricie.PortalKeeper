Imports System.Xml.Serialization
Imports DotNetNuke.Services.Authentication

Namespace Configuration

    ''' <summary>
    ''' Configuration class for a custom Authentication provider
    ''' </summary>
    <XmlRoot("authenticationService")> _
    Public Class AuthenticationServiceInfo
        Inherits TypedEntityElementInfo



        Private _Type As String = ""
        Private _SettingsControlSrc As String = ""
        Private _LoginControlSrc As String = ""
        Private _LogoffControlSrc As String = ""


        Public Sub New()
            MyBase.New()
        End Sub


        Public Sub New(ByVal type As String, ByVal settingsControlSrc As String, ByVal loginControlSrc As String, ByVal logoffControlSrc As String)
            Me._Type = type
            Me._SettingsControlSrc = settingsControlSrc
            Me._LoginControlSrc = loginControlSrc
            Me._LogoffControlSrc = logoffControlSrc
        End Sub



        <XmlElement("type")> _
        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property

        <XmlElement("settingsControlSrc")> _
        Public Property SettingsControlSrc() As String
            Get
                Return _SettingsControlSrc
            End Get
            Set(ByVal value As String)
                _SettingsControlSrc = value
            End Set
        End Property

        <XmlElement("loginControlSrc")> _
        Public Property LoginControlSrc() As String
            Get
                Return _LoginControlSrc
            End Get
            Set(ByVal value As String)
                _LoginControlSrc = value
            End Set
        End Property


        <XmlElement("logoffControlSrc")> _
        Public Property LogoffControlSrc() As String
            Get
                Return _LogoffControlSrc
            End Get
            Set(ByVal value As String)
                _LogoffControlSrc = value
            End Set
        End Property


        Public Overloads Overrides Function IsInstalled(ByVal type As Type) As Boolean
            Return AuthenticationController.GetAuthenticationServiceByType(Me.Type) IsNot Nothing
        End Function



        Public Overrides Sub ProcessConfig(ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install

                    Dim tempAuthSystem As AuthenticationInfo = AuthenticationController.GetAuthenticationServiceByType(Me.Type)
                    Dim authSystem As New AuthenticationInfo
                    Dim isNew As Boolean = False
                    If (tempAuthSystem Is Nothing) Then
                        authSystem.IsEnabled = True
                        isNew = True
                        authSystem.PackageID = -1
                    Else
                        authSystem.AuthenticationID = tempAuthSystem.AuthenticationID
                        authSystem.IsEnabled = tempAuthSystem.IsEnabled
                    End If
                    authSystem.AuthenticationType = Me.Type

                    authSystem.LoginControlSrc = Me.LoginControlSrc
                    authSystem.LogoffControlSrc = Me.LogoffControlSrc
                    authSystem.SettingsControlSrc = Me.SettingsControlSrc
                    If isNew Then
                        AuthenticationController.AddAuthentication(authSystem)
                    Else
                        AuthenticationController.UpdateAuthentication(authSystem)
                    End If
                Case ConfigActionType.Uninstall
                    Dim tempAuthSystem As AuthenticationInfo = AuthenticationController.GetAuthenticationServiceByType(Me.Type)
                    If tempAuthSystem IsNot Nothing Then
                        AuthenticationController.DeleteAuthentication(tempAuthSystem)
                    End If

            End Select
        End Sub
    End Class



End Namespace


