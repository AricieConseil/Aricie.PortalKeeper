Imports DotNetNuke.UI.WebControls

Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class EditOnlyAttribute
        Inherits SingleEditorModeAttribute

        Public Sub New()
            MyBase.New(PropertyEditorMode.Edit)
        End Sub

    End Class
End NameSpace