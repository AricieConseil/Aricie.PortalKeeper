Imports Aricie.ComponentModel
Imports System.Threading
Imports DotNetNuke.Services.Exceptions

Namespace Services.Workers

    ''' <summary>
    ''' Queue d'attente de résolutions
    ''' </summary>
    ''' <remarks></remarks>
    Public Class WaitQueue
        Inherits TaskQueue(Of Boolean)

        Public Sub New(ByVal objTask As TaskQueueInfo)
            MyBase.New(AddressOf WaitInternal, objTask.NbThreads, objTask.IsBackground, objTask.InitialWaitTime.Value, objTask.WakeUpWaitTime.Value, objTask.TaksWaitTime.Value)

        End Sub

        ''' <summary>
        ''' Attend numTimes réponses
        ''' </summary>
        ''' <param name="numTimes"></param>
        ''' <remarks></remarks>
        Public Sub Wait(ByVal numTimes As Integer)
            Dim params(numTimes) As Boolean
            Me.EnqueueTasks(params)
        End Sub

        ''' <summary>
        ''' Attend une réponse
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub WaitOne()
            Me.EnqueueTask(True)
        End Sub


        Private Shared Sub WaitInternal(ByVal dumbParam As Boolean)

        End Sub

    End Class

    ''' <summary>
    ''' Queue de traitement
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class TaskQueue(Of T)
        Implements IDisposable

        Private _ID As Guid
        Private _Action As Action(Of T)
        Private _NbThreads As Integer = 1

        Private _Queue As New Queue(Of T)
        Private _Started As Boolean
        Private _WakeUp As Boolean


        Private _InitialWaitTime As TimeSpan
        Private _WakeUpWaitTime As TimeSpan
        Private _TaksWaitTime As TimeSpan

        Private _Locker As New Object
        Private _TerminateWorkers As Integer = 0

#Region "cTors"

        ''' <summary>
        ''' T étant le type de paramètre utilisé par la fonction appelée (action),
        ''' la TaskQueue permet d'appeler plusieurs fois la même fonction avec des paramètres différents en asynchrone.
        ''' </summary>
        ''' <param name="action">Méthode à utiliser par les threads.</param>
        ''' <param name="objTask">Objet de paramètres de la TaskQueue</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal action As Action(Of T), ByVal objTask As TaskQueueInfo)
            Me.New(action, objTask.NbThreads, objTask.IsBackground, objTask.InitialWaitTime.Value, objTask.WakeUpWaitTime.Value, objTask.TaksWaitTime.Value)

        End Sub

        Public Sub New(ByVal action As Action(Of T), ByVal nbThreads As Integer, ByVal background As Boolean, _
                        ByVal initialWaitTime As TimeSpan, ByVal wakeUpWaitTime As TimeSpan, ByVal taskWaitTime As TimeSpan)

            Me._Action = action
            Me._ID = Guid.NewGuid
            Me._NbThreads = nbThreads
            Me._InitialWaitTime = initialWaitTime
            Me._WakeUpWaitTime = wakeUpWaitTime
            Me._TaksWaitTime = taskWaitTime
            AsyncWorker.Instance.Start(Me._ID.ToString, AddressOf Me.Work, nbThreads, background, Me)

        End Sub

        <Obsolete()> _
        Public Sub New(ByVal action As Action(Of T), ByVal nbThreads As Integer, ByVal background As Boolean, _
                      ByVal waitTimeBeforeStart As TimeSpan)

            Me._Action = action
            Me._ID = Guid.NewGuid
            Me._NbThreads = nbThreads
            Me._TaksWaitTime = waitTimeBeforeStart
            AsyncWorker.Instance.Start(Me._ID.ToString, AddressOf Me.Work, nbThreads, background, Me)



        End Sub

#End Region

#Region "Public methods"

        ''' <summary>
        ''' Adds tasks to the queue
        ''' </summary>
        ''' <param name="tasks"></param>
        ''' <remarks></remarks>
        Public Sub EnqueueTasks(ByVal tasks As IEnumerable(Of T))
            SyncLock _Locker
                For Each task As T In tasks
                    _Queue.Enqueue(task)
                Next
                Monitor.PulseAll(_Locker)

            End SyncLock
        End Sub

        ''' <summary>
        ''' Add task to the queue
        ''' </summary>
        ''' <param name="task"></param>
        ''' <remarks></remarks>
        Public Sub EnqueueTask(ByVal task As T)
            SyncLock _Locker
                _Queue.Enqueue(task)
                Monitor.PulseAll(_Locker)

            End SyncLock
        End Sub

        ''' <summary>
        ''' Terminates tasks waiting in the queue
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Terminate()
            Thread.VolatileWrite(_TerminateWorkers, 1)
            SyncLock _Locker
                Monitor.PulseAll(_Locker)
            End SyncLock

            Dim timeOut As TimeSpan = TimeSpan.FromMilliseconds(300 / Me._NbThreads) + Me._TaksWaitTime
            If Not Me._Started Then
                timeOut += Me._InitialWaitTime
            ElseIf Me._WakeUp Then
                timeOut += Me._WakeUpWaitTime
            End If
            AsyncWorker.Instance.JoinAll(Me._ID.ToString, timeOut)
        End Sub

#End Region


#Region "Private methods"

        ''' <summary>
        ''' runs methods waiting in the queue
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Work()
            If Not Me._InitialWaitTime = TimeSpan.Zero Then
                Thread.Sleep(Me._InitialWaitTime)
            End If
            _Started = True

            While Thread.VolatileRead(Me._TerminateWorkers) = 0
                Try
                    Dim objTask As T
                    _WakeUp = False
                    SyncLock _Locker


                        While _Queue.Count = 0 AndAlso Thread.VolatileRead(Me._TerminateWorkers) = 0

                            Monitor.Wait(_Locker)
                            _WakeUp = True


                        End While
                        If Thread.VolatileRead(Me._TerminateWorkers) = 1 Then
                            Exit While
                        End If


                        objTask = _Queue.Dequeue

                    End SyncLock

                    If _WakeUp AndAlso Not Me._WakeUpWaitTime = TimeSpan.Zero Then
                        Thread.Sleep(Me._WakeUpWaitTime)
                    End If

                    _Action.Invoke(objTask)
                    Me.OnActionPerformed()
                    If Not Me._TaksWaitTime = TimeSpan.Zero Then
                        Thread.Sleep(Me._TaksWaitTime)
                    End If
                Catch ex As Exception
                    Aricie.Providers.SystemServiceProvider.Instance().LogException(ex)
                End Try

            End While

        End Sub

        ''' <summary>
        ''' Callback on action run
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub OnActionPerformed()
            RaiseEvent ActionPerformed(Me, New GenericEventArgs(Of Integer)(_Queue.Count))
        End Sub

        Public Event ActionPerformed As EventHandler(Of GenericEventArgs(Of Integer))


#End Region

#Region " IDisposable Support "

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Private disposedValue As Boolean = False
        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Terminate()
                End If
            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class
End Namespace
