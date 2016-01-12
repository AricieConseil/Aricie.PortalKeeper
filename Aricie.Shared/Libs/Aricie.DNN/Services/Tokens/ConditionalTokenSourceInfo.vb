Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering
    ''' <summary>
    ''' Holder class for conditional token source info
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Class ConditionalTokenSourceInfo
        <Editor(GetType(DictionaryEditControl), GetType(EditControl))> _
            <KeyEditor(GetType(CustomTextEditControl), GetType(TokenSourceInfo.TokenValueAttributes))> _
            <ValueEditor(GetType(PropertyEditorEditControl))> _
            <LabelMode(LabelMode.Top), Orientation(Orientation.Vertical)> _
        Public Property ConditionalSources() As New SerializableDictionary(Of String, TokenSourceInfo)
    End Class
End Namespace