Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DNNRouteInfo

        Public Sub New()

        End Sub

        Public Sub New(folderName As String, mainNamespace As String)
            Me.FolderName = folderName
            Me.Namespaces.Add(mainNamespace)
        End Sub

        Public Property FolderName As String = PortalKeeperContext(Of SimpleEngineEvent).MODULE_NAME

        Public Property Namespaces As New List(Of String)


    End Class
End NameSpace