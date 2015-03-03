Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
        <DisplayName("Expression Action Provider")> _
        <Description("This provider runs by evaluating a boolean expression and returning the result, or a IActionPrvoider expression, running and returning the result of the corresponding provider.")> _
    Public Class ExpressionActionProvider(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        <ExtendedCategory("Specifics")> _
        Public Property IsActionProvider As Boolean

        <ConditionalVisible("IsActionProvider", True, True)>
        <ExtendedCategory("Specifics")> _
        Public Property FleeExpression() As New FleeExpressionInfo(Of Boolean)

        <ConditionalVisible("IsActionProvider", False, True)>
        <ExtendedCategory("Specifics")> _
        Public Property ActionProviderExpression As New FleeExpressionInfo(Of IActionProvider(Of TEngineEvents))



        <ExtendedCategory("Specifics")> _
        Public Property ContextVarName As String = "Context"


        Protected Overloads Overrides Function Run(actionContext As PortalKeeperContext(Of TEngineEvents), aSync As Boolean) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim newLookup As New SimpleContextLookup(actionContext)
            newLookup.Items(Me.ContextVarName) = actionContext
            If Me.IsActionProvider Then
                Dim objProvider As IActionProvider(Of TEngineEvents) = Me.ActionProviderExpression.Evaluate(actionContext, newLookup)
                Return objProvider.Run(actionContext)
            Else
                Return Me._FleeExpression.Evaluate(actionContext, newLookup)
            End If
        End Function
    End Class
End Namespace