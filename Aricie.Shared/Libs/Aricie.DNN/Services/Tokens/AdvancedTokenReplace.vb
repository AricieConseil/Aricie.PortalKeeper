Imports Aricie.DNN.UI
Imports Aricie.Services
Imports Aricie.DNN.Settings

Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Tokens

Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.UI


Namespace Services

   

    ''' <summary>
    ''' Extended DotnetNuke Token replace system
    ''' </summary>
    ''' <remarks>
    ''' Voici un exemple d'utilisation de l'advance token replace :
    ''' <h1>Newsletter</h1>
    ''' <h2>il y a [newsletter:Actus:Count] actus</h2>
    ''' [newsletter:Actus:Count:]
    '''  <p>Les voici :</p>
    '''  <ul>
    '''  [newsletter:Actus:]
    '''   <li><a href="[newsletter:GetActuUrl:[:IdActu]]">[:Title]</a></li>
    '''  [/newsletter:Actus:]
    '''  </ul>
    ''' [/newsletter:Actus:Count:]
    ''' <h2>il y a [newsletter:Reunions:Count] réunions</h2>
    ''' [newsletter:Reunions:Count:]
    '''  <p>Les voici :</p>
    '''  <ul>
    '''  [newsletter:Reunions:]
    '''   <li><a href="[newsletter:GetReunionUrl:[:IdEvent]]">[:Title]</a></li>
    '''  [/newsletter:Reunions:]
    '''  </ul>
    ''' [/newsletter:Reunions:Count:]
    ''' </remarks>
    Public Class AdvancedTokenReplace
        Inherits DotNetNuke.Services.Tokens.TokenReplace

        Private Const No_Object As String = "no_object"

#Region "members"


        private const TextExpression as String = "(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]*\DELIM_CLOSE)|(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]+)|(?<text>\DELIM_CLOSE[^\DELIM_OPEN\DELIM_CLOSE]+)|(?<text>[^\DELIM_OPEN\DELIM_CLOSE]+)"

        'Private Const ExpressionDefault As String = "(?:\DELIM_OPEN(?:(?<object>[a-zA-Z]+[^\DELIM_OPEN\DELIM_CLOSE:\s]*):(?<property>[^\DELIM_OPEN\DELIM_CLOSE\|\/]*[^\DELIM_OPEN\DELIM_CLOSE\|\/:]+))(?:\|(?:(?<format>[^\DELIM_OPEN\DELIM_CLOSE]+)\|(?<ifEmpty>[^\DELIM_OPEN\DELIM_CLOSE]+))|\|(?:(?<format>[^\|\DELIM_OPEN\DELIM_CLOSE]+)))?\DELIM_CLOSE)|(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]*\DELIM_CLOSE)|(?<text>[^\DELIM_OPEN\DELIM_CLOSE]+)"
        Private Const ExpressionDefault As String = "(?:\DELIM_OPEN(?:(?<object>"& Aricie.Constants.Content.RegularNameValidator &")\:(?<property>[^\DELIM_OPEN\DELIM_CLOSE\|\/]*[^\DELIM_OPEN\DELIM_CLOSE\|\/:]+))(?:\|(?:(?<format>[^\DELIM_OPEN\DELIM_CLOSE]+)\|(?<ifEmpty>[^\DELIM_OPEN\DELIM_CLOSE]+))|\|(?:(?<format>[^\|\DELIM_OPEN\DELIM_CLOSE]+)))?\DELIM_CLOSE)|" &  TextExpression
        'Private Const ExpressionObjectLess As String = "(?:\DELIM_OPEN(?:(?<object>[a-zA-Z]+[^\DELIM_CLOSE\DELIM_OPEN:\s]*):(?<property>[^\DELIM_CLOSE\DELIM_OPEN\|]*[^\DELIM_OPEN\DELIM_CLOSE\|\/:]+))(?:\|(?:(?<format>[^\DELIM_CLOSE\DELIM_OPEN]+)\|(?<ifEmpty>[^\DELIM_CLOSE\DELIM_OPEN]+))|\|(?:(?<format>[^\|\DELIM_CLOSE\DELIM_OPEN]+)))?\DELIM_CLOSE)|(?:(?<object>\DELIM_OPEN)(?<property>[A-Z]+[A-Z0-9._]*)(?:\|(?:(?<format>[^\DELIM_CLOSE\DELIM_OPEN]+)\|(?<ifEmpty>[^\DELIM_CLOSE\DELIM_OPEN]+))|\|(?:(?<format>[^\|\DELIM_CLOSE\DELIM_OPEN]+)))?\DELIM_CLOSE)|(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]*\DELIM_CLOSE)|(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]+:)|(?<text>\DELIM_CLOSE[^\DELIM_OPEN\DELIM_CLOSE]+)|(?<text>[^\DELIM_OPEN\DELIM_CLOSE]+)"
        Private Const ExpressionObjectLess As String = "(?:\DELIM_OPEN(?:(?<object>"& Aricie.Constants.Content.RegularNameValidator &")\:(?<property>[^\DELIM_CLOSE\DELIM_OPEN\|]*[^\DELIM_OPEN\DELIM_CLOSE\|\/:]+))(?:\|(?:(?<format>[^\DELIM_CLOSE\DELIM_OPEN]+)\|(?<ifEmpty>[^\DELIM_CLOSE\DELIM_OPEN]+))|\|(?:(?<format>[^\|\DELIM_CLOSE\DELIM_OPEN]+)))?\DELIM_CLOSE)|(?:(?<object>\DELIM_OPEN)(?<property>[A-Z]+[A-Z0-9._]*)(?:\|(?:(?<format>[^\DELIM_CLOSE\DELIM_OPEN]+)\|(?<ifEmpty>[^\DELIM_CLOSE\DELIM_OPEN]+))|\|(?:(?<format>[^\|\DELIM_CLOSE\DELIM_OPEN]+)))?\DELIM_CLOSE)|" & TextExpression


        Private Const glbLoopTokenReplaceCacheKey As String = "LoopToken"

        Private Const glbLoopRegExpression As String = glbListLoopExpression & glbInnerLoopRegExpression & glbInterTextRegExpression
        'Private Const glbLoopRegExpression As String = "(?<LoopToken>(?:\[(?<ListToken>(?<object>[^\]\[\:]+)\:(?<property>[^\]\[\|]+))\:\])(?<InnerContent>(?:\[\:(?<InnerToken>(?<property>[^\]\[\|]+)(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?)\])|(?<InnerText>\[[^\]\[]+\])|(?<InnerText>[^\]\[]+))+(?:\[\/\k<ListToken>\:\]))"
        Private Const glbListLoopExpression As String = "(?:\DELIM_OPEN(?:(?<EndLoop>\/))?(?<ListToken>(?<object>[^\DELIM_CLOSE\DELIM_OPEN\:]+)\:(?<property>[^\DELIM_CLOSE\DELIM_OPEN\|]+))\:\DELIM_CLOSE)"
        'Private Const glbEndLoopRegExpression As String = "(?:\[\/\k<ListToken>\:\])"
        Private Const glbInnerLoopRegExpression As String = "|(?:\DELIM_OPEN\:(?<InnerToken>(?<property>[^\DELIM_CLOSE\DELIM_OPEN\|]+)(?:\|(?:(?<format>[^\DELIM_CLOSE\DELIM_OPEN]+)\|(?<ifEmpty>[^\DELIM_CLOSE\DELIM_OPEN]+))|\|(?:(?<format>[^\|\DELIM_CLOSE\DELIM_OPEN]+)))?)\DELIM_CLOSE)"
        'Private Const glbInnerTextRegExpression As String = "|(?<InnerText>\[[^\]\[]+\])|(?<InnerText>[^\]\[]+)"
        Private Const glbInterTextRegExpression As String = "|(?<text>\DELIM_OPEN[^\DELIM_CLOSE\DELIM_OPEN]*\DELIM_CLOSE)|(?<text>\DELIM_OPEN[^\DELIM_OPEN\DELIM_CLOSE]+)|(?<text>\DELIM_CLOSE[^\DELIM_OPEN\DELIM_CLOSE]+)|(?<text>[^\DELIM_CLOSE\DELIM_OPEN]+)"


        Private _ResourceFile As String = ""
        Private _SimpleTokens As New Dictionary(Of String, String)(StringComparer.InvariantCultureIgnoreCase)
        Private _Delimiter As DelimiterType = DelimiterType.Brackets

        Private _ProcessLoops As Boolean = True


#End Region

#Region "cTors"



        Public Sub New()
            MyBase.New(Scope.NoSettings, "", NukeHelper.PortalSettings, Nothing)
            Me.CurrentAccessLevel = Scope.DefaultSettings
            Me.AccessingUser = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
            Me.User = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
            Me.PropertySource = New Dictionary(Of String, IPropertyAccess)(StringComparer.InvariantCultureIgnoreCase)
            Me.UseObjectLessExpression = True

            Me.PropertySource(No_Object) = New DictionaryPropertyAccess(_SimpleTokens)
            If (Me.CurrentAccessLevel >= Scope.Configuration) Then
                If Me.PortalSettings Is Nothing Then
                    Me.PortalSettings = NukeHelper.PortalSettings
                End If
                If Me.PortalSettings IsNot Nothing Then
                    Me.SetObjectReplace(Me.PortalSettings, "portal")
                    'MyBase.PropertySource.Item("portal") = Me.PortalSettings
                    MyBase.PropertySource.Item("tab") = Me.PortalSettings.ActiveTab
                End If
                MyBase.PropertySource.Item("host") = New HostPropertyAccess
                Me.SetObjectReplace(DnnContext.Current, "Context")
            End If
            If (((Me.CurrentAccessLevel >= Scope.DefaultSettings) AndAlso (Not Me.User Is Nothing)) _
                    AndAlso (Me.User.UserID <> -1)) Then
                MyBase.PropertySource.Item("user") = Me.User
                MyBase.PropertySource.Item("membership") = New MembershipPropertyAccess(Me.User)
                MyBase.PropertySource.Item("profile") = New ProfilePropertyAccess(Me.User)
            End If
        End Sub

        ''' <summary>
        ''' Build AdvancedTokenReplace and add User data
        ''' </summary>
        ''' <param name="objUser"></param>
        ''' <remarks></remarks>
        Public Sub New(objUser As UserInfo)
            Me.New()
            Me.User = objUser
            MyBase.PropertySource.Item("user") = Me.User
        End Sub

        ''' <summary>
        ''' Build AdvancedTokenReplace and add Module data
        ''' </summary>
        ''' <param name="objModuleInfo"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal objModuleInfo As ModuleInfo)
            Me.New()
            SetModule(objModuleInfo)
        End Sub

        ''' <summary>
        ''' Build AdvancedTokenReplace and add Module data and resource file
        ''' </summary>
        ''' <param name="moduleInfo"></param>
        ''' <param name="resourceFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal moduleInfo As ModuleInfo, ByVal resourceFile As String)
            Me.New(moduleInfo)
            Me._ResourceFile = resourceFile
        End Sub

        ''' <summary>
        ''' Build AdvancedTokenReplace and add resource file
        ''' </summary>
        ''' <param name="resourceFile"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal resourceFile As String)
            Me.New()
            Me._ResourceFile = resourceFile
        End Sub

        ''' <summary>
        ''' Build AdvancedTokenReplace and add resource file
        ''' </summary>
        ''' <param name="resourceFile"></param>
        ''' <param name="customObject"></param>
        ''' <param name="customCaption"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal resourceFile As String, ByVal customObject As Object, ByVal customCaption As String)
            Me.New(resourceFile)
            Me.SetObjectReplace(customObject, customCaption)
        End Sub

        Public Sub New(ByVal customObject As Object, ByVal customCaption As String)
            Me.New("", customObject, customCaption)
        End Sub

#End Region


#Region "Properties"

        ''' <summary>
        ''' Adds module data to the ATR
        ''' </summary>
        ''' <param name="objModule"></param>
        ''' <remarks></remarks>
        Public Sub SetModule(ByVal objModule As ModuleInfo)
            If (Not objModule Is Nothing) Then
                Me.ModuleInfo = objModule
                MyBase.PropertySource.Item("module") = objModule
            End If

        End Sub

        ''' <summary>
        ''' Adds Tab data to the ATR
        ''' </summary>
        ''' <param name="objTab"></param>
        ''' <remarks></remarks>
        Public Sub SetTab(ByVal objTab As TabInfo)
            If (objTab IsNot Nothing) Then
                MyBase.PropertySource.Item("tab") = objTab
            End If

        End Sub

        ''' <summary>
        ''' Adds Resource file to the ATR
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ResourceFile() As String
            Get
                Return _ResourceFile
            End Get
            Set(ByVal value As String)
                _ResourceFile = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the ATR uses ObjectLess expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectLessExpression() As Boolean
            Get
                Return Me.UseObjectLessExpression
            End Get
            Set(ByVal value As Boolean)
                Me.UseObjectLessExpression = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Collection of simple tokens
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SimpleTokens() As Dictionary(Of String, String)
            Get
                Return _SimpleTokens
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                _SimpleTokens = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ATR processing loops
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProcessLoops() As Boolean
            Get
                Return _ProcessLoops
            End Get
            Set(ByVal value As Boolean)
                _ProcessLoops = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the delimiter for tokens
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Delimiter() As DelimiterType
            Get
                Return _Delimiter
            End Get
            Set(ByVal value As DelimiterType)
                _Delimiter = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the delimiter opening value, ignoring the Delimiter property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DelimOpen() As String
            Get
                Select Case Delimiter
                    Case DelimiterType.Braces
                        Return "{"c
                    Case Else
                        Return "["c
                End Select
            End Get
        End Property

        ''' <summary>
        ''' Gets the delimiter closing value, ignoring the Delimiter property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DelimClose() As String
            Get
                Select Case Delimiter
                    Case DelimiterType.Braces
                        Return "}"c
                    Case Else
                        Return "]"c
                End Select
            End Get
        End Property


        ''' <summary>
        ''' Gets the regular expression used by the ATR
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property RegExpression() As String
            Get
                Dim toReturn As String = ExpressionDefault
                If UseObjectLessExpression Then
                    toReturn = ExpressionObjectLess
                End If
                Return ReplaceDelims(toReturn)
            End Get
        End Property

        ''' <summary>
        ''' Gets the loop regular expression used by the ATR
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property LoopRegExpression() As String
            Get
                Dim toReturn As String = glbLoopRegExpression
                Return ReplaceDelims(toReturn)
            End Get
        End Property

        ''' <summary>
        ''' Gets the cache key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property TokenReplaceCacheKey() As String
            Get
                If UseObjectLessExpression Then
                    Return "TokenReplaceRegEx_Objectless"
                Else
                    Return "TokenReplaceRegEx_Default"
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the Regular expression for the token to be replaced
        ''' </summary>
        ''' <remarks>Propriété à modifier afin quelle utilise un singleton plutot qu'une mise en cache</remarks>
        ''' <value>A regular Expression</value>   
        Protected Overloads ReadOnly Property TokenizerRegex() As Regex
            Get
                Dim cache As Regex = GetGlobal(Of Regex)(TokenReplaceCacheKey, Delimiter.ToString)
                If cache Is Nothing Then
                    cache = New Regex(RegExpression, RegexOptions.Compiled)
                    SetGlobal(Of Regex)(cache, TokenReplaceCacheKey, Delimiter.ToString)
                End If
                Return cache
            End Get
        End Property

        ''' <summary>
        ''' Gets the regular expression for the loop to be replaced
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected ReadOnly Property LoopTokenizerRegex() As Regex
            Get
                Dim cache As Regex = GetGlobal(Of Regex)(glbLoopTokenReplaceCacheKey, Delimiter.ToString)
                If (cache Is Nothing) Then
                    cache = New Regex(LoopRegExpression, RegexOptions.Compiled)
                    SetGlobal(Of Regex)(cache, glbLoopTokenReplaceCacheKey, Delimiter.ToString)
                End If
                Return cache
            End Get
        End Property

