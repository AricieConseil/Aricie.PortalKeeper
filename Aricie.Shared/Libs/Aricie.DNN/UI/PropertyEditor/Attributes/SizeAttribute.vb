Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class SizeAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal Size As Integer)
            Me._Size = Size
        End Sub


        ' Properties
        Public ReadOnly Property Size() As Integer
            Get
                Return Me._Size
            End Get
        End Property


        ' Fields
        Private _Size As Integer = 0
    End Class
End Namespace
