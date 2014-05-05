Imports Aricie.DNN.ComponentModel

Namespace UI.WebControls.EditControls
    Public Interface IProviderContainer
        Inherits ISelector


        Function GetNewItem(ByVal collectionPropertyName As String, ByVal providerName As String) As Object

    End Interface
End NameSpace