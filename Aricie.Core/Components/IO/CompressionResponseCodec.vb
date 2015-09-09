Imports System.Web
Imports System.IO

Namespace IO
    Public Class CompressionResponseCodec
        Inherits DefaultHttpResponseCodec

        Public Overrides Function CanProcess(context As HttpContext) As Boolean
            Dim contentEncodingHeader As String = context.Response.Headers.Item("Content-Encoding")
            If Not contentEncodingHeader.IsNullOrEmpty() Then
                contentEncodingHeader = contentEncodingHeader.ToLowerInvariant()
                Select Case contentEncodingHeader
                    Case "gzip"
                        Return True
                    Case "deflate"
                        Return True
                End Select
            End If
            Return False
        End Function

        Public Overrides Function Decode(context As HttpContext, responseStream As MemoryStream, ByRef state As Object) As String
            Dim contentEncodingHeader As String = context.Response.Headers.Item("Content-Encoding")
            If Not contentEncodingHeader.IsNullOrEmpty() Then
                state = contentEncodingHeader.ToLowerInvariant()
                Select Case contentEncodingHeader
                    Case "gzip"
                        responseStream = responseStream.Decompress(CompressionMethod.Gzip)
                    Case "deflate"
                        responseStream = responseStream.Decompress(CompressionMethod.Deflate)
                End Select
            Else
                state = String.Empty
            End If

            Return MyBase.Decode(context, responseStream, state)


        End Function

        Public Overrides Function Encode(context As HttpContext, responseString As String, ByVal state As Object) As Stream
            Dim toReturn As Stream = MyBase.Encode(context, responseString, state)
            Dim contentEncodingHeader As String = DirectCast(state, String)
            If Not contentEncodingHeader.IsNullOrEmpty() Then
                Select Case contentEncodingHeader
                    Case "gzip"
                        toReturn = toReturn.Compress(CompressionMethod.Gzip)
                    Case "deflate"
                        toReturn = toReturn.Compress(CompressionMethod.Deflate)
                End Select
            End If
            Return toReturn
        End Function
    End Class
End Namespace