Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports Aricie.DNN.Security.Cryptography
Imports Aricie.Security.Cryptography
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Web
Imports System.Text
Imports Aricie.Text
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Entities
Imports System.Threading
Imports Aricie.DNN.Services.Flee

Namespace Services.Filtering
    ''' <summary>
    ''' Class representing filtering and transformation applied to a string
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Class ExpressionFilterInfo

        Private _DefaultCharReplacement As String = Nothing
        Private _TransformList As New List(Of StringTransformInfo)

        'confort members
        'Private _StringMap As Dictionary(Of String, String)
        'Private _CharsMap As Dictionary(Of Char, Char)
        'Private _RegexMap As Dictionary(Of Regex, Object)
        'Private _BuildLock As New Object
        Private _Encryption As EncryptionInfo


#Region "ctors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal buildDefaultTransforms As DefaultTransforms)
            Me.BuildDefault(buildDefaultTransforms)
        End Sub

        Public Sub New(ByVal maxLength As Integer, ByVal forceToLower As Boolean, ByVal encodePreProcessing As EncodeProcessing, ByVal buildDefaultTransforms As DefaultTransforms)
            Me.New()
            Me._MaxLength = maxLength
            Me.ForceToLower = forceToLower
            Me._EncodePreProcessing = encodePreProcessing
            Me.BuildDefault(buildDefaultTransforms)

        End Sub

