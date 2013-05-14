Imports System.Xml.Serialization

Namespace Configuration

    
    '<XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    '<XmlInclude(GetType(SimpleNodeInfo))> _
    '<XmlInclude(GetType(ComplexNodeInfo))> _
    <XmlRoot("nodes"), XmlType(AnonymousType:=True)> _
    <Serializable()> _
    Public Class NodesInfo
        'Inherits List(Of NodeInfo)

        '<nodes configfile="web.config">


        Private _Nodes As New List(Of NodeInfo)



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

        ' <XmlElement(GetType(NodeInfo))> _
        '<XmlElement(GetType(SimpleNodeInfo))> _
        '<XmlElement(GetType(ComplexNodeInfo))> _
        '<XmlElement(GetType(StandardComplexNodeInfo))> _
        <XmlElement("node")> _
        Public Property Nodes() As List(Of NodeInfo)
            Get
                Return _Nodes
            End Get
            Set(ByVal value As List(Of NodeInfo))
                _Nodes = value
            End Set
        End Property


    End Class
End Namespace