Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports System.Threading
Imports Aricie.DNN.Diagnostics
Imports System.Globalization
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.Skins.Controls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Security.Trial
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum SynchronizationMode
        Monitor
        Mutex
    End Enum



    <ActionButton(IconName.Calendar, IconOptions.Normal)> _
    <XmlInclude(GetType(UserVariableInfo))> _
    <Serializable()> _
    Public Class BotFarmInfo(Of TEngineEvent As IConvertible)
        Implements IEnabled





#Region "Private members"



        Private _Schedule As New STimeSpan(TimeSpan.FromSeconds(15))

        Private _EnableLogs As Boolean

        Private _Synchronization As SynchronizationMode = SynchronizationMode.Monitor

        Private _MutexName As String = "Aricie.PKP.FarmMutex"

        Private _SynchronisationTimeout As New STimeSpan(TimeSpan.FromSeconds(1))

#End Region

#Region "Public properties"

        Public Property Enabled() As Boolean Implements IEnabled.Enabled

        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <ExtendedCategory("Bots")>
        Public Property Bots As New SimpleList(Of BotInfo(Of TEngineEvent))


        <ExtendedCategory("UserBots")>
        Public Property EnableUserBots As Boolean


        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <ConditionalVisible("EnableUserBots", False, True)> _
        <ExtendedCategory("UserBots")>
        Public Property UserBots As New SimpleList(Of UserBotSettings(Of TEngineEvent))


        <XmlIgnore()> _
        <Browsable(False)>
        Public ReadOnly Property AvailableUserBots() As IDictionary(Of String, UserBotSettings(Of TEngineEvent))
            Get
                Return ProviderList(Of UserBotSettings(Of TEngineEvent)).GetAvailable(Me.UserBots.Instances)
            End Get
        End Property

        <ExtendedCategory("TechnicalSettings")> _
         <Width(300)> _
            <LineCount(2)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RunningServers() As String = ""

        <ExtendedCategory("TechnicalSettings")>
        Public Property Schedule() As STimeSpan
            Get
                Return _Schedule
            End Get
            Set(ByVal value As STimeSpan)
                _Schedule = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")>
        Public Property EnableLogs() As Boolean
            Get
                Return _EnableLogs
            End Get
            Set(ByVal value As Boolean)
                _EnableLogs = value
            End Set
        End Property


        <ExtendedCategory("TechnicalSettings")>
        Public Property Synchronization() As SynchronizationMode
            Get
                Return _Synchronization
            End Get
            Set(ByVal value As SynchronizationMode)
                _Synchronization = value
            End Set
        End Property

        <Required(True)> _
        <ConditionalVisible("Synchronization", False, True, SynchronizationMode.Mutex)> _
        <ExtendedCategory("TechnicalSettings")>
        Public Property MutexName() As String
            Get
                Return _MutexName
            End Get
            Set(ByVal value As String)
                _MutexName = value
            End Set
        End Property


        <ExtendedCategory("TechnicalSettings")>
        Public Property SynchronisationTimeout() As STimeSpan
            Get
                Return _SynchronisationTimeout
            End Get
            Set(ByVal value As STimeSpan)
                _SynchronisationTimeout = value
            End Set
        End Property


#End Region


