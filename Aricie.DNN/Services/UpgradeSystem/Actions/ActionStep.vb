Namespace Services.UpgradeSystem.Actions

    ''' <summary>
    ''' Enumeration des différentes étapes possibles pour l'exécution d'une action de mise à jour
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ActionStep
        MigrationStarting ' action lancée juste avant qu'on commence à itérer sur la liste des étapes de migration

        VersionUpgradeStarting ' action lancée au début d'une étape d'upgrade
        PortalUpgradeStarting ' action lancée au début d'une étape d'upgrade de portail: seules les actions de portail et de modules sont prises en compte
        ModuleUpgradeStarting ' action lancée au début d'une étape d'upgrade de module: seules les actions de modules sont prises en compte
        ModuleUpgradeEnded ' action lancée à la fin d'une étape d'upgrade de module: seules les actions de modules sont prises en compte
        PortalUpgradeEnded ' action lancée à la fin d'une étape d'upgrade de portail: seules les actions de portail et de modules sont prises en compte
        VersionUpgradeEnded ' action lancée à la fin d'une étape d'upgrade

        MigrationEnded ' action lancée après que la liste des étapes de migration ait été passée
    End Enum

End Namespace