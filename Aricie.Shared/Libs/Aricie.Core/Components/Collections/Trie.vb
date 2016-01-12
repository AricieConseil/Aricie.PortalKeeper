Namespace Collections



    ''' <summary>
    ''' Generic class that represents a Trie
    ''' </summary>
    ''' <remarks>There are 3 generic arguments of the type of lookup symbols, lookup keys as symbol lists, and content values</remarks>
    ''' End Class

    
    Public Class Trie(Of TKey, TList As IEnumerable(Of TKey), TValue)
        Inherits HybridDictionary(Of TKey, Trie(Of TKey, TList, TValue))
        Implements ITrie(Of TList, TValue)
        'Inherits TrieNode(Of TKey, TList, TValue)
        'Implements ITrieNode(Of Trie(Of TKey, TList, TValue), TKey, TValue)



        Public Sub New()
            Me.New(Nothing, 7)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey))
            Me.New(comparer, 7)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey), ByVal dictionaryCreationThreshold As Integer)
            MyBase.New(0, comparer, dictionaryCreationThreshold)
            'Me.dictionaryThresold = dictionaryCreationThreshold
            'Me._Spliter = objSpliter
        End Sub


        'Private Sub New(ByVal parent As Trie(Of TKey, TValue))

        'End Sub

        Protected _Value As TValue
        'Protected dictionaryThresold As Integer
        'Private _Spliter As Spliter(Of TList, TKey)


        Public Property Value() As TValue
            Get
                Return Me._Value
            End Get
            Set(ByVal value As TValue)
                Me._Value = value
            End Set
        End Property


        Public ReadOnly Property ItemCount() As Integer Implements ITrie(Of TList, TValue).ItemCount
            Get
                Dim counter As Integer
                Me.InternalCount(counter)
                Return counter
            End Get
        End Property

        Public Sub [Set](ByVal prefix As TList, ByVal value As TValue) Implements ITrie(Of TList, TValue).[Set]
            Dim traverser As IEnumerator(Of TKey) = prefix.GetEnumerator
            'Dim traverser As IEnumerator(Of TKey) = Me._Spliter.Invoke(prefix)
            Me.InternalSet(traverser, value)
        End Sub

        Public Overloads Function Contains(ByVal prefix As TList) As Boolean Implements ITrie(Of TList, TValue).Contains
            Return Me.Find(prefix) IsNot Nothing
        End Function

        Public Function Find(ByVal prefix As TList) As TValue Implements ITrie(Of TList, TValue).Find
            Dim partialMatch As Boolean
            Dim toReturn As TValue = Me.Search(prefix, partialMatch)
            If partialMatch Then
                Return Nothing
            Else
                Return toReturn
            End If
        End Function

        Public Function Search(ByVal prefix As TList, ByRef partialMatch As Boolean) As TValue Implements ITrie(Of TList, TValue).Search
            Dim traverser As IEnumerator(Of TKey) = prefix.GetEnumerator
            Return Me.InternalSearch(traverser, partialMatch)
        End Function

        Public Overloads Sub Remove(ByVal prefix As TList) Implements ITrie(Of TList, TValue).Remove
            Dim removed As Boolean
            Dim traverser As IEnumerator(Of TKey) = prefix.GetEnumerator
            'Dim traverser As IEnumerator(Of TKey) = Me._Spliter.Invoke(prefix)
            Me.InternalRemove(traverser, removed)
            If Not removed Then
                Throw New ArgumentException("prefix was not found in trie", "prefix")
            End If
        End Sub

        Public Function AsList() As IList(Of TValue)
            Dim toReturn As New List(Of TValue)(Me.ItemCount)
            If _Value IsNot Nothing Then
                toReturn.Add(_Value)
            End If
            Me.FillList(toReturn)
            Return toReturn
        End Function



#Region "Protected methods"


        Protected Overridable Sub SetValue(ByVal objValue As TValue)
            Me._Value = objValue

        End Sub



        Protected Overridable Sub InternalSet(ByVal traverser As IEnumerator(Of TKey), ByVal objValue As TValue)
            If traverser.MoveNext Then
                Dim child As Trie(Of TKey, TList, TValue) = Nothing
                If Me.TryGetValue(traverser.Current, child) Then
                    child.InternalSet(traverser, objValue)
                Else
                    Me.CreateChildNode(traverser, objValue)
                End If
            Else
                Me.SetValue(objValue)
            End If
        End Sub


        Protected Overridable Function InternalSearch(ByVal traverser As System.Collections.Generic.IEnumerator(Of TKey), ByRef partialMatch As Boolean) As TValue
            If traverser.MoveNext Then
                Dim current As TKey = traverser.Current
                Dim child As Trie(Of TKey, TList, TValue) = Nothing
                If Not Me.TryGetValue(current, child) Then
                    partialMatch = True
                    Return Me._Value
                End If
                Return child.InternalSearch(traverser, partialMatch)
            Else
                partialMatch = False
                Return Me._Value
            End If
        End Function

        Protected Overridable Function InternalRemove(ByVal traverser As System.Collections.Generic.IEnumerator(Of TKey), ByRef removed As Boolean) As Boolean
            If traverser.MoveNext Then
                Dim child As Trie(Of TKey, TList, TValue) = Nothing
                If Me.TryGetValue(traverser.Current, child) Then
                    Return RemoveChildNode(child, traverser, removed)
                End If
            Else
                If Me._Value IsNot Nothing Then
                    removed = True
                    Me._Value = Nothing
                    If Me.Count = 0 Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function


        Protected Overridable Function CreateChildNode(ByVal prefix As IEnumerator(Of TKey), ByVal objValue As TValue) As Trie(Of TKey, TList, TValue)
            Dim toReturn As New Trie(Of TKey, TList, TValue)(Me.Comparer, Me.thresold)
            Dim key As TKey = prefix.Current
            toReturn.InternalSet(prefix, objValue)
            MyBase.Add(key, toReturn)
            Return toReturn
        End Function


        Protected Overridable Function RemoveChildNode(ByVal child As Trie(Of TKey, TList, TValue), _
                            ByVal traverser As IEnumerator(Of TKey), ByRef removed As Boolean) As Boolean
            Dim key As TKey = traverser.Current
            If child.InternalRemove(traverser, removed) Then
                If Not MyBase.Remove(key) Then
                    'removed = False
                    Throw New KeyNotFoundException
                End If
                If Me.Count = 0 AndAlso Me._Value Is Nothing Then
                    Return True
                End If
            End If
            Return False
        End Function



#End Region

#Region "Private methods"

        Private Sub InternalCount(ByRef counter As Integer)
            If Me._Value IsNot Nothing Then
                counter += 1
            End If
            For Each objTrie As Trie(Of TKey, TList, TValue) In Me.Values
                objTrie.InternalCount(counter)
            Next
        End Sub

        Private Sub FillList(ByRef objList As List(Of TValue))
            For Each node As Trie(Of TKey, TList, TValue) In Me.Values
                If node._Value IsNot Nothing Then
                    objList.Add(node._Value)
                End If
            Next
            For Each node As Trie(Of TKey, TList, TValue) In Me.Values
                node.FillList(objList)
            Next
        End Sub

#End Region

    End Class

    '
    'Public Class TrieNode(Of TKey, TList As IEnumerable(Of TKey), TValue)
    '    'Inherits HybridDictionary(Of TKey, Trie(Of TKey, TList, TValue))

    '    Private _Value As TValue

    '    'Public Sub New(ByVal capacity As Integer, ByVal comparer As IEqualityComparer(Of TKey), ByVal objThresold As Integer)
    '    '    MyBase.New(capacity, comparer, objThresold)
    '    'End Sub


    '    Private _Children As Trie(Of TKey, TList, TValue)


    '    Public Property Children() As Trie(Of TKey, TList, TValue)
    '        Get
    '            Return _Children
    '        End Get
    '        Set(ByVal value As Trie(Of TKey, TList, TValue))
    '            _Children = value
    '        End Set
    '    End Property

    '    Public Property Value() As TValue
    '        Get
    '            Return _Value
    '        End Get
    '        Set(ByVal value As TValue)
    '            _Value = value
    '        End Set
    '    End Property


End Namespace