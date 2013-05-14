

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    Public Class CustomSorter (Of T)
        Implements IComparer(Of T)

        Public _Comparison As Comparison(Of T)

        Public Sub New (ByVal comparison As Comparison(Of T))
            Me._Comparison = comparison
        End Sub


        Public Function Compare (ByVal x As T, ByVal y As T) As Integer Implements IComparer(Of T).Compare
            Return Me._Comparison.Invoke (x, y)
        End Function
    End Class
End Namespace