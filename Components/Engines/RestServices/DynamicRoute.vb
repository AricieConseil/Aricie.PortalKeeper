Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
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
                Me.DNNRoute.Entity.FolderName = "Aricie.Portalkeeper"
                Me.DNNRoute.Entity.Namespaces.Add("Aricie.PortalKeeper.DNN7")
            End If
        End Sub

        Public Property Template As String = "{controller}/{action}"


        Public Property DNNRoute As New EnabledFeature(Of DNNRouteInfo)


        Public Property Defaults As New Variables()

        Public Property Constraints As New Variables()

    End Class
End NameSpace