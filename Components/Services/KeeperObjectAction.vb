Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum EventHandlerType
        DelegateExpression
        KeeperAction
    End Enum


    <Serializable()> _
    Public Class KeeperObjectAction(Of TEngineEvents As IConvertible)
        Inherits GeneralObjectAction

        <ConditionalVisible("HasType", False, True)> _
       <ConditionalVisible("ActionMode", False, True, ObjectActionMode.AddEventHandler)> _
        Public Property EventHandlerType As EventHandlerType


        <ConditionalVisible("HasType", False, True)> _
       <ConditionalVisible("ActionMode", False, True, ObjectActionMode.AddEventHandler)> _
       <ConditionalVisible("EventHandlerType", False, True, EventHandlerType.DelegateExpression)> _
        Public Overrides Property DelegateExpression As FleeExpressionInfo(Of [Delegate])
            Get
                Return MyBase.DelegateExpression
            End Get
            Set(value As FleeExpressionInfo(Of [Delegate]))
                MyBase.DelegateExpression = value
            End Set
        End Property

        <ConditionalVisible("HasType", False, True)> _
      <ConditionalVisible("ActionMode", False, True, ObjectActionMode.AddEventHandler)> _
      <ConditionalVisible("EventHandlerType", False, True, EventHandlerType.KeeperAction)> _
        Public Property KeeperAction As New KeeperAction(Of TEngineEvents)

        Protected Overrides Sub AddEventHandler(owner As Object, globalVars As Services.IContextLookup, target As Object, candidateEvent As Reflection.EventInfo)
            If Me.EventHandlerType = PortalKeeper.EventHandlerType.DelegateExpression Then
                MyBase.AddEventHandler(owner, globalVars, target, candidateEvent)
            Else
                ReflectionHelper.AddEventHandler(candidateEvent, target, Sub()
                                                                             Me.KeeperAction.Run(DirectCast(owner, PortalKeeperContext(Of TEngineEvents)))
                                                                         End Sub)
            End If

        End Sub

    End Class

End Namespace


