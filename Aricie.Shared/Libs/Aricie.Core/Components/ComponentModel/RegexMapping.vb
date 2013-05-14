Namespace ComponentModel
    <Serializable()> _
    Public Class RegexMapping

        Private _pattern As String = ""
        Private _sample As String = ""


        Public Sub New()

        End Sub

        Public Sub New(ByVal pattern As String, ByVal sample As String)
            Me._pattern = pattern
            Me._sample = sample
        End Sub


        
        Public Property Pattern() As String
            Get
                Return _pattern
            End Get
            Set(ByVal value As String)
                _pattern = value
            End Set
        End Property

        Public Property Sample() As String
            Get
                Return _sample
            End Get
            Set(ByVal value As String)
                _sample = value
            End Set
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim regEx As RegexMapping = CType(obj, RegexMapping)

            Return Me.Pattern.Equals(regEx.Pattern) AndAlso Me.Sample.Equals(regEx.Sample)
        End Function
    End Class
End Namespace