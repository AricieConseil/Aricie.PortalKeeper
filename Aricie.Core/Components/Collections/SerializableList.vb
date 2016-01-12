Imports System.Xml.Serialization
Imports Aricie.Services
Imports System.Linq
Imports System.Xml

Namespace Collections
    ''' <summary>
    ''' Generic List with a self contained sub types generic serialization mechanism
    ''' </summary>
    
    Public Class SerializableList(Of T)
        Inherits List(Of T)
        Implements IXmlSerializable


        Private Shared _SubTypesSerializer As XmlSerializer = ReflectionHelper.GetSerializer(Of List(Of String))("SubTypes")

        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(ByVal collection As IEnumerable(Of T))
            MyBase.New(collection)
        End Sub

        Public Sub New(ByVal collection As IEnumerable)
            MyBase.New()
            For Each objItem As T In collection
                Me.Add(objItem)
            Next
        End Sub

        Public Sub New(ByVal capacity As Integer)
            MyBase.New(capacity)
        End Sub



        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml
            Dim depth As Integer = reader.Depth
            reader.ReadStartElement()
            Dim typeNames As List(Of String) = Nothing
            Try
                typeNames = DirectCast(_SubTypesSerializer.Deserialize(reader), List(Of String))

                Dim serializer As XmlSerializer
                If typeNames IsNot Nothing AndAlso typeNames.Count > 0 Then
                    Dim types As List(Of Type) = (From strType In typeNames _
                             Select objType = ReflectionHelper.CreateType(ReflectionHelper.GetSafeTypeName(strType), False) _
                             Where objType IsNot Nothing).ToList()
                    serializer = ReflectionHelper.GetSerializer(Of List(Of T))(types.ToArray)
                Else
                    serializer = ReflectionHelper.GetSerializer(Of List(Of T))()
                End If

                Dim objList As List(Of T) = DirectCast(serializer.Deserialize(reader), List(Of T))
                For Each objT As T In objList
                    Me.Add(objT)
                Next
            Catch ex As Exception
                Dim strTypes As String = ""
                If typeNames IsNot Nothing Then
                    strTypes = String.Join(";", typeNames.ToArray())
                End If
                Dim newEx As New ApplicationException("Serializable list of " & GetType(T).AssemblyQualifiedName & " failed to deserialize, with list of inner types:" & strTypes, ex)
                ExceptionHelper.LogException(newEx)
                Dim limitCount As Integer = 0
                While reader.Depth > depth AndAlso limitCount < 100000
                    reader.Skip()
                    limitCount += 1
                End While
                If limitCount >= 100000 Then
                    Throw
                End If
            Finally
                reader.ReadEndElement()
            End Try
        End Sub



        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml
            Dim types As New List(Of Type)
            Dim typeNames As List(Of String) = Nothing
            If Me.Count > 0 Then
                For Each value As T In Me
                    Dim tempType As Type = value.GetType
                    If Not types.Contains(tempType) Then
                        types.Add(tempType)
                    End If
                Next
                If ReflectionHelper.AddSubtypes(writer, GetType(T)) Then
                    types = types.Union(ReflectionHelper.GetSerializerTypes(GetType(List(Of T)), "")).ToList()
                End If
               
                typeNames = (From objtype In types _
                                Where objtype IsNot GetType(T)
                                    Select ReflectionHelper.GetSafeTypeName(objtype)).ToList()
            Else
                typeNames = New List(Of String)
            End If
            _SubTypesSerializer.Serialize(writer, typeNames)

            Dim serializer As XmlSerializer = ReflectionHelper.GetSerializer(Of List(Of T))(types.ToArray)
            serializer.Serialize(writer, New List(Of T)(Me))
        End Sub

        'Public Class SubTypes
        '    Inherits List(Of String)

        'End Class

        Public Shared Function FromEnumerable(objCollec As IEnumerable) As SerializableList(Of T)
            Return New SerializableList(Of T)(objCollec)
        End Function

    End Class
End Namespace