#End Region






        ''' <summary>
        ''' Encoding preprocessing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("GlobalFilter")> _
        <DefaultValue(EncodeProcessing.None)> _
        Public Property EncodePreProcessing() As EncodeProcessing = EncodeProcessing.None

        <DefaultValue(HtmlEncodeMethod.HtmlEncode)> _
        <ConditionalVisible("EncodePreProcessing", False, True, EncodeProcessing.HtmlEncode)> _
        <ExtendedCategory("GlobalFilter")> _
        Public Property PreHtmlEncodeMethod As HtmlEncodeMethod = HtmlEncodeMethod.HtmlEncode


        <DefaultValue(CaseChange.None)> _
        <ExtendedCategory("GlobalFilter")> _
        Public Property CaseChange As CaseChange

        ''' <summary>
        ''' Forces input to lowercase output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)>
        <DefaultValue(False)> _
        Public Property ForceToLower() As Boolean
            Get
                Return Nothing
            End Get
            Set(value As Boolean)
                If value Then
                    Me.CaseChange = CaseChange.ToLower
                End If
            End Set
        End Property

        ''' <summary>
        ''' Is the output Trimmed the output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("GlobalFilter")> _
        <DefaultValue(TrimType.None)> _
        Public Property Trim As TrimType

        ''' <summary>
        ''' The char to be trimmed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("Trim", True, True, TrimType.None)> _
        <ExtendedCategory("GlobalFilter")> _
        <DefaultValue("-")> _
        Public Property TrimChar As String = "-"

         <ExtendedCategory("GlobalFilter")> _
        <DefaultValue(False)> _
        Public  Property Reverse() As Boolean

        <DefaultValue(false)> _
        <ExtendedCategory("GlobalFilter")> _
        Public Property ApplyFormat As Boolean

        <ExtendedCategory("GlobalFilter")> _
        <ConditionalVisible("ApplyFormat", False, True)> _
        Public Property FormatPattern As CData = "{0}"

        '<CollectionEditor(False, False, True, True, 5, CollectionDisplayStyle.Accordion, True)> _
        ''' <summary>
        ''' Transformation list to replace elements of the input
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Transformations")> _
        Public Property TransformList() As List(Of StringTransformInfo)
            Get
                Return _TransformList
            End Get
            Set(ByVal value As List(Of StringTransformInfo))
                _TransformList = value
                'Me.ClearMap()
            End Set
        End Property

        ''' <summary>
        ''' Prevent char replacements to output multiple separation chars
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Transformations")> _
        <DefaultValue(True)> _
        Public Property PreventDoubleDefaults As Boolean = True

        ''' <summary>
        ''' returns default char replacement
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Transformations")> _
        Public ReadOnly Property DefaultCharReplacement() As String
            Get
                If _DefaultCharReplacement Is Nothing Then

                    Dim tempChar As String = Nothing
                    Try
                        tempChar = Me.ProcessTransformations(" "c, Nothing, False, "-"c, True)
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                    End Try
                    If tempChar.IsNullOrEmpty() OrElse tempChar = " "c Then
                        _DefaultCharReplacement = "-"c
                    Else
                        _DefaultCharReplacement = tempChar
                    End If

                End If
                Return _DefaultCharReplacement
            End Get
        End Property

       

        <ExtendedCategory("Categorization")> _
        Public Property Categorization As New EnabledFeature(Of CategorizationInfo)

        <ExtendedCategory("Advanced")> _
        <DefaultValue(Base64Convert.None)> _
        Public Property Base64Convert As Base64Convert = Base64Convert.None

        <ExtendedCategory("Advanced")> _
        <ConditionalVisible("Base64Convert", False, True)> _
        <DefaultValue(SimpleEncoding.UTF8)> _
        Public Property TargetEncoding As SimpleEncoding = SimpleEncoding.UTF8

        <DefaultValue(false)> _
        <ExtendedCategory("Advanced")> _
        Public Property UseCompression As Boolean

        <DefaultValue(CompressionMethod.Deflate)> _
        <ExtendedCategory("Advanced")> _
        <ConditionalVisible("UseCompression", False, True)> _
        Public Property CompressMethod() As CompressionMethod

        <DefaultValue(CompressionDirection.Compress)> _
        <ExtendedCategory("Advanced")> _
        <ConditionalVisible("UseCompression", False, True)> _
        Public Property CompressDirection() As CompressionDirection = CompressionDirection.Compress

        <DefaultValue(False)> _
        <ExtendedCategory("Advanced")> _
        Public Property UseEncryption As Boolean

        <DefaultValue(CryptoTransformDirection.Encrypt)> _
        <ExtendedCategory("Advanced")> _
       <ConditionalVisible("UseEncryption", False, True)> _
        Public Property CryptoDirection As CryptoTransformDirection = CryptoTransformDirection.Encrypt

        Private _Base64Salt As String = ""

        <ExtendedCategory("Advanced")> _
        <ConditionalVisible("UseEncryption", False, True)> _
        Public Property Base64Salt As String
            Get
                If Not Me.UseEncryption Then
                    Return Nothing
                End If
                If _Base64Salt.IsNullOrEmpty() Then
                    _Base64Salt = CryptoHelper.GetNewSalt(30).ToBase64()
                End If
                Return _Base64Salt
            End Get
            Set(value As String)
                _Base64Salt = value
            End Set
        End Property

        <ExtendedCategory("Advanced")> _
          <ConditionalVisible("UseEncryption", False, True)> _
        Public Property Encryption As EncryptionInfo
            Get
                If Not Me.UseEncryption Then
                    Return Nothing
                ElseIf _Encryption Is Nothing Then
                    _Encryption = New EncryptionInfo
                End If
                Return _Encryption
            End Get
            Set(value As EncryptionInfo)
                _Encryption = value
            End Set
        End Property

        ''' <summary>
        ''' Encoding postprocessing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("GlobalFilter")> _
         <DefaultValue(EncodeProcessing.None)> _
        Public Property EncodePostProcessing() As EncodeProcessing = EncodeProcessing.None

        <ConditionalVisible("EncodePostProcessing", False, True, EncodeProcessing.HtmlEncode)> _
        <ExtendedCategory("GlobalFilter")> _
        <DefaultValue(HtmlEncodeMethod.HtmlEncode)> _
        Public Property PostHtmlEncodeMethod As HtmlEncodeMethod = HtmlEncodeMethod.HtmlEncode

        ''' <summary>
        ''' Maximum length for the output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>-1 to disable</remarks>
        <Required(True)> _
        <ExtendedCategory("GlobalFilter")> _
        <DefaultValue(-1)> _
        Public Property MaxLength() As Integer = -1


        ''' <summary>
        ''' Complete sub filters to be executed in order for fine tuning of transformations, just after the parent filter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("AdditionalFilters")> _
        Public Property AdditionalFilters As New List(Of ExpressionFilterInfo)




        ''' <summary>
        ''' Simulation data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Simulation")> _
            <Width(500)> _
            <LineCount(8)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <XmlIgnore()> _
        Public Overridable Property SimulationData() As New CData

        Private _SimulationResult As String = ""

        <Browsable(False)> _
        Public ReadOnly Property Hasresult As Boolean
            Get
                Return Not _SimulationResult.IsNullOrEmpty()
            End Get
        End Property

        ''' <summary>
        ''' Result of the simulation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("Hasresult")> _
        <ExtendedCategory("Simulation")> _
        Public ReadOnly Property SimulationResult() As String
            Get
                Return _SimulationResult
            End Get
        End Property

        <ExtendedCategory("Simulation")> _
        <ActionButton(IconName.Refresh, IconOptions.Normal)> _
        Public Sub RunSimulation(ByVal pe As AriciePropertyEditorControl)
            'If Not String.IsNullOrEmpty(Me.SimulationData.Value) Then
            Dim toReturn As String = Me.Process(Me.SimulationData.Value, Nothing)
            If toReturn IsNot Nothing Then
                Me._SimulationResult = toReturn
            End If
            pe.ItemChanged = True
            'End If
        End Sub



        ' ''' <summary>
        ' ''' Char-based transformation map
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<XmlIgnore(), Browsable(False)> _
        'Public ReadOnly Property CharsMap() As Dictionary(Of Char, Char)
        '    Get
        '        If _CharsMap Is Nothing Then
        '            SyncLock _BuildLock
        '                If _CharsMap Is Nothing Then
        '                    Me.BuildMap()
        '                End If
        '            End SyncLock
        '        End If
        '        Return _CharsMap
        '    End Get
        'End Property

        ' ''' <summary>
        ' ''' string-based transformation map
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<XmlIgnore(), Browsable(False)> _
        'Public ReadOnly Property StringMap() As Dictionary(Of String, String)
        '    Get
        '        If _StringMap Is Nothing Then
        '            SyncLock _BuildLock
        '                If _StringMap Is Nothing Then
        '                    Me.BuildMap()
        '                End If
        '            End SyncLock
        '        End If
        '        Return _StringMap
        '    End Get
        'End Property

        ' ''' <summary>
        ' ''' Regex-based transformation map
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<XmlIgnore(), Browsable(False)> _
        'Public ReadOnly Property RegexMap() As Dictionary(Of Regex, Object)
        '    Get
        '        If _RegexMap Is Nothing Then
        '            SyncLock _BuildLock
        '                If _RegexMap Is Nothing Then
        '                    Me.BuildMap()
        '                End If
        '            End SyncLock
        '        End If
        '        Return _RegexMap
        '    End Get
        'End Property


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

          Public Function ProcessList(ByVal originalStrings As IEnumerable(Of  String)) As List(Of String)
            Return Me.ProcessList(originalStrings, Nothing)
        End Function


        ''' <summary>
        ''' Escapes the string passed as the parameter according to the rules defined in the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="originalString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ProcessList(ByVal originalStrings As IEnumerable(Of  String), contextVars As IContextLookup) As List(Of String)
            dim toReturn as New List(Of String)
            For Each originalString As String In originalStrings
                toReturn.Add(Me.Process(originalString, contextVars))
            Next
            Return toReturn
        End Function

        Public Function Process(ByVal originalString As String) As String
            Return Me.Process(originalString, Nothing)
        End Function


        ''' <summary>
        ''' Escapes the string passed as the parameter according to the rules defined in the ExpressionFilterInfo
        ''' </summary>
        ''' <param name="originalString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Process(ByVal originalString As String, contextVars As IContextLookup) As String

            Dim toReturn As String = originalString
            'Dim intMaxLength As Integer = Me._MaxLength
            Dim noMaxLength As Boolean = (Me._MaxLength = -1)




            'encode preprocessing
            Select Case Me._EncodePreProcessing
                Case EncodeProcessing.HtmlEncode
                    toReturn = toReturn.HtmlEncode(Me.PreHtmlEncodeMethod)
                Case EncodeProcessing.HtmlDecode
                    toReturn = HttpUtility.HtmlDecode(toReturn)
                Case EncodeProcessing.UrlEncode
                    toReturn = HttpUtility.UrlEncode(toReturn)
                Case EncodeProcessing.UrlDecode
                    toReturn = HttpUtility.UrlDecode(toReturn)
                Case Else
                    toReturn = originalString
            End Select



            'forceLower
            If Me.CaseChange <> CaseChange.None Then
                Select Case CaseChange
                    Case CaseChange.ToLower
                        toReturn = toReturn.ToLower()
                    Case CaseChange.ToLowerInvariant
                        toReturn = toReturn.ToLowerInvariant()
                    Case CaseChange.ToUpper
                        toReturn = toReturn.ToUpper()
                    Case CaseChange.ToUpperInvariant
                        toReturn = toReturn.ToUpperInvariant()
                    Case Text.CaseChange.ToPascal
                        toReturn = toReturn.ToPascalCase()
                    Case Text.CaseChange.ToCamel
                        toReturn = toReturn.ToCamelCase()
                    Case Text.CaseChange.ToTitle
                        toReturn = toReturn.ToTitleCase()
                End Select
            End If

            toReturn = ProcessTransformations(toReturn, contextVars)
            

            If Me.Trim <> TrimType.None Then
                If Not String.IsNullOrEmpty(TrimChar) Then
                    Select Case Me.Trim
                        Case TrimType.Trim
                            toReturn = toReturn.Trim(TrimChar(0))
                        Case TrimType.TrimStart
                            toReturn = toReturn.TrimStart(TrimChar(0))
                        Case TrimType.TrimEnd
                            toReturn = toReturn.TrimEnd(TrimChar(0))
                    End Select
                Else
                    Select Case Me.Trim
                        Case TrimType.Trim
                            toReturn = toReturn.Trim()
                        Case TrimType.TrimStart
                            toReturn = toReturn.TrimStart()
                        Case TrimType.TrimEnd
                            toReturn = toReturn.TrimEnd()
                    End Select
                End If
            End If

            if Reverse
                toReturn = toReturn.Reverse()
            End If

            If Me.ApplyFormat Then
                toReturn = String.Format(FormatPattern, toReturn)
            End If

            If Base64Convert <> Base64Convert.None Then
                Select Case Base64Convert
                    Case Base64Convert.ToBase64
                        toReturn = toReturn.GetBase64FromEncoding(TargetEncoding.GetEncoding())
                    Case Base64Convert.FromBase64
                        toReturn = toReturn.GetFromBase64(TargetEncoding.GetEncoding())
                End Select
            End If

            If Me.UseCompression Then
                Select Case CompressDirection
                    Case CompressionDirection.Compress
                        toReturn = toReturn.Compress(CompressMethod)
                    Case CompressionDirection.Decompress
                        toReturn = toReturn.Decompress(CompressMethod)
                End Select
            End If

            If Me.UseEncryption Then
                Select Case CryptoDirection
                    Case CryptoTransformDirection.Encrypt
                        toReturn = Encryption.DoEncrypt(toReturn.ToUTF8(), Base64Salt.FromBase64()).ToBase64()
                    Case CryptoTransformDirection.Decrypt
                        toReturn = Encryption.Decrypt(toReturn.FromBase64(), Base64Salt.FromBase64()).FromUTF8()
                End Select

            End If


            'encode postprocessing
            Select Case Me._EncodePostProcessing
                Case EncodeProcessing.HtmlEncode
                    toReturn = toReturn.HtmlEncode(Me.PostHtmlEncodeMethod)
                Case EncodeProcessing.HtmlDecode
                    toReturn = HttpUtility.HtmlDecode(toReturn)
                Case EncodeProcessing.UrlEncode
                    toReturn = HttpUtility.UrlEncode(toReturn)
                Case EncodeProcessing.UrlDecode
                    toReturn = HttpUtility.UrlDecode(toReturn)
            End Select

            'limited Length
            If Not noMaxLength AndAlso toReturn.Length > Me._MaxLength Then
                toReturn = toReturn.Substring(0, Me._MaxLength)
            End If

            If Me.Categorization.Enabled Then
                toReturn = Me.Categorization.Entity.Match(toReturn, contextVars)
            End If

            'recursive call to process additional filters sequentially

            Return Me.AdditionalFilters.Aggregate(toReturn, Function(current, subExpression) subExpression.Process(current, contextVars))

        End Function




