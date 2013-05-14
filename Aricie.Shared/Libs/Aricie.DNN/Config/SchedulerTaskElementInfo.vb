Imports DotNetNuke.Services.Scheduling
Imports Aricie.Services
Imports System.Reflection

Namespace Configuration

    ''' <summary>
    ''' Enumeration for represent of common time units
    ''' </summary>
    Public Enum TimeUnit
        s
        m
        h
        d
    End Enum

    ''' <summary>
    ''' Represents a duration by a value and a unit
    ''' </summary>
    Public Structure TimeLapse

        Public Sub New(timeLapse As Integer, timeMeasurement As String)
            Me.New(timeLapse, Common.GetEnum(Of TimeUnit)(timeMeasurement))
        End Sub

        Public Sub New(ByVal value As Integer, ByVal unit As TimeUnit)
            Me.Value = value
            Me.Unit = unit
        End Sub

        Public Value As Integer
        Public Unit As TimeUnit

        Public Shared Function FromTimeSpan(ByVal span As TimeSpan) As TimeLapse
            If span.Days > 0 Then
                Return New TimeLapse(span.Days, TimeUnit.d)
            ElseIf span.Hours > 0 Then
                Return New TimeLapse(span.Hours, TimeUnit.h)
            ElseIf span.Minutes > 0 Then
                Return New TimeLapse(span.Minutes, TimeUnit.m)
            Else
                Return New TimeLapse(span.Seconds, TimeUnit.s)
            End If
        End Function

        Public Function ToTimeSpan() As TimeSpan
            Select Case Me.Unit
                Case TimeUnit.d
                    Return TimeSpan.FromDays(Me.Value)
                Case TimeUnit.h
                    Return TimeSpan.FromHours(Me.Value)
                Case TimeUnit.m
                    Return TimeSpan.FromMinutes(Me.Value)
                Case TimeUnit.s
                    Return TimeSpan.FromSeconds(Me.Value)
            End Select
        End Function



    End Structure

    ''' <summary>
    ''' Configuration class for a scheduled Task
    ''' </summary>
    Public Class SchedulerTaskElementInfo
        Inherits TypedEntityElementInfo






        Public Sub New(ByVal objType As Type, ByVal timeSpan As TimeSpan)
            MyBase.New(objType)
            Me._TimeSpan = timeSpan
        End Sub

        Public Sub New(ByVal friendlyName As String, ByVal objType As Type, ByVal timeSpan As TimeSpan)
            Me.New(objType, timeSpan)
            Me._FriendlyName = friendlyName
        End Sub

        Private _TimeSpan As TimeSpan


        Public Property TimeSpan() As TimeSpan
            Get
                Return _TimeSpan
            End Get
            Set(ByVal value As TimeSpan)
                _TimeSpan = value
            End Set
        End Property


        Private _FriendlyName As String = ""

        Public Property FriendlyName() As String
            Get
                Return _FriendlyName
            End Get
            Set(ByVal value As String)
                _FriendlyName = value
            End Set
        End Property



        Public Overloads Overrides Function IsInstalled(ByVal type As Type) As Boolean
            Return ScheduleClientExists(Me.EntityType)
        End Function

        Private Function ScheduleClientExists(ByVal objType As Type) As Boolean
            Dim items As ArrayList = SchedulingProvider.Instance.GetSchedule()
            For Each item As ScheduleItem In items
                If item.TypeFullName = ReflectionHelper.GetSafeTypeName(objType) Then
                    Return True
                End If
            Next
            Return False
        End Function



        Public Overrides Sub ProcessConfig(ByVal actionType As ConfigActionType)
            Dim typeName As String = ReflectionHelper.GetSafeTypeName(Me.EntityType)
            Select Case actionType

                Case ConfigActionType.Install
                    Dim sItem As ScheduleItem

                    Dim item As ScheduleItem = SchedulingProvider.Instance.GetSchedule(typeName, "")
                    If item IsNot Nothing AndAlso item.ScheduleID <> -1 Then
                        SchedulingProvider.Instance.DeleteSchedule(item)
                    End If
                    sItem = New ScheduleItem()
                    If Aricie.DNN.Services.NukeHelper.DnnVersion.Major > 4 Then
                        Dim friendlyProp As PropertyInfo = Nothing
                        If ReflectionHelper.GetPropertiesDictionary(Of ScheduleItem).TryGetValue("FriendlyName", friendlyProp) Then
                            friendlyProp.SetValue(sItem, Me.FriendlyName, Nothing)

                        End If
                    End If

                    sItem.TypeFullName = typeName
                    Dim lapse As TimeLapse = TimeLapse.FromTimeSpan(Me.TimeSpan)
                    sItem.TimeLapse = lapse.Value
                    sItem.TimeLapseMeasurement = lapse.Unit.ToString
                    sItem.RetryTimeLapse = 2 * lapse.Value
                    sItem.RetryTimeLapseMeasurement = lapse.Unit.ToString
                    sItem.RetainHistoryNum = 50
                    sItem.Enabled = True
                    SchedulingProvider.Instance.AddSchedule(sItem)
                Case ConfigActionType.Uninstall
                    Dim item As ScheduleItem = SchedulingProvider.Instance.GetSchedule(typeName, "")
                    If item IsNot Nothing AndAlso item.ScheduleID <> -1 Then
                        SchedulingProvider.Instance.DeleteSchedule(item)
                    End If

            End Select
        End Sub
    End Class


End Namespace


