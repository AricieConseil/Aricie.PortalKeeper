Imports System.ComponentModel

Namespace Collections


    ''' <summary>
    ''' Generic Dictionary with a dual list/hash table implementation
    ''' </summary>
    ''' <remarks>A configured thresold determines when to switch implementations</remarks>
    
    Public Class HybridDictionary(Of TKey, TValue)
        Implements System.Collections.Generic.IDictionary(Of TKey, TValue)


        Public Const DEFAULT_THRESOLD As Integer = 8

        Protected dict As Dictionary(Of TKey, TValue)
        Protected list As List(Of KeyValuePair(Of TKey, TValue))
        Protected thresold As Integer

        Private _Comparer As IEqualityComparer(Of TKey)

        <Browsable(False)> _
        Public Property Comparer() As IEqualityComparer(Of TKey)
            Get
                Return _Comparer
            End Get
            Set(ByVal value As IEqualityComparer(Of TKey))
                _Comparer = value
            End Set
        End Property



        Public Sub New()
            Me.New(0, Nothing)
        End Sub

        Public Sub New(ByVal dictionary As SerializableDictionary(Of TKey, TValue))
            Me.New(dictionary, Nothing, DEFAULT_THRESOLD)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey))
            Me.New(0, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            Me.New(capacity, Nothing)
        End Sub

        Public Sub New(ByVal dictionary As SerializableDictionary(Of TKey, TValue), ByVal comparer As IEqualityComparer(Of TKey), ByVal objThresold As Integer)
            Me.New(DirectCast(IIf((Not dictionary Is Nothing), dictionary.Count, 0), Integer), comparer, objThresold)
            If (dictionary Is Nothing) Then
                Throw New ArgumentNullException("dictionary")
            End If
            Dim pair As KeyValuePair(Of TKey, TValue)
            For Each pair In dictionary
                Me.Add(pair.Key, pair.Value)
            Next
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of TKey))
            Me.New(capacity, comparer, DEFAULT_THRESOLD)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of TKey), ByVal objThresold As Integer)
            Me.Initialize(capacity, comparer, objThresold)
        End Sub

        Private Sub Initialize(ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of TKey), ByVal objThresold As Integer)
            If (capacity < 0) Then
                Throw New ArgumentOutOfRangeException("capacity")
            End If
            Me.thresold = objThresold
            If (comparer Is Nothing) Then
                Me._Comparer = EqualityComparer(Of TKey).Default
            Else
                Me._Comparer = comparer
            End If
            If capacity > Me.thresold Then
                Me.list = New List(Of KeyValuePair(Of TKey, TValue))(Me.thresold)
                Me.dict = New Dictionary(Of TKey, TValue)(capacity, Me._Comparer)
            Else
                Me.list = New List(Of KeyValuePair(Of TKey, TValue))(capacity)
            End If
        End Sub


        'Private ReadOnly Property Dictionary() As Dictionary(Of TKey, TValue)
        '    Get
        '        If dict Is Nothing Then
        '            SwitchToDictionary()
        '        End If
        '        Return dict
        '    End Get
        'End Property

        Private Sub SwitchToDictionary()
            dict = New Dictionary(Of TKey, TValue)(list.Count, Me._Comparer)
            For Each objItem As KeyValuePair(Of TKey, TValue) In list
                dict.Add(objItem.Key, objItem.Value)
            Next
        End Sub

        Private Sub SwitchToList()
            list.Clear()
            For Each objItem As KeyValuePair(Of TKey, TValue) In dict
                list.Add(objItem)
            Next
            dict = Nothing
        End Sub

        Public Overloads Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of TKey, TValue)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Add
            Me.Add(item.Key, item.Value)
        End Sub

        Public Overridable Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Clear
            Me.list.Clear()
            Me.dict = Nothing
        End Sub

        Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of TKey, TValue)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Contains
            If Me.dict IsNot Nothing Then
                Return Me.dict.ContainsKey(item.Key)
            Else
                For Each local As KeyValuePair(Of TKey, TValue) In Me.list
                    If Me.Comparer.Equals(local.Key, item.Key) AndAlso System.Collections.Generic.EqualityComparer(Of TValue).Default.Equals(item.Value, local.Value) Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of TKey, TValue), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).CopyTo
            If Me.dict IsNot Nothing Then
                DirectCast(Me.dict, ICollection(Of KeyValuePair(Of TKey, TValue))).CopyTo(array, arrayIndex)
            Else
                Me.list.CopyTo(array, arrayIndex)
            End If
        End Sub

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Count
            Get
                If Me.dict IsNot Nothing Then
                    Return Me.dict.Count
                Else
                    Return Me.list.Count
                End If
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Overloads Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of TKey, TValue)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Remove
            Return Remove(item.Key)
        End Function

        Public Overridable Overloads Sub Add(ByVal key As TKey, ByVal value As TValue) Implements System.Collections.Generic.IDictionary(Of TKey, TValue).Add
            If Me.dict IsNot Nothing Then
                Me.dict.Add(key, value)
            ElseIf Me.list.Count >= Me.thresold Then
                Me.SwitchToDictionary()
                Me.dict.Add(key, value)
            Else
                Me.list.Add(New KeyValuePair(Of TKey, TValue)(key, value))
            End If
        End Sub

        Public Overridable Function ContainsKey(ByVal key As TKey) As Boolean Implements System.Collections.Generic.IDictionary(Of TKey, TValue).ContainsKey
            If (key Is Nothing) Then
                Throw New ArgumentNullException("key")
            End If
            If Me.dict IsNot Nothing Then
                Return Me.dict.ContainsKey(key)
            Else
                For Each local As KeyValuePair(Of TKey, TValue) In Me.list
                    If Me.Comparer.Equals(local.Key, key) Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        <Browsable(False)> _
        Default Public Property Item(ByVal key As TKey) As TValue Implements System.Collections.Generic.IDictionary(Of TKey, TValue).Item
            Get
                Dim toReturn As TValue = Nothing
                If Me.TryGetValue(key, toReturn) Then
                    Return toReturn
                End If
                Throw New KeyNotFoundException()
            End Get
            Set(ByVal value As TValue)
                Me.SetItem(key, value)
            End Set
        End Property


        <Browsable(False)> _
        Public ReadOnly Property Keys() As System.Collections.Generic.ICollection(Of TKey) Implements System.Collections.Generic.IDictionary(Of TKey, TValue).Keys
            Get
                If dict IsNot Nothing Then
                    Return Me.dict.Keys
                Else
                    Return New ConvertedCollection(Of KeyValuePair(Of TKey, TValue), TKey)(Me.list, _
                                            New Converter(Of KeyValuePair(Of TKey, TValue), TKey)(AddressOf Me.GetKey), Me._Comparer)
                End If
            End Get
        End Property

        Public Overridable Overloads Function Remove(ByVal key As TKey) As Boolean Implements System.Collections.Generic.IDictionary(Of TKey, TValue).Remove
            If Me.dict Is Nothing Then
                Return Me.RemoveInList(key)
            ElseIf Me.dict.Count <= Me.thresold Then
                Me.SwitchToList()
                Return Me.RemoveInList(key)
            Else
                Me.dict.Remove(key)
            End If
        End Function


        Private Function RemoveInList(ByVal key As TKey) As Boolean
            For i As Integer = 0 To Me.list.Count - 1
                If Me.Comparer.Equals(Me.list(i).Key, key) Then
                    Me.list.RemoveAt(i)
                    Return True
                End If
            Next
            Return False
        End Function

        Public Function TryGetValue(ByVal key As TKey, ByRef value As TValue) As Boolean Implements System.Collections.Generic.IDictionary(Of TKey, TValue).TryGetValue
            If dict IsNot Nothing Then
                Return Me.dict.TryGetValue(key, value)
            Else
                For Each local As KeyValuePair(Of TKey, TValue) In Me.list
                    If Me.Comparer.Equals(local.Key, key) Then
                        value = local.Value
                        Return True
                    End If
                Next
                Return False
            End If
        End Function

        <Browsable(False)> _
        Public ReadOnly Property Values() As System.Collections.Generic.ICollection(Of TValue) Implements System.Collections.Generic.IDictionary(Of TKey, TValue).Values
            Get
                If dict IsNot Nothing Then
                    Return Me.dict.Values
                Else
                    Return New ConvertedCollection(Of KeyValuePair(Of TKey, TValue), TValue)(Me.list, _
                                           New Converter(Of KeyValuePair(Of TKey, TValue), TValue)(AddressOf Me.GetValue), Nothing)
                End If
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).GetEnumerator
            If dict IsNot Nothing Then
                Return Me.dict.GetEnumerator
            Else
                Return Me.list.GetEnumerator
            End If
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function


        Public Function GetKey(ByVal objPair As KeyValuePair(Of TKey, TValue)) As TKey
            Return objPair.Key
        End Function

        Public Function GetValue(ByVal objPair As KeyValuePair(Of TKey, TValue)) As TValue
            Return objPair.Value
        End Function

        Protected Overridable Sub SetItem(ByVal key As TKey, ByVal value As TValue)
            If Me.dict IsNot Nothing Then
                Me.dict(key) = value
            Else
                For i As Integer = 0 To Me.list.Count - 1
                    If Me.Comparer.Equals(Me.list(i).Key, key) Then
                        Me.list(i) = New KeyValuePair(Of TKey, TValue)(key, value)
                        Exit Sub
                    End If
                Next
                Me.Add(key, value)
            End If
        End Sub



    End Class




End Namespace




