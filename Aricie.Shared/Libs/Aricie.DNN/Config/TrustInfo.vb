Imports System.Xml
Imports Aricie.DNN.Services
Imports System.Xml.Serialization
Imports DotNetNuke.Common.Utilities

Namespace Configuration
    ''' <summary>
    ''' Component to switch application trust level
    ''' </summary>
    <Serializable()> _
    Public Class TrustInfo
        Inherits XmlConfigElementInfo


        Public Sub New()

        End Sub

        Public Sub New(level As String)
            Me._Level = level
        End Sub

        Public Sub New(level As String, originUrl As String)
            Me.New(level)
            Me._OriginUrl = originUrl
        End Sub

        Private _Level As String = "Medium"

        Public Property Level() As String
            Get
                Return _Level
            End Get
            Set(ByVal value As String)
                _Level = value
            End Set
        End Property

        Private _OriginUrl As String = ""

        Public Property OriginUrl() As String
            Get
                Return _OriginUrl
            End Get
            Set(ByVal value As String)
                _OriginUrl = value
            End Set
        End Property


        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean
            Dim xPath As String = NukeHelper.DefineWebServerElementPath("system.web") & "/trust[@level='" & Me._Level.ToString & "'"
            If Not String.IsNullOrEmpty(Me._OriginUrl) Then
                xPath &= " and @originUrl='" & Me._OriginUrl & "'"
            End If
            xPath &= "]"
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode(xPath)
            Return (Not moduleNode Is Nothing)
        End Function


        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install


                    Dim node As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web")), NodeInfo.NodeAction.add, StandardComplexNodeInfo.NodeCollision.save, NukeHelper.DefineWebServerElementPath("system.web") & "/trust")
                    node.Children.Add(New TrustAddInfo(Me))
                    Dim xmlConfig As XmlDocument = Config.Load()
                    'node.Action = NodeInfo.NodeAction.add
                    'If (NukeHelper.DefineWebServerElementPath("system.web/trust").Length > 10) Then
                    If (targetNodes.Nodes.Count = 1) Then
                        targetNodes.Nodes.Add(node)
                    ElseIf (targetNodes.Nodes.Count < 1) Then
                        Dim trust As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web")
                        Dim trustNode As XmlNode = xmlConfig.CreateNode(XmlNodeType.Element, "trust", "")

                        Dim levelAttr As XmlAttribute = xmlConfig.CreateAttribute("level")
                        levelAttr.Value = Level.ToString
                        Dim originUrlAttr As XmlAttribute = xmlConfig.CreateAttribute("originUrl")
                        originUrlAttr.Value = ".*"
                        trustNode.Attributes.Append(levelAttr)
                        trustNode.Attributes.Append(originUrlAttr)
                        trust.AppendChild(trustNode)
                        Config.Save(xmlConfig)
                    End If
                    'End If





                Case ConfigActionType.Uninstall
                    'Dim node As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web")), NodeInfo.NodeAction.update, StandardComplexNodeInfo.NodeCollision.save, NukeHelper.DefineWebServerElementPath("system.web") & "/trust")
                    'node.Children.Add(New TrustAddInfo(New TrustInfo))
                    'targetNodes.Nodes.Add(node)
            End Select

        End Sub
    End Class
End Namespace