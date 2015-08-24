Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Configuration
Imports DotNetNuke.UI.WebControls
Imports Aricie.ComponentModel
Imports Aricie.DNN.Services.Errors
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.Entities.Users

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Shield, IconOptions.Normal)> _
    <Serializable()> _
    Public Class FirewallConfig
        Inherits FirewallSettings
        Implements IMergeable(Of FirewallSettings, FirewallConfig)


        Private _RecoveryParam As String

        Private _RestartParam As String


        <ExtendedCategory("")> _
        <MainCategory()> _
        Public Property EnablePortalLevelSettings() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        Public Property EnableRecoveryParams As Boolean


        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("EnableRecoveryParams")> _
        <Width(450)> _
                    <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RecoveryParam() As String
            Get
                If Not Me.EnableRecoveryParams Then
                    Return String.Empty
                End If
                If String.IsNullOrEmpty(_RecoveryParam) Then
                    _RecoveryParam = HttpUtility.UrlEncode(UserController.GeneratePassword(15))
                End If
                Return _RecoveryParam
            End Get
            Set(ByVal value As String)
                _RecoveryParam = value
            End Set
        End Property


        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("EnableRecoveryParams")> _
        <Width(450)> _
                    <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RestartParam() As String
            Get
                If Not Me.EnableRecoveryParams Then
                    Return String.Empty
                End If
                If String.IsNullOrEmpty(_RestartParam) Then
                    _RestartParam = HttpUtility.UrlEncode(UserController.GeneratePassword(15))
                End If
                Return _RestartParam
            End Get
            Set(ByVal value As String)
                _RestartParam = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property DosSettings() As New DenialOfServiceSettings

        'todo: make obsolete
        <Browsable(False)> _
        Public Property RestServices() As RestServicesSettings

        <ExtendedCategory("Compression")> _
        Public Property UrlCompression As New UrlCompressionInfo()

        <ExtendedCategory("CustomErrorPage")> _
        <SortOrder(400)> _
        Public Property CustomErrorsConfig() As New VirtualCustomErrorsInfo




        Public Function GetMerge(ByVal portalConfig As FirewallSettings) As FirewallConfig Implements IMergeable(Of FirewallSettings, FirewallConfig).GetMerge
            If Not portalConfig.Enabled Then
                Return Me
            End If
            'todo: review and complete that method

            Dim newKey As String = Me.cacheKey & portalConfig.cacheKey

            Dim toReturn As FirewallConfig = GetGlobal(Of FirewallConfig)(newKey)

            If toReturn Is Nothing Then
                toReturn = ReflectionHelper.CloneObject(Of FirewallConfig)(Me)
                'toReturn.RecoveryParam = toReturn.RecoveryParam & "," & portalConfig.RecoveryParam
                toReturn.RequestScope = portalConfig.RequestScope
                toReturn.IgnoredExtensions = toReturn.IgnoredExtensions & "," & portalConfig.IgnoredExtensions
                toReturn.Rules.AddRange(portalConfig.Rules)
                'Aricie.Services.CacheHelper.SetCacheDependant(Of FirewallConfig)(toReturn, New String() {PortalKeeperConfig.GetFilePath(True), _
                '                                    SettingsController.GetModuleSettingsKey(Of FirewallSettings)(SettingsScope.PortalSettings, NukeHelper.PortalId)}, _
                '                                     Constants.Cache.GlobalExpiration, newKey)
                SetCacheDependant(Of FirewallConfig)(toReturn, New String() {PortalKeeperConfig.GetFilePath(True)}, Constants.Cache.GlobalExpiration, newKey)
            End If
            Return toReturn
        End Function


        '<ExtendedCategory("CustomErrorPage")> _
        '<ActionButton("~/images/fwd.gif")> _
        'Public Sub SetCustomErrors(pmb As AriciePortalModuleBase)
        '    Dim customErrorsUpdater As IUpdateProvider = PortalKeeperConfig.Instance.GetUpdateProvider
        '    Configuration.ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Install, customErrorsUpdater)
        '    Skin.AddModuleMessage(pmb, Localization.GetString("CustomErrorsSaved.Message", pmb.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
        'End Sub
    End Class
End Namespace
