Imports System.Runtime.Serialization
Imports System.Security.Cryptography.X509Certificates

Namespace Security
    <Serializable> _
    Friend Class CertificateException
        Inherits Exception
        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub

        ' A constructor is needed for serialization when an 
        ' exception propagates from a remoting server to the client.  
        Protected Sub New(info As SerializationInfo, context As StreamingContext)
        End Sub

        Public Shared Function NoPrivateKeyMessage(thumpPrint As String) As String
            Return String.Format("Certificate with ThumbPrint ""{0}"" has no private key.", thumpPrint)
        End Function

        Public Shared Function CertificateNotFoundMessage(thumbPrint As String, store As X509Store) As String
            Return String.Format("Certificate with ThumbPrint ""{0}"" was not found in {1}\{3}", thumbPrint, store.Location, store.Name)
        End Function
    End Class
End Namespace