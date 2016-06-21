Imports System.ComponentModel
Imports DotNetNuke.Services.Personalization
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <DisplayName("Load Profile Property")> _
    <Description("This provider allows to load an indentity or personnalization profile property for a given user.")> _
    Public Class ProfileLoadActionProvider(Of TEngineEvents As IConvertible)
        Inherits ProfileActionProviderBase(Of TEngineEvents)


        <ConditionalVisible("DefaultValue")> _
        Public Property SaveDefault As Boolean

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Object = Nothing
            Dim objUser = Me.GetUser(actionContext)
            Select Case ProfileType
                Case PortalKeeper.ProfileType.Personalization
                    Dim pInfo As PersonalizationInfo = NukeHelper.PersonnalizationController.LoadProfile(objUser.UserID, objUser.PortalID)
                    toReturn = Personalization.GetProfile(pInfo, Me.NamingContainer, PropertyName)
                Case PortalKeeper.ProfileType.Identity
                    If AsString Then
                        toReturn = objUser.Profile.GetPropertyValue(PropertyName)
                    Else
                        Dim objDef As GeneralPropertyDefinition = GeneralPropertyDefinition.FromDNNProfileDefinition(objUser.Profile.GetProperty(PropertyName))
                        toReturn = objDef.TypedValue
                    End If
            End Select
            If toReturn Is Nothing AndAlso Me.DefaultValue.Enabled Then
                toReturn = Me.DefaultValue.Entity.Evaluate(actionContext, actionContext)
                if SaveDefault
                    me.SaveProfile(toReturn, objUser)
                End If
            End If
            Return toReturn
        End Function

        Protected Overrides Function GetOutputType() As Type
            If AsString Then
                Return GetType(String)
            Else
                Dim objDef As GeneralPropertyDefinition = GeneralPropertyDefinition.FromDNNProfileDefinition(DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionByName(NukeHelper.PortalId, PropertyName))
                Return objDef.GetValueType()
            End If
        End Function
    End Class
End NameSpace