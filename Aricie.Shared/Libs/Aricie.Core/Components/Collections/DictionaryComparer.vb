Namespace Collections
    ''' <summary>
    ''' Generic Dictionary comparer for custom sorts
    ''' </summary>
    Public Class DictionaryComparer(Of TKey, TValue, TInput)
        Implements IComparer(Of TInput)

        Private _dico As SerializableDictionary(Of TKey, TValue)
        Private _Converter As Converter(Of TInput, TKey)
        Private _ValueComparer As IComparer(Of TValue) = Comparer(Of TValue).Default

        Sub New(ByVal dico As SerializableDictionary(Of TKey, TValue), ByVal converter As Converter(Of TInput, TKey), ByVal valueComparer As IComparer(Of TValue))
            _dico = dico
            _Converter = converter
            If valueComparer IsNot Nothing Then
                _ValueComparer = valueComparer
            End If
        End Sub


        Public Function GetValue(ByVal input As TInput, ByVal defaultValue As TValue) As TValue
            Dim toReturn As TValue
            Dim convertedInput As TKey = _Converter(input)
            If convertedInput IsNot Nothing Then
                If Me._dico.TryGetValue(convertedInput, toReturn) Then
                    Return toReturn
                End If
            End If
            Return defaultValue
        End Function

        Public Function Compare(ByVal x As TInput, ByVal y As TInput) As Integer Implements System.Collections.Generic.IComparer(Of TInput).Compare

            Dim convertedX As TKey = _Converter(x)
            Dim convertedY As TKey = _Converter(y)

            Dim valueX, valueY As TValue

            'Try Get Values
            _dico.TryGetValue(convertedX, valueX)
            _dico.TryGetValue(convertedY, valueY)

            'Check if 1 or more values are Nothing
            If valueX Is Nothing AndAlso valueY Is Nothing Then
                Return 0
            ElseIf valueX Is Nothing Then
                Return -1
            ElseIf valueY Is Nothing Then
                Return 1
            End If

            Return _ValueComparer.Compare(valueX, valueY)
        End Function
    End Class
End Namespace