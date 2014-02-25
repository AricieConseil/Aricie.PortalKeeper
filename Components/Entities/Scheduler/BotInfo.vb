Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Diagnostics
Imports Aricie.Collections
Imports DotNetNuke.Common
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Services
Imports System.Globalization
Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.Services.Flee
Imports System.Threading
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Settings
Imports FileHelper = Aricie.DNN.Services.FileHelper

Namespace Aricie.DNN.Modules.PortalKeeper




    <ActionButton(IconName.Android, IconOptions.Normal)> _
    <DefaultProperty("Summary")> _
    <Serializable()> _
    Public Class BotInfo(Of TEngineEvent As IConvertible)
        Inherits RuleEngineSettings(Of TEngineEvent)

#Region "Private members"


        'Private _Enabled As Boolean = False

        'Private _Period As TimeSpan = TimeSpan.FromMinutes(10)
        Private _Schedule As New STimeSpan(TimeSpan.FromMinutes(10))

        'Private _ApiUrl As String

        Private _ForceRun As Boolean



        Private _LogPath As String = "PortalKeeper/"

        Private _RetainHistoryNb As Integer = 5

        'Private _DumpAllVars As Boolean = True
        'Private _DumpVariables As String = ""

        Private _IncludeLastDump As Boolean

        Private _BotHistory As New WebBotHistory


        Private _RunningServers As String = ""





        <NonSerialized()> _
        Private Shared _AsyncLocks As New Dictionary(Of String, Dictionary(Of Integer, String))


        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property AsyncLockBot() As Dictionary(Of Integer, String)
            Get
                Dim toReturn As Dictionary(Of Integer, String) = Nothing
                If Not _AsyncLocks.TryGetValue(Me.Name, toReturn) Then
                    SyncLock _AsyncLocks
                        If Not _AsyncLocks.TryGetValue(Me.Name, toReturn) Then
                            toReturn = New Dictionary(Of Integer, String)
                            _AsyncLocks(Me.Name) = toReturn
                        End If
                    End SyncLock
                End If
                Return toReturn
            End Get
        End Property


#End Region


