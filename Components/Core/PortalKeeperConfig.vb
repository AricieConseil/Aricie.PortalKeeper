Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Configuration
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class PortalKeeperConfig
        Inherits AutoModuleConfig(Of PortalKeeperConfig)

        Public Overrides Function GetModuleName() As String
            Return PortalKeeperContext(Of RequestEvent).MODULE_NAME
        End Function

        Private _RestServices As New RestServicesSettings

        <ExtendedCategory("RestServices")> _
        Public Property RestServices() As RestServicesSettings
            Get
                If Me.FirewallConfig.RestServices IsNot Nothing Then
                    _RestServices = Me.FirewallConfig.RestServices
                    Me.FirewallConfig.RestServices = Nothing
                End If
                Return _RestServices
            End Get
            Set(value As RestServicesSettings)
                _RestServices = value
            End Set
        End Property

        <ExtendedCategory("Firewall")> _
        Public Property FirewallConfig() As New FirewallConfig

        <ExtendedCategory("ControlAdapters")> _
        Public Property ControlAdapters As New ControlAdaptersConfig


        <ExtendedCategory("Scheduler")> _
        Public Property SchedulerFarm() As New BotFarmInfo(Of ScheduleEvent)


          

        Public Function GetRuleEnginesSettings(Of TEngineEvents As IConvertible)() As IEnumerable(Of RuleEngineSettings(Of TEngineEvents))
            Dim toReturn As New List(Of RuleEngineSettings(Of TEngineEvents))
            Select Case GetType(TEngineEvents).Name
                Case GetType(RequestEvent).Name
                    toReturn.Add(DirectCast(DirectCast(Me.FirewallConfig, Object), RuleEngineSettings(Of TEngineEvents)))
                Case GetType(ScheduleEvent).Name
                    For Each objBot As BotInfo(Of ScheduleEvent) In Me.SchedulerFarm.Bots.Instances
                        toReturn.Add(DirectCast(DirectCast(objBot, Object), RuleEngineSettings(Of TEngineEvents)))
                    Next
            End Select
            Return toReturn
        End Function

        Public Function GetBotFarm(Of TEngineEvents As IConvertible)() As BotFarmInfo(Of TEngineEvents)
            Select Case GetType(TEngineEvents).Name
                Case GetType(ScheduleEvent).Name
                    Return DirectCast(DirectCast(Me.SchedulerFarm, Object), BotFarmInfo(Of TEngineEvents))
                Case Else
                    Return Nothing
            End Select
        End Function


        'Public Function GetUpdateProvider() As IUpdateProvider
        '    If Me.FirewallConfig.CustomErrorsConfig.UseAshx Then
        '        Me.FirewallConfig.CustomErrorsConfig.VirtualHandlerPath = NukeHelper.GetModuleDirectoryPath(PortalKeeperConfig.Instance.GetModuleName) & "Error.ashx"
        '        Return New VirtualCustomErrorUpdater(Me.FirewallConfig.CustomErrorsConfig)
        '    Else
        '        Return New CustomErrorDynamicHandlerUpdater(GetType(KeeperErrorsHandler), Me.FirewallConfig.CustomErrorsConfig)
        '    End If
        'End Function

      

    End Class
End Namespace
