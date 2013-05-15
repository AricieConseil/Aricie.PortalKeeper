Imports Aricie.DNN.Services.Errors

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class KeeperErrorsHandler
        Inherits CustomErrorsHandler

        Public Overrides Function GetCustomErrors() As Aricie.DNN.Services.Errors.VirtualCustomErrorsInfo
            Return PortalKeeperContext(Of RequestEvent).Instance.CurrentFirewallConfig.CustomErrorsConfig
        End Function
    End Class
End Namespace


