Imports OpenRasta.Security
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DNNAuthenticationProvider
        Implements IAuthenticationProvider



        Public Function GetByUsername(p As String) As Credentials Implements IAuthenticationProvider.GetByUsername
            Dim toReturn As New Credentials()
            Dim objUser As UserInfo = UserController.GetUserByName(DnnContext.Current.Portal.PortalId, p)
            If objUser.UserID >= 0 Then
                toReturn.Username = p
                toReturn.Password = UserController.GetPassword(objUser, "")
                toReturn.Roles = objUser.Roles
            End If
            Return toReturn
        End Function
    End Class
End Namespace
