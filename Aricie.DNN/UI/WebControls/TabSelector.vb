Imports DotNetNuke.Entities.Tabs
Imports Aricie.DNN.Services

Namespace UI.WebControls
    Public Class TabSelector
        Inherits SelectorControl(Of TabInfo)


        Public Overrides Function GetEntitiesG() As IList(Of TabInfo)

            Dim toReturn As New List(Of TabInfo)(DirectCast(NukeHelper.TabController.GetTabs(PortalId).ToArray(GetType(TabInfo)), TabInfo()))
            If NukeHelper.User IsNot Nothing AndAlso NukeHelper.User.IsSuperUser Then
                toReturn.AddRange(DirectCast(NukeHelper.TabController.GetTabs(-1).ToArray(GetType(TabInfo)), TabInfo()))
            End If
            Return toReturn
        End Function
    End Class
End Namespace
