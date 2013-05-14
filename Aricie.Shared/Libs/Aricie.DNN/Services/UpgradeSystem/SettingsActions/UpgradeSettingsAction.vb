Imports System.Xml.Linq
Imports Aricie.DNN.Settings
Imports Aricie.DNN.Services.UpgradeSystem.Actions.Settings.Configuration

Namespace Services.UpgradeSystem.Actions.Settings

    ''' <summary>
    ''' Classe de base de mise à jour des paramètres
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class UpgradeSettingsAction(Of T)
        Inherits AbstractUpgradeAction
        Implements IUpgradeSettingsAction

        Protected MustOverride ReadOnly Property Scope As SettingsScope

        ''' <summary>
        ''' La fonction de mise à jour des paramètres: passe la version cible et le document xml des paramètres. Il faut renvoyer le document xml des paramètres 
        ''' modifiés pour correspondre au nouveau chargement via déserialisation
        ''' </summary>
        ''' <value></value>
        ''' <returns>XDocument représentant la nouvelle structure des paramètres </returns>
        ''' <remarks></remarks>
        Public Property UpgradeOperation As Func(Of Version, XDocument, ScopeInformation, XDocument)


        Protected Friend Sub Upgrade(ByVal TargetVersion As Version, ByVal scopeId As Integer, ByVal configRepo As IConfigurationRepository) Implements IUpgradeSettingsAction.UpgradeTo
            Dim ClassKey As String = Aricie.Constants.GetKey(Of T)()
            Dim Document = configRepo.RetrieveConfig(Scope, scopeId, ClassKey) '
            Document = UpgradeOperation(TargetVersion, Document, New ScopeInformation(Scope, scopeId))
            configRepo.StoreConfig(Scope, scopeId, ClassKey, Document)
        End Sub


    End Class
End Namespace