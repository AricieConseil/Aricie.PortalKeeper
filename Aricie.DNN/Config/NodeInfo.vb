Imports System.Xml.Serialization

Namespace Configuration


    
    '<XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    <XmlInclude(GetType(SimpleNodeInfo))> _
    <XmlInclude(GetType(ComplexNodeInfo))> _
    <XmlInclude(GetType(StandardComplexNodeInfo))> _
    <XmlRoot("node")> _
    <Serializable()> _
    Public Class NodeInfo

        '<node path="/configuration/system.web/httpModules" action="update" key="name" collision="overwrite">
        '     <add name="Compression" type="DotNetNuke.HttpModules.Compression.CompressionModule, DotNetNuke.HttpModules" />
        '</node>

        Public Enum NodeAction
            add
            insertbefore
            insertafter
            remove
            removeattribute
            update
            updateattribute
        End Enum

        Private _Path As String = ""
        Private _Action As NodeAction = NodeAction.add

        Public Sub New()
        End Sub


        Public Sub New(ByVal path As String, ByVal action As NodeAction)
            Me._Path = path
            Me._Action = action
        End Sub



        <XmlAttribute("path")> _
        Public Property Path() As String
            Get
                Return _Path
            End Get
            Set(ByVal value As String)
                _Path = value
            End Set
        End Property


        <XmlAttribute("action")> _
        Public Property Action() As NodeAction
            Get
                Return _Action
            End Get
            Set(ByVal value As NodeAction)
                _Action = value
            End Set
        End Property


    End Class
End Namespace