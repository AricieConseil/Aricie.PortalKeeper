
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports System.Xml.Serialization
Imports Aricie.DNN.Security.Trial

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum RuleEngineMode
        Actions
        Rules
    End Enum

    Public Class SimpleRuleEngine
        Inherits RuleEngineSettings(Of Boolean)

        <XmlIgnore()> _
      <Browsable(False)> _
        Public Overrides Property Mode As RuleEngineMode
            Get
                Return RuleEngineMode.Actions
            End Get
            Set(value As RuleEngineMode)
                'do nothing
            End Set
        End Property

    End Class

    <Serializable()> _
    Public Class RuleEngineSettings(Of TEngineEvents As IConvertible)
        Inherits NamedConfig

        Public Overridable Property Mode As RuleEngineMode

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ExtendedCategory("Variables")> _
            <SortOrder(2)> _
        Public Property Variables() As Variables = New Variables



        <ConditionalVisible("Mode", False, True, RuleEngineMode.Actions)> _
        <ExtendedCategory("Actions")> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(1)> _
        Public Property Actions As New KeeperAction(Of TEngineEvents)

        '<Editor(GetType(ListEditControl), GetType(EditControl))> _
        '<InnerEditor(GetType(PropertyEditorEditControl)), LabelMode(LabelMode.Top)> _
        '<CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
        <ConditionalVisible("Mode", False, True, RuleEngineMode.Rules)> _
        <ExtendedCategory("Rules")> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(1)> _
        Public Property Rules() As New List(Of KeeperRule(Of TEngineEvents))

        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property EnableSimpleLogs() As Boolean


        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
       <ConditionalVisible("EnableSimpleLogs", False, True)> _
        Public Property EnableStopWatch() As Boolean


        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("EnableSimpleLogs", False, True)> _
        Public Property LogDump As Boolean

        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("LogDump", False, True)> _
        Public Property DumpAllVars() As Boolean

        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
         <ConditionalVisible("DumpAllVars", True, True)> _
          <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(4), Width(500)> _
        Public Property DumpVariables() As String = ""


        Private _dumpVarLock As New Object
        Private _DumpVarList As List(Of String)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property DumpVarList() As List(Of String)
            Get
                If _DumpVarList Is Nothing Then
                    SyncLock _dumpVarLock
                        If _DumpVarList Is Nothing Then
                            '_DumpVarList = New List(Of String)
                            'Dim strVars As String() = Me._DumpVariables.Split(","c)
                            'For Each strVar As String In strVars
                            '    If strVar.Trim <> "" Then
                            '        _DumpVarList.Add(strVar.Trim())
                            '    End If
                            'Next
                            _DumpVarList = ParseStringList(Me._DumpVariables)
                        End If
                    End SyncLock
                End If
                Return _DumpVarList
            End Get
        End Property



        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property ExceptionDumpAllVars() As Boolean

        <ConditionalVisible("ExceptionDumpAllVars", True, True)> _
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
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        Public Property ConditionProviders() As ProviderList(Of ConditionProviderConfig(Of TEngineEvents)) = New ProviderList(Of ConditionProviderConfig(Of TEngineEvents))

        <ExtendedCategory("Providers")> _
                       <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
            <LabelMode(LabelMode.Top)> _
            <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(900)> _
        Public Property ActionProviders() As ProviderList(Of ActionProviderConfig(Of TEngineEvents)) = New ProviderList(Of ActionProviderConfig(Of TEngineEvents))


        Public Overridable Sub ProcessRules(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal objEvent As TEngineEvents, ByVal endSequence As Boolean)
            context.ProcessRules(objEvent, Me, endSequence)
        End Sub

        Public Function BatchRun(events As IEnumerable(Of TEngineEvents), userParams As IDictionary(Of String, Object)) As PortalKeeperContext(Of TEngineEvents)
            Dim objEnableStopWatch As Boolean = Me.EnableStopWatch OrElse Me.EnableSimpleLogs

            Dim existingContext As New PortalKeeperContext(Of TEngineEvents)
            If objEnableStopWatch Then
                'Dim objStep As New StepInfo(Debug.RequestTiming, "Start " & configRules.Name & " " & objEvent.ToString(CultureInfo.InvariantCulture), WorkingPhase.InProgress, False, False, -1, Me.FlowId)
                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} Init", Me.Name), _
                                            WorkingPhase.InProgress, False, False, -1, existingContext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            existingContext.Init(Me, userParams)
            If objEnableStopWatch Then
                'Dim objStep As New StepInfo(Debug.RequestTiming, "Start " & configRules.Name & " " & objEvent.ToString(CultureInfo.InvariantCulture), WorkingPhase.InProgress, False, False, -1, Me.FlowId)
                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("End {0} Init", Me.Name), _
                                            WorkingPhase.InProgress, False, False, -1, existingContext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
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
