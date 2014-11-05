Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports System.Runtime.Remoting.Contexts
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Ticket, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Log Event Action")> _
        <Description("Inserts a new log in the DotNetNuke event log. Automatic email alert can be configured accordingly")> _
    Public Class LogEventAction(Of TEngineEvents As IConvertible)
        Inherits MessageBasedAction(Of TEngineEvents)



        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal enableTokenReplace As Boolean)
            MyBase.New(enableTokenReplace)
        End Sub

        Private _LogTypeKey As String = "AdminAlert"



        <ExtendedCategory("Specifics")> _
         <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
           <Selector(GetType(LogTypeSelector), "LogTypeFriendlyName", "LogTypeKey", False, False, "", "", True, False)> _
        Public Property LogTypeKey() As String
            Get
                Return _LogTypeKey
            End Get
            Set(ByVal value As String)
                _LogTypeKey = value
            End Set
        End Property







        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim objLog As New LogInfo()
            objLog.LogTypeKey = Me._LogTypeKey
            objLog.LogPortalID = actionContext.DnnContext.Portal.PortalId
            objLog.LogServerName = actionContext.DnnContext.ServerName
            objLog.LogUserID = actionContext.DnnContext.User.UserID
            Dim message As String = GetMessage(actionContext)
            objLog.LogProperties.Add(New LogDetailInfo("Message", message))
            Dim ctl As New LogController
            ctl.AddLog(objLog)
            Return True
        End Function
    End Class
End Namespace
