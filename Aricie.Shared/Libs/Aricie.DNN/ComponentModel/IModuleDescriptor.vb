Namespace ComponentModel
    Public Interface IModuleDescriptor

        Function GetModel() As IModelDescriptor

        Function GetEntityView(ByVal entityName As String) As IEntityView



    End Interface
End Namespace

