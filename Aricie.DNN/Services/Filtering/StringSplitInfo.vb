Imports Aricie.Business.Filters
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes

Namespace Services.Filtering
    Public Class StringSplitInfo




        Public Sub New()

        End Sub

        Public Sub New(strSeparator As String)
            Separators.One = strSeparator
        End Sub


        Public Property LineSplit() As Boolean

        <ConditionalVisible("LineSplit", True)>
        Public Property Separators As New OneOrMore(Of string) (",")


        Public Property EmptyEntries As StringSplitOptions = StringSplitOptions.RemoveEmptyEntries

        Public  Property ReturnList As Boolean

        <ConditionalVisible("ReturnList")>
        Public  Property FilterStrings As New EnabledFeature(Of ExpressionFilterInfo)

         Public Function Process(ByVal originalString As String) As IEnumerable(Of String)
            Return Process(originalString, Nothing)
         End Function

        Public Function Process(ByVal originalString As String, contextVars As IContextLookup) As IEnumerable(Of String)
            Dim toReturn As IEnumerable(Of String)
            If LineSplit Then
                toReturn = originalString.Split(New String() {vbCrLf, vbLf}, EmptyEntries)
            Else
                toReturn = originalString.Split(Separators.All.ToArray(), EmptyEntries)
            End If
            If ReturnList Then
                toReturn = New List(Of String)(toReturn)
                if FilterStrings.Enabled
                    toReturn = FilterStrings.Entity.ProcessList(toReturn)
                End If
            End If
            Return toReturn
        End Function

        Public Function GetOutputType() As Type
            If ReturnList Then
                Return GetType(List(Of String))
            Else
                Return GetType(String())
            End If
        End Function

    End Class
End NameSpace