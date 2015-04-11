Imports System.Xml
Imports Aricie.DNN.Services

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

        Public Property Level As String = "Medium"
        Public Property OriginUrl As String = ""

        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean
            Dim xPath As String = NukeHelper.DefineWebServerElementPath("system.web") & "/trust[@level='" & Me._Level.ToString & "'"
            If Not String.IsNullOrEmpty(Me._OriginUrl) Then
                xPath &= " and @originUrl='" & Me._OriginUrl & "'"
            End If
            xPath &= "]"
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode(xPath)
            Return (moduleNode IsNot Nothing)
        End Function


        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install

                    Dim node As New StandardComplexNodeInfo((NukeHelper.DefineWebServerElementPath("system.web")), NodeInfo.NodeAction.update, _
                                                            StandardComplexNodeInfo.NodeCollision.save, NukeHelper.DefineWebServerElementPath("system.web") & "/trust")
                    node.Children.Add(New TrustAddInfo(Me))
                    targetNodes.Nodes.Add(node)

                Case ConfigActionType.Uninstall
                    ' not implemented
            End Select

        End Sub
    End Class
End Namespace