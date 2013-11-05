Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Web
Imports System.Text
Imports Aricie.DNN.ComponentModel

Namespace Services.Filtering

    Public Enum TrimType
        None
        Trim
        TrimStart
        TrimEnd
    End Enum


    ''' <summary>
    ''' Enumeration of the possible encodings for the filter
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum EncodeProcessing
        None
        UrlEncode
        UrlDecode
        HtmlEncode
        HtmlDecode
    End Enum

    ''' <summary>
    ''' Enumeration of the possible transformations for the filter
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DefaultTransforms
        None
        UrlPart
        UrlFull
    End Enum

    ''' <summary>
    ''' Class representing filtering and transformation applied to a string
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class ExpressionFilterInfo

        Private _DefaultCharReplacement As String = Nothing
        Private _TransformList As New List(Of StringTransformInfo)

        'confort members
        Private _StringMap As Dictionary(Of String, String)
        Private _CharsMap As Dictionary(Of Char, Char)
        Private _RegexMap As Dictionary(Of Regex, String)
        Private _BuildLock As New Object


#Region "ctors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal buildDefaultTransforms As DefaultTransforms)
            Me.BuildDefault(buildDefaultTransforms)
        End Sub

        Public Sub New(ByVal maxLength As Integer, ByVal forceToLower As Boolean, ByVal encodePreProcessing As EncodeProcessing, ByVal buildDefaultTransforms As DefaultTransforms)
            Me.New()
            Me._MaxLength = maxLength
            Me._ForceToLower = forceToLower
            Me._EncodePreProcessing = encodePreProcessing
            Me.BuildDefault(buildDefaultTransforms)
        End Sub

#End Region



        ''' <summary>
        ''' Forces input to lowercase output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ForceToLower() As Boolean = False

        ''' <summary>
        ''' Encoding preprocessing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EncodePreProcessing() As EncodeProcessing = EncodeProcessing.None

        ''' <summary>
        ''' Transformation list to replace elements of the input
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <CollectionEditor(False, False, True, True, 5, CollectionDisplayStyle.Accordion, True)> _
        Public Property TransformList() As List(Of StringTransformInfo)
            Get
                Return _TransformList
            End Get
            Set(ByVal value As List(Of StringTransformInfo))
                _TransformList = value
                Me.ClearMap()
            End Set
        End Property

        ''' <summary>
        ''' returns default char replacement
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DefaultCharReplacement() As String
            Get
                If _DefaultCharReplacement Is Nothing Then
                    If Me.CharsMap.ContainsKey(" "c) Then
                        _DefaultCharReplacement = Me.CharsMap(" "c).ToString
                    ElseIf Not Me.StringMap.TryGetValue(" ", _DefaultCharReplacement) Then
                        _DefaultCharReplacement = "-"
                    End If
                End If
                Return _DefaultCharReplacement
            End Get
        End Property

        ''' <summary>
        ''' Prevent char replacements to output multiple separation chars
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PreventDoubleDefaults As Boolean = True



        ''' <summary>
        ''' Is the output Trimmed the output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Trim As TrimType

        ''' <summary>
        ''' The char to be trimmed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("Trim", True, True, TrimType.None)> _
        Public Property TrimChar As String = "-"
         



        ''' <summary>
        ''' Encoding postprocessing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EncodePostProcessing() As EncodeProcessing = EncodeProcessing.None


        ''' <summary>
        ''' Maximum length for the output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>-1 to disable</remarks>
        <Required(True)> _
        Public Property MaxLength() As Integer = -1


        ''' <summary>
        ''' Complete sub filters to be executed in order for fine tuning of transformations, just after the parent filter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AdditionalFilters As New List(Of ExpressionFilterInfo)





        ''' <summary>
        ''' Char-based transformation map
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore(), Browsable(False)> _
        Public ReadOnly Property CharsMap() As Dictionary(Of Char, Char)
            Get
                If _CharsMap Is Nothing Then
                    SyncLock _BuildLock
                        If _CharsMap Is Nothing Then
                            Me.BuildMap()
                        End If
                    End SyncLock
                End If
                Return _CharsMap
            End Get
        End Property

        ''' <summary>
        ''' string-based transformation map
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore(), Browsable(False)> _
        Public ReadOnly Property StringMap() As Dictionary(Of String, String)
            Get
                If _StringMap Is Nothing Then
                    SyncLock _BuildLock
                        If _StringMap Is Nothing Then
                            Me.BuildMap()
                        End If
                    End SyncLock
                End If
                Return _StringMap
            End Get
        End Property

        ''' <summary>
        ''' Regex-based transformation map
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore(), Browsable(False)> _
        Public ReadOnly Property RegexMap() As Dictionary(Of Regex, String)
            Get
                If _RegexMap Is Nothing Then
                    SyncLock _BuildLock
                        If _RegexMap Is Nothing Then
                            Me.BuildMap()
                        End If
                    End SyncLock
                End If
                Return _RegexMap
            End Get
        End Property


