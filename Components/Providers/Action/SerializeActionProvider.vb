Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
'Imports Jayrock.Json.Conversion
'Imports Jayrock.Json
Imports DotNetNuke.UI.WebControls

Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Pencil, IconOptions.Normal)> _
   <Serializable()> _
       <DisplayName("Serialize Action Provider")> _
       <Description("This provider allows to serialize a given entity, result of dynamic expression, into a string")> _
    Public Class SerializeActionProvider(Of TEngineEvents As IConvertible)
        Inherits SerializeActionProviderBase(Of TEngineEvents)



        Private _inputExpression As New FleeExpressionInfo(Of Object)

        <ExtendedCategory("Serialization")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property InputExpression() As FleeExpressionInfo(Of Object)
            Get
                Return _inputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of Object))
                _inputExpression = value
            End Set
        End Property





        'Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
        '    Return Serialize(actionContext)
        'End Function

        Public Overrides Function GetContent(actionContext As PortalKeeperContext(Of TEngineEvents)) As Object
            Return _inputExpression.Evaluate(actionContext, actionContext)
        End Function
    End Class
End Namespace