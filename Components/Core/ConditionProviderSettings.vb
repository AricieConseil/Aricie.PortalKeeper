Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.Xml.Serialization
Imports System.Globalization
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ConditionProviderSettings(Of TEngineEvents As IConvertible)
        Inherits AutoProvider(Of ConditionProviderConfig(Of TEngineEvents), ConditionProviderSettings(Of TEngineEvents), IConditionProvider(Of TEngineEvents))

        Public Property Negate() As Boolean

        Public Property IsMandatory() As Boolean

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

        Protected Friend Sub CheckIsCorrectStepRange(KC As PortalKeeperContext(Of TEngineEvents))
            If (Config IsNot Nothing) Then
                Dim CurrentStep = KC.CurrentEventStep.ToInt32(CultureInfo.InvariantCulture)
                If (Config.MinTEngineEvents.ToInt32(CultureInfo.InvariantCulture) > CurrentStep _
                    OrElse Config.MaxTEngineEvents.ToInt32(CultureInfo.InvariantCulture) < CurrentStep) Then
                    ' curl up and die here
                    Throw New InvalidOperationException(String.Format("The condition ""{0}"" cannot be evaluated at the current step {1}, its provider ""{2}"" covers only the steps {3} to {4}", _
                                                                      Name, KC.CurrentEventStep, Config.Name, Config.MinTEngineEvents, Config.MaxTEngineEvents))
                End If
            End If
        End Sub

    End Class
End Namespace