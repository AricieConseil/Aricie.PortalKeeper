Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Services

Namespace UI.WebControls
    Public Class DesktopModuleSelector
        Inherits SelectorControl(Of DesktopModuleInfo)


        Public Overrides Function GetEntitiesG() As IList(Of DesktopModuleInfo)

            Return New List(Of DesktopModuleInfo)( _
                DirectCast( _
                    NukeHelper.DesktopModuleController.GetDesktopModulesByPortal(PortalSettings.PortalId).ToArray(GetType(DesktopModuleInfo)) _
                , DesktopModuleInfo()))
        End Function
    End Class
End Namespace
