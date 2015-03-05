Imports System.Xml.Serialization

Namespace Configuration

    '<XmlType(AnonymousType:=True)> _
    '<XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    <XmlInclude(GetType(NodeInfo))> _
    <XmlInclude(GetType(SimpleNodeInfo))> _
    <XmlInclude(GetType(ComplexNodeInfo))> _
    <XmlInclude(GetType(StandardComplexNodeInfo))> _
    <XmlRoot("nodes")> _
    <Serializable()> _
    Public Class NodesInfo
        'Inherits List(Of NodeInfo)

        '<nodes configfile="web.config">





        Private _ConfigFile As String = ""

        Public Sub New(ByVal configFile As String)
            Me._ConfigFile = configFile
        End Sub

        Public Sub New()
            Me.New("web.config")
        End Sub

        <XmlAttribute("configfile")> _
        Public Property ConfigFile() As String
            Get
                Return _ConfigFile
            End Get
            Set(ByVal value As String)
                _ConfigFile = value
            End Set
        End Property

        '<XmlElement("node")> _
        ' <XmlElement("node", GetType(NodeInfo))> _
        '<XmlElement("node", GetType(SimpleNodeInfo))> _
        '<XmlElement("node", GetType(ComplexNodeInfo))> _
        '<XmlElement("node", GetType(StandardComplexNodeInfo))> _
        ' <XmlArrayItemAttribute("node", GetType(NodeInfo))> _
        '<XmlArrayItemAttribute("node", GetType(SimpleNodeInfo))> _
        '<XmlArrayItemAttribute("node", GetType(ComplexNodeInfo))> _
        '<XmlArrayItemAttribute("node", GetType(StandardComplexNodeInfo))> _
        '<XmlArrayItemAttribute("node")> _
        '  <XmlArrayAttribute()> _
        <XmlElement("node")> _
        Public Property Nodes() As New NodesList()
          


    End Class


    Public Class NodesList
        Inherits List(Of NodeInfo)


    End Class



End Namespace