Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Reflection
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class CompressionEnabledWebClient
        Inherits WebClient

        Public Sub New(ByVal verb As String)
            Me._Method = verb
        End Sub


        Private _Method As String

        Private _Timeout As TimeSpan = TimeSpan.FromMilliseconds(5000)


        Public Property Method() As String
            Get
                Return _Method
            End Get
            Set(ByVal value As String)
                _Method = value
            End Set
        End Property


        Public Property Timeout() As TimeSpan
            Get
                Return _Timeout
            End Get
            Set(ByVal value As TimeSpan)
                _Timeout = value
            End Set
        End Property

        Public Property VirtualProxy As Boolean


        Protected Overrides Function GetWebRequest(ByVal address As System.Uri) As System.Net.WebRequest
            'Dim toReturn As HttpWebRequest = DotNetNuke.Common.Globals.GetExternalRequest(address.AbsoluteUri)

            Dim toreturn As WebRequest = MyBase.GetWebRequest(address)

            'toReturn.Headers("User-Agent") = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1"
            'toReturn.Headers("Accept") = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
            toreturn.Headers("Accept-Charset") = "ISO-8859-1,utf-8;q=0.7,*;q=0.7"
            toreturn.Headers("Accept-Encoding") = "gzip, deflate"
            toreturn.Method = Me._Method
            toreturn.Timeout = CInt(Me._Timeout.TotalMilliseconds)
            If TypeOf toreturn Is HttpWebRequest Then
                Dim httpRequest As HttpWebRequest = DirectCast(toreturn, HttpWebRequest)
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1"
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
                httpRequest.AutomaticDecompression = DecompressionMethods.Deflate Or DecompressionMethods.GZip

                If httpRequest.Proxy IsNot Nothing AndAlso Me.VirtualProxy Then
                    FieldProxyServicePoint.SetValue(httpRequest.ServicePoint, False)
                End If

            End If

            Return toreturn
        End Function


        Public Shared Function GetWebClient(ByVal method As String, ByVal timeout As TimeSpan) As CompressionEnabledWebClient
            Dim toReturn As New CompressionEnabledWebClient(method)
            toReturn._Timeout = timeout
            toReturn.Headers("User-Agent") = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1"
            'toReturn.Headers("User-Agent") = "Wget/1.9.1"

            toReturn.Headers("Accept") = "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8"
            toReturn.Headers("Accept-Charset") = "ISO-8859-1,utf-8;q=0.7,*;q=0.7"
            toReturn.Headers("Accept-Encoding") = "gzip, deflate"
            'If ServicePointManager.SecurityProtocol <> SecurityProtocolType.Ssl3 Then
            '    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
            'End If
            ServicePointManager.ServerCertificateValidationCallback = AddressOf ValidateServerCertficate

            Return toReturn

        End Function

        Private Shared Function ValidateServerCertficate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslpolicyerrors As System.Net.Security.SslPolicyErrors) As Boolean

            'If (sslpolicyerrors = sslpolicyerrors.None) Then
            '    Return True
            'End If

            Return True


        End Function

        Private Shared _FieldProxyServicePoint As FieldInfo

        Private Shared ReadOnly Property FieldProxyServicePoint As FieldInfo
            Get
                If _FieldProxyServicePoint Is Nothing Then
                    _FieldProxyServicePoint = DirectCast(ReflectionHelper.GetMember(GetType(ServicePoint), "m_ProxyServicePoint"), FieldInfo)
                End If
                Return _FieldProxyServicePoint
            End Get
        End Property

    End Class
End Namespace