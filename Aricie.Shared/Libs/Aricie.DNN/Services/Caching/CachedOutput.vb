
Namespace Services.Caching
    
    Public Class CachedOutput

        Private _TimeStamp As DateTime
        Private _Path As String = String.Empty
        Private _Vary As String = String.Empty
        Private _ResponseFileName As String = String.Empty
        Private _ResponseBytes As Byte()
        Private _ResponseExpiry As DateTime
        Private _ResponseHash As Long

        Public Property Path() As String
            Get
                Return _Path
            End Get
            Set(ByVal value As String)
                _Path = value
            End Set
        End Property

        Public Property Vary() As String
            Get
                Return _Vary
            End Get
            Set(ByVal value As String)
                _Vary = value
            End Set
        End Property

        Public Property TimeStamp() As Date
            Get
                Return _TimeStamp
            End Get
            Set(ByVal value As Date)
                _TimeStamp = value
            End Set
        End Property

        Public Property ResponseBytes() As Byte()
            Get
                Return _ResponseBytes
            End Get
            Set(ByVal value As Byte())
                _ResponseBytes = value
            End Set
        End Property

        Public Property ResponseFileName() As String
            Get
                Return _ResponseFileName
            End Get
            Set(ByVal value As String)
                _ResponseFileName = value
            End Set
        End Property


        Public Property ResponseExpiry() As DateTime
            Get
                Return _ResponseExpiry
            End Get
            Set(ByVal value As DateTime)
                _ResponseExpiry = value
            End Set
        End Property

        Public Property ResponseHash() As Long
            Get
                Return _ResponseHash
            End Get
            Set(ByVal value As Long)
                _ResponseHash = value
            End Set
        End Property


    End Class
End Namespace