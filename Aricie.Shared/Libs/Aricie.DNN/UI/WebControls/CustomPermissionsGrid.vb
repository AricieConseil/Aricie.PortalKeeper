Imports DotNetNuke.Security.Permissions.Controls
Imports DotNetNuke.Security.Permissions

Namespace UI.WebControls

    Public Class CustomPermissionsGrid
        Inherits PermissionsGrid



        Private ReadOnly Property InternalAutorizedRoles() As List(Of Integer)
            Get
                If ViewState("InternalAutorizedRoles") Is Nothing Then
                    ViewState("InternalAutorizedRoles") = New List(Of Integer)
                End If

                Return CType(ViewState("InternalAutorizedRoles"), List(Of Integer))
            End Get
        End Property

        Public ReadOnly Property AutorizedRoles() As List(Of Integer)
            Get
                UpdatePermissions()
                Return Me.InternalAutorizedRoles
            End Get
        End Property

        Protected Overrides Function GetPermission(ByVal objPerm As DotNetNuke.Security.Permissions.PermissionInfo, ByVal role As DotNetNuke.Security.Roles.RoleInfo, ByVal column As Integer) As Boolean
            Return Me.InternalAutorizedRoles.Contains(role.RoleID)
        End Function

        Protected Overrides Sub UpdatePermission(ByVal permission As DotNetNuke.Security.Permissions.PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal allowAccess As Boolean)

            If allowAccess Then

                If Not Me.InternalAutorizedRoles.Contains(roleId) Then
                    Me.InternalAutorizedRoles.Add(roleId)
                End If
            Else
                If Me.InternalAutorizedRoles.Contains(roleId) Then
                    Me.InternalAutorizedRoles.Remove(roleId)
                End If
            End If

        End Sub

        Protected Overrides Function GetPermissions() As ArrayList

            Dim permissions As New ArrayList

            Dim p As New PermissionInfo

            p.PermissionCode = "VIEW"
            p.PermissionKey = "VIEW"
            p.PermissionName = "View"

            permissions.Add(p)

            Return permissions
        End Function

        Public Overrides Sub GenerateDataGrid()

        End Sub

        Protected Overrides Function GetUsers() As System.Collections.ArrayList
            Return Nothing
        End Function
    End Class

End Namespace
