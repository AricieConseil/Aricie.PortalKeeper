Namespace UI.WebControls.EditControls
    ''' <summary>
    ''' Collection wrapper that embed the paginated navigation of a Source Collection
    ''' </summary>
    ''' <remarks>The page size and current index are given and computed according to the collection life cycle.</remarks>
    <Serializable()> _
    Public Class PagedCollection
        Implements ICollection

        Private _PageSize As Integer
        Private _PageIndex As Integer
        Protected Source As ICollection
        Protected _CurrentItems As ArrayList
        'Protected _SortFieldName As String

        Public Sub New()

        End Sub

        'Public Sub New(ByVal source As ICollection, ByVal pageSize As Integer, ByVal pageIndex As Integer)
        '    Me.New(source, pageSize, pageIndex, String.Empty)
        'End Sub

        Public Sub New(ByVal source As ICollection, ByVal pageSize As Integer, ByVal pageIndex As Integer) ', sortFieldName As String)
            Me.Source = source
            If pageSize < 1 Then
                pageSize = 1000000
            End If
            Me.PageSize = pageSize
            Me.PageIndex = pageIndex
            'Me._SortFieldName = sortFieldName
        End Sub

        Public Sub ClearCurrentItems()
            Me._CurrentItems = Nothing
        End Sub

        Public ReadOnly Property CurrentItems() As ArrayList
            Get
                If _CurrentItems Is Nothing Then

                    Dim tempEnum As IEnumerator
                    'If Me._SortFieldName <> String.Empty Then
                    '    Dim tempSortedCollection As New ArrayList(Me.Source)
                    '    tempSortedCollection.Sort(New Aricie.Business.Filters.SimpleComparer(Me._SortFieldName, System.ComponentModel.ListSortDirection.Ascending))
                    '    tempEnum = tempSortedCollection.GetEnumerator
                    'Else
                    '    tempEnum = Me.Source.GetEnumerator
                    'End If
                    tempEnum = Me.Source.GetEnumerator
                    _CurrentItems = New ArrayList(Source.Count)
                    Dim minIndex As Integer = PageIndex * PageSize
                    Dim maxIndex As Integer = (PageIndex + 1) * PageSize - 1

                    Dim i As Integer = 0
                    While tempEnum.MoveNext
                        If i >= minIndex AndAlso i <= maxIndex Then
                            _CurrentItems.Add(tempEnum.Current)
                        End If
                        i += 1
                    End While

                End If
                Return _CurrentItems
            End Get
        End Property

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
            Me.CurrentItems.CopyTo(array, index)
        End Sub

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return Me.CurrentItems.Count
            End Get
        End Property

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return Me.Source.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return Me.Source.SyncRoot
            End Get
        End Property

        Public Property PageIndex() As Integer
            Get
                Return _PageIndex
            End Get
            Set(ByVal value As Integer)
                If Not value.Equals(Me._PageIndex) Then
                    _PageIndex = value
                    Me.ClearCurrentItems()
                End If
            End Set
        End Property

        Public Property PageSize() As Integer
            Get
                Return _PageSize
            End Get
            Set(ByVal value As Integer)
                If Not value.Equals(Me._PageSize) Then
                    _PageSize = value
                    Me.ClearCurrentItems()
                End If
            End Set
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Me.CurrentItems.GetEnumerator
        End Function

        Public Function IsPaginated() As Boolean
            Return Me.Source.Count > Me._PageSize
        End Function

    End Class
End Namespace