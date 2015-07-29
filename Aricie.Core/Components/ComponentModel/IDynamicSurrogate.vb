Namespace ComponentModel
    Public Interface IDynamicSurrogate

        Function ConvertToSurrogate(ByVal sourceObject As Object) As Object

        Function ConvertFromSurrogate(ByVal surrogateObject As Object) As Object

    End Interface
End NameSpace