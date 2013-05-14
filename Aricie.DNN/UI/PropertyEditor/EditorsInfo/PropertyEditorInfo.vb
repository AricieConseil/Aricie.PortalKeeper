

Namespace UI.WebControls.EditorInfos
    Public Class PropertyEditorInfo
        Inherits AricieEditorInfo

        Private _parent As Object
        Private _parents As List(Of String)
        Private _refetch As Boolean = True
        Private _allowDelete As Boolean = True


        Public Property Parent() As Object
            Get
                Return _parent
            End Get
            Set(ByVal value As Object)
                _parent = value
            End Set
        End Property

        Public Property Parents() As List(Of String)
            Get
                Return _parents
            End Get
            Set(ByVal value As List(Of String))
                _parents = value
            End Set
        End Property

        Public Property Refetch() As Boolean
            Get
                Return _refetch
            End Get
            Set(ByVal value As Boolean)
                _refetch = value
            End Set
        End Property

        Public Property AllowDelete() As Boolean
            Get
                Return _allowDelete
            End Get
            Set(ByVal value As Boolean)
                _allowDelete = value
            End Set
        End Property
    End Class
End Namespace

