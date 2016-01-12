Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Security

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.SignOut, IconOptions.Normal)>
    <DisplayName("Log Off Action")>
    <Description("Signs the user out of his current session")>
    Public Class LogOffAction
        Inherits ActionProvider(Of RequestEvent)



        Public Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of RequestEvent)) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim ps As New PortalSecurity
            ps.SignOut()
            Return True
        End Function


    End Class
End Namespace