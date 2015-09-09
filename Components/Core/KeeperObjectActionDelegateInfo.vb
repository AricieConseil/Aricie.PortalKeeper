Imports System.Reflection

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class KeeperObjectActionDelegateInfo(Of TEngineEvents As IConvertible)

        Public Sub New()

        End Sub

        Public Sub New(objActions As KeeperObjectAction(Of TEngineEvents))
            Me.KeeperObjectAction = objActions
        End Sub

        Public Sub New(objActions As KeeperObjectAction(Of TEngineEvents), objContext As PortalKeeperContext(Of TEngineEvents))
            Me.New(objActions)
            ActionContext = objContext
        End Sub

        Public Property EventParameters As ParameterInfo()

        Public Property KeeperObjectAction As KeeperObjectAction(Of TEngineEvents)

        Public Property ActionContext As PortalKeeperContext(Of TEngineEvents)

        'ParamArray objParams() As Object
        Public Sub RunHandler(sender As Object, e As EventArgs)
            Dim tempEvent = ActionContext.CurrentEventStep
            If KeeperObjectAction.SetHandlerEvent.Enabled Then
                ActionContext.CurrentEventStep = KeeperObjectAction.SetHandlerEvent.Entity
            End If
            If KeeperObjectAction.PassArguments Then
                ActionContext.SetVar(KeeperObjectAction.CandidateEventParams(0).Name, sender)
                Dim objParamName As String = KeeperObjectAction.CandidateEventParams(1).Name
                If objParamName = "e" Then
                    objParamName = DynamicControlAdapter.EventArgsVarName
                End If
                ActionContext.SetVar(objParamName, e)
            End If
            Me.KeeperObjectAction.KeeperAction.Run(ActionContext)
            If KeeperObjectAction.SetHandlerEvent.Enabled Then
                ActionContext.CurrentEventStep = tempEvent
            End If
        End Sub

    End Class
End NameSpace