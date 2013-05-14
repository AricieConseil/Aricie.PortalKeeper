Imports System.Xml.Serialization

Namespace Configuration

    '<XmlRoot("node")> _
    'Public Class CustomComplexNodeInfo
    '    Inherits ComplexNodeInfo
    '    Implements IXmlSerializable

    '    Private _Children As New List(Of CustomAddInfo)

    '    Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal key As String, ByVal collision As NodeCollision)
    '        MyBase.New(path, action, key, collision)
    '    End Sub

    '    Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal collision As NodeCollision, ByVal targetPath As String)
    '        MyBase.New(path, action, collision, targetPath)
    '    End Sub

    '    Public Sub New()
    '        MyBase.New()
    '    End Sub


    '    Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
    '        Return Nothing
    '    End Function

    '    Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml

    '        If reader.HasAttributes Then
    '            Dim attributeCount As Integer = reader.AttributeCount
    '            Dim i As Integer
    '            For i = 0 To attributeCount - 1
    '                reader.MoveToAttribute(i)
    '                Select Case reader.Name
    '                    Case "path"
    '                        Me.Path = reader.GetAttribute(i)
    '                    Case "action"
    '                        Me.Action = Aricie.Common.GetEnum(Of NodeAction)(reader.GetAttribute(i))
    '                    Case "key"
    '                        Me.Key = reader.GetAttribute(i)
    '                    Case "collision"
    '                        Me.Collision = Aricie.Common.GetEnum(Of NodeCollision)(reader.GetAttribute(i))
    '                    Case "targetPath"
    '                        Me.TargetPath = reader.GetAttribute(i)
    '                End Select
    '            Next
    '            While reader.MoveToElement
    '                Dim newElt As New CustomAddInfo()
    '                newElt.ReadXml(reader)
    '                Me._Children.Add(newElt)
    '            End While
    '        End If

    '    End Sub

    '    Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml

    '        writer.WriteAttributeString("path", Me.Path)
    '        writer.WriteAttributeString("action", Me.Action.ToString)
    '        If Not String.IsNullOrEmpty(Me.Key) Then
    '            writer.WriteAttributeString("key", Me.Key)
    '        End If
    '        writer.WriteAttributeString("collision", Me.Collision.ToString)
    '        If Not String.IsNullOrEmpty(Me.TargetPath) Then
    '            writer.WriteAttributeString("targetPath", Me.TargetPath)
    '        End If
    '        For Each objCustom As CustomAddInfo In Me.Children
    '            writer.WriteStartElement(objCustom.ElementName)
    '            objCustom.WriteXml(writer)
    '            writer.WriteEndElement()
    '        Next
    '    End Sub


    '    Public Property Children() As List(Of CustomAddInfo)
    '        Get
    '            Return _Children
    '        End Get
    '        Set(ByVal value As List(Of CustomAddInfo))
    '            _Children = value
    '        End Set
    '    End Property

    'End Class





    ' <XmlInclude(GetType(CustomErrorAddInfo))> _
    '<XmlInclude(GetType(CustomErrorsAddInfo))> _
    '<XmlInclude(GetType(TrustAddInfo))> _
    <XmlRoot("node")> _
    <Serializable()> _
    Public Class StandardComplexNodeInfo
        Inherits ComplexNodeInfo


        Private _Children As New List(Of AddInfo)

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal key As String, ByVal collision As NodeCollision)
            MyBase.New(path, action, key, collision)
        End Sub

        Public Sub New(ByVal path As String, ByVal action As NodeAction, ByVal collision As NodeCollision, ByVal targetPath As String)
            MyBase.New(path, action, collision, targetPath)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        '<XmlElement( GetType(AddInfo))> _
        '<XmlElement( GetType(TrustAddInfo))> _
        '<XmlElement( GetType(CustomErrorsAddInfo))> _
        '<XmlElement( GetType(CustomErrorAddInfo))> _
        <XmlElement("trust", GetType(TrustAddInfo))> _
        <XmlElement("customErrors", GetType(CustomErrorsAddInfo))> _
        <XmlElement("error", GetType(CustomErrorAddInfo))> _
        <XmlElement("add", GetType(AddInfo))> _
        Public Property Children() As List(Of AddInfo)
            Get
                Return _Children
            End Get
            Set(ByVal value As List(Of AddInfo))
                _Children = value
            End Set
        End Property


    End Class
End Namespace