#Region "Public Properties"

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property Summary As String
            Get
                Dim enableString As String = "Master " & IIf(Me.Enabled AndAlso Not Me.MasterBotDisabled, "Enabled", "Disabled").ToString
                Dim userString As String = "User bot"
                Dim targetUserBot As UserBotSettings(Of ScheduleEvent) = Nothing
                For Each objUserBot As UserBotSettings(Of ScheduleEvent) In PortalKeeperConfig.Instance.SchedulerFarm.UserBots.Instances
                    If objUserBot.BotName = Me.Name Then
                        targetUserBot = objUserBot
                    End If
                Next
                If targetUserBot Is Nothing Then
                    userString = "No " & userString
                Else
                    userString &= IIf(Me.Enabled AndAlso PortalKeeperConfig.Instance.SchedulerFarm.EnableUserBots AndAlso targetUserBot.Enabled, " Enabled", " Disabled").ToString
                End If
                Dim forcedString As String = IIf(Me._ForceRun, "Forced", "Unforced").ToString
                Return Me.Name.PadRight(50) & " - " & enableString.PadRight(20) & " - " & userString.PadRight(20) & " - " & forcedString.PadRight(10) & " - " & Me.Schedule.FormattedDuration
            End Get
        End Property


        <ExtendedCategory("")> _
      <XmlIgnore()> _
        Public ReadOnly Property MasterBotDisabled() As Boolean
            Get
                For Each objUserBotSettings As UserBotSettings(Of ScheduleEvent) In PortalKeeperConfig.Instance.SchedulerFarm.AvailableUserBots.Values
                    If objUserBotSettings.BotName = Me.Name AndAlso objUserBotSettings.DisableTemplateBot Then
                        Return True
                    End If
                Next
                Return False
            End Get
        End Property


        <ExtendedCategory("")> _
        Public Property ForceRun() As Boolean
            Get
                Return _ForceRun
            End Get
            Set(ByVal value As Boolean)
                _ForceRun = value
            End Set
        End Property


        '<XmlIgnore()> _
        '<Browsable(False)> _
        'Public Property Period() As TimeSpan
        '    Get
        '        Return _Period
        '    End Get
        '    Set(ByVal value As TimeSpan)
        '        _Period = value
        '    End Set
        'End Property



        <ExtendedCategory("Schedule")> _
        <SortOrder(2)> _
        Public Property Schedule() As STimeSpan
            Get
                Return Me._Schedule
            End Get
            Set(ByVal value As STimeSpan)
                Me._Schedule = value
            End Set
        End Property

        '<Obsolete("Only for xml retrocompatibility")> _
        'Can't set the attribute or the xmlserializer won't take it into account
        <Browsable(False)> _
        Public Property PeriodMinutes() As Integer
            Get
                Return 0
            End Get
            Set(ByVal value As Integer)
                If value <> 0 Then
                    Me._Schedule.Value = TimeSpan.FromMinutes(value)
                End If
            End Set
        End Property

        '<ExtendedCategory("Engine")> _
        '<Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        '    <LabelMode(LabelMode.Top)> _
        'Public Property BotEngine() As RuleEngineSettings(Of BotEvent)
        '    Get
        '        Return _BotEngine
        '    End Get
        '    Set(ByVal value As RuleEngineSettings(Of BotEvent))
        '        _BotEngine = value
        '    End Set
        'End Property

        '<CollectionEditor(True, False, False, True, 10, CollectionDisplayStyle.List)> _

        'Private _UserName As String = ""

        '<Browsable(False)> _
        'Public Property UserName() As String
        '    Get
        '        Return _UserName
        '    End Get
        '    Set(ByVal value As String)
        '        _UserName = value
        '    End Set
        'End Property







        <ExtendedCategory("History")> _
        Public Property LogPath() As String
            Get
                Return _LogPath
            End Get
            Set(ByVal value As String)
                _LogPath = value
            End Set
        End Property

        Private Function GetLogMapPath() As String
            Dim botName As String = Me.Name
            'If Not String.IsNullOrEmpty(Me._UserName) Then
            '    botName &= "-" & Me._UserName.Trim
            'End If
            Return FileHelper.GetAbsoluteMapPath(Me._LogPath & "BotHistory-" & botName & ".config", True)
        End Function

        <Required(True)> _
            <ExtendedCategory("History")> _
        Public Property RetainHistoryNb() As Integer
            Get
                Return _RetainHistoryNb
            End Get
            Set(ByVal value As Integer)
                _RetainHistoryNb = value
            End Set
        End Property

        <ExtendedCategory("History")> _
        Public Property AddDumpToHistory() As Boolean

        <ExtendedCategory("History")> _
         <ConditionalVisible("AddDumpToHistory", False, True)> _
        Public Property HistoryDumpAllVars() As Boolean

        <ExtendedCategory("History")> _
         <ConditionalVisible("AddDumpToHistory", False, True)> _
          <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
            LineCount(4), Width(500)> _
        Public Property HistoryDumpVariables() As String = ""


        Private _HistorydumpVarLock As New Object
        Private _HistoryDumpVarList As List(Of String)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property HistoryDumpVariablesDumpVarList() As List(Of String)
            Get
                If _HistoryDumpVarList Is Nothing Then
                    SyncLock _HistorydumpVarLock
                        If _HistoryDumpVarList Is Nothing Then
                            '_DumpVarList = New List(Of String)
                            'Dim strVars As String() = Me._DumpVariables.Split(","c)
                            'For Each strVar As String In strVars
                            '    If strVar.Trim <> "" Then
                            '        _DumpVarList.Add(strVar.Trim())
                            '    End If
                            'Next
                            _HistoryDumpVarList = ParseStringList(Me.HistoryDumpVariables)
                        End If
                    End SyncLock
                End If
                Return _HistoryDumpVarList
            End Get
        End Property

        <ConditionalVisible("AddDumpToHistory", False, True)> _
       <ExtendedCategory("History")> _
        Public Property IncludeLastDump() As Boolean
            Get
                Return _IncludeLastDump
            End Get
            Set(ByVal value As Boolean)
                _IncludeLastDump = value
            End Set
        End Property


        <XmlIgnore()> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <IsReadOnly(True)> _
            <ExtendedCategory("History")> _
        Public ReadOnly Property BotHistory() As WebBotHistory
            Get
                Return LoadFileSettings(Of WebBotHistory)(GetLogMapPath(), True)
            End Get
        End Property

        Public Sub SaveHistory(ByVal botHistory As WebBotHistory, ByVal runContext As PortalKeeperContext(Of TEngineEvent))
            SaveFileSettings(Of WebBotHistory)(GetLogMapPath(), botHistory)
        End Sub

        <ExtendedCategory("TechnicalSettings")> _
         <Width(300)> _
            <LineCount(2)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RunningServers() As String
            Get
                Return _RunningServers
            End Get
            Set(ByVal value As String)
                _RunningServers = value.Trim()
            End Set
        End Property


        Private _UseTaskQueue As Boolean
        Private _TaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100))
        <NonSerialized()> _
        Private WithEvents _AsynchronousRunTaskQueue As TaskQueue(Of BotRunContext(Of TEngineEvent))


        <ExtendedCategory("TechnicalSettings")> _
        Public Property UseTaskQueue() As Boolean
            Get
                Return _UseTaskQueue
            End Get
            Set(ByVal value As Boolean)
                _UseTaskQueue = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        Public Property UseMutex() As Boolean

        <ConditionalVisible("UseMutex", False, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property SynchronisationTimeout() As New STimeSpan(TimeSpan.FromSeconds(2))

        <ExtendedCategory("TechnicalSettings")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ConditionalVisible("UseTaskQueue", False, True)> _
        Public Property TaskQueueInfo() As TaskQueueInfo
            Get
                Return _TaskQueueInfo
            End Get
            Set(ByVal value As TaskQueueInfo)
                _TaskQueueInfo = value
            End Set
        End Property


        Private ReadOnly Property AsynchronousRunTaskQueue() As TaskQueue(Of BotRunContext(Of TEngineEvent))
            Get
                If _AsynchronousRunTaskQueue Is Nothing Then
                    _AsynchronousRunTaskQueue = New TaskQueue(Of BotRunContext(Of TEngineEvent))(AddressOf InternalRun, Me._TaskQueueInfo)
                    'AddHandler _PersisterTaskQueue.ActionPerformed, AddressOf Me.PersisterTaskQueue_ActionPerformed
                End If
                Return _AsynchronousRunTaskQueue
            End Get
        End Property



        'Public Overrides Sub ProcessRules(ByVal context As PortalKeeperContext(Of TEngineEvent), ByVal objEvent As TEngineEvent, ByVal endSequence As Boolean)
        '    MyBase.ProcessRules(context, objEvent, endSequence)
        'End Sub



#End Region


#Region "Public methods"

        Public Function RunBot(ByVal botContext As BotRunContext(Of TEngineEvent), ByVal forceRun As Boolean) As Boolean
            Dim toReturn As Boolean
            If Me.MatchServer Then
                Dim asyncbotLocks As Dictionary(Of Integer, String) = AsyncLockBot
                If Not asyncbotLocks.ContainsKey(botContext.AsyncLockId) Then
                    If (forceRun AndAlso Me._ForceRun) _
                    OrElse (Not forceRun AndAlso Me.Enabled AndAlso botContext.History.LastRun.Add(Me.Schedule.Value) <= Now) Then
                        Dim runUnlocked As Boolean
                        SyncLock _AsyncLocks
                            If Not asyncbotLocks.ContainsKey(botContext.AsyncLockId) Then
                                asyncbotLocks(botContext.AsyncLockId) = botContext.Id
                                runUnlocked = True
                            End If
                        End SyncLock
                        If runUnlocked Then
                            If Not Me._UseTaskQueue Then
                                toReturn = Me.InternalRun(botContext)
                            Else
                                Me.AsynchronousRunTaskQueue.EnqueueTask(botContext)
                                toReturn = True
                            End If
                        End If
                    End If
                End If
            End If
            Return toReturn
        End Function

#End Region



#Region "Private methods"
        <NonSerialized()> _
        Private _ServerList As List(Of String)

        Private Function MatchServer() As Boolean
            If _ServerList Is Nothing Then
                _ServerList = New List(Of String)
                For Each strServer As String In Me._RunningServers.Split(","c)
                    If strServer.Trim <> "" Then
                        _ServerList.Add(strServer.Trim.ToUpper)
                    End If
                Next
            End If
            Return _ServerList.Count = 0 OrElse _ServerList.Contains(ServerName.ToUpper)
        End Function

        Private Function InternalRun(ByVal botContext As BotRunContext(Of TEngineEvent)) As Boolean
            Dim toReturn As Boolean
            Try
                If Me.UseMutex Then
                    Using objMutex As New Mutex(False, "AsyncBot" & botContext.AsyncLockId.ToString(CultureInfo.InvariantCulture))
                        If objMutex.WaitOne(Me._SynchronisationTimeout.Value) Then
                            Try
                                InternalRunUnlocked(botContext)
                                toReturn = True
                            Catch ex As Exception
                                AsyncLogger.Instance.AddException(ex)
                            Finally
                                objMutex.ReleaseMutex()
                            End Try
                        End If
                    End Using
                Else
                    InternalRunUnlocked(botContext)
                    toReturn = True
                End If
            Catch ex As Exception
                AsyncLogger.Instance.AddException(ex)
            Finally
                Dim asyncbotLocks As Dictionary(Of Integer, String) = AsyncLockBot
                SyncLock _AsyncLocks
                    Dim lockId As String = Nothing
                    If asyncbotLocks.TryGetValue(botContext.AsyncLockId, lockId) Then
                        If lockId = botContext.Id Then
                            asyncbotLocks.Remove(botContext.AsyncLockId)
                        Else
                            Throw New ApplicationException("concurrence issue during bot execution")
                        End If
                    End If
                End SyncLock
            End Try
            Return toReturn
        End Function


        Private Sub InternalRunUnlocked(ByVal botContext As BotRunContext(Of TEngineEvent))
            Dim currentWebBotEvent As New BotInfoEvent

            If botContext.EngineContext Is Nothing Then
                botContext.EngineContext = New PortalKeeperContext(Of TEngineEvent)
                botContext.EngineContext.Init(Me, botContext.UserParams)
            End If

            Dim objEnableStopWatch As Boolean = Me.EnableStopWatch OrElse Me.EnableSimpleLogs
            If objEnableStopWatch Then
                'Dim objStep As New StepInfo(Debug.RequestTiming, "Start " & configRules.Name & " " & objEvent.ToString(CultureInfo.InvariantCulture), WorkingPhase.InProgress, False, False, -1, Me.FlowId)
                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} Start", Me.Name), _
                                            WorkingPhase.InProgress, False, False, -1, botContext.EngineContext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If

            If botContext.History.BotCallHistory.Count > Me.RetainHistoryNb Then
                botContext.History.BotCallHistory.RemoveRange(Me.RetainHistoryNb, botContext.History.BotCallHistory.Count - Me.RetainHistoryNb)
            End If


            If Me._IncludeLastDump Then
                If botContext.History.BotCallHistory.Count > 0 Then
                    For Each objVarPair As KeyValuePair(Of String, Object) In botContext.History.BotCallHistory(0).VariablesDump
                        botContext.UserParams("Last" & objVarPair.Key) = objVarPair.Value
                    Next
                End If
            End If

            Me.BatchRun(botContext.Events, botContext.EngineContext)

            'botContext.EngineContext.Init(Me, botContext.UserParams)
            'For Each eventStep As TEngineEvent In botContext.Events
            '    Me.ProcessRules(botContext.EngineContext, eventStep, False)
            'Next

            botContext.History.LastRun = currentWebBotEvent.Time
            botContext.History.BotCallHistory.Insert(0, currentWebBotEvent)

            If Me.AddDumpToHistory Then
                Dim dump As SerializableDictionary(Of String, Object) = botContext.EngineContext.GetDump(Me.HistoryDumpAllVars, Me.HistoryDumpVariablesDumpVarList)

                currentWebBotEvent.VariablesDump = dump
            End If

            'currentWebBotEvent.PayLoad = HtmlEncode(ReflectionHelper.Serialize(dump).OuterXml)



            currentWebBotEvent.Duration = Now.Subtract(currentWebBotEvent.Time)
            currentWebBotEvent.Success = True


            'If Me.BotStatsInfo Is Nothing OrElse Me.BotStatsInfo.Count = 0 Then
            '    Me.BotStatsInfo = New List(Of WebBotStatInfo)
            '    currentStats = New WebBotStatInfo
            'Else
            '    currentStats = Me.BotStatsInfo(0)
            'End If
            botContext.History.NumberOfBotCall += 1
            If currentWebBotEvent.Success Then
                botContext.History.NumberOfSucceedBotCall += 1
                If botContext.History.NumberOfSucceedBotCall > 1 Then
                    Dim totalWebBotDuration As TimeSpan = TimeSpan.FromTicks(botContext.History.AverageDuration.Value.Ticks * (botContext.History.NumberOfSucceedBotCall - 1))
                    totalWebBotDuration = totalWebBotDuration.Add(currentWebBotEvent.Duration)
                    botContext.History.AverageDuration.Value = TimeSpan.FromTicks(totalWebBotDuration.Ticks \ botContext.History.NumberOfSucceedBotCall)
                Else
                    botContext.History.AverageDuration.Value = currentWebBotEvent.Duration
                End If
                If currentWebBotEvent.Duration < botContext.History.MinDuration Then
                    botContext.History.MinDuration.Value = currentWebBotEvent.Duration
                End If
                If currentWebBotEvent.Duration > botContext.History.MaxDuration OrElse botContext.History.MaxDuration = TimeSpan.MaxValue Then
                    botContext.History.MaxDuration.Value = currentWebBotEvent.Duration
                End If
            End If
            'If Me.BotStatsInfo.Count = 0 Then
            '    Me.BotStatsInfo.Add(currentStats)
            'End If

            If botContext.RunEndDelegate IsNot Nothing Then
                If objEnableStopWatch Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} Finalizing", Me.Name), _
                                                WorkingPhase.InProgress, False, False, -1, botContext.EngineContext.FlowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                botContext.RunEndDelegate.Invoke(botContext.History, botContext.EngineContext)
            End If


            If objEnableStopWatch Then
                'Dim objStep As New StepInfo(Debug.RequestTiming, "Start " & configRules.Name & " " & objEvent.ToString(CultureInfo.InvariantCulture), WorkingPhase.InProgress, False, False, -1, Me.FlowId)
                Dim objStep As StepInfo
                If Me.LogDump Then
                    Dim dump As SerializableDictionary(Of String, Object) = botContext.EngineContext.GetDump()
                    Dim tempItems As New List(Of KeyValuePair(Of String, String))(dump.Count)
                    For Each objItem As KeyValuePair(Of String, Object) In dump
                        If objItem.Value IsNot Nothing Then
                            tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, ReflectionHelper.Serialize(objItem.Value).InnerXml))
                        Else
                            tempItems.Add(New KeyValuePair(Of String, String)(objItem.Key, ""))
                        End If
                    Next
                    objStep = New StepInfo(Debug.PKPDebugType, String.Format("End {0}", Me.Name), _
                                                WorkingPhase.InProgress, True, False, -1, botContext.EngineContext.FlowId, tempItems.ToArray)
                Else
                    objStep = New StepInfo(Debug.PKPDebugType, String.Format("End {0}", Me.Name), _
                                                WorkingPhase.InProgress, True, False, -1, botContext.EngineContext.FlowId)
                End If

                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
        End Sub


        'Private Function GetDump(ByVal context As PortalKeeperContext(Of TEngineEvent)) As SerializableDictionary(Of String, Object)
        '    Dim toReturn As SerializableDictionary(Of String, Object)
        '    Dim dumpVar As Object = Nothing
        '    If Me._DumpAllVars Then
        '        toReturn = New SerializableDictionary(Of String, Object)(context.Items.Count)
        '        For Each objVar As KeyValuePair(Of String, Object) In context.Items
        '            If Not (objVar.Key = "UserBot" OrElse (Me._IncludeLastDump AndAlso objVar.Key.StartsWith("Last"))) Then
        '                toReturn(objVar.Key) = objVar.Value
        '            End If
        '        Next
        '    Else
        '        toReturn = New SerializableDictionary(Of String, Object)(Me.DumpVarList.Count)
        '        For Each key As String In Me.DumpVarList
        '            If key.IndexOf("."c) > -1 Then
        '                Dim exp As New SimpleExpression(Of Object)(key)
        '                Dim objVar As Object = exp.Evaluate(context, context)
        '                If objVar IsNot Nothing Then
        '                    toReturn(key) = objVar
        '                End If
        '            Else
        '                If context.Items.TryGetValue(key, dumpVar) Then
        '                    toReturn(key) = dumpVar
        '                End If
        '            End If
        '        Next
        '    End If

        '    Return toReturn
        'End Function


#End Region


    End Class
End Namespace
