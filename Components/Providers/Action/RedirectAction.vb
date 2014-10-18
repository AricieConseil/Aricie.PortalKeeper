Imports System.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.ExternalLink, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Redirect Action")> _
        <Description("Redirect the current client to a specified url")> _
    Public Class RedirectAction
        Inherits RedirectAction(Of RequestEvent)

    End Class


    <ActionButton(IconName.ExternalLink, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Redirect Action")> _
        <Description("Redirect the current client to a specified url")> _
    Public Class RedirectAction(Of TEngineEvents As IConvertible)
        Inherits ActionProvider(Of TEngineEvents)

        

        '<ExtendedCategory("Specifics")> _
        Public Property Target() As New ControlUrlInfo
           

        '<ExtendedCategory("Specifics")> _
        Public Property EndResponse() As Boolean
            


        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.DebuggerBreak Then
                Me.CallDebuggerBreak()
            End If
            'actionContext.DnnContext.Response.Redirect(Me._Target.UrlPath, Me._EndResponse)
            Return Me.Target.Redirect(actionContext.DnnContext.HttpContext)
        End Function


    End Class
End Namespace