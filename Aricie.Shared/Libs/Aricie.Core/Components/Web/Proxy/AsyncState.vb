Imports System.Net
Imports System.Web

Namespace Web.Proxy


    Public Class AsyncState
        Public Method As String
        Public Context As HttpContext
        Public Url As String
        Public CacheDuration As Integer
        Public OutboundRequest As HttpWebRequest
    End Class
End NameSpace