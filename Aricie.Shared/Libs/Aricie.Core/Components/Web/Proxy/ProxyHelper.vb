Imports System
Imports System.Globalization
Imports System.Web
Imports System.Net
Imports System.Runtime.Remoting.Contexts
Imports System.Security.Policy


Namespace Web.Proxy

    Public Module ProxyHelper

        Public Function GetProxyBaseUrl(proxifiedRequest As HttpRequest) As String
            If Not proxifiedRequest(StreamingProxy.STR_URL).IsNullOrEmpty() Then
                Return proxifiedRequest.Url.GetLeftPart(UriPartial.Path)
            End If
            Dim urlPath As String = proxifiedRequest.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped)
            'urlPath = urlPath.Substring(proxifiedRequest.ApplicationPath.Length)
            Dim fileName As String = System.IO.Path.GetFileName(urlPath)
            If Not fileName.IsNullOrEmpty Then
                fileName = "/" & fileName
            End If
            Dim toreturn As String = proxifiedRequest.Url.GetLeftPart(UriPartial.Authority) & proxifiedRequest.ApplicationPath & fileName
            Return toreturn
        End Function

        Public Function GetOriginalUrl(proxifiedRequest As HttpRequest) As String
            Dim toReturn As String = proxifiedRequest(StreamingProxy.STR_URL)
            If String.IsNullOrEmpty(toReturn) Then
                Dim urlPath As String = proxifiedRequest.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped).Trim("/"c)
                urlPath = urlPath.Substring(proxifiedRequest.ApplicationPath.Length)
                Dim fileName As String = System.IO.Path.GetFileName(urlPath)
                toReturn = urlPath.Substring(0, urlPath.LastIndexOf(fileName, StringComparison.Ordinal)).Trim("/"c)
                toReturn = UnCloakScheme(toReturn)
                If Not proxifiedRequest.Url.Query.IsNullOrEmpty() Then
                    Dim baseUri As New Uri(toReturn)
                    toReturn = baseUri.ModifyQueryString(proxifiedRequest.QueryString, Nothing)
                End If
            End If
            
            Return toReturn
        End Function

        Private Function CloakScheme(ByVal url As String) As String
            Return url.Replace("://", "/"c)
        End Function

        Private Function UnCloakScheme(ByVal url As String) As String
            Dim slashShortcut As Integer = url.Substring(0, Math.Min(8, url.Length)).IndexOf("/"c)
            If slashShortcut > 0 Then
                Dim colonIndex As Integer = url.IndexOf(":"c)
                If colonIndex < 0 OrElse slashShortcut < colonIndex Then
                    url = url.Substring(0, slashShortcut) & ":/" & url.Substring(slashShortcut)
                End If
            End If
            Return url
        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, baseOriginalUrl As Uri) As String
            originalUrl = originalUrl.Trim(""""c).Trim("'"c)
            If originalUrl.StartsWith("data:") OrElse originalUrl.StartsWith("javascript") Then
                Return originalUrl
            End If
            If originalUrl.StartsWith("//") Then
                originalUrl = "http:" & originalUrl
            End If
            If Uri.IsWellFormedUriString(originalUrl, UriKind.Relative) Then
                Dim absoluteUri As New Uri(baseOriginalUrl, originalUrl)
                originalUrl = absoluteUri.ToString()
            End If
            Return ProxifyUrl(originalUrl, baseProxyUrl)
        End Function


        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String) As String
            If originalUrl.Contains("?") OrElse originalUrl.PathHasInvalidChars() Then
                Return ProxifyUrl(originalUrl, baseProxyUrl, False)
            Else
                Return ProxifyUrl(originalUrl, baseProxyUrl, True)
            End If


        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, pathProxy As Boolean) As String

            'If originalUrl.StartsWith("//") Then
            '    originalUrl = "http:" & originalUrl
            'End If
            Dim baseProxyfileName As String = System.IO.Path.GetFileName(baseProxyUrl)
            Dim baseproxyBase As String = baseProxyUrl.Replace(baseProxyfileName, "")
            If originalUrl.StartsWith(baseproxyBase) Then
                Return originalUrl
            End If

            If pathProxy Then
                'If originalUrl.StartsWith("http://") Then
                '    originalUrl = "http" & originalUrl.Substring(7)
                'ElseIf originalUrl.StartsWith("https://") Then
                '    originalUrl = "https" & originalUrl.Substring(8)
                'End If
                originalUrl = CloakScheme(originalUrl)
                Dim proxifiedUrl As String = System.Web.HttpUtility.UrlEncode(originalUrl)
                Return baseproxyBase & proxifiedUrl & "/"c & baseProxyfileName
            Else
                Dim proxifiedUrl As String = System.Web.HttpUtility.UrlEncode(originalUrl)
                Return String.Format("{0}?{1}={2}", baseProxyUrl, StreamingProxy.STR_URL, proxifiedUrl)
            End If


        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, ByVal minutesCacheDuration As Integer) As String



            Dim toReturn As String = ProxifyUrl(originalUrl, baseProxyUrl)

            If minutesCacheDuration > 0 Then
                If toReturn.Contains("?"c) Then
                    toReturn = String.Format("{0}&{1}={2}", toReturn, StreamingProxy.STR_CACHE_DURATION, minutesCacheDuration.ToString(CultureInfo.InvariantCulture))
                Else
                    toReturn = String.Format("{0}?{1}={2}", toReturn, StreamingProxy.STR_CACHE_DURATION, minutesCacheDuration.ToString(CultureInfo.InvariantCulture))
                End If

            End If
            Return toReturn
        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, ByVal httpVerb As HttpVerb, ByVal minutesCacheDuration As Integer) As String

            Dim toReturn As String = ProxifyUrl(originalUrl, baseProxyUrl, minutesCacheDuration)

            If httpVerb <> Web.HttpVerb.Get Then
                If toReturn.Contains("?"c) Then
                    toReturn = String.Format("{0}&{1}={2}", toReturn, StreamingProxy.STR_METHOD, httpVerb.ToString())
                Else
                    toReturn = String.Format("{0}?{1}={2}", toReturn, StreamingProxy.STR_METHOD, httpVerb.ToString())
                End If
            End If

            Return toReturn
        End Function


        Public Function CreateScalableHttpWebRequest(ByVal url As String) As HttpWebRequest
            Dim request As HttpWebRequest = TryCast(WebRequest.Create(url), HttpWebRequest)
            request.Headers.Add("Accept-Encoding", "gzip")
            request.AutomaticDecompression = DecompressionMethods.GZip
            request.MaximumAutomaticRedirections = 2
            request.ReadWriteTimeout = 5000
            request.Timeout = 3000
            request.Accept = "*/*"
            request.Headers.Add("Accept-Language", "en-US")
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.6) Gecko/20070725 Firefox/2.0.0.6"

            Return request
        End Function

        Public Sub CacheResponse(ByVal context As HttpContext, ByVal durationInMinutes As Integer)
            Dim duration As TimeSpan = TimeSpan.FromMinutes(durationInMinutes)

            context.Response.Cache.SetCacheability(HttpCacheability.[Public])
            context.Response.Cache.SetExpires(DateTime.Now.Add(duration))
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate")
            context.Response.Cache.SetMaxAge(duration)
        End Sub
        Public Sub DoNotCacheResponse(ByVal context As HttpContext)
            context.Response.Cache.SetNoServerCaching()
            context.Response.Cache.SetNoStore()
            context.Response.Cache.SetMaxAge(TimeSpan.Zero)
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate")
            context.Response.Cache.SetExpires(DateTime.Now.AddYears(-1))
        End Sub
    End Module
End Namespace
