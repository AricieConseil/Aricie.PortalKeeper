Imports Aricie.Providers
Imports System.Web
Imports System.Globalization


Namespace Services
    Public Module CacheHelper


#Region "Public Methods"


#Region "Basic Methods"

        Public Function GetCache(ByVal key As String) As Object
            Return SystemServiceProvider.Instance().GetCache(key)

        End Function

        Public Sub SetCache(ByVal key As String, ByVal value As Object)
            SystemServiceProvider.Instance().SetCache(key, value)
        End Sub

        Public Sub SetCache(ByVal key As String, ByVal value As Object, ByVal absoluteExpiration As TimeSpan)

            SystemServiceProvider.Instance().SetCacheDependant(key, value, absoluteExpiration)

        End Sub

        Public Sub RemoveCache(ByVal key As String)
            SystemServiceProvider.Instance().RemoveCache(key)
        End Sub

        

        Public Sub SetCacheDependant(ByVal key As String, ByVal value As Object, ByVal expiration As TimeSpan, _
                                                ByVal ParamArray dependencies() As String)

            SystemServiceProvider.Instance().SetCacheDependant(Key, value, expiration, dependencies)

        End Sub

        Public Sub ClearCache()
            SystemServiceProvider.Instance().ClearCache()
        End Sub

#End Region

#Region "Advanced Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <param name="dependency"></param>
        ''' <param name="args"></param>
        ''' <remarks> YSDF: cette fonction était commentée je l'ai decommentée
        ''' </remarks>
        ''' 
        <Obsolete("Use version with typed object and explicit expiration")> _
        Public Sub SetCacheDependant(Of T)(ByVal obj As Object, _
                                             ByVal dependency As String, ByVal ParamArray args() As String)
            Dim expiration As TimeSpan = Constants.Cache.GlobalExpiration
            Dim key As String = Constants.GetKey(Of T)(args)
            SetCacheDependant(key, obj, expiration, dependency)

        End Sub

        Public Sub SetCache(Of T)(ByVal obj As T, ByVal absoluteExpiration As TimeSpan, _
                                             ByVal ParamArray args() As String)

            Dim key As String = Constants.GetKey(Of T)(args)
            SystemServiceProvider.Instance().SetCacheDependant(key, obj, absoluteExpiration)

        End Sub

        Public Sub SetCacheDependant(Of T)(ByVal obj As T, ByVal dependency As String, ByVal absoluteExpiration As TimeSpan, _
                                              ByVal ParamArray args() As String)

            Dim key As String = Constants.GetKey(Of T)(args)
            SetCacheDependant(key, obj, absoluteExpiration, dependency)

        End Sub

        Public Sub SetCacheDependant(Of T)(ByVal obj As T, ByVal dependencies() As String, ByVal absoluteExpiration As TimeSpan, _
                                              ByVal ParamArray args() As String)

            Dim key As String = Constants.GetKey(Of T)(args)
            SetCacheDependant(key, obj, absoluteExpiration, dependencies)

        End Sub

        Public Sub RemoveCache(Of T)(ByVal ParamArray args() As String)
            Dim key As String = Constants.GetKey(Of T)(args)
            RemoveCache(key)
        End Sub


        Public Function GetGlobal(Of T)(ByVal ParamArray args() As String) As T

            Dim key As String = Constants.GetKey(Of T)(args)

            Return DirectCast(GetCache(key), T)

        End Function

        Public Sub SetGlobal(Of T)(ByVal obj As Object, ByVal ParamArray args() As String)

            Dim key As String = Constants.GetKey(Of T)(args)

            SetCache(key, obj, Constants.Cache.GlobalExpiration)

        End Sub


        Public Function GetPersonal(Of T)(ByVal userId As Integer, ByVal ParamArray args() As String) As T

            Array.Resize(args, args.Length + 1)
            args(args.Length - 1) = "UserId" & userId

            Return GetGlobal(Of T)(args)

        End Function

        Public Sub SetPersonal(Of T)(ByVal obj As T, ByVal userId As Integer, ByVal ParamArray args() As String)

            Array.Resize(args, args.Length + 1)
            args(args.Length - 1) = "UserId" & userId

            SetGlobal(Of T)(obj, args)

        End Sub


        Public Function GetGlobal(Of T, H)(ByVal ParamArray args() As String) As T

            Dim key As String = Constants.GetKey(Of T, H)(args)

            Return DirectCast(GetCache(key), T)

        End Function

        Public Sub SetGlobal(Of T, H)(ByVal obj As T, ByVal ParamArray args() As String)

            Dim key As String = Constants.GetKey(Of T, H)(args)

            SetCacheDependant(key, obj, Constants.Cache.GlobalExpiration)

        End Sub


