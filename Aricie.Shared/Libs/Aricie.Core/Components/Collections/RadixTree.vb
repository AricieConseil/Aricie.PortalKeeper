Namespace Collections

    ''' <summary>
    ''' Collection class for a Radix tree
    ''' </summary>
    ''' <remarks>It is inherited from a Trie and handles branch condensation automatically.</remarks>
    <Serializable()> _
    Public Class RadixTree(Of TKey, TList As IEnumerable(Of TKey), TValue)
        Inherits Trie(Of TKey, TList, TValue)

        Public Sub New()
            Me.New(Nothing, 7)
        End Sub

        Public Sub New(ByVal dictionaryCreationThreshold As Integer)
            Me.New(Nothing, dictionaryCreationThreshold)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey))
            Me.New(comparer, 7)
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of TKey), ByVal dictionaryCreationThreshold As Integer) ', ByVal parent As RadixTree(Of TKey, TList, TValue))
            MyBase.New(comparer, dictionaryCreationThreshold)
        End Sub

        Protected Sub New(ByVal comparer As IEqualityComparer(Of TKey), ByVal dictionaryCreationThreshold As Integer, ByVal tail As IEnumerator(Of TKey), ByVal objValue As TValue)
            MyBase.New(comparer, dictionaryCreationThreshold)
            Me.Init(tail, objValue)
        End Sub


        Private _Tail As List(Of TKey)



#Region "overrides"


        Protected Overrides Sub InternalSet(ByVal traverser As IEnumerator(Of TKey), ByVal objValue As TValue)

            If Me._Tail Is Nothing Then
                'no tail corresponds to a regular trie
                MyBase.InternalSet(traverser, objValue)
            Else
                'Dim matchingTailPrefix As New List(Of TKey)()
                Dim newTailSize As Integer = 0
                Dim tailTraverser As IEnumerator(Of TKey) = Me._Tail.GetEnumerator
                tailTraverser.MoveNext()
                Do
                    If Not traverser.MoveNext Then
                        Me.Split(newTailSize, tailTraverser)
                        Me.SetValue(objValue)
                        Exit Sub
                    ElseIf Not Me.Comparer.Equals(tailTraverser.Current, traverser.Current) Then
                        Me.Split(newTailSize, tailTraverser)
                        Me.CreateChildNode(traverser, objValue)
                        Exit Sub
                    End If
                    newTailSize += 1
                Loop While tailTraverser.MoveNext
                MyBase.InternalSet(traverser, objValue)
            End If

        End Sub


        Protected Overrides Function InternalSearch(ByVal traverser As System.Collections.Generic.IEnumerator(Of TKey), ByRef partialMatch As Boolean) As TValue
            If Me._Tail Is Nothing Then
                Return MyBase.InternalSearch(traverser, partialMatch)
            Else
                Dim tailTraverser As IEnumerator(Of TKey) = Me._Tail.GetEnumerator
                tailTraverser.MoveNext()
                Do
                    If Not traverser.MoveNext OrElse Not Me.Comparer.Equals(tailTraverser.Current, traverser.Current) Then
                        Return Nothing
                    End If
                Loop While tailTraverser.MoveNext
                Return MyBase.InternalSearch(traverser, partialMatch)
            End If
        End Function

        Protected Overrides Function InternalRemove(ByVal traverser As System.Collections.Generic.IEnumerator(Of TKey), ByRef removed As Boolean) As Boolean
            If Me._Tail Is Nothing Then
                Return MyBase.InternalRemove(traverser, removed)
            Else
                Dim tailTraverser As IEnumerator(Of TKey) = Me._Tail.GetEnumerator
                tailTraverser.MoveNext()
                Do
                    If Not traverser.MoveNext OrElse Not Me.Comparer.Equals(tailTraverser.Current, traverser.Current) Then
                        removed = False
                        Return False
                    End If

                Loop While tailTraverser.MoveNext
                Return MyBase.InternalRemove(traverser, removed)
            End If

        End Function

        Protected Overrides Function CreateChildNode(ByVal prefix As System.Collections.Generic.IEnumerator(Of TKey), ByVal objValue As TValue) As Trie(Of TKey, TList, TValue)
            Dim key As TKey = prefix.Current
            Dim toReturn As New RadixTree(Of TKey, TList, TValue)(Me.Comparer, Me.thresold, prefix, objValue)
            MyBase.Add(key, toReturn)
            Return toReturn
        End Function

        Protected Overrides Function RemoveChildNode(ByVal child As Trie(Of TKey, TList, TValue), ByVal traverser As System.Collections.Generic.IEnumerator(Of TKey), ByRef removed As Boolean) As Boolean
            If Not MyBase.RemoveChildNode(child, traverser, removed) Then
                If Me._Value Is Nothing AndAlso Me.Count = 1 Then
                    Me.Merge()
                End If
                Return False
            End If
            Return True
        End Function



#End Region

#Region "Private methods"

        Private Sub Init(ByVal tail As IEnumerator(Of TKey), ByVal objValue As TValue)
            Me.SetValue(objValue)
            If tail.MoveNext Then
                Me._Tail = New List(Of TKey)
                Do
                    Me._Tail.Add(tail.Current)
                Loop While tail.MoveNext
            End If
        End Sub

        Private Sub Merge()

            For Each objKey As TKey In Me.Keys
                Dim currentTrie As RadixTree(Of TKey, TList, TValue) = DirectCast(Me(objKey), RadixTree(Of TKey, TList, TValue))
                If Not MyBase.Remove(objKey) Then
                    'removed = False
                    Throw New KeyNotFoundException
                End If
                Me._Tail.Add(objKey)
                Me.SetValue(currentTrie._Value)
                If currentTrie._Tail IsNot Nothing Then
                    Me._Tail.AddRange(currentTrie._Tail)
                End If
                For Each child As KeyValuePair(Of TKey, Trie(Of TKey, TList, TValue)) In currentTrie
                    Me.Add(child)
                Next
            Next

        End Sub

        Private Sub Split(ByVal newTailSize As Integer, ByVal tailTraverser As IEnumerator(Of TKey))

            Dim tempArray() As KeyValuePair(Of TKey, Trie(Of TKey, TList, TValue)) = Nothing
            If Me.Count > 0 Then
                ReDim tempArray((Me.Count - 1))
                Me.CopyTo(tempArray, 0)
                Me.Clear()
            End If
            Dim tailChild As RadixTree(Of TKey, TList, TValue) = DirectCast(Me.CreateChildNode(tailTraverser, Me._Value), RadixTree(Of TKey, TList, TValue))
            Me._Value = Nothing
            If tempArray IsNot Nothing Then
                For Each objItem As KeyValuePair(Of TKey, Trie(Of TKey, TList, TValue)) In tempArray
                    tailChild.Add(objItem.Key, objItem.Value)
                Next
            End If

            If newTailSize > 0 Then
                Me._Tail = Me._Tail.GetRange(0, newTailSize)
            Else
                Me._Tail = Nothing
            End If

        End Sub

#End Region


    End Class
End Namespace