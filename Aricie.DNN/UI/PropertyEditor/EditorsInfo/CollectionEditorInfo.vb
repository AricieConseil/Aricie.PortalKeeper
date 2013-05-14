

Namespace UI.WebControls.EditorInfos
    Public Class CollectionEditorInfo
        Inherits AricieEditorInfo

        Private _Collection As IList
        Private _dataFieldText As String
        Private _dataFieldValue As String
        Private _title As String
        Private _allowAdd As Boolean
        Private _parent As Object
        Private _parents As List(Of String)

        Public Property Collection() As IList
            Get
                Return _Collection
            End Get
            Set(ByVal value As IList)
                _Collection = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Property AllowAdd() As Boolean
            Get
                Return _allowAdd
            End Get
            Set(ByVal value As Boolean)
                _allowAdd = value
            End Set
        End Property

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

        ' Properties
        Public Property DataFieldText() As String
            Get
                Return _dataFieldText
            End Get
            Set(ByVal value As String)
                Me._dataFieldText = value
            End Set
        End Property

        Public Property DataFieldValue() As String
            Get
                Return _dataFieldValue
            End Get
            Set(ByVal value As String)
                Me._dataFieldValue = value
            End Set
        End Property
    End Class
End Namespace

