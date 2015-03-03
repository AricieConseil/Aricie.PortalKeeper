Imports Aricie.DNN.Services.Workers
Imports Aricie.ComponentModel
Imports Aricie.Services
Imports Aricie.DNN.Settings

Namespace Diagnostics
    ''' <summary>
    ''' Asynchronous generic file based logger
    ''' </summary>
    ''' <remarks>Monitors a list of entities to be Xml Serialized</remarks>
    Public Class ExternalFileAsyncLogger(Of T)

        Public Shared Function Instance() As ExternalFileAsyncLogger(Of T)
            Return ReflectionHelper.GetSingleton(Of ExternalFileAsyncLogger(Of T))()
        End Function

        Private _LogTaskQueue As TaskQueue(Of ExternalFileLog(Of T))
        Private _LogLists As New Dictionary(Of String, Predicate(Of T))


        Private ReadOnly Property LogTaskQueue() As TaskQueue(Of ExternalFileLog(Of T))
            Get
                If _LogTaskQueue Is Nothing Then
                    _LogTaskQueue = New TaskQueue(Of ExternalFileLog(Of T))(New Action(Of ExternalFileLog(Of T))(AddressOf UpdateLogList), 1, True, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10))
                    AddHandler _LogTaskQueue.ActionPerformed, AddressOf Me._PersisterTaskQueue_ActionPerformed
                End If
                Return _LogTaskQueue
            End Get
        End Property

        Friend Sub _PersisterTaskQueue_ActionPerformed(ByVal sender As Object, ByVal e As GenericEventArgs(Of Integer))
            'no more items to add?
            If e.Item = 0 Then
                Me.InternalPerformSaves()
            End If

        End Sub

        Public Sub AddLog(ByVal objLog As ExternalFileLog(Of T))

            Me.LogTaskQueue.EnqueueTask(objLog)
        End Sub

        Private Sub InternalPerformSaves()

            For Each entry As KeyValuePair(Of String, Predicate(Of T)) In _LogLists
                Try

                    Dim logList As List(Of T) = GetCurrentLogList(entry.Key)

                    If entry.Value IsNot Nothing Then
                        logList = logList.FindAll(entry.Value)
                    End If

                    SaveFileSettings(Of List(Of T))(entry.Key, logList)

                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                End Try
            Next

        End Sub

        Private Sub UpdateLogList(ByVal objLog As ExternalFileLog(Of T))
            Try

                Dim logList As List(Of T) = GetCurrentLogList(objLog.LogFileMapPath)
                logList.Add(objLog.LogValue)
                Me._LogLists(objLog.LogFileMapPath) = objLog.PurgeFilter

            Catch ex As Exception
                ExceptionHelper.LogException(ex)
            End Try
        End Sub


        Public Shared Function GetCurrentLogList(ByVal objLogFileMapPath As String) As List(Of T)


            Return LoadFileSettings(Of List(Of T))(objLogFileMapPath, True)

        End Function



    End Class
End Namespace