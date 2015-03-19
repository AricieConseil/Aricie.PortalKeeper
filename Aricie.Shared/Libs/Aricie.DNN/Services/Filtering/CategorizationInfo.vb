Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Services.Filtering

    Public Enum CategorizationMode
        DictionaryContainsInput
        InputContainsKey
    End Enum

    <Serializable()> _
    Public Class CategorizationInfo

        Public Property DefaultToUnchanged As Boolean

        <ConditionalVisible("DefaultToUnchanged", True)> _
        Public Property DefaultValue As String = "N.A."

        <ExtendedCategory("", "Comparison")> _
        Public Property Mode As CategorizationMode = CategorizationMode.InputContainsKey

        <ConditionalVisible("Mode", False, True, CategorizationMode.DictionaryContainsInput)> _
        <ExtendedCategory("", "Comparison")> _
        Public Property Comparer As New ComparerInfo()

        <ConditionalVisible("Mode", False, True, CategorizationMode.InputContainsKey)> _
        <ExtendedCategory("", "Comparison")> _
        Public Property Comparizon As StringComparison = StringComparison.OrdinalIgnoreCase



        <CollectionEditor(False, True, False, True, 20, CollectionDisplayStyle.Accordion, True, 0, "Key")> _
        <Orientation(System.Web.UI.WebControls.Orientation.Vertical)> _
        Public Property SubTree As New SerializableSortedDictionary(Of String, List(Of String))


        Private _ReverseDictionary As SerializableDictionary(Of String, String)

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property ReverseDictionary As SerializableDictionary(Of String, String)
            Get
                If _ReverseDictionary Is Nothing Then
                    SyncLock Me
                        _ReverseDictionary = New SerializableDictionary(Of String, String)(Me.Comparer.GetComparer())
                        For Each objPair As KeyValuePair(Of String, List(Of String)) In Me.SubTree
                            objPair.Value.ForEach(Sub(objValue) _ReverseDictionary.Add(objValue, objPair.Key))
                        Next
                    End SyncLock
                End If
                Return _ReverseDictionary
            End Get
        End Property


        Public Function Match(value As String) As String
            Select Case Mode
                Case CategorizationMode.InputContainsKey
                    For Each objPair As KeyValuePair(Of String, String) In ReverseDictionary
                        If value.IndexOf(objPair.Key, Me.Comparizon) > -1 Then
                            Return objPair.Value
                        End If
                    Next
                Case CategorizationMode.DictionaryContainsInput
                    Dim toReturn As String = Nothing
                    If ReverseDictionary.TryGetValue(value, toReturn) Then
                        Return toReturn
                    End If
            End Select
            If Me.DefaultToUnchanged Then
                Return value
            Else
                Return DefaultValue
            End If
        End Function


    End Class
End NameSpace