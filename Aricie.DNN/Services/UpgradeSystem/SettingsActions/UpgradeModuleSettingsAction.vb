
Imports Aricie.DNN.Settings

Namespace Services.UpgradeSystem.Actions.Settings

    ''' <summary>
    ''' Implémentation de la classe de mise à jour des paramètres de module
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class UpgradeModuleSettingsAction(Of T)
        Inherits UpgradeSettingsAction(Of T)
        Implements IUpgradeModuleSettingsAction


        Protected Overrides ReadOnly Property Scope As SettingsScope
            Get
                Return SettingsScope.ModuleSettings
            End Get
        End Property
    End Class
End Namespace