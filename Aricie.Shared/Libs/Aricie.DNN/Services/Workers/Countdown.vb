Imports System.Threading

Namespace Services.Workers
    
    Public Class Countdown
        Private _locker As New Object()
        Private _value As Integer

        Public Sub New()
        End Sub
        Public Sub New(initialCount As Integer)
            Value = initialCount
        End Sub

        Public Property Value As Integer
            Get
                Return _value
            End Get
            Set(value As Integer)
                _value = value
            End Set
        End Property

        Public Sub Signal()
            AddCount(-1)
        End Sub

        Public Sub AddCount(amount As Integer)
            SyncLock _locker
                Value += amount
                If Value <= 0 Then
                    Monitor.PulseAll(_locker)
                End If
            End SyncLock
        End Sub

        Public Sub Wait()
            SyncLock _locker
                While Value > 0
                    Monitor.Wait(_locker)
                End While
            End SyncLock
        End Sub
    End Class
End Namespace