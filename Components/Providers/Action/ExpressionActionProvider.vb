Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Ciloci.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
        <System.ComponentModel.DisplayName("Expression Action Provider")> _
        <Description("This provider runs by evaluating a boolean expression and returning the result. The current PortalKeeperContext can be used as a dedicated variable")> _
    Public Class ExpressionActionProvider(Of TEngineEvents As IConvertible)
        Inherits ActionProvider(Of TEngineEvents)



        <ExtendedCategory("Specifics")> _
        Public Property FleeExpression() As New FleeExpressionInfo(Of Boolean)

        <ExtendedCategory("Specifics")> _
        Public Property ContextVarName As String = "Context"

        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Dim newLookup As New SimpleContextLookup(actionContext)
            newLookup.Items(Me.ContextVarName) = actionContext
            Return Me._FleeExpression.Evaluate(actionContext, newLookup)
        End Function

    End Class
End Namespace