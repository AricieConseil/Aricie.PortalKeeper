Imports Aricie.DNN.Services.Workers
Imports DotNetNuke.Services.Log.EventLog
Imports Aricie.DNN.Services
Imports Aricie.Services
Imports System.Xml
Imports System.Globalization

Namespace Diagnostics

    ''' <summary>
    ''' Generic definition for a base Logger class
    ''' </summary>
    ''' <remarks>Handles its own stopwatch for precise measurements</remarks>
    Public MustInherit Class DebugLoggerBase(Of T As DebugInfo)
        Implements IDisposable


        Private Const glbSerializedPropertyName As String = "Serialized"
        Public Const glbDebugTypePropertyName As String = "From"

        Private Shared _StopWatch As Stopwatch
        Private Shared _StopWatchStartTime As DateTime


        Private Shared Sub StartStopWatch()
            _StopWatch = New Stopwatch()
            _StopWatchStartTime = DateTime.Now
            _StopWatch.Start()
        End Sub

        Protected Shared ReadOnly Property StopWatchStartTime() As DateTime
            Get
                If _StopWatch Is Nothing Then
                    StartStopWatch()
                End If
                Return _StopWatchStartTime
            End Get
        End Property

        Protected Shared ReadOnly Property StopWatch() As Stopwatch
            Get
                If _StopWatch Is Nothing Then
                    StartStopWatch()
                End If
                Return _StopWatch
            End Get
        End Property

        Public Shared ReadOnly Property Now As DateTime
            Get
                Return StopWatchStartTime.Add(StopWatch.Elapsed)
            End Get
        End Property

        Public Function GetTimeStamp() As DateTime
            Return StopWatchStartTime.Add(StopWatch.Elapsed)
        End Function

        Public Const glbDnnLogTypeKey As String = "DEBUG"

        Private _LogTaskQueueNoSerialize As TaskQueue(Of T)
        Private _LogTaskQueueSerialize As TaskQueue(Of T)


        Protected Overridable ReadOnly Property AddLogTypeAndName() As Boolean
            Get
                Return True
            End Get
        End Property


        Private ReadOnly Property LogTaskQueueNoSerialize() As TaskQueue(Of T)
            Get
                If _LogTaskQueueNoSerialize Is Nothing Then

                    Dim objTaskInfo As New TaskQueueInfo(1, True, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(9))
#If (Debug) Then
                    objTaskInfo.EnablePerformanceCounters = True
                    objTaskInfo.PerformanceCounterInstanceName &= "-DebugLogger-NoSerialize"
#End If
                    _LogTaskQueueNoSerialize = New TaskQueue(Of T)(New Action(Of T)(AddressOf ProcessDebugInfoNoSerialize), objTaskInfo)
                End If
                Return _LogTaskQueueNoSerialize
            End Get
        End Property

        Private ReadOnly Property LogTaskQueueSerialize() As TaskQueue(Of T)
            Get
                If _LogTaskQueueSerialize Is Nothing Then

                    Dim objTaskInfo As New TaskQueueInfo(1, True, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(9))
#If (Debug) Then
                    objTaskInfo.EnablePerformanceCounters = True
                    objTaskInfo.PerformanceCounterInstanceName &= "-DebugLogger-Serialize"
#End If
                    _LogTaskQueueSerialize = New TaskQueue(Of T)(New Action(Of T)(AddressOf ProcessDebugInfoSerialize), objTaskInfo)

                End If
                Return _LogTaskQueueSerialize
            End Get
        End Property

