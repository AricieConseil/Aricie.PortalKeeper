Namespace Services.UpgradeSystem.Actions

    ''' <summary>
    ''' Action de mise à jour globale sur l'ensemble du site, ne sera lancée qu'une fois
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GlobalUpgradeAction
        Inherits StepUpgradeAction

        ''' <summary>
        ''' l'étape ou doit se dérouler cette action globale
        ''' </summary>
        ''' <param name="ExecutingStep">Une valeur de [ActionStep]</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ExecutingStep As ActionStep)
            MyBase.New(ExecutingStep)
        End Sub

        ''' <summary>
        ''' L'opération devant être lancée globalement sur le site: elle reçoit la version visée
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UpgradeOperation As Action(Of Version)

        Protected Friend Sub UpgradeTo(ByVal TargetVersion As Version)
            If UpgradeOperation IsNot Nothing Then
                UpgradeOperation.Invoke(TargetVersion)
            End If
        End Sub

    End Class

End Namespace