
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports Aricie.Collections
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper

    'Public MustInherit Class ProviderHost(Of TSettings As {IProviderSettings}, TProvider As IProvider(Of TSettings), TProviderSettings As {AutoProvider(Of TProvider, TProviderSettings), New})

    <ActionButton(IconName.Question, IconOptions.Normal)> _
    <Serializable()> _
    Public Class KeeperCondition(Of TEngineEvents As IConvertible)
        Inherits ProviderHost(Of ConditionProviderConfig(Of TEngineEvents), ConditionProviderSettings(Of TEngineEvents), IConditionProvider(Of TEngineEvents))
        'Implements IConditionProvider(Of TEngineEvents)





        Public Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean 'Implements IConditionProvider(Of TEngineEvents).Match
            Dim toReturn As Boolean = False
            Dim enableStopWatch As Boolean = context.EnableStopWatch
            Dim avConditions = ProviderList(Of ConditionProviderSettings(Of TEngineEvents)).GetAvailable(Me.Instances).Values
            If avConditions.Count = 0 Then
                toReturn = True
            End If
            For Each element As ConditionProviderSettings(Of TEngineEvents) In avConditions
                ' check our evaluation range...
                element.CheckIsCorrectStepRange(context)

                If enableStopWatch Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("Eval {0} - Start", element.Name), WorkingPhase.InProgress, False, False, -1, context.FlowId)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                If element.IsMandatory Then
                    If Not (element.GetProvider.Match(context) Xor element.Negate) Then
                        If enableStopWatch Then
                            Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", False.ToString())
                            Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("End Eval - {0} ", element.Name), WorkingPhase.InProgress, False, False, -1, context.FlowId, conditionResult)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        Return False
                    Else
                        toReturn = True
                    End If
                ElseIf Not toReturn Then
                    toReturn = element.GetProvider.Match(context) Xor element.Negate
                End If
                If enableStopWatch Then
                    Dim conditionResult As New KeyValuePair(Of String, String)("Condition Result", toReturn.ToString())
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "End Eval Condition: " & element.Name, WorkingPhase.InProgress, False, False, -1, context.FlowId, conditionResult)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
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


    End Class


End Namespace


