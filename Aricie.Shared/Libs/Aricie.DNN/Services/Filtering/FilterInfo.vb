Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering

    <Serializable()> _
    Public Class FilterInfo

        Private _expression As New ExpressionFilterInfo
        Private _modules As New List(Of FilterModuleInfo)

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top), SortOrder(0)> _
        Public Property Expression() As ExpressionFilterInfo
            Get
                Return _expression
            End Get
            Set(ByVal value As ExpressionFilterInfo)
                _expression = value
            End Set
        End Property

        <Editor(GetType(ListEditControl), GetType(EditControl)), _
            LabelMode(LabelMode.Top), SortOrder(1)> _
        Public Property Modules() As List(Of FilterModuleInfo)
            Get
                Return _modules
            End Get
            Set(ByVal value As List(Of FilterModuleInfo))
                _modules = value
            End Set
        End Property
    End Class
End Namespace
