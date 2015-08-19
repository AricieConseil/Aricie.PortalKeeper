Imports System.Web
Imports System.Globalization

Namespace Services.Caching
    Public Class ValidationCallBackInfo


        Private Shared _HeaderSeparators() As Char = New Char() {","c, " "c}

        Public Sub New()

        End Sub

        Public Sub New(ByVal objTimestamp As DateTime, ByVal setExpiration As Boolean, ByVal objExpires As DateTime)
            Me.Timestamp = objTimestamp
            Me.Expiration = objExpires
            Me.IsExpiresSet = setExpiration
        End Sub

        Public Property Timestamp As DateTime
        Public Property IsExpiresSet As Boolean
        Public Property Expiration As DateTime
        'Public FriendlyUrl As String = ""



        Public Overridable Function ValidateCacheCallback(ByVal context As HttpContext) As Boolean
            Dim headerValues As String() = Nothing
            Dim cacheControlHeader As String = context.Request.Headers.Item("Cache-Control")
            If (cacheControlHeader IsNot Nothing) Then
                headerValues = cacheControlHeader.Split(_HeaderSeparators)
                Dim headerValue As String
                For num As Integer = 0 To headerValues.Length - 1
                    headerValue = headerValues(num)
                    If headerValue = "no-cache" OrElse headerValue = "no-store" Then
                        Return False
                    End If
                    If headerValue.StartsWith("max-age=") Then
                        Dim maxAgeSeconds As Integer
                        Try
                            maxAgeSeconds = Convert.ToInt32(headerValue.Substring(8), CultureInfo.InvariantCulture)
                        Catch
                            maxAgeSeconds = -1
                        End Try
                        If (maxAgeSeconds >= 0) Then
                            Dim currentAgeSeconds As Integer = CInt(((context.Timestamp.Ticks - Me.Timestamp.Ticks) \ 10000000))
                            If (currentAgeSeconds >= maxAgeSeconds) Then
                                Return False
                            End If
                        End If
                    ElseIf headerValue.StartsWith("min-fresh=") Then
                        Dim num5 As Integer
                        Try
                            num5 = Convert.ToInt32(headerValue.Substring(10), CultureInfo.InvariantCulture)
                        Catch
                            num5 = -1
                        End Try
                        If (num5 >= 0 AndAlso Me.IsExpiresSet) Then
                            Dim num7 As Integer = CInt((Me.Expiration.Ticks - context.Timestamp.Ticks) \ 10000000)
                            If (num7 < num5) Then
                                Return False
                            End If
                        End If
                    End If
                Next
            End If
            Dim pragmaHeader As String = context.Request.Headers.Item("Pragma")
            If (pragmaHeader IsNot Nothing) Then
                headerValues = pragmaHeader.Split(_HeaderSeparators)
                For num As Integer = 0 To headerValues.Length - 1
                    If (headerValues(num) = "no-cache") Then
                        Return False
                    End If
                Next
            End If
            Return True

        End Function


    End Class
End NameSpace