Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.Services.Personalization

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.User, IconOptions.Normal)>
    Public MustInherit Class ProfileActionProviderBase(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        Public Property UserMode As ProfileUserMode

        <ConditionalVisible("UserMode", True, True, ProfileUserMode.CurrentUser, ProfileUserMode.ByUserinfo)>
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)>
        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        Public Property PortalId As Integer

        <ConditionalVisible("UserMode", False, True, ProfileUserMode.ByUserId)>
        Public Property UserIdExpression As New SimpleOrExpression(Of Integer)

        <ConditionalVisible("UserMode", False, True, ProfileUserMode.ByUsername)>
        Public Property UsernameExpression As New SimpleOrExpression(Of String)

        <ConditionalVisible("UserMode", False, True, ProfileUserMode.ByUserinfo)>
        Public Property UserInfoExpression As New SimpleExpression(Of UserInfo)

        Public Function GetUser(objContext As PortalKeeperContext(Of TEngineEvents)) As UserInfo
            Dim toReturn As UserInfo = Nothing
            Select Case UserMode
                Case ProfileUserMode.CurrentUser
                    toReturn = objContext.DnnContext.User
                Case ProfileUserMode.ByUserId
                    toReturn = UserController.GetUser(Me.PortalId, UserIdExpression.GetValue(objContext, objContext), True)
                Case ProfileUserMode.ByUsername
                    toReturn = UserController.GetUserByName(Me.PortalId, UsernameExpression.GetValue(objContext, objContext), True)
                Case ProfileUserMode.ByUserinfo
                    toReturn = UserInfoExpression.Evaluate(objContext, objContext)
            End Select
            Return toReturn
        End Function

        Public Property ProfileType As ProfileType

        <ConditionalVisible("ProfileType", False, True, ProfileType.Identity)>
        Public Property AsString As Boolean


        <Required(True)>
        <ConditionalVisible("ProfileType", False, True, ProfileType.Personalization)>
        Public Property NamingContainer As String = ""

        <Required(True)>
        Public Property PropertyName As String = ""

        Property DefaultValue() As New EnabledFeature(Of AnonymousGeneralVariableInfo)

        Protected Sub SaveProfile(objValue As Object, objUser As UserInfo)
             Select Case ProfileType
                Case PortalKeeper.ProfileType.Personalization
                    Dim pInfo As PersonalizationInfo = NukeHelper.PersonnalizationController.LoadProfile(objUser.UserID, objUser.PortalID)
                    Personalization.SetProfile(pInfo, Me.NamingContainer, Me.PropertyName, objValue)
                    NukeHelper.PersonnalizationController.SaveProfile(pInfo)
                Case PortalKeeper.ProfileType.Identity
                    If AsString Then
                        objUser.Profile.SetProfileProperty(PropertyName, CStr(objValue))
                    Else
                        Dim objDef As GeneralPropertyDefinition = GeneralPropertyDefinition.FromDNNProfileDefinition(objUser.Profile.GetProperty(PropertyName))
                        objUser.Profile.SetProfileProperty(PropertyName, objDef.PropertyValue)
                    End If
                    UserController.UpdateUser(objUser.PortalID, objUser)
            End Select
        End Sub


    End Class
End Namespace