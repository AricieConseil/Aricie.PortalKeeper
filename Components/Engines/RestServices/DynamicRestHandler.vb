Imports OpenRasta.Web

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DynamicRestHandler

        Private _Context As ICommunicationContext
        Private _ServiceConfiguration As RestService


        Public Sub New(context As ICommunicationContext)
            _Context = context
            Dim resourcekey As Object = _Context.PipelineData.ResourceKey
            _ServiceConfiguration = PortalKeeperConfig.Instance.RestServices.FindServiceByKey(resourcekey)
            If _ServiceConfiguration Is Nothing Then
                Throw New ApplicationException("Rest Service Configuration not found")
            End If
        End Sub

        Public Function Head() As OperationResult
            Return ProcessRequest(HttpMethod.HEAD)
        End Function

        Public Function [Get]() As OperationResult
            Return ProcessRequest(HttpMethod.GET)
        End Function

        Public Function Post() As OperationResult
            Return ProcessRequest(HttpMethod.POST)
        End Function

        Public Function Put() As OperationResult
            Return ProcessRequest(HttpMethod.PUT)
        End Function

        Public Function Delete() As OperationResult
            Return ProcessRequest(HttpMethod.DELETE)
        End Function

        Public Function Options() As OperationResult
            Return ProcessRequest(HttpMethod.OPTIONS)
        End Function

        Private Function ProcessRequest(method As HttpMethod) As OperationResult
            Dim toReturn As OperationResult = New OperationResult.OK()
            Try


            Catch ex As Exception
                toReturn = New OperationResult.InternalServerError()
            End Try
            Return toReturn
        End Function


    End Class
End NameSpace