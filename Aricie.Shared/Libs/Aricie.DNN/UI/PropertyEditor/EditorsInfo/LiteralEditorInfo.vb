Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditorInfos
    Public Class LiteralEditorInfo
        Inherits AricieEditorInfo

        Private _parentPropertyEditor As PropertyEditorControl

        Public Property ParentPropertyEditor() As PropertyEditorControl
            Get
                Return _parentPropertyEditor
            End Get
            Set(ByVal value As PropertyEditorControl)
                _parentPropertyEditor = value
            End Set
        End Property
    End Class
End Namespace

