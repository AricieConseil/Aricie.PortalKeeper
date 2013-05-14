

Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class WidthAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal width As Integer)
            Me._Width = width
        End Sub


        ' Properties
        Public ReadOnly Property Width() As Integer
            Get
                Return Me._Width
            End Get
        End Property


        ' Fields
        Private _Width As Integer = 0
    End Class
End Namespace
