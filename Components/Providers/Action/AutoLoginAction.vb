Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Web.Configuration
Imports Aricie.ComponentModel
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.Entities
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Utilities
Imports System.Globalization
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.SignIn, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Auto Login")> _
        <Description("Logs the current user with predefined credentials")> _
    Public Class AutoLoginAction
        Inherits ActionProvider(Of RequestEvent)

        Private _CurrentAutoLoginInfo As New AutoLoginInfo

        Private _AutoLoginUserName As String

        <ExtendedCategory("Specifics")> _
        Public Property CurrentAutoLoginInfo() As AutoLoginInfo
            Get
                Return _CurrentAutoLoginInfo
            End Get
            Set(ByVal value As AutoLoginInfo)
                _CurrentAutoLoginInfo = value
            End Set
        End Property

        Public Function DecryptTicket(ByVal Value As String, ByVal encryptionKey As String) As String
            Dim objSecurity As New PortalSecurity

            '[DNN-8257] - Can't do URLEncode/URLDecode as it introduces issues on decryption (with / = %2f), so we use a modifed Base64
            Value = Value.Replace("_", "/")
            Value = Value.Replace("-", "+")
            Value = Value.Replace("%3d", "=")

            Return objSecurity.Decrypt(encryptionKey, Value)
        End Function


        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim ps As New PortalSecurity
            If actionContext.DnnContext.IsAuthenticated Then
                ps.SignOut()
            End If
            Dim status As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
            If Me.CurrentAutoLoginInfo.AutoLoginMode = AutoLoginInfo.AutoLoginModeType.Manual Then
                Dim objUser As UserInfo = UserController.UserLogin(actionContext.DnnContext.Portal.PortalId, Me.CurrentAutoLoginInfo.AutoLoginUserName, _
                                           Me.CurrentAutoLoginInfo.AutoLoginPassword, "", actionContext.DnnContext.Portal.PortalName, _
                                           actionContext.DnnContext.Request.UserHostAddress, status, False)
                If objUser IsNot Nothing AndAlso status = UserLoginStatus.LOGIN_SUCCESS Then
                    HttpContext.Current.Items("UserInfo") = objUser
                End If
            Else
                ' On récupère le paramètre dans la requête
                Dim ticketEncryptedValue As String
                Dim ticketDecryptedValue As String
                If actionContext.DnnContext.Request.Params(Me.CurrentAutoLoginInfo.TicketAuthParamName) IsNot Nothing Then
                    ticketEncryptedValue = actionContext.DnnContext.Request.Params(Me.CurrentAutoLoginInfo.TicketAuthParamName).ToString

                    ' On le décrypte avec la clé fourni sinon avec la clé par défaut
                    If Me.CurrentAutoLoginInfo.EncryptionKey IsNot Nothing AndAlso Me.CurrentAutoLoginInfo.EncryptionKey <> "" Then
                        ticketDecryptedValue = Me.DecryptTicket(ticketEncryptedValue, Me.CurrentAutoLoginInfo.EncryptionKey)
                    Else
                        ' La clé de décryptage n'est pas renseigné, on utilise la clé par défaut de DNN
                        Dim configSection As New MachineKeySection()
                        ticketDecryptedValue = Me.DecryptTicket(ticketEncryptedValue, configSection.DecryptionKey)
                    End If

                    'Récupération des informations - On pourrait rajouter la possibilité de choisir le séprateur dans la config de l'action - A voir
                    Dim ticketValues As String()
                    Dim username As String = ""
                    Dim ticketExpiredDate As String = ""
                    ticketValues = ticketDecryptedValue.Split("$"c)
                    If ticketValues.Length > 1 Then
                        username = ticketValues(0)
                        ticketExpiredDate = ticketValues(1)
                    End If

                    If Not String.IsNullOrEmpty(username) AndAlso Not String.IsNullOrEmpty(ticketExpiredDate) Then
                        Dim expiredDate As Date = Convert.ToDateTime(ticketExpiredDate, CultureInfo.InvariantCulture)
                        If expiredDate <> Nothing AndAlso expiredDate.Ticks > Date.Now.Ticks Then
                            Dim objUser As UserInfo
                            objUser = UserController.GetUserByName(actionContext.DnnContext.Portal.PortalId, username)
                            'On fait l'authentification comme d'hab !
                            If objUser IsNot Nothing Then
                                UserController.UserLogin(actionContext.DnnContext.Portal.PortalId, objUser, actionContext.DnnContext.Portal.PortalName, actionContext.DnnContext.Request.UserHostAddress, False)
                                HttpContext.Current.Items("UserInfo") = objUser
                                status = UserLoginStatus.LOGIN_SUCCESS
                            End If
                        End If
                    End If
                End If
            End If

            Return status = UserLoginStatus.LOGIN_SUCCESS OrElse status = UserLoginStatus.LOGIN_SUPERUSER
        End Function


    End Class
End Namespace