Imports DotNetNuke.UI.WebControls

Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class ViewOnlyAttribute
        Inherits SingleEditorModeAttribute

        Public Sub New()
            MyBase.New(PropertyEditorMode.View)
        End Sub

    End Class


    <AttributeUsage(AttributeTargets.Property)> _
    Public Class EditOnlyAttribute
        Inherits SingleEditorModeAttribute

        Public Sub New()
            MyBase.New(PropertyEditorMode.Edit)
        End Sub

    End Class


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

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class PasswordModeAttribute
        Inherits Attribute



    End Class
End Namespace