Imports DotNetNuke.Services.Log.EventLog
Imports Aricie.Services
Imports System.Globalization

Namespace Diagnostics



    ''' <summary>
    ''' Lightweight structure to handle quantities of aggregations
    ''' </summary>
    Public Structure CumulatedStepLog

        Public Sub New(ByVal objLog As LogInfo, ByVal objDuration As TimeSpan, ByVal stepNb As Integer)
            Me.Log = objLog
            Me.CumulatedDuration = objDuration
            Me.StepNb = stepNb
            Me.NbRepeats = 1
        End Sub

        Public Log As LogInfo
        Public CumulatedDuration As TimeSpan
        Public StepNb As Integer
        Public NbRepeats As Integer

    End Structure

    Public Structure TimingCounter

        Public Sub New(ByVal startTime As TimeSpan)
            Me.StartTime = startTime
            Me.StepNumber = 1
            Me.LastStepWorkingPhase = WorkingPhase.InProgress
            Me.ExternalLoadTime = TimeSpan.Zero
            Me.LastStepTime = TimeSpan.Zero
            Me.CumulatedLogs = New Dictionary(Of String, CumulatedStepLog)
        End Sub

        Public StartTime As TimeSpan

        Public ExternalLoadTime As TimeSpan

        Public LastStepTime As TimeSpan

        Public LastStepWorkingPhase As WorkingPhase

        Public StepNumber As Integer

        Public CumulatedLogs As Dictionary(Of String, CumulatedStepLog)


    End Structure




    ''' <summary>
    ''' Precise performance based logger
    ''' </summary>
    Public Class PerformanceLogger
        Inherits DebugLoggerBase(Of StepInfo)

        Private _PerfTimers As New Dictionary(Of String, TimingCounter)


#Region "cTors"

        Public Shared Function Instance() As PerformanceLogger
            Return ReflectionHelper.GetSingleton(Of PerformanceLogger)()
        End Function

