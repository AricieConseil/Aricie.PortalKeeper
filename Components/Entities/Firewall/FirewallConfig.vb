Imports System.ComponentModel
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.UI.WebControls
Imports Aricie.ComponentModel
Imports Aricie.DNN.Settings
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Errors
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins.Controls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class FirewallConfig
        Inherits FirewallSettings
        Implements IMergeable(Of FirewallSettings, FirewallConfig)

        <ExtendedCategory("")> _
        <MainCategory()> _
        Public Property EnablePortalLevelSettings() As Boolean

        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1000)> _
        Public Property DosSettings() As New DenialOfServiceSettings

        'todo: make obsolete
        <Browsable(False)> _
        Public Property RestServices() As RestServicesSettings

        <ExtendedCategory("CustomErrorPage")> _
        <SortOrder(400)> _
        Public Property CustomErrorsConfig() As New VirtualCustomErrorsInfo




        Public Function GetMerge(ByVal portalConfig As FirewallSettings) As FirewallConfig Implements IMergeable(Of FirewallSettings, FirewallConfig).GetMerge
            If Not portalConfig.Enabled Then
                Return Me
            End If

            Dim newKey As String = Me.cacheKey & portalConfig.cacheKey

            Dim toReturn As FirewallConfig = Aricie.Services.CacheHelper.GetGlobal(Of FirewallConfig)(newKey)

            If toReturn Is Nothing Then
                toReturn = Aricie.Services.ReflectionHelper.CloneObject(Of FirewallConfig)(Me)
                toReturn.RecoveryParam = toReturn.RecoveryParam & "," & portalConfig.RecoveryParam
                toReturn.RequestScope = portalConfig.RequestScope
                toReturn.IgnoredExtensions = toReturn.IgnoredExtensions & "," & portalConfig.IgnoredExtensions
                toReturn.Rules.AddRange(portalConfig.Rules)
                'Aricie.Services.CacheHelper.SetCacheDependant(Of FirewallConfig)(toReturn, New String() {PortalKeeperConfig.GetFilePath(True), _
                '                                    SettingsController.GetModuleSettingsKey(Of FirewallSettings)(SettingsScope.PortalSettings, NukeHelper.PortalId)}, _
                '                                     Constants.Cache.GlobalExpiration, newKey)
                Aricie.Services.CacheHelper.SetCacheDependant(Of FirewallConfig)(toReturn, New String() {PortalKeeperConfig.GetFilePath(True)}, Constants.Cache.GlobalExpiration, newKey)
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
