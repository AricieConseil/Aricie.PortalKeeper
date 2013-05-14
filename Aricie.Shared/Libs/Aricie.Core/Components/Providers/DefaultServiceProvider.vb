Namespace Providers
    ''' <summary>
    ''' Default System provider concrete implementation with minimal service support.
    ''' </summary>
    ''' <remarks>The caching set is implemented as a local singleton dictionary</remarks>
    Public Class DefaultServiceProvider
        Inherits SystemServiceProvider



        Private Shared _Sink As New Dictionary(Of String, Object)

        Public Overrides Function GetCache(ByVal key As String) As Object
            Dim toReturn As Object = Nothing
            _Sink.TryGetValue(key, toReturn)
            Return toReturn
        End Function

        Public Overrides Sub SetCache(ByVal key As String, ByVal value As Object)
            SetSink(key, value)
        End Sub

        Public Overrides Sub RemoveCache(ByVal key As String)
            SetSink(key, Nothing)
        End Sub

        Public Overrides Sub SetCacheDependant(ByVal Key As String, ByVal value As Object, ByVal expiration As TimeSpan, _
                                                ByVal ParamArray dependencies() As String)
            SetCache(Key, value)
        End Sub

        Private Shared Sub SetSink(ByVal key As String, ByVal value As Object)

            SyncLock _Sink
                If value Is Nothing Then
                    If _Sink.ContainsKey(key) Then
                        _Sink.Remove(key)
                    End If
                Else
                    _Sink(key) = value
                End If

            End SyncLock

        End Sub



    End Class
End Namespace

