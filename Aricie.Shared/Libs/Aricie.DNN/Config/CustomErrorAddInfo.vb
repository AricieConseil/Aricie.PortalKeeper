Imports System.Globalization
Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Errors

Namespace Configuration


    ''' <summary>
    ''' Custom Error Add merge node
    ''' </summary>
    <XmlRoot("error")> _
    Public Class CustomErrorAddInfo
        Inherits AddInfoBase

        Public Sub New()
        End Sub


        Public Sub New(ByVal objError As CustomErrorInfo)
            Me.Attributes("statusCode") = CInt(objError.Status).ToString(CultureInfo.InvariantCulture)
            Me.Attributes("redirect") = objError.Redirect.UrlPath
        End Sub



    End Class
End Namespace