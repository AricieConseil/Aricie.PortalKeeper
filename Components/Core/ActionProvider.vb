
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Threading
Imports Aricie.DNN.Security.Trial
Imports Aricie.Services
Imports Aricie.DNN.Services.Workers

Namespace Aricie.DNN.Modules.PortalKeeper



    <Serializable()> _
     <DisplayName("Empty Action Provider")> _
        <Description("This provider performs no particular action but can be used for adding a sleep time for instance")> _
    Public Class ActionProvider(Of TEngineEvents As IConvertible)
        Inherits ActionProviderSettings(Of TEngineEvents)
        Implements IActionProvider(Of TEngineEvents)

        Private _Condition As New KeeperCondition(Of TEngineEvents)
        Private _AlternateAction As New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("TechnicalSettings")> _
     <SortOrder(950)> _
        Public Property AddSleepTime() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <ConditionalVisible("AddSleepTime", False, True)> _
        <SortOrder(950)> _
        Public Property SleepTime() As New STimeSpan()

        <ExtendedCategory("TechnicalSettings")> _
         <SortOrder(950)> _
        Public Property UseSemaphore As Boolean

        <SortOrder(950)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property SemaphoreName As String = "Aricie-ActionSemaphore"

        <SortOrder(950)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property NbConcurrentThreads As Integer = 1

        <SortOrder(950)> _
        <ConditionalVisible("UseSemaphore", False, True)> _
       <ExtendedCategory("TechnicalSettings")> _
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

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("CaptureRunDuration", False, True)> _
        <SortOrder(1000)> _
        Public Property RunDurationVarName() As String = String.Empty

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property DisablePerformanceLogger() As Boolean






        Public Function RunAndSleep(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean Implements IActionProvider(Of TEngineEvents).Run
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
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
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
            If Me.DisablePerformanceLogger Then
                PerformanceLogger.Instance.DisableLog(actionContext.FlowId)
            End If
            Dim toreturn As Boolean
            Dim runStart As DateTime
            If Me._CaptureRunDuration Then
                runStart = PerformanceLogger.Instance.Now
            End If
            If (Not Me._ConditionalAction) OrElse Me._Condition.Match(actionContext) Then
                toreturn = Me.Run(actionContext)
            Else
                toreturn = Me._AlternateAction.Run(actionContext)
            End If
            If Me._AddSleepTime AndAlso Me._SleepTime <> TimeSpan.Zero Then
                Thread.Sleep(Me._SleepTime.Value)
            End If
            If Me._CaptureRunDuration Then
                Dim duration As TimeSpan = PerformanceLogger.Instance.Now.Subtract(runStart)
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



    End Class
End Namespace