#End Region

#Region "Public methods"

        Private _ObjectSet As New HashSet(Of Object)


        Public Function IsSet(obj As Object) As Boolean
            Return _ObjectSet.Contains(obj)
        End Function

        Public Sub SetIsSet(obj As Object)
            _ObjectSet.Add(obj)
        End Sub

        Public Sub SetPropertySource(ByVal caption As String, propAccess As IPropertyAccess)
            Dim strCaption As String = caption.ToLowerInvariant
            Me.PropertySource(strCaption) = propAccess
        End Sub


        ''' <summary>
        ''' Adds an object to the collection of source data for the ATR
        ''' </summary>
        ''' <param name="customObject"></param>
        ''' <param name="customCaption"></param>
        ''' <remarks></remarks>
        Public Sub SetObjectReplace(ByVal customObject As Object, ByVal customCaption As String)

            Dim strCaption As String = customCaption.ToLowerInvariant
            Dim propAccess As IPropertyAccess = Nothing
            If Me.PropertySource.TryGetValue(strCaption, propAccess) AndAlso TypeOf propAccess Is DeepObjectPropertyAccess Then
                DirectCast(propAccess, DeepObjectPropertyAccess).TokenSource = customObject
            Else
                Me.PropertySource(strCaption) = New DeepObjectPropertyAccess(customObject, Me.ResourceFile)
            End If

            _ObjectSet.Add(customObject)

        End Sub

        ''' <summary>
        ''' Removes an object from the collection of source data for the ATR
        ''' </summary>
        ''' <param name="customCaption"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveObjectReplace(ByVal customCaption As String) As Boolean
            Return Me.PropertySource.Remove(customCaption.ToLower(CultureInfo.InvariantCulture))
        End Function

        <Obsolete("Obsolete because of bad overload, use ReplaceTokens")> _
        Public Overloads Function ReplaceEnvironmentTokens(ByVal strSourceText As String, ByVal customObject As Object, _
                                                            ByVal customCaption As String) As String
            Return Me.ReplaceSetTokens(strSourceText, customObject, customCaption)
        End Function

        ''' <summary>
        ''' Replaces tokens from the source text
        ''' </summary>
        ''' <param name="strSourceText"></param>
        ''' <param name="customObject"></param>
        ''' <param name="customCaption"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReplaceSetTokens(ByVal strSourceText As String, ByVal customObject As Object, _
                                          ByVal customCaption As String) As String
            Me.SetObjectReplace(customObject, customCaption)
            Return Me.ReplaceTokens(strSourceText)
        End Function

        ''' <summary>
        ''' Replaces all tokens from the source text
        ''' </summary>
        ''' <param name="strSourceText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReplaceAllTokens(ByVal strSourceText As String) As String
            Return Me.ReplaceTokens(strSourceText)
        End Function

