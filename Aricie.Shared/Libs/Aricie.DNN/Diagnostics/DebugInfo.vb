Imports Aricie.Collections
Imports System.Xml.Serialization
Imports System.Threading
Imports DotNetNuke.Common
Imports Aricie.DNN.Services
Imports System.Globalization

Namespace Diagnostics
    ''' <summary>
    ''' A piece of Debug information
    ''' </summary>
    ''' <remarks>The AdditionalProperties property can hold arbitrary additional structured data.</remarks>
    <Serializable()> _
    Public Class DebugInfo

        <NonSerialized()> _
        Private _AdditionalProperties() As KeyValuePair(Of String, String) = Nothing
        Private _Properties As SerializableDictionary(Of String, String)

        Public Property LabelInsert As IConvertible = Nothing

        Public Property Label As String = String.Empty
        Public Property DebugType As String = String.Empty
        Public ReadOnly Property Name As String
            Get
                If LabelInsert IsNot Nothing Then
                    Return String.Format(Label, LabelInsert.ToString(CultureInfo.InvariantCulture))
                End If
                Return Label
            End Get
        End Property
        Public Property Description As String = String.Empty
        Public Property MemoryUsage As Boolean
        Public Property PortalId As Integer = -1
        Public Property ThreadId As String = String.Empty
        Public Property ThreadCulture As String = String.Empty
        Public Property ServerName As String = String.Empty

        Public Property RequestUrl As String = String.Empty

        Public Property IpAddress As String = String.Empty

        Public Property UserName As String = String.Empty

        <XmlIgnore()> _
        Public Property AdditionalProperties() As KeyValuePair(Of String, String)()
            Get
                Return _AdditionalProperties
            End Get
            Set(ByVal value As KeyValuePair(Of String, String)())
                _AdditionalProperties = value
            End Set
        End Property

        Public Property Properties() As SerializableDictionary(Of String, String)
            Get
                If _Properties Is Nothing Then
                    _Properties = New SerializableDictionary(Of String, String)
                    For Each objPair As KeyValuePair(Of String, String) In _AdditionalProperties
                        _Properties(objPair.Key) = objPair.Value
                    Next
                End If
                Return _Properties
            End Get
            Set(ByVal value As SerializableDictionary(Of String, String))
                _Properties = value
            End Set
        End Property

        Public Sub New()
            Me._ThreadId = Thread.CurrentThread.GetHashCode.ToString
            Me._ThreadCulture = Thread.CurrentThread.CurrentCulture.ToString
            Me.ServerName = Globals.ServerName
            Dim objDnnContext As DnnContext = DnnContext.Current
            Me.IpAddress = objDnnContext.IPAddress.ToString
            Me.UserName = objDnnContext.User.Username
            Me.PortalId = objDnnContext.Portal.PortalId
            Me.RequestUrl = objDnnContext.AbsoluteUri
        End Sub

        Public Sub New(ByVal debugType As String, ByVal name As String, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.New()
            Me._DebugType = debugType
            Me.Label = name
            Me._AdditionalProperties = additionalProperties
        End Sub

        Public Sub New(ByVal debugType As String, ByVal name As String, ByVal description As String, _
                       ByVal logMemoryUsage As Boolean, ByVal portalId As Integer, ByVal ParamArray additionalProperties() As KeyValuePair(Of String, String))
            Me.New(debugType, name, additionalProperties)
            Me._Description = description
            Me._MemoryUsage = logMemoryUsage
            Me._PortalId = portalId
        End Sub

    End Class

End Namespace
