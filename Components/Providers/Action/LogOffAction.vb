Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.SignOut, IconOptions.Normal)> _
    <Serializable()> _
        <System.ComponentModel.DisplayName("Log Off Action")> _
        <Description("Signs the user out of his current session")> _
    Public Class LogOffAction
        Inherits ActionProvider(Of RequestEvent)



        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            Dim ps As New DotNetNuke.Security.PortalSecurity
            ps.SignOut()
            Return True
        End Function


    End Class
End Namespace