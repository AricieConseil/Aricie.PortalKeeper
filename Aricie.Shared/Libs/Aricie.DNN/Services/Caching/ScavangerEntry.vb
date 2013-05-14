Namespace Services.Caching
    Public Class ScavangerEntry
        ' Methods
        Friend Sub New(ByVal filename As String, ByVal utcDelete As DateTime)
            Me._filename = filename
            Me._utcDelete = utcDelete
        End Sub


        ' Properties
        Friend ReadOnly Property Filename() As String
            Get
                Return Me._filename
            End Get
        End Property

        Friend ReadOnly Property UtcDelete() As DateTime
            Get
                Return Me._utcDelete
            End Get
        End Property


        ' Fields
        Private _filename As String
        Private _utcDelete As DateTime
    End Class
End Namespace