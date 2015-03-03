

Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ActionProviderConfig(Of TEngineEvents As IConvertible)
        Inherits KeeperProviderConfig(Of TEngineEvents, IActionProvider(Of TEngineEvents))


        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal objType As Type)
            MyBase.New(objType)
        End Sub

        Public Sub New(ByVal objType As Type, ByVal minLifeCycleEvent As TEngineEvents, ByVal maxLifeCycleEvent As TEngineEvents, ByVal defaultLifeCycleEvent As TEngineEvents)
            MyBase.New(objType, minLifeCycleEvent, maxLifeCycleEvent, defaultLifeCycleEvent)
        End Sub


        Private _IsAsyncEnabled As Nullable(Of Boolean)

        <ExtendedCategory("")> _
        Public ReadOnly Property IsAsyncEnabled() As Boolean
            Get
                If Not _IsAsyncEnabled.HasValue Then
                    Dim prov As IActionProvider(Of TEngineEvents) = Me.GetTypedProvider
                    If prov IsNot Nothing AndAlso TypeOf prov Is AsyncEnabledActionProvider(Of TEngineEvents) Then
                        _IsAsyncEnabled = True
                    Else
                        _IsAsyncEnabled = False
                    End If
                End If
                Return _IsAsyncEnabled.Value
            End Get
        End Property





    End Class
End Namespace