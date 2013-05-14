Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class UserControlAttribute
        Inherits Attribute

        Public Sub New(ByVal userControlPath As String)

            Me.UserControlPath = userControlPath

        End Sub

        Public Property UserControlPath As String

    End Class

End Namespace