#Region "Public methods"
        ''' <summary>
        ''' Default values for the transformation
        ''' </summary>
        ''' <param name="defaultTrans"></param>
        ''' <remarks></remarks>
        Public Sub BuildDefault(ByVal defaultTrans As DefaultTransforms)

            Me._TransformList.Clear()
            Select Case defaultTrans
                Case DefaultTransforms.UrlFull
                    Me.ForceToLower = True
                    Dim fromString As String = "éèêë€ïìîòôõöûüùâàäãçñÿ"
                    Dim totoString As String = "eeeeeiiioooouuuaaaacny"
                    Me._TransformList.Add(New StringTransformInfo(StringFilterType.CharsReplace, fromString, totoString))
                Case DefaultTransforms.UrlPart
                    Me.EncodePreProcessing = EncodeProcessing.HtmlDecode
                    Me.ForceToLower = True
                    Dim fromString As String = "éèêë€ïìîòôõöûüùâàäãçñÿ"
                    Dim totoString As String = "eeeeeiiioooouuuaaaacny"
                    Dim reservedChars As String = "$&+,/:;=?@'#."
                    Dim unsafeChars As String = "  <>%{}|\^~[]`*–!‘’""«»()"
                    Me._TransformList.Add(New StringTransformInfo(StringFilterType.CharsReplace, fromString, totoString))
                    Me._TransformList.Add(New StringTransformInfo(StringFilterType.CharsReplace, unsafeChars & reservedChars, "-"))
            End Select



        End Sub



       
            

        ''' <summary>
        ''' Escapes the string passed as the parameter according to the rules defined in the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="originalString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Process(ByVal originalString As String) As String

            Dim toReturn As String
            Dim maxLength As Integer = Me._MaxLength
            Dim noMaxLentgth As Boolean = (maxLength = -1)

            'encode preprocessing
            Select Case Me._EncodePreProcessing
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
            If Me._ForceToLower Then
                toReturn = toReturn.ToLowerInvariant()
            End If

            'dealing with regexs
            For Each objRegexReplace As KeyValuePair(Of Regex, String) In Me.RegexMap
                If String.IsNullOrEmpty(objRegexReplace.Value) Then
                    toReturn = objRegexReplace.Key.Replace(toReturn, String.Empty)
                Else
                    toReturn = objRegexReplace.Key.Replace(toReturn, objRegexReplace.Value)
                End If

            Next

            'dealing with string replaces
            For Each objStringReplace As KeyValuePair(Of String, String) In Me.StringMap
                If String.IsNullOrEmpty(objStringReplace.Value) Then
                    toReturn = toReturn.Replace(objStringReplace.Key, String.Empty)
                Else
                    toReturn = toReturn.Replace(objStringReplace.Key, objStringReplace.Value)
                End If

            Next

            'dealing with char replaces
            If Me.CharsMap.Count > 0 Then

                Dim toReturnBuilder As New StringBuilder()

                Dim sourceChar, replaceChar As Char
                Dim i As Integer = 0
                Dim previousChar As Char = Char.MinValue
                Dim previousCharIsReplace As Boolean = False
                While i < toReturn.Length AndAlso (noMaxLentgth OrElse toReturnBuilder.Length <= maxLength)
                    sourceChar = toReturn(i)
                    If Me._CharsMap.TryGetValue(sourceChar, replaceChar) Then
                        If replaceChar <> Char.MinValue Then
                            If replaceChar <> previousChar OrElse replaceChar <> DefaultCharReplacement() OrElse Not PreventDoubleDefaults Then
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

            If Me.Trim <> TrimType.None Then
                If Not String.IsNullOrEmpty(TrimChar) Then
                    Select Case Me.Trim
                        Case TrimType.Trim
                            toReturn = toReturn.Trim(TrimChar(0))
                        Case TrimType.TrimStart
                            toReturn = toReturn.Trim(TrimChar(0))
                        Case TrimType.TrimEnd
                            toReturn = toReturn.Trim(TrimChar(0))
                    End Select
                End If
            End If

            'encode postprocessing
            Select Case Me._EncodePostProcessing
                Case EncodeProcessing.HtmlEncode
                    toReturn = HttpUtility.HtmlEncode(toReturn)
                Case EncodeProcessing.HtmlDecode
                    toReturn = HttpUtility.HtmlDecode(toReturn)
                Case EncodeProcessing.UrlEncode
                    toReturn = HttpUtility.UrlEncode(toReturn)
                Case EncodeProcessing.UrlDecode
                    toReturn = HttpUtility.UrlDecode(toReturn)
            End Select

            'limited Length
            If Not noMaxLentgth AndAlso toReturn.Length > maxLength Then
                toReturn = toReturn.Substring(0, maxLength)
            End If

            'recursive call to process additional filters sequentially
            For Each subExpression As ExpressionFilterInfo In Me.AdditionalFilters

                toReturn = subExpression.Process(toReturn)

            Next

            Return toReturn

        End Function






#End Region

#Region "Private methods"

        ''' <summary>
        ''' Builds the transformation map from the transformations configured
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BuildMap()

            Dim tempStringMap As New Dictionary(Of String, String)
            Dim tempcharsMap As New Dictionary(Of Char, Char)
            Dim tempRegexMap As New Dictionary(Of Regex, String)



            'If Me.TransformList.Count = 0 Then
            '    Me.BuildDefault()
            'Else
            SyncLock _TransformList
                For Each transform As StringTransformInfo In Me._TransformList
                    Select Case transform.FilterType
                        Case StringFilterType.CharsReplace
                            If transform.ReplaceValue.Length = 0 Then
                                AddCharsTrim(tempcharsMap, transform.SourceValue)
                            ElseIf transform.ReplaceValue.Length > 1 OrElse transform.SourceValue.Length = 1 Then
                                AddCharsMap(tempcharsMap, transform.SourceValue, transform.ReplaceValue)
                            Else
                                AddCharsDefault(tempcharsMap, transform.SourceValue, transform.ReplaceValue(0))
                            End If
                        Case StringFilterType.StringReplace
                            tempStringMap(transform.SourceValue) = transform.ReplaceValue
                        Case StringFilterType.RegexReplace
                            Try
                                Dim objRegex As New Regex(transform.SourceValue, RegexOptions.Compiled)
                                tempRegexMap(objRegex) = transform.ReplaceValue
                            Catch ex As Exception
                                Aricie.Services.ExceptionHelper.LogException(ex)
                            End Try
                    End Select
                Next
            End SyncLock

            Me._StringMap = tempStringMap
            Me._CharsMap = tempcharsMap
            Me._RegexMap = tempRegexMap
            'End If

        End Sub

        ''' <summary>
        '''  Clears the transformation map
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ClearMap()
            Me._StringMap = Nothing
            Me._CharsMap = Nothing
            Me._RegexMap = Nothing
        End Sub


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


#End Region

        ''' <summary>
        ''' Equality comparer 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing OrElse Not TypeOf obj Is ExpressionFilterInfo Then
                Return False
            End If
            Dim objToCompare As ExpressionFilterInfo = DirectCast(obj, ExpressionFilterInfo)
            If objToCompare Is Nothing Then
                Return False
            End If
            If objToCompare.MaxLength <> Me._MaxLength _
                    OrElse objToCompare.ForceToLower <> Me._ForceToLower _
                    OrElse objToCompare.EncodePreProcessing <> Me._EncodePreProcessing _
                    OrElse objToCompare.TransformList.Count <> Me._TransformList.Count Then
                Return False
            End If
            For i As Integer = 0 To Me._TransformList.Count - 1
                If Not Me._TransformList(i).Equals(objToCompare.TransformList(i)) Then
                    Return False
                End If
            Next
            Return True
        End Function

    End Class

End Namespace
