

Namespace UI.WebControls.EditorInfos
    Public Class DataListEditorInfo
        Inherits CollectionEditorInfo

        Private _displayMode As String
        Private _isExpanded As Boolean
        Private _allowDelete As Boolean


        Public Property DisplayMode() As String
            Get
                Return _displayMode
            End Get
            Set(ByVal value As String)
                _displayMode = value
            End Set
        End Property

        Public Property IsExpanded() As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                _isExpanded = value
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
