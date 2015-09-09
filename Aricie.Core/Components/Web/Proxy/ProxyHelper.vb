Imports System
Imports System.Globalization
Imports System.Web
Imports System.Net


Namespace Web.Proxy

    Public Module ProxyHelper

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String) As String

            If originalUrl.StartsWith("//") Then
                originalUrl = "http:" & originalUrl
            End If
            Dim proxifiedUrl As String = System.Web.HttpUtility.UrlEncode(originalUrl)
            Return String.Format("{0}?u={1}", baseProxyUrl, proxifiedUrl)

        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, ByVal minutesCacheDuration As Integer) As String



            Dim toReturn As String = ProxifyUrl(originalUrl, baseProxyUrl)

            If minutesCacheDuration > 0 Then
                toReturn = String.Format("{0}&c={1}", toReturn, minutesCacheDuration.ToString(CultureInfo.InvariantCulture))
            End If
            Return toReturn
        End Function

        Public Function ProxifyUrl(ByVal originalUrl As String, baseProxyUrl As String, ByVal httpVerb As HttpVerb, ByVal minutesCacheDuration As Integer) As String

            Dim toReturn As String = ProxifyUrl(originalUrl, baseProxyUrl, minutesCacheDuration)

            If httpVerb <> Web.HttpVerb.Get Then
                toReturn = String.Format("{0}&m={1}", toReturn, httpVerb.ToString())
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
