Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports OpenRasta.Web

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    <DisplayName("Return Rest Operation Action")> _
    <Description("Ends a rest web service dynamic method with a status and an optional result resource")> _
    Public Class ReturnRestOperationAction
        Inherits CacheableAction(Of SimpleEngineEvent)

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


        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of SimpleEngineEvent), async As Boolean) As Object
            Return OperationResultExpression.Evaluate(actionContext, actionContext)
        End Function


        Public Overrides Function RunFromObject(actionContext As PortalKeeperContext(Of SimpleEngineEvent), async As Boolean, cachedObject As Object) As Boolean

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
End NameSpace