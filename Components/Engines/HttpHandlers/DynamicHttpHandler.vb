Imports Aricie.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper

   
    Public Class DynamicHttpHandler
        Implements IHttpHandler

        Private _Settings As HttpHandlerSettings

        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

            Dim handlersConfig As HttpHandlersConfig = PortalKeeperConfig.Instance.HttpHandlers
            If Not handlersConfig.Enabled Then
                Throw New HttpException(404, "Dynamic Http Handlers are disabled in Portal Keeper Configuration")
            End If
            Dim mappedHandler As HttpHandlerSettings = handlersConfig.MapDynamicHandler(context)
            If mappedHandler Is Nothing Then
                Throw New HttpException(404, "Dynamic Http Handler not found in Portal Keeper Configuration")
            End If
            If Not mappedHandler.DynamicHandler.Enabled Then
                Throw New HttpException(403, "Dynamic Http Handler was disabled in Portal Keeper Configuration")
            End If
            Dim keeperContext As PortalKeeperContext(Of SimpleEngineEvent) = PortalKeeperContext(Of SimpleEngineEvent).Instance(HttpContext.Current)
            mappedHandler.ProcessRequest(keeperContext)


        End Sub
    End Class
End Namespace