Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class ValueFieldAttribute
        Inherits Attribute

        Private _value As String
        Private _typeCode As TypeCode

        Public Sub New(ByVal value As String, ByVal typeCode As TypeCode)
            _value = value
            _typeCode = typeCode
        End Sub

        Public Property Value() As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

        Public Property TypeCode() As TypeCode
            Get
                Return _typeCode
            End Get
            Set(ByVal value As TypeCode)
                _typeCode = value
            End Set
        End Property

    End Class

End Namespace
