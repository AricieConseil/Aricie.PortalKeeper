Imports System.Linq

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DNNRouteInfo

        Public Sub New()
        End Sub

        Private _defaultNamespace As String = ""
        Private _namespaces As New List(Of String)

        Public Sub New(folderName As String, mainNamespace As String)
            Me.New()
            Me.FolderName = folderName
            _defaultNamespace = mainNamespace
        End Sub

        Public Property FolderName As String = PortalKeeperContext(Of SimpleEngineEvent).MODULE_NAME

        Public Property Namespaces As List(Of String)
            Get
               Return _namespaces
            End Get
            Set
                _namespaces = value
                 If Not _defaultNamespace.IsNullOrEmpty() AndAlso Not _namespaces.Contains(_defaultNamespace) Then
                    _namespaces.Add(_defaultNamespace)
                End If
            End Set
        End Property
    End Class
End NameSpace