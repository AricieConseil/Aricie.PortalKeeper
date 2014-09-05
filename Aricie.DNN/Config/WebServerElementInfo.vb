Imports System.Xml
Imports Aricie.DNN.Services
Imports System.ComponentModel

Namespace Configuration
    ''' <summary>
    ''' Base class for ASP.Net webserver elements
    ''' </summary>
    ''' <remarks>Can deal with II6/II7 configuration models</remarks>
    <Serializable()> _
    Public MustInherit Class WebServerElementInfo
        Inherits TypedXmlConfigElementInfo


        Protected MustOverride Function BuildAddNode(ByVal usePrecondition As Boolean) As WebServerAddInfo

        <Browsable(False)> _
        Public Overridable ReadOnly Property OnlyIIS6() As Boolean
            Get
                Return False
            End Get
        End Property


        Private _InsertBeforeKey As String = ""
        Private _InsertBeforeKeyIIS6 As String = ""
        Private _InsertAfterKey As String = ""
        Private _InsertAfterKeyIIS6 As String = ""

        Protected keyNameIIS6 As String = "name"
        Protected keyNameIIS7 As String = "name"

        Protected sectionNameIIS6 As String = ""
        Protected sectionNameIIS7 As String = ""

        Protected precondition As String = ""


        Protected Overridable Function GetKeyIIS6() As String
            Return Me.Name
        End Function

        Protected Overridable Function GetKeyIIS7() As String
            Return Me.Name
        End Function


        Public Sub New()

        End Sub

        Public Sub New(ByVal name As String, ByVal moduleType As Type)
            MyBase.New(name, moduleType)
        End Sub


        <Browsable(False)> _
        Public Property InsertBeforeKey() As String
            Get
                Return _InsertBeforeKey
            End Get
            Set(ByVal value As String)
                _InsertBeforeKey = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property InsertAfterKey() As String
            Get
                Return _InsertAfterKey
            End Get
            Set(ByVal value As String)
                _InsertAfterKey = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property InsertBeforeKey6() As String
            Get

                If Me._InsertBeforeKeyIIS6 = "" Then
                    Return Me._InsertBeforeKey
                End If
                Return _InsertBeforeKeyIIS6
            End Get
            Set(ByVal value As String)
                _InsertBeforeKeyIIS6 = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property InsertAfterKey6() As String
            Get
                If Me._InsertAfterKeyIIS6 = "" Then
                    Return Me.InsertAfterKey
                End If
                Return _InsertAfterKeyIIS6
            End Get
            Set(ByVal value As String)
                _InsertAfterKeyIIS6 = value
            End Set
        End Property

        Public Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean
            If Me.Type IsNot Nothing Then
                'Dim xmlConfig As XmlDocument = Config.Load()
                Dim objNode As XmlNode
                'DNN 7 is nolonger support IIS 6
                'Changed in 18/01/2013 by Yi

                If NukeHelper.DnnVersion.Major < 7 Then
                    objNode = xmlConfig.SelectSingleNode(NukeHelper.DefineWebServerElementPath("system.web") & "/" & sectionNameIIS6 & "/add[@" & keyNameIIS6 & "='" & Me.GetKeyIIS6 & "']")

                Else
                    objNode = xmlConfig.SelectSingleNode(NukeHelper.DefineWebServerElementPath("system.webServer") & "/" & sectionNameIIS7 & "/add[@" & keyNameIIS7 & "='" & Me.GetKeyIIS7 & "']")

                End If

                Return (Not objNode Is Nothing)
            End If
            Return False
        End Function


        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)
            Select Case actionType
                Case ConfigActionType.Install
                    Dim node As StandardComplexNodeInfo = Nothing
                    Dim node2 As StandardComplexNodeInfo = Nothing
                    If Me.InsertBeforeKey = "" AndAlso Me.InsertAfterKey = "" Then
                        node = New StandardComplexNodeInfo(NukeHelper.DefineWebServerElementPath("system.web") & "/" & sectionNameIIS6, NodeInfo.NodeAction.update, keyNameIIS6, StandardComplexNodeInfo.NodeCollision.overwrite)
                        If Not OnlyIIS6 Then
                            node2 = New StandardComplexNodeInfo(NukeHelper.DefineWebServerElementPath("system.webServer") & "/" & sectionNameIIS7, NodeInfo.NodeAction.update, keyNameIIS7, StandardComplexNodeInfo.NodeCollision.overwrite)
                        End If

                    Else
                        Dim targetChildKey As String = Me.InsertBeforeKey
                        Dim targetChildKey6 As String = Me.InsertBeforeKey6
                        Dim action As NodeInfo.NodeAction = NodeInfo.NodeAction.insertbefore
                        If Me.InsertBeforeKey = "" Then
                            targetChildKey = Me.InsertAfterKey
                            targetChildKey6 = Me.InsertAfterKey6
                            action = NodeInfo.NodeAction.insertafter
                        End If
                        node = New StandardComplexNodeInfo(NukeHelper.DefineWebServerElementPath("system.web") & "/" & sectionNameIIS6 & "/add[@" & keyNameIIS6 & "='" & targetChildKey6 & "']", action, keyNameIIS6, StandardComplexNodeInfo.NodeCollision.overwrite)
                        If Not OnlyIIS6 Then
                            node2 = New StandardComplexNodeInfo(NukeHelper.DefineWebServerElementPath("system.webServer") & "/" & sectionNameIIS7 & "/add[@" & keyNameIIS7 & "='" & targetChildKey & "']", action, keyNameIIS7, StandardComplexNodeInfo.NodeCollision.overwrite)
                        End If
                    End If
                    Dim addNode6 As WebServerAddInfo = Me.BuildAddNode(False)
                    node.Children.Add(addNode6)
                    targetNodes.Nodes.Add(node)
                    If Not OnlyIIS6 Then
                        Dim addNode7 As WebServerAddInfo = Me.BuildAddNode(True)
                        node2.Children.Add(addNode7)
                        targetNodes.Nodes.Add(node2)
                    End If
                Case ConfigActionType.Uninstall
                    Dim node As New NodeInfo(NukeHelper.DefineWebServerElementPath("system.web") & "/" & sectionNameIIS6 & "/add[@" & keyNameIIS6 & "='" & Me.GetKeyIIS6 & "']", NodeInfo.NodeAction.remove)
                    targetNodes.Nodes.Add(node)
                    If Not OnlyIIS6 Then
                        Dim node2 As New NodeInfo(NukeHelper.DefineWebServerElementPath("system.webServer") & "/" & sectionNameIIS7 & "/add[@" & keyNameIIS7 & "='" & Me.GetKeyIIS7 & "']", NodeInfo.NodeAction.remove)
                        targetNodes.Nodes.Add(node2)
                    End If

            End Select
        End Sub

    End Class


End Namespace


