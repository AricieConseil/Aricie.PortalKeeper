Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports System.Xml

Namespace Collections
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
End NameSpace