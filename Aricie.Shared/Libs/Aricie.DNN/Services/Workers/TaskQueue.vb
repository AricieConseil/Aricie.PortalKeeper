Imports Aricie.ComponentModel
Imports System.Threading
Imports DotNetNuke.Services.Exceptions
Imports Amib.Threading

Namespace Services.Workers
    ''' <summary>
    ''' Processing Queue
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class TaskQueue(Of T)
        Implements IDisposable

        Private _ID As Guid
        Private _Action As Action(Of T)


        Private _TaskInfo As TaskQueueInfo
        'Private _NbThreads As Integer = 1


        'Private _Queue As New Queue(Of T)
        Private _Started As Boolean
        Private _WakeUp As Boolean


        'Private _InitialWaitTime As TimeSpan
        'Private _WakeUpWaitTime As TimeSpan
        'Private _TaksWaitTime As TimeSpan

        Private _Locker As New Object
        Private _TerminateWorkers As Integer = 0

        Private _SmartPool As SmartThreadPool


        Private _Timer As Timer

#Region "cTors"

        ''' <summary>
        ''' T étant le type de paramètre utilisé par la fonction appelée (action),
        ''' la TaskQueue permet d'appeler plusieurs fois la même fonction avec des paramètres différents en asynchrone.
        ''' </summary>
        ''' <param name="action">Méthode à utiliser par les threads.</param>
        ''' <param name="objTask">Objet de paramètres de la TaskQueue</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal action As Action(Of T), ByVal objTask As TaskQueueInfo)
            'Me.New(action, objTask.NbThreads, objTask.IsBackground, objTask.InitialWaitTime.Value, objTask.WakeUpWaitTime.Value, objTask.TaksWaitTime.Value)
            Me._Action = action
            Me._ID = Guid.NewGuid
            Me._TaskInfo = objTask
            'AsyncWorker.Instance.Start(Me._ID.ToString, AddressOf Me.Work, objTask.NbThreads, objTask.IsBackground, Me)

            Dim smtpInfo As New STPStartInfo()
            smtpInfo.AreThreadsBackground = Me._TaskInfo.IsBackground
            smtpInfo.MaxWorkerThreads = Me._TaskInfo.NbThreads
            smtpInfo.ThreadPoolName = Me._ID.ToString()
            smtpInfo.UseCallerCallContext = True
            smtpInfo.UseCallerHttpContext = True
            smtpInfo.PostExecuteWorkItemCallback = AddressOf PostExecuteWorkItemCallback
            smtpInfo.ThreadPriority = Me._TaskInfo.ThreadPriority
            smtpInfo.IdleTimeout = CInt(Me._TaskInfo.IdleTimeout.Value.TotalMilliseconds)
            If Me._TaskInfo.EnablePerformanceCounters Then
                smtpInfo.PerformanceCounterInstanceName = Me._TaskInfo.PerformanceCounterInstanceName
                'smtpInfo.EnableLocalPerformanceCounters = True
            End If
            If Me._TaskInfo.InitialWaitTime.Value <> TimeSpan.Zero Then
                smtpInfo.StartSuspended = True
            End If

            _SmartPool = New SmartThreadPool(smtpInfo)

            If Me._TaskInfo.InitialWaitTime.Value <> TimeSpan.Zero Then
                _Timer = New Timer(New TimerCallback(AddressOf Me.StartSmartThreadPool), Me, Me._TaskInfo.InitialWaitTime.Value, TimeSpan.FromMilliseconds(-1))
            End If


            AddHandler SmartPool.OnThreadTermination, AddressOf OnThreadTermination
            'SmartPool.Start()



        End Sub

        Private Sub StartSmartThreadPool(ByVal state As Object)
            _Timer = Nothing
            _SmartPool.Start()
        End Sub


        Public Sub New(ByVal action As Action(Of T), ByVal nbThreads As Integer, ByVal background As Boolean, _
                        ByVal initialWaitTime As TimeSpan, ByVal wakeUpWaitTime As TimeSpan, ByVal taskWaitTime As TimeSpan)
            Me.New(action, New TaskQueueInfo(nbThreads, background, initialWaitTime, wakeUpWaitTime, taskWaitTime))
            'Me._NbThreads = nbThreads
            'Me._InitialWaitTime = initialWaitTime
            'Me._WakeUpWaitTime = wakeUpWaitTime
            'Me._TaksWaitTime = taskWaitTime


        End Sub

        <Obsolete()> _
        Public Sub New(ByVal action As Action(Of T), ByVal nbThreads As Integer, ByVal background As Boolean, _
                      ByVal taskWaitTime As TimeSpan)
            Me.New(action, nbThreads, background, taskWaitTime, TimeSpan.Zero, TimeSpan.Zero)
        End Sub

        Public ReadOnly Property SmartPool As SmartThreadPool
            Get
                Return _SmartPool
            End Get
        End Property

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
                    SmartPool.QueueWorkItem(Of T)(AddressOf Me.Work, task)
                    '        _Queue.Enqueue(task)
                Next
                '    Monitor.PulseAll(_Locker)

            End SyncLock

        End Sub

        ''' <summary>
        ''' Add task to the queue
        ''' </summary>
        ''' <param name="task"></param>
        ''' <remarks></remarks>
        Public Sub EnqueueTask(ByVal task As T)
            SyncLock _Locker
                SmartPool.QueueWorkItem(Of T)(AddressOf Me.Work, task)
            End SyncLock

            'SyncLock _Locker
            '    _Queue.Enqueue(task)
            '    Monitor.PulseAll(_Locker)

            'End SyncLock
        End Sub

        ''' <summary>
        ''' Terminates tasks waiting in the queue
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Terminate()
            SyncLock _Locker
                Thread.VolatileWrite(_TerminateWorkers, 1)
                SmartPool.Shutdown(True, TimeSpan.FromMilliseconds(300))

                '    Monitor.PulseAll(_Locker)
            End SyncLock

            'Dim timeOut As TimeSpan = TimeSpan.FromMilliseconds(300 / Me._TaskInfo.NbThreads) + Me._TaskInfo.TaksWaitTime
            'If Not Me._Started Then
            '    timeOut += Me._TaskInfo.InitialWaitTime
            'ElseIf Me._WakeUp Then
            '    timeOut += Me._TaskInfo.WakeUpWaitTime
            'End If
            'AsyncWorker.Instance.JoinAll(Me._ID.ToString, timeOut)
        End Sub

#End Region


#Region "Private methods"

        ''' <summary>
        ''' runs methods waiting in the queue
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Work(state As T)
           
            Try
                'SyncLock _Locker
                If Not _Started Then
                    'If Not Me._TaskInfo.InitialWaitTime = TimeSpan.Zero Then
                    '    Thread.Sleep(Me._TaskInfo.InitialWaitTime)
                    'End If
                    _Started = True
                ElseIf _WakeUp AndAlso Not Me._TaskInfo.WakeUpWaitTime = TimeSpan.Zero Then
                    Thread.Sleep(Me._TaskInfo.WakeUpWaitTime)
                    _WakeUp = False
                ElseIf Not Me._TaskInfo.TaksWaitTime = TimeSpan.Zero Then
                    Thread.Sleep(Me._TaskInfo.TaksWaitTime)
                End If
                    Me._Action.Invoke(state)
                    'End SyncLock




                    'Catch ex As ThreadInterruptedException
                    '    Try
                    '        Aricie.Services.ExceptionHelper.LogException(ex)
                    '    Catch
                    '    End Try
                    'Catch ex As ThreadAbortException
                    '    Thread.ResetAbort()
                    '    Try
                    '        Aricie.Services.ExceptionHelper.LogException(ex)
                    '    Catch
                    '    End Try
            Catch ex As Exception
                Try
                    Aricie.Services.ExceptionHelper.LogException(ex)
                Catch
                End Try
            End Try



        End Sub


        ' ''' <summary>
        ' ''' runs methods waiting in the queue
        ' ''' </summary>
        ' ''' <remarks></remarks>
        'Private Sub Work()
        '    If Not Me._TaskInfo.InitialWaitTime = TimeSpan.Zero Then
        '        Thread.Sleep(Me._TaskInfo.InitialWaitTime)
        '    End If
        '    _Started = True

        '    While Thread.VolatileRead(Me._TerminateWorkers) = 0
        '        Try
        '            Dim objTask As T
        '            _WakeUp = False
        '            SyncLock _Locker


        '                While _Queue.Count = 0 AndAlso Thread.VolatileRead(Me._TerminateWorkers) = 0

        '                    Monitor.Wait(_Locker)
        '                    _WakeUp = True


        '                End While
        '                If Thread.VolatileRead(Me._TerminateWorkers) = 1 Then
        '                    Exit While
        '                End If


        '                objTask = _Queue.Dequeue

        '            End SyncLock

        '            If _WakeUp AndAlso Not Me._TaskInfo.WakeUpWaitTime = TimeSpan.Zero Then
        '                Thread.Sleep(Me._TaskInfo.WakeUpWaitTime)
        '            End If

        '            _Action.Invoke(objTask)
        '            Me.OnActionPerformed()
        '            If Not Me._TaskInfo.TaksWaitTime = TimeSpan.Zero Then
        '                Thread.Sleep(Me._TaskInfo.TaksWaitTime)
        '            End If
        '        Catch ex As Exception
        '            Aricie.Providers.SystemServiceProvider.Instance().LogException(ex)
        '        End Try

        '    End While

        'End Sub

        Private Sub PostExecuteWorkItemCallback(ByVal wir As IWorkItemResult)
            Me.OnActionPerformed()
        End Sub

        ''' <summary>
        ''' Callback on action run
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub OnActionPerformed()
            RaiseEvent ActionPerformed(Me, New GenericEventArgs(Of Integer)(Me.SmartPool.WaitingCallbacks))
        End Sub

        Public Event ActionPerformed As EventHandler(Of GenericEventArgs(Of Integer))

        Private Sub OnIdle(ByVal workitemsgroup As IWorkItemsGroup)
            _WakeUp = True
        End Sub

        Private Sub OnThreadTermination()
            If _SmartPool.WaitingCallbacks = 0 Then
                _WakeUp = True
            End If
        End Sub

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
