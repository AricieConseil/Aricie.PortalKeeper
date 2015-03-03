Imports System.Xml.Serialization

Namespace Configuration


    <XmlInclude(GetType(StandardComplexNodeInfo))> _
      <XmlRoot("node")> _
    <Serializable()> _
    Public Class ComplexNodeInfo
        Inherits NodeInfo

        '<node path="/configuration/system.web/httpModules" action="update" key="name" collision="overwrite">
        '     <add name="Compression" type="DotNetNuke.HttpModules.Compression.CompressionModule, DotNetNuke.HttpModules" />
        '</node>

        Public Enum NodeCollision
            overwrite
            save
        End Enum

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal key As String, ByVal collision As NodeCollision)
            MyBase.New(path, action)
            Me._Key = key
            Me._Collision = collision
        End Sub

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal collision As NodeCollision, ByVal targetPath As String)
            MyBase.New(path, action)
            Me._TargetPath = targetPath
            Me._Collision = collision
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub


        <XmlAttribute("key")> _
        Public Property Key() As String  =""

        <XmlAttribute("collision")> _
        Public Property Collision() As NodeCollision = NodeCollision.overwrite

        <XmlAttribute("targetpath")> _
        Public Property TargetPath() As String = ""

    End Class


End Namespace