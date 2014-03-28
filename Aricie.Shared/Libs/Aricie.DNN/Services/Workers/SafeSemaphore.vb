Imports System.Security.Permissions
Imports System.Threading
Imports Aricie.Services

Namespace Services.Workers
    <HostProtection(SecurityAction.LinkDemand, Synchronization:=True, ExternalThreading:=True)>
    Public Class SafeSemaphore
        Implements IDisposable

        'Inherits WaitHandle

        'Private Shared MAX_PATH As Integer

        'Shared Sub New()
        '    SafeSemaphore.MAX_PATH = 260
        'End Sub

        Private _MaxCount As Integer
        Private _Name As String

        Shared Sub New()
            _Watch = New Stopwatch
            _Watch.Start()
        End Sub


        Public Sub New(maxCount As Integer)
            Me.New(maxCount, Nothing)
        End Sub


        Public Sub New(maxCount As Integer, name As String)
            'MyBase.New()
            Me._MaxCount = maxCount
            Me._Name = name
        End Sub

        Private Shared _Watch As New Stopwatch

        Public Function Wait() As Boolean
            Return Me.Wait(TimeSpan.Zero)
        End Function

        Public Function Wait(timeout As TimeSpan) As Boolean
            Dim startTime As TimeSpan = _Watch.Elapsed
            Dim switchPeriod As TimeSpan = TimeSpan.FromMilliseconds(1)
            Dim maxWait As DateTime = DateTime.MaxValue
            If Not timeout = TimeSpan.Zero Then
                switchPeriod = TimeSpan.FromTicks(timeout.Ticks \ 100)
            End If
            Dim index As Long = _Watch.Elapsed.Ticks Mod _MaxCount
            Dim owned As Boolean
            Do
                owned = Me.WaitMutex(CInt(index), switchPeriod)
            Loop Until owned OrElse timeout <> TimeSpan.Zero AndAlso timeout < _Watch.Elapsed.Subtract(startTime)
            Return owned
        End Function

        Private lock As New Object

        Public Sub Release()
            If _Mutex IsNot Nothing Then
                SyncLock lock
                    Try
                        If _Mutex IsNot Nothing Then
                            _Mutex.ReleaseMutex()
                        End If
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    Finally
                        _Mutex = Nothing
                    End Try
                End SyncLock
            End If
        End Sub

        Private _Mutex As Mutex

        Private Function WaitMutex(index As Integer, timeout As TimeSpan) As Boolean
            Dim owned As Boolean
            Dim objMutex As New Mutex(False, "SafeSemMutex" & Me._Name & index)
            Try

                'Dim allowEveryoneRule As New MutexAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), MutexRights.FullControl, AccessControlType.Allow)
                'Dim securitySettings As New MutexSecurity()
                'securitySettings.AddAccessRule(allowEveryoneRule)
                'objMutex.SetAccessControl(securitySettings)
                If (Not timeout = TimeSpan.Zero AndAlso objMutex.WaitOne(timeout)) OrElse (timeout = TimeSpan.Zero AndAlso objMutex.WaitOne()) Then
                    SyncLock lock
                        _Mutex = objMutex
                    End SyncLock
                    owned = True
                End If
            Catch ex As AbandonedMutexException
                ExceptionHelper.LogException(ex)
                objMutex.ReleaseMutex()
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            End Try
            Return owned
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    
                    ' TODO: dispose managed state (managed objects).
                End If
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Me.Release()
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End NameSpace