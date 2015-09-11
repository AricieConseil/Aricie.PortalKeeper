Imports System.Text.RegularExpressions

Namespace Text
    Public Class EventBasedMatchEvaluator


        Public Event Evaluate As EventHandler(Of MatchEvaluatorEventArgs)

        Public Function GetEvaluator() As MatchEvaluator
            Return AddressOf OnEvaluate
        End Function

        Public Function OnEvaluate(ByVal match As Match) As String
            Dim args As New MatchEvaluatorEventArgs(match)
            RaiseEvent Evaluate(Me, args)
            Return args.Replacement
        End Function


    End Class
End NameSpace