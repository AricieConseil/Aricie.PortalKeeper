Imports System.ComponentModel
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Xml.Schema
Imports Aricie.Services
Imports Fasterflect
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace ComponentModel


    <JsonConverter(GetType(SerializableJsonSerializer))>
    Public Class Serializable(Of T)
        Implements IXmlSerializable



        Public Sub New()

        End Sub

        Public Sub New(objValue As T)
            Me.Value = objValue
        End Sub

        
        <JsonProperty()> _
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


       Public Class SerializableJsonSerializer
        Inherits JsonConverter
        Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)

            writer.WriteStartObject()
            writer.WritePropertyName("TypeName")
            Dim serializableValue As Object = value.GetType().DelegateForGetPropertyValue("Value")(value)
            'dim tempContractResolver = serializer.ContractResolver
            'serializer.ContractResolver = WritablePropertiesOnlyResolver.InstanceWritable
            serializer.Serialize(writer, ReflectionHelper.GetSafeTypeName(serializableValue.GetType()))
            writer.WritePropertyName("Value")
            serializer.Serialize(writer, serializableValue)
            writer.WriteEndObject()
            'serializer.ContractResolver = tempContractResolver
            

        End Sub

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
            Dim toReturn as Object  = ReflectionHelper.CreateObject(objectType)

            Dim jsonobject As JObject = JObject.Load(reader)
            Dim targettypeproperty As JProperty = jsonobject.Property("TypeName")
            Dim targettype As Type = ReflectionHelper.CreateType(targettypeproperty.Value.ToString())
            Dim valueproperty As JProperty = jsonobject.Property("Value")
            Dim objvalue As Object 
            if targettype Is GetType(Boolean)
                objvalue = System.Convert.ChangeType(valueproperty.Value.ToString(Newtonsoft.Json.Formatting.None), targettype)
            Else 
                objvalue = JsonConvert.DeserializeObject(valueproperty.Value.ToString(Newtonsoft.Json.Formatting.None), targettype)
            End If
            
            toReturn.GetType().DelegateForSetPropertyValue("Value")(toReturn, objvalue)
            Return toReturn


            'todo: some alternate of that kind should work for a better layout
            'Dim targetProp As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(toReturn.GetType())("Value")
            'Dim previousTypeNameHandling = serializer.TypeNameHandling
            'serializer.TypeNameHandling = TypeNameHandling.Objects
            'Dim objValue As Object = serializer.Deserialize(reader, targetProp.PropertyType)
            'serializer.TypeNameHandling = previousTypeNameHandling
            'targetProp.SetValue(toReturn, objValue, Nothing)
            'Return toReturn

        End Function

        Public Overrides Function CanConvert(objectType As Type) As Boolean
            Return GetType(Serializable(Of )).IsAssignableFrom(objectType)
        End Function
    End Class

End Namespace