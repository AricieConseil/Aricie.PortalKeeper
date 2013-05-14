Namespace Collections
    ''' <summary>
    ''' Automatic Converter Wrapping Enumerable
    ''' </summary>
    ''' <remarks>The conversion is performed only on sequential access.</remarks>
    Public Class ConvertedEnumerable(Of TKey, TValue)
        Implements IEnumerable(Of TValue)

        Protected collection As IEnumerable(Of TKey)
        Protected converter As Converter(Of TKey, TValue)


        Public Sub New(ByVal objList As IEnumerable(Of TKey), ByVal objConverter As Converter(Of TKey, TValue))
            Me.collection = objList
            Me.converter = objConverter
        End Sub


        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of TValue) Implements System.Collections.Generic.IEnumerable(Of TValue).GetEnumerator
            Return New ConvertedEnumerator(Of TKey, TValue)(Me.collection.GetEnumerator, Me.converter)
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function
    End Class
End Namespace