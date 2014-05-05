Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Workers
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ActionProviderSettings(Of TEngineEvents As IConvertible)
        Inherits AutoProvider(Of ActionProviderConfig(Of TEngineEvents), ActionProviderSettings(Of TEngineEvents), IActionProvider(Of TEngineEvents))


        Public ReadOnly Property HasEvent As Boolean
            Get
                Return GetType(TEngineEvents) IsNot GetType(Boolean)
            End Get
        End Property

        <ConditionalVisible("HasEvent", True, True)> _
        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Property LifeCycleEvent() As TEngineEvents

        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Property StopOnFailure() As Boolean

        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Overridable Property ExitAction() As Boolean


        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property RethrowException() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property DontLogExceptions() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("DontLogExceptions", False, True)> _
        <SortOrder(1000)> _
        Public Property CaptureException() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("CaptureException", False, True)> _
        <SortOrder(1000)> _
        Public Property ExceptionVarName() As String = "ActionException"

      

        


        Protected Overrides Function CreateProvider() As IActionProvider(Of TEngineEvents)
            Dim toreturn As IActionProvider(Of TEngineEvents)
            Dim engines As IEnumerable(Of RuleEngineSettings(Of TEngineEvents)) = PortalKeeperConfig.Instance.GetRuleEnginesSettings(Of TEngineEvents)()
            For Each engine As RuleEngineSettings(Of TEngineEvents) In engines
                Dim provConfig As ActionProviderConfig(Of TEngineEvents) = Nothing
                If engine.ActionProviders.Available.TryGetValue(Me.ProviderName, provConfig) Then
                    If TypeOf Me Is IActionProvider(Of TEngineEvents) Then
                        toreturn = DirectCast(Me, IActionProvider(Of TEngineEvents))
                    Else
                        toreturn = provConfig.GetTypedProvider
                    End If
                    toreturn.Config = provConfig
                    'toreturn.Settings = Me
                    Return toreturn
                End If
            Next
            Return Nothing

        End Function





        Protected Overridable Function GetAdvancedTokenReplace(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As AdvancedTokenReplace
            Dim objTokenReplace As New AdvancedTokenReplace()
            objTokenReplace.SetObjectReplace(actionContext, "Context")
            objTokenReplace.SetObjectReplace(actionContext.Items, "Items")
            If actionContext.CurrentRule IsNot Nothing Then
                objTokenReplace.SetObjectReplace(actionContext.CurrentRule, "Rule")
            End If

            Return objTokenReplace
        End Function


    End Class
End Namespace