#End Region

       


        Protected Overrides ReadOnly Property AddLogTypeAndName() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Sub AddDebugInfo(ByVal objToLog As StepInfo, ByVal serialize As Boolean)
            If Me._DisabledLogs.Count = 0 OrElse Not Me._DisabledLogs.Contains(objToLog.FlowId) Then
                objToLog.StopElapsed = StopWatch.Elapsed
                If Me._AgregatedLogs.Count > 0 AndAlso Me._AgregatedLogs.Contains(objToLog.FlowId) Then
                    objToLog.Cumulated = True
                End If
                MyBase.AddDebugInfo(objToLog, serialize)
            End If
        End Sub

        Private _DisabledLogs As New HashSet(Of String)
        Private _AgregatedLogs As New HashSet(Of String)


        Public Sub AgregateLogs(id As String)
            SyncLock _AgregatedLogs
                _AgregatedLogs.Add(id)
            End SyncLock
        End Sub

        Public Sub DisableAregates(id As String)
            SyncLock _AgregatedLogs
                _AgregatedLogs.Remove(id)
            End SyncLock
        End Sub

        Public Sub DisableLog(id As String)
            SyncLock _DisabledLogs
                _DisabledLogs.Add(id)
            End SyncLock
        End Sub

        Public Sub EnableLog(id As String)
            SyncLock _DisabledLogs
                _DisabledLogs.Remove(id)
            End SyncLock
        End Sub


        Protected Overrides Sub FillLogObject(ByVal objToLog As StepInfo, ByRef objEventLogInfo As LogInfo, _
                                              ByRef discardLog As Boolean, ByRef previousLogs As IList(Of LogInfo))
            Dim key As String = objToLog.DebugType
            If Not String.IsNullOrEmpty(objToLog.FlowId) Then
                key &= objToLog.FlowId
            Else
                key &= objToLog.ThreadId
            End If

            Dim currentPerfTimer As TimingCounter = Nothing

            Dim timerNotFound As Boolean
            SyncLock _PerfTimers
                timerNotFound = Not _PerfTimers.TryGetValue(key, currentPerfTimer)
            End SyncLock

            If timerNotFound Then
                currentPerfTimer = New TimingCounter(objToLog.StopElapsed)
                objToLog.IsNew = True
            Else
                objToLog.Elapsed = objToLog.StopElapsed.Subtract(currentPerfTimer.LastStepTime)
                objToLog.CumulatedElapsed = objToLog.Elapsed

                If Not objToLog.Cumulated Then
                    currentPerfTimer.StepNumber += 1
                    If currentPerfTimer.CumulatedLogs.Count > 0 Then
                        SyncLock DirectCast(currentPerfTimer.CumulatedLogs, ICollection).SyncRoot
                            For Each objCumulatedLog As CumulatedStepLog In currentPerfTimer.CumulatedLogs.Values
                                previousLogs.Add(objCumulatedLog.Log)
                            Next
                        End SyncLock

                        currentPerfTimer.CumulatedLogs.Clear()
                    End If
                Else

                    Dim toAdd As CumulatedStepLog = Nothing

                    discardLog = True
                    SyncLock DirectCast(currentPerfTimer.CumulatedLogs, ICollection).SyncRoot
                        If currentPerfTimer.CumulatedLogs.TryGetValue(objToLog.Name, toAdd) Then
                            objToLog.CumulatedElapsed = objToLog.Elapsed.Add(toAdd.CumulatedDuration)
                            toAdd.CumulatedDuration = objToLog.CumulatedElapsed
                            toAdd.NbRepeats = toAdd.NbRepeats + 1
                            toAdd.Log = objEventLogInfo
                            objToLog.StepNumber = toAdd.StepNb
                        Else
                            currentPerfTimer.StepNumber += 1
                            toAdd = New CumulatedStepLog(objEventLogInfo, objToLog.CumulatedElapsed, currentPerfTimer.StepNumber)
                        End If
                        objToLog.NbCumulatedSteps = toAdd.NbRepeats
                        currentPerfTimer.CumulatedLogs(objToLog.Name) = toAdd
                    End SyncLock

                End If
            End If

            If objToLog.StepNumber = -1 Then
                objToLog.StepNumber = currentPerfTimer.StepNumber
            End If
            objToLog.FlowStartTime = StopWatchStartTime.Add(currentPerfTimer.StartTime)

            objEventLogInfo.AddProperty("Step", objToLog.StepNumber.ToString(CultureInfo.InvariantCulture))

            objEventLogInfo.AddProperty("Name", objToLog.Name)

            objEventLogInfo.AddProperty(glbDebugTypePropertyName, objToLog.DebugType)

            objEventLogInfo.AddProperty("Last Step", objToLog.IsLastStep.ToString)

            objEventLogInfo.AddProperty("Aggregated Step", objToLog.Cumulated.ToString(CultureInfo.InvariantCulture))

            Dim totalElapsed As TimeSpan = TimeSpan.Zero
            If Not objToLog.IsNew Then

                totalElapsed = objToLog.StopElapsed.Subtract(currentPerfTimer.StartTime)


                Dim stepDurationPropName As String = "Step Duration"

                If currentPerfTimer.LastStepWorkingPhase = WorkingPhase.EndOverhead Then
                    stepDurationPropName = "External Code Duration"
                    currentPerfTimer.ExternalLoadTime = currentPerfTimer.ExternalLoadTime.Add(objToLog.Elapsed)
                End If



                If objToLog.Cumulated Then
                    objEventLogInfo.AddProperty("Nb Occurence Step", objToLog.NbCumulatedSteps.ToString)
                    stepDurationPropName = "Aggregated " & stepDurationPropName
                End If


                objEventLogInfo.AddProperty(stepDurationPropName, FormatTimeSpan(objToLog.CumulatedElapsed))

                If Not objToLog.Cumulated Then

                    objEventLogInfo.AddProperty("Total Inner Code Duration", FormatTimeSpan(totalElapsed.Subtract(currentPerfTimer.ExternalLoadTime)))
                    objEventLogInfo.AddProperty("Total External Code Duration", FormatTimeSpan(currentPerfTimer.ExternalLoadTime))
                    objEventLogInfo.AddProperty("Total Flow Duration", FormatTimeSpan(totalElapsed))
                    objEventLogInfo.AddProperty("Inner code / Total duration", ((1 - currentPerfTimer.ExternalLoadTime.Ticks / totalElapsed.Ticks) * 100).ToString("F") & " %")

                End If


                Dim endTimeName As String = "Step End Time"
                If objToLog.IsLastStep Then
                    endTimeName = "Flow End Time"
                End If
                objEventLogInfo.AddProperty(endTimeName, StopWatchStartTime.Add(objToLog.StopElapsed).ToString("yyyy, dddd, MMMM dd, h:mm:ss.FFFFFFF"))

            End If
            objToLog.TotalElapsed = totalElapsed


            objEventLogInfo.AddProperty("Flow Start Time", objToLog.FlowStartTime.ToString("yyyy, dddd, MMMM dd, h:mm:ss.FFFFFFF"))
            objEventLogInfo.AddProperty("Total Elapsed", FormatTimeSpan(objToLog.StopElapsed))
            objEventLogInfo.AddProperty("Flow Id", objToLog.FlowId)

            'objEventLogInfo.AddProperty("Current Processing Time", Me.GetTimeStamp.ToString("yyyy, dddd, MMMM dd, h:mm:ss.FFFFFFF"))

            MyBase.FillLogObject(objToLog, objEventLogInfo, discardLog, previousLogs)



            If objToLog.IsLastStep Then
                SyncLock _PerfTimers
                    _PerfTimers.Remove(key)
                End SyncLock

            Else
                currentPerfTimer.LastStepWorkingPhase = objToLog.WorkingPhase
                currentPerfTimer.LastStepTime = objToLog.StopElapsed
                SyncLock _PerfTimers
                    _PerfTimers(key) = currentPerfTimer
                End SyncLock
            End If
        End Sub
    End Class

End Namespace