#End Region


#Region "protected Overrides"

        ''' <summary>
        ''' Callback on token replaced
        ''' </summary>
        ''' <param name="strObjectName"></param>
        ''' <param name="strPropertyName"></param>
        ''' <param name="strFormat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OnReplacedTokenValue(ByVal strObjectName As String, ByVal strPropertyName As String, _
                                                         ByVal strFormat As String) As String
            Return MyBase.replacedTokenValue(strObjectName, strPropertyName, strFormat)

        End Function

        ''' <summary>
        ''' Replaces tokens from the source text
        ''' </summary>
        ''' <param name="strSourceText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function ReplaceTokens(ByVal strSourceText As String) As String
            If strSourceText Is Nothing Then
                Return String.Empty
            End If


            If Me.ProcessLoops Then
                strSourceText = Me.ReplaceLoopTokens(strSourceText)
            End If

            Dim Result As New System.Text.StringBuilder
            For Each currentMatch As Match In TokenizerRegex.Matches(strSourceText)

                Dim strObjectName As String = currentMatch.Result("${object}")
                If strObjectName.Length > 0 Then
                    If strObjectName = Me.DelimOpen Then strObjectName = ObjectLessToken
                    Dim strPropertyName As String = currentMatch.Result("${property}")
                    Dim strFormat As String = currentMatch.Result("${format}")
                    Dim strIfEmptyReplacment As String = currentMatch.Result("${ifEmpty}")
                    Dim strConversion As String = replacedTokenValue(strObjectName, strPropertyName, strFormat)
                    If strIfEmptyReplacment.Length > 0 AndAlso strConversion.Length = 0 Then strConversion = strIfEmptyReplacment
                    Result.Append(strConversion)
                Else
                    dim textResult as String =currentMatch.Result("${text}")
                    Result.Append(textResult)
                End If
            Next
            Return Result.ToString()

        End Function

        ''' <summary>
        ''' Replaces loop tokens from the source text
        ''' </summary>
        ''' <param name="sourceText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ReplaceLoopTokens(ByVal sourceText As String) As String

            Dim enumerator As IEnumerator = Nothing
            If String.IsNullOrEmpty(sourceText) Then
                Return String.Empty
            End If
            Dim builder As New StringBuilder

            Try
                enumerator = Me.LoopTokenizerRegex.Matches(sourceText).GetEnumerator

                Dim outerLoopProcessor As LoopTokenProcessor = Nothing

                Do While enumerator.MoveNext
                    Dim current As Match = DirectCast(enumerator.Current, Match)

                    'Is it a loop token?
                    If current.Groups("ListToken").Success Then
                        'Token prefix
                        Dim listToken As String = current.Groups("ListToken").Value
                        'Optning loop token?
                        If Not current.Groups("EndLoop").Success Then
                            If outerLoopProcessor Is Nothing Then
                                Dim strObjectName As String = current.Result("${object}")
                                'Valid loop token?
                                If Me.PropertySource.ContainsKey(strObjectName) AndAlso TypeOf Me.PropertySource(strObjectName) Is DeepObjectPropertyAccess Then
                                    Dim propAccess As DeepObjectPropertyAccess = DirectCast(Me.PropertySource(strObjectName), DeepObjectPropertyAccess)
                                    Dim strPropertyName As String = current.Result("${property}")
                                    'Get the Loop/If collection (the If collection contains either 1 element (True) or 0 element (False)
                                    Dim collec As IEnumerable = propAccess.GetEnumerable(strPropertyName)
                                    'Create LoopProcessor
                                    outerLoopProcessor = New LoopTokenProcessor(listToken, collec)

                                    'Invalid loop token
                                Else
                                    Dim strMessage As String = String.Format("bad use of loop token {0} , No corresponding PropertyAccess found ", current.Value)
                                    If Me.DebugMessages Then
                                        builder.Append(strMessage)
                                    Else
                                        Throw New ArgumentException(strMessage)
                                    End If


                                End If
                            Else
                                outerLoopProcessor.AddMatch(current)
                            End If
                        Else
                            'Closing loop token
                            'Do we have an opening token?
                            If outerLoopProcessor Is Nothing Then
                                Dim strMessage As String = String.Format("End Loop token {0} does not match any start token", listToken)
                                If Me.DebugMessages Then
                                    builder.Append(strMessage)
                                Else
                                    Throw New ArgumentException(strMessage)
                                End If
                            End If
                            'Is it the root level?
                            If outerLoopProcessor.ListToken <> listToken Then
                                'Nope, Queue the current processor to the parent processor's tokens

                                outerLoopProcessor.AddMatch(current)

                            Else
                                'Yes, start processing
                                outerLoopProcessor.RunProcessor(Me, builder)
                                outerLoopProcessor = Nothing
                            End If

                        End If

                        'Internal Token
                    ElseIf outerLoopProcessor Is Nothing Then
                        'outer text, not part from loop
                        builder.Append(current.Groups("text").Value)

                    Else
                        outerLoopProcessor.AddMatch(current)
                    End If
                Loop
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            Finally
                If enumerator IsNot Nothing AndAlso TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
            Return builder.ToString

        End Function

