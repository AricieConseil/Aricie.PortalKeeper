Namespace ComponentModel
    Public Class DynamicSurrogate(Of TSource, TSurrogate)
        Implements IDynamicSurrogate


        Public Property ConvertTo As Func(Of TSource, TSurrogate)
        Public Property ConvertFrom As Func(Of TSurrogate, TSource)


        Public Function ConvertFromSurrogate(surrogateObject As Object) As Object Implements IDynamicSurrogate.ConvertFromSurrogate
            Return ConvertFrom.Invoke(DirectCast(surrogateObject, TSurrogate))
        End Function

        Public Function ConvertToSurrogate(sourceObject As Object) As Object Implements IDynamicSurrogate.ConvertToSurrogate
            Return ConvertTo.Invoke(DirectCast(sourceObject, TSource))
        End Function
    End Class
End NameSpace