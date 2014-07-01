Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Code, IconOptions.Normal)> _
    <DisplayName("Dynamic Expression Condition")> _
   <Description("Matches according to the evaluation of a dynamic code expression defined in the Flee language")> _
   <Serializable()> _
    Public Class FleeExpressionCondition(Of TEngineEvents As IConvertible)
        Inherits ConditionProvider(Of TEngineEvents)
        Implements IExpressionVarsProvider



        Private _FleeExpression As New FleeExpressionInfo(Of Boolean)

        <ExtendedCategory("Specifics")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property FleeExpression() As FleeExpressionInfo(Of Boolean)
            Get
                Return _FleeExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of Boolean))
                _FleeExpression = value
            End Set
        End Property

        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return Me._FleeExpression.Evaluate(context, context)
        End Function


        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            Me.FleeExpression.AddVariables(currentProvider, existingVars)
        End Sub
    End Class
End Namespace