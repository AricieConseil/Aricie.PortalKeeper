Imports System.ComponentModel
Imports System.Linq
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Xml.Schema
Imports Aricie.Services
Imports Fasterflect
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace ComponentModel
    Public Class SerializableJsonSerializer
        Inherits JsonConverter
        Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
            
            writer.WriteStartObject()
            writer.WritePropertyName("Type")
            Dim serializableValue As Object =ReflectionHelper.GetPropertiesDictionary( value.GetType())("Value").GetValue(value, Nothing)
            serializer.Serialize(writer, ReflectionHelper.GetSafeTypeName( serializableValue.GetType()))
            writer.WritePropertyName("Value")
            serializer.Serialize(writer, serializableValue)
            writer.WriteEndObject()
        End Sub

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
            Dim toReturn As Object = existingValue
            Dim jsonObject As JObject = JObject.Load(reader)
            Dim targetTypeProperty as JProperty = jsonObject.Property("Type")
            dim targetType As Type = ReflectionHelper.CreateType(targetTypeProperty.Value.ToString())
            Dim valueProperty as JProperty = jsonObject.Property("Value")
            Dim objValue As Object = JsonConvert.DeserializeObject(valueProperty.Value.ToString(),targetType)
            ReflectionHelper.GetPropertiesDictionary( toReturn.GetType())("Value").SetValue(toReturn, objValue, Nothing)
            Return toreturn
        End Function

        Public Overrides Function CanConvert(objectType As Type) As Boolean
            Return GetType(Serializable(Of )).IsAssignableFrom(objectType)
        End Function
    End Class

    <JsonConverter(GetType(SerializableJsonSerializer))>
    Public Class Serializable(Of T)
        Implements IXmlSerializable



        Public Sub New()

        End Sub

        Public Sub New(objValue As T)
            Me.Value = objValue
        End Sub

        

        <XmlIgnore()>
        Public Property Value As T


        Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
            reader.MoveToContent()

            Dim isEmptyElement As [Boolean] = reader.IsEmptyElement
            reader.ReadStartElement()
            If Not isEmptyElement Then
                Dim readObject As Object = Nothing
                ReadXmlObjectProperty(reader, "Value", readObject)
                Value = DirectCast(readObject, T)
                reader.ReadEndElement()
            End If
        End Sub

        Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml

            WriteXmlObjectProperty(writer, "Value", Value)
        End Sub

        Public Shared Function ReadXmlObjectProperty(reader As XmlReader, name As String, ByRef objValue As Object) As Boolean
            objValue = Nothing

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
                    objValue = ReflectionHelper.Deserialize(objType, reader)
                Else
                    ' Type not found within this namespace: get the raw string!
                    Dim xmlTypeName As String = typeName.Substring(typeName.LastIndexOf("."c) + 1)
                    objValue = reader.ReadElementString(xmlTypeName)
                End If
                reader.ReadEndElement()
            End If

            Return True
        End Function
        Public Shared Sub WriteXmlObjectProperty(writer As XmlWriter, name As String, objValue As Object)
            If objValue IsNot Nothing Then
                Dim valueType As Type = objValue.[GetType]()
                writer.WriteStartElement(name)
                writer.WriteAttributeString("Type", ReflectionHelper.GetSafeTypeName(valueType))
                writer.WriteRaw(ToXmlString(objValue, valueType))
                writer.WriteFullEndElement()
            End If
        End Sub

        Public Shared Function ToXmlString(item As Object, type As Type) As String

            Return ReflectionHelper.Serialize(item, True).Beautify(True)

        End Function


        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function


    End Class
End Namespace