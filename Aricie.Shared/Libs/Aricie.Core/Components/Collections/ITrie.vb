Namespace Collections

    'Public Delegate Function Spliter(Of Tlist, TItem)(ByVal list As Tlist) As IEnumerator(Of TItem)

    'Public Interface IEnumeratorBuilder(Of TList, TItem)

    '    Function GetEnumerator(ByVal list As TList) As IEnumerator(Of TItem)

    'End Interface

    'Public Interface ITrieNode(Of TTrieNode, TKey, TValue)
    '    Inherits IDictionary(Of TKey, TTrieNode)

    '    Property Value() As TValue

    ''' <summary>
    ''' Interface for a Trie like lookup structure
    ''' </summary>
    ''' End Interface


    Public Interface ITrie(Of TList, TValue)

        ReadOnly Property ItemCount() As Integer

        Sub [Set](ByVal prefix As TList, ByVal value As TValue)

        Sub Remove(ByVal prefix As TList)

        Function Contains(ByVal prefix As TList) As Boolean

        Function Find(ByVal prefix As TList) As TValue

        Function Search(ByVal prefix As TList, ByRef partialMatch As Boolean) As TValue

    End Interface


    'Public Interface IRadixTree(Of TList, TValue)
    '    Inherits ITrieNode(Of TList, TValue)
    '    Inherits ITrie(Of TList, TValue)


    'End Interface
End Namespace