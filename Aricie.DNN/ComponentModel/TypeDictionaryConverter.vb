Imports System.ComponentModel
Imports System.Globalization
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services

Namespace ComponentModel
    Public Class TypeDictionaryConverter

        Public Property SelectedType As New DotNetType()


        Public Property ParsingCulture As New CulturePicker

        Public Property CustomObjectToDictionary As New EnabledFeature(Of Variables(Of String))

        <Browsable(False)> _
        Public ReadOnly Property CustomToDictionaryIsEnabeld As Boolean
            Get
                Return CustomObjectToDictionary.Enabled
            End Get
        End Property


        <ConditionalVisible("CustomToDictionaryIsEnabeld")> _
        Public Property CustomObjectVariableName As String = "CustomObject"

        Public Property CustomDictionaryToObject As New EnabledFeature(Of Variables)

        <Browsable(False)> _
        Public ReadOnly Property CustomToObjectIsEnabeld As Boolean
            Get
                Return CustomDictionaryToObject.Enabled
            End Get
        End Property

        <ConditionalVisible("CustomToObjectIsEnabeld")> _
        Public Property CustomDictionaryVariableName As String = "CustomDictionary"

        <ConditionalVisible("CustomToObjectIsEnabeld", True)> _
        Public Property CreateEmptyObjects As Boolean

        <ConditionalVisible("CustomToObjectIsEnabeld", True)> _
        <ConditionalVisible("CreateEmptyObjects")> _
        Public Property InitializeLists As Boolean


        Public Function GetList(objContext As IContextLookup, values As IEnumerable(of Object) )As List(Of Dictionary(Of String, String))
            dim toReturn as New List(Of Dictionary(Of String, String))
            For Each value As Object In values
                toReturn.Add(GetDictionary(objContext, value))
            Next
            Return toReturn
        End Function

        Public Function GetDictionary(objContext As IContextLookup, value As Object) As Dictionary(Of String, String)
            If Not CustomObjectToDictionary.Enabled Then
                Return ObjectExtensions.AsStringDictionary(value, ParsingCulture.GetCulture())
            End If
            objContext.Items(CustomObjectVariableName) = value
            Dim objectDictionary As Dictionary(Of String, Object) = Me.CustomObjectToDictionary.Entity.EvaluateVariables(objContext, objContext)

            Return objectDictionary.ToDictionary(Function(objPair) objPair.Key, Function(objPair) DirectCast(objPair.Value, IConvertible).ToString(CultureInfo.InvariantCulture))
        End Function


        Public Function GetObject(objContext As IContextLookup, value As Dictionary(Of String, String)) As Object
            If Not CustomObjectToDictionary.Enabled Then
                Return Me.SelectedType.GetDotNetType().ToObject(value, ParsingCulture.GetCulture(), CreateEmptyObjects, InitializeLists)
            End If
            objContext.Items(CustomDictionaryVariableName) = value
            Dim objectDictionary As Dictionary(Of String, Object) = Me.CustomDictionaryToObject.Entity.EvaluateVariables(objContext, objContext)
            Return Me.SelectedType.GetDotNetType().ToObject(objectDictionary)
        End Function

    End Class
End NameSpace