#End Region

#Region "Private methods"

        Private Function ProcessTransformations(ByVal inputString As String, contextVars As IContextLookup) As String
            Return Me.ProcessTransformations(inputString, contextVars, Me.PreventDoubleDefaults, Me.DefaultCharReplacement)
        End Function

        Private Function ProcessTransformations(ByVal inputString As String, contextVars As IContextLookup, boolPreventDoubleDefaults As Boolean, charDefaultCharReplacement As String) As String
            Return ProcessTransformations(inputString, contextVars, boolPreventDoubleDefaults, charDefaultCharReplacement, False)
        End Function

        Private Function ProcessTransformations(ByVal inputString As String, contextVars As IContextLookup, boolPreventDoubleDefaults As Boolean, charDefaultCharReplacement As String, charReplaceOnly As Boolean) As String
            Dim toreturn As String = inputString
            For Each transform As StringTransformInfo In Me._TransformList
                If Not charReplaceOnly OrElse transform.FilterType = StringFilterType.CharsReplace Then
                    toreturn = transform.Process(toreturn, contextVars, boolPreventDoubleDefaults, charDefaultCharReplacement)
                End If
            Next
            Return toreturn
        End Function

        ' ''' <summary>
        ' ''' Builds the transformation map from the transformations configured
        ' ''' </summary>
        ' ''' <remarks></remarks>
        'Private Sub BuildMap()

        '    Dim tempStringMap As New Dictionary(Of String, String)
        '    Dim tempcharsMap As New Dictionary(Of Char, Char)
        '    Dim tempRegexMap As New Dictionary(Of Regex, Object)



        '    'If Me.TransformList.Count = 0 Then
        '    '    Me.BuildDefault()
        '    'Else
        '    SyncLock _TransformList
        '        For Each transform As StringTransformInfo In Me._TransformList
        '            Select Case transform.FilterType
        '                Case StringFilterType.CharsReplace
        '                    If transform.ReplaceValue.Length = 0 Then
        '                        AddCharsTrim(tempcharsMap, transform.SourceValue)
        '                    ElseIf transform.ReplaceValue.Length > 1 OrElse transform.SourceValue.Length = 1 Then
        '                        AddCharsMap(tempcharsMap, transform.SourceValue, transform.ReplaceValue)
        '                    Else
        '                        AddCharsDefault(tempcharsMap, transform.SourceValue, transform.ReplaceValue(0))
        '                    End If
        '                Case StringFilterType.StringReplace
        '                    tempStringMap(transform.SourceValue) = transform.ReplaceValue
        '                Case StringFilterType.RegexReplace
        '                    Try
        '                        Dim objRegex As New Regex(transform.SourceValue, transform.RegexOptions)
        '                        If transform.UseCallBack Then
        '                            tempRegexMap(objRegex) = transform.CallBack
        '                        Else
        '                            tempRegexMap(objRegex) = transform.ReplaceValue
        '                        End If

        '                    Catch ex As Exception
        '                        Aricie.Services.ExceptionHelper.LogException(ex)
        '                    End Try
        '            End Select
        '        Next
        '    End SyncLock

        '    Me._StringMap = tempStringMap
        '    Me._CharsMap = tempcharsMap
        '    Me._RegexMap = tempRegexMap
        '    'End If

        'End Sub

        ' ''' <summary>
        ' '''  Clears the transformation map
        ' ''' </summary>
        ' ''' <remarks></remarks>
        'Private Sub ClearMap()
        '    Me._StringMap = Nothing
        '    Me._CharsMap = Nothing
        '    Me._RegexMap = Nothing
        'End Sub



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
                    OrElse objToCompare.CaseChange <> Me._CaseChange _
                    OrElse objToCompare.EncodePreProcessing <> Me._EncodePreProcessing _
                    OrElse objToCompare.EncodePostProcessing <> Me.EncodePostProcessing _
                    OrElse objToCompare.Trim <> Me.Trim _
                    OrElse objToCompare.TrimChar <> Me.TrimChar _
                    OrElse objToCompare.ApplyFormat <> Me.ApplyFormat _
                    OrElse objToCompare.FormatPattern.Value <> Me.FormatPattern.Value _
                    OrElse objToCompare.PreventDoubleDefaults <> Me.PreventDoubleDefaults _
                    OrElse objToCompare.DefaultCharReplacement <> Me.DefaultCharReplacement _
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
