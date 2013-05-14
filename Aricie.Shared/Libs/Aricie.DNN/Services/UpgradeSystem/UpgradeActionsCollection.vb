Imports Aricie.DNN.Services.UpgradeSystem.Actions
Imports Aricie.DNN.Services.UpgradeSystem.Actions.Settings

Namespace Services.UpgradeSystem

    ''' <summary>
    ''' La classe qui renvoie la collection d'action à effectuer pour atteindre un numéro de version spécifique sur un module
    ''' </summary>
    ''' <remarks></remarks>
    Public Class UpgradeActionsCollection

        ''' <summary>
        ''' </summary>
        ''' <param name="DesiredTarget">La version que permet d'atteindre cette collection d'actions</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal DesiredTarget As Version)
            Target = DesiredTarget
        End Sub

        Private Target As Version
        Public ReadOnly Property TargetVersion As Version
            Get
                Return Target
            End Get
        End Property

        Public Property GlobalUpgradeActions As New List(Of GlobalUpgradeAction)
        Public Property PortalUpgradeActions As New List(Of PortalUpgradeAction)
        Public Property ModuleUpgradeActions As New List(Of ModuleUpgradeAction)

        Public Property UpgradeHostSettingsActions As New List(Of IUpgradeHostSettingsAction)
        Public Property UpgradePortalSettingsActions As New List(Of IUpgradePortalSettingsAction)
        Public Property UpgradeModuleSettingsActions As New List(Of IUpgradeModuleSettingsAction)
    End Class
End Namespace