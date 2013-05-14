Namespace UI.WebControls.EditControls
    ''' <summary>
    ''' List wrapper that embed the paginated navigation of a Source list
    ''' </summary>
    <Serializable()> _
    Public Class PagedList
        Inherits PagedCollection
        Implements IList

        Public Sub New()

        End Sub

        Public Sub New(ByVal source As IList, ByVal pageSize As Integer, ByVal pageIndex As Integer)
            MyBase.New(source, pageSize, pageIndex)
        End Sub

        Protected ReadOnly Property ListSource() As IList
            Get
                Return DirectCast(Me.Source, IList)
            End Get
        End Property


        Public Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
            Me.ListSource.Add(value)
            Me.ClearCurrentItems()
        End Function

        Public Sub Clear() Implements System.Collections.IList.Clear
            Me.ListSource.Clear()
            Me.ClearCurrentItems()
        End Sub

        Public Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
            Me.CurrentItems.Contains(value)
        End Function

        Public Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
            Me.CurrentItems.IndexOf(value)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert
            Me.CurrentItems.Insert(index + PageIndex * PageSize, value)
            Me.ClearCurrentItems()
        End Sub

        Public ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
            Get
                Return Me.ListSource.IsFixedSize
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IList.IsReadOnly
            Get
                Return Me.ListSource.IsReadOnly
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Object Implements System.Collections.IList.Item
            Get
                Return CurrentItems(index)
            End Get
            Set(ByVal value As Object)
                Me.ListSource.Item(index + PageIndex * PageSize) = value
                Me.ClearCurrentItems()
            End Set
        End Property

        Public Sub Remove(ByVal value As Object) Implements System.Collections.IList.Remove
            Me.ListSource.Remove(value)
            Me.ClearCurrentItems()
        End Sub

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.IList.RemoveAt
            Me.ListSource.RemoveAt(index + PageIndex * PageSize)
            Me.ClearCurrentItems()
        End Sub
    End Class
End Namespace