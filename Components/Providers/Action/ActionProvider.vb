
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Threading
Imports Aricie.DNN.Security.Trial

Namespace Aricie.DNN.Modules.PortalKeeper



    <Serializable()> _
     <DisplayName("Empty Action Provider")> _
        <Description("This provider performs no particular action but can be used for adding a sleep time for instance")> _
    Public Class ActionProvider(Of TEngineEvents As IConvertible)
        Inherits ActionProviderSettings(Of TEngineEvents)
        Implements IActionProvider(Of TEngineEvents)


        <ExtendedCategory("TechnicalSettings")> _
     <SortOrder(1000)> _
        Public Property AddSleepTime() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <ConditionalVisible("AddSleepTime", False, True)> _
        <SortOrder(1000)> _
        Public Property SleepTime() As New STimeSpan()


        <ExtendedCategory("ConditonalSettings")> _
        <SortOrder(500)> _
        Public Property ConditionalAction() As Boolean

        <ExtendedCategory("ConditonalSettings")> _
        <ConditionalVisible("ConditionalAction", False, True, True)> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(500)> _
        Public Property Condition() As New KeeperCondition(Of TEngineEvents)

        <ExtendedCategory("ConditonalSettings")> _
        <ConditionalVisible("ConditionalAction", False, True, True)> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        <SortOrder(500)> _
        Public Property AlternateAction() As New KeeperAction(Of TEngineEvents)




        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property CaptureRunDuration() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("CaptureRunDuration", False, True)> _
        <SortOrder(1000)> _
        Public Property RunDurationVarName() As String = String.Empty


        Public Function RunAndSleep(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean Implements IActionProvider(Of TEngineEvents).Run
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
            Return toreturn
        End Function


        Public Overridable Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return False
        End Function



    End Class
End Namespace