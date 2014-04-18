Imports Aricie.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Structure BotEndStatus(Of TEngineEvent As IConvertible)

        Public Sub New(ByVal botHistory As WebBotHistory, ByVal runContext As PortalKeeperContext(Of TEngineEvent))
            Me.History = botHistory
            Me.Context = runContext
        End Sub

        Public Property History As WebBotHistory
        Public Property Context As PortalKeeperContext(Of TEngineEvent)
    End Structure


    <Serializable()> _
    Public Class BotRunContext(Of TEngineEvent As IConvertible)

        

        Public Event Init As EventHandler(Of GenericEventArgs(Of BotRunContext(Of TEngineEvent)))

        Public Event Finalize As EventHandler(Of GenericEventArgs(Of BotRunContext(Of TEngineEvent)))

        'Private Sub New()
        '    Me.AsyncLockId = -1
        'End Sub

        Public Sub New(objBot As BotInfo(Of TEngineEvent), nextSchedule As DateTime)
            Me.AsyncLockId = objBot.Name.GetHashCode()
            Me.NextSchedule = nextSchedule
        End Sub

        Public Property NextSchedule As DateTime

        Public Enabled As Boolean

        Private _Id As String

        Public ReadOnly Property Id As String
            Get
                If String.IsNullOrEmpty(_Id) Then
                    _Id = Guid.NewGuid.ToString()
                End If
                Return _Id
            End Get
        End Property

        Public Property AsyncLockId() As Integer

        Public Property Events() As IList(Of TEngineEvent)

        Public Property History() As WebBotHistory

        Public Property EngineContext() As PortalKeeperContext(Of TEngineEvent)

        Public Property UserParams() As IDictionary(Of String, Object)


        Public Sub OnInit()
            RaiseEvent Init(Me, New GenericEventArgs(Of BotRunContext(Of TEngineEvent))(Me))
        End Sub

        Public Sub OnFinalize()
            RaiseEvent Finalize(Me, New GenericEventArgs(Of BotRunContext(Of TEngineEvent))(Me))
        End Sub


       

    End Class
End Namespace