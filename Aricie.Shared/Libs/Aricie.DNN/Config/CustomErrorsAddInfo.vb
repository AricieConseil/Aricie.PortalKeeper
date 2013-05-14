Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Errors
Imports System.Web.Configuration
Imports System.Web
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Entities

Namespace Configuration
    ''' <summary>
    ''' Custom Errors Add merge node
    ''' </summary>
    <XmlRoot("customErrors")> _
    <Serializable()> _
    Public Class CustomErrorsAddInfo
        Inherits AddInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal objCustomErrors As CustomErrorsInfo)
            Me.Attributes("mode") = objCustomErrors.Mode.ToString
            'If System.Environment.Version.Major > 2 Then
            Me.Attributes("redirectMode") = objCustomErrors.DefaultRedirect.RedirectMode.ToString
            'End If
            If Not String.IsNullOrEmpty(objCustomErrors.DefaultRedirect.Url) Then
                Me.Attributes("defaultRedirect") = objCustomErrors.DefaultRedirect.UrlPath
            End If
        End Sub


    End Class
End Namespace