Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace ComponentModel

    <Flags()> _
    Public Enum CompoundHour
        None = 0
        OneAM = 1
        TwoAM = 2
        ThreeAM = 4
        FourAM = 8
        FiveAM = 16
        SixAM = 32
        SevenAM = 64
        EightAM = 128
        NineAM = 256
        TenAM = 512
        ElevenAM = 1024
        Midday = 2048
        OnePM = 4096
        TwoPM = 8192
        ThreePM = 16384
        FourPM = 32768
        FivePM = 65536
        SixPM = 131072
        SevenPM = 262144
        EightPM = 524288
        NinePM = 1048576
        TenPM = 2097152
        ElevenPM = 4194304
        Midnight = 8388608
    End Enum


    <Flags()> _
    Public Enum CompoundMonth
        None = 0
        January = 1
        February = 2
        March = 4
        April = 8
        May = 16
        June = 32
        July = 64
        August = 128
        September = 256
        October = 512
        November = 1024
        December = 2048
    End Enum

    <Flags()> _
    Public Enum CompoundWeekOfMonth
        None = 0
        First = 1
        Second = 2
        Third = 4
        Fourth = 8
        Fifth = 16
    End Enum


    <Flags()> _
    Public Enum CompoundDayOfWeek
        None = 0
        Monday = 1
        Tuesday = 2
        Wednesday = 4
        Thursday = 8
        Friday = 16
        Saturday = 32
        Sunday = 64
    End Enum

    Public Enum ScheduleType
        Period
        FixedTimes
        Combined
    End Enum

    Public Enum FixedTimeMode
        Every
        Once
        Specifics
    End Enum

    Public Module DateHelper

        <Extension> _
        Public Function GetWeekNumber([date] As DateTime) As Integer
            Return [date].GetWeekNumber(CultureInfo.CurrentCulture)
        End Function

        <Extension> _
        Public Function GetWeekNumber([date] As DateTime, culture As CultureInfo) As Integer
            Return culture.Calendar.GetWeekOfYear([date], culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek)
        End Function


        <Extension> _
        Public Function GetWeekNumberOfMonth([date] As DateTime) As Integer
            Return [date].GetWeekNumberOfMonth(CultureInfo.CurrentCulture)
        End Function

        <Extension> _
        Public Function GetWeekNumberOfMonth([date] As DateTime, culture As CultureInfo) As Integer
            Return [date].GetWeekNumber(culture) - New DateTime([date].Year, [date].Month, 1).GetWeekNumber(culture) + 1
        End Function

        <Extension> _
        Public Function GetCurrentDay([date] As DateTime) As DateTime
            Return New DateTime([date].Year, [date].Month, [date].Day)
        End Function


        <Extension> _
        Public Function GetCurrentMonth([date] As DateTime) As DateTime
            Return New DateTime([date].Year, [date].Month, 1)
        End Function


        <Extension> _
        Public Function GetNextMonth([date] As DateTime) As DateTime
            If [date].Month < 12 Then
                Return New DateTime([date].Year, [date].Month + 1, 1)
            Else
                Return New DateTime([date].Year + 1, 1, 1)
            End If
        End Function

        <Extension> _
        Public Function GetNextYear([date] As DateTime) As DateTime
            Return New DateTime([date].Year, 1, 1)
        End Function


        <Extension()> _
        Public Function GetCompoundHour([date] As DateTime) As CompoundHour
            Select Case [date].Hour
                Case 0
                    Return CompoundHour.Midnight
                Case 1
                    Return CompoundHour.OneAM
                Case 2
                    Return CompoundHour.TwoAM
                Case 3
                    Return CompoundHour.ThreeAM
                Case 4
                    Return CompoundHour.FourAM
                Case 5
                    Return CompoundHour.FiveAM
                Case 6
                    Return CompoundHour.SixAM
                Case 7
                    Return CompoundHour.SevenAM
                Case 8
                    Return CompoundHour.EightAM
                Case 9
                    Return CompoundHour.NineAM
                Case 10
                    Return CompoundHour.TenAM
                Case 11
                    Return CompoundHour.ElevenAM
                Case 12
                    Return CompoundHour.Midday
                Case 13
                    Return CompoundHour.OnePM
                Case 14
                    Return CompoundHour.TwoPM
                Case 15
                    Return CompoundHour.ThreePM
                Case 16
                    Return CompoundHour.FourPM
                Case 17
                    Return CompoundHour.FivePM
                Case 18
                    Return CompoundHour.SixPM
                Case 19
                    Return CompoundHour.SevenPM
                Case 20
                    Return CompoundHour.EightPM
                Case 21
                    Return CompoundHour.NinePM
                Case 22
                    Return CompoundHour.TenPM
                Case 23
                    Return CompoundHour.ElevenAM

            End Select
        End Function

        <Extension()> _
        Public Function GetHour(objCompound As CompoundHour) As Integer
            Select Case objCompound
                Case CompoundHour.Midnight
                    Return 0
                Case CompoundHour.OneAM
                    Return 1
                Case CompoundHour.TwoAM
                    Return 2
                Case CompoundHour.ThreeAM
                    Return 3
                Case CompoundHour.FourAM
                    Return 4
                Case CompoundHour.FiveAM
                    Return 5
                Case CompoundHour.SixAM
                    Return 6
                Case CompoundHour.SevenAM
                    Return 7
                Case CompoundHour.EightAM
                    Return 8
                Case CompoundHour.NineAM
                    Return 9
                Case CompoundHour.TenAM
                    Return 10
                Case CompoundHour.ElevenAM
                    Return 11
                Case CompoundHour.Midday
                    Return 12
                Case CompoundHour.OnePM
                    Return 13
                Case CompoundHour.TwoPM
                    Return 14
                Case CompoundHour.ThreePM
                    Return 15
                Case CompoundHour.FourPM
                    Return 16
                Case CompoundHour.FivePM
                    Return 17
                Case CompoundHour.SixPM
                    Return 18
                Case CompoundHour.SevenPM
                    Return 19
                Case CompoundHour.EightPM
                    Return 20
                Case CompoundHour.NinePM
                    Return 21
                Case CompoundHour.TenPM
                    Return 22
                Case CompoundHour.ElevenAM
                    Return 23
            End Select
            Throw New ArgumentException("Invalid Hour", "objCompound")
        End Function

        <Extension()> _
        Public Function GetCompoundMonth(objDate As DateTime) As CompoundMonth
            Select Case objDate.Month
                Case 1
                    Return CompoundMonth.January
                Case 2
                    Return CompoundMonth.February
                Case 3
                    Return CompoundMonth.March
                Case 4
                    Return CompoundMonth.April
                Case 5
                    Return CompoundMonth.May
                Case 6
                    Return CompoundMonth.June
                Case 7
                    Return CompoundMonth.July
                Case 8
                    Return CompoundMonth.August
                Case 9
                    Return CompoundMonth.September
                Case 10
                    Return CompoundMonth.October
                Case 11
                    Return CompoundMonth.November
                Case 12
                    Return CompoundMonth.December
            End Select
        End Function

        <Extension()> _
        Public Function GetMonth(objCompound As CompoundMonth) As Integer
            Select Case objCompound
                Case CompoundMonth.January
                    Return 1
                Case CompoundMonth.February
                    Return 2
                Case CompoundMonth.March
                    Return 3
                Case CompoundMonth.April
                    Return 4
                Case CompoundMonth.May
                    Return 5
                Case CompoundMonth.June
                    Return 6
                Case CompoundMonth.July
                    Return 7
                Case CompoundMonth.August
                    Return 8
                Case CompoundMonth.September
                    Return 9
                Case CompoundMonth.October
                    Return 10
                Case CompoundMonth.November
                    Return 11
                Case CompoundMonth.December
                    Return 12
            End Select
            Throw New ArgumentException("Invalid Month", "objCompound")
        End Function

        <Extension()> _
        Public Function GetCompoundWeek(objDate As DateTime) As CompoundWeekOfMonth
            Select Case objDate.GetWeekNumberOfMonth()
                Case 1
                    Return CompoundWeekOfMonth.First
                Case 2
                    Return CompoundWeekOfMonth.Second
                Case 3
                    Return CompoundWeekOfMonth.Third
                Case 4
                    Return CompoundWeekOfMonth.Fourth
                Case 5
                    Return CompoundWeekOfMonth.Fifth
            End Select
        End Function

        <Extension()> _
        Public Function GetWeekNumber(objCompound As CompoundWeekOfMonth) As Integer
            Select Case objCompound
                Case CompoundWeekOfMonth.First
                    Return 1
                Case CompoundWeekOfMonth.Second
                    Return 2
                Case CompoundWeekOfMonth.Third
                    Return 3
                Case CompoundWeekOfMonth.Fourth
                    Return 4
                Case CompoundWeekOfMonth.Fifth
                    Return 5
            End Select
            Throw New ArgumentException("Invalid Week Number", "objCompound")
        End Function


        <Extension()> _
        Public Function GetCompoundDayOfWeek(objDate As DateTime) As CompoundDayOfWeek
            Select Case objDate.DayOfWeek
                Case DayOfWeek.Monday
                    Return CompoundDayOfWeek.Monday
                Case DayOfWeek.Tuesday
                    Return CompoundDayOfWeek.Tuesday
                Case DayOfWeek.Wednesday
                    Return CompoundDayOfWeek.Wednesday
                Case DayOfWeek.Thursday
                    Return CompoundDayOfWeek.Thursday
                Case DayOfWeek.Friday
                    Return CompoundDayOfWeek.Friday
                Case DayOfWeek.Saturday
                    Return CompoundDayOfWeek.Saturday
                Case DayOfWeek.Sunday
                    Return CompoundDayOfWeek.Sunday
            End Select
        End Function

        <Extension()> _
        Public Function GetDayOfWeek(objCompound As CompoundDayOfWeek) As DayOfWeek
            Select Case objCompound
                Case CompoundDayOfWeek.Monday
                    Return DayOfWeek.Monday
                Case CompoundDayOfWeek.Tuesday
                    Return DayOfWeek.Tuesday
                Case CompoundDayOfWeek.Wednesday
                    Return DayOfWeek.Wednesday
                Case CompoundDayOfWeek.Thursday
                    Return DayOfWeek.Thursday
                Case CompoundDayOfWeek.Friday
                    Return DayOfWeek.Friday
                Case CompoundDayOfWeek.Saturday
                    Return DayOfWeek.Saturday
                Case CompoundDayOfWeek.Sunday
                    Return DayOfWeek.Sunday
            End Select
            Throw New ArgumentException("Invalid Day of Week", "objCompound")
        End Function


    End Module
End Namespace