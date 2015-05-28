Imports Aricie.DNN.Settings
Imports Aricie.DNN.Services
Imports System.Globalization
Imports Aricie.DNN.Diagnostics
Imports Aricie.Collections
Imports Aricie.DNN.Services.Flee
Imports Aricie.Services


'===============================================================================
'HISTORY:
'-------------------------------------------------------------------------------
' 18/04/2011 - [JBB] - Modification du RequestOutOfScope (pb de return)
'-------------------------------------------------------------------------------




Namespace Aricie.DNN.Modules.PortalKeeper


    <Serializable()> _
    Public Class PortalKeeperContext(Of TEngineEvents As IConvertible)
        Inherits ContextBase(Of PortalKeeperContext(Of TEngineEvents))



#Region "Constants"

        Public Const MODULE_NAME As String = "Aricie.PortalKeeper"

#End Region

#Region "Private members"


        Private _Disabled As Boolean = False
        Private _PortalKeeperConfig As PortalKeeperConfig
        Private _CurrentFirewallConfig As FirewallConfig
        Friend beginRequestPassed As Boolean


        Private Shared _PortalKeeperModuleDefId As Integer = -1
        Private Shared _SharedResourceFile As String = ""

        'Private _MatchedRules As New List(Of KeeperRule(Of TEngineEvents))
        Private _CurrentEventStep As TEngineEvents
        Private _PreviousSteps As New List(Of TEngineEvents)


        'Private Shared _PortalUrlSettings As New Dictionary(Of Integer, FriendlierUrlInfo)

        Private Shared ReadOnly Property PortalKeeperModuleDefId() As Integer
            Get
                If (_PortalKeeperModuleDefId = -1) Then
                    _PortalKeeperModuleDefId = GetModuleDefIdByModuleName(MODULE_NAME)
                End If
                Return _PortalKeeperModuleDefId
            End Get
        End Property

        'Private _EnableStopWatch As Nullable(Of Boolean)

        'Public ReadOnly Property EnableLogs As Boolean
        '    Get
        '        If Me._CurrentEngine IsNot Nothing Then
        '            Return Me._CurrentEngine.EnableStopWatch OrElse Me._CurrentEngine.EnableSimpleLogs
        '        Else
        '            Return Me.CurrentFirewallConfig.EnableStopWatch OrElse Me.CurrentFirewallConfig.EnableSimpleLogs
        '        End If
        '    End Get
        'End Property


        'Public ReadOnly Property EnableStopWatch() As Boolean
        '    Get
        '        If Me._CurrentEngine IsNot Nothing Then
        '            Return Me._CurrentEngine.EnableStopWatch
        '        Else
        '            Return Me.CurrentFirewallConfig.EnableStopWatch
        '        End If
        '    End Get
        'End Property


        Public ReadOnly Property LoggingLevel As LoggingLevel
            Get
                If Me.CurrentEngine IsNot Nothing Then
                    Return Me.CurrentEngine.LoggingLevel
                End If
                Return Me.CurrentFirewallConfig.LoggingLevel
            End Get
        End Property

        Private _RequestScopeOutOf As Nullable(Of Boolean)

        Public ReadOnly Property RequestOutOfScope() As Boolean
            Get
                If Not _RequestScopeOutOf.HasValue Then
                    If Not Me.CurrentFirewallConfig.RequestIsInScope(Me.DnnContext.HttpContext) Then
                        Me._RequestScopeOutOf = True
                    Else
                        Me._RequestScopeOutOf = False
                    End If
                    'Return False
                End If
                Return _RequestScopeOutOf.Value
            End Get
        End Property


#End Region

