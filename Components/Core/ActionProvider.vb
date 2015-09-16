
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Threading
Imports Aricie.DNN.Security.Trial
Imports Aricie.Services
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper



    <Serializable()> _
     <DisplayName("Empty Action Provider")> _
        <Description("This provider performs no particular action but can be used for adding a sleep time for instance")> _
    Public Class ActionProvider(Of TEngineEvents As IConvertible)
        Inherits ActionProviderSettings(Of TEngineEvents)
        Implements IActionProvider(Of TEngineEvents)
        Implements IExpressionVarsProvider


        Private _Condition As New KeeperCondition(Of TEngineEvents)
        Private _AlternateAction As New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(950)> _
        Public Property AddSleepTime() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("AddSleepTime", False, True)> _
        <SortOrder(950)> _
        Public Property SleepTime() As New STimeSpan()

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("AddSleepTime", False, True)> _
        <SortOrder(950)> _
        Public Property RandomizeSleepTime As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("AddSleepTime", False, True)> _
        <ConditionalVisible("RandomizeSleepTime", False, True)> _
        <SortOrder(950)> _
        Public Property RandomizeSleepTimeOverOnly As Boolean


        <SortOrder(951)> _
        <ExtendedCategory("TechnicalSettings", "Synchronization")> _
        Public Property WaitSynchronisationHandle As New EnabledFeature(Of SimpleOrExpression(Of String))(New SimpleOrExpression(Of String)("Synchro"))


        <ExtendedCategory("TechnicalSettings", "Synchronization")> _
        <SortOrder(952)> _
        Public Property UseSemaphore As Boolean

        <Required(True)> _
        <SortOrder(952)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
        <ExtendedCategory("TechnicalSettings", "Synchronization")> _
        Public Property SemaphoreName As String = "Aricie-ActionSemaphore"

        <SortOrder(952)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
        <ExtendedCategory("TechnicalSettings", "Synchronization")> _
        Public Property NbConcurrentThreads As Integer = 1

        <SortOrder(952)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
        <ExtendedCategory("TechnicalSettings", "Synchronization")> _
        Public Property SynchronisationTimeout() As New STimeSpan(TimeSpan.Zero)


        <AutoPostBack()> _
        <ExtendedCategory("ConditonalSettings")> _
        <SortOrder(500)> _
        Public Property ConditionalAction() As Boolean

        <ExtendedCategory("ConditonalSettings")> _
        <ConditionalVisible("ConditionalAction", False, True, True)> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(500)>
        Public Property Condition() As KeeperCondition(Of TEngineEvents)
            Get
                If Me.ConditionalAction Then
                    Return _Condition
                End If
                Return Nothing
            End Get
            Set(value As KeeperCondition(Of TEngineEvents))
                _Condition = value
            End Set
        End Property

        <ExtendedCategory("ConditonalSettings")> _
        <ConditionalVisible("ConditionalAction", False, True, True)> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <SortOrder(500)>
        Public Property AlternateAction() As KeeperAction(Of TEngineEvents)
            Get
                If Me.ConditionalAction Then
                    Return _AlternateAction
                End If
                Return Nothing
            End Get
            Set(value As KeeperAction(Of TEngineEvents))
                _AlternateAction = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property CaptureRunDuration() As Boolean

        <Required(True)> _
        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("CaptureRunDuration", False, True)> _
        <SortOrder(1000)> _
        Public Property RunDurationVarName() As String = String.Empty

        <ExtendedCategory("TechnicalSettings", "Debug")> _
        <SortOrder(2000)> _
        Public Property DisablePerformanceLogger() As Boolean



        <ExtendedCategory("TechnicalSettings", "Debug")> _
        <SortOrder(2000)> _
        Public Property DebuggerBreak As Boolean

        <ExtendedCategory("TechnicalSettings", "Debug")> _
        <ConditionalVisible("DebuggerBreak", False, True)> _
        <SortOrder(2000)> _
        Public Property DebuggerBreakEarly As Boolean





        Public Overridable Function RunAndSleep(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean Implements IActionProvider(Of TEngineEvents).Run
            If Me.WaitSynchronisationHandle.Enabled Then
                Dim key As String = Me.WaitSynchronisationHandle.Entity.GetValue(actionContext, actionContext)
                Dim counter As Object = Nothing
                If actionContext.Items.TryGetValue(key, counter) Then
                    DirectCast(counter, Countdown).Wait()
                Else
                    Throw New ApplicationException("No Synchronisation handle was found in the context variables with name " & key)
                End If
            End If
            If Me.UseSemaphore Then
                Dim owned As Boolean
                'Dim semaphoreId As String = "AsyncBot" & botContext.AsyncLockId.ToString(CultureInfo.InvariantCulture)
                'todo: check if a global semaphore is necessary (see the code below for security access)
                'semaphoreId = String.Format("Global\{0}", semaphoreId)
                'Using objSemaphore As New Semaphore(0, Me.NbConcurrentThreads, Me.SemaphoreName)
                Using objSemaphore As New SafeSemaphore(Me.NbConcurrentThreads, Me.SemaphoreName)
                    Try
                        'Dim allowEveryoneRule As New MutexAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), MutexRights.FullControl, AccessControlType.Allow)
                        'Dim securitySettings As New MutexSecurity()
                        'securitySettings.AddAccessRule(allowEveryoneRule)
                        'objMutex.SetAccessControl(securitySettings)
                        If (Me.SynchronisationTimeout.Value <> TimeSpan.Zero AndAlso objSemaphore.Wait(Me.SynchronisationTimeout.Value)) OrElse (Me.SynchronisationTimeout.Value = TimeSpan.Zero AndAlso objSemaphore.Wait()) Then
                            owned = True
                            Return RunAndSleepUnlocked(actionContext)
                        Else
                            Return False
                        End If
                    Catch ex As AbandonedMutexException
                        ExceptionHelper.LogException(ex)
                        owned = True
                    Finally
                        If owned Then
                            objSemaphore.Release()
                        End If
                    End Try
                End Using
            Else
                Return RunAndSleepUnlocked(actionContext)
            End If



        End Function


        Public Function RunAndSleepUnlocked(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.DebuggerBreak AndAlso Me.DebuggerBreakEarly Then
                CallDebuggerBreak()
            End If
            If Me.DisablePerformanceLogger Then
                PerformanceLogger.Instance.DisableLog(actionContext.FlowId)
            End If
            Dim toreturn As Boolean
            Dim runStart As DateTime
            If Me._CaptureRunDuration Then
                runStart = PerformanceLogger.Now
            End If
            If (Not Me._ConditionalAction) OrElse Me._Condition.Match(actionContext) Then
                toreturn = Me.Run(actionContext)
            Else
                Me._AlternateAction.Run(actionContext)
                toreturn = False
            End If
            If Me._AddSleepTime AndAlso Me._SleepTime <> TimeSpan.Zero Then

                Dim sleepDuration As TimeSpan = Me._SleepTime.Value
                If Me.RandomizeSleepTime Then
                    Dim objTicks As Long = sleepDuration.Ticks * 2
                    Dim randomByte As Byte() = Aricie.Security.Cryptography.CryptoHelper.GetNewSalt(8)
                    Dim objRandomLong As Long = BitConverter.ToInt64(randomByte, 0)
                    If objRandomLong = Long.MinValue Then
                        objRandomLong = Long.MaxValue
                    Else
                        objRandomLong = Math.Abs(objRandomLong)
                    End If
                    Dim objRandomTicks As Long
                    If RandomizeSleepTimeOverOnly Then
                        objRandomTicks = objRandomLong Mod (2 * objTicks)
                    Else
                        objRandomTicks = objTicks + (objRandomLong Mod objTicks)
                    End If
                    sleepDuration = TimeSpan.FromTicks(objRandomTicks)
                End If
                If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "Action Sleep Start", _
                                                WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                Thread.Sleep(sleepDuration)
                If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "End Action Sleep", _
                                                WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If

            End If
            If Me._CaptureRunDuration Then
                Dim duration As TimeSpan = PerformanceLogger.Now.Subtract(runStart)
                actionContext.SetVar(Me._RunDurationVarName, duration)
            End If
            If Me.DisablePerformanceLogger Then
                PerformanceLogger.Instance.EnableLog(actionContext.FlowId)
            End If
            Return toreturn
        End Function




        Public Overridable Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return False
        End Function



        Public Overridable Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            If Me.CaptureRunDuration Then
                existingVars(Me.RunDurationVarName) = GetType(TimeSpan)
            End If
            If Me.ConditionalAction Then
                Me.Condition.AddVariables(currentProvider, existingVars)
                Me.AlternateAction.AddVariables(currentProvider, existingVars)
            End If
        End Sub


        Protected Sub Sleep(actioncontext As PortalKeeperContext(Of TEngineEvents), sleepTime As TimeSpan)

            If actioncontext.LoggingLevel = LoggingLevel.Detailed Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, "Sleep Spreadsheet Command Start", _
                                            WorkingPhase.EndOverhead, False, False, -1, actioncontext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            Thread.Sleep(sleepTime)
            If actioncontext.LoggingLevel = LoggingLevel.Detailed Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, "End Sleep Spreadsheet Command", _
                                            WorkingPhase.InProgress, False, False, -1, actioncontext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If

        End Sub

    End Class
End Namespace