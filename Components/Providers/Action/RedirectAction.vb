Imports System.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.ExternalLink, IconOptions.Normal)>
    <DisplayName("Redirect Action")>
    <Description("Redirect the current client to a specified url")>
    Public Class RedirectAction
        Inherits RedirectAction(Of RequestEvent)

    End Class


    <ActionButton(IconName.ExternalLink, IconOptions.Normal)>
    <DisplayName("Redirect Action")>
    <Description("Redirect the current client to a specified url")>
    Public Class RedirectAction(Of TEngineEvents As IConvertible)
        Inherits ActionProvider(Of TEngineEvents)

        <ExtendedCategory("Specifics")>
        Public Property CurrentRequestUrl As Boolean

        <ConditionalVisible("CurrentRequestUrl", True, True)>
        <ExtendedCategory("Specifics")>
        Public Property TargetUrl() As New SimpleOrExpression(Of ControlUrlInfo, String)

        '<ExtendedCategory("Specifics")> _
        <Browsable(False)>
        Public Property Target() As ControlUrlInfo
            Get
                Return Nothing
            End Get
            Set(value As ControlUrlInfo)
                Me.TargetUrl.Mode = SimpleOrExpressionMode.Simple
                Me.TargetUrl.Simple = value
            End Set
        End Property

        <ExtendedCategory("Specifics")>
        Public Property AdditionalQuery As New StringVariables

        <ExtendedCategory("Specifics")>
        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List)>
        Public Property RemovedQuery As New List(Of String)



        <ExtendedCategory("Specifics")>
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

        Private Sub RedirectManually(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), strTarget As String)
            If Me.AdditionalQuery.Instances.Count > 0 OrElse RemovedQuery.Count > 0 Then
                Dim targetUri As Uri = New Uri(strTarget)
                strTarget = targetUri.ModifyQueryString(Me.AdditionalQuery.EvaluateToNameValueCollection(actionContext, actionContext), RemovedQuery)
            End If
            actionContext.DnnContext.Response.Redirect(strTarget, Me.EndResponse)
        End Sub

    End Class
End Namespace