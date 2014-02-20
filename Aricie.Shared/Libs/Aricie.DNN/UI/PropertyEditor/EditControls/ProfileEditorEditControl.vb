Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls
    Public Class ProfileEditorEditControl
        Inherits PropertyEditorEditControl

        Protected Overrides Function GetNewEditor() As DotNetNuke.UI.WebControls.PropertyEditorControl
            Return New ProfileEditorControl
        End Function

    End Class
End NameSpace