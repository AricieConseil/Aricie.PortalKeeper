Imports Aricie.ComponentModel
Imports System.Threading
Imports System.Linq
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

        Private _Started As Boolean
        Private _WakeUp As Boolean

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
            Me._Action = action
            Me._ID = Guid.NewGuid
            Me._TaskInfo = objTask
            If Not _TaskInfo.Synchronous Then
                Dim smtpInfo As New STPStartInfo()
                smtpInfo.AreThreadsBackground = Me._TaskInfo.IsBackground
                smtpInfo.MaxWorkerThreads = Me._TaskInfo.NbThreads
                smtpInfo.ThreadPoolName = Me._ID.ToString()
                smtpInfo.UseCallerCallContext = True
                smtpInfo.UseCallerHttpContext = True
                smtpInfo.PostExecuteWorkItemCallback = AddressOf PostExecuteWorkItemCallback
                If Me._TaskInfo.ApartmentState <> ApartmentState.Unknown Then
                    smtpInfo.ApartmentState = Me._TaskInfo.ApartmentState
                End If
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
                'todo use groups if needed cause the method throws a notsupportedexception instead otherwise
                'AddHandler SmartPool.OnIdle, AddressOf OnIdle
            End If



        End Sub

        Private Sub StartSmartThreadPool(ByVal state As Object)
            _Timer = Nothing
            _SmartPool.Start()
        End Sub


        Public Sub New(ByVal action As Action(Of T), ByVal nbThreads As Integer, ByVal background As Boolean, _
                        ByVal initialWaitTime As TimeSpan, ByVal wakeUpWaitTime As TimeSpan, ByVal taskWaitTime As TimeSpan)
            Me.New(action, New TaskQueueInfo(nbThreads, background, initialWaitTime, wakeUpWaitTime, taskWaitTime))
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
            If Not _TaskInfo.Synchronous Then
                SyncLock _Locker
                    For Each task As T In tasks
                        SmartPool.QueueWorkItem(Of T)(AddressOf Me.Work, task)
                    Next
                End SyncLock
            Else
                For Each objTask As T In tasks
                    Me.Work(objTask)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Add task to the queue
        ''' </summary>
        ''' <param name="task"></param>
        ''' <remarks></remarks>
        Public Sub EnqueueTask(ByVal task As T)
            If Not _TaskInfo.Synchronous Then
                SyncLock _Locker
                    SmartPool.QueueWorkItem(Of T)(AddressOf Me.Work, task)
                End SyncLock
            Else
                Me.Work(task)
            End If
        End Sub

        ''' <summary>
        ''' Terminates tasks waiting in the queue
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Terminate()
            SyncLock _Locker
                Thread.VolatileWrite(_TerminateWorkers, 1)
                SmartPool.Shutdown(True, TimeSpan.FromMilliseconds(300))
            End SyncLock
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
                    _Started = True
                ElseIf _WakeUp AndAlso Not Me._TaskInfo.WakeUpWaitTime = TimeSpan.Zero Then
                    Thread.Sleep(Me._TaskInfo.WakeUpWaitTime)
                    _WakeUp = False
                ElseIf Not Me._TaskInfo.TaksWaitTime = TimeSpan.Zero Then
                    Thread.Sleep(Me._TaskInfo.TaksWaitTime)
                End If
                Me._Action.Invoke(state)
            Catch ex As Exception
                Try
                    Aricie.Services.ExceptionHelper.LogException(ex)
                Catch
                End Try
            End Try
        End Sub




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
