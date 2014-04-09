Imports DotNetNuke.Services.Log.EventLog
Imports Aricie.DNN.Services.Workers
Imports System.Threading
Imports Aricie.Services
Imports System.Web
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.Services

Namespace Diagnostics

    ''' <summary>
    ''' Asynchronous DotNetNuke Logger.
    ''' </summary>
    ''' <remarks>Has a default period of 10 ms between each DNN log API call,and queues early call to wait for the providers to be loaded.</remarks>
    Public Class AsyncLogger
        Implements IDisposable



        Public Shared Function Instance() As AsyncLogger
            Return ReflectionHelper.GetSingleton(Of AsyncLogger)()
        End Function


        Private _PendingLock As New Object
        Private _LogWaitingList As New Queue(Of LogInfo)

        Private _LogTaskQueue As TaskQueue(Of LogInfo)
        Private _DeleteLogTaskQueue As TaskQueue(Of LogInfo)

        Private _EventLogStarted As Boolean = False
        Private _Timer As Timer

        Private Sub StartLogging()
            FlushPendingLogs()
            _EventLogStarted = True
        End Sub

        Private Sub StarterTimerCallBack(ByVal state As Object)
            If Not _EventLogStarted Then
                StartLogging()
            End If
        End Sub

        Public Sub CheckLogStarted()
            If Not _EventLogStarted Then
                If HttpContext.Current IsNot Nothing _
                    AndAlso HttpContext.Current.CurrentHandler IsNot Nothing Then
                    StartLogging()
                ElseIf _Timer Is Nothing Then
                    _Timer = New Timer(New TimerCallback(AddressOf Me.StarterTimerCallBack), Me, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1))
                End If
            End If
        End Sub


        Private ReadOnly Property LogTaskQueue() As TaskQueue(Of LogInfo)
            Get
                If _LogTaskQueue Is Nothing Then
                    Dim objTaskInfo As New TaskQueueInfo(1, True, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMilliseconds(10))
#If DEBUG Then
                    objTaskInfo.EnablePerformanceCounters = True
                    objTaskInfo.PerformanceCounterInstanceName &= "-AsyncLogger"
#End If
                    _LogTaskQueue = New TaskQueue(Of LogInfo)(New Action(Of LogInfo)(AddressOf LogSlowly), objTaskInfo)
                End If
                Return _LogTaskQueue
            End Get
        End Property

        Private ReadOnly Property DeleteLogTaskQueue() As TaskQueue(Of LogInfo)
            Get
                If _DeleteLogTaskQueue Is Nothing Then
                    _DeleteLogTaskQueue = New TaskQueue(Of LogInfo)(New Action(Of LogInfo)(AddressOf DeleteLogSlowly), 1, True, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4), TimeSpan.FromMilliseconds(10))
                End If
                Return _DeleteLogTaskQueue
            End Get
        End Property

        Public Sub AddLog(ByVal objLog As LogInfo)

            If Not _EventLogStarted Then

                SyncLock _PendingLock
                    If Not _EventLogStarted Then
                        _LogWaitingList.Enqueue(objLog)
                    Else
                        LogTaskQueue.EnqueueTask(objLog)
                    End If
                End SyncLock
                'this is because the DNN logging provider needs a few items already in cache before it gets operational: 
                'We have to wait for those items to be cached first if we don't want to get a stackoverflow by recursion
                CheckLogStarted()
            Else
                LogTaskQueue.EnqueueTask(objLog)
            End If

        End Sub

        Public Sub DeleteLogs(ByVal objLogs As IEnumerable(Of LogInfo))

            DeleteLogTaskQueue.EnqueueTasks(objLogs)

        End Sub

        Private Shared _ExceptionBeingLogged As Boolean

        Public Sub AddException(ByVal ex As Exception)
            If _EventLogStarted Then
                If Not _ExceptionBeingLogged Then
                    _ExceptionBeingLogged = True
                    ExceptionHelper.LogException(ex)
                    _ExceptionBeingLogged = False
                End If
            Else
                Dim dnnex As New BasePortalException(ex.ToString, ex)
                Dim objLogInfo As New LogInfo()
                objLogInfo.LogTypeKey = ExceptionLogController.ExceptionLogType.GENERAL_EXCEPTION.ToString()
                objLogInfo.LogProperties.Add(New LogDetailInfo("AssemblyVersion", dnnex.AssemblyVersion))
                objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", CInt(dnnex.PortalID).ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("PortalName", dnnex.PortalName))
                objLogInfo.LogProperties.Add(New LogDetailInfo("UserID", CInt(dnnex.UserID).ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("UserName", dnnex.UserName))
                objLogInfo.LogProperties.Add(New LogDetailInfo("ActiveTabID", CInt(dnnex.ActiveTabID).ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("ActiveTabName", dnnex.ActiveTabName))
                objLogInfo.LogProperties.Add(New LogDetailInfo("RawURL", dnnex.RawURL))
                objLogInfo.LogProperties.Add(New LogDetailInfo("AbsoluteURL", dnnex.AbsoluteURL))
                objLogInfo.LogProperties.Add(New LogDetailInfo("AbsoluteURLReferrer", dnnex.AbsoluteURLReferrer))
                objLogInfo.LogProperties.Add(New LogDetailInfo("UserAgent", dnnex.UserAgent))
                objLogInfo.LogProperties.Add(New LogDetailInfo("DefaultDataProvider", dnnex.DefaultDataProvider))
                objLogInfo.LogProperties.Add(New LogDetailInfo("ExceptionGUID", dnnex.ExceptionGUID))
                objLogInfo.LogProperties.Add(New LogDetailInfo("InnerException", dnnex.InnerException.Message))
                objLogInfo.LogProperties.Add(New LogDetailInfo("FileName", dnnex.FileName))
                objLogInfo.LogProperties.Add(New LogDetailInfo("FileLineNumber", CInt(dnnex.FileLineNumber).ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("FileColumnNumber", CInt(dnnex.FileColumnNumber).ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("Method", dnnex.Method))
                objLogInfo.LogProperties.Add(New LogDetailInfo("StackTrace", ex.StackTrace))
                objLogInfo.LogProperties.Add(New LogDetailInfo("Message", ex.Message))
                objLogInfo.LogProperties.Add(New LogDetailInfo("Source", ex.Source))
                objLogInfo.LogPortalID = dnnex.PortalID

                Me.AddLog(objLogInfo)
            End If

        End Sub



        Public Sub FlushPendingLogs()
            If _LogWaitingList.Count > 0 Then
                Dim tempList As List(Of LogInfo)
                SyncLock _PendingLock
                    tempList = New List(Of LogInfo)(_LogWaitingList)
                    _LogWaitingList.Clear()
                End SyncLock

                LogTaskQueue.EnqueueTasks(tempList)
            End If
        End Sub

        Private Shared _LoggingProgessing As Boolean


        Private Sub LogSlowly(ByVal objLog As LogInfo)
            If Not _LoggingProgessing Then
                _LoggingProgessing = True

                NukeHelper.LogController.AddLog(objLog)
                _LoggingProgessing = False
            Else : Me.AddLog(objLog)
            End If
        End Sub

        Private Sub DeleteLogSlowly(ByVal objLog As LogInfo)

            NukeHelper.LogController.DeleteLog(objLog)

        End Sub


        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    If Me._DeleteLogTaskQueue IsNot Nothing Then
                        Me._DeleteLogTaskQueue.Dispose()
                    End If
                    If Me._LogTaskQueue IsNot Nothing Then
                        Me._LogTaskQueue.Dispose()
                    End If
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
