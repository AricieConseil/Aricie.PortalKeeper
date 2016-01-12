Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
'Imports Jayrock.Json.Conversion
'Imports Jayrock.Json
Imports DotNetNuke.UI.WebControls

Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Pencil, IconOptions.Normal)>
    <DisplayName("Serialize Action Provider")>
    <Description("This provider allows to serialize a given entity, result of dynamic expression, into a string")>
    Public Class SerializeActionProvider(Of TEngineEvents As IConvertible)
        Inherits SerializeActionProviderBase(Of TEngineEvents)




        Private _inputExpression As New FleeExpressionInfo(Of Object)

        <ExtendedCategory("Serialization")>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))>
        <LabelMode(LabelMode.Top)>
        Public Property InputExpression() As FleeExpressionInfo(Of Object)
            Get
                Return _inputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of Object))
                _inputExpression = value
            End Set
        End Property

        Public Overrides Function GetContent(actionContext As PortalKeeperContext(Of TEngineEvents)) As Object
            Return _inputExpression.Evaluate(actionContext, actionContext)
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(String)
        End Function
    End Class
End Namespace