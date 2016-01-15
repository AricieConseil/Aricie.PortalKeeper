Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports Aricie.ComponentModel
Imports Aricie.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Collections
    ''' <summary>
    ''' Generic Serializable Dictionary with a self contained sub types generic serialization mechanism
    ''' </summary>
    
    <JsonConverter(GetType(SerializableDictionaryJsonSerializer))> _
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

        Public Function GetKeysList() As IList(Of TKey)
            Return New List(Of TKey)(Me.Keys)
        End Function

        Public Function GetValuesList() As IList(Of TValue)
            Return New List(Of TValue)(Me.Values)
        End Function

        Public  Function  GetDictionary () As Dictionary(Of TKey,TValue)
            Return new Dictionary(Of TKey,TValue)(me)
        End Function

    End Class


    ''' <summary>
    ''' Generic Sorted Dictionary with a self contained sub types generic serialization mechanism
    ''' </summary>
    
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




     Public Class SerializableDictionaryJsonSerializer
            Inherits JsonConverter
        Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
            

            Dim objDico As Object = directcast( ReflectionHelper.GetMembersDictionary(value.GetType(),True, False)("GetDictionary"), MethodInfo).Invoke(value, Nothing)
            Dim previousTypeNameHandling = serializer.TypeNameHandling
            serializer.TypeNameHandling = TypeNameHandling.Auto
            serializer.Serialize(writer, objDico)
            serializer.TypeNameHandling = previousTypeNameHandling

        End Sub

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, objSerializer As JsonSerializer) As Object

              Dim toReturn as Object '= existingValue
            if toReturn Is nothing
                toReturn = ReflectionHelper.CreateObject(objectType)
            End If

           Dim jsonobject As JObject = JObject.Load(reader)
            Dim deserialized As Object 
            
            Dim settings As New JsonSerializerSettings() With {.TypeNameHandling = TypeNameHandling.All}
            settings.SetDefaultSettings()
           deserialized= JsonConvert.DeserializeObject(jsonobject.ToString(), objectType.BaseType, settings)

            
            Dim deserializedList As IDictionary = DirectCast( deserialized, IDictionary)
            For Each k As Object In deserializedList.Keys
               DirectCast( toReturn, IDictionary)(k) = deserializedList(k)
            Next
            
            Return toReturn
            

        End Function

        Public Overrides Function CanConvert(objectType As Type) As Boolean
            Return GetType(SerializableDictionary(Of, )).IsAssignableFrom(objectType)
        End Function
    End Class


End Namespace