#End Region


#Region "Page caching methods"




        Public Sub AddResponseCaching(ByVal objResponse As HttpResponse, ByVal duration As TimeSpan, Optional ByVal validationHandler As HttpCacheValidateHandler = Nothing)

            If validationHandler Is Nothing Then
                validationHandler = New HttpCacheValidateHandler(AddressOf ValidateCache)
            End If

            objResponse.Cache.SetCacheability(HttpCacheability.Public)
            Dim timeStamp As DateTime = Now
            Dim expiration As DateTime = timeStamp.Add(duration)
            objResponse.Cache.SetExpires(expiration)

            objResponse.Cache.SetValidUntilExpires(True)
            'For Each qs As String In application.Request.QueryString.Keys
            '    objResponse.Cache.VaryByParams(qs) = True
            'Next
            objResponse.Cache.VaryByParams("*") = True

            objResponse.Cache.SetVaryByCustom("browser")
            Dim callBackInfo As New ValidationCallBackInfo(timeStamp, True, expiration)

            objResponse.Cache.AddValidationCallback(validationHandler, callBackInfo)

            'todo: repair this dependency
            'Dim tabid As Integer = NukeHelper.PortalSettings.ActiveTab.TabID
            'If tabid <> -1 Then
            '    Dim prefixDep As String
            '    Select Case Aricie.DNN.Services.NukeHelper.DnnVersion.Major
            '        Case 5
            '            prefixDep = "DNN_"
            '        Case Else
            '            prefixDep = ""
            '    End Select
            '    objResponse.AddCacheItemDependencies(New String() {String.Format(prefixDep & "TabModules{0}", tabid.ToString)})
            'End If

        End Sub

        Public Sub ValidateCache(ByVal context As HttpContext, ByVal data As Object, ByRef status As HttpValidationStatus)

            If context.Request.IsAuthenticated Then
                status = HttpValidationStatus.IgnoreThisRequest
            Else
                Dim callBackInfo As ValidationCallBackInfo = DirectCast(data, ValidationCallBackInfo)
                If callBackInfo Is Nothing OrElse ValidateCacheCallback(context, callBackInfo) Then
                    status = HttpValidationStatus.Valid
                Else
                    status = HttpValidationStatus.IgnoreThisRequest
                End If
            End If
        End Sub

        Private Function ValidateCacheCallback(ByVal context As HttpContext, ByVal callBackInfo As ValidationCallBackInfo) As Boolean

            'If callBackInfo.OriginalUrl <> "" Then
            '    Dim fupContext As FriendlierUrlContext = FriendlierUrlContext.Instance()
            '    If fupContext.PreRewriteUri.AbsoluteUri <> callBackInfo.FriendlyUrl Then
            '        Return False
            '    End If
            'End If

            Dim headerValues As String() = Nothing
            Dim cacheControlHeader As String = context.Request.Headers.Item("Cache-Control")
            If (Not cacheControlHeader Is Nothing) Then
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
                            Dim currentAgeSeconds As Integer = CInt(((context.Timestamp.Ticks - callBackInfo.Timestamp.Ticks) / 10000000))
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
                        If (num5 >= 0 AndAlso callBackInfo.IsExpiresSet) Then
                            Dim num7 As Integer = CInt(((callBackInfo.Expiration.Ticks - context.Timestamp.Ticks) / 10000000))
                            If (num7 < num5) Then
                                Return False
                            End If
                        End If
                    End If
                Next
            End If
            Dim pragmaHeader As String = context.Request.Headers.Item("Pragma")
            If (Not pragmaHeader Is Nothing) Then
                headerValues = pragmaHeader.Split(_HeaderSeparators)

                For num As Integer = 0 To headerValues.Length - 1
                    If (headerValues(num) = "no-cache") Then
                        Return False
                    End If
                Next
            End If
            Return True

        End Function

        Private _HeaderSeparators() As Char = New Char() {","c, " "c}

#End Region


#End Region


#Region "private methods"


#End Region
    End Module
End Namespace


