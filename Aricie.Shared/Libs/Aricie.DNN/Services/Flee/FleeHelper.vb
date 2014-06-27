Imports System.Reflection
Imports Aricie.Services

Namespace Services.Flee
    Public Module FleeHelper

 
        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of Object, Object)()) As IDictionary
            Dim toReturn As New Dictionary(Of Object, Object)
            For Each objPair As KeyValuePair(Of Object, Object) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function

        ''' <summary>
        ''' Sets properties on an object through reflexion
        ''' </summary>
        ''' <param name="objTarget"></param>
        ''' <param name="prop1"></param>
        ''' <param name="obj1"></param>
        ''' <param name="prop2"></param>
        ''' <param name="obj2"></param>
        ''' <param name="prop3"></param>
        ''' <param name="obj3"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetProperties(ByVal objTarget As Object, ByVal prop1 As String, ByVal obj1 As Object, Optional ByVal prop2 As String = "", Optional ByVal obj2 As Object = Nothing, Optional ByVal prop3 As String = "", Optional ByVal obj3 As Object = Nothing) As Object
            Dim toReturn As Object = objTarget
            If toReturn IsNot Nothing Then
                Dim propDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(toReturn.GetType)
                Dim currentPropInfo As PropertyInfo = Nothing
                If Not String.IsNullOrEmpty(prop1) Then
                    If propDico.TryGetValue(prop1, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj1, Nothing)
                    End If
                End If
                If Not String.IsNullOrEmpty(prop2) Then
                    If propDico.TryGetValue(prop2, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj2, Nothing)
                    End If
                End If
                If Not String.IsNullOrEmpty(prop3) Then
                    If propDico.TryGetValue(prop3, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj3, Nothing)
                    End If
                End If
            End If
            Return toReturn
        End Function

        Public Function Format(ByVal tempalte As String, value As String) As String
            Return String.Format(tempalte, value)
        End Function

        Public Function Format(ByVal tempalte As String, value1 As String, value2 As String) As String
            Return String.Format(tempalte, value1, value2)
        End Function



    End Module

    ''' <summary>
    ''' Helper for flee manipulation
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class FleeHelper(Of T)

        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of Object, T)()) As IDictionary(Of Object, T)
            Dim toReturn As New Dictionary(Of Object, T)
            For Each objPair As KeyValuePair(Of Object, T) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function
        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of String, T)()) As IDictionary(Of String, T)
            Dim toReturn As New Dictionary(Of String, T)
            For Each objPair As KeyValuePair(Of String, T) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function



    End Class

End NameSpace