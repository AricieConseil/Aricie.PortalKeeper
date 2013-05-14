Namespace Services.UpgradeSystem.Actions

    ''' <summary>
    ''' Action de mise à jour des modules qui sera lancée pour chaque instance du module
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ModuleUpgradeAction
        Inherits StepUpgradeAction

        ''' <summary>
        ''' l'étape ou doit se dérouler cette action de mise à jour de module
        ''' </summary>
        ''' <param name="ExecutingStep">Une valeur de [ActionStep]</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ExecutingStep As ActionStep)
            MyBase.New(ExecutingStep)
        End Sub

        ''' <summary>
        ''' L'opération devant être lancée pour chaque module: elle reçoit la version visée, l'identifiant du portail et l'identifiant du module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UpgradeOperation As Action(Of Version, Integer, Integer)

        Protected Friend Sub UpgradeTo(ByVal TargetVersion As Version, ByVal portalId As Integer, ByVal moduleId As Integer)
            If UpgradeOperation IsNot Nothing Then
                UpgradeOperation.Invoke(TargetVersion, portalId, moduleId)
            End If
        End Sub


    End Class

End Namespace