Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.Globalization
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ConditionProviderSettings(Of TEngineEvents As IConvertible)
        Inherits AutoProvider(Of ConditionProviderConfig(Of TEngineEvents), ConditionProviderSettings(Of TEngineEvents), IConditionProvider(Of TEngineEvents))

        <ExtendedCategory("Condition")> _
        Public Property Negate() As Boolean

        <ExtendedCategory("Condition")> _
        Public Property IsMandatory() As Boolean


        <ExtendedCategory("TechnicalSettings")> _
        Public Property LoggingLevel As LoggingLevel = LoggingLevel.Detailed

        <ExtendedCategory("TechnicalSettings")> _
        Public Property LogDumpSettings As New DumpSettings()

        <ExtendedCategory("TechnicalSettings")> _
        Public Property DebuggerBreak As Boolean

        <SortOrder(200)> _
        <ExtendedCategory("PreConditionActions")> _
        Public Property AddPreConditionActions As Boolean

        Private _PreConditionActions As KeeperAction(Of TEngineEvents)

        <SortOrder(200)> _
        <ConditionalVisible("AddPreConditionActions", False, True)> _
        <ExtendedCategory("PreConditionActions")> _
        Public Property PreConditionActions As KeeperAction(Of TEngineEvents)
            Get
                If Not Me.AddPreConditionActions Then
                    Return Nothing
                End If
                If _PreConditionActions Is Nothing Then
                    _PreConditionActions = New KeeperAction(Of TEngineEvents)
                End If
                Return _PreConditionActions
            End Get
            Set(value As KeeperAction(Of TEngineEvents))
                _PreConditionActions = value
            End Set
        End Property

        <SortOrder(200)> _
        <ConditionalVisible("AddPreConditionActions", False, True)> _
        <ExtendedCategory("PreConditionActions")> _
        Public Property StopActions As Boolean

        <SortOrder(200)> _
        <ConditionalVisible("StopActions", False, True)> _
        <ConditionalVisible("AddPreConditionActions", False, True)> _
        <ExtendedCategory("PreConditionActions")> _
        Public Property StopMatches As Boolean


        Protected Overrides Function CreateProvider() As IConditionProvider(Of TEngineEvents)
            Dim toreturn As IConditionProvider(Of TEngineEvents)
            Dim engines As IEnumerable(Of RuleEngineSettings(Of TEngineEvents)) = PortalKeeperConfig.Instance.GetRuleEnginesSettings(Of TEngineEvents)()
            For Each engine As RuleEngineSettings(Of TEngineEvents) In engines
                Dim provConfig As ConditionProviderConfig(Of TEngineEvents) = Nothing
                If engine.ConditionProviders.Available.TryGetValue(Me.ProviderName, provConfig) Then
                    If TypeOf Me Is IConditionProvider(Of TEngineEvents) Then
                        toreturn = DirectCast(Me, IConditionProvider(Of TEngineEvents))
                    Else
                        toreturn = provConfig.GetTypedProvider
                    End If
                    toreturn.Config = provConfig
                    Return toreturn
                End If
            Next
            Return Nothing
        End Function

        Protected Friend Sub CheckIsCorrectStepRange(keeperContext As PortalKeeperContext(Of TEngineEvents))
            If (Config IsNot Nothing) Then
                Dim objCurrentStep = keeperContext.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture)
                If objCurrentStep > 0 AndAlso (Config.MinTEngineEvents.ToInt32(CultureInfo.InvariantCulture) > objCurrentStep _
                    OrElse Config.MaxTEngineEvents.ToInt32(CultureInfo.InvariantCulture) < objCurrentStep) Then
                    ' curl up and die here
                    Throw New InvalidOperationException(String.Format("The condition ""{0}"" cannot be evaluated at the current step {1}, its provider ""{2}"" covers only the steps {3} to {4}", _
                                                                      Name, keeperContext.CurrentEventStep, Config.Name, Config.MinTEngineEvents, Config.MaxTEngineEvents))
                End If
            End If
        End Sub

    End Class
End Namespace