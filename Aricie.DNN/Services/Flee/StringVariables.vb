Imports System.Collections.Specialized
Imports Aricie.Collections

Namespace Services.Flee
    
    Public Class StringVariables
        Inherits Variables(Of String)

        Public Function EvaluateToNameValueCollection(ByVal owner As Object, ByVal globalVars As IContextLookup) As NameValueCollection
            Dim tempDico As SerializableDictionary(Of String, Object) = Me.EvaluateVariables(owner, globalVars)
            Dim toReturn As New NameValueCollection(tempDico.Count)
            For Each tempPair As KeyValuePair(Of String, Object) In tempDico
                toReturn.Add(tempPair.Key, tempPair.Value.ToString())
            Next
            Return toReturn
        End Function

    End Class
End NameSpace