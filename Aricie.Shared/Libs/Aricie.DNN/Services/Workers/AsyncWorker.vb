Imports System.Threading
Imports Aricie.Services

Namespace Services.Workers
    ''' <summary>
    ''' Class to run threads operation safely
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SafeRunner

        Private _Start As ThreadStart


        Public Sub New(ByVal objStart As ThreadStart)
            Me._Start = objStart
        End Sub

        ''' <summary>
        ''' Runs the thread that has 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Run()
            Try
                _Start.Invoke()
            Catch ex As ThreadInterruptedException
                Try
                    Aricie.Services.ExceptionHelper.LogException(ex)
                Catch
                End Try
            Catch ex As ThreadAbortException
                Thread.ResetAbort()
                Try
                    Aricie.Services.ExceptionHelper.LogException(ex)
                Catch
                End Try
            End Try

        End Sub


    End Class

    ''' <summary>
    ''' Async worker class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AsyncWorker
        Implements IDisposable


        ''' <summary>
        ''' Returns single instance
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Instance() As AsyncWorker
            Return ReflectionHelper.GetSingleton(Of AsyncWorker)()
        End Function

        Private _CustomPool As New Dictionary(Of String, List(Of Thread))
        Private _Disposables As New Dictionary(Of String, List(Of IDisposable))

        ''' <summary>
        ''' Adds a thread to run asynchronously
        ''' </summary>
        ''' <param name="objStart"></param>
        ''' <param name="background"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AsyncRun(ByVal objStart As ThreadStart, ByVal background As Boolean) As Thread
            Dim worker As New Thread(New ThreadStart(AddressOf (New SafeRunner(objStart)).Run))
            worker.Priority = ThreadPriority.Lowest
            If background Then
                worker.IsBackground = True
            End If
            worker.Start()
            Return worker
        End Function

        ''' <summary>
        ''' Starts a thread with dispoable objects
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="objStart"></param>
        ''' <param name="nbThreads"></param>
        ''' <param name="background"></param>
        ''' <param name="objIDisposable"></param>
        ''' <remarks></remarks>
        Public Sub Start(ByVal key As String, ByVal objStart As ThreadStart, ByVal nbThreads As Integer, ByVal background As Boolean, ByVal objIDisposable As IDisposable)
            If objIDisposable IsNot Nothing Then
                SyncLock Me._Disposables
                    Dim tempDisposables As List(Of IDisposable) = Nothing

                    If Not Me._Disposables.TryGetValue(key, tempDisposables) Then
                        tempDisposables = New List(Of IDisposable)
                    End If
                    If Not tempDisposables.Contains(objIDisposable) Then
                        tempDisposables.Add(objIDisposable)

                        Me._Disposables(key) = tempDisposables

                    End If
                End SyncLock
            End If

            Dim tempThreadList As List(Of Thread) = Nothing
            SyncLock Me._CustomPool
                If Not _CustomPool.TryGetValue(key, tempThreadList) Then
                    tempThreadList = New List(Of Thread)(nbThreads)
                    For i As Integer = 0 To nbThreads - 1
                        Dim worker As Thread = AsyncRun(objStart, background)
                        tempThreadList.Add(worker)
                    Next
                    _CustomPool(key) = tempThreadList
                End If
            End SyncLock


            'todo: voir ce qu'il en est
            'Else
            '    Throw New ApplicationException("Custom Pool already in use?")


        End Sub

        ''' <summary>
        ''' Joins all threads to wait on their end
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="timeOut"></param>
        ''' <remarks></remarks>
        Public Sub JoinAll(ByVal key As String, ByVal timeOut As TimeSpan)
            If _CustomPool.ContainsKey(key) Then
                For Each worker As Thread In _CustomPool(key)
                    If worker IsNot Nothing AndAlso worker.IsAlive Then
                        If Not worker.Join(timeOut) Then
                            ForceThreadEnd(worker)
                        End If
                    End If
                Next
            End If

        End Sub

        ''' <summary>
        ''' Force a thread end
        ''' </summary>
        ''' <param name="worker"></param>
        ''' <remarks></remarks>
        Public Shared Sub ForceThreadEnd(ByVal worker As Thread)
            If (worker.ThreadState And ThreadState.WaitSleepJoin) = ThreadState.WaitSleepJoin Then
                worker.Interrupt()
                Thread.Sleep(50)
            End If
            If worker.IsAlive Then
                worker.Abort()
            End If
        End Sub

        ''' <summary>
        ''' Force all threads to terminate
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub TerminateAll()
            'Dim ex As New Exception("Disposing AsyncWorker")
            'DotNetNuke.Services.Exceptions.LogException(ex)

            Dim tempList As New List(Of List(Of IDisposable))(Me._Disposables.Values)
            For i As Integer = 0 To tempList.Count - 1
                Dim listIdispo As List(Of IDisposable) = tempList(i)
                If listIdispo IsNot Nothing Then
                    For j As Integer = 0 To listIdispo.Count - 1
                        Dim objIdisposable As IDisposable = listIdispo(j)
                        If objIdisposable IsNot Nothing Then
                            objIdisposable.Dispose()
                        End If
                    Next
                End If
            Next

            For Each key As String In _CustomPool.Keys
                JoinAll(key, TimeSpan.FromMilliseconds(100))
            Next

        End Sub



        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Try
                        TerminateAll()
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                    End Try
                End If

            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
