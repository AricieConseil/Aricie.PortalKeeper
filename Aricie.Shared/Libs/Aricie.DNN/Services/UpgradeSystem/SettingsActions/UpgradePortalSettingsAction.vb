Imports System.Xml.Linq
Imports Aricie.DNN.Settings

Namespace Services.UpgradeSystem.Actions.Settings

    ''' <summary>
    ''' Implémentation de la classe de mise à jour des paramètres de portail
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class UpgradePortalSettingsAction(Of T)
        Inherits UpgradeSettingsAction(Of T)
        Implements IUpgradePortalSettingsAction



        Protected Overrides ReadOnly Property Scope As SettingsScope
            Get
                Return SettingsScope.PortalSettings
            End Get
        End Property


    End Class
End Namespace