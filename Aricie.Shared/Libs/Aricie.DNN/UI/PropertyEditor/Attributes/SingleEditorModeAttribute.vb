Imports DotNetNuke.UI.WebControls

Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class SingleEditorModeAttribute
        Inherits Attribute

        Public Sub New()

        End Sub

        Public Sub New(editMode As PropertyEditorMode)
            Me.EditorMode = editMode
        End Sub

        Public Property EditorMode As PropertyEditorMode

    End Class
End NameSpace