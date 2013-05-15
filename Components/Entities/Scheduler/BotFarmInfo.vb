Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services
Imports System.Xml.Serialization
Imports System.Threading
Imports Aricie.DNN.Diagnostics
Imports System.Globalization
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.Skins.Controls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum SynchronizationMode
        Monitor
        Mutex
    End Enum

    <XmlInclude(GetType(UserVariableInfo))> _
    <Serializable()> _
    Public Class BotFarmInfo(Of TEngineEvent As IConvertible)
        Inherits NamedConfig


#Region "Private members"



        Private _Bots As New SimpleList(Of BotInfo(Of TEngineEvent))

        Private _EnableUserBots As Boolean

        Private _InitVector As String = String.Empty

        Private _EncryptionKey As String = String.Empty

        Private _DnnDecryptionKey As String = String.Empty

        Private _UserBots As New SimpleList(Of UserBotSettings(Of TEngineEvent))

        Private _Schedule As New STimeSpan(TimeSpan.FromSeconds(15))

        Private _EnableLogs As Boolean

        Private _Synchronization As SynchronizationMode = PortalKeeper.SynchronizationMode.Monitor

        Private _MutexName As String = "Aricie.PKP.FarmMutex"

        Private _SynchronisationTimeout As New STimeSpan(TimeSpan.FromSeconds(1))

#End Region

