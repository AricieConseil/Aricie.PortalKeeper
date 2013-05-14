Namespace ComponentModel
    Public Interface ISelector
        Function GetSelector(ByVal propertyName As String) As IList

    End Interface

    Public Interface ISelector(Of T)
        Inherits ISelector

        Function GetSelectorG(ByVal propertyName As String) As IList(Of T)

    End Interface
End Namespace