
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

        Public Function FindServiceByKey(resourceKey As Object) As RestService
            Dim toReturn As RestService = Nothing
            Dim resourceType As Type = TryCast(resourceKey, Type)
            If resourceType IsNot Nothing Then
                For Each objService As RestService In Me.Services.Instances
                    If objService.ResourceType.GetDotNetType() Is resourceType Then
                        toReturn = objService
                    End If
                Next
            End If
            Return toReturn
        End Function

    End Class

End Namespace
