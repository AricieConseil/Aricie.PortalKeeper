Imports DotNetNuke.Entities.Tabs
Imports Aricie.DNN.Services

Namespace UI.WebControls
    Public Class TabSelector
        Inherits SelectorControl(Of TabInfo)


        Public Overrides Function GetEntitiesG() As IList(Of TabInfo)
            Dim controller As New TabController
            Return New List(Of TabInfo)(DirectCast(controller.GetTabs(PortalId).ToArray(GetType(TabInfo)), TabInfo()))
        End Function
    End Class
End Namespace
