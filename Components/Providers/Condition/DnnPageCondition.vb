Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <Serializable()> _
        <System.ComponentModel.DisplayName("DotNetNuke Page Condition")> _
        <Description("Matches according to the target DNN Page as defined in the url")> _
    Public Class DnnPageCondition
        Inherits SelectionSetCondition


        Public Overrides Function GetCurrentValue(ByVal context As PortalKeeperContext(Of RequestEvent)) As Integer
            Dim currentPage As DotNetNuke.Entities.Tabs.TabInfo = context.DnnContext.Portal.ActiveTab
            If currentPage IsNot Nothing Then
                Return currentPage.TabID
            End If
            Return -1
        End Function

        Public Overrides Function GetSelectorAttribute() As System.Attribute

            Return New SelectorAttribute(GetType(TabSelector), "TabPath", "TabID", True, False, "", "", False, False)
        End Function
    End Class
End Namespace