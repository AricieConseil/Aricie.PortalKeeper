Imports System.Xml.Serialization
Imports System.Xml
Imports System.Xml.Schema
Imports Aricie.Services

Namespace ComponentModel

    <Serializable()> _
    Public Class Serializable(Of T)
        Implements IXmlSerializable


        Public Sub New()

        End Sub

        Public Sub New(objValue As T)
            Me.Value = objValue
        End Sub


        <XmlIgnore()> _
        Public Property Value As T


        Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
            reader.MoveToContent()

            Dim isEmptyElement As [Boolean] = reader.IsEmptyElement
            reader.ReadStartElement()
            If Not isEmptyElement Then

                ReadXmlObjectProperty(reader, "Value", DirectCast(Value, Object))
                reader.ReadEndElement()
            End If
        End Sub

        Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml

            WriteXmlObjectProperty(writer, "Value", Value)
        End Sub

        Public Shared Function ReadXmlObjectProperty(reader As XmlReader, name As String, ByRef value As Object) As Boolean
            value = Nothing

            ' Moves to the element
            While Not reader.IsStartElement(name)
                Return False
            End While
            ' Get the serialized type
            Dim typeName As String = reader.GetAttribute("Type")

            Dim isEmptyElement As [Boolean] = reader.IsEmptyElement
            reader.ReadStartElement()
            If Not isEmptyElement Then
                Dim objType As Type = ReflectionHelper.CreateType(typeName)

                If objType IsNot Nothing Then
                    ' Deserialize it
                    'Dim serializer As New XmlSerializer(objType)
                    'value = serializer.Deserialize(reader)
                    value = ReflectionHelper.Deserialize(objType, reader)
                Else
                    ' Type not found within this namespace: get the raw string!
                    Dim xmlTypeName As String = typeName.Substring(typeName.LastIndexOf("."c) + 1)
                    value = reader.ReadElementString(xmlTypeName)
                End If
                reader.ReadEndElement()
            End If

            Return True
        End Function
        Public Shared Sub WriteXmlObjectProperty(writer As XmlWriter, name As String, value As Object)
            If value IsNot Nothing Then
                Dim valueType As Type = value.[GetType]()
                writer.WriteStartElement(name)
                writer.WriteAttributeString("Type", ReflectionHelper.GetSafeTypeName(valueType))
                writer.WriteRaw(ToXmlString(value, valueType))
                writer.WriteFullEndElement()
            End If
        End Sub

        Public Shared Function ToXmlString(item As Object, type As Type) As String

            Return ReflectionHelper.Serialize(item).OuterXml

        End Function


        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function


    End Class
End Namespace