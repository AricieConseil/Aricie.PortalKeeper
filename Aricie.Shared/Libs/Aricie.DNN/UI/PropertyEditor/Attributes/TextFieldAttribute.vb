Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class TextFieldAttribute
        Inherits Attribute

        Private _text As String

        Public Sub New(ByVal text As String)
            _text = text
        End Sub

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property


    End Class

End Namespace
