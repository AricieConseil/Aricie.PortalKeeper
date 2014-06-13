Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Filtering
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <Serializable()> _
    <DefaultProperty("Time")> _
    Public Class BotInfoEvent

        Public Sub New()
            Me._Time = Now
        End Sub


#Region "Private members"


        Private _Success As Boolean = False

        Private _Time As DateTime

        Private _Duration As TimeSpan = Constants.UnlimitedTimeSpan

        Private _VariablesDump As New SerializableDictionary(Of String, Object)

        Private _PayLoad As String = String.Empty







#End Region


#Region "Public Properties"

        Public Property Success() As Boolean
            Get
                Return _Success
            End Get
            Set(ByVal value As Boolean)
                _Success = value
            End Set
        End Property

        Public Property Time() As DateTime
            Get
                Return _Time
            End Get
            Set(ByVal value As DateTime)
                _Time = value
            End Set
        End Property


        <Browsable(False)> _
        Public Property Duration() As TimeSpan
            Get
                Return _Duration
            End Get
            Set(ByVal value As TimeSpan)
                _Duration = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property DurationAsString() As String
            Get
                Return _Duration.ToString
            End Get
            Set(ByVal value As String)
                _Duration = TimeSpan.Parse(value)
            End Set
        End Property

        <XmlIgnore()> _
        Public ReadOnly Property DurationString() As String
            Get
                If Me.Duration = Constants.UnlimitedTimeSpan Then
                    Return "NA"
                Else
                    Return Me.Duration.Seconds.ToString & " s " & Me.Duration.Milliseconds & " ms "
                End If
            End Get
        End Property


        <Browsable(False)> _
        Public Property VariablesDump() As SerializableDictionary(Of String, Object)
            Get
                Return _VariablesDump
            End Get
            Set(ByVal value As SerializableDictionary(Of String, Object))
                _VariablesDump = value
            End Set
        End Property

        <Width(500)> _
        <LineCount(50)> _
        <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        <XmlIgnore()> _
        <LabelMode(LabelMode.Top)> _
        Public ReadOnly Property PayLoad() As String
            Get
                Return ReflectionHelper.Serialize(_VariablesDump).Beautify()
            End Get
        End Property

#End Region
    End Class
End Namespace
