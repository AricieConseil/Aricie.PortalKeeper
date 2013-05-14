Namespace Collections
    ''' <summary>
    ''' Generic class to wrap a collection of convertible items into their converted counterparts
    ''' </summary>
    Public Class ConvertedCollection(Of TKey, TValue)
        Inherits ConvertedEnumerable(Of TKey, TValue)
        Implements ICollection(Of TValue)


        Private comparer As IEqualityComparer(Of TValue)



        Public Sub New(ByVal objList As ICollection(Of TKey), ByVal objConverter As Converter(Of TKey, TValue))
            Me.New(objList, objConverter, Nothing)
        End Sub



        Public Sub New(ByVal objList As ICollection(Of TKey), ByVal objConverter As Converter(Of TKey, TValue), ByVal objComparer As IEqualityComparer(Of TValue))
            MyBase.New(objList, objConverter)
            If objComparer IsNot Nothing Then
                Me.comparer = objComparer
            Else
                Me.comparer = EqualityComparer(Of TValue).Default
            End If
        End Sub


        Public Sub Add(ByVal item As TValue) Implements System.Collections.Generic.ICollection(Of TValue).Add
            Throw New NotImplementedException()
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of TValue).Clear
            Throw New NotImplementedException()
        End Sub

        Public Function Contains(ByVal item As TValue) As Boolean Implements System.Collections.Generic.ICollection(Of TValue).Contains
            For Each local As TKey In Me.collection
                If Me.comparer.Equals(Me.converter(local), item) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub CopyTo(ByVal array() As TValue, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of TValue).CopyTo
            Dim i As Integer = 0
            For Each key As TKey In Me.collection
                array(arrayIndex + i) = Me.converter(key)
                i += 1
            Next
        End Sub

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of TValue).Count
            Get
                Return DirectCast(Me.collection, ICollection(Of TKey)).Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of TValue).IsReadOnly
            Get
                Return True
            End Get
        End Property

        Public Function Remove(ByVal item As TValue) As Boolean Implements System.Collections.Generic.ICollection(Of TValue).Remove
            Throw New NotImplementedException()
        End Function





    End Class
End Namespace