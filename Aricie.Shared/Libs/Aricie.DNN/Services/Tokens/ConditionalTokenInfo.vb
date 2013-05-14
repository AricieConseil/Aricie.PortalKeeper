Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering
    ''' <summary>
    ''' Conditional token holder class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class ConditionalTokenInfo
        Public Property Key() As String = String.Empty

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <Orientation(Orientation.Vertical)> _
        Public Property ConditionalToken() As New ConditionalTokenSourceInfo
    End Class
End Namespace