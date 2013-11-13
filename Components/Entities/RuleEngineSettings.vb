Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports System.Globalization
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class RuleEngineSettings(Of TEngineEvents As IConvertible)
        Inherits NamedConfig

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ExtendedCategory("Variables")> _
            <SortOrder(2)> _
        Public Property Variables() As Variables = New Variables

        <ExtendedCategory("Rules")> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
        <InnerEditor(GetType(PropertyEditorEditControl)), LabelMode(LabelMode.Top)> _
        <CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
        <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        <SortOrder(1)> _
        Public Property Rules() As List(Of KeeperRule(Of TEngineEvents)) = New List(Of KeeperRule(Of TEngineEvents))

        <ExtendedCategory("TechnicalSettings")> _
       <ConditionalVisible("ShowTechnicalSettings", False, True, True)> _
       <SortOrder(1000)> _
        Public Property EnableStopWatch() As Boolean


        <ExtendedCategory("TechnicalSettings")> _
         <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
           LineCount(4), Width(500)> _
           <SortOrder(1000)> _
        Public Property ExceptionDumpVars() As String = String.Empty

        <ExtendedCategory("Providers")> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
        <CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
        <LabelMode(LabelMode.Top)> _
        <SortOrder(900)> _
        <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        Public Property ConditionProviders() As ProviderList(Of ConditionProviderConfig(Of TEngineEvents)) = New ProviderList(Of ConditionProviderConfig(Of TEngineEvents))

        <ExtendedCategory("Providers")> _
                       <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
            <LabelMode(LabelMode.Top)> _
            <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        <SortOrder(900)> _
        Public Property ActionProviders() As ProviderList(Of ActionProviderConfig(Of TEngineEvents)) = New ProviderList(Of ActionProviderConfig(Of TEngineEvents))


        Public Overridable Sub ProcessRules(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal objEvent As TEngineEvents, ByVal endSequence As Boolean)
            context.ProcessRules(objEvent, Me, endSequence)
        End Sub

        Public Function BatchRun(events As IEnumerable(Of TEngineEvents), userParams As IDictionary(Of String, Object)) As PortalKeeperContext(Of TEngineEvents)
            Dim existingContext As New PortalKeeperContext(Of TEngineEvents)
            existingContext.Init(Me, userParams)
            Me.BatchRun(events, existingContext)
            Return existingContext
        End Function

        Public Sub BatchRun(events As IEnumerable(Of TEngineEvents), ByRef existingContext As PortalKeeperContext(Of TEngineEvents))
            For Each eventStep As TEngineEvents In events
                Me.ProcessRules(existingContext, eventStep, False)
            Next
        End Sub


    End Class
End Namespace
