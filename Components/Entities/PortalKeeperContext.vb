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

        Public ReadOnly Property EnableStopWatch() As Boolean
            Get
                If Me._CurrentEngine IsNot Nothing Then
                    Return Me._CurrentEngine.EnableStopWatch
                Else
                    Return Me.CurrentFirewallConfig.EnableStopWatch
                End If
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
                Return Me.DnnContext.Portal.PortalAlias.PortalAliasID
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
            Me.Init(configRules, Nothing)
        End Sub

        Public Sub Init(ByVal configRules As RuleEngineSettings(Of TEngineEvents), ByVal userParams As IDictionary(Of String, Object))
            Me._CurrentEngine = configRules
            Dim vars As Dictionary(Of String, Object) = configRules.Variables.EvaluateVariables(Me, Me)
            If userParams IsNot Nothing Then
                For Each objPair As KeyValuePair(Of String, Object) In userParams
                    vars(objPair.Key) = objPair.Value
                Next
            End If
            For Each objVar As KeyValuePair(Of String, Object) In vars
                Me.SetVar(objVar.Key, objVar.Value)
            Next
        End Sub

        Public Function MatchRules(ByVal configRules As List(Of KeeperRule(Of TEngineEvents))) As List(Of KeeperRule(Of TEngineEvents))
            Dim toReturn As New List(Of KeeperRule(Of TEngineEvents))
            Dim objRule As KeeperRule(Of TEngineEvents)
            For i As Integer = 0 To configRules.Count - 1
                objRule = configRules(i)
                If objRule.Enabled _
                    AndAlso objRule.MatchingLifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = Me.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture) Then
                    Try
                        If objRule.Condition.Match(Me) Then
                            toReturn.Add(objRule)
                            If objRule.StopRule Then
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                        Dim xmlDump As String = ""
                        If Me._CurrentEngine.ExceptionDumpVars.Length > 0 Then
                            Dim dumpVars As List(Of String) = ParseStringList(Me._CurrentEngine.ExceptionDumpVars)
                            Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump(False, dumpVars)
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

        Public Property CurrentEngine() As RuleEngineSettings(Of TEngineEvents)
        

        Public Sub ProcessRules(ByVal objEvent As TEngineEvents, ByVal configRules As RuleEngineSettings(Of TEngineEvents), ByVal endSequence As Boolean)
            If Not Me._CurrentEngine Is configRules Then
                Me._CurrentEngine = configRules
            End If
            Dim enableStopWatch As Boolean = Me.EnableStopWatch
            If enableStopWatch Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} - {1} - Start", objEvent.ToString(CultureInfo.InvariantCulture), configRules.Name), _
                                            WorkingPhase.InProgress, False, False, -1, Me.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Me.CurrentEventStep = objEvent



            For Each matchedRule As KeeperRule(Of TEngineEvents) In Me.MatchRules(configRules.Rules)
                matchedRule.RunActions(Me)
            Next
            
            If enableStopWatch Then

                Dim objStep As StepInfo
                If endSequence AndAlso Me._CurrentEngine.LogDump Then

                    Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump()
                    Dim tempItems As New List(Of KeyValuePair(Of String, String))(dump.Count)
                    For Each objItem As KeyValuePair(Of String, Object) In dump
                        If objItem.Value IsNot Nothing Then
                            tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, ReflectionHelper.Serialize(objItem.Value).InnerXml))
                        Else
                            tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, ""))
                        End If
                    Next

                    objStep = New StepInfo(Debug.PKPDebugType, String.Format("End - {0} - {1}", objEvent.ToString(CultureInfo.InvariantCulture), configRules.Name), _
                                           WorkingPhase.EndOverhead, endSequence, False, -1, Me.FlowId, tempItems.ToArray)
                Else
                    objStep = New StepInfo(Debug.PKPDebugType, String.Format("End - {0} - {1}", objEvent.ToString(CultureInfo.InvariantCulture), configRules.Name), _
                                           WorkingPhase.EndOverhead, endSequence, False, -1, Me.FlowId)
                End If

                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
        End Sub


        Public Overrides Function GetInstance() As PortalKeeperContext(Of TEngineEvents)
            Return DnnContext.Current.GetService(Of PortalKeeperContext(Of TEngineEvents))()
        End Function


        Public Function GetDump(dumpAllVars As Boolean, dumpaVarList As ICollection(Of String)) As SerializableDictionary(Of String, Object)


            Return Me.GetDump(dumpAllVars, dumpaVarList, Nothing)
        End Function

        Public Function GetDump() As SerializableDictionary(Of String, Object)


            Return Me.GetDump(Me._CurrentEngine.DumpAllVars, Me._CurrentEngine.DumpVarList, Nothing)
        End Function

        Public Function GetDump(dumpAllVars As Boolean, dumpaVarList As ICollection(Of String), defaultValue As Object) As SerializableDictionary(Of String, Object)
            Dim toReturn As SerializableDictionary(Of String, Object)
            Dim dumpVar As Object = Nothing
            If dumpAllVars Then
                SyncLock (Items)
                    toReturn = New SerializableDictionary(Of String, Object)(Me.Items.Count)
                    For Each objVar As KeyValuePair(Of String, Object) In Me.Items
                        If objVar.Value IsNot Nothing Then
                            If Not (objVar.Key = "UserBot" OrElse objVar.Key = "User" OrElse (objVar.Key.StartsWith("Last"))) Then
                                toReturn(objVar.Key) = objVar.Value
                            End If
                        End If
                    Next
                End SyncLock
            Else
                toReturn = New SerializableDictionary(Of String, Object)(dumpaVarList.Count)
                For Each key As String In dumpaVarList
                    If key.IndexOf("."c) > -1 Then
                        Dim exp As New SimpleExpression(Of Object)(key)
                        Dim objVar As Object
                        Try
                            objVar = exp.Evaluate(Me, Me)
                        Catch
                            objVar = defaultValue
                        End Try
                        If objVar IsNot Nothing Then
                            toReturn(key) = objVar
                        End If
                    Else
                        If Me.Items.TryGetValue(key, dumpVar) Then
                            toReturn(key) = dumpVar
                        End If
                    End If
                Next
            End If

            Return toReturn
        End Function

        Public Function ComputeDynamicExpressions(inputExpressions As String, skipNull As Boolean) As SerializableDictionary(Of String, Object)
            Dim toReturn As New SerializableDictionary(Of String, Object)
            Dim fieldNames As Dictionary(Of String, String) = ParsePairs(inputExpressions, True)
            Dim dump As SerializableDictionary(Of String, Object) = Me.GetDump(False, fieldNames.Keys)
            Dim fieldName As String = Nothing
            For Each dumpVar As KeyValuePair(Of String, Object) In dump
                If fieldNames.TryGetValue(dumpVar.Key, fieldName) Then
                    If (Not skipNull) OrElse dumpVar.Value IsNot Nothing Then
                        toReturn(fieldName) = dumpVar.Value
                    End If
                End If
            Next
            Return toReturn
        End Function

#End Region

    End Class
End Namespace


