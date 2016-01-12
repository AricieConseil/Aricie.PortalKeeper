Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Google.GData.Client
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class OAuth2AccessToken

        Public ReadOnly Property Status As OAuth2TokenStatus
            Get
                If OAuthParams Is Nothing Then
                    Return OAuth2TokenStatus.Init
                Else
                    If Not OAuthParams.AccessToken.IsNullOrEmpty() Then
                        Return OAuth2TokenStatus.Authorized
                    Else
                        Return OAuth2TokenStatus.PendingAuthorization
                    End If
                End If
            End Get
        End Property

        Private _AuthenticateUrl As String = ""

        <ConditionalVisible("Status", False, True, OAuth2TokenStatus.PendingAuthorization)> _
        <Editor(GetType(AricieUrlEditControl), GetType(EditControl))> _
        Public ReadOnly Property AuthenticateUrl As String
            Get
                If _AuthenticateUrl.IsNullOrEmpty() Then
                    If Me.Status = OAuth2TokenStatus.PendingAuthorization Then
                        _AuthenticateUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(OAuthParams)
                    End If
                End If
                Return _AuthenticateUrl
            End Get
        End Property

        <Required(True)> _
        <ConditionalVisible("Status", False, True, OAuth2TokenStatus.PendingAuthorization)> _
        Public Property AccessCode As String

        <ConditionalVisible("Status", False, True, OAuth2TokenStatus.Authorized)> _
        Public ReadOnly Property AccessToken As String
            Get
                Return OAuthParams.AccessToken
            End Get
        End Property

        <Browsable(False)> _
        Public Property OAuthParams As OAuth2Parameters

        <ConditionalVisible("Status", False, True, OAuth2TokenStatus.PendingAuthorization)> _
        <ActionButton(IconName.Certificate, IconOptions.Normal)> _
        Public Sub ValidateAccessCode(ByVal pe As AriciePropertyEditorControl)

            Me.OAuthParams.AccessCode = Me.AccessCode
            OAuthUtil.GetAccessToken(Me.OAuthParams)

            pe.DisplayLocalizedMessage("AccessCodeValidated.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            'pe.DisplayMessage(Me.ExpressionBuilder.GetType.AssemblyQualifiedName, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
        End Sub

        Public Function GetFactory(applicationName As String) As GOAuth2RequestFactory
            Return New GOAuth2RequestFactory(Nothing, applicationName, Me.OAuthParams)
        End Function

    End Class
End NameSpace