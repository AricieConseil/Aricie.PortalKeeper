Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Google.GData.Client
Imports Aricie.DNN.UI.WebControls
Imports Google.GData.Spreadsheets
Imports System.Xml.Serialization

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class GDataServiceInfo(Of T As Service, TEngineEvents As IConvertible)

        Public Sub New()

        End Sub
        Public Sub New(scope As String)
            Me.AuthenticationSettings = New SimpleOrExpression(Of GoogleOAuth2Info)(New GoogleOAuth2Info(scope))
        End Sub


        <Browsable(False)> _
        Public ReadOnly Property AuthenticationReady As Boolean
            Get
                Dim auth = AuthenticationSettings.GetValue()
                If auth IsNot Nothing Then
                    Return auth.Enabled()
                End If
                Return False
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property Authorized As Boolean
            Get
                Dim auth = AuthorizationToken.GetValue()
                If auth IsNot Nothing Then
                    Return auth.Status = OAuth2TokenStatus.Authorized
                End If
                Return False
            End Get
        End Property

        Public Property AuthenticationSettings As New SimpleOrExpression(Of GoogleOAuth2Info)

        Public Property AuthorizationToken As New SimpleOrExpression(Of OAuth2AccessToken)

        <ConditionalVisible("AuthenticationReady", False, True)> _
        <ActionButton(IconName.Certificate, IconOptions.Normal)> _
        Public Sub InitializeToken(ByVal pe As AriciePropertyEditorControl)
            Me.AuthorizationToken.GetValue().OAuthParams = Me.AuthenticationSettings.GetValue().GetParameters()
            pe.DisplayLocalizedMessage("OAuth2TokenInitialized.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
        End Sub

        Public Sub Init(actionContext As PortalKeeperContext(Of TEngineEvents))
            If GetType(SpreadsheetsService).IsAssignableFrom(GetType(T)) Then
                Dim authSettings As GoogleOAuth2Info = Me.AuthenticationSettings.GetValue(actionContext, actionContext)
                Dim objService = New SpreadsheetsService(authSettings.ApplicationName)
                objService.RequestFactory = Me.AuthorizationToken.GetValue(actionContext, actionContext).GetFactory(authSettings.ApplicationName)
                _Service = DirectCast(DirectCast(objService, Object), T)
            End If
        End Sub

        Private _Service As T

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property Service As T
            Get
                Return _Service
            End Get
        End Property

    End Class
End NameSpace