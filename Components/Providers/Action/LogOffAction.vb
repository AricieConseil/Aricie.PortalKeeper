Imports System.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper

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