Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports System.Globalization
Imports Aricie.Services
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.ComponentModel
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper

    


    <ActionButton(IconName.Tasks, IconOptions.Normal)> _
    <Serializable()> _
    Public Class KeeperAction(Of TEngineEvents As IConvertible)
        Inherits ProviderHost(Of ActionProviderConfig(Of TEngineEvents), ActionProviderSettings(Of TEngineEvents), IActionProvider(Of TEngineEvents))
        Implements IExpressionVarsProvider

        Private Const DefaultEventStep As String = "Default"

        Public Property TimeLimit As New EnabledFeature(Of SimpleOrExpression(Of STimeSpan, TimeSpan))

        Public Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            'Dim enableStopWatch As Boolean = actionContext.EnableStopWatch

            Dim availableActions As ICollection(Of ActionProviderSettings(Of TEngineEvents)) = ProviderList(Of ActionProviderSettings(Of TEngineEvents)).GetAvailable(Me.Instances).Values
            Dim toReturn As Boolean = True
            Dim endTime As DateTime
            If TimeLimit.Enabled Then
                Dim timeKey As String = "EndTime-" & actionContext.CurrentRule.Name

                If Not actionContext.Items.ContainsKey(timeKey) Then

                    Dim timePair = TimeLimit.Entity.GetValuePair(actionContext, actionContext)
                    If TimeLimit.Entity.Mode = SimpleOrExpressionMode.Simple Then
                        endTime = Now.Add(timePair.Value.Value)
                    Else
                        endTime = Now.Add(timePair.Key)
                    End If
                Else
                    endTime = DirectCast(actionContext.Item(timeKey), DateTime)
                End If
            End If
            For Each element As ActionProviderSettings(Of TEngineEvents) In availableActions
                If TimeLimit.Enabled AndAlso Now > endTime Then
                    Exit For
                End If
                Dim prov As IActionProvider(Of TEngineEvents) = element.GetProvider
                Dim intCurrEvent As Integer = actionContext.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture)
                If element.LifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent _
                        OrElse (element.LifeCycleEvent.ToString(CultureInfo.InvariantCulture) = DefaultEventStep _
                                AndAlso (prov.Config.DefaultTEngineEvents.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent _
                                         OrElse (prov.Config.DefaultTEngineEvents.ToString(CultureInfo.InvariantCulture) = DefaultEventStep _
                                                 AndAlso (actionContext.CurrentRule Is Nothing _
                                                        OrElse actionContext.CurrentRule.MatchingLifeCycleEvent.ToInt32(CultureInfo.InvariantCulture) = intCurrEvent) _
                                                        OrElse prov.Config.MinTEngineEvents.ToString(CultureInfo.InvariantCulture) = prov.Config.MaxTEngineEvents.ToString(CultureInfo.InvariantCulture)))) Then

                    Try
                        If intCurrEvent > 0 AndAlso prov.Config.MinTEngineEvents.ToInt32(CultureInfo.InvariantCulture) > intCurrEvent _
                       OrElse prov.Config.MaxTEngineEvents.ToInt32(CultureInfo.InvariantCulture) < intCurrEvent Then
                            ' curl up and die here
                            Throw New InvalidOperationException(String.Format("The action ""{0}"" cannot be executed at the current step {1}, its provider ""{2}"" covers only the steps {3} to {4}", _
                                                                              element.Name, actionContext.CurrentEventStep, prov.Config.Name, prov.Config.MinTEngineEvents, prov.Config.MaxTEngineEvents))
                        Else
                            If (Not element.DisableLog) Then
                                actionContext.LogStart(element.Name, element.LoggingLevel, False)
                            End If

                            toReturn = toReturn And prov.Run(actionContext)
                        End If
                    Catch ex As Exception
                        If element.CaptureException Then
                            actionContext.Items(element.ExceptionVarName) = ex
                        End If
                        Dim xmlDump As String = ""
                        If actionContext.CurrentEngine.ExceptionDumpSettings.EnableDump Then
                            Dim dump As SerializableDictionary(Of String, Object) = actionContext.GetDump(actionContext.CurrentEngine.ExceptionDumpSettings)
                            xmlDump = ReflectionHelper.Serialize(dump).Beautify(True)
                        End If
                        Dim message As String
                        If actionContext.CurrentRule IsNot Nothing Then
                            message = String.Format("Action Exception,  Engine Name: {2} {0} Rule Name: {3} {0} Action Name: {4} {0}, InnerException: {1} {0} Dumped Vars: {5}", _
                                                   UIConstants.TITLE_SEPERATOR & vbCrLf, ex.ToString(), actionContext.CurrentEngine.Name, actionContext.CurrentRule.Name, element.Name, xmlDump)
                        Else
                            message = String.Format("Action Exception, Engine Name: {2} {0} No Rule {0} Action Name: {3} {0}, InnerException: {1} {0}  Dumped Vars: {4}", _
                                                   UIConstants.TITLE_SEPERATOR & vbCrLf, ex.ToString(), actionContext.CurrentEngine.Name, element.Name, xmlDump)
                        End If

                        Dim newEx As New ApplicationException(message, ex)
                        If Not element.DontLogExceptions Then
                            DotNetNuke.Services.Exceptions.LogException(newEx)
                        End If


                        If element.ExceptionActions.Enabled Then
                            toReturn = element.ExceptionActions.Entity.Run(actionContext)
                        Else
                            toReturn = False
                        End If
                        If element.RethrowException Then
                            Throw newEx
                        End If

                    Finally
                        If (Not element.DisableLog) AndAlso actionContext.LoggingLevel >= element.LoggingLevel Then
                            Dim actionResult As New KeyValuePair(Of String, String)("Action Result", toReturn.ToString(CultureInfo.InvariantCulture))
                            'Dim objStep As New StepInfo(Debug.PKPDebugType, String.Format("End - {0}", element.Name), WorkingPhase.InProgress, False, False, -1, actionContext.FlowId, actionResult)
                            'PerformanceLogger.Instance.AddDebugInfo(objStep)
                            actionContext.LogEnd(element.Name, False, element.LoggingLevel, element.LogDumpSettings, actionResult)
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

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            For Each objAction As ActionProviderSettings(Of TEngineEvents) In Me.Instances
                Dim objActionProvider As IExpressionVarsProvider = TryCast(objAction, IExpressionVarsProvider)
                If objActionProvider IsNot Nothing Then
                    If objActionProvider Is currentProvider Then
                        Exit For
                    End If
                    objActionProvider.AddVariables(currentProvider, existingVars)
                End If
            Next
        End Sub


    End Class
End Namespace