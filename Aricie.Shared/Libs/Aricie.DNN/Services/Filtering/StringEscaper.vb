Imports System.Web
Imports System.Text.RegularExpressions
Imports System.Text

Namespace Services.Filtering
    ''' <summary>
    ''' Class that runs the ExpressionFilterInfo transformation on a input
    ''' </summary>
    ''' <remarks></remarks>
    Public Class StringEscaper



        Private _Filter As ExpressionFilterInfo
        Private _DefaultCharReplacement As String = Nothing


#Region "cTor"

        ''' <summary>
        ''' Builds the string escaper for the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="filter"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal filter As ExpressionFilterInfo)
            Me._Filter = filter
        End Sub

#End Region

        ''' <summary>
        ''' The Filter that will be executed against the input
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Filter() As ExpressionFilterInfo
            Get
                Return _Filter
            End Get
            Set(ByVal value As ExpressionFilterInfo)
                _Filter = value
            End Set
        End Property


        ''' <summary>
        ''' returns default char replacement
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDefaultCharReplacement() As String
            If _DefaultCharReplacement Is Nothing Then
                If Me.Filter.CharsMap.ContainsKey(" "c) Then
                    _DefaultCharReplacement = Me.Filter.CharsMap(" "c).ToString
                ElseIf Not Me.Filter.StringMap.TryGetValue(" ", _DefaultCharReplacement) Then
                    _DefaultCharReplacement = "-"
                End If
            End If
            Return _DefaultCharReplacement
        End Function

        ''' <summary>
        ''' Escapes the string passed as the parameter according to the rules defined in the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="originalString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EscapeString(ByVal originalString As String) As String

            Dim toReturn As String
            Dim maxLength As Integer = Me.Filter.MaxLength
            Dim noMaxLentgth As Boolean = (maxLength = -1)

            'encode preprocessing
            Select Case Me.Filter.EncodePreProcessing
                Case EncodeProcessing.HtmlEncode
                    toReturn = HttpUtility.HtmlEncode(originalString)
                Case EncodeProcessing.HtmlDecode
                    toReturn = HttpUtility.HtmlDecode(originalString)
                Case EncodeProcessing.UrlEncode
                    toReturn = HttpUtility.UrlEncode(originalString)
                Case EncodeProcessing.UrlDecode
                    toReturn = HttpUtility.UrlDecode(originalString)
                Case Else
                    toReturn = originalString
            End Select

            'forceLower
            If Me.Filter.ForceToLower Then
                toReturn = toReturn.ToLowerInvariant()
            End If

            'dealing with regexs
            For Each objRegexReplace As KeyValuePair(Of Regex, String) In Me.Filter.RegexMap
                If String.IsNullOrEmpty(objRegexReplace.Value) Then
                    toReturn = objRegexReplace.Key.Replace(toReturn, String.Empty)
                Else
                    toReturn = objRegexReplace.Key.Replace(toReturn, objRegexReplace.Value)
                End If

            Next

            'dealing with string replaces
            For Each objStringReplace As KeyValuePair(Of String, String) In Me.Filter.StringMap
                If String.IsNullOrEmpty(objStringReplace.Value) Then
                    toReturn = toReturn.Replace(objStringReplace.Key, String.Empty)
                Else
                    toReturn = toReturn.Replace(objStringReplace.Key, objStringReplace.Value)
                End If

            Next

            'dealing with char replaces
            If Me.Filter.CharsMap.Count > 0 Then

                Dim toReturnBuilder As New StringBuilder()

                Dim sourceChar, replaceChar As Char
                Dim i As Integer = 0
                Dim previousChar As Char = Char.MinValue
                Dim previousCharIsReplace As Boolean = False
                While i < toReturn.Length AndAlso (noMaxLentgth OrElse toReturnBuilder.Length <= maxLength)
                    sourceChar = toReturn(i)
                    If Me.Filter.CharsMap.TryGetValue(sourceChar, replaceChar) Then
                        If replaceChar <> Char.MinValue Then
                            If replaceChar <> previousChar OrElse replaceChar <> GetDefaultCharReplacement() Then
                                previousChar = replaceChar
                                toReturnBuilder.Append(replaceChar)
                            End If
                        End If
                    Else
                        previousChar = sourceChar
                        toReturnBuilder.Append(sourceChar)
                    End If

                    i += 1
                End While

                toReturn = toReturnBuilder.ToString()

            End If

            'limited Length
            If Not noMaxLentgth AndAlso toReturn.Length > maxLength Then
                toReturn = toReturn.Substring(0, maxLength)
            End If

            Return toReturn

        End Function

        ''' <summary>
        ''' Equality comparison
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim objToCompare As StringEscaper = TryCast(obj, StringEscaper)
            If objToCompare Is Nothing Then
                Return False
            End If
            Return objToCompare.Filter.Equals(Me.Filter)
        End Function

    End Class
End Namespace