#End Region


#Region "Help"
        ''' <summary>
        ''' Returns the tokens list as a datatable for help purpose
        ''' </summary>
        ''' <param name="dt"></param>
        ''' <param name="prefix"></param>
        ''' <param name="objectType"></param>
        ''' <param name="levelNb"></param>
        ''' <param name="filter"></param>
        ''' <remarks></remarks>
        Public Shared Sub GetTokenDatatable(ByRef dt As DataTable, ByVal prefix As String, ByVal objectType As Type, _
                                                ByVal levelNb As Integer, Optional ByVal filter As ArrayList = Nothing)
            Dim itemColumn As New DataColumn("Item", GetType(String))
            Dim tokenColumn As New DataColumn("Token", GetType(String))
            Dim dr As DataRow

            If dt.Columns("Item") Is Nothing AndAlso dt.Columns("Token") Is Nothing Then
                dt.Columns.Add(itemColumn)
                dt.Columns.Add(tokenColumn)
            End If
            dr = dt.NewRow()
            dr("Item") = "------ " & prefix & " ------"
            dr("Token") = ""
            dt.Rows.Add(dr)
            Dim _
                innerProperties As Dictionary(Of String, PropertyInfo) = _
                    ReflectionHelper.GetPropertiesDictionary(objectType)
            If prefix <> "" Then
                prefix &= ":"c
            End If
            For Each propName As String In innerProperties.Keys
                'Si le filtre n'est pas vide, on récupére tous les tokens présent dans le filtre
                If filter IsNot Nothing Then
                    If filter.Contains(propName) Then
                        If innerProperties(propName).PropertyType.IsValueType OrElse _
                            innerProperties(propName).PropertyType Is GetType(String) Then
                            dr = dt.NewRow()
                            dr("Item") = propName
                            dr("Token") = prefix & propName
                            dt.Rows.Add(dr)
                        End If
                    End If

                Else
                    If innerProperties(propName).PropertyType.IsValueType OrElse _
                        innerProperties(propName).PropertyType Is GetType(String) Then
                        dr = dt.NewRow()
                        dr("Item") = propName
                        dr("Token") = prefix & propName
                        dt.Rows.Add(dr)

                    Else
                        If levelNb < 3 Then
                            GetTokenDatatable(dt, prefix & propName, innerProperties(propName).PropertyType, _
                                                                 levelNb + 1)
                        Else
                            dr = dt.NewRow()
                            dr("Item") = propName
                            dr("Token") = prefix & propName & ":<Any Property Name>..."
                            dt.Rows.Add(dr)
                        End If
                    End If
                End If

            Next

        End Sub

        ''' <summary>
        ''' Adds token help
        ''' </summary>
        ''' <param name="parentControl"></param>
        ''' <remarks></remarks>
        Public Sub InsertTokenHelp(ByVal parentControl As Control)

            Dim sectionContainer As Control = Nothing
            FormHelper.AddSection(parentControl, "TokenHelp", False, sectionContainer)
            Dim lit As New LiteralControl(GetTokenHelp)
            sectionContainer.Controls.Add(lit)
            'AddSection(parentControl, lit, "TokenHelp")

        End Sub

        ''' <summary>
        ''' Returns token help
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTokenHelp() As String

            Return GetTokenHelp(DefaultItemsTemplate, DefaultItemTemplate, DefaultTokenTemplate)

        End Function

        ''' <summary>
        ''' returns token help
        ''' </summary>
        ''' <param name="itemsTemplate"></param>
        ''' <param name="itemTemplate"></param>
        ''' <param name="itemTokenTemplate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTokenHelp(ByVal itemsTemplate As String, ByVal itemTemplate As String, ByVal itemTokenTemplate As String) As String

            Dim key As Integer = Me.PropertySource.Keys.GetHashCode
            Dim toReturn As String = GetGlobal(Of String)(key.ToString(CultureInfo.InvariantCulture), _
                                                            itemTemplate.GetHashCode.ToString( _
                                                                                               CultureInfo. _
                                                                                                  InvariantCulture))
            If String.IsNullOrEmpty(toReturn) Then
                If String.IsNullOrEmpty(itemsTemplate) Then
                    itemsTemplate = DefaultItemsTemplate
                End If
                If String.IsNullOrEmpty(itemTemplate) Then
                    itemTemplate = DefaultItemTemplate
                End If
                If String.IsNullOrEmpty(itemTokenTemplate) Then
                    itemTokenTemplate = DefaultTokenTemplate
                End If

                Dim builder As New StringBuilder()
                Dim itemsHeader, itemsFooter, itemHeader, itemMiddle, itemFooter, tokenHeader, tokenFooter As String

                Dim itemsIndex As Integer = itemsTemplate.IndexOf(ItemsTemplateToken, StringComparison.OrdinalIgnoreCase)
                Dim itemIndex As Integer = itemTemplate.IndexOf(ItemTemplateToken, StringComparison.OrdinalIgnoreCase)
                Dim tokensIndex As Integer = itemTemplate.IndexOf(ItemTokensTemplateToken, StringComparison.OrdinalIgnoreCase)
                Dim tokenIndex As Integer = itemTokenTemplate.IndexOf(ItemTokenTemplateToken, StringComparison.OrdinalIgnoreCase)
                If itemsIndex = -1 Or itemIndex = -1 Or tokensIndex = -1 Or tokenIndex = -1 Then
                    Throw New ArgumentException("the templates provided for token system help content are invalid")
                End If

                itemsHeader = itemsTemplate.Substring(0, itemsIndex)
                itemsFooter = itemsTemplate.Substring(itemsIndex + ItemsTemplateToken.Length)
                itemHeader = itemTemplate.Substring(0, itemIndex)
                itemMiddle = itemTemplate.Substring(itemIndex + ItemTemplateToken.Length, tokensIndex - itemIndex - ItemTemplateToken.Length)
                itemFooter = itemTemplate.Substring(tokensIndex + ItemTokensTemplateToken.Length)
                tokenHeader = itemTokenTemplate.Substring(0, tokenIndex)
                tokenFooter = itemTokenTemplate.Substring(tokenIndex + ItemTokenTemplateToken.Length)

                builder.Append(itemsHeader)
                Dim sources As Dictionary(Of String, List(Of String)) = Me.GetTokenSources()
                Dim prefix As String
                For Each item As String In sources.Keys
                    builder.Append(itemHeader)
                    builder.Append(item)
                    builder.Append(itemMiddle)
                    If item <> No_Object Then
                        prefix = item & ":"
                    Else
                        prefix = ""
                    End If
                    For Each prop As String In sources(item)
                        builder.Append(tokenHeader)
                        builder.Append(prefix & HtmlEncode(prop))
                        builder.Append(tokenFooter)
                    Next
                    builder.Append(itemFooter)
                Next
                builder.Append(itemsFooter)

                toReturn = builder.ToString()
                SetGlobal(Of String)(toReturn, key.ToString(CultureInfo.InvariantCulture), _
                                       itemTemplate.GetHashCode.ToString(CultureInfo.InvariantCulture))
            End If
            Return toReturn
        End Function


        Public Const DefaultItemsTemplate As String = "<ul>[Items]</ul>"
        Public Const DefaultItemTemplate As String = "<li><span class=""SubHead"">[Item]</span><ul class=""SubSubHead"">[Tokens]</ul></li>"
        Public Const DefaultTokenTemplate As String = "<li>[Token]</li>"

        Public Const ItemTemplateToken As String = "[Item]"
        Public Const ItemsTemplateToken As String = "[Items]"
        Public Const ItemTokensTemplateToken As String = "[Tokens]"
        Public Const ItemTokenTemplateToken As String = "[Token]"




