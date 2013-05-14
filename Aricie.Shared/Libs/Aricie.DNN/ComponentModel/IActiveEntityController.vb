Namespace ComponentModel
    Public Interface IActiveEntityController
        Inherits IEntityController

        Function CreateItem(ByVal moduleId As Integer) As Object

        Function GetPrimaryKey(ByVal item As Object) As Object()

        Function GetItemsCount(ByVal moduleId As Integer) As Integer

        Function GetLastItem(ByVal moduleID As Integer) As Object

    End Interface
End Namespace