#Region "Instance Properties"

        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal value As Boolean)
                _Disabled = value
            End Set
        End Property

        'be aware that only the host config is returned before DNN Begin_Request is passed
        Public ReadOnly Property CurrentFirewallConfig() As FirewallConfig
            Get
                If _CurrentFirewallConfig Is Nothing Then
                    Dim globalConfig As FirewallConfig = Me.GlobalConfig.FirewallConfig
                    If Not beginRequestPassed Then
                        Return globalConfig
                    End If
                    If globalConfig.EnablePortalLevelSettings Then
                        Dim portalConfig As FirewallSettings = _
                            GetModuleSettings(Of FirewallSettings)(SettingsScope.PortalSettings, PortalId)
                        _CurrentFirewallConfig = globalConfig.GetMerge(portalConfig)
                    Else
                        _CurrentFirewallConfig = globalConfig
                    End If
                End If
                Return _CurrentFirewallConfig
            End Get
        End Property

        Public ReadOnly Property GlobalConfig() As PortalKeeperConfig
            Get
                If _PortalKeeperConfig Is Nothing Then
                    _PortalKeeperConfig = PortalKeeperConfig.Instance
                End If
                Return _PortalKeeperConfig
            End Get
        End Property


        Public ReadOnly Property CurrentAliasId() As Integer
            Get
                Return Me.DnnContext.PortalAlias.PortalAliasID
            End Get
        End Property


        Friend Property CurrentEventStep() As TEngineEvents
            Get
                Return _CurrentEventStep
            End Get
            Set(ByVal value As TEngineEvents)
                If _CurrentEventStep IsNot Nothing AndAlso Not _CurrentEventStep.Equals(value) Then
                    SyncLock _PreviousSteps
                        _PreviousSteps.Add(_CurrentEventStep)
                        _CurrentEventStep = value
                    End SyncLock
                End If
            End Set
        End Property




        'Public Property MatchedRules() As List(Of KeeperRule(Of TEngineEvents))
        '    Get
        '        Return _MatchedRules
        '    End Get
        '    Set(ByVal value As List(Of KeeperRule(Of TEngineEvents)))
        '        _MatchedRules = value
        '    End Set
        'End Property

        Public ReadOnly Property PreviousSteps() As List(Of TEngineEvents)
            Get
                Return _PreviousSteps
            End Get
        End Property

        Friend Property CurrentRule As KeeperRule(Of TEngineEvents)

        Public Property CurrentEngine() As RuleEngineSettings(Of TEngineEvents)

        Public ReadOnly Property ActionContext As PortalKeeperContext(Of TEngineEvents)
            Get
                Return Me
            End Get
        End Property





#End Region

#Region "Shared Properties"

        'Public Shared ReadOnly Property Instance() As PortalKeeperContext
        '    Get

        '        Return DnnContext.Current().GetService(Of PortalKeeperContext)()

        '    End Get
        'End Property

        'Public Shared ReadOnly Property Instance(ByVal context As HttpContext) As PortalKeeperContext
        '    Get

        '        Return DnnContext.Current(context).GetService(Of PortalKeeperContext)()

        '    End Get
        'End Property

        Public Shared ReadOnly Property SharedResourceFile() As String
            Get
                If _SharedResourceFile = "" Then
                    _SharedResourceFile = GetModuleSharedResourceFile(PortalKeeperModuleDefId)
                End If
                Return _SharedResourceFile
            End Get
        End Property




#End Region

