Imports Aricie.DNN.Services.UpgradeSystem.Actions.Settings.Configuration

Namespace Services.UpgradeSystem.Actions.Settings
    ''' <summary>
    ''' Interface générique pour les actions de mise à jour des paramètres
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IUpgradeSettingsAction
        Sub UpgradeTo(ByVal TargetVersion As Version, ByVal scopeId As Integer, ByVal configRepo As IConfigurationRepository)
    End Interface

    ''' <summary>
    ''' Interface pour les mises à jour de paramètres globaux
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IUpgradeHostSettingsAction
        Inherits IUpgradeSettingsAction
    End Interface

    ''' <summary>
    ''' Interface pour les mises à jour de paramètres de portail
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IUpgradePortalSettingsAction
        Inherits IUpgradeSettingsAction
    End Interface

    ''' <summary>
    ''' Interface pour les mises à jour de paramètres de module
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IUpgradeModuleSettingsAction
        Inherits IUpgradeSettingsAction
    End Interface
End Namespace