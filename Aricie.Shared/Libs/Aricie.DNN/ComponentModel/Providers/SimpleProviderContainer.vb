Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports System.Web.UI.WebControls

Namespace ComponentModel
    Public MustInherit Class SimpleProviderContainer(Of T)
        Implements IProviderContainer


        <ProvidersSelector("Text", "Value")> _
        Public Overridable Property Items As New SerializableList(Of T)


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Dim toReturn As New ListItemCollection()
            Select Case propertyName
                Case "Items"
                    For Each objPair As KeyValuePair(Of String, String) In GetProviderIdsByNames()
                        toReturn.Add(New ListItem(objPair.Key, objPair.Value))
                    Next
            End Select
            Return toReturn
        End Function

        Public Function GetNewItem(collectionPropertyName As String, providerName As String) As Object Implements IProviderContainer.GetNewItem
            Dim toReturn As Object = Nothing
            Select Case collectionPropertyName
                Case "Items"
                    toReturn = GetNewItem(providerName)
            End Select
            Return toReturn
        End Function


        Public MustOverride Function GetProviderIdsByNames() As IEnumerable(Of KeyValuePair(Of String, String))

        Public MustOverride Function GetNewItem(providerId As String) As T

    End Class
End Namespace