Namespace Services.Caching
    Public Class FileOutputSettings


        Private _InfoFileName As String = String.Empty
        Private _DataFileName As String = String.Empty
        Private _ServeFromMemory As Boolean
        Private _FileNamePrefix As String


        Public Property ServeFromMemory() As Boolean
            Get
                Return _ServeFromMemory
            End Get
            Set(ByVal value As Boolean)
                _ServeFromMemory = value
            End Set
        End Property

        Public Property InfoFileName() As String
            Get
                Return _InfoFileName
            End Get
            Set(ByVal value As String)
                _InfoFileName = value
            End Set
        End Property

        Public Property DataFileName() As String
            Get
                Return _DataFileName
            End Get
            Set(ByVal value As String)
                _DataFileName = value
            End Set
        End Property

        Public Property FileNamePrefix() As String
            Get
                Return _FileNamePrefix
            End Get
            Set(ByVal value As String)
                _FileNamePrefix = value
            End Set
        End Property

        Public Function GetPrefix(ByVal varybyId As String) As String
            If Not String.IsNullOrEmpty(varybyId) Then
                Return String.Format("{0}_q_{1:x8}", Me._FileNamePrefix, varybyId.GetHashCode)
            End If
            Return Me._FileNamePrefix
        End Function


    End Class
End Namespace