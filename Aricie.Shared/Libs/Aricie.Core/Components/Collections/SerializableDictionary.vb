Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml
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
                            reader.ReadEndElement()
                            If Not valueSerializers.TryGetValue(valueType, valueSerializer) Then
                                valueSerializer = ReflectionHelper.GetSerializer(valueType)
                                valueSerializers(valueType) = valueSerializer
                            End If
                        End If
                        Dim value As TValue = DirectCast(valueSerializer.Deserialize(reader), TValue)
                        reader.ReadEndElement()
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
            Dim key As TKey
            For Each key In dico.Keys
                Dim value As TValue = dico.Item(key)
                If value IsNot Nothing Then
                    writer.WriteStartElement("item")
                    writer.WriteStartElement("key")
                    Dim keyType As Type = key.GetType
                    If key.GetType.IsSubclassOf(GetType(TKey)) Then
                        writer.WriteStartElement("subtype")
                        writer.WriteValue(ReflectionHelper.GetSafeTypeName(keyType))
                        writer.WriteEndElement()
                    End If
                    If Not keySerializers.TryGetValue(keyType, keySerializer) Then
                        keySerializer = ReflectionHelper.GetSerializer(keyType)
                        keySerializers(keyType) = keySerializer
                    End If
                    keySerializer.Serialize(writer, key)
                    writer.WriteEndElement()
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
                    valueSerializer.Serialize(writer, value)
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                End If
            Next
        End Sub


    End Class







    ''' <summary>
    ''' Generic Serializable Dictionary with a self contained sub types generic serialization mechanism
    ''' </summary>
    <Serializable()> _
        <XmlRoot("dictionary")> _
    Public Class SerializableDictionary(Of TKey, TValue)
        Inherits Dictionary(Of TKey, TValue)
        Implements IXmlSerializable

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue))
            MyBase.New(cloneDictionary)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey))
            MyBase.New(comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            MyBase.New(capacity)
        End Sub


        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue), ByVal comparer As IEqualityComparer(Of TKey))
            MyBase.New(cloneDictionary, comparer)
        End Sub

        Public Sub New(ByVal cloneDictionary As IEnumerable(Of KeyValuePair(Of TKey, TValue)))
            Me.New()
            For Each objPair As KeyValuePair(Of TKey, TValue) In cloneDictionary
                Me.Add(objPair.Key, objPair.Value)
            Next
        End Sub


        Public Sub New(ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of TKey))
            MyBase.New(capacity, comparer)
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub


        ' Methods
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            SerializationDictionaryHelper(Of TKey, TValue).ReadXml(reader, Me)
        End Sub

        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            SerializationDictionaryHelper(Of TKey, TValue).WriteXml(writer, Me)
        End Sub

        Public ReadOnly Property SubTree(ByVal key As TKey) As SerializableDictionary(Of TKey, TValue)
            Get
                Dim toReturn As TValue = Nothing
                If Me.TryGetValue(key, toReturn) Then
                    If toReturn IsNot Nothing Then
                        Return DirectCast(DirectCast(toReturn, Object), SerializableDictionary(Of TKey, TValue))
                    Else
                        toReturn = DirectCast(DirectCast(New SerializableDictionary(Of TKey, TValue), Object), TValue)
                        Me.Add(key, toReturn)
                    End If
                End If
                Return Nothing
            End Get
        End Property

        Public Function GetSubTree(key As TKey) As SerializableDictionary(Of TKey, TValue)
            Return Me.SubTree(key)
        End Function


        'Public Class SubTypes
        '    Inherits List(Of String)

        'End Class

    End Class


    ''' <summary>
    ''' Generic Sorted List with a self contained sub types generic serialization mechanism
    ''' </summary>
    <Serializable()> _
       <XmlRoot("sortedList")> _
    Public Class SerializableSortedList(Of TKey, TValue)
        Inherits SortedList(Of TKey, TValue)
        Implements IXmlSerializable

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue))
            MyBase.New(cloneDictionary)
        End Sub

        Public Sub New(ByVal comparer As IComparer(Of TKey))
            MyBase.New(comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            MyBase.New(capacity)
        End Sub


        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue), ByVal comparer As IComparer(Of TKey))
            MyBase.New(cloneDictionary, comparer)
        End Sub

        Public Sub New(ByVal cloneDictionary As IEnumerable(Of KeyValuePair(Of TKey, TValue)))
            Me.New()
            For Each objPair As KeyValuePair(Of TKey, TValue) In cloneDictionary
                Me.Add(objPair.Key, objPair.Value)
            Next
        End Sub


        Public Sub New(ByVal capacity As Integer, ByVal comparer As IComparer(Of TKey))
            MyBase.New(capacity, comparer)
        End Sub



        ' Methods
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            SerializationDictionaryHelper(Of TKey, TValue).ReadXml(reader, Me)
        End Sub

        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            SerializationDictionaryHelper(Of TKey, TValue).WriteXml(writer, Me)
        End Sub

        Public ReadOnly Property SubTree(ByVal key As TKey) As SerializableSortedList(Of TKey, TValue)
            Get
                Dim toReturn As TValue = Nothing
                If Me.TryGetValue(key, toReturn) Then
                    If toReturn IsNot Nothing Then
                        Return DirectCast(DirectCast(toReturn, Object), SerializableSortedList(Of TKey, TValue))
                    Else
                        toReturn = DirectCast(DirectCast(New SerializableSortedList(Of TKey, TValue), Object), TValue)
                        Me.Add(key, toReturn)
                    End If
                End If
                Return Nothing
            End Get
        End Property

        Public Function GetSubTree(key As TKey) As SerializableSortedList(Of TKey, TValue)
            Return Me.SubTree(key)
        End Function


        'Public Class SubTypes
        '    Inherits List(Of String)

        'End Class

    End Class

    ''' <summary>
    ''' Generic Sorted Dictionary with a self contained sub types generic serialization mechanism
    ''' </summary>
    <Serializable()> _
      <XmlRoot("sortedDictionary")> _
    Public Class SerializableSortedDictionary(Of TKey, TValue)
        Inherits SortedDictionary(Of TKey, TValue)
        Implements IXmlSerializable

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue))
            MyBase.New(cloneDictionary)
        End Sub

        Public Sub New(ByVal comparer As IComparer(Of TKey))
            MyBase.New(comparer)
        End Sub


        Public Sub New(ByVal cloneDictionary As IDictionary(Of TKey, TValue), ByVal comparer As IComparer(Of TKey))
            MyBase.New(cloneDictionary, comparer)
        End Sub

        Public Sub New(ByVal cloneDictionary As IEnumerable(Of KeyValuePair(Of TKey, TValue)))
            Me.New()
            For Each objPair As KeyValuePair(Of TKey, TValue) In cloneDictionary
                Me.Add(objPair.Key, objPair.Value)
            Next
        End Sub




        ' Methods
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            SerializationDictionaryHelper(Of TKey, TValue).ReadXml(reader, Me)
        End Sub

        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            SerializationDictionaryHelper(Of TKey, TValue).WriteXml(writer, Me)
        End Sub

        Public ReadOnly Property SubTree(ByVal key As TKey) As SerializableSortedDictionary(Of TKey, TValue)
            Get
                Dim toReturn As TValue = Nothing
                If Me.TryGetValue(key, toReturn) Then
                    If toReturn IsNot Nothing Then
                        Return DirectCast(DirectCast(toReturn, Object), SerializableSortedDictionary(Of TKey, TValue))
                    Else
                        toReturn = DirectCast(DirectCast(New SerializableSortedDictionary(Of TKey, TValue), Object), TValue)
                        Me.Add(key, toReturn)
                    End If
                End If
                Return Nothing
            End Get
        End Property

        Public Function GetSubTree(key As TKey) As SerializableSortedDictionary(Of TKey, TValue)
            Return Me.SubTree(key)
        End Function


        'Public Class SubTypes
        '    Inherits List(Of String)

        'End Class

    End Class


End Namespace



