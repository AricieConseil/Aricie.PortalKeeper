Imports System.Xml.Serialization
Imports System.Xml

Namespace Configuration



    '<XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    <XmlInclude(GetType(StandardComplexNodeInfo))> _
      <XmlRoot("node")> _
    <Serializable()> _
    Public Class ComplexNodeInfo
        Inherits NodeInfo

        '<node path="/configuration/system.web/httpModules" action="update" key="name" collision="overwrite">
        '     <add name="Compression" type="DotNetNuke.HttpModules.Compression.CompressionModule, DotNetNuke.HttpModules" />
        '</node>

        Private _Key As String = ""
        Private _Collision As NodeCollision = NodeCollision.overwrite
        Private _TargetPath As String = ""

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
        Public Property Key() As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                _Key = value
            End Set
        End Property


        <XmlAttribute("collision")> _
        Public Property Collision() As NodeCollision
            Get
                Return _Collision
            End Get
            Set(ByVal value As NodeCollision)
                _Collision = value
            End Set
        End Property

        <XmlAttribute("targetpath")> _
        Public Property TargetPath() As String
            Get
                Return _TargetPath
            End Get
            Set(ByVal value As String)
                _TargetPath = value
            End Set
        End Property


    End Class


    ' <XmlRoot("node")> _
    'Public Class CustomComplexNodeInfo
    '     Inherits StandardComplexNodeInfo
    '     Implements IXmlSerializable






    '     Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal key As String, ByVal collision As NodeCollision)
    '         MyBase.New(path, action, key, collision)
    '     End Sub

    '     Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal collision As NodeCollision, ByVal targetPath As String)
    '         MyBase.New(path, action, collision, targetPath)
    '     End Sub

    '     Public Sub New()
    '         MyBase.New()
    '     End Sub


    '     Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
    '         Return Nothing
    '     End Function

    '     Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
    '         'reader.ReadStartElement("add")
    '         If reader.HasAttributes Then
    '             Dim attributeCount As Integer = reader.AttributeCount
    '             Dim i As Integer
    '             For i = 0 To attributeCount - 1
    '                 reader.MoveToAttribute(i)
    '                 Select Case reader.Name
    '                     Case "path"
    '                         Me.Path = reader.GetAttribute(i)
    '                     Case "action"
    '                         Me.Action = Aricie.Common.GetEnum(Of NodeAction)(reader.GetAttribute(i))
    '                     Case "collision"
    '                         Me.Collision = Aricie.Common.GetEnum(Of NodeCollision)(reader.GetAttribute(i))
    '                     Case "key"
    '                         Me.Key = reader.GetAttribute(i)
    '                     Case "targetpath"
    '                         Me.TargetPath = reader.GetAttribute(i)
    '                 End Select
    '             Next i


    '         End If
    '         'reader.ReadEndElement()
    '     End Sub

    '     Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml

    '     End Sub
    ' End Class

End Namespace