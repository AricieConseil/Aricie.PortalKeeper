Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.ComponentModel


''' <summary>
''' Serializable and Display friendly class for a Timespan value
''' </summary>
<Serializable()> _
<XmlRoot("t")> _
Public Class STimeSpan
    Implements ICloneable



    Private _Value As TimeSpan

    Public Sub New()
    End Sub

    Public Sub New(ByVal spanValue As TimeSpan)
        Me._Value = spanValue
    End Sub

    Public Shared Widening Operator CType(ByVal element As STimeSpan) As TimeSpan
        If (Not element Is Nothing) Then
            Return element.Value
        End If
        Return Nothing
    End Operator

    Public Shared Widening Operator CType(ByVal [element] As TimeSpan) As STimeSpan
        Return New STimeSpan(element)
    End Operator

    <Browsable(False)> _
        <XmlIgnore()> _
    Public Property Value() As TimeSpan
        Get
            Return _Value
        End Get
        Set(ByVal value As TimeSpan)
            _Value = value
        End Set
    End Property

    <Xml.Serialization.XmlAttribute("d")> _
    <Browsable(False)> _
    Public Property Duration() As String
        Get
            Return Me._Value.ToString
        End Get
        Set(ByVal value As String)
            Me._Value = TimeSpan.Parse(value)
        End Set
    End Property

    <Category("")> _
    <MainCategoryAttribute()> _
    Public ReadOnly Property FormattedDuration() As String
        Get
            Return Me.ToString
        End Get
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Days() As Integer
        Get
            Return Me._Value.Days
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromDays(Me._Value.Days)).Add(TimeSpan.FromDays(value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Hours() As Integer
        Get
            Return Me._Value.Hours
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromHours(Me._Value.Hours)).Add(TimeSpan.FromHours(value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Minutes() As Integer
        Get
            Return Me._Value.Minutes
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromMinutes(Me._Value.Minutes)).Add(TimeSpan.FromMinutes(value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Seconds() As Integer
        Get
            Return Me._Value.Seconds
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromSeconds(Me._Value.Seconds)).Add(TimeSpan.FromSeconds(value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Milliseconds() As Integer
        Get
            Return Me._Value.Milliseconds
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromMilliseconds(Me._Value.Milliseconds)).Add(TimeSpan.FromMilliseconds(value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Microseconds() As Integer
        Get
            Return Aricie.Common.GetMicroSeconds(Me._Value)
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromTicks(10 * Aricie.Common.GetMicroSeconds(Me._Value))).Add(TimeSpan.FromTicks(10 * value))
        End Set
    End Property

    <XmlIgnore()> _
    <Category("Details")> _
    Public Property Ticks() As Integer
        Get
            Return CInt(Me._Value.Ticks Mod 10)
        End Get
        Set(ByVal value As Integer)
            Me._Value = Me._Value.Subtract(TimeSpan.FromTicks(Me._Value.Ticks Mod 10).Add(TimeSpan.FromTicks(value)))
        End Set
    End Property



    Public Overrides Function ToString() As String
        Return Common.FormatTimeSpan(Me._Value, True)
    End Function


    Public Shared Function Convert(ByVal objSpan As STimeSpan) As Double
        Return CDbl(objSpan.Value.Ticks)
    End Function

    Public Shared Function Convert(ByVal objDouble As Double) As STimeSpan
        Return New STimeSpan(TimeSpan.FromTicks(CLng(objDouble)))
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New STimeSpan(Me.Value)
    End Function
End Class
