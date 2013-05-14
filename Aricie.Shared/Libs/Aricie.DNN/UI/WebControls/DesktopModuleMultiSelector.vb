Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Services

Namespace UI.WebControls

    Public Class DesktopModuleMultiSelector
        Inherits MultiSelectorControl(Of DesktopModuleInfo)

        Private _PortalId As Integer = -1

        Public Property PortalId() As Integer
            Get
                If _PortalId = -1 Then
                    Me._PortalId = NukeHelper.PortalId
                End If
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property

        Public Overrides Function GetEntitiesG() As IList(Of DesktopModuleInfo)
            Dim dmc As New DesktopModuleController
            Return New List(Of DesktopModuleInfo)(DirectCast(dmc.GetDesktopModulesByPortal(Me.PortalId).ToArray(GetType(DesktopModuleInfo)), DesktopModuleInfo()))
        End Function

    End Class

End Namespace
