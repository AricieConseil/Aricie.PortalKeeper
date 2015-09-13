Imports System.Globalization
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.Services

Namespace ComponentModel
    Public Class TypeDictionaryConverter

        Public Property SelectedType As New DotNetType()

        Public Property CustomObjectToDictionary As New EnabledFeature(Of Variables(Of String))

        Public Property CustomDictionaryToObject As New EnabledFeature(Of Variables)

        Public Function GetDictionary(objContext As IContextLookup, value As Object) As IDictionary(Of String, String)
            If Not CustomObjectToDictionary.Enabled Then
                Return ObjectExtensions.AsStringDictionary(value)
            End If
            Dim objectDictionary As Dictionary(Of String, Object) = Me.CustomObjectToDictionary.Entity.EvaluateVariables(objContext, objContext)
            Return objectDictionary.ToDictionary(Function(objPair) objPair.Key, Function(objPair) DirectCast(objPair.Value, IConvertible).ToString(CultureInfo.InvariantCulture))
        End Function

        Public Function GetObject(objContext As IContextLookup, value As IDictionary(Of String, String)) As Object
            If Not CustomObjectToDictionary.Enabled Then
                Return Me.SelectedType.GetDotNetType().ToObject(value)
            End If
            Dim objectDictionary As Dictionary(Of String, Object) = Me.CustomDictionaryToObject.Entity.EvaluateVariables(objContext, objContext)
            Return Me.SelectedType.GetDotNetType().ToObject(objectDictionary)
        End Function

    End Class
End NameSpace