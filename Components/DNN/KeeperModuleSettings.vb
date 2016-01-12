Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class KeeperModuleSettings
        Implements ISelector(Of UserBotSettings(Of ScheduleEvent))

        Private _AssignUserBot As Boolean

        Private _UserBotName As String = ""

        Private _DisplayBotConfig As Boolean

        Private _DisplayRankings As Boolean

        <ExtendedCategory("UserBots")> _
        Public ReadOnly Property EnableUserBots() As Boolean
            Get
                Return PortalKeeperConfig.Instance.SchedulerFarm.EnableUserBots
            End Get
        End Property


        '<ExtendedCategory("UserBots")> _
        '<ConditionalVisible("EnableUserBots", False, True)> _
        'Public Property AssignUserBot() As Boolean
        '    Get
        '        Return PortalKeeperConfig.Instance.SchedulerFarm.EnableUserBots AndAlso _AssignUserBot
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _AssignUserBot = value
        '    End Set
        'End Property


        '<ConditionalVisible("AssignUserBot", False, True)> _
        '<ProvidersSelector()> _
        <ExtendedCategory("UserBots")> _
           <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
           <Selector("Name", "Name", False, True, "<Select a User Bot>", "", False, True)> _
        <AutoPostBack()> _
        Public Property UserBotName() As String
            Get
                Return _UserBotName
            End Get
            Set(ByVal value As String)
                _UserBotName = value
            End Set
        End Property

        <ExtendedCategory("UserBots")> _
            <ConditionalVisible("UserBotName", True, True, "")> _
        Public Property DisplayBotConfig() As Boolean
            Get
                Return _DisplayBotConfig
            End Get
            Set(ByVal value As Boolean)
                _DisplayBotConfig = value
            End Set
        End Property

        <ExtendedCategory("UserBots")> _
            <ConditionalVisible("UserBotName", True, True, "")> _
        Public Property DisplayRankings() As Boolean
            Get
                Return _DisplayRankings
            End Get
            Set(ByVal value As Boolean)
                _DisplayRankings = value
            End Set
        End Property

        <ConditionalVisible("UserBotName", True, True, "")> _
        <ExtendedCategory("UserBots")> _
        Public Property EnableActionCommands() As Boolean = True

        '<ConditionalVisible("EnableActionCommands", False, True)> _
        '<ExtendedCategory("UserBots")> _
        'Public Property DisableComputationRefresh() As Boolean


        <ExtendedCategory("UserBots")> _
          <ConditionalVisible("UserBotName", True, True, "")> _
          <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
          <LabelMode(LabelMode.Top)> _
        Public ReadOnly Property UserBot() As UserBotSettings(Of ScheduleEvent)
            Get
                Dim toReturn As UserBotSettings(Of ScheduleEvent) = Nothing
                PortalKeeperConfig.Instance.SchedulerFarm.AvailableUserBots.TryGetValue(Me._UserBotName, toReturn)
                Return toReturn
            End Get
        End Property

        Public Function GetSelector(ByVal propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        Public Function GetSelectorG(ByVal propertyName As String) As IList(Of UserBotSettings(Of ScheduleEvent)) Implements ISelector(Of UserBotSettings(Of ScheduleEvent)).GetSelectorG
            Return New List(Of UserBotSettings(Of ScheduleEvent))(PortalKeeperConfig.Instance.SchedulerFarm.AvailableUserBots.Values)
        End Function
    End Class
End Namespace