Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Workers
    ''' <summary>
    ''' Information class for task queue
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class TaskQueueInfo

        Public Sub New()

        End Sub



        Public Sub New(ByVal nbThread As Integer, ByVal isBackground As Boolean, ByVal initialWaitTime As TimeSpan, ByVal wakeUpTime As TimeSpan, ByVal taskWaitTime As TimeSpan)
            Me._NbThreads = nbThread
            Me._IsBackground = isBackground
            Me._InitialWaitTime = New STimeSpan(initialWaitTime)
            Me._WakeUpWaitTime = New STimeSpan(wakeUpTime)
            Me._TaksWaitTime = New STimeSpan(taskWaitTime)
        End Sub

        ''' <summary>
        ''' Time to wait for task
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("")> _
        <MainCategory()> _
        Public Property TaksWaitTime() As New STimeSpan

        ''' <summary>
        ''' Initial waiting time for task
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("AdvancedSettings")> _
        Public Property InitialWaitTime() As New STimeSpan

        ''' <summary>
        ''' Waiting time before wake up
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("AdvancedSettings")> _
        Public Property WakeUpWaitTime() As New STimeSpan

        ''' <summary>
        ''' Number of threads that will run the task
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("AdvancedSettings")> _
        Public Property NbThreads() As Integer = 1

        ''' <summary>
        ''' Task in the background
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("AdvancedSettings")> _
        Public Property IsBackground() As Boolean = True


    End Class
End Namespace