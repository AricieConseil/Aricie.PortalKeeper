


Namespace Collections


    ''' <summary>
    ''' Converted Enumerator where the conversion is a cast
    ''' </summary>
    Public Class CastingEnumerator(Of Tkey As TValue, TValue)
        Inherits ConvertedEnumerator(Of Tkey, TValue)


        Public Sub New(ByVal baseEnumerator As IEnumerator(Of Tkey))
            MyBase.New(baseEnumerator, New Converter(Of Tkey, TValue)(AddressOf CastConvert))
        End Sub

        Public Shared Function CastConvert(ByVal key As Tkey) As TValue
            Return DirectCast(key, TValue)
        End Function

    End Class

    ''' <summary>
    ''' utomatic Converter Wrapping Enumerator
    ''' </summary>
    Public Class ConvertedEnumerator(Of Tkey, TValue)
        Implements IEnumerator(Of TValue)



        Private _BaseEnumerator As IEnumerator(Of Tkey)
        Private _Converter As Converter(Of Tkey, TValue)

        Public Sub New(ByVal baseEnumerator As IEnumerator(Of Tkey), ByVal objConverter As Converter(Of Tkey, TValue))
            Me._BaseEnumerator = baseEnumerator
            Me._Converter = objConverter
        End Sub


        Public ReadOnly Property Current() As TValue Implements System.Collections.Generic.IEnumerator(Of TValue).Current
            Get
                Return Me._Converter.Invoke(_BaseEnumerator.Current)
            End Get
        End Property

        Public ReadOnly Property Current1() As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Me.Current
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            Return Me._BaseEnumerator.MoveNext
        End Function

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            Me._BaseEnumerator.Reset()
        End Sub

        Public Sub Dispose() Implements System.IDisposable.Dispose
            Me._BaseEnumerator.Dispose()
        End Sub
    End Class


End Namespace



