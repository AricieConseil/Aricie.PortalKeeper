Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports System.Globalization

Namespace Aricie.DNN.Modules.PortalKeeper



    
    Public Class ActionProviderSettings(Of TEngineEvents As IConvertible)
        Inherits AutoProvider(Of ActionProviderConfig(Of TEngineEvents), ActionProviderSettings(Of TEngineEvents), IActionProvider(Of TEngineEvents))

        <Browsable(False)> _
        Public ReadOnly Property HasEvent As Boolean
            Get
                Return GetType(TEngineEvents) IsNot GetType(Boolean)
            End Get
        End Property

        <ConditionalVisible("HasEvent", False, True)> _
        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Property LifeCycleEvent() As TEngineEvents

        Private _LifeCycleEventIsDefault As Nullable(Of Boolean)

        <Browsable(False)> _
        Public ReadOnly Property LifeCycleEventIsDefault As Boolean
            Get
                If Not _LifeCycleEventIsDefault.HasValue Then
                    _LifeCycleEventIsDefault = (LifeCycleEvent.ToString(CultureInfo.InvariantCulture) = KeeperAction(Of TEngineEvents).DefaultEventStep)
                End If
                Return _LifeCycleEventIsDefault.Value
            End Get
        End Property

        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Property StopOnFailure() As Boolean

        <ExtendedCategory("RuleSettings")> _
        <SortOrder(700)> _
        Public Overridable Property ExitAction() As Boolean




        <SortOrder(2000)> _
        <ExtendedCategory("TechnicalSettings", "Debug")> _
        Public Property DisableLog As Boolean

        <SortOrder(2000)> _
        <ExtendedCategory("TechnicalSettings", "Debug")> _
         <ConditionalVisible("DisableLog", True, True)> _
        Public Property LoggingLevel As LoggingLevel = LoggingLevel.Detailed

        <SortOrder(2000)> _
         <ExtendedCategory("TechnicalSettings", "Debug")> _
         <ConditionalVisible("DisableLog", True, True)> _
        Public Property LogDumpSettings As New DumpSettings()

        <ExtendedCategory("TechnicalSettings", "Exceptions")> _
        <SortOrder(1000)> _
        Public Property ExceptionActions As New EnabledFeature(Of KeeperAction(Of TEngineEvents))

        <ExtendedCategory("TechnicalSettings", "Exceptions")> _
        <SortOrder(1000)> _
        Public Property RethrowException() As Boolean

        <ExtendedCategory("TechnicalSettings", "Exceptions")> _
        <SortOrder(1000)> _
        Public Property DontLogExceptions() As Boolean


        <ExtendedCategory("TechnicalSettings", "Exceptions")> _
        <ConditionalVisible("DontLogExceptions", False, True)> _
        <SortOrder(1000)> _
        Public Property CaptureException() As Boolean

        <Required(True)> _
        <ExtendedCategory("TechnicalSettings", "Exceptions")> _
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
            Return actionContext.GetAdvancedTokenReplace()
        End Function


    End Class
End Namespace