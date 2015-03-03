Imports System.ComponentModel
Imports DotNetNuke.Services.Personalization
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Entities.Portals

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Desktop, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Customize Environment Action")> _
        <Description("Tweaks various parameters from the dnn environment")> _
    Public Class CustomizeEnvironmentAction
        Inherits ActionProvider(Of RequestEvent)

        Public Enum PortalRegistrationType
            ' Fields
            DefaultRegistration = 4
            NoRegistration = 0
            PrivateRegistration = 1
            PublicRegistration = 2
            VerifiedRegistration = 3
        End Enum

        Private _SubPortalRegistrationType As PortalRegistrationType = PortalRegistrationType.DefaultRegistration


        Private _EnforceViewMode As Boolean


        <ExtendedCategory("Specifics")> _
        Public Property SubPortalRegistrationType() As PortalRegistrationType
            Get
                Return _SubPortalRegistrationType
            End Get
            Set(ByVal value As PortalRegistrationType)
                _SubPortalRegistrationType = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
        Public Property EnforceViewMode() As Boolean
            Get
                Return _EnforceViewMode
            End Get
            Set(ByVal value As Boolean)
                _EnforceViewMode = value
            End Set
        End Property


        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            If Me._SubPortalRegistrationType <> PortalRegistrationType.DefaultRegistration Then
                actionContext.DnnContext.Portal.UserRegistration = CInt(Me._SubPortalRegistrationType)
            End If
            If Me._EnforceViewMode AndAlso actionContext.DnnContext.IsAuthenticated Then
                If actionContext.DnnContext.Portal.UserMode <> PortalSettings.Mode.View Then
                    Personalization.SetProfile("Usability", ("UserMode" & actionContext.DnnContext.Portal.PortalId.ToString), "View")
                    Personalization.SetProfile("Usability", ("ContentVisible" & actionContext.DnnContext.Portal.PortalId.ToString), True.ToString)
                End If
                Return True
            End If
            Return False
        End Function


    End Class
End Namespace