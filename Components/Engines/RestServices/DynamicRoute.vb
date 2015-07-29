Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DynamicRoute
        Inherits NamedConfig

        Public Sub New()

        End Sub

        Public Sub New(strName As String, strPattern As String)
            Me.Name = strName
            Me.Template = strPattern
        End Sub

        Public Property Template As String = "{controller}/{action}"

        Public Property DNNRoute As New EnabledFeature(Of DNNRouteInfo)


    End Class
End NameSpace