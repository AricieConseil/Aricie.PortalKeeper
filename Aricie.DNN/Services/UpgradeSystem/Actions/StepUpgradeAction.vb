Namespace Services.UpgradeSystem.Actions

    ''' <summary>
    ''' Classe abstraite pour les actions exécutables sur une étape
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class StepUpgradeAction
        Inherits AbstractUpgradeAction

        Private _ActivatingActionStep As ActionStep
        Public ReadOnly Property ActivatingActionStep() As ActionStep
            Get
                Return _ActivatingActionStep
            End Get
        End Property


        Public Sub New(ByVal ExecutionStep As ActionStep)
            _ActivatingActionStep = ExecutionStep
        End Sub



    End Class

End Namespace