Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports System.Net
Imports System.ComponentModel
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
            Dim toReturn As OperationResult
            Try


            Catch ex As Exception
                toReturn = New OperationResult.InternalServerError()
            End Try
            Return toReturn
        End Function


    End Class


    Public Enum RestHandlerType
        CustomHandler
        DynamicHandler
    End Enum

    <Serializable()> _
    Public Class RestService
        Inherits NamedConfig



        Public Property ResourceType As New DotNetType

        Public Property AtUri As String = "/MyResource"

        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List, EnableExport:=False)> _
        Public Property AlternateUris As New List(Of String)

        Public Property RestHandlerType As RestHandlerType

        <ConditionalVisible("RestHandlerType", False, True, RestHandlerType.CustomHandler)> _
        Public Property HandlerType As New DotNetType

        <ConditionalVisible("RestHandlerType", False, True, RestHandlerType.DynamicHandler)> _
        Public Property DynamicMethods() As New List(Of DynamicRestMethod)


        Public Property AsJsonDataContract As Boolean = True

        Public Property AsXmlDataContract As Boolean = True

        Public Property AsXmlSerializer As Boolean = True


    End Class

    <Serializable()> _
    Public Class DynamicRestMethod
        Inherits RuleEngineSettings(Of RestEngineEvent)

        <CollectionEditor(False, True, False, False, 0, CollectionDisplayStyle.List, False, 5, "", True)> _
        Public Property HttpVerbs() As New List(Of WebMethod)


    End Class



    <Serializable()> _
     <System.ComponentModel.DisplayName("Return Rest Operation Action")> _
      <Description("Ends a rest web service dynamic method with a status and an optional result resource")> _
    Public Class ReturnRestOperationAction
        Inherits CacheableAction(Of RestEngineEvent)

        <Browsable(False)> _
        Public Overrides Property ExitAction() As Boolean
            Get
                Return True
            End Get
            Set(value As Boolean)
                'do nothing
            End Set
        End Property


        Public Property OperationResultType() As OperationResultType = OperationResultType.Ok

        Public Property OperationResultExpression() As New SimpleExpression(Of Object)


        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of RestEngineEvent), async As Boolean) As Object
            Return OperationResultExpression.Evaluate(actionContext, actionContext)
        End Function


        Public Overrides Function RunFromObject(actionContext As PortalKeeperContext(Of RestEngineEvent), async As Boolean, cachedObject As Object) As Boolean

            Dim operationResult As Object = cachedObject
            Dim toReturn As OperationResult
            Select Case Me.OperationResultType
                Case OperationResultType.Ok
                    toReturn = New OperationResult.OK
                Case OperationResultType.BadRequest
                    toReturn = New OperationResult.BadRequest
                Case OperationResultType.Created
                    toReturn = New OperationResult.Created
                Case OperationResultType.Forbidden
                    toReturn = New OperationResult.Forbidden
                Case OperationResultType.Found
                    toReturn = New OperationResult.Found
                Case OperationResultType.Gone
                    Dim openRastaContext As ICommunicationContext = actionContext.GetItem(Of ICommunicationContext)()
                    If openRastaContext IsNot Nothing Then
                        toReturn = New OperationResult.Gone(openRastaContext)
                    Else
                        Throw New ApplicationException("No Communication Context available to run ReturnOperationAction " & Me.Name)
                    End If
                Case OperationResultType.MethodNotAllowed
                    toReturn = New OperationResult.MethodNotAllowed
                Case OperationResultType.MovedPermanently
                    toReturn = New OperationResult.MovedPermanently
                Case OperationResultType.MovedTemporarily
                    toReturn = New OperationResult.MovedTemporarily
                Case OperationResultType.MultipleRepresentations
                    toReturn = New OperationResult.MultipleRepresentations
                Case OperationResultType.NoContent
                    toReturn = New OperationResult.NoContent
                Case OperationResultType.NotFound
                    toReturn = New OperationResult.NotFound
                Case OperationResultType.NotModified
                    toReturn = New OperationResult.NotModified
                Case OperationResultType.RequestMediaTypeUnsupported
                    toReturn = New OperationResult.RequestMediaTypeUnsupported
                Case OperationResultType.ResponseMediaTypeUnsupported
                    toReturn = New OperationResult.ResponseMediaTypeUnsupported
                Case OperationResultType.SeeOther
                    toReturn = New OperationResult.SeeOther
                Case OperationResultType.Unauthorized
                    toReturn = New OperationResult.Unauthorized
                Case Else
                    toReturn = New OperationResult.InternalServerError
            End Select
            toReturn.ResponseResource = cachedObject
            actionContext.SetItem(Of OperationResult)(toReturn)
            Return True

        End Function
    End Class

    Public Enum OperationResultType
        Ok
        BadRequest
        Created
        Forbidden
        Found
        Gone
        SeeOther
        Unauthorized
        MovedPermanently
        MovedTemporarily
        InternalServerError
        MethodNotAllowed
        MultipleRepresentations
        NoContent
        NotFound
        NotModified
        RequestMediaTypeUnsupported
        ResponseMediaTypeUnsupported
    End Enum

End Namespace