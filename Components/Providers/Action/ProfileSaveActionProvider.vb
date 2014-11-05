Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports DotNetNuke.Services.Personalization
Imports Aricie.DNN.Services
Imports DotNetNuke.Entities.Users

Namespace Aricie.DNN.Modules.PortalKeeper
    <DisplayName("Save Profile Property")> _
    <Description("This provider allows to save an identity or personnalization profile property for a given user.")> _
    Public Class ProfileSaveActionProvider(Of TEngineEvents As IConvertible)
        Inherits ProfileActionProviderBase(Of TEngineEvents)


        <Browsable(False)> _
        Public Overrides ReadOnly Property ShowOutput As Boolean
            Get
                Return False
            End Get
        End Property

        Public Property Value As New FleeExpressionInfo(Of Object)

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim objUser = Me.GetUser(actionContext)
            Dim objValue As Object = Me.Value.Evaluate(actionContext, actionContext)
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
            Return True
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return GetType(Boolean)
        End Function
    End Class
End NameSpace