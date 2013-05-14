

Namespace ComponentModel
    ''' <summary>
    ''' Attribute to flag main Category within the Component Model Category Attributes of a Class
    ''' </summary>
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class MainCategoryAttribute
        Inherits Attribute
        ' Methods
        Public Sub New()

        End Sub
    End Class
End Namespace