#Region "Public properties"












        <MainCategory()> _
        <ExtendedCategory("Bots")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        Public Property Bots() As SimpleList(Of BotInfo(Of TEngineEvent))
            Get
                Return _Bots
            End Get
            Set(ByVal value As SimpleList(Of BotInfo(Of TEngineEvent)))
                _Bots = value
            End Set
        End Property


        <ExtendedCategory("UserBots")> _
        Public Property EnableUserBots() As Boolean
            Get
                Return _EnableUserBots
            End Get
            Set(ByVal value As Boolean)
                _EnableUserBots = value
            End Set
        End Property


        <ExtendedCategory("UserBots")> _
        <ConditionalVisible("EnableUserBots", False, True)> _
       <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
           <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        Public Property UserBots() As SimpleList(Of UserBotSettings(Of TEngineEvent))
            Get
                Return _UserBots
            End Get
            Set(ByVal value As SimpleList(Of UserBotSettings(Of TEngineEvent)))
                _UserBots = value
            End Set
        End Property


        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property AvailableUserBots() As IDictionary(Of String, UserBotSettings(Of TEngineEvent))
            Get
                Return ProviderList(Of UserBotSettings(Of TEngineEvent)).GetAvailable(Me._UserBots.Instances)
            End Get
        End Property


        <IsReadOnly(True)> _
        <ExtendedCategory("UserBots")> _
        <ConditionalVisible("EnableUserBots", False, True)> _
        Public Property InitVector() As String
            Get
                If String.IsNullOrEmpty(_InitVector) Then
                    _InitVector = DotNetNuke.Entities.Users.UserController.GeneratePassword(16)
                End If
                Return _InitVector
            End Get
            Set(ByVal value As String)
                _InitVector = value
            End Set
        End Property


        <Browsable(False)> _
        Public Property EncryptedKey() As String
            Get
                Try
                    If String.IsNullOrEmpty(_EncryptionKey) Then
                        Me._EncryptionKey = DotNetNuke.Entities.Users.UserController.GeneratePassword(64)
                    End If
                    Return Aricie.Common.Encrypt(_EncryptionKey, Me.DnnDecyptionKey, Me._InitVector)
                Catch ex As Exception
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Try
                        _EncryptionKey = Aricie.Common.Decrypt(value, Me.DnnDecyptionKey, Me._InitVector)
                    Catch ex As Exception
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
                    End Try
                End If
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("UserBots")> _
        <ConditionalVisible("EnableUserBots", False, True)> _
          <Width(450)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property EncryptionKey() As String
            Get
                Return _EncryptionKey
            End Get
            Set(ByVal value As String)
                _EncryptionKey = value
            End Set
        End Property

        Private ReadOnly Property DnnDecyptionKey() As String
            Get
                If String.IsNullOrEmpty(_DnnDecryptionKey) Then
                    _DnnDecryptionKey = NukeHelper.WebConfigDocument.SelectSingleNode("configuration/system.web/machineKey").Attributes("decryptionKey").Value
                End If
                Return _DnnDecryptionKey
            End Get
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        Public Property Schedule() As STimeSpan
            Get
                Return _Schedule
            End Get
            Set(ByVal value As STimeSpan)
                _Schedule = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        Public Property EnableLogs() As Boolean
            Get
                Return _EnableLogs
            End Get
            Set(ByVal value As Boolean)
                _EnableLogs = value
            End Set
        End Property




        <ExtendedCategory("TechnicalSettings")> _
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
      <ExtendedCategory("TechnicalSettings")> _
        Public Property MutexName() As String
            Get
                Return _MutexName
            End Get
            Set(ByVal value As String)
                _MutexName = value
            End Set
        End Property


        <ExtendedCategory("TechnicalSettings")> _
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
        <ActionButton("~/images/fwd.gif")> _
        Public Sub RunForcedBots(pmb As AriciePortalModuleBase)
            Dim flowid As String = Guid.NewGuid.ToString
            If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

                Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run Start", _
                                            WorkingPhase.InProgress, False, False, -1, flowid)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Dim nbRuns As Integer = Me.RunBots(CType(PortalKeeperSchedule.ScheduleEventList, Global.System.Collections.Generic.IList(Of TEngineEvent)), True, Guid.NewGuid.ToString)

            If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

                Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run End", _
                                            WorkingPhase.InProgress, True, False, -1, flowid)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If

            Skin.AddModuleMessage(pmb, String.Format(Localization.GetString("ManualRun.Message", pmb.LocalResourceFile), nbRuns), ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub


        Public Function RunBots(ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean, flowId As String) As Integer
            'Dim minNextRun As DateTime = lastRun.Add(Me._Schedule)
            'If minNextRun < Now Then
            Dim toreturn As Integer
            Select Case Me._Synchronization
                Case SynchronizationMode.Monitor
                    If System.Threading.Monitor.TryEnter(botlock, Me._SynchronisationTimeout.Value) Then
                        Try
                            toreturn = Me.RunBotsUnlocked(events, forceRun, flowId)
                        Catch ex As Exception
                            Aricie.DNN.Diagnostics.AsyncLogger.Instance.AddException(ex)
                            'DotNetNuke.Services.Exceptions.LogException(ex)
                        Finally
                            System.Threading.Monitor.Exit(botlock)
                        End Try
                        'lastRun = Now
                    End If
                Case SynchronizationMode.Mutex
                    Using objMutex As New Mutex(False, Me._MutexName)
                        If objMutex.WaitOne(Me._SynchronisationTimeout.Value) Then
                            Try
                                toreturn = Me.RunBotsUnlocked(events, forceRun, flowId)
                            Catch ex As Exception
                                Aricie.DNN.Diagnostics.AsyncLogger.Instance.AddException(ex)
                                'DotNetNuke.Services.Exceptions.LogException(ex)
                            Finally
                                objMutex.ReleaseMutex()
                            End Try
                        End If
                    End Using
            End Select
            'Else
            'Thread.Sleep(GetHalfSchedule)
            'End If
            Return toreturn
        End Function

        'Private Function GetHalfSchedule() As TimeSpan
        '    Return TimeSpan.FromTicks(Me._Schedule.Ticks \ 2)
        'End Function


        Private Function RunBotsUnlocked(ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean, flowId As String) As Integer
            Dim toreturn As Integer
            'Dim flowid As String = ""
            If Me._EnableLogs Then
                'If flowId = "" Then
                '    flowId = Guid.NewGuid.ToString
                'End If
                'flowId = Guid.NewGuid.ToString
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Run Start", _
                                            WorkingPhase.InProgress, False, False, -1, flowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            For Each webBot As BotInfo(Of TEngineEvent) In Me._Bots.Instances
                'Dim unused As Boolean = True
                'For Each objUserBotSettings As UserBotSettings(Of TEngineEvent) In Me.AvailableUserBots.Values
                '    If objUserBotSettings.BotName = webBot.Name AndAlso objUserBotSettings.DisableTemplateBot Then
                '        unused = False
                '    End If
                'Next
                If Not webBot.MasterBotDisabled AndAlso Not webBot.AsyncLockBot.ContainsKey(-1) Then
                    'Dim botHistory = Aricie.DNN.Settings.SettingsController.LoadFileSettings(Of WebBotHistory)(GetLogMapPath(), True)
                    Dim runContext As New BotRunContext(Of TEngineEvent)
                    runContext.Events = events
                    runContext.History = webBot.BotHistory
                    runContext.RunEndDelegate = AddressOf webBot.SaveHistory

                    If webBot.RunBot(runContext, forceRun) Then
                        toreturn += 1
                    End If
                    'Aricie.DNN.Settings.SettingsController.SaveFileSettings(Of WebBotHistory)(GetLogMapPath(), webBot.BotHistory)
                End If
            Next
            If Me._EnableUserBots Then
                If Me._EnableLogs Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "User Bots Start", _
                                                WorkingPhase.InProgress, False, False, -1, flowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                For Each userSettings As UserBotSettings(Of TEngineEvent) In Me.AvailableUserBots.Values

                    toreturn += userSettings.RunUserBots(Me._EncryptionKey, Me._InitVector, events, forceRun)
                Next
            End If
            If Me._EnableLogs Then
                Dim nbRuns As New KeyValuePair(Of String, String)("Nb of Bots run", toreturn.ToString(CultureInfo.InvariantCulture))
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Run End", _
                                            WorkingPhase.InProgress, False, False, -1, flowId, nbRuns)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Return toreturn
        End Function


#End Region

    End Class
End Namespace