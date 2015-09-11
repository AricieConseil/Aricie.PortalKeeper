

Namespace Services.Filtering

    ''' <summary>
    ''' Class that runs the ExpressionFilterInfo transformation on a input
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("Use Process method Directly")> _
    Public Class StringEscaper



        Private _Filter As ExpressionFilterInfo
        'Private _DefaultCharReplacement As String = Nothing


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


        ' ''' <summary>
        ' ''' returns default char replacement
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        Public Function GetDefaultCharReplacement() As String
            Return Me.Filter.DefaultCharReplacement()
        End Function
        '    If _DefaultCharReplacement Is Nothing Then
        '        If Me.Filter.CharsMap.ContainsKey(" "c) Then
        '            _DefaultCharReplacement = Me.Filter.CharsMap(" "c).ToString
        '        ElseIf Not Me.Filter.StringMap.TryGetValue(" ", _DefaultCharReplacement) Then
        '            _DefaultCharReplacement = "-"
        '        End If
        '    End If
        '    Return _DefaultCharReplacement
        'End Function

        ''' <summary>
        ''' Escapes the string passed as the parameter according to the rules defined in the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="originalString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EscapeString(ByVal originalString As String) As String


            Return Me.Filter.Process(originalString)

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
