
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
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
Imports Aricie.DNN.Entities
Imports FileHelper = Aricie.DNN.Services.FileHelper
Imports Aricie.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper




    <ActionButton(IconName.Android, IconOptions.Normal)> _
    <DefaultProperty("Summary")> _
    Public Class BotInfo(Of TEngineEvent As IConvertible)
        Inherits RuleEngineSettings(Of TEngineEvent)

#Region "Private members"



        Private _Schedule As New STimeSpan(TimeSpan.FromMinutes(10))

        Private _ForceRun As Boolean



        Private _LogPath As String = "Aricie.PortalKeeper/"

        Private _RetainHistoryNb As Integer = 5

        Private _IncludeLastDump As Boolean

        Private _BotHistory As New WebBotHistory


        Private _RunningServers As String = ""


        Private _UseTaskQueue As Boolean
        Private _TaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(100))
        '<NonSerialized()> _
        Private WithEvents _AsynchronousRunTaskQueue As TaskQueue(Of BotRunContext(Of TEngineEvent))



#End Region


#Region "Public Properties"

        
        Public Overrides Property Mode As RuleEngineMode = RuleEngineMode.Rules

            

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property Summary As String
            Get
                Dim enableString As String = "Master " & IIf(Me.Enabled AndAlso Not Me.MasterBotDisabled, "On", "Off").ToString
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
                    userString &= IIf(Me.Enabled AndAlso PortalKeeperConfig.Instance.SchedulerFarm.EnableUserBots AndAlso targetUserBot.Enabled, " On", " Off").ToString
                End If
                Dim forcedString As String = IIf(Me._ForceRun, "Forced", "Unforced").ToString
                Return String.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}", Me.Name.PadRight(50), UIConstants.TITLE_SEPERATOR, enableString.PadRight(20), userString.PadRight(20), forcedString.PadRight(10), Me.BotSchedule.FormattedValueShort)
            End Get
        End Property


        Private _MasterBotDisabled As Nullable(Of Boolean)

        <ExtendedCategory("")> _
      <XmlIgnore()> _
        Public ReadOnly Property MasterBotDisabled() As Boolean
            Get
                If Not _MasterBotDisabled.HasValue Then
                    For Each objUserBotSettings As UserBotSettings(Of ScheduleEvent) In PortalKeeperConfig.Instance.SchedulerFarm.AvailableUserBots.Values
                        If objUserBotSettings.BotName = Me.Name AndAlso objUserBotSettings.DisableTemplateBot Then
                            _MasterBotDisabled = True
                        End If
                    Next
                    If Not _MasterBotDisabled.HasValue Then
                        _MasterBotDisabled = False
                    End If
                End If
                Return _MasterBotDisabled.Value
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

        <ExtendedCategory("Schedule")> _
        <SortOrder(2)> _
        Public Property BotSchedule As New ScheduleInfo()

        <ExtendedCategory("Schedule")> _
        Public Property AddCustomSchedule As Boolean

        <ConditionalVisible("AddCustomSchedule")> _
        <ExtendedCategory("Schedule")> _
        Public Property CustomSchedule As New FleeExpressionInfo(Of ScheduleInfo)


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
        Public Property HistoryDumpSettings As New DumpSettings



        'Public Property AddDumpToHistory() As Boolean

        '<ExtendedCategory("History")> _
        ' <ConditionalVisible("AddDumpToHistory", False, True)> _
        'Public Property HistoryDumpAllVars() As Boolean

        '<ExtendedCategory("History")> _
        ' <ConditionalVisible("AddDumpToHistory", False, True)> _
        '  <ConditionalVisible("HistoryDumpAllVars", True, True)> _
        '  <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
        '    LineCount(4), Width(500)> _
        'Public Property HistoryDumpVariables() As String = ""


        'Private _HistorydumpVarLock As New Object
        'Private _HistoryDumpVarList As List(Of String)

        '<XmlIgnore()> _
        '<Browsable(False)> _
        'Public ReadOnly Property HistoryDumpVariablesDumpVarList() As List(Of String)
        '    Get
        '        If _HistoryDumpVarList Is Nothing Then
        '            SyncLock _HistorydumpVarLock
        '                If _HistoryDumpVarList Is Nothing Then
        '                    _HistoryDumpVarList = ParseStringList(Me.HistoryDumpVariables)
        '                End If
        '            End SyncLock
        '        End If
        '        Return _HistoryDumpVarList
        '    End Get
        'End Property

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

        Public Sub SaveHistory(ByVal sender As Object, e As GenericEventArgs(Of BotRunContext(Of TEngineEvent)))
            SaveFileSettings(Of WebBotHistory)(GetLogMapPath(), e.Item.History)
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
            <ConditionalVisible("UseTaskQueue", False, True)> _
        Public Property TaskQueueInfo() As TaskQueueInfo
            Get
                Return _TaskQueueInfo
            End Get
            Set(ByVal value As TaskQueueInfo)
                _TaskQueueInfo = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        Public Property UseMutex() As Boolean

        <ConditionalVisible("UseMutex", False, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property SynchronisationTimeout() As New STimeSpan(TimeSpan.FromSeconds(2))

        Private ReadOnly Property AsynchronousRunTaskQueue() As TaskQueue(Of BotRunContext(Of TEngineEvent))
            Get
                If _AsynchronousRunTaskQueue Is Nothing Then

                    _AsynchronousRunTaskQueue = New TaskQueue(Of BotRunContext(Of TEngineEvent))(AddressOf InternalRun, Me._TaskQueueInfo)
                End If
                Return _AsynchronousRunTaskQueue
            End Get
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        Public Property RunSeveralInstances As Boolean

        <ExtendedCategory("TechnicalSettings")> _
         <ConditionalVisible("RunSeveralInstances", False, True)> _
        Public Property InstanceNumber As Integer = 1

        <ExtendedCategory("TechnicalSettings")> _
         <ConditionalVisible("RunSeveralInstances", False, True)> _
        Public Property SharedContext As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        Public Property NoSave As Boolean




#End Region


#Region "Public methods"

        Public Function RunBot(ByVal botContext As BotRunContext(Of TEngineEvent), ByVal isForcedRun As Boolean) As Boolean
            Dim toReturn As Boolean
            If Me.MatchServer Then
              
                Dim cloneContext As BotRunContext(Of TEngineEvent) = Nothing
                Dim instanceNb = 1
                If Me.RunSeveralInstances Then
                    instanceNb = Me.InstanceNumber
                End If
                For i As Integer = 0 To instanceNb - 1
                    If Me.RunSeveralInstances AndAlso Not Me.SharedContext Then
                        If i > 0 Then
                            botContext = cloneContext
                        End If
                        If i < instanceNb - 1 Then
                            cloneContext = ReflectionHelper.CloneObject(Of BotRunContext(Of TEngineEvent))(botContext)
                        End If
                    End If
                    If (isForcedRun AndAlso Me._ForceRun) _
                    OrElse (Not isForcedRun AndAlso Me.Enabled AndAlso botContext.NextSchedule <= Now) Then
                        If Not Me._UseTaskQueue Then
                            toReturn = Me.InternalRun(botContext)
                        Else
                            SyncLock (botContext)
                                Me.AsynchronousRunTaskQueue.EnqueueTask(botContext)
                            End SyncLock
                            toReturn = True
                        End If
                    End If
                Next
            End If
            Return toReturn
        End Function

#End Region



#Region "Private methods"
        '<NonSerialized()> _
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

        Public Shared Function GetMutexId(key As String) As String
            Return "AsyncBot" & key
        End Function

        Private Function InternalRun(ByVal botContext As BotRunContext(Of TEngineEvent)) As Boolean
            Dim toReturn As Boolean
            If Me.UseMutex Then
                Dim owned As Boolean
                Dim mutexId As String = GetMutexId(botContext.AsyncLockId.ToString(CultureInfo.InvariantCulture))
                'todo: check if a global mutex is necessary (see the code below for security access)
                'mutexId = String.Format("Global\{0}", mutexId)
                Using objMutex As New Mutex(False, mutexId)
                    Try
                        'Dim allowEveryoneRule As New MutexAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), MutexRights.FullControl, AccessControlType.Allow)
                        'Dim securitySettings As New MutexSecurity()
                        'securitySettings.AddAccessRule(allowEveryoneRule)
                        'objMutex.SetAccessControl(securitySettings)

                        If (Not Me.SynchronisationTimeout.Value = TimeSpan.Zero AndAlso objMutex.WaitOne(Me.SynchronisationTimeout.Value)) OrElse (Me.SynchronisationTimeout.Value = TimeSpan.Zero AndAlso objMutex.WaitOne()) Then
                            owned = True
                            InternalRunUnlocked(botContext)
                            toReturn = botContext.Enabled
                        End If
                    Catch ex As AbandonedMutexException
                        ExceptionHelper.LogException(ex)
                        owned = True
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    Finally
                        If owned Then
                            objMutex.ReleaseMutex()
                        End If
                    End Try
                End Using
            Else
                InternalRunUnlocked(botContext)
                toReturn = botContext.Enabled
            End If
            Return toReturn
        End Function

       
        Private Sub InternalRunUnlocked(ByVal botContext As BotRunContext(Of TEngineEvent))
            Dim currentWebBotEvent As New BotInfoEvent
            'Dim objEnableLogsOrStopWatch As Boolean = Me.EnableStopWatch OrElse Me.EnableSimpleLogs
            SyncLock botContext
                botContext.OnInit()
                If botContext.Enabled Then

                    If Me._IncludeLastDump Then
                        If botContext.History.BotCallHistory.Count > 0 Then
                            For Each objVarPair As KeyValuePair(Of String, Object) In botContext.History.BotCallHistory(0).VariablesDump
                                botContext.UserParams("Last" & objVarPair.Key) = objVarPair.Value
                            Next
                        End If
                    End If

                    If botContext.EngineContext Is Nothing Then
                        botContext.EngineContext = Me.InitContext(botContext.UserParams)
                        Dim lastrunDate As DateTime = DateTime.MinValue
                        If botContext.History IsNot Nothing Then
                            lastrunDate = botContext.History.LastRun
                        End If
                        botContext.EngineContext.SetVar("LastRun", lastrunDate)
                    End If

                End If

            End SyncLock

            If botContext.Enabled Then

                Dim objCustomSchedule As ScheduleInfo = Nothing
                Dim skip As Boolean


                If Me.AddCustomSchedule Then
                    objCustomSchedule = Me.CustomSchedule.Evaluate(botContext.EngineContext, botContext.EngineContext)
                    Dim customNext As DateTime = botContext.History.GetNextSchedule(objCustomSchedule)
                    If customNext > Now Then
                        skip = True
                        SyncLock botContext
                            botContext.History.NextSchedule = customNext
                        End SyncLock
                    End If
                End If

                If Not skip Then
                    Dim isSuccess As Boolean
                    Try
                        Me.BatchRun(botContext.Events, botContext.EngineContext)
                        isSuccess = True
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                        isSuccess = False
                    End Try

                    SyncLock botContext

                        If Me.HistoryDumpSettings.EnableDump Then
                            Dim dump As SerializableDictionary(Of String, Object) = botContext.EngineContext.GetDump(Me.HistoryDumpSettings)

                            currentWebBotEvent.VariablesDump = dump
                        End If

                        currentWebBotEvent.Duration = Now.Subtract(currentWebBotEvent.Time)
                        currentWebBotEvent.Success = isSuccess

                        Dim objNextSchedule = Me.BotSchedule.GetNextSchedule(currentWebBotEvent.Time)
                        If objCustomSchedule IsNot Nothing Then
                            Dim customNextSchedule As DateTime = objCustomSchedule.GetNextSchedule(currentWebBotEvent.Time)
                            If customNextSchedule > objNextSchedule Then
                                objNextSchedule = customNextSchedule
                            End If
                        End If
                        currentWebBotEvent.NextSchedule = objNextSchedule


                        botContext.History.Increment(currentWebBotEvent, Me.RetainHistoryNb)
                       
                    End SyncLock
                    'Else

                    '    SyncLock botContext
                    '        botContext.History.LastRun = Now
                    '    End SyncLock

                End If

                If Not Me.NoSave Then

                    'If objEnableLogsOrStopWatch Then
                    '    Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} Finalizing", Me.Name), _
                    '                                WorkingPhase.InProgress, False, False, -1, botContext.EngineContext.FlowId)
                    '    PerformanceLogger.Instance.AddDebugInfo(objStep)
                    'End If
                    botContext.EngineContext.LogStart("Finalizing", LoggingLevel.Steps, False)

                    botContext.OnFinalize()

                    botContext.EngineContext.LogEnd("Finalizing", False, False, LoggingLevel.Steps, Nothing)

                End If


                botContext.EngineContext.LogEndEngine()

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
