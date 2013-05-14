Namespace Services.UpgradeSystem.Actions

    ''' <summary>
    ''' Action de mise à jour des portails qui sera lancée pour chaque portail
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PortalUpgradeAction
        Inherits StepUpgradeAction

        ''' <summary>
        ''' l'étape ou doit se dérouler cette action de mise à jour de portail
        ''' </summary>
        ''' <param name="ExecutingStep">Une valeur de [ActionStep]</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ExecutingStep As ActionStep)
            MyBase.New(ExecutingStep)
        End Sub

        ''' <summary>
        ''' L'opération devant être lancée pour chaque portail: elle reçoit la version visée et l'identifiant du portail 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UpgradeOperation As Action(Of Version, Integer)

        Protected Friend Sub UpgradeTo(ByVal TargetVersion As Version, ByVal portalId As Integer)
            If UpgradeOperation IsNot Nothing Then
                UpgradeOperation.Invoke(TargetVersion, portalId)
            End If
        End Sub


    End Class

End Namespace