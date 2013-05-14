Imports System.Text.RegularExpressions
Imports System.Text

Namespace Services
    ''' <summary>
    ''' Loop processor for Advanced Token Replace
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LoopTokenProcessor

        Public Sub New(ByVal listToken As String, ByVal enumerable As IEnumerable)
            Me._ListToken = listToken
            Me._Enumerable = enumerable
        End Sub

        ''' <summary>
        ''' Token list for the processor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ListToken() As String = String.Empty
        ''' <summary>
        ''' Enumerable for loop processing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Enumerable() As IEnumerable

        Private _SubTokens As New List(Of SubToken)
        ''' <summary>
        ''' List of subtokens that will be applied inside of the loop
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SubTokens() As List(Of SubToken)
            Get
                Return _SubTokens
            End Get
        End Property

        ''' <summary>
        ''' Adds a subtoken match 
        ''' </summary>
        ''' <param name="match"></param>
        ''' <remarks></remarks>
        Public Sub AddMatch(ByVal match As Match)
            Me._SubTokens.Add(New SubToken(match))
        End Sub

        ''' <summary>
        ''' Adds a subtoken processor
        ''' </summary>
        ''' <param name="subProcessor"></param>
        ''' <remarks></remarks>
        Public Sub AddSubProcessor(ByVal subProcessor As LoopTokenProcessor)
            Me._SubTokens.Add(New SubToken(subProcessor))
        End Sub



        
        ''' <summary>
        ''' Runs the loop with the configured data
        ''' </summary>
        ''' <param name="atr"></param>
        ''' <param name="builder"></param>
        ''' <remarks></remarks>
        Public Sub RunProcessor(ByVal atr As AdvancedTokenReplace, ByVal builder As StringBuilder)
            For Each obj As Object In Me.Enumerable

                Dim newPrefix1 As String = Me.ListToken.Replace(":"c, "-"c)
                Dim newPrefix2 As String = "CurrentItem"
                atr.SetObjectReplace(obj, newPrefix1)
                atr.SetObjectReplace(obj, newPrefix2)

                Dim tempSB As New StringBuilder

                For Each objSubToken As SubToken In Me.SubTokens
                    If objSubToken.Match.Groups("InnerToken").Success Then

                        Dim strPropertyName As String = objSubToken.Match.Groups("property").Value
                        Dim strFormat As String = objSubToken.Match.Groups("format").Value
                        Dim defaultValue As String = objSubToken.Match.Groups("ifEmpty").Value
                        Dim strValue As String = atr.OnReplacedTokenValue(newPrefix1, strPropertyName, strFormat)
                        If ((defaultValue.Length > 0) AndAlso (strValue.Length = 0)) Then
                            strValue = defaultValue
                        End If
                        tempSB.Append(strValue)

                    Else
                        tempSB.Append(objSubToken.Match.Value)
                    End If
                Next
                builder.Append(atr.ReplaceAllTokens(tempSB.ToString))
            Next

        End Sub


    End Class
End Namespace