#Region "methods"

        '<NonSerialized()> _
        'Private lastRun As DateTime = DateTime.MinValue
        Private Shared botlock As New Object


        'Private Shared _FarmMutex As Mutex

        'Private ReadOnly Property FarmMutex As Mutex
        '    Get
        '        If _FarmMutex Is Nothing Then
        '            _FarmMutex = New Mutex(False, Me._MutexName)
        '        End If
        '        Return _FarmMutex
        '    End Get
        'End Property



        <ActionButton(IconName.Rocket, IconOptions.Normal)>
        Public Sub RunForcedBots(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            Dim flowid As String = Guid.NewGuid.ToString
            If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

                Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run Start", WorkingPhase.InProgress, False, False, -1, flowid)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Dim nbRuns As Integer = Me.RunBots(CType(PortalKeeperSchedule.ScheduleEventList, IList(Of TEngineEvent)), True, Guid.NewGuid.ToString)

            If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

                Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run End", WorkingPhase.InProgress, True, False, -1, flowid)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If

            ape.DisplayMessage(String.Format(Localization.GetString("ManualRun.Message", ape.LocalResourceFile), nbRuns), ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub


        Public Function RunBots(ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean, flowId As String) As Integer
            Dim toreturn As Integer
            If Me.MatchServer() Then

                Select Case Me._Synchronization
                    Case SynchronizationMode.Monitor
                        If Monitor.TryEnter(botlock, Me._SynchronisationTimeout.Value) Then
                            Try
                                toreturn = Me.RunBotsUnlocked(events, forceRun, flowId)
                            Catch ex As Exception
                                AsyncLogger.Instance.AddException(ex)
                                'DotNetNuke.Services.Exceptions.LogException(ex)
                            Finally
                                Monitor.Exit(botlock)
                            End Try
                            'lastRun = Now
                        End If
                    Case SynchronizationMode.Mutex
                        Dim owned As Boolean
                        Dim mutexId As String = _MutexName
                        'todo: check if a global mutex is necessary (see the code below for security access)
                        'mutexId = String.Format("Global\{0}", mutexId)
                        Using objMutex As New Mutex(False, mutexId)
                            Try

                                If objMutex.WaitOne(Me._SynchronisationTimeout.Value) Then
                                    owned = True
                                    toreturn = Me.RunBotsUnlocked(events, forceRun, flowId)
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
                End Select
            End If
           
            'Else
            'Thread.Sleep(GetHalfSchedule)
            'End If
            Return toreturn
        End Function

        ''<ConditionalVisible("Storage", False, True, UserBotStorage.Personalisation)> _
        '<ActionButton(IconName.Unlock, IconOptions.Normal)> _
        'Public Sub UnlockUserBotManager()
        '    For Each objUserBot As UserBotSettings(Of TEngineEvent) In Me.UserBots.Instances
        '        objUserBot.SetEncrypter(Me)
        '    Next
        'End Sub


        'Private Function GetHalfSchedule() As TimeSpan
        '    Return TimeSpan.FromTicks(Me._Schedule.Ticks \ 2)
        'End Function


        Private Function RunBotsUnlocked(ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean, flowId As String) As Integer
            Dim toreturn As Integer
            If Me._EnableLogs Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Run Start", WorkingPhase.InProgress, False, False, -1, flowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            For Each webBot As BotInfo(Of TEngineEvent) In Me.Bots.Instances

                If webBot.Enabled AndAlso Not webBot.MasterBotDisabled Then 'AndAlso Not webBot.AsyncLockBot.ContainsKey(-1)
                    Dim history = webBot.BotHistory
                    'Dim nextSchedule As DateTime = history.LastRun.Add(webBot.Schedule.Value)
                    Dim nextSchedule As DateTime = history.GetNextSchedule(webBot.BotSchedule)
                    If nextSchedule <= Now OrElse forceRun AndAlso webBot.ForceRun Then
                        Dim runContext As New BotRunContext(Of TEngineEvent)(webBot, nextSchedule)
                        runContext.Enabled = True
                        runContext.Events = events
                        runContext.History = history
                        AddHandler runContext.Finish, AddressOf webBot.SaveHistory
                        If webBot.RunBot(runContext, forceRun) Then
                            toreturn += 1
                        End If
                    End If
                End If
            Next
            If Me.EnableUserBots Then
                If Me._EnableLogs Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "User Bots Start", WorkingPhase.InProgress, False, False, -1, flowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                For Each userSettings As UserBotSettings(Of TEngineEvent) In Me.AvailableUserBots.Values

                    toreturn += userSettings.RunUserBots(events, forceRun)
                Next
            End If
            If Me._EnableLogs Then
                Dim nbRuns As New KeyValuePair(Of String, String)("Nb of Bots run", toreturn.ToString(CultureInfo.InvariantCulture))
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Run End", WorkingPhase.InProgress, False, False, -1, flowId, nbRuns)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Return toreturn
        End Function

        'Public Sub Encrypt(Of T)(ByVal objSmartFile As SmartFile(Of T)) Implements IEncrypter.Encrypt
        '    If Not objSmartFile.Encrypted Then
        '        Dim salt As Byte() = Nothing
        '        Dim newPayLoad As String = Common.Encrypt(objSmartFile.PayLoad, Me._EncryptionKey, Me._InitVector, salt)
        '        objSmartFile.Encrypt(newPayLoad, salt)
        '    End If
        'End Sub

        'Public Sub Decrypt(Of T)(ByVal objSmartFile As SmartFile(Of T)) Implements IEncrypter.Decrypt
        '    If objSmartFile.Encrypted Then
        '        Dim newPayLoad As String = Common.Decrypt(objSmartFile.PayLoad, Me._EncryptionKey, "", objSmartFile.SaltBytes)
        '        objSmartFile.Decrypt(newPayLoad)
        '    End If
        'End Sub


        <NonSerialized()> _
        Private _ServerList As List(Of String)

        Private Function MatchServer() As Boolean
            If _ServerList Is Nothing Then
                _ServerList = New List(Of String)
                If Not RunningServers.IsNullOrEmpty() Then
                    For Each strServer As String In Me._RunningServers.Trim.Split(","c)
                        If strServer.Trim <> "" Then
                            _ServerList.Add(strServer.Trim.ToUpper)
                        End If
                    Next
                End If
            End If
            Return _ServerList.Count = 0 OrElse _ServerList.Contains(DotNetNuke.Common.Globals.ServerName.ToUpper)
        End Function


#End Region



    End Class
End Namespace