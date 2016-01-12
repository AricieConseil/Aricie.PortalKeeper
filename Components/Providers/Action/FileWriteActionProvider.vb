Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.FloppyO, IconOptions.Normal)>
    <DisplayName("File Write Action")>
    <Description("This provider allows to write a content to a file, given its path and the content to write by dynamic expressions")>
    Public Class FileWriteActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileWriteActionProviderBase(Of TEngineEvents)




        Private _InputExpression As New FleeExpressionInfo(Of String)

        'Private _InputEntityExpression As New FleeExpressionInfo(Of IEnumerable)







        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)>
        <ExtendedCategory("File")>
        Public Property InputExpression() As FleeExpressionInfo(Of String)
            Get
                Return _InputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _InputExpression = value
            End Set
        End Property





        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property InputEntityExpression() As FleeExpressionInfo(Of IEnumerable)
        '    Get
        '        Return _InputEntityExpression
        '    End Get
        '    Set(ByVal value As FleeExpressionInfo(Of IEnumerable))
        '        _InputEntityExpression = value
        '    End Set
        'End Property

        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property InputEntityVarName As String = "CurrentInput"

        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        '<Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
        '  LineCount(4), Width(500)> _
        '  <SortOrder(1000)> _
        'Public Property FieldVars() As String = String.Empty









        Public Overrides Function GetContent(actionContext As PortalKeeperContext(Of TEngineEvents)) As String
            Return Me._InputExpression.Evaluate(actionContext, actionContext)
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(Boolean)
        End Function
    End Class
End Namespace