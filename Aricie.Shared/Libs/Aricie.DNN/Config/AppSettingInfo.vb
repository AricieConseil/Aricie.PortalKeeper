Imports System.Xml

Namespace Configuration

    ''' <summary>
    ''' Base class for Appsettings configuration element
    ''' </summary>
    Public Class AppSettingInfo
        Inherits XmlNamedConfigElementInfo

        Public Sub New(ByVal key As String, ByVal value As String)
            MyBase.New(key)
            Me._Value = value
        End Sub

        Public Property Value As String = ""

        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode("configuration/appSettings/add[@key='" & Me.Name & "']")
            Return (Not moduleNode Is Nothing)
        End Function

        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install
                    Dim node As New StandardComplexNodeInfo("/configuration/appSettings", NodeInfo.NodeAction.update, "key", StandardComplexNodeInfo.NodeCollision.overwrite)
                    node.Children.Add(New AppSettingAddInfo(Me.Name, Me.Value))
                    targetNodes.Nodes.Add(node)

                Case ConfigActionType.Uninstall
                    Dim node As New NodeInfo("/configuration/appSettings/add[@key='" & Me.Name & "']", NodeInfo.NodeAction.remove)
                    targetNodes.Nodes.Add(node)
            End Select
        End Sub
    End Class



End Namespace


