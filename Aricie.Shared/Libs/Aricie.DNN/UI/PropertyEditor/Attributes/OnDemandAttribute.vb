Namespace UI.Attributes
    <AttributeUsage(AttributeTargets.Property)> _
    Public Class OnDemandAttribute
        Inherits Attribute

        Private _Value As Boolean

        Public Sub New(value As Boolean)
            Me._Value = value
        End Sub

        Public Property Value() As Boolean
            Get
                Return _Value
            End Get
            Set(ByVal value As Boolean)
                _Value = value
            End Set
        End Property




    End Class
End Namespace