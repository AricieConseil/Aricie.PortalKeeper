Imports DotNetNuke.UI.WebControls

Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class ViewOnlyAttribute
        Inherits SingleEditorModeAttribute

        Public Sub New()
            MyBase.New(PropertyEditorMode.View)
        End Sub

    End Class
End NameSpace