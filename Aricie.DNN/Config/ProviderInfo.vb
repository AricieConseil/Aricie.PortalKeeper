Imports System.Xml
Imports Aricie.Services

Namespace Configuration

    ''' <summary>
    ''' Base class for DotNetNuke providers
    ''' </summary>
    Public Class ProviderInfo
        Inherits TypedXmlConfigElementInfo

        Public Sub New(ByVal providersName As String, ByVal name As String, ByVal objType As Type, ByVal restoreDefaultName As String)
            MyBase.New(name, objType)
            Me._ProvidersName = providersName
            Me._RestoreDefaultName = restoreDefaultName
        End Sub

        Public Property ProvidersName() As String = String.Empty
        Public Property RestoreDefaultName() As String = String.Empty
        Public Property Attributes() As New Dictionary(Of String, String)

        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean
            Return Me.ProviderExists(xmlConfig, True)
        End Function

        Public Function ProviderExists(ByVal xmlConfig As XmlDocument, ByVal andIsDefault As Boolean) As Boolean

            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode("configuration/dotnetnuke/" & Me.ProvidersName & "/providers/add[@name='" & Me.Name & "']")
            If moduleNode Is Nothing Then
                Return False
            End If
            Dim configType As String = moduleNode.Attributes("type").Value
            Dim providerType As String = ReflectionHelper.GetSafeTypeName(Me.Type)
            If Not configType = providerType Then
                Return False
            End If
            Return ((Not andIsDefault) Or moduleNode.ParentNode.ParentNode.Attributes("defaultProvider").Value = Me.Name)
        End Function

        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install
                    Dim node As New StandardComplexNodeInfo("/configuration/dotnetnuke/" & Me.ProvidersName & "/providers", NodeInfo.NodeAction.update, "name", StandardComplexNodeInfo.NodeCollision.overwrite)
                    node.Children.Add(New ProviderAddInfo(Me.Name, Me.Type, Me.Attributes))
                    targetNodes.Nodes.Add(node)
                    Dim node2 As New SimpleNodeInfo("/configuration/dotnetnuke/" & Me.ProvidersName, NodeInfo.NodeAction.updateattribute, _
                                                    "defaultProvider", Me.Name)
                    targetNodes.Nodes.Add(node2)
                Case ConfigActionType.Uninstall
                    Dim node As New SimpleNodeInfo("/configuration/dotnetnuke/" & Me.ProvidersName, NodeInfo.NodeAction.updateattribute, _
                                                    "defaultProvider", Me.RestoreDefaultName)
                    targetNodes.Nodes.Add(node)
                    Dim node2 As New NodeInfo("/configuration/dotnetnuke/" & Me.ProvidersName & "/providers/add[@name='" & Me.Name & "']", NodeInfo.NodeAction.remove)
                    targetNodes.Nodes.Add(node2)
            End Select
        End Sub
    End Class



End Namespace


