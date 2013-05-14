Namespace ComponentModel
    Public Interface IEntityController

        Function GetItems(ByVal moduleId As Integer) As List(Of Object)

        Function GetItem(ByVal moduleId As Integer, ByVal ParamArray pKey() As Object) As Object

        Function AddItem(ByVal moduleId As Integer, ByVal item As Object) As Integer

        Sub DeleteItem(ByVal item As Object)

        Sub UpdateItem(ByVal item As Object)

    End Interface
End Namespace