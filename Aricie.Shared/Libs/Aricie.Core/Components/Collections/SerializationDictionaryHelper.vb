Imports System.Xml
Imports System.Xml.Serialization
Imports Aricie.Services
Imports Aricie.Providers

Namespace Collections
    ''' <summary>
    ''' Helper classes to deal with the self contained serialization container of type hierarchies
    ''' </summary>
    Public Class SerializationDictionaryHelper(Of TKey, TValue)

        Public Shared Sub ReadXml(ByVal reader As XmlReader, dico As IDictionary(Of TKey, TValue))
            'Dim keySerializer As New XmlSerializer(GetType(TKey))
            'Dim valueSerializer As New XmlSerializer(GetType(TValue))


            Dim valueSerializers As New Dictionary(Of Type, XmlSerializer)
            Dim keySerializers As New Dictionary(Of Type, XmlSerializer)

            'Dim typeSerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of String)()

            Dim nativeKeySerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of TKey)()
            Dim nativeValueSerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of TValue)()

            Dim keySerializer, valueSerializer As XmlSerializer
            Dim wasEmpty As Boolean = reader.IsEmptyElement
            reader.Read()

            If Not wasEmpty Then
                Do While (reader.NodeType <> XmlNodeType.EndElement)
                    Dim depth As Integer = reader.Depth
                    reader.ReadStartElement("item")
                    Try
                        reader.ReadStartElement("key")
                        keySerializer = nativeKeySerializer
                        If reader.IsStartElement("subtype") Then
                            reader.ReadStartElement("subtype")
                            Dim strType As String = reader.ReadContentAsString
                            reader.ReadEndElement()
                            Dim keyType As Type
                            Try
                                keyType = ReflectionHelper.CreateType(strType)
                            Catch ex As Exception
                                keyType = ReflectionHelper.CreateType(ReflectionHelper.GetSafeTypeName(strType))
                            End Try
                            If Not keySerializers.TryGetValue(keyType, keySerializer) Then
                                keySerializer = ReflectionHelper.GetSerializer(keyType)
                                keySerializers(keyType) = keySerializer
                            End If
                        End If
                        Dim key As TKey = DirectCast(keySerializer.Deserialize(reader), TKey)
                        reader.ReadEndElement()
                        Dim value As TValue = Nothing
                        If reader.IsStartElement("value") Then
                            reader.ReadStartElement("value")
                            valueSerializer = nativeValueSerializer
                            If reader.IsStartElement("subtype") Then
                                reader.ReadStartElement("subtype")
                                Dim valueType As Type
                                Dim strType As String = reader.ReadContentAsString
                                Try
                                    valueType = ReflectionHelper.CreateType(strType)
                                Catch ex As Exception
                                    valueType = ReflectionHelper.CreateType(ReflectionHelper.GetSafeTypeName(strType))
                                End Try
                                If valueType Is Nothing Then
                                    Throw New ApplicationException(String.Format("Type {0} could not be created while deserializing dictionary", strType))
                                End If
                                reader.ReadEndElement()
                                If Not valueSerializers.TryGetValue(valueType, valueSerializer) Then
                                    valueSerializer = ReflectionHelper.GetSerializer(valueType)
                                    valueSerializers(valueType) = valueSerializer
                                End If
                            End If
                            value = DirectCast(valueSerializer.Deserialize(reader), TValue)
                            reader.ReadEndElement()
                        End If
                        dico.Add(key, value)
                    Catch ex As Exception
                        SystemServiceProvider.Instance.LogException(ex)
                        While reader.Depth > depth
                            reader.Skip()
                        End While
                    Finally
                        reader.ReadEndElement()
                        reader.MoveToContent()
                    End Try
                Loop
                reader.ReadEndElement()
            End If
        End Sub

        Public Shared Sub WriteXml(ByVal writer As XmlWriter, dico As IDictionary(Of TKey, TValue))
            Dim valueSerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of TValue)()
            Dim keySerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of TKey)()

            Dim valueSerializers As New Dictionary(Of Type, XmlSerializer)
            Dim keySerializers As New Dictionary(Of Type, XmlSerializer)
            For Each keyValue In dico
                writer.WriteStartElement("item")
                writer.WriteStartElement("key")
                Dim keyType As Type = keyValue.Key.GetType()
                If keyType.IsSubclassOf(GetType(TKey)) Then
                    writer.WriteStartElement("subtype")
                    writer.WriteValue(ReflectionHelper.GetSafeTypeName(keyType))
                    writer.WriteEndElement()
                End If
                If Not keySerializers.TryGetValue(keyType, keySerializer) Then
                    keySerializer = ReflectionHelper.GetSerializer(keyType)
                    keySerializers(keyType) = keySerializer
                End If
                keySerializer.Serialize(writer, keyValue.Key)
                writer.WriteEndElement()
                Dim value As TValue = keyValue.Value
                If value IsNot Nothing Then
                    writer.WriteStartElement("value")
                    Dim valueType As Type = value.GetType
                    If value.GetType.IsSubclassOf(GetType(TValue)) Then
                        writer.WriteStartElement("subtype")
                        writer.WriteValue(ReflectionHelper.GetSafeTypeName(valueType))
                        writer.WriteEndElement()
                    End If
                    If Not valueSerializers.TryGetValue(value.GetType, valueSerializer) Then
                        valueSerializer = ReflectionHelper.GetSerializer(valueType)
                        valueSerializers(valueType) = valueSerializer
                    End If
                    Try
                        valueSerializer.Serialize(writer, value)
                    Catch ex As Exception
                        LogException(ex)
                    End Try

                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            Next
        End Sub


    End Class
End NameSpace