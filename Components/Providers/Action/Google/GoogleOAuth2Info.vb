Imports Aricie.DNN.Entities
Imports Aricie.ComponentModel
Imports Google.GData.Client
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class GoogleOAuth2Info
        Inherits APICredentials

        Public Sub New()

        End Sub

        Public Sub New(strScope As String)
            Me.Scope = strScope
        End Sub

        Private Const DeviceRedirectUri As String = "urn:ietf:wg:oauth:2.0:oob"

        <Required(True)> _
        Public Property ApplicationName As String = "<Your application name here>"

        Public Property Scope As New CData("https://docs.google.com/feeds")


        Public Function GetParameters() As OAuth2Parameters
            Dim toReturn As New OAuth2Parameters()
            toReturn.ClientId = Me.Key
            toReturn.ClientSecret = Me.Secret
            toReturn.RedirectUri = DeviceRedirectUri
            toReturn.Scope = Scope
            Return toReturn
        End Function

    End Class
End NameSpace