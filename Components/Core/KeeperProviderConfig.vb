Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class KeeperProviderConfig(Of TEngineEvents As IConvertible, T As IProvider)
        Inherits ProviderConfig(Of T)


        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal objType As Type)
            MyBase.New(objType)
        End Sub

        Public Sub New(ByVal objType As Type, ByVal minLifeCycleEvent As TEngineEvents, ByVal maxLifeCycleEvent As TEngineEvents, ByVal defaultLifeCycleEvent As TEngineEvents)
            MyBase.New(objType)
            Me._MinTEngineEvents = minLifeCycleEvent
            Me._MaxTEngineEvents = maxLifeCycleEvent
            Me._DefaultTEngineEvents = defaultLifeCycleEvent
        End Sub

        <Browsable(False)> _
        Public ReadOnly Property HasEvent As Boolean
            Get
                Return GetType(TEngineEvents) IsNot GetType(Boolean)
            End Get
        End Property

        <ConditionalVisible("HasEvent", True, True)> _
        <Category("RequestEvents")> _
        Public Property MinTEngineEvents() As TEngineEvents

        <ConditionalVisible("HasEvent", True, True)> _
        <Category("RequestEvents")> _
        Public Property MaxTEngineEvents() As TEngineEvents

        <ConditionalVisible("HasEvent", True, True)> _
        <Category("RequestEvents")> _
        Public Property DefaultTEngineEvents() As TEngineEvents
            
    End Class
End Namespace