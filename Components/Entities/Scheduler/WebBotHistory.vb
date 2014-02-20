Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.Constants
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Archive, IconOptions.Normal)> _
    <Serializable()> _
    Public Class WebBotHistory

#Region "Private members"


        Private _NumberOfBotCall As Integer


        Private _NumberOfSucceedBotCall As Integer


        Private _MinDuration As New STimeSpan(Constants.UnlimitedTimeSpan)


        Private _MaxDuration As New STimeSpan(Constants.UnlimitedTimeSpan)


        Private _AverageDuration As New STimeSpan(Constants.UnlimitedTimeSpan)

        Private _LastRun As DateTime = DateTime.MinValue

        Private _BotCallHistory As New List(Of BotInfoEvent)

#End Region


#Region "Public Properties"

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
                If _NumberOfSucceedBotCall > 2 Then
                    Dim totalTime As New TimeSpan(_AverageDuration.Value.Ticks * _NumberOfSucceedBotCall)
                    Dim avgTimeWOExtrems As TimeSpan = TimeSpan.FromTicks((totalTime.Ticks - _MinDuration.Value.Ticks - _MaxDuration.Value.Ticks) \ (_NumberOfSucceedBotCall - 2))
                    Return Aricie.Common.FormatTimeSpan(avgTimeWOExtrems, True)
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





    End Class


End Namespace
