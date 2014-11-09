Imports System.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee

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

        <ExtendedCategory("Specifics")> _
         Public Property CurrentRequestUrl As Boolean

        <ConditionalVisible("CurrentRequestUrl", True, True)> _
        <ExtendedCategory("Specifics")> _
        Public Property TargetUrl() As New SimpleOrExpression(Of ControlUrlInfo, String)

        '<ExtendedCategory("Specifics")> _
        <Browsable(False)> _
        Public Property Target() As ControlUrlInfo
            Get
                Return Nothing
            End Get
            Set(value As ControlUrlInfo)
                Me.TargetUrl.Mode = SimpleOrExpressionMode.Simple
                Me.TargetUrl.Simple = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        Public Property AdditionalQuery As New StringVariables

        <ExtendedCategory("Specifics")> _
        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List)> _
        Public Property RemovedQuery As New List(Of String)
           


        <ExtendedCategory("Specifics")> _
        Public Property EndResponse() As Boolean
            


        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            If Me.CurrentRequestUrl Then
                RedirectManually(actionContext, actionContext.DnnContext.Request.Url.ToString())
                Return True
            Else
                Dim valuePair As KeyValuePair(Of String, ControlUrlInfo) = TargetUrl.GetValuePair(actionContext, actionContext)
                If Not valuePair.Key.IsNullOrEmpty() Then
                    RedirectManually(actionContext, valuePair.Key)
                    Return True
                End If
                If valuePair.Value IsNot Nothing Then
                    Return valuePair.Value.Redirect(actionContext.DnnContext.HttpContext, Me.AdditionalQuery.EvaluateToNameValueCollection(actionContext, actionContext), RemovedQuery)
                End If
            End If
        End Function

        Private Sub RedirectManually(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), target As String)
            If Me.AdditionalQuery.Instances.Count > 0 OrElse RemovedQuery.Count > 0 Then
                Dim targetUri As Uri = New Uri(target)
                target = targetUri.ModifyQueryString(Me.AdditionalQuery.EvaluateToNameValueCollection(actionContext, actionContext), RemovedQuery)
            End If
            actionContext.DnnContext.Response.Redirect(target, Me.EndResponse)
        End Sub

    End Class
End Namespace