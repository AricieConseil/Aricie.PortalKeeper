
Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    'Public MustInherit Class ProviderHost(Of TSettings As {IProviderSettings}, TProvider As IProvider(Of TSettings), TProviderSettings As {AutoProvider(Of TProvider, TProviderSettings), New})

    <ActionButton(IconName.Question, IconOptions.Normal)> _
    <Serializable()> _
    Public Class KeeperCondition(Of TEngineEvents As IConvertible)
        Inherits ProviderHost(Of ConditionProviderConfig(Of TEngineEvents), ConditionProviderSettings(Of TEngineEvents), IConditionProvider(Of TEngineEvents))
        Implements IExpressionVarsProvider


        'Implements IConditionProvider(Of TEngineEvents)





        Public Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean 'Implements IConditionProvider(Of TEngineEvents).Match
            Dim toReturn As Boolean = False
            'Dim enableStopWatch As Boolean = context.EnableStopWatch
            Dim avConditions = ProviderList(Of ConditionProviderSettings(Of TEngineEvents)).GetAvailable(Me.Instances).Values
            If avConditions.Count = 0 Then
                toReturn = True
            End If
            For Each element As ConditionProviderSettings(Of TEngineEvents) In avConditions
                ' check our evaluation range...
                element.CheckIsCorrectStepRange(context)

                
                If element.IsMandatory Then
                    If context.LoggingLevel >= element.LoggingLevel Then
                        'Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("Eval {0} - Start", element.Name), WorkingPhase.InProgress, False, False, -1, context.FlowId)
                        'PerformanceLogger.Instance.AddDebugInfo(objStep)
                        context.LogStart(String.Format("Eval {0}", element.Name), element.LoggingLevel, False)
                    End If
                    If Not (element.GetProvider().Match(context) Xor element.Negate) Then
                        'If enableStopWatch Then
                        '    Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", False.ToString())
                        '    Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("End Eval - {0} ", element.Name), WorkingPhase.InProgress, False, False, -1, context.FlowId, conditionResult)
                        '    PerformanceLogger.Instance.AddDebugInfo(objStep)
                        'End If
                        If context.LoggingLevel >= element.LoggingLevel Then
                            Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", False.ToString())
                            context.LogEnd(String.Format("Eval {0}", element.Name), False, element.LoggingLevel, element.LogDumpSettings, conditionResult)
                        End If
                        toReturn = False
                        Exit For
                    Else
                        toReturn = True
                    End If
                    If context.LoggingLevel >= element.LoggingLevel Then
                        Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", toReturn.ToString())
                        context.LogEnd(String.Format("Eval {0}", element.Name), False, element.LoggingLevel, element.LogDumpSettings, conditionResult)
                    End If
                ElseIf Not toReturn Then
                    If context.LoggingLevel >= element.LoggingLevel Then
                        'Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("Eval {0} - Start", element.Name), WorkingPhase.InProgress, False, False, -1, context.FlowId)
                        'PerformanceLogger.Instance.AddDebugInfo(objStep)
                        context.LogStart(String.Format("Eval {0}", element.Name), element.LoggingLevel, False)
                    End If
                    toReturn = element.GetProvider().Match(context) Xor element.Negate
                    If context.LoggingLevel >= element.LoggingLevel Then
                        Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", toReturn.ToString())
                        context.LogEnd(String.Format("Eval {0}", element.Name), False, element.LoggingLevel, element.LogDumpSettings, conditionResult)
                    End If
                End If
                'If enableStopWatch Then
                '    Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", toReturn.ToString())
                '    Dim objStep As New StepInfo(Debug.PKPDebugType, "End Eval Condition: " & element.Name, WorkingPhase.InProgress, False, False, -1, context.FlowId, conditionResult)
                '    PerformanceLogger.Instance.AddDebugInfo(objStep)
                'End If
               
            Next
            Return toReturn
        End Function



        Public Overrides Function GetAvailableProviders() As IDictionary(Of String, ConditionProviderConfig(Of TEngineEvents))
            Dim toReturn As New SerializableDictionary(Of String, ConditionProviderConfig(Of TEngineEvents))
            Dim engines As IEnumerable(Of RuleEngineSettings(Of TEngineEvents)) = PortalKeeperConfig.Instance.GetRuleEnginesSettings(Of TEngineEvents)()
            For Each engine As RuleEngineSettings(Of TEngineEvents) In engines
                For Each prov As KeyValuePair(Of String, ConditionProviderConfig(Of TEngineEvents)) In engine.ConditionProviders.Available
                    toReturn(prov.Key) = prov.Value
                Next
            Next
            Return toReturn
        End Function


        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            For Each objCondition As ConditionProviderSettings(Of TEngineEvents) In Me.Instances
                Dim objConditionProvider As IExpressionVarsProvider = TryCast(objCondition, IExpressionVarsProvider)
                If objConditionProvider IsNot Nothing Then
                    If objConditionProvider Is currentProvider Then
                        Exit For
                    End If
                    objConditionProvider.AddVariables(currentProvider, existingVars)
                End If
            Next
        End Sub


    End Class


End Namespace


