Imports System.IO
Imports System.Text
Imports System.Web

Namespace IO
    Public Class DefaultHttpResponseCodec
        Inherits HttpResponseCodecBase
        'Implements IHttpResponseCodec



        Public Overrides Function Decode(context As HttpContext, responseStream As MemoryStream, ByRef state As Object) As String ' Implements IHttpResponseCodec.Decode

            'Return context.Response.ContentEncoding.GetString(responseStream.ToArray())
            Dim sr As New StreamReader(responseStream, context.Response.ContentEncoding, True)
            Dim toReturn As String = sr.ReadToEnd()
            context.Response.ContentEncoding = sr.CurrentEncoding
            Return toReturn
        End Function

        Public Overrides Function Encode(context As HttpContext, responseString As String, ByVal state As Object) As Stream 'Implements IHttpResponseCodec.Encode

           
            Dim objBytes As Byte() = context.Response.ContentEncoding.GetBytes(responseString)

            Dim toReturn As New MemoryStream(objBytes)

            Return toReturn
        End Function

        Public Overrides Function CanProcess(context As HttpContext) As Boolean 'Implements IHttpResponseCodec.CanProcess
            Return True
        End Function
    End Class
End Namespace