Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Mail
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.Text

Namespace Aricie.DNN.Modules.PortalKeeper
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

        Private _FromAddress As String = ""

        Private _ToAddresses As String = ""

        Private _EmailObject As String = ""

        Private _EmailPriority As MailPriority = MailPriority.Normal

        Private _BodyFormat As MailFormat = MailFormat.Html

        Private _Encoding As SimpleEncoding

        <ExtendedCategory("Specifics")> _
        Public Property FromAddress() As String
            Get
                Return _FromAddress
            End Get
            Set(ByVal value As String)
                _FromAddress = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        <Width(500)> _
            <LineCount(4)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property ToAddresses() As String
            Get
                Return _ToAddresses
            End Get
            Set(ByVal value As String)
                _ToAddresses = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        <Width(500)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property EmailObject() As String
            Get
                Return _EmailObject
            End Get
            Set(ByVal value As String)
                _EmailObject = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        Public Property EmailPriority() As MailPriority
            Get
                Return _EmailPriority
            End Get
            Set(ByVal value As MailPriority)
                _EmailPriority = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        Public Property BodyFormat() As MailFormat
            Get
                Return _BodyFormat
            End Get
            Set(ByVal value As MailFormat)
                _BodyFormat = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        Public Property Encoding() As SimpleEncoding
            Get
                Return _Encoding
            End Get
            Set(ByVal value As SimpleEncoding)
                _Encoding = value
            End Set
        End Property


        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvent), ByVal aSync As Boolean) As Boolean
            Dim message As String = GetMessage(actionContext)
            Dim mailObject As String = GetMessage(actionContext, Me._EmailObject)
            Dim targetAddresses As String = GetMessage(actionContext, Me._ToAddresses)
            Dim fromAddress As String = Me._FromAddress
            If fromAddress = "" Then
                fromAddress = NukeHelper.PortalSettings.Email
                If String.IsNullOrEmpty(fromAddress) Then
                    fromAddress = DirectCast(NukeHelper.PortalSettings.HostSettings("HostEmail"), String)
                End If
                'fromAddress = DotNetNuke.Entities.Users.UserController.GetUser(NukeHelper.PortalSettings.PortalId, NukeHelper.PortalSettings.AdministratorId, True).Email
            End If
            DotNetNuke.Services.Mail.Mail.SendMail(fromAddress, targetAddresses, "", "", Me._EmailPriority, mailObject, Me._BodyFormat, EncodingHelper.GetEncoding(Me._Encoding), message, _
                                                   "", "", "", "", "")
            Return True
        End Function
    End Class
End Namespace