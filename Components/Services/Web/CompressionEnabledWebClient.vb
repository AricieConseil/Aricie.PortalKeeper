Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Reflection
Imports Aricie.Services
Imports System.Net.Security

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class CompressionEnabledWebClient
        Inherits WebClient

        Private Const DefaultUserAgent As String = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:27.0) Gecko/20100101 Firefox/27.0"
        Private Const DefaultAccept As String = "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8"
        Private Const DefaultAcceptCharset As String = "ISO-8859-1,utf-8;q=0.7,*;q=0.7"
        Private Const DefaultAcceptEncoding As String = "gzip, deflate"

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

       


        Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest

            Dim toreturn As WebRequest = MyBase.GetWebRequest(address)

            toreturn.Headers("Accept-Charset") = DefaultAcceptCharset
            toreturn.Headers("Accept-Encoding") = DefaultAcceptEncoding
            toreturn.Method = Me._Method
            toreturn.Timeout = CInt(Me._Timeout.TotalMilliseconds)

            If TypeOf toreturn Is HttpWebRequest Then
                Dim httpRequest As HttpWebRequest = DirectCast(toreturn, HttpWebRequest)
                httpRequest.UserAgent = DefaultUserAgent
                httpRequest.Accept = DefaultAccept
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
            toReturn.Headers("User-Agent") = DefaultUserAgent

            toReturn.Headers("Accept") = DefaultAccept
            toReturn.Headers("Accept-Charset") = DefaultAcceptCharset
            toReturn.Headers("Accept-Encoding") = DefaultAcceptEncoding

            'todo: removing that, can't remember why ssl certificate validation was off
            'ServicePointManager.ServerCertificateValidationCallback = AddressOf ValidateServerCertficate

            Return toReturn

        End Function

        Private Shared Function ValidateServerCertficate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslpolicyerrors As SslPolicyErrors) As Boolean

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