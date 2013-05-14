Namespace Services.UpgradeSystem

    ''' <summary>
    ''' Interface de récupération des actions de mise à jour
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Interface IUpgradeActionsProvider(Of T)
        ''' <summary>
        ''' Renvoie la collection d'actions à effectuer pour mise à jour
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetActions() As UpgradeActionsCollection
    End Interface
End Namespace