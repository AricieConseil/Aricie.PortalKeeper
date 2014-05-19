Imports DotNetNuke.Entities.Portals
Imports Aricie.DNN.Services

Namespace UI.WebControls
    Public Class PortalSelector
        Inherits SelectorControl(Of PortalInfo)



        Public Overrides Function GetEntitiesG() As IList(Of PortalInfo)
            Return New List(Of PortalInfo)(DirectCast(NukeHelper.PortalController.GetPortals.ToArray(GetType(PortalInfo)), PortalInfo()))
            'Dim pi As New PortalInfo
            'pi.PortalID
            'pi.PortalName
        End Function

    End Class
End NameSpace