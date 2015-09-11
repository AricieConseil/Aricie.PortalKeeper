Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.Services.Flee

Namespace Entities
    Public Class FilteredString


        Public Property Source As New SimpleExpression(Of String)

        Public Property Filter As New ExpressionFilterInfo()


        Public Function Process(ByVal owner As Object, ByVal items As Aricie.DNN.Services.IContextLookup) As String
            Return Filter.Process(Source.Evaluate(owner, items), items)
        End Function


    End Class
End NameSpace