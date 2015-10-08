Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Road, IconOptions.Normal)> _
    Public Class DynamicRoute
        Inherits NamedConfig

        Public Sub New()

        End Sub

        Public Sub New(strName As String, strPattern As String)
            Me.Name = strName
            Me.Template = strPattern
        End Sub

        Public Sub New(strName As String, strPattern As String, ByVal makeDNNRoute As Boolean)
            Me.New(strName, strPattern)
            If makeDNNRoute Then
                Me.DNNRoute.Enabled = True
            Else
                Me.DNNRoute.Enabled = False
            End If
        End Sub

        Public Property Template As String = "{controller}/{action}"


        Public Property DNNRoute As New EnabledFeature(Of DNNRouteInfo)(New DNNRouteInfo(PortalKeeperContext(Of SimpleEngineEvent).MODULE_NAME, _
                                                                                         PortalKeeperContext(Of SimpleEngineEvent).DYNAMIC_CONTROLLER_NAMESPACE), True)


        Public Property Defaults As New Variables()

        Public Property Constraints As New Variables()

    End Class
End Namespace