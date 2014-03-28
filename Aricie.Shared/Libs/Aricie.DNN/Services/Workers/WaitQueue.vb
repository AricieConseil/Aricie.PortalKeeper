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
End NameSpace