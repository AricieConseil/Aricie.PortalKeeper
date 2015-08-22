
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.Services
Imports DotNetNuke.Services.Localization
Imports Aricie.DNN.UI.WebControls
Imports System.Reflection
Imports System.Linq
Imports Aricie.Services
Imports System.Globalization

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class RuleEngineSettings(Of TEngineEvents As IConvertible)
        Inherits NamedConfig
        Implements IExpressionVarsProvider
        Implements IContextSource

        Public Overrides Function GetFriendlyDetails() As String
            Dim actionRuleCount As Integer
            Dim actionRuleSuffix As String = "actions"
            If Mode = RuleEngineMode.Actions Then
                actionRuleCount = Actions.Instances.Count
            Else
                actionRuleCount = Rules.Count
                actionRuleSuffix = "rules"
            End If
            Return String.Format("{4}{0}{1} params{0}{2} {3}", UIConstants.TITLE_SEPERATOR, _
                                 Me.Variables.Instances.Count.ToString(), _
                                 actionRuleCount.ToString(), _
                                 actionRuleSuffix, _
                                 MyBase.GetFriendlyDetails())
        End Function

        Public Sub New()
            'Me.ImportDefaultProviders()
        End Sub

        <ExtendedCategory("Rules")> _
        Public Overridable Property Mode As RuleEngineMode

        <ExtendedCategory("Variables")> _
            <SortOrder(2)> _
        Public Property Variables As New Variables()



        <ConditionalVisible("Mode", False, True, RuleEngineMode.Actions)> _
        <ExtendedCategory("Rules")> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(1)> _
        Public Overridable Property Actions As New KeeperAction(Of TEngineEvents)


        <ExtendedCategory("Rules")> _
        <ConditionalVisible("Mode", False, True, RuleEngineMode.Actions)> _
        <SortOrder(1)>
        Public Property InitialCondition() As New KeeperCondition(Of TEngineEvents)


        '<Editor(GetType(ListEditControl), GetType(EditControl))> _
        '<InnerEditor(GetType(PropertyEditorEditControl)), LabelMode(LabelMode.Top)> _
        '<CollectionEditor(False, False, True, True, 10, CollectionDisplayStyle.Accordion, True)> _
        <ConditionalVisible("Mode", False, True, RuleEngineMode.Rules)> _
        <ExtendedCategory("Rules")> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(1)> _
        Public Overridable Property Rules() As New List(Of KeeperRule(Of TEngineEvents))

        <SortOrder(1000)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property LoggingLevel As LoggingLevel = LoggingLevel.None

        ' <SortOrder(1000)> _
        ' <ExtendedCategory("TechnicalSettings")> _
        ' Public Property EnableSimpleLogs() As Boolean


        ' <SortOrder(1000)> _
        ' <ExtendedCategory("TechnicalSettings")> _
        '<ConditionalVisible("EnableSimpleLogs", False, True)> _
        ' Public Property EnableStopWatch() As Boolean


        <SortOrder(1000)> _
       <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("LoggingLevel", True, True, LoggingLevel.None)> _
        Public Property LogEndDumpSettings As New DumpSettings()

        <SortOrder(1000)> _
      <ExtendedCategory("TechnicalSettings")> _
        Public Property ExceptionDumpSettings As New DumpSettings()

        '<ExtendedCategory("TechnicalSettings")> _
        '<SortOrder(1000)> _
        'Public Property ExceptionDumpAllVars() As Boolean

        '<ConditionalVisible("ExceptionDumpAllVars", True, True)> _
        '<ExtendedCategory("TechnicalSettings")> _
        ' <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
        '   LineCount(4), Width(500)> _
        '   <SortOrder(1000)> _
        'Public Property ExceptionDumpVars() As String = String.Empty




        <ExtendedCategory("Providers", "Condition")> _
        <SortOrder(900)> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        Public Property ConditionProviders() As New ProviderList(Of ConditionProviderConfig(Of TEngineEvents))

        <ExtendedCategory("Providers", "Action")> _
        <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        <SortOrder(900)> _
        Public Property ActionProviders() As New ProviderList(Of ActionProviderConfig(Of TEngineEvents))

        <ExtendedCategory("Providers")> _
        <ActionButton(IconName.Repeat, IconOptions.Normal)>
        Public Sub ImportAvailableProviders(pe As AriciePropertyEditorControl)
            Dim nbImported As Integer = Me.ImportAvailableProviders()
            pe.ItemChanged = True
            pe.DisplayMessage(String.Format(Localization.GetString("DefaultProvidersImported.Message", pe.LocalResourceFile), nbImported), ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        Public Function ImportAvailableProviders() As Integer
            Dim toReturn As Integer
            For Each objAssembly As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                toReturn += ImportProviderTypes(objAssembly)
            Next
            Return toReturn
        End Function

        Public Function ImportDefaultProviders() As Integer
            Return ImportProviderTypes(GetType(PortalKeeperConfig).Assembly)
        End Function


        Public Function ImportProviderTypes(objAssembly As Assembly) As Integer
            Dim toReturn As Integer
            Dim assemblyTypes As Type() = Nothing
            Try
                assemblyTypes = objAssembly.GetExportedTypes()
            Catch
                Return 0
            End Try
            For Each objType As Type In assemblyTypes
                If Not objType.IsAbstract Then
                    If (Aricie.Common.IsAssignableToGenericType(objType, GetType(IConditionProvider(Of ))) AndAlso objType IsNot GetType(ConditionProvider(Of ))) _
                        OrElse (Aricie.Common.IsAssignableToGenericType(objType, GetType(IActionProvider(Of ))) AndAlso objType IsNot GetType(ActionProvider(Of ))) Then
                        If objType.IsGenericType Then
                            objType = objType.MakeGenericType(GetType(TEngineEvents))
                        End If
                        Dim objInterfaces As New List(Of Type)(objType.GetInterfaces())
                        Dim found As Boolean
                        If objInterfaces.Contains(GetType(IConditionProvider(Of TEngineEvents))) Then
                            If Me.ConditionProviders.Any(Function(objProvider) objProvider.ProviderType.IsAssignableFrom(objType)) Then
                                found = True
                            Else
                                found = False
                            End If
                            If Not found Then
                                Me.ConditionProviders.Add(New ConditionProviderConfig(Of TEngineEvents)(objType))
                                toReturn += 1
                            End If
                        ElseIf objInterfaces.Contains(GetType(IActionProvider(Of TEngineEvents))) Then
                            If Me.ActionProviders.Any(Function(objProvider) objProvider.ProviderType.IsAssignableFrom(objType)) Then
                                found = True
                            Else
                                found = False
                            End If
                            If Not found Then
                                Me.ActionProviders.Add(New ActionProviderConfig(Of TEngineEvents)(objType))
                                toReturn += 1
                            End If
                        End If
                    End If
                End If
            Next
            Return toReturn
        End Function

        Public Overridable Sub ProcessRules(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal objEvent As TEngineEvents, ByVal endSequence As Boolean, ByVal endoverhead As Boolean)
            context.ProcessRules(objEvent, Me, endSequence, endoverhead)
        End Sub

        Public Function BatchRun(events As IEnumerable(Of TEngineEvents), userParams As IDictionary(Of String, Object)) As PortalKeeperContext(Of TEngineEvents)

            Dim newContext As PortalKeeperContext(Of TEngineEvents) = Me.InitContext(userParams)

            Me.BatchRun(events, newContext)

            Return newContext
        End Function

        Public Function InitContext() As PortalKeeperContext(Of TEngineEvents)
            Dim toReturn As New PortalKeeperContext(Of TEngineEvents)
            toReturn.SetEngine(Me)
            toReturn.LogStartEngine()
            toReturn.Init(Me)
            Return toReturn
        End Function


        Public Function InitContext(userParams As IDictionary(Of String, Object)) As PortalKeeperContext(Of TEngineEvents)
            Dim toReturn As PortalKeeperContext(Of TEngineEvents) = Me.InitContext()
            toReturn.InitParams(userParams)
            Return toReturn
        End Function


        Public Sub BatchRun(events As IEnumerable(Of TEngineEvents), ByRef existingContext As PortalKeeperContext(Of TEngineEvents))
            For Each eventStep As TEngineEvents In events
                Me.ProcessRules(existingContext, eventStep, False, False)
            Next
        End Sub


        Public Overridable Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables

            'If Not existingVars.ContainsKey(FleeExpressionBuilder.ExpressionOwnerVar) Then
            existingVars(FleeExpressionBuilder.ExpressionOwnerVar) = GetType(PortalKeeperContext(Of TEngineEvents))
            'End If

            For Each objVar As VariableInfo In Me.Variables.Instances
                existingVars(objVar.Name) = ReflectionHelper.CreateType(objVar.VariableType)
            Next

            If Me.Mode = RuleEngineMode.Rules Then
                For Each objRule As KeeperRule(Of TEngineEvents) In Me.Rules
                    If objRule Is currentProvider Then
                        Exit For
                    End If
                    objRule.AddVariables(currentProvider, existingVars)
                Next
            ElseIf Me.Actions IsNot currentProvider Then
                Me.Actions.AddVariables(currentProvider, existingVars)
            End If

        End Sub


        Public Function HasContect(ByVal objType As Type) As Boolean Implements IContextSource.HasContext
            Return objType Is GetType(PortalKeeperContext(Of TEngineEvents))
        End Function

        Public Function GetContext(ByVal objType As Type) As Object Implements IContextSource.GetContext

            Return InitContext(Nothing)
        End Function
    End Class
End Namespace
