Imports Aricie.DNN.Configuration
Imports DotNetNuke.Services.Scheduling
Imports DotNetNuke.Services.Exceptions
Imports System.Threading
Imports Aricie.DNN.Diagnostics

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class PortalKeeperSchedule
        Inherits SchedulerClient



        ' Methods
        Public Sub New(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
            Me.ScheduleHistoryItem = objScheduleHistoryItem
        End Sub
        'cette méthode est appelée automatiquement
        'dès que le module est lancé dans le planificateur de tâche. 

        Public Overrides Sub DoWork()
            Try
                'todo: deal with potential multiple appdomains firing simultaneous schedules
                Me.Progressing()

                Me.DoBotCalls()
                Me.ScheduleHistoryItem.Succeeded = True
                Me.ScheduleHistoryItem.AddLogNote("WebBots by Aricie.PortalKeeper completed.")
            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote(("WebBots by Aricie.PortalKeeper failed with the following exception. " & ex.ToString))
                LogException(ex)
            End Try
        End Sub

        'Private Shared scheduleLock As New Object
        'Private Shared waitDuration As TimeSpan = TimeSpan.FromTicks((PortalKeeperSchedule.SchedulePeriod.Ticks * 10) \ 100)

        Public Sub DoBotCalls()

            Dim schedulerFarm As BotFarmInfo(Of ScheduleEvent) = PortalKeeperConfig.Instance.SchedulerFarm
            If schedulerFarm.Enabled Then
                Dim limitTime As DateTime = Now.Add(TimeSpan.FromTicks((New TimeLapse(Me.ScheduleHistoryItem.TimeLapse, Me.ScheduleHistoryItem.TimeLapseMeasurement).ToTimeSpan.Ticks * 98) \ 100))
                Dim flowid As String = ""
                If schedulerFarm.EnableLogs Then
                    flowid = Guid.NewGuid.ToString
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "Scheduler Start", _
                                                WorkingPhase.InProgress, False, False, -1, flowid)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
                Dim nextSchedule As DateTime = DateTime.MinValue
                Dim maxRunDuration As TimeSpan = TimeSpan.Zero
                Dim loopEndTime As DateTime = DateTime.MinValue
                Dim loopStartTime As DateTime
                Do
                    If schedulerFarm.EnableLogs Then
                        Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Loop Start", _
                                                    WorkingPhase.InProgress, False, False, -1, flowid)
                        PerformanceLogger.Instance.AddDebugInfo(objStep)
                    End If
                    loopStartTime = Now
                    If loopStartTime >= nextSchedule Then
                        nextSchedule = loopStartTime.Add(schedulerFarm.Schedule.Value)
                        schedulerFarm.RunBots(ScheduleEventList, False, flowid)
                        loopEndTime = Now
                        maxRunDuration = TimeSpan.FromTicks(Math.Max(maxRunDuration.Ticks, loopEndTime.Subtract(loopStartTime).Ticks))

                    Else
                        If schedulerFarm.EnableLogs Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Sleep Start", _
                                                        WorkingPhase.EndOverhead, False, False, -1, flowid)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        Thread.Sleep(TimeSpan.FromTicks(Math.Min(nextSchedule.Subtract(loopStartTime).Ticks, limitTime.Subtract(loopStartTime).Ticks)))
                        If schedulerFarm.EnableLogs Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Sleep End", _
                                                        WorkingPhase.InProgress, False, False, -1, flowid)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        loopEndTime = Now
                    End If

                    If schedulerFarm.EnableLogs Then
                        Dim loopStartLog As New KeyValuePair(Of String, String)("Loop Start", loopStartTime.ToString)
                        Dim scheduleValue As New KeyValuePair(Of String, String)("Schedule Duration", schedulerFarm.Schedule.Value.ToString)
                        Dim nextScheduleLog As New KeyValuePair(Of String, String)("Next Schedule", nextSchedule.ToString)
                        Dim loopEndLog As New KeyValuePair(Of String, String)("Loop End", loopEndTime.ToString)
                        Dim maxDurationLog As New KeyValuePair(Of String, String)("Max Run Duration", maxRunDuration.ToString)
                        Dim limitTimeLog As New KeyValuePair(Of String, String)("Limit Time", limitTime.ToString)
                        Dim objStep As New StepInfo(Debug.PKPDebugType, "Farm Loop End", _
                                                    WorkingPhase.InProgress, False, False, -1, flowid, loopStartLog, loopEndLog, nextScheduleLog, maxDurationLog, limitTimeLog)
                        PerformanceLogger.Instance.AddDebugInfo(objStep)
                    End If
                Loop Until (nextSchedule.Add(maxRunDuration) > limitTime) _
                            OrElse (loopEndTime.Add(maxRunDuration) > limitTime)
                If schedulerFarm.EnableLogs Then
                    Dim objStep As New StepInfo(Debug.PKPDebugType, "Scheduler End", _
                                                WorkingPhase.InProgress, True, False, -1, flowid)
                    PerformanceLogger.Instance.AddDebugInfo(objStep)
                End If
            End If

        End Sub

        Private Shared _ScheduleEventList As New List(Of ScheduleEvent)

        Public Shared ReadOnly Property ScheduleEventList() As List(Of ScheduleEvent)
            Get
                If _ScheduleEventList.Count = 0 Then
                    SyncLock _ScheduleEventList
                        If _ScheduleEventList.Count = 0 Then
                            _ScheduleEventList.Add(ScheduleEvent.Init)
                            _ScheduleEventList.Add(ScheduleEvent.Run1)
                            '_ScheduleEventList.Add(ScheduleEvent.Default)
                            _ScheduleEventList.Add(ScheduleEvent.Run2)
                            _ScheduleEventList.Add(ScheduleEvent.Run3)
                            _ScheduleEventList.Add(ScheduleEvent.Unload)
                        End If
                    End SyncLock
                End If
                Return _ScheduleEventList
            End Get
        End Property


    End Class
End Namespace


