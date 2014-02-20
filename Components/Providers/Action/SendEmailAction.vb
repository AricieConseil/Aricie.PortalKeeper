Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Mail
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.Text
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.EnvelopeO, IconOptions.Normal)> _
   <Serializable()> _
       <System.ComponentModel.DisplayName("Send Email Action")> _
       <Description("Sends an email to a list of destination addresses. Token replace is available for all text fields and target address list.")> _
    Public Class SendEmailAction(Of TEngineEvent As IConvertible)
        Inherits MessageBasedAction(Of TEngineEvent)



        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal enableTokenReplace As Boolean)
            MyBase.New(enableTokenReplace)
        End Sub



        <ExtendedCategory("Specifics")>
        Public Property FromAddress As String = ""

        <Editor(GetType(CustomTextEditControl), GetType(EditControl)), LineCount(4), Width(500), ExtendedCategory("Specifics")>
        Public Property ToAddresses As String = ""

        <Editor(GetType(CustomTextEditControl), GetType(EditControl)), Width(500), ExtendedCategory("Specifics")>
        Public Property EmailObject As String = ""

        <ExtendedCategory("Specifics")>
        Public Property EmailPriority As MailPriority = MailPriority.Normal

        <ExtendedCategory("Specifics")>
        Public Property BodyFormat As MailFormat = MailFormat.Html

        <ExtendedCategory("Specifics")>
        Public Property Encoding As SimpleEncoding = SimpleEncoding.UTF8


        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvent), ByVal aSync As Boolean) As Boolean
            Dim message As String = GetMessage(actionContext)
            Dim mailObject As String = GetMessage(actionContext, Me.EmailObject)
            Dim targetAddresses As String = GetMessage(actionContext, Me.ToAddresses)
            Dim fromAddress As String = Me.FromAddress
            If fromAddress = "" Then
                fromAddress = NukeHelper.PortalSettings.Email
                If String.IsNullOrEmpty(fromAddress) Then
                    fromAddress = DirectCast(NukeHelper.PortalSettings.HostSettings("HostEmail"), String)
                End If
                'fromAddress = DotNetNuke.Entities.Users.UserController.GetUser(NukeHelper.PortalSettings.PortalId, NukeHelper.PortalSettings.AdministratorId, True).Email
            End If
            DotNetNuke.Services.Mail.Mail.SendMail(fromAddress, targetAddresses, "", "", Me.EmailPriority, mailObject, Me.BodyFormat, EncodingHelper.GetEncoding(Me.Encoding), message, _
                                                   "", "", "", "", "")
            Return True
        End Function
    End Class
End Namespace

