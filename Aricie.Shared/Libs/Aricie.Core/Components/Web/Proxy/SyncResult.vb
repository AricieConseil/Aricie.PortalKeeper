Imports System.Threading
Imports System.Web

Namespace Web.Proxy
    Public Class SyncResult
        Implements IAsyncResult


        Public Content As CachedContent
        Public Context As HttpContext


        Private ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
            Get
                Return New Object()
            End Get
        End Property

        Private ReadOnly Property AsyncWaitHandle() As WaitHandle Implements IAsyncResult.AsyncWaitHandle
            Get
                Return New ManualResetEvent(True)
            End Get
        End Property

        Private ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
            Get
                Return True
            End Get
        End Property

        Private ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
            Get
                Return True
            End Get
        End Property
    End Class


End NameSpace