#Region "Public Methods"

        Friend Sub SetEngine(newEngine As RuleEngineSettings(Of TEngineEvents))
            Me._CurrentEngine = newEngine
        End Sub


        Public Sub Init(ByVal configRules As RuleEngineSettings(Of TEngineEvents))
            Me._CurrentEngine = configRules
            Me.InitParams(configRules.Variables)
        End Sub


        Public Overloads Sub InitParams(ByVal params As VariablesBase)
            If params IsNot Nothing Then
                'Me.LogStart("Init Params", PortalKeeper.LoggingLevel.Steps, False)
                Dim vars As Dictionary(Of String, Object) = params.EvaluateVariables(Me, Me)
                Me.InitParams(vars)
                'Me.LogEnd("Init Params", False, False, PortalKeeper.LoggingLevel.Steps, Nothing)
            End If
        End Sub


        Public Overloads Sub InitParams(ByVal params As IDictionary(Of String, Object))
            If params IsNot Nothing Then
                For Each objPair As KeyValuePair(Of String, Object) In params
                    Me.SetVar(objPair.Key, objPair.Value)
                Next
            End If
        End Sub

        Public Sub ProcessRules(ByVal objEvent As TEngineEvents, ByVal configRules As RuleEngineSettings(Of TEngineEvents), ByVal endSequence As Boolean, ByVal endoverhead As Boolean)
            If Me._CurrentEngine IsNot configRules Then
                Me._CurrentEngine = configRules
            End If
            Me.CurrentEventStep = objEvent
            If configRules.Mode = RuleEngineMode.Rules Then
                Me.LogStartEventStep()
                Dim matchedRules = Me.MatchRules(configRules.Rules)
                If matchedRules.Count > 0 Then
                    For Each matchedRule As KeeperRule(Of TEngineEvents) In matchedRules
                        matchedRule.RunActions(Me)
                    Next
                End If
                Me.LogEndEventStep(False, endoverhead)
            Else
                If configRules.InitialCondition.Instances.Count = 0 OrElse configRules.InitialCondition.Match(Me) Then
                    Me.LogStartEventStep()
                    configRules.Actions.Run(Me)
                    Me.LogEndEventStep(False, endoverhead)
                End If
            End If
            If endSequence Then
                Me.LogEndEngine()
            End If
        End Sub


       

        Public Function MatchRules(ByVal configRules As List(Of KeeperRule(Of TEngineEvents))) As List(Of KeeperRule(Of TEngineEvents))
            Dim toReturn As New List(Of KeeperRule(Of TEngineEvents))
            Dim objRule As KeeperRule(Of TEngineEvents)
            For i As Integer = 0 To configRules.Count - 1
                objRule = configRules(i)
                If objRule.Enabled _
                    AndAlso objRule.MatchingLifeCycleEvent.Equals(Me.CurrentEventStep) Then
                    'AndAlso objRule.MatchingLifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = Me.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture) Then
                    Try
                        If objRule.Condition.Match(Me) Then
                            toReturn.Add(objRule)
                            If objRule.StopRule Then
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                        Dim xmlDump As String = ""
                        If Me._CurrentEngine.ExceptionDumpSettings.EnableDump Then
                            Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump(Me._CurrentEngine.ExceptionDumpSettings)
                            xmlDump = ReflectionHelper.Serialize(dump).InnerXml
                        End If
                        Dim message As String = String.Format("Rule Condition Exception, Engine Name: {0}, Rule Name: {1}, Dumped Vars: {2}, InnerException: ", Me._CurrentEngine.Name, objRule.Name, xmlDump)
                        Dim newEx As New ApplicationException(message, ex)
                        DotNetNuke.Services.Exceptions.LogException(newEx)
                    End Try
                End If
            Next
            Return toReturn
        End Function

        'Private _CurrentEngine As RuleEngineSettings(Of TEngineEvents)



        Public Function GetAdvancedTokenReplace() As AdvancedTokenReplace
            Dim objTokenReplace As New AdvancedTokenReplace()
            objTokenReplace.SetObjectReplace(Me, "Context")
            objTokenReplace.SetObjectReplace(Me.Items, "Items")
            If ActionContext.CurrentRule IsNot Nothing Then
                objTokenReplace.SetObjectReplace(Me.CurrentRule, "Rule")
            End If
            Return objTokenReplace
        End Function



        Public Overrides Function GetInstance() As PortalKeeperContext(Of TEngineEvents)
            Return DnnContext.Current.GetService(Of PortalKeeperContext(Of TEngineEvents))()
        End Function

        Private Sub LogStartEventStep(ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.LogStart(Me.CurrentEventStep, LoggingLevel.Steps, False, additionalProperties)
        End Sub

        Public Sub LogStart(eventStep As IConvertible, minLevel As LoggingLevel, endInnerCode As Boolean, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            If (Me.LoggingLevel >= minLevel) Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{{0}} - {0} - Start", _CurrentEngine.Name), _
                                            WorkingPhase.InProgress, False, False, -1, Me.FlowId, additionalProperties)
                objStep.LabelInsert = eventStep
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
        End Sub

        Public Sub LogStartEngine(ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.LogStart("Agent", LoggingLevel.Simple, False, additionalProperties)
        End Sub

        Private Sub LogEndEventStep(ByVal endSequence As Boolean, ByVal endoverhead As Boolean)
            Me.LogEnd(Me._CurrentEventStep, endSequence, endoverhead, LoggingLevel.Steps, Nothing)
        End Sub

        Public Sub LogEndEngine(ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.LogEnd("Agent", True, True, LoggingLevel.Simple, Me.CurrentEngine.LogEndDumpSettings, additionalProperties)
        End Sub

        Public Sub LogEnd(eventStep As IConvertible, ByVal endSequence As Boolean, ByVal endoverhead As Boolean, minLevel As LoggingLevel, ByVal objDumpSettings As DumpSettings, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            If (Me.LoggingLevel >= minLevel) Then
                Dim tempItems As New List(Of KeyValuePair(Of String, String))()
                'If endSequence AndAlso Me._CurrentEngine.LogDump Then

                '    Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump()

                '    tempItems = DumpToLogs(dump)
                'End If
                If objDumpSettings IsNot Nothing AndAlso objDumpSettings.EnableDump Then
                    Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump(objDumpSettings)
                    tempItems = DumpToLogs(dump)
                End If
                Dim logTitle As String
               
                Dim nextPhase As WorkingPhase = WorkingPhase.InProgress
                If endoverhead Then
                    nextPhase = WorkingPhase.EndOverhead
                End If
                tempItems.AddRange(additionalProperties)
                Dim objStep As StepInfo 
                If eventStep Is Nothing Then
                    logTitle = String.Format("End - {0}", _CurrentEngine.Name)
                    objStep = New StepInfo(Debug.PKPDebugType, logTitle, _
                                          nextPhase, endSequence, False, -1, Me.FlowId, tempItems.ToArray)
                Else
                    logTitle = String.Format("End - {{0}} - {0}", _CurrentEngine.Name)
                    objStep = New StepInfo(Debug.PKPDebugType, logTitle, _
                                          nextPhase, endSequence, False, -1, Me.FlowId, tempItems.ToArray)
                    objStep.LabelInsert = eventStep
                End If
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
        End Sub


        Public Function DumpToLogs(dump As Dictionary(Of String, Object)) As List(Of KeyValuePair(Of String, String))
            Dim tempItems As New List(Of KeyValuePair(Of String, String))()

            For Each objItem As KeyValuePair(Of String, Object) In dump
                If objItem.Value IsNot Nothing Then
                    Dim serialized As String
                    Try
                        serialized = ReflectionHelper.Serialize(objItem.Value).Beautify()
                    Catch ex As Exception
                        serialized = ex.ToString()
                    End Try
                    tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, serialized))
                Else
                    tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, ""))
                End If
            Next
            Return tempItems
        End Function


      
        Public Function GetDump() As SerializableDictionary(Of String, Object)


            Return Me.GetDump(Me.CurrentEngine.LogEndDumpSettings)
        End Function

        Public Function GetDump(ByVal objDumpSettings As DumpSettings) As SerializableDictionary(Of String, Object)
            Dim toReturn As SerializableDictionary(Of String, Object) = Nothing
            If objDumpSettings.EnableDump Then
                Dim dumpVar As Object = Nothing
                If objDumpSettings.DumpAllVars Then
                    SyncLock (Items)
                        toReturn = New SerializableDictionary(Of String, Object)(Me.Items.Count)
                        For Each objVar As KeyValuePair(Of String, Object) In Me.Items
                            If objVar.Value IsNot Nothing Then
                                If Not (objVar.Key = "UserBot" OrElse objVar.Key = "User") Then
                                    toReturn(objVar.Key) = objVar.Value
                                End If
                            End If
                        Next
                    End SyncLock
                Else
                    toReturn = New SerializableDictionary(Of String, Object)(objDumpSettings.DumpVarList.Count)
                    For Each key As String In objDumpSettings.DumpVarList
                        If key.IndexOf("="c) > -1 Then
                            Dim splitVar() As String = key.Split("="c)
                            If splitVar.Length = 2 Then
                                Dim newKey As String = splitVar(0)
                                Dim exp As New SimpleExpression(Of Object)(splitVar(1))
                                Dim objValue As Object
                                Try
                                    objValue = exp.Evaluate(Me, Me)
                                Catch
                                    objValue = objDumpSettings.DefaultValue.Evaluate(Me, Me)
                                End Try
                                If objValue Is Nothing Then
                                    If Not objDumpSettings.SkipNull Then
                                        toReturn(newKey) = objDumpSettings.DefaultValue.Evaluate(Me, Me)
                                    End If
                                Else
                                    toReturn(newKey) = objValue
                                End If
                            End If
                        ElseIf key.IndexOf("."c) > -1 Then
                            Dim exp As New SimpleExpression(Of Object)(key)
                            Dim objValue As Object
                            Try
                                objValue = exp.Evaluate(Me, Me)
                            Catch
                                objValue = objDumpSettings.DefaultValue.Evaluate(Me, Me)
                            End Try
                            If objValue Is Nothing Then
                                If Not objDumpSettings.SkipNull Then
                                    toReturn(key) = objDumpSettings.DefaultValue.Evaluate(Me, Me)
                                End If
                            Else
                                toReturn(key) = objValue
                            End If
                        Else
                            If Me.Items.TryGetValue(key, dumpVar) Then
                                toReturn(key) = dumpVar
                            ElseIf Not objDumpSettings.SkipNull Then
                                toReturn(key) = objDumpSettings.DefaultValue.Evaluate(Me, Me)
                            End If
                        End If


                    Next
                End If
                'Else
                '    toReturn = New SerializableDictionary(Of String, Object)
            End If


            Return toReturn
        End Function


#End Region

    End Class
End Namespace


