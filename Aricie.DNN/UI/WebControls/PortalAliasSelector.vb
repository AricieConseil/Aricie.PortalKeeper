Imports DotNetNuke.Entities.Portals
Imports Aricie.DNN.Services

Namespace UI.WebControls

    Public Class PortalAliasSelector
        Inherits SelectorControl(Of PortalAliasInfo)



        Public Overrides Function GetEntitiesG() As IList(Of PortalAliasInfo)
            Dim pac As New PortalAliasController
            Dim toReturn As New List(Of PortalAliasInfo)
            'Jesse: je rétabli le sélecteur cross-portail (on en a besoin)
            'cf CurrentPortalAliasSelector pour un filtrage sur le portail courant
          'Return NukeHelper.PortalAliasesByPortalId(NukeHelper.PortalId)
            Dim collection As PortalAliasCollection = pac.GetPortalAliases()
            For Each item As DictionaryEntry In collection
                toReturn.Add(CType(item.Value, PortalAliasInfo))
            Next

            Return toReturn

        End Function

    End Class


    Public Class CurrentPortalAliasSelector
        Inherits PortalAliasSelector

        Public Overrides Function GetEntitiesG() As System.Collections.Generic.IList(Of DotNetNuke.Entities.Portals.PortalAliasInfo)
            Return New List(Of PortalAliasInfo)(MyBase.GetEntitiesG().Where(Function(portalAlias As PortalAliasInfo) portalAlias.PortalID = NukeHelper.PortalId))
        End Function

    End Class
End Namespace
