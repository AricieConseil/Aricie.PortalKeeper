Namespace UI
    Public Class CollectionField
        Inherits Field

        Private _dataTextField As String
        Private _dataValueField As String
        Private _dataSourceId As String

        Public Property DataTextField() As String
            Get
                Return Me._dataValueField
            End Get
            Set(ByVal value As String)
                Me._dataValueField = value
            End Set
        End Property

        Public Property DataValueField() As String
            Get
                Return Me._dataTextField
            End Get
            Set(ByVal value As String)
                Me._dataTextField = value
            End Set
        End Property

        Public Property DataSourceId() As String
            Get
                Return _dataSourceId
            End Get
            Set(ByVal value As String)
                _dataSourceId = value
            End Set
        End Property
    End Class
End Namespace