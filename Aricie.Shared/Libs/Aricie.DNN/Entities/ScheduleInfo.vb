Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.WebControls
Imports System.ComponentModel
Imports System.Text
Imports System.Globalization
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization

Namespace Entities
    <DefaultProperty("FormattedValueShort")> _
    <ActionButton(IconName.Calendar, IconOptions.Normal)> _
    <Serializable()> _
    Public Class ScheduleInfo

        Public Property ScheduleType As ScheduleType

        Public ReadOnly Property FormattedValue As String
            Get
                Return GetFormattedValue(False)
            End Get
        End Property

        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        Public Property YearMode As FixedTimeMode = FixedTimeMode.Every

        <ExtendedCategory("Config")> _
        <AutoPostBack()> _
         <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        <ConditionalVisible("YearMode", False, True, FixedTimeMode.Specifics)> _
        Public Property SpecificYears As New List(Of Integer)
       
        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        Public Property MonthMode As FixedTimeMode = FixedTimeMode.Every

        <ExtendedCategory("Config")> _
        <AutoPostBack()> _
         <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        <ConditionalVisible("MonthMode", False, True, FixedTimeMode.Specifics)> _
        Public Property SpecificMonths As CompoundMonth

        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        Public Property WeekMode As FixedTimeMode = FixedTimeMode.Every

        <ExtendedCategory("Config")> _
        <AutoPostBack()> _
         <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        <ConditionalVisible("WeekMode", False, True, FixedTimeMode.Specifics)> _
        Public Property SpecificWeeks As CompoundWeekOfMonth

        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        Public Property DayMode As FixedTimeMode = FixedTimeMode.Every

        <ExtendedCategory("Config")> _
        <AutoPostBack()> _
         <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        <ConditionalVisible("DayMode", False, True, FixedTimeMode.Specifics)> _
        Public Property SpecificDays As CompoundDayOfWeek

        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        Public Property HourMode As FixedTimeMode = FixedTimeMode.Every

        <ExtendedCategory("Config")> _
        <AutoPostBack()> _
         <ConditionalVisible("ScheduleType", False, True, ScheduleType.FixedTimes, ScheduleType.Combined)> _
        <ConditionalVisible("HourMode", False, True, FixedTimeMode.Specifics)> _
        Public Property SpecificHours As CompoundHour

        <ExtendedCategory("Config")> _
        <ConditionalVisible("ScheduleType", False, True, ScheduleType.Period, ScheduleType.Combined)> _
       Public Property Period As New STimeSpan(TimeSpan.FromHours(1))

       

        <ExtendedCategory("Config")> _
        Public ReadOnly Property FormattedValueShort As String
            Get
                Return GetFormattedValue(True)
            End Get
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Tests")> _
        <LabelMode(LabelMode.Top)> _
        Public Property TestDate As DateTime = Now


        Private _TestResult As Nullable(Of DateTime)

        <Browsable(False)> _
        Public ReadOnly Property HasResult As Boolean
            Get
                Return _TestResult.HasValue()
            End Get
        End Property

        <ConditionalVisible("HasResult")> _
        <ExtendedCategory("Tests")> _
        Public ReadOnly Property TestResult As String
            Get
                If _TestResult.HasValue Then
                    Return _TestResult.Value.ToLongDateString() & " " & _TestResult.Value.ToLongTimeString()
                End If
                Return Nothing
            End Get
        End Property

        <ExtendedCategory("Tests")> _
        <ActionButton(IconName.Question, IconOptions.Normal)> _
        Public Sub Test(ByVal ape As AriciePropertyEditorControl)
            _TestResult = Me.GetNextSchedule(TestDate)
            ape.ItemChanged = True
        End Sub


        Public Shared Function GetCurrentSpecifics() As ScheduleInfo
            Dim toReturn As New ScheduleInfo()
            toReturn.ScheduleType = ScheduleType.FixedTimes
            Dim objNow As DateTime = Now
            toReturn.YearMode = FixedTimeMode.Specifics
            toReturn.SpecificYears.Add(objNow.Year)
            toReturn.MonthMode = FixedTimeMode.Specifics
            toReturn.SpecificMonths = objNow.GetCompoundMonth()
            toReturn.WeekMode = FixedTimeMode.Specifics
            toReturn.SpecificWeeks = objNow.GetCompoundWeek()
            toReturn.HourMode = FixedTimeMode.Specifics
            toReturn.SpecificHours = objNow.GetCompoundHour()
            Return toReturn
        End Function






        Public Function GetNextSchedule(previousSchedule As DateTime) As DateTime
            Select Case Me.ScheduleType
                Case Aricie.ComponentModel.ScheduleType.Period
                    Return previousSchedule.Add(Me.Period.Value)
                Case Aricie.ComponentModel.ScheduleType.FixedTimes, Aricie.ComponentModel.ScheduleType.Combined
                    Dim toReturn As DateTime = DateTime.MaxValue
                    Dim currentTarget As DateTime = Now
                    Dim tempPeriodTarget As DateTime
                    Dim stepSpan As TimeSpan
                    If Me.ScheduleType = Aricie.ComponentModel.ScheduleType.Combined Then
                        stepSpan = Me.Period.Value
                        tempPeriodTarget = previousSchedule.Add(stepSpan)
                    Else
                        tempPeriodTarget = previousSchedule.Date.AddHours(previousSchedule.Hour + 1)
                    End If

                    If currentTarget < tempPeriodTarget Then
                        currentTarget = tempPeriodTarget
                    End If
                    While currentTarget <> toReturn
                        toReturn = currentTarget
                        currentTarget = GetNextHour(currentTarget, previousSchedule)
                        currentTarget = GetNextDay(currentTarget, previousSchedule)
                        currentTarget = GetNextWeek(currentTarget, previousSchedule)
                        currentTarget = GetNextMonth(currentTarget, previousSchedule)
                        currentTarget = GetNextYear(currentTarget, previousSchedule)
                    End While
                    Return toReturn
            End Select
        End Function


        Public Function GetFormattedValue(shortVersion As Boolean) As String
            Select Case Me.ScheduleType
                Case Aricie.ComponentModel.ScheduleType.FixedTimes, Aricie.ComponentModel.ScheduleType.Combined
                    Dim toReturn As New StringBuilder()
                    Select Case Me.YearMode
                        Case FixedTimeMode.Once
                            Return "Once"
                        Case FixedTimeMode.Every
                            toReturn.Append(IIf(shortVersion, "EY,", "Every Year,"))
                        Case FixedTimeMode.Specifics
                            toReturn.Append(IIf(shortVersion, "OY,", "On Years: "))
                            Dim first As Boolean = True
                            For Each objYear As Integer In Me.SpecificYears

                                If Not first Then
                                    toReturn.Append(IIf(shortVersion, ",", ", "))
                                Else
                                    first = False
                                End If
                                toReturn.Append(objYear.ToString(CultureInfo.InvariantCulture))
                            Next
                            toReturn.Append(",")

                    End Select
                    Select Case Me.MonthMode
                        Case FixedTimeMode.Once
                            toReturn.Append(IIf(shortVersion, " OMaY,", " One Month a year"))
                        Case FixedTimeMode.Every
                            toReturn.Append(IIf(shortVersion, " EM,", " Every month, "))
                        Case FixedTimeMode.Specifics
                            toReturn.Append(IIf(shortVersion, " OM:", " On Months: "))
                            Dim first As Boolean = True
                            For Each objCompound As CompoundMonth In Common.GetEnumMembers(Of CompoundMonth)()
                                If objCompound <> CompoundHour.None Then
                                    If (objCompound And Me.SpecificMonths) = objCompound Then
                                        If Not first Then
                                            toReturn.Append(IIf(shortVersion, ",", ", "))
                                        Else
                                            first = False
                                        End If
                                        If shortVersion Then
                                            toReturn.Append(objCompound.ToString().Substring(0, 2))
                                        Else
                                            toReturn.Append(objCompound.ToString())
                                        End If

                                    End If
                                End If
                            Next
                            toReturn.Append(",")

                    End Select
                    Select Case Me.WeekMode
                        Case FixedTimeMode.Once
                            toReturn.Append(IIf(shortVersion, " OWaM,", " One Week a Month,"))
                        Case FixedTimeMode.Every
                            toReturn.Append(IIf(shortVersion, " EW,", " Every Week,"))
                        Case FixedTimeMode.Specifics
                            toReturn.Append(IIf(shortVersion, " OMW:", " On Month weeks: "))
                            Dim first As Boolean = True
                            For Each objCompound As CompoundWeekOfMonth In Common.GetEnumMembers(Of CompoundWeekOfMonth)()
                                If objCompound <> CompoundHour.None Then
                                    If (objCompound And Me.SpecificWeeks) = objCompound Then
                                        If Not first Then
                                            toReturn.Append(IIf(shortVersion, ",", ", "))
                                        Else
                                            first = False
                                        End If
                                        toReturn.Append(objCompound.GetWeekNumber().ToString(CultureInfo.InvariantCulture))
                                    End If
                                End If
                            Next
                            toReturn.Append(",")

                    End Select
                    Select Case Me.DayMode
                        Case FixedTimeMode.Once
                            toReturn.Append(IIf(shortVersion, " OaW,", " Once a Week,"))
                        Case FixedTimeMode.Every
                            toReturn.Append(IIf(shortVersion, " EWD,", " Every Week Day,"))
                        Case FixedTimeMode.Specifics
                            toReturn.Append(IIf(shortVersion, " OWD:", " On Week Days: "))
                            Dim first As Boolean = True
                            For Each objCompound As CompoundDayOfWeek In Common.GetEnumMembers(Of CompoundDayOfWeek)()
                                If objCompound <> CompoundHour.None Then

                                    If (objCompound And Me.SpecificDays) = objCompound Then
                                        If Not first Then
                                            toReturn.Append(IIf(shortVersion, ",", ", "))
                                        Else
                                            first = False
                                        End If
                                        If shortVersion Then
                                            toReturn.Append(objCompound.GetDayOfWeek().ToString().Substring(0, 2))
                                        Else
                                            toReturn.Append(objCompound.GetDayOfWeek().ToString())
                                        End If

                                    End If
                                End If

                            Next
                            toReturn.Append(",")
                    End Select
                    Select Case Me.HourMode
                        Case FixedTimeMode.Once
                            toReturn.Append(IIf(shortVersion, " OaD", " Once a Day"))
                        Case FixedTimeMode.Every
                            toReturn.Append(IIf(shortVersion, " EH", " Every Hour"))
                        Case FixedTimeMode.Specifics
                            toReturn.Append(IIf(shortVersion, " OH:", " On Hours: "))
                            Dim first As Boolean = True
                            For Each objCompound As CompoundHour In Common.GetEnumMembers(Of CompoundHour)()
                                If objCompound <> CompoundHour.None Then

                                    If (objCompound And Me.SpecificHours) = objCompound Then
                                        If Not first Then
                                            toReturn.Append(IIf(shortVersion, ",", ", "))
                                        Else
                                            first = False
                                        End If
                                        toReturn.Append(objCompound.GetHour())
                                    End If
                                End If

                            Next
                    End Select

                    If Me.ScheduleType = Aricie.ComponentModel.ScheduleType.Combined Then
                        toReturn.Append(IIf(shortVersion, ", " & Me.Period.FormattedDuration, ", Every " & Me.Period.FormattedDuration))
                    End If
                    Return toReturn.ToString()
                Case Else
                    Return Me.Period.FormattedDuration
            End Select
        End Function


        Private Function GetNextHour(target As DateTime, previousSchedule As DateTime) As DateTime
            Dim toReturn As DateTime = target
            Select Case HourMode
                Case FixedTimeMode.Once
                    Dim targetDuration As TimeSpan = target.Subtract(previousSchedule)
                    Dim nextDay As DateTime = previousSchedule.GetCurrentDay().AddDays(1)
                    If targetDuration < nextDay.Subtract(previousSchedule) Then
                        toReturn = nextDay
                    End If
                    'Case FixedTimeMode.Every
                    '    Dim stepSpan As TimeSpan
                    '    If Me.ScheduleType = Aricie.ComponentModel.ScheduleType.Combined Then
                    '        stepSpan = Me.Period.Value
                    '    Else
                    '        stepSpan = TimeSpan.FromHours(1)
                    '    End If
                    '    If targetDuration < stepSpan Then
                    '        toReturn = previousSchedule.Add(stepSpan)
                    '    End If
                Case FixedTimeMode.Specifics
                    If Me.SpecificHours <> CompoundHour.None Then
                        If (target.GetCompoundHour() And Me.SpecificHours) <> target.GetCompoundHour Then
                            Dim nextCompound As CompoundHour = Common.GetNextCyclicFlag(Of CompoundHour)(target.GetCompoundHour(), Me.SpecificHours)
                            Dim nbHours As Integer = (nextCompound.GetHour() - target.Hour)
                            If nbHours < 0 OrElse nbHours = 0 AndAlso Me.ScheduleType = Aricie.ComponentModel.ScheduleType.FixedTimes Then
                                nbHours += 24
                            End If
                            toReturn = New DateTime(target.Year, target.Month, target.Day, target.Hour, 0, 0).AddHours(nbHours)
                        End If
                    Else
                        toReturn = DateTime.MaxValue
                    End If
            End Select
            Return toReturn
        End Function


        Private Function GetNextDay(target As DateTime, previousSchedule As DateTime) As DateTime
            Dim toReturn As DateTime = target
            Select Case DayMode
                Case FixedTimeMode.Once
                    Dim targetDay As New DateTime(previousSchedule.Year, previousSchedule.Month, previousSchedule.Day)
                    Dim nextWeek As DateTime = targetDay.AddDays(7)
                    If target < nextWeek Then
                        toReturn = nextWeek
                    End If
                Case FixedTimeMode.Specifics
                    If Me.SpecificDays <> CompoundDayOfWeek.None Then
                        Dim targetCompoundDay As CompoundDayOfWeek = target.GetCompoundDayOfWeek()
                        If (targetCompoundDay And Me.SpecificDays) <> targetCompoundDay Then
                            Dim nextCompound As CompoundDayOfWeek = Common.GetNextCyclicFlag(Of CompoundDayOfWeek)(targetCompoundDay, Me.SpecificDays)
                            Dim nbDays As Integer = nextCompound.GetDayOfWeek() - target.DayOfWeek
                            If nbDays < 0 OrElse nbDays = 0 AndAlso Me.ScheduleType = ScheduleType.FixedTimes Then
                                nbDays += 7
                            End If
                            toReturn = target.GetCurrentDay().AddDays(nbDays)
                        End If
                    Else
                        toReturn = DateTime.MaxValue
                    End If
            End Select
            Return toReturn
        End Function

        Private Function GetNextWeek(target As DateTime, previousSchedule As DateTime) As DateTime
            Dim toReturn As DateTime = target
            Select Case WeekMode
                Case FixedTimeMode.Once
                    Dim previousMonth As DateTime = previousSchedule.GetCurrentMonth()
                    Dim targetMonth As DateTime = target.GetCurrentMonth()
                    If targetMonth <= previousMonth Then
                        Dim targetWeek As Integer = target.GetWeekNumberOfMonth()
                        Dim previousWeek As Integer = previousSchedule.GetWeekNumberOfMonth()
                        If targetWeek <> previousWeek Then
                            toReturn = target.GetNextMonth()
                        End If
                    End If
                Case FixedTimeMode.Specifics
                    If Me.SpecificWeeks <> CompoundWeekOfMonth.None Then
                        Dim targetCompound As CompoundWeekOfMonth = target.GetCompoundWeek()
                        If (targetCompound And Me.SpecificWeeks) <> targetCompound Then
                            Dim nextCompound As CompoundWeekOfMonth = Common.GetNextCyclicFlag(Of CompoundWeekOfMonth)(targetCompound, Me.SpecificWeeks)
                            toReturn = toReturn.GetCurrentDay()
                            Do
                                toReturn = toReturn.AddDays(1)
                            Loop Until toReturn.GetCompoundWeek() = nextCompound
                        End If
                    Else
                        toReturn = DateTime.MaxValue
                    End If
            End Select
            Return toReturn
        End Function

        Private Function GetNextMonth(target As DateTime, previousSchedule As DateTime) As DateTime
            Dim toReturn As DateTime = target
            Select Case MonthMode
                Case FixedTimeMode.Once
                    If target.Year <= previousSchedule.Year AndAlso target.Month > previousSchedule.Month Then
                        toReturn = target.GetNextYear()
                    End If
                Case FixedTimeMode.Specifics
                    If Me.SpecificMonths <> CompoundMonth.None Then
                        Dim targetCompound As CompoundMonth = target.GetCompoundMonth()
                        If (targetCompound And Me.SpecificMonths) <> targetCompound Then
                            Dim nextCompound As CompoundMonth = Common.GetNextCyclicFlag(Of CompoundMonth)(targetCompound, Me.SpecificMonths)
                            toReturn = toReturn.GetCurrentMonth()
                            Do
                                toReturn = toReturn.GetNextMonth()
                            Loop Until toReturn.GetCompoundMonth() = nextCompound
                        End If
                    Else
                        toReturn = DateTime.MaxValue
                    End If
            End Select
            Return toReturn
        End Function

        Private Function GetNextYear(target As DateTime, previousSchedule As DateTime) As DateTime
            Dim toReturn As DateTime = target
            Select Case YearMode
                Case FixedTimeMode.Once
                    If previousSchedule <> DateTime.MinValue Then
                        toReturn = DateTime.MaxValue
                    End If
                Case FixedTimeMode.Specifics
                    If Not Me.SpecificYears.Contains(target.Year) Then
                        toReturn = DateTime.MaxValue
                        If Me.SpecificYears.Count > 0 Then
                            For Each objYear As Integer In Me.SpecificYears
                                If objYear >= previousSchedule.Year AndAlso objYear > target.Year AndAlso objYear < toReturn.Year Then
                                    toReturn = New DateTime(objYear, 1, 1)
                                End If
                            Next
                        End If
                    End If
            End Select
            Return toReturn
        End Function



    End Class
End Namespace