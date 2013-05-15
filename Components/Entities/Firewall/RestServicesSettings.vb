
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.Entities.Users
Imports OpenRasta.IO

Namespace Aricie.DNN.Modules.PortalKeeper

    <Serializable()> _
    Public Class RestServicesSettings

        Public Property Enabled As Boolean
        Public Property EnableOpenRastaLogger As Boolean
        Public Property EnableDigestAuthentication As Boolean


        Public Property Services As New SimpleList(Of RestService)

    End Class

End Namespace
