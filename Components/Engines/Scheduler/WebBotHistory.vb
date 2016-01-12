Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.Constants
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Archive, IconOptions.Normal)> _
    Public Class WebBotHistory

#Region "Private members"


        Private _NumberOfBotCall As Integer


        Private _NumberOfSucceedBotCall As Integer


        Private _MinDuration As New STimeSpan(UnlimitedTimeSpan)


        Private _MaxDuration As New STimeSpan(UnlimitedTimeSpan)



        Private _LastRun As DateTime = DateTime.MinValue

        Private _BotCallHistory As New List(Of BotInfoEvent)

#End Region




#Region "Public Properties"

        Public Property NextSchedule As Nullable(Of DateTime)


        <IsReadOnly(True)> _
        <ExtendedCategory("Stats")> _
        Public Property LastRun() As Date
            Get
                Return _LastRun
            End Get
            Set(ByVal value As Date)
                _LastRun = value
            End Set
        End Property

        <IsReadOnly(True)> _
        <ExtendedCategory("Stats")> _
        Public Property NumberOfBotCall() As Integer
            Get
                Return _NumberOfBotCall
            End Get
            Set(ByVal value As Integer)
                _NumberOfBotCall = value
            End Set
        End Property

        <IsReadOnly(True)> _
        <ExtendedCategory("Stats")> _
        Public Property NumberOfSucceedBotCall() As Integer
            Get
                Return _NumberOfSucceedBotCall
            End Get
            Set(ByVal value As Integer)
                _NumberOfSucceedBotCall = value
            End Set
        End Property

        <ExtendedCategory("Stats")> _
        Public ReadOnly Property NumberOfFailedBotCall() As Integer
            Get
                Return _NumberOfBotCall - _NumberOfSucceedBotCall
            End Get
        End Property

        <Browsable(False)> _
        Public Property TotalDuration() As New STimeSpan(TimeSpan.Zero)


        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property TotalDurationString() As String
            Get
                Return FormatTimeSpan(Me.TotalDuration.Value, True)
            End Get
        End Property

        <Browsable(False)> _
        Public Property MinDuration() As STimeSpan
            Get
                Return _MinDuration
            End Get
            Set(ByVal value As STimeSpan)
                _MinDuration = value
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property MinDurationString() As String
            Get
                Return FormatTimeSpan(Me._MinDuration.Value, True)
            End Get
        End Property

        <Browsable(False)> _
        Public Property MaxDuration() As STimeSpan
            Get
                Return _MaxDuration
            End Get
            Set(ByVal value As STimeSpan)
                _MaxDuration = value
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property MaxDurationString() As String
            Get
                Return FormatTimeSpan(Me._MaxDuration.Value, True)
            End Get
        End Property



        <Browsable(False)> _
        Public ReadOnly Property AverageDuration() As TimeSpan
            Get
                If Me.NumberOfSucceedBotCall > 0 Then
                    Return TimeSpan.FromTicks(Me.TotalDuration.Value.Ticks \ Me.NumberOfSucceedBotCall)
                End If
                Return TimeSpan.Zero
            End Get
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property AverageDurationString() As String
            Get
                Return FormatTimeSpan(AverageDuration, True)
            End Get
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property AverageDurationWOExtremsString() As String
            Get
                If _NumberOfSucceedBotCall > 2 Then
                    Dim totalTime As New TimeSpan(AverageDuration.Ticks * _NumberOfSucceedBotCall)
                    Dim avgTimeWoExtrems As TimeSpan = TimeSpan.FromTicks((totalTime.Ticks - _MinDuration.Value.Ticks - _MaxDuration.Value.Ticks) \ (_NumberOfSucceedBotCall - 2))
                    Return FormatTimeSpan(avgTimeWoExtrems, True)
                Else
                    Return AverageDurationString
                End If
            End Get
        End Property

        


        <IsReadOnly(True)> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        <ExtendedCategory("CallHistory")> _
        <CollectionEditor(True, False, False, True, 10, CollectionDisplayStyle.Accordion)> _
        Public Property BotCallHistory() As List(Of BotInfoEvent)
            Get
                Return _BotCallHistory
            End Get
            Set(ByVal value As List(Of BotInfoEvent))
                _BotCallHistory = value
            End Set
        End Property




#End Region


        Public Function GetNextSchedule(objSchedule As ScheduleInfo) As DateTime
            If Me.NextSchedule.HasValue Then
                Return NextSchedule.Value
            Else
                Return objSchedule.GetNextSchedule(Me.LastRun)
            End If
        End Function

        Public Sub Increment(currentWebBotEvent As BotInfoEvent, maxNb As Integer)

            Me.LastRun = currentWebBotEvent.Time
            Me.NextSchedule = currentWebBotEvent.NextSchedule

            If Me.BotCallHistory.Count > maxNb Then
                Me.BotCallHistory.RemoveRange(maxNb, Me.BotCallHistory.Count - maxNb)
            End If

            Me.BotCallHistory.Insert(0, currentWebBotEvent)

            Me.NumberOfBotCall += 1
            If currentWebBotEvent.Success Then
                Me.NumberOfSucceedBotCall += 1
                Me.TotalDuration += currentWebBotEvent.Duration
                If currentWebBotEvent.Duration < Me.MinDuration Then
                    Me.MinDuration.Value = currentWebBotEvent.Duration
                End If
                If currentWebBotEvent.Duration > Me.MaxDuration OrElse Me.MaxDuration = TimeSpan.MaxValue Then
                    Me.MaxDuration.Value = currentWebBotEvent.Duration
                End If
            End If

        End Sub



    End Class


End Namespace
