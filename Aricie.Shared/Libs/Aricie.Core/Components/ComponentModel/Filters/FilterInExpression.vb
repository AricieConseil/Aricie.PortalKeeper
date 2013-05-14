Namespace Business.Filters
    Public Structure FilterInExpression
        Public Sub New(ByVal filter As IFilter, ByVal op As OperatorFilterExp)
            Me.Filter = filter
            Me.OperatorFilterExp = op
        End Sub

        Public Filter As IFilter
        Public OperatorFilterExp As OperatorFilterExp
    End Structure
End NameSpace