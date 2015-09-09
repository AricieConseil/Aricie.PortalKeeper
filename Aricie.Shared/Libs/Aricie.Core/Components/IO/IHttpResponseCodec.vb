Imports System.IO
Imports System.Web

Namespace IO
    Public MustInherit Class HttpResponseCodecBase

        Public MustOverride Function CanProcess(context As HttpContext) As Boolean

        Public MustOverride Function Decode(context As HttpContext, responseStream As MemoryStream, ByRef state As Object) As String

        Public MustOverride Function Encode(context As HttpContext, responseString As String, ByVal state As Object) As Stream

    End Class
End NameSpace