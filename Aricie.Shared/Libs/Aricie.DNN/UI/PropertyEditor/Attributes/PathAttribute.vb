

Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class PathAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal path As String)
            Me._path = path
        End Sub


        ' Properties
        Public ReadOnly Property Path() As String
            Get
                Return Me._path
            End Get
        End Property


        ' Fields
        Private _path As String = String.Empty
    End Class
End Namespace
