
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ConditionProviderConfig(Of TEngineEvents As IConvertible)
        Inherits KeeperProviderConfig(Of TEngineEvents, IConditionProvider(Of TEngineEvents))

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal objType As Type)
            MyBase.New(objType)
        End Sub

        Public Sub New(ByVal objType As Type, ByVal minLifeCycleEvent As TEngineEvents, ByVal maxLifeCycleEvent As TEngineEvents, ByVal defaultLifeCycleEvent As TEngineEvents)
            MyBase.New(objType, minLifeCycleEvent, maxLifeCycleEvent, defaultLifeCycleEvent)
        End Sub

        Private _IsDosEnabled As Nullable(Of Boolean)

        <ExtendedCategory("")> _
        Public ReadOnly Property IsDoSEnabled() As Boolean
            Get
                If Not _IsDosEnabled.HasValue Then
                    Dim prov As IConditionProvider(Of TEngineEvents) = Me.GetTypedProvider
                    If prov IsNot Nothing AndAlso TypeOf prov Is IDoSEnabledConditionProvider(Of TEngineEvents) Then
                        _IsDosEnabled = True
                    Else
                        _IsDosEnabled = False
                    End If
                End If
                Return _IsDosEnabled.Value
            End Get
        End Property



    End Class
End Namespace