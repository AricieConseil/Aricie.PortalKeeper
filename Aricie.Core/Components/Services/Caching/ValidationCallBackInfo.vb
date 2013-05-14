Namespace Services
    ''' <summary>
    ''' Stamp class for callback monitoring
    ''' </summary>
    Public Class ValidationCallBackInfo

        Public Sub New(ByVal objTimestamp As DateTime, ByVal setExpiration As Boolean, ByVal objExpires As DateTime)
            Me.Timestamp = objTimestamp
            Me.Expiration = objExpires
            Me.IsExpiresSet = setExpiration
        End Sub

        Public Timestamp As DateTime
        Public IsExpiresSet As Boolean
        Public Expiration As DateTime
        Public OriginalUrl As String = ""

    End Class
End NameSpace