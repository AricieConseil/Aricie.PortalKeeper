Namespace UI.WebControls.EditorInfos
    Public Class GridViewEditorInfo
        Inherits CollectionEditorInfo

        Private _linkedForm As Integer
        Private _allowEdit As Boolean
        Private _pageSize As Integer

        Public Property LinkedForm() As Integer
            Get
                Return _linkedForm
            End Get
            Set(ByVal value As Integer)
                _linkedForm = value
            End Set
        End Property

        Public Property AllowEdit() As Boolean
            Get
                Return _allowEdit
            End Get
            Set(ByVal value As Boolean)
                _allowEdit = value
            End Set
        End Property

        Public Property PageSize() As Integer
            Get
                Return _pageSize
            End Get
            Set(ByVal value As Integer)
                _pageSize = value
            End Set
        End Property
    End Class
End Namespace
