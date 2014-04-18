Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.Diagnostics
Imports System.Globalization
Imports Aricie.Services
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Tasks, IconOptions.Normal)> _
    <Serializable()> _
    Public Class KeeperAction(Of TEngineEvents As IConvertible)
        Inherits ProviderHost(Of ActionProviderConfig(Of TEngineEvents), ActionProviderSettings(Of TEngineEvents), IActionProvider(Of TEngineEvents))


        Public Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Dim enableStopWatch As Boolean = actionContext.EnableStopWatch

            Dim availableActions As ICollection(Of ActionProviderSettings(Of TEngineEvents)) = ProviderList(Of ActionProviderSettings(Of TEngineEvents)).GetAvailable(Me.Instances).Values
            Dim toReturn As Boolean = True
            For Each element As ActionProviderSettings(Of TEngineEvents) In availableActions
                Dim prov As IActionProvider(Of TEngineEvents) = element.GetProvider
                Dim intCurrEvent As Integer = actionContext.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture)
                If element.LifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent _
                        OrElse (element.LifeCycleEvent.ToString(CultureInfo.InvariantCulture) = "Default" _
                                AndAlso (prov.Config.DefaultTEngineEvents.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent _
                                         OrElse (prov.Config.DefaultTEngineEvents.ToString(CultureInfo.InvariantCulture) = "Default" _
                                                 AndAlso actionContext.CurrentRule.MatchingLifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent))) Then

                    Try
                        If intCurrEvent > 0 AndAlso prov.Config.MinTEngineEvents.ToInt32(CultureInfo.InvariantCulture) > intCurrEvent _
                       OrElse prov.Config.MaxTEngineEvents.ToInt32(CultureInfo.InvariantCulture) < intCurrEvent Then
                            ' curl up and die here
                            Throw New InvalidOperationException(String.Format("The action ""{0}"" cannot be executed at the current step {1}, its provider ""{2}"" covers only the steps {3} to {4}", element.Name, actionContext.CurrentEventStep, prov.Config.Name, prov.Config.MinTEngineEvents, prov.Config.MaxTEngineEvents))
                        Else

                            If enableStopWatch Then
                                Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("{0} - Start", element.Name), WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                                PerformanceLogger.Instance.AddDebugInfo(objStep)
                            End If
                            toReturn = toReturn And prov.Run(actionContext)
                        End If
                    Catch ex As Exception
                        If element.CaptureException Then
                            actionContext.Items(element.ExceptionVarName) = ex
                        End If
                        Dim xmlDump As String = ""
                        If actionContext.CurrentEngine.ExceptionDumpAllVars OrElse actionContext.CurrentEngine.ExceptionDumpVars.Length > 0 Then
                            Dim dumpVars As List(Of String) = ParseStringList(actionContext.CurrentEngine.ExceptionDumpVars)
                            Dim dump As SerializableDictionary(Of String, Object) = actionContext.GetDump(actionContext.CurrentEngine.ExceptionDumpAllVars, dumpVars)
                            xmlDump = ReflectionHelper.Serialize(dump).InnerXml
                        End If
                        Dim message As String = String.Format("Action Exception, InnerException: {1} {0} Engine Name: {2} {0} Rule Name: {3} {0} Action Name: {4} {0} Dumped Vars: {5}", vbCrLf, ex.ToString(), actionContext.CurrentEngine.Name, actionContext.CurrentRule.Name, element.Name, xmlDump)
                        Dim newEx As New ApplicationException(message, ex)
                        If Not element.DontLogExceptions Then
                            DotNetNuke.Services.Exceptions.LogException(newEx)
                        End If
                        If element.RethrowException Then
                            Throw newEx
                        End If
                    Finally
                        If enableStopWatch Then
                            Dim actionResult As New KeyValuePair(Of String, String)("Action Result", toReturn.ToString(CultureInfo.InvariantCulture))
                            Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("End - {0}", element.Name), WorkingPhase.InProgress, False, False, -1, actionContext.FlowId, actionResult)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                    End Try
                    If (Not toReturn) AndAlso element.StopOnFailure Then
                        Return False
                    ElseIf toReturn AndAlso element.ExitAction Then
                        Exit For
                    End If
                End If

            Next
            Return toReturn
        End Function


        Public Overrides Function GetAvailableProviders() As IDictionary(Of String, ActionProviderConfig(Of TEngineEvents))
            Dim toReturn As New SerializableDictionary(Of String, ActionProviderConfig(Of TEngineEvents))
            Dim engines As IEnumerable(Of RuleEngineSettings(Of TEngineEvents)) = PortalKeeperConfig.Instance.GetRuleEnginesSettings(Of TEngineEvents)()
            For Each engine As RuleEngineSettings(Of TEngineEvents) In engines
                For Each prov As KeyValuePair(Of String, ActionProviderConfig(Of TEngineEvents)) In engine.ActionProviders.Available
                    toReturn(prov.Key) = prov.Value
                Next
            Next
            Return toReturn
        End Function
    End Class
End Namespace