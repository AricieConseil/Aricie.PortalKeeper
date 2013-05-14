Namespace Diagnostics
    ''' <summary>
    ''' Generic class for a file based log container
    ''' </summary>
    ''' <remarks>A purge filter can be added to manage purge sequences</remarks>
    Public Class ExternalFileLog(Of T)

        Public Sub New(ByVal logFileMapPath As String, ByVal logValue As T)
            Me._LogFileMapPath = logFileMapPath
            Me._LogValue = logValue
        End Sub

        Public Sub New(ByVal logFileMapPath As String, ByVal logValue As T, ByVal purgeFilter As Predicate(Of T))
            Me.New(logFileMapPath, logValue)
            Me._PurgeFilter = purgeFilter
        End Sub

        Private _LogFileMapPath As String = String.Empty
        Private _LogValue As T
        Private _PurgeFilter As Predicate(Of T)


        Public Property LogFileMapPath() As String
            Get
                Return _LogFileMapPath
            End Get
            Set(ByVal value As String)
                _LogFileMapPath = value
            End Set
        End Property


        Public Property LogValue() As T
            Get
                Return _LogValue
            End Get
            Set(ByVal value As T)
                _LogValue = value
            End Set
        End Property

        Public Property PurgeFilter() As Predicate(Of T)
            Get
                Return _PurgeFilter
            End Get
            Set(ByVal value As Predicate(Of T))
                _PurgeFilter = value
            End Set
        End Property


    End Class
End Namespace