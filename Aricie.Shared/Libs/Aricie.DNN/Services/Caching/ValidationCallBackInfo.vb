Imports System.Web
Imports System.Globalization
Imports Aricie.DNN.Entities

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


       

        Public Overridable Function ValidateCacheCallback(ByVal context As HttpContext) As HttpValidationStatus
            If Me.IsExpiresSet Then
                If Now > Expiration Then
                    Return HttpValidationStatus.Invalid
                End If
            End If
           

            Dim headerValues As String() = Nothing
            Dim cacheControlHeader As String = context.Request.Headers.Item("Cache-Control")
            If (cacheControlHeader IsNot Nothing) Then
                headerValues = cacheControlHeader.Split(_HeaderSeparators)
                Dim headerValue As String
                For num As Integer = 0 To headerValues.Length - 1
                    headerValue = headerValues(num)
                    If headerValue = "no-cache" OrElse headerValue = "no-store" Then
                        Return HttpValidationStatus.IgnoreThisRequest
                    End If
                    If headerValue.StartsWith("max-age=") Then
                        Dim maxAgeSeconds As Integer
                        Try
                            maxAgeSeconds = Convert.ToInt32(headerValue.Substring(8), CultureInfo.InvariantCulture)
                        Catch
                            maxAgeSeconds = -1
                        End Try
                        If (maxAgeSeconds >= 0) Then
                            Dim currentAgeSeconds As Integer = CInt(((context.Timestamp.Ticks - Me.Timestamp.Ticks) \ TimeSpan.TicksPerSecond))
                            If (currentAgeSeconds >= maxAgeSeconds) Then
                                Return HttpValidationStatus.IgnoreThisRequest
                            End If
                        End If
                    ElseIf headerValue.StartsWith("min-fresh=") Then
                        Dim minFreshSec As Integer
                        Try
                            minFreshSec = Convert.ToInt32(headerValue.Substring(10), CultureInfo.InvariantCulture)
                        Catch
                            minFreshSec = -1
                        End Try
                        If (minFreshSec >= 0 AndAlso Me.IsExpiresSet) Then
                            Dim num7 As Integer = CInt((Me.Expiration.Ticks - context.Timestamp.Ticks) \ TimeSpan.TicksPerSecond)
                            If (num7 < minFreshSec) Then
                                Return HttpValidationStatus.IgnoreThisRequest
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
                        Return HttpValidationStatus.IgnoreThisRequest
                    End If
                Next
            End If

          
            Return HttpValidationStatus.Valid

        End Function


    End Class
End NameSpace