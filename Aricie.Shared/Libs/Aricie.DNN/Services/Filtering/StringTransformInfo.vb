Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports System.Security.Cryptography.Xml
Imports System.Text
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Aricie.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Services.Filtering

    ''' <summary>
    ''' Type of string filtering
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum StringFilterType
        CharsReplace
        StringReplace
        RegexReplace
    End Enum

    ''' <summary>
    ''' Class for string transformation
    ''' </summary>
    ''' <remarks></remarks>
    
    <DefaultProperty("FriendlyName")> _
    Public Class StringTransformInfo


        Public Sub New()
        End Sub

        Public Sub New(ByVal filterType As StringFilterType, ByVal sourcevalue As String, ByVal replacevalue As String)
            Me.New()
            Me._FilterType = filterType
            Me.Source.Simple = sourcevalue
            Me.Replace.Simple = replacevalue
        End Sub

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                Return String.Format("{1} {0} {2} {0} {3}", Aricie.ComponentModel.UIConstants.TITLE_SEPERATOR, Me.FilterType.ToString(), Source.FriendlyName, Replace.FriendlyName)
            End Get
        End Property

        ''' <summary>
        ''' Type of filter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <SortOrder(0)> _
        Public Property FilterType() As StringFilterType = StringFilterType.CharsReplace

       


        ''' <summary>
        ''' Source value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>The source value will change depending on what filter type is used</remarks>
        <SortOrder(1)> _
        Public Property Source As New SimpleOrSimpleExpression(Of CData)("")


        'todo: remove that obsolete property once migrated
        <Browsable(False)> _
        Public Property SourceValue() As String
            Get
                Return Nothing
            End Get
            Set(value As String)
                Me.Source.Simple = value
                Me.Source.Mode = SimpleOrExpressionMode.Simple
            End Set
        End Property

        <DefaultValue(False)> _
        <ConditionalVisible("FilterType", False, True, StringFilterType.RegexReplace)> _
        Public Property UseCallBack As Boolean

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property ShowReplace As Boolean
            Get
                Return (Not Me.FilterType = StringFilterType.RegexReplace) OrElse Not UseCallBack
            End Get
        End Property

        <DefaultValue(False)> _
        <ConditionalVisible("ShowReplace")> _
        Public Property EmptyReplace As Boolean

        ''' <summary>
        ''' Replace value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>The replace value will change depending on what filter type is used, most natbly for char replacement</remarks>
        <ConditionalVisible("EmptyReplace", True)> _
        <ConditionalVisible("ShowReplace")> _
        Public Property Replace As New SimpleOrSimpleExpression(Of CData)("")

        'todo: remove that obsolete property once migrated
        <Browsable(False)> _
        Public Property ReplaceValue() As String
            Get
                Return Nothing
            End Get
            Set(value As String)
                Me.Replace.Simple = value
                Me.Replace.Mode = SimpleOrExpressionMode.Simple
            End Set
        End Property

        Private Function GetReplace(contextVars As IContextLookup) As String
            If EmptyReplace Then
                Return ""
            End If
            Return Replace.GetValue(contextVars)
        End Function

        <ConditionalVisible("FilterType", False, True, StringFilterType.RegexReplace)> _
        <ConditionalVisible("UseCallBack")> _
        Public Property CallBack As New SimpleExpression(Of MatchEvaluator)("")


        <DefaultValue(System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.CultureInvariant)> _
        <ConditionalVisible("FilterType", False, True, StringFilterType.RegexReplace)> _
        Public Property RegexOptions As RegexOptions = System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.CultureInvariant


        Private _CharsMaps As New Dictionary(Of String, Dictionary(Of Char, Char))
        Private _Regexes As New Dictionary(Of String, Regex)

        Public Function Process(ByVal inputString As String, contextVars As IContextLookup, preventDoubleDefaults As Boolean, defaultCharReplacement As String) As String
            Dim toReturn As String = inputString
            Dim strSource As String = Me.Source.GetValue(contextVars)

            Select Case Me.FilterType
                Case StringFilterType.CharsReplace
                    Dim strReplace As String = Me.GetReplace(contextVars)
                    Dim charMap As Dictionary(Of Char, Char) = Nothing
                    Dim key As String = strSource & strReplace
                    If Not _CharsMaps.TryGetValue(key, charMap) Then
                        charMap = New Dictionary(Of Char, Char)
                        If strReplace.Length = 0 Then
                            AddCharsTrim(charMap, strSource)
                        ElseIf strReplace.Length > 1 OrElse strSource.Length = 1 Then
                            AddCharsMap(charMap, strSource, strReplace)
                        Else
                            AddCharsDefault(charMap, strSource, strReplace(0))
                        End If
                        SyncLock _CharsMaps
                            _CharsMaps(key) = charMap
                        End SyncLock
                    End If
                    If charMap.Count > 0 Then

                        Dim toReturnBuilder As New StringBuilder()

                        Dim sourceChar, replaceChar As Char
                        Dim i As Integer = 0
                        Dim previousChar As Char = Char.MinValue
                        While i < toReturn.Length
                            sourceChar = toReturn(i)
                            If charMap.TryGetValue(sourceChar, replaceChar) Then
                                If replaceChar <> Char.MinValue Then
                                    If replaceChar <> previousChar OrElse Not preventDoubleDefaults OrElse replaceChar <> defaultCharReplacement Then
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


                Case StringFilterType.StringReplace
                    Dim strReplace As String = Me.GetReplace(contextVars)
                    toReturn = toReturn.Replace(strSource, strReplace)
                Case StringFilterType.RegexReplace
                    Dim objRegex As Regex = Nothing
                    If Not _Regexes.TryGetValue(strSource, objRegex) Then
                        objRegex = New Regex(strSource, Me.RegexOptions)
                        SyncLock _Regexes
                            _Regexes(strSource) = objRegex
                        End SyncLock
                    End If
                    If UseCallBack Then
                        
                        Dim matchEval As MatchEvaluator = CallBack.Evaluate(contextVars)
                        If matchEval IsNot Nothing Then
                            toReturn = objRegex.Replace(toReturn, matchEval)
                        End If
                    Else
                        Dim strReplace As String = Me.GetReplace(contextVars)
                        toReturn = objRegex.Replace(toReturn, strReplace)
                    End If

            End Select
            Return toReturn
        End Function


        ''' <summary>
        ''' Add char map to the transformation map
        ''' </summary>
        ''' <param name="targetDico"></param>
        ''' <param name="fromStr"></param>
        ''' <param name="toStr"></param>
        ''' <remarks></remarks>
        Private Sub AddCharsMap(ByRef targetDico As Dictionary(Of Char, Char), ByVal fromStr As String, ByVal toStr As String)

            If fromStr.Length <> toStr.Length Then
                Throw New ArgumentException("characters map source string and match replace have different lengths")
            End If

            For i As Integer = 0 To fromStr.Length - 1
                targetDico(fromStr(i)) = toStr(i)
            Next

        End Sub

        ''' <summary>
        ''' Add char trim to the transformation map
        ''' </summary>
        ''' <param name="targetDico"></param>
        ''' <param name="escapeString"></param>
        ''' <remarks></remarks>
        Private Sub AddCharsTrim(ByRef targetDico As Dictionary(Of Char, Char), ByVal escapeString As String)

            For i As Integer = 0 To escapeString.Length - 1
                targetDico(escapeString(i)) = Char.MinValue
            Next

        End Sub

        ''' <summary>
        ''' Add char default to the transformation map
        ''' </summary>
        ''' <param name="targetDico"></param>
        ''' <param name="escapeString"></param>
        ''' <param name="defaultChar"></param>
        ''' <remarks></remarks>
        Private Sub AddCharsDefault(ByRef targetDico As Dictionary(Of Char, Char), ByVal escapeString As String, ByVal defaultChar As Char)

            For i As Integer = 0 To escapeString.Length - 1
                targetDico(escapeString(i)) = defaultChar
            Next

        End Sub



        ''' <summary>
        ''' Equality
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim objToCompare As StringTransformInfo = TryCast(obj, StringTransformInfo)
            If objToCompare Is Nothing Then
                Return False
            End If
            Return objToCompare.FilterType = Me._FilterType _
                AndAlso objToCompare.Source.Equals(Me.Source) _
                AndAlso objToCompare.Replace.Equals(Me.Replace) _
                AndAlso Not objToCompare.UseCallBack OrElse (Me.UseCallBack AndAlso objToCompare.CallBack.Expression = Me.CallBack.Expression)

        End Function


    End Class


End Namespace