#End Region


#Region "Private methods"

        ''' <summary>
        ''' Returns token sources
        ''' </summary>
        ''' <param name="prefix"></param>
        ''' <param name="objectType"></param>
        ''' <param name="levelNb"></param>
        ''' <param name="membersDone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetTokenSources(ByVal prefix As String, ByVal objectType As Type, _
                                              ByVal levelNb As Integer, ByRef membersDone As List(Of MemberInfo)) As List(Of String)
            Dim toReturn As New List(Of String)
            Dim innerMembers As Dictionary(Of String, MemberInfo) = _
                    ReflectionHelper.GetMembersDictionary(objectType)
            If prefix <> "" Then
                prefix &= ":"c
            End If


            Dim objType As Type

            For Each memberName As String In innerMembers.Keys
                Dim objMember As MemberInfo = innerMembers(memberName)
                Dim params As New List(Of String)
                Dim tempParams() As ParameterInfo
                If objMember IsNot Nothing Then
                    Dim flag2 As Boolean = True
                    Dim customAttributes As Object() = objMember.GetCustomAttributes(GetType(BrowsableAttribute), True)
                    If (customAttributes.Length > 0) Then
                        Dim attribute As BrowsableAttribute = DirectCast(customAttributes(0), BrowsableAttribute)
                        If Not attribute.Browsable Then
                            flag2 = False
                        End If
                    End If
                    If Not flag2 Then
                        Continue For
                    End If
                    Select Case objMember.MemberType
                        Case MemberTypes.Property
                            Dim propInfo As PropertyInfo = DirectCast(objMember, PropertyInfo)
                            objType = propInfo.PropertyType
                            tempParams = propInfo.GetIndexParameters
                            For Each param As ParameterInfo In tempParams
                                params.Add(param.Name)
                            Next
                        Case MemberTypes.Field
                            Dim fInfo As FieldInfo = DirectCast(objMember, FieldInfo)
                            objType = fInfo.FieldType
                        Case MemberTypes.Method
                            Dim mInfo As MethodInfo = DirectCast(objMember, MethodInfo)
                            objType = mInfo.ReturnType
                            If objType Is Nothing OrElse mInfo.Name.StartsWith("get_") OrElse mInfo.Name.StartsWith("set_") _
                                OrElse mInfo.Name.StartsWith("add_") OrElse mInfo.Name.StartsWith("remove_") Then
                                Continue For
                            End If
                            tempParams = mInfo.GetParameters
                            For Each param As ParameterInfo In tempParams
                                params.Add(param.Name)
                            Next
                        Case Else
                            Continue For
                    End Select

                    Dim newPrefix As String = prefix & memberName
                    For Each param As String In params
                        newPrefix &= ":<" & param & ">"
                    Next
                    If objType.IsValueType OrElse objType Is GetType(String) Then
                        toReturn.Add(newPrefix)
                    Else

                        If levelNb < 3 Then
                            Try
                                If Not membersDone.Contains(objMember) Then
                                    toReturn.AddRange(GetTokenSources(newPrefix, objType, levelNb + 1, membersDone))
                                    membersDone.Add(objMember)
                                End If
                            Catch ex As Exception
                            End Try
                        Else
                            toReturn.Add(prefix & memberName & ":<Any Property Name>...")
                        End If
                    End If
                End If


            Next
            Return toReturn
        End Function


        ''' <summary>
        ''' Returns token sources
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetTokenSources() As Dictionary(Of String, List(Of String))
            Dim toReturn As New Dictionary(Of String, List(Of String))
            Dim membersDone As New List(Of MemberInfo)
            Dim deepAccessesToFilter As New Dictionary(Of String, DeepObjectPropertyAccess)

            For Each propertyAccessItemKey As String In Me.PropertySource.Keys

                toReturn(propertyAccessItemKey) = New List(Of String)
                Dim propertyAccessItem As IPropertyAccess = Me.PropertySource(propertyAccessItemKey)

                If TypeOf propertyAccessItem Is CulturePropertyAccess Then
                    toReturn(propertyAccessItemKey).AddRange([Enum].GetNames(GetType(CultureDropDownTypes)))
                ElseIf TypeOf propertyAccessItem Is CulturePropertyAccess _
                       OrElse TypeOf propertyAccessItem Is DictionaryPropertyAccess Then
                    If propertyAccessItemKey = No_Object Then
                        toReturn(propertyAccessItemKey).AddRange(Me.SimpleTokens.Keys)
                    Else
                        toReturn(propertyAccessItemKey).Add("<Any column/key name>")
                    End If

                ElseIf TypeOf propertyAccessItem Is DateTimePropertyAccess Then
                    toReturn(propertyAccessItemKey).Add("now")
                    toReturn(propertyAccessItemKey).Add("current")
                ElseIf TypeOf propertyAccessItem Is ArrayListPropertyAccess Then
                    toReturn(propertyAccessItemKey).Add("<Any valid index>")
                ElseIf TypeOf propertyAccessItem Is TicksPropertyAccess Then
                    toReturn(propertyAccessItemKey).Add("now")
                    toReturn(propertyAccessItemKey).Add("today")
                    toReturn(propertyAccessItemKey).Add("ticksperday")
                ElseIf TypeOf propertyAccessItem Is MembershipPropertyAccess Then
                    toReturn(propertyAccessItemKey).AddRange(GetTokenSources("", GetType(UserMembership), 3, membersDone))
                ElseIf TypeOf propertyAccessItem Is ProfilePropertyAccess Then
                    Dim profileProperties As ProfilePropertyDefinitionCollection = _
                            ProfileController.GetPropertyDefinitionsByPortal(Me.PortalSettings.PortalId)
                    For Each profProp As ProfilePropertyDefinition In profileProperties
                        toReturn(propertyAccessItemKey).Add(profProp.PropertyName)
                    Next
                    toReturn(propertyAccessItemKey).AddRange(GetTokenSources("", GetType(UserMembership), 3, membersDone))
                ElseIf TypeOf propertyAccessItem Is HostPropertyAccess Then
                    Dim hostSettings As Hashtable = SettingsController.FetchSettings(SettingsScope.HostSettings, -1)
                    For Each settingsKey As String In hostSettings
                        toReturn(propertyAccessItemKey).Add(settingsKey)
                    Next
                ElseIf TypeOf propertyAccessItem Is PropertyAccess Then
                    toReturn(propertyAccessItemKey).Add("<Any property's name from the underlying object>")
                ElseIf TypeOf propertyAccessItem Is DeepObjectPropertyAccess Then
                    deepAccessesToFilter(propertyAccessItemKey) = DirectCast(propertyAccessItem, DeepObjectPropertyAccess)
                Else
                    'the object probably is a self contained entity (still guess though)
                    toReturn(propertyAccessItemKey).AddRange(GetTokenSources("", DirectCast(propertyAccessItem, Object).GetType, 3, membersDone))
                End If

            Next



            Dim deepAccessesFilterLists As New Dictionary(Of String, Dictionary(Of String, Boolean))

            For level As Integer = 3 To 0 Step -1
                For Each propertyAccessItemKey As String In deepAccessesToFilter.Keys
                    If Not deepAccessesFilterLists.ContainsKey(propertyAccessItemKey) Then
                        deepAccessesFilterLists(propertyAccessItemKey) = New Dictionary(Of String, Boolean)
                    End If
                    Dim deepAccessFilterList As Dictionary(Of String, Boolean) = deepAccessesFilterLists(propertyAccessItemKey)
                    Dim propertyAccessItem As DeepObjectPropertyAccess = deepAccessesToFilter(propertyAccessItemKey)
                    Dim sources As List(Of String) = GetTokenSources("", DirectCast(propertyAccessItem, DeepObjectPropertyAccess).TokenSource.GetType(), level, membersDone)
                    For Each source As String In sources
                        deepAccessFilterList(source) = True
                    Next
                    deepAccessesFilterLists(propertyAccessItemKey) = deepAccessFilterList
                Next
            Next

            For Each propertyAccessItemKey As String In deepAccessesFilterLists.Keys
                Dim sources As Dictionary(Of String, Boolean) = deepAccessesFilterLists(propertyAccessItemKey)
                Dim sortedSources As New List(Of String)(sources.Keys)
                sortedSources.Sort()
                toReturn(propertyAccessItemKey).AddRange(sortedSources)
            Next

            Return toReturn
        End Function

        ''' <summary>
        ''' Replace delimiters
        ''' </summary>
        ''' <param name="sourceRegex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ReplaceDelims(ByVal sourceRegex As String) As String
            Return sourceRegex.Replace("DELIM_OPEN", Me.DelimOpen).Replace("DELIM_CLOSE", Me.DelimClose)
        End Function

#End Region

    End Class
End Namespace
