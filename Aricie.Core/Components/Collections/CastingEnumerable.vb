Namespace Collections
    Public Class CastingEnumerable(Of TKey As TValue, TValue)
        Inherits ConvertedEnumerable(Of TKey, TValue)
        Public Sub New(ByVal objList As IEnumerable(Of TKey))
            MyBase.New(objList, AddressOf CastingEnumerator(Of TKey, TValue).CastConvert)

        End Sub


    End Class
End Namespace