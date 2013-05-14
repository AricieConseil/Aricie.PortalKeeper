Imports System.Linq
Imports Aricie.DNN.Settings
Imports System.Xml.Linq
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Services.UpgradeSystem.Actions.Settings.Configuration
Imports Aricie.DNN.Services.UpgradeSystem.Actions

Namespace Services.UpgradeSystem
    ''' <summary>
    ''' Moteur de mise à jour d'une classe (forcément le controller d'un module IUpgradeable)
    ''' </summary>
    ''' <typeparam name="T">Le type de la classe que l'on souhaite mettre à jour</typeparam>
    ''' <remarks></remarks>
    Public Class UpgradeEngine(Of T As IUpgradeable)

        Private UpgradedModuleName As String

        ''' <summary>
        ''' </summary>
        ''' <param name="ForModule">Nom du module optionnel pour information</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ForModule As String)
            UpgradedModuleName = ForModule
        End Sub

        ''' <summary>
        ''' Recherche et ajoute automatiquement au moteur toutes les classes de type IUpgradeActionProvider(of T) contenues dans l'assembly appellante
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResolveAndAddUpgrades()
            Dim EngineUpgrades = (From t In Reflection.Assembly.GetCallingAssembly().GetTypes() Where T.GetInterfaces().Contains(GetType(IUpgradeActionsProvider(Of T))) AndAlso T.GetConstructor(Type.EmptyTypes) IsNot Nothing Select Activator.CreateInstance(T)).Cast(Of IUpgradeActionsProvider(Of T))().ToList()
            EngineUpgrades.ForEach(Sub(provider) AddUpgradeActionsCollectionFromProvider(provider))
        End Sub

        Private Property UpgradeActions As New List(Of UpgradeActionsCollection)

        Public Sub AddUpgradeActionsCollectionFromProvider(ByVal mr As IUpgradeActionsProvider(Of T))
            UpgradeActions.Add(mr.GetActions())
        End Sub

        ''' <summary>
        ''' Permet de faire migrer le moteur automatiquement vers une version données. Le moteur cherche la version précédente à cette version donnée et exécute les opérations
        ''' correspondant à des versions intermédiaires. A utiliser avec le moteur d'upgrade DNN pour les upgrades incrémentales.
        ''' </summary>
        ''' <param name="to">Le numéro de version de destination. Attention, ce numéro doit avoir des règles de migration définies.</param>
        ''' <remarks></remarks>
        Public Sub MigrateSettingsTo(ByVal [to] As Version)

            If (Not UpgradeActions.Where(Function(ua) ua.TargetVersion = [to]).Any()) Then
                ' s'il n'existe aucune action pour arriver à cette version, on renvoie une exception
                Throw New InvalidOperationException("No upgrade operation exists for version " & [to].ToString() & " .  Aborting upgrade path to this version")
            End If

            ' récupérons la version suivante
            Dim PreviousVersion = UpgradeActions.Select(Function(r) r.TargetVersion).Distinct().Where(Function(v) v < [to]).OrderBy(Function(v) v).LastOrDefault()
            If (PreviousVersion Is Nothing) Then
                ' on considère qu'on migre à partir de la version 0
                PreviousVersion = New Version("0.0.0")
            End If
            MigrateSettings(PreviousVersion, [to])
        End Sub

        

        ''' <summary>
        ''' Effectue la mise à jour entre deux versions en effectuant toutes les opérations comprises entre ces deux actions.
        ''' </summary>
        ''' <param name="from">Version initiale</param>
        ''' <param name="to">Version visée</param>
        ''' <remarks>Les opérations sélectionnées n'incluent pas celles de la version initiale, mais incluent celle de la version visée</remarks>
        Public Sub MigrateSettings(ByVal from As Version, ByVal [to] As Version)
            Try
                Dim configRepo As IConfigurationRepository = New ConfigurationRepository()
                Dim ApplicableUpgradeActions = UpgradeActions.Where(Function(r) r.TargetVersion > [from] AndAlso r.TargetVersion <= [to]).OrderBy(Function(r) r.TargetVersion)
                Dim UAD As New UpgradeActionsDelayer(ApplicableUpgradeActions, UpgradedModuleName)

                UAD.RunPreMigrationActions()
                For Each rc As UpgradeActionsCollection In ApplicableUpgradeActions
                    UAD.RunPreUpgradeActions(rc.TargetVersion)

                    For Each hsa In rc.UpgradeHostSettingsActions
                        hsa.UpgradeTo(rc.TargetVersion, -1, configRepo) ' les host settings se moquent de l'identifiant
                    Next

                    For Each portal As PortalInfo In New PortalController().GetPortals()
                        UAD.RunPrePortalUpgradeActions(rc.TargetVersion, portal.PortalID)

                        For Each action In rc.UpgradePortalSettingsActions
                            action.UpgradeTo(rc.TargetVersion, portal.PortalID, configRepo)
                        Next

                        For Each mi As ModuleInfo In New ModuleController().GetModulesByDefinition(portal.PortalID, UpgradedModuleName)
                            UAD.RunPreModuleUpgradeActions(rc.TargetVersion, portal.PortalID, mi.ModuleID)

                            For Each action In rc.UpgradeModuleSettingsActions
                                action.UpgradeTo(rc.TargetVersion, mi.ModuleID, configRepo)
                            Next

                            UAD.RunPostModuleUpgradeActions(rc.TargetVersion, portal.PortalID, mi.ModuleID)
                        Next
                        UAD.RunPostPortalUpgradeActions(rc.TargetVersion, portal.PortalID)
                    Next
                    UAD.RunPostUpgradeActions(rc.TargetVersion)
                Next
                UAD.RunPostMigrationActions()

                configRepo.CommitConfigChanges()
            Catch ex As Exception
                ' pour l'instant je la relance bêtement
                Throw
            End Try
        End Sub


        ''' <summary>
        ''' Permet de stocker les actions qui doivent être lancées regroupées à un moment donné du processus de mise à jour
        ''' </summary>
        ''' <remarks></remarks>
        Private Class UpgradeActionsDelayer

            Private CurrentApplicableUpgradeActions As IEnumerable(Of UpgradeActionsCollection)
            Private GlobalActions As IEnumerable(Of Actions.GlobalUpgradeAction)
            Private PortalActions As IEnumerable(Of Actions.PortalUpgradeAction)
            Private ModuleActions As IEnumerable(Of Actions.ModuleUpgradeAction)
            Private MinVersion As Version
            Private MaxVersion As Version
            Private UpgradedModuleName As String

            Public Sub New(ByVal ApplicableUpgradeActions As IEnumerable(Of UpgradeActionsCollection), ByVal ModuleName As String)
                CurrentApplicableUpgradeActions = ApplicableUpgradeActions

                MinVersion = ApplicableUpgradeActions.Min(Function(uac) uac.TargetVersion)
                MaxVersion = ApplicableUpgradeActions.Max(Function(uac) uac.TargetVersion)

                UpgradedModuleName = ModuleName
            End Sub

            Private Sub SelectActions()
                ActionsSelection(CurrentApplicableUpgradeActions)
            End Sub

            Private Sub SelectActions(ByVal target As Version)
                ActionsSelection(CurrentApplicableUpgradeActions.Where(Function(uac) uac.TargetVersion = target))
            End Sub

            Private Sub ActionsSelection(ByVal UACs As IEnumerable(Of UpgradeActionsCollection))
                GlobalActions = UACs.SelectMany(Function(uac) uac.GlobalUpgradeActions)
                PortalActions = UACs.SelectMany(Function(uac) uac.PortalUpgradeActions)
                ModuleActions = UACs.SelectMany(Function(uac) uac.ModuleUpgradeActions)
            End Sub

            Public Sub RunPreMigrationActions()
                SelectActions()
                RunGlobaAndDescendantsActions(ActionStep.MigrationStarting, MinVersion)
            End Sub

            Public Sub RunPostMigrationActions()
                SelectActions()
                RunGlobaAndDescendantsActions(ActionStep.MigrationEnded, MaxVersion)
            End Sub

            Public Sub RunPreUpgradeActions(ByVal Target As Version)
                SelectActions(Target)
                RunGlobaAndDescendantsActions(ActionStep.VersionUpgradeStarting, Target)
            End Sub

            Public Sub RunPostUpgradeActions(ByVal Target As Version)
                SelectActions(Target)
                RunGlobaAndDescendantsActions(ActionStep.VersionUpgradeEnded, Target)
            End Sub

            Public Sub RunPrePortalUpgradeActions(ByVal Target As Version, ByVal PortalId As Integer)
                SelectActions(Target)
                RunPortalAndDescendantsActions(ActionStep.PortalUpgradeStarting, Target, PortalId)
            End Sub

            Public Sub RunPostPortalUpgradeActions(ByVal Target As Version, ByVal PortalId As Integer)
                SelectActions(Target)
                RunPortalAndDescendantsActions(ActionStep.PortalUpgradeEnded, Target, PortalId)
            End Sub

            Public Sub RunPreModuleUpgradeActions(ByVal Target As Version, ByVal PortalId As Integer, ByVal ModuleId As Integer)
                SelectActions(Target)
                RunModuleActions(ActionStep.ModuleUpgradeStarting, Target, PortalId, ModuleId)
            End Sub

            Public Sub RunPostModuleUpgradeActions(ByVal Target As Version, ByVal PortalId As Integer, ByVal ModuleId As Integer)
                SelectActions(Target)
                RunModuleActions(ActionStep.ModuleUpgradeEnded, Target, PortalId, ModuleId)
            End Sub

            Private Sub RunGlobaAndDescendantsActions(ByVal CurrentStep As ActionStep, ByVal TargetVersion As Version)
                For Each ga In GlobalActions.Where(Function(g) g.ActivatingActionStep = CurrentStep)
                    ga.UpgradeTo(TargetVersion)
                Next

                For Each pi As PortalInfo In New PortalController().GetPortals()
                    RunPortalAndDescendantsActions(CurrentStep, TargetVersion, pi.PortalID)
                Next
            End Sub

            Private Sub RunPortalAndDescendantsActions(ByVal CurrentStep As ActionStep, ByVal TargetVersion As Version, ByVal portalId As Integer)
                For Each pa In PortalActions.Where(Function(p) p.ActivatingActionStep = CurrentStep)
                    pa.UpgradeTo(TargetVersion, portalId)
                Next

                For Each mi As ModuleInfo In New ModuleController().GetModulesByDefinition(portalId, UpgradedModuleName)
                    RunModuleActions(CurrentStep, TargetVersion, portalId, mi.ModuleID)
                Next
            End Sub

            Private Sub RunModuleActions(ByVal CurrentStep As ActionStep, ByVal TargetVersion As Version, ByVal portalId As Integer, ByVal moduleId As Integer)
                For Each ma In ModuleActions.Where(Function(m) m.ActivatingActionStep = CurrentStep)
                    ma.UpgradeTo(TargetVersion, portalId, moduleId)
                Next
            End Sub
        End Class


    End Class
End Namespace