#Region "Public methods"

       


        Public Sub AddDebugInfo(ByVal objToLog As T)
            Me.AddDebugInfo(objToLog, False)

        End Sub

        ''' <summary>
        ''' Queues a log addition
        ''' </summary>
        ''' <param name="objToLog">the generic object to log</param>
        ''' <param name="serializeObject">a boolean to determine if the object should be included serialized in the wrapping DNN log event</param>
        ''' <remarks></remarks>
        Public Overridable Sub AddDebugInfo(ByVal objToLog As T, ByVal serializeObject As Boolean)

            AsyncLogger.Instance.CheckLogStarted()
            If serializeObject Then
                LogTaskQueueSerialize.EnqueueTask(objToLog)
            Else
                LogTaskQueueNoSerialize.EnqueueTask(objToLog)
            End If

        End Sub

        Public Function GetLogs(ByVal portalId As Integer, ByVal debugType As String, ByVal minDateTime As DateTime, ByVal clearLogs As Boolean, ByVal deleteLogs As Boolean) As List(Of T)

            Dim toReturn As New List(Of T)

            Dim nbRecords As Integer
            Dim objEventLogs As LogInfoArray = Nothing
            Try
                If portalId <> -1 Then
                    objEventLogs = NukeHelper.LogController.GetLog(portalId, glbDnnLogTypeKey, 10, 0, nbRecords)
                    objEventLogs = NukeHelper.LogController.GetLog(portalId, glbDnnLogTypeKey, nbRecords, 1, nbRecords)
                Else
                    objEventLogs = NukeHelper.LogController.GetLog(glbDnnLogTypeKey, 10, 1, nbRecords)
                    objEventLogs = NukeHelper.LogController.GetLog(glbDnnLogTypeKey, nbRecords, 0, nbRecords)
                End If
                Dim isDebugType As Boolean
                Dim serializeProp As LogDetailInfo

                For Each objLog As LogInfo In objEventLogs
                    If objLog.LogCreateDate > minDateTime Then
                        isDebugType = False
                        serializeProp = Nothing
                        For Each objProperty As LogDetailInfo In objLog.LogProperties
                            Select Case objProperty.PropertyName
                                Case glbDebugTypePropertyName
                                    If objProperty.PropertyValue = debugType Then
                                        isDebugType = True
                                    End If
                                Case glbSerializedPropertyName
                                    serializeProp = objProperty
                                Case Else
                                    Continue For
                            End Select
                        Next
                        If isDebugType AndAlso serializeProp IsNot Nothing Then
                            Try
                                Dim obj As T = ReflectionHelper.Deserialize(Of T)(serializeProp.PropertyValue)
                                toReturn.Add(obj)
                            Catch ex As Exception
                                ExceptionHelper.LogException(ex)
                            End Try

                        End If
                    End If
                Next
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            Finally
                If clearLogs Then
                    Try
                        NukeHelper.LogController.ClearLog()
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                        Try
                            DotNetNuke.Data.DataProvider.Instance.ExecuteScript("TRUNCATE TABLE  {databaseOwner}[{objectQualifier}EventLog]")
                        Catch ex
                            ExceptionHelper.LogException(ex)
                        End Try
                    End Try

                    'AsyncWorker.Instance.AsyncRun(AddressOf NukeHelper.LogController.ClearLog, True)
                Else
                    If objEventLogs IsNot Nothing Then
                        If objEventLogs.Count > 0 AndAlso deleteLogs Then
                            Dim arrLogs(objEventLogs.Count) As LogInfo
                            objEventLogs.CopyTo(arrLogs, 0)
                            AsyncLogger.Instance.DeleteLogs(arrLogs)
                        End If
                    Else
                        Try
                            NukeHelper.LogController.ClearLog()
                        Catch ex As Exception
                            ExceptionHelper.LogException(ex)
                            Try
                                DotNetNuke.Data.DataProvider.Instance.ExecuteScript("TRUNCATE TABLE  {databaseOwner}[{objectQualifier}EventLog]")
                            Catch ex
                                ExceptionHelper.LogException(ex)
                            End Try
                        End Try
                    End If

                End If
            End Try


            Return toReturn

        End Function



#End Region

#Region "Private methods"

        Private Sub ProcessDebugInfoSerialize(ByVal objToLog As T)
            ProcessDebugInfo(objToLog, True)
        End Sub

        Private Sub ProcessDebugInfoNoSerialize(ByVal objToLog As T)
            ProcessDebugInfo(objToLog, False)
        End Sub



        Private Sub ProcessDebugInfo(ByVal objToLog As T, ByVal serialize As Boolean)
            Dim objEventLogInfo As New LogInfo()
            Try
                Dim startTimeStamp As DateTime = Me.GetTimeStamp

                objEventLogInfo.LogTypeKey = glbDnnLogTypeKey

                If objToLog.PortalId <> -1 Then
                    objEventLogInfo.LogPortalID = objToLog.PortalId
                End If

                If AddLogTypeAndName Then
                    objEventLogInfo.AddProperty(glbDebugTypePropertyName, objToLog.DebugType)
                    objEventLogInfo.AddProperty("Name", objToLog.Name)
                End If


                Dim discardLog As Boolean
                Dim previousLogs As IList(Of LogInfo) = New List(Of LogInfo)
                Me.FillLogObject(objToLog, objEventLogInfo, discardLog, previousLogs)

                If serialize Then
                    Try
                        Dim doc As XmlDocument = ReflectionHelper.Serialize(objToLog)
                        objEventLogInfo.LogProperties.Add(New LogDetailInfo(glbSerializedPropertyName, doc.OuterXml))
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try

                End If


                For Each objLog As LogInfo In previousLogs
                    AsyncLogger.Instance.AddLog(objLog)
                Next

                If Not discardLog Then

                    objEventLogInfo.AddProperty("Debug Processing TimeStamp", startTimeStamp.ToString("yyyy, dddd, MMMM dd, h:mm:ss.FFFFFFF"))
                    objEventLogInfo.AddProperty("Debug Processing Duration", FormatTimeSpan(Me.GetTimeStamp.Subtract(startTimeStamp)))
                    AsyncLogger.Instance.AddLog(objEventLogInfo)
                End If
            Catch ex As Exception
                objEventLogInfo.LogProperties.Add(New LogDetailInfo("Logger Exception", ex.ToString))
                Try
                    AsyncLogger.Instance.AddLog(objEventLogInfo)
                Catch ex2 As Exception

                End Try
            End Try


        End Sub

        Protected Overridable Sub FillLogObject(ByVal objToLog As T, ByRef objEventLogInfo As LogInfo, ByRef discardLog As Boolean, ByRef previousLogs As IList(Of LogInfo))

            If objToLog.Description <> "" Then
                objEventLogInfo.LogProperties.Add(New LogDetailInfo("Description", objToLog.Description))
            End If
            If objToLog.AdditionalProperties IsNot Nothing Then
                For Each objPair As KeyValuePair(Of String, String) In objToLog.AdditionalProperties
                    If Not String.IsNullOrEmpty(objPair.Key) Then
                        objEventLogInfo.LogProperties.Add(New LogDetailInfo(objPair.Key.ToString, objPair.Value.ToString))
                    Else
                        Dim test As T = objToLog
                    End If

                Next
            End If
            If objToLog.MemoryUsage Then
                Dim totalMemory As String = CInt(Math.Round(GC.GetTotalMemory(False) / 1048576)).ToString() & "Mo"
                objEventLogInfo.AddProperty("Memory", totalMemory)
            End If
            objEventLogInfo.AddProperty("Thread Id", objToLog.ThreadId)
            objEventLogInfo.AddProperty("Thread Culture", objToLog.ThreadCulture)
            objEventLogInfo.AddProperty("AppDomain Id", AppDomain.CurrentDomain.Id.ToString(CultureInfo.InvariantCulture))
            objEventLogInfo.AddProperty("Process Id", Process.GetCurrentProcess.Id.ToString(CultureInfo.InvariantCulture))
        End Sub


#End Region





        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    If Me._LogTaskQueueNoSerialize IsNot Nothing Then
                        Me._LogTaskQueueNoSerialize.Dispose()
                    End If
                    If Me._LogTaskQueueSerialize IsNot Nothing Then
                        Me._LogTaskQueueSerialize.Dispose()
                    End If
                    If _StopWatch IsNot Nothing Then
                        _StopWatch.Stop()
                        _StopWatch = Nothing
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
