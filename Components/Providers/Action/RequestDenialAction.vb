Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum RequestDenialType
        Abort = 0
        Send401 = 401
        Send403 = 403
        Send404 = 404
        Send418 = 418
        Send429 = 429
        Send503 = 503
    End Enum

    Public Enum RequestAbortType
        CloseResponse
        FlushAndClose
        EndResponse
    End Enum

    Public Enum RequestDenialMode
        ThrowHttpException
        SendStatus
    End Enum

    <ActionButton(IconName.MinusCircle, IconOptions.Normal)> _
   <Serializable()> _
       <DisplayName("Request Denial Action")> _
       <Description("Blocks the request. A status code can be sent or the client connection can be simply closed")> _
    Public Class RequestDenialAction
        Inherits ActionProvider(Of RequestEvent)



        Private _RequestDenialActionType As RequestDenialType = RequestDenialType.Send403
        Private _RequestAbortType As RequestAbortType = RequestAbortType.CloseResponse
        Private _RequestDenialActionMode As RequestDenialMode = RequestDenialMode.ThrowHttpException
        Private _ExceptionMessage As String = ""
        Private _RewriteContent As String = String.Empty

        <ExtendedCategory("Specifics")> _
        Public Property RequestDenialActionType() As RequestDenialType
            Get
                Return _RequestDenialActionType
            End Get
            Set(ByVal value As RequestDenialType)
                _RequestDenialActionType = value
            End Set
        End Property


        <ConditionalVisible("RequestDenialActionType", False, True, RequestDenialType.Abort)> _
        <ExtendedCategory("Specifics")> _
        Public Property RequestAbortType() As RequestAbortType
            Get
                Return _RequestAbortType
            End Get
            Set(ByVal value As RequestAbortType)
                _RequestAbortType = value
            End Set
        End Property

        <ConditionalVisible("RequestDenialActionType", True, True, RequestDenialType.Abort)> _
        <ExtendedCategory("Specifics")> _
        Public Property RequestDenialActionMode() As RequestDenialMode
            Get
                Return _RequestDenialActionMode
            End Get
            Set(ByVal value As RequestDenialMode)
                _RequestDenialActionMode = value
            End Set
        End Property

        <ConditionalVisible("RequestDenialActionType", True, True, RequestDenialType.Abort)> _
        <ConditionalVisible("RequestDenialActionMode", False, True, RequestDenialMode.ThrowHttpException)> _
        <ExtendedCategory("Specifics")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(5)> _
            <Width(200)> _
        Public Property ExceptionMessage() As String
            Get
                Return _ExceptionMessage
            End Get
            Set(ByVal value As String)
                _ExceptionMessage = value
            End Set
        End Property

        <ConditionalVisible("RequestDenialActionType", True, True, RequestDenialType.Abort)> _
        <ConditionalVisible("RequestDenialActionMode", False, True, RequestDenialMode.SendStatus)> _
        <ExtendedCategory("Specifics")> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(20)> _
            <Width(300)> _
        Public Property RewriteContent() As String
            Get
                Return _RewriteContent
            End Get
            Set(ByVal value As String)
                _RewriteContent = value
            End Set
        End Property



        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Select Case Me._RequestDenialActionType
                Case RequestDenialType.Abort
                    Select Case Me._RequestAbortType
                        Case RequestAbortType.CloseResponse
                            actionContext.DnnContext.Response.Close()
                            actionContext.DnnContext.HttpContext.ApplicationInstance.CompleteRequest()
                        Case RequestAbortType.EndResponse
                            'actionContext.DnnContext.Response.End()
                            actionContext.DnnContext.HttpContext.ApplicationInstance.CompleteRequest()
                        Case RequestAbortType.FlushAndClose
                            actionContext.DnnContext.Response.Flush()
                            actionContext.DnnContext.HttpContext.ApplicationInstance.CompleteRequest()
                    End Select
                Case RequestDenialType.Send401, RequestDenialType.Send404, RequestDenialType.Send403
                    Dim statusCode As Integer = CInt(Me._RequestDenialActionType)
                    Select Case Me._RequestDenialActionMode
                        Case RequestDenialMode.SendStatus
                            actionContext.DnnContext.Response.StatusCode = statusCode
                            Dim toWrite As String = Me.GetAdvancedTokenReplace(actionContext).ReplaceAllTokens(Me._RewriteContent)
                            actionContext.DnnContext.Response.Write(toWrite)
                            'actionContext.DnnContext.Response.End()
                        Case RequestDenialMode.ThrowHttpException
                            Dim message As String = ""
                            If Not String.IsNullOrEmpty(Me._ExceptionMessage) Then
                                message = Me.GetAdvancedTokenReplace(actionContext).ReplaceAllTokens(Me._ExceptionMessage)
                            End If
                            Throw New HttpException(statusCode, message)
                    End Select
            End Select
            Return True
        End Function




    End Class
End Namespace