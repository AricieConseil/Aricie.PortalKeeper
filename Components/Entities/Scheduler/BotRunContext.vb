Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class BotRunContext(Of TEngineEvent As IConvertible)

        Private _Id As String

        Public ReadOnly Property Id As String
            Get
                If String.IsNullOrEmpty(_Id) Then
                    _Id = Guid.NewGuid.ToString()
                End If
                Return _Id
            End Get
        End Property

        Private _AsyncLockId As Integer = -1

        Public Property AsyncLockId() As Integer
            Get
                Return _AsyncLockId
            End Get
            Set(ByVal value As Integer)
                _AsyncLockId = value
            End Set
        End Property


        Private _Envents As IList(Of TEngineEvent)

        Public Property Events() As IList(Of TEngineEvent)
            Get
                Return _Envents
            End Get
            Set(ByVal value As IList(Of TEngineEvent))
                _Envents = value
            End Set
        End Property


        Private _History As WebBotHistory


        Public Property History() As WebBotHistory
            Get
                Return _History
            End Get
            Set(ByVal value As WebBotHistory)
                _History = value
            End Set
        End Property


        Private _EngineContext As PortalKeeperContext(Of TEngineEvent)
        Public Property EngineContext() As PortalKeeperContext(Of TEngineEvent)
            Get
                Return _EngineContext
            End Get
            Set(ByVal value As PortalKeeperContext(Of TEngineEvent))
                _EngineContext = value
            End Set
        End Property


        Private _UserParams As IDictionary(Of String, Object)
        Public Property UserParams() As IDictionary(Of String, Object)
            Get
                Return _UserParams
            End Get
            Set(ByVal value As IDictionary(Of String, Object))
                _UserParams = value
            End Set
        End Property


        Private _RunEndDelegate As BotRunEndEventHandler
        Public Property RunEndDelegate() As BotRunEndEventHandler
            Get
                Return _RunEndDelegate
            End Get
            Set(ByVal value As BotRunEndEventHandler)
                _RunEndDelegate = value
            End Set
        End Property


        Public Delegate Sub BotRunEndEventHandler(ByVal botHistory As WebBotHistory, ByVal runContext As PortalKeeperContext(Of TEngineEvent))

    End Class
End Namespace