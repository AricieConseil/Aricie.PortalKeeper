Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization

Namespace Entities
    ''' <summary>
    ''' Configuration Entity class for a Web Ping engine
    ''' </summary>
    <Serializable()> _
    Public Class PingUrlInfo
        Inherits NamedConfig

        Private _PingUrl As String = ""
        Private _Schedule As New STimeSpan(TimeSpan.FromMinutes(10))
        Private _AlertDuration As New STimeSpan(TimeSpan.FromSeconds(10))
        Private _SendAlertEmails As Boolean
        Private _AlertEmail As String = String.Empty

        Private _NumberOfRuns As Integer
        Private _NumberOfSucceedPings As Integer
        Private _MinDuration As New STimeSpan(Constants.UnlimitedTimeSpan)
        Private _MaxDuration As New STimeSpan(Constants.UnlimitedTimeSpan)
        Private _AverageDuration As New STimeSpan(Constants.UnlimitedTimeSpan)
        Private _LastRun As DateTime = DateTime.MinValue
        Private _LastRunDuration As New STimeSpan(Constants.UnlimitedTimeSpan)

        <ExtendedCategory("Definition")> _
                   <Required(True)> _
           <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
           <Width(500)> _
        Public Property PingUrl() As String
            Get
                Return _PingUrl
            End Get
            Set(ByVal value As String)
                If value <> _PingUrl Then
                    _PingUrl = value
                    ResetStats()
                End If
            End Set
        End Property


        Private Sub ResetStats()
            _NumberOfRuns = 0
            _NumberOfSucceedPings = 0
            _MinDuration.Value = Constants.UnlimitedTimeSpan
            _MaxDuration.Value = Constants.UnlimitedTimeSpan
            _AverageDuration.Value = Constants.UnlimitedTimeSpan
            _LastRun = DateTime.MinValue
            _LastRunDuration.Value = Constants.UnlimitedTimeSpan
        End Sub

        <ExtendedCategory("Definition")> _
        Public Property Schedule() As STimeSpan
            Get
                Return _Schedule
            End Get
            Set(ByVal value As STimeSpan)
                _Schedule = value
            End Set
        End Property


        <ExtendedCategory("Alerts")> _
        Public Property SendAlertEmails() As Boolean
            Get
                Return _SendAlertEmails
            End Get
            Set(ByVal value As Boolean)
                _SendAlertEmails = value
            End Set
        End Property

        <ExtendedCategory("Alerts")> _
        <ConditionalVisible("SendAlertEmails", False, True)> _
        Public Property AlertDuration() As STimeSpan
            Get
                Return _AlertDuration
            End Get
            Set(ByVal value As STimeSpan)
                _AlertDuration = value
            End Set
        End Property


        <ExtendedCategory("Alerts")> _
        <ConditionalVisible("SendAlertEmails", False, True)> _
        <RegularExpressionValidator(Aricie.Constants.Content.EmailValidator)> _
        <Required(True)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
           <Width(300)> _
        Public Property AlertEmail() As String
            Get
                Return _AlertEmail
            End Get
            Set(ByVal value As String)
                _AlertEmail = value
            End Set
        End Property






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

        <Browsable(False)> _
        Public Property LastRunDuration() As STimeSpan
            Get
                Return _LastRunDuration
            End Get
            Set(ByVal value As STimeSpan)
                _LastRunDuration = value
            End Set
        End Property

        <XmlIgnore()> _
       <ExtendedCategory("Stats")> _
        Public ReadOnly Property LastRunDurationString() As String
            Get
                Return Aricie.Common.FormatTimeSpan(Me._LastRunDuration.Value, True)
            End Get
        End Property


        <IsReadOnly(True)> _
        <ExtendedCategory("Stats")> _
        Public Property NumberOfPings() As Integer
            Get
                Return _NumberOfRuns
            End Get
            Set(ByVal value As Integer)
                _NumberOfRuns = value
            End Set
        End Property

        <IsReadOnly(True)> _
        <ExtendedCategory("Stats")> _
        Public Property NumberOfSucceedPings() As Integer
            Get
                Return _NumberOfSucceedPings
            End Get
            Set(ByVal value As Integer)
                _NumberOfSucceedPings = value
            End Set
        End Property

        <ExtendedCategory("Stats")> _
        Public ReadOnly Property NumberOfFailedPings() As Integer
            Get
                Return _NumberOfRuns - _NumberOfSucceedPings
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
                Return Aricie.Common.FormatTimeSpan(Me._MinDuration.Value, True)
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
                Return Aricie.Common.FormatTimeSpan(Me._MaxDuration.Value, True)
            End Get
        End Property

        <Browsable(False)> _
        Public Property AverageDuration() As STimeSpan
            Get
                Return _AverageDuration
            End Get
            Set(ByVal value As STimeSpan)
                _AverageDuration = value
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property AverageDurationString() As String
            Get
                Return Aricie.Common.FormatTimeSpan(Me._AverageDuration.Value, True)
            End Get
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Stats")> _
        Public ReadOnly Property AverageDurationWOExtremsString() As String
            Get
                If _NumberOfSucceedPings > 2 Then
                    Dim totalTime As New TimeSpan(_AverageDuration.Ticks * _NumberOfSucceedPings)
                    Dim avgTimeWOExtrems As New TimeSpan(CInt(Math.Abs((totalTime.Ticks - (_MinDuration.Ticks + _MaxDuration.Ticks)) / (_NumberOfSucceedPings - 2))))
                    Return Aricie.Common.FormatTimeSpan(avgTimeWOExtrems, True)
                Else
                    Return AverageDurationString
                End If
            End Get
        End Property


        Public Sub UpdateStats(success As Boolean, duration As TimeSpan)
            Me._LastRun = Now
            Me._NumberOfRuns += 1
            If success Then
                Me._LastRunDuration.Value = duration
                Me._NumberOfSucceedPings += 1
                If Me._NumberOfSucceedPings > 1 Then
                    Dim totalWebBotDuration As TimeSpan = TimeSpan.FromTicks(Me._AverageDuration.Value.Ticks * (Me._NumberOfSucceedPings - 1))
                    totalWebBotDuration = totalWebBotDuration.Add(duration)
                    Me._AverageDuration.Value = TimeSpan.FromTicks(totalWebBotDuration.Ticks \ Me._NumberOfSucceedPings)
                Else
                    Me._AverageDuration.Value = duration
                End If
                If duration < Me._MinDuration Then
                    Me._MinDuration.Value = duration
                End If
                If duration > Me._MaxDuration OrElse Me._MaxDuration = TimeSpan.MaxValue Then
                    Me._MaxDuration.Value = duration
                End If
            Else
                Me._LastRunDuration.Value = Constants.UnlimitedTimeSpan
            End If
        End Sub


    End Class




End Namespace