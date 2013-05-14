
Imports Aricie.DNN.Configuration
Imports DotNetNuke.Security.Permissions
Imports Aricie.Services
Imports Aricie.DNN.Services
Imports System.Globalization

Namespace Security

    ''' <summary>
    ''' Interface to retrieve module permissions
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Interface IModulePermissionsProvider(Of T)

        Function GetPermissionEnumInfo(ByVal perm As T) As PermissionEnumInfo

    End Interface

    ''' <summary>
    ''' Helper class for generic permission sets
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>The generic argument represents the set of permissions. The class can install/uninstall the permission set through IConfigElementInfo implementation</remarks>
    Public MustInherit Class PermissionHelper(Of T)
        Implements IModulePermissionsProvider(Of T)
        Implements IConfigElementInfo

#Region "Private members"

        Private Shared _PermissionController As New PermissionController
        Private Const glbPermDependency As String = Constants.glbPrefix & "-Perms"

        Private _PermissionEnumByKey As Dictionary(Of String, T)
        Private _PermissionEnumInfoByEnum As Dictionary(Of T, PermissionEnumInfo)
        Private _PermissionsInstalled As Boolean = False
#End Region

#Region "Public members"

        ''' <summary>
        ''' Récupération des permissions pour chaque type indépendant
        ''' </summary>
        ''' <param name="perm"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PermissionDef(ByVal perm As T) As PermissionEnumInfo
            Get
                If _PermissionEnumInfoByEnum Is Nothing Then
                    _PermissionEnumInfoByEnum = New Dictionary(Of T, PermissionEnumInfo)
                    For Each objPerm As T In GetEnumMembers(Of T)()
                        _PermissionEnumInfoByEnum(objPerm) = Me.GetPermissionEnumInfo(objPerm)
                    Next

                End If
                Return _PermissionEnumInfoByEnum(perm)
            End Get
        End Property

        ''' <summary>
        ''' Liste de définition de modules
        ''' </summary>
        ''' <param name="perm"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ModuleDefIds(ByVal perm As T) As List(Of Integer)
            Get
                Dim toReturn As List(Of Integer) = GetGlobal(Of List(Of Integer))("permDefIds", Me.GetType.FullName, perm.ToString)
                If toReturn Is Nothing Then
                    toReturn = New List(Of Integer)
                    Dim permEnumInfo As PermissionEnumInfo = Me.PermissionDef(perm)
                    If permEnumInfo.PermissionType = PermissionType.ModuleSpecific Then
                        For Each moduleName As String In Me.PermissionDef(perm).ModuleNames
                            Dim mDefid As Integer = GetModuleDefIdByModuleName(moduleName)
                            If mDefid <> -1 Then
                                toReturn.Add(mDefid)
                            End If
                        Next
                    Else
                        toReturn.Add(-1)
                    End If

                    SetCacheDependant(Of List(Of Integer))(toReturn, glbPermDependency, TimeSpan.FromMinutes(20), "permDefIds", Me.GetType.FullName, perm.ToString)
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' récupération de la permission DNN pour un objet
        ''' </summary>
        ''' <param name="permission"></param>
        ''' <param name="moduleDefId"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PermissionInfo(ByVal permission As T, Optional ByVal moduleDefId As Integer = -1) As PermissionInfo
            Get
                Dim toReturn As PermissionInfo = GetGlobal(Of PermissionInfo)(permission.ToString, "Mod", moduleDefId.ToString(CultureInfo.InvariantCulture))
                If toReturn Is Nothing Then

                    'Dim key As String = Me.GetPermissionKey(permission)
                    Dim permEnumInfo As PermissionEnumInfo = Me.PermissionDef(permission)
                    Select Case permEnumInfo.PermissionType
                        Case PermissionType.Folder, PermissionType.Tab, PermissionType.ModuleGlobal
                            Dim objPermissions As ArrayList = _PermissionController.GetPermissionByCodeAndKey(permEnumInfo.Code, permEnumInfo.Key)
                            If objPermissions IsNot Nothing AndAlso objPermissions.Count > 0 Then
                                toReturn = DirectCast(objPermissions(0), PermissionInfo)
                                SetCacheDependant(Of PermissionInfo)(toReturn, glbPermDependency, TimeSpan.FromMinutes(20), permission.ToString, "Tab")
                            End If
                        Case PermissionType.ModuleSpecific
                            If moduleDefId = -1 AndAlso Me.PermissionDef(permission).PermissionType = PermissionType.ModuleSpecific Then
                                moduleDefId = ModuleDefIds(permission)(0)
                            End If
                            Dim objPermissions As ArrayList = _PermissionController.GetPermissionsByModuleDefID(moduleDefId)
                            For Each perm As PermissionInfo In objPermissions
                                If perm.PermissionCode = permEnumInfo.Code AndAlso perm.PermissionKey = permEnumInfo.Key Then
                                    toReturn = perm

                                End If
                            Next
                    End Select

                    If toReturn Is Nothing Then

                        toReturn = New PermissionInfo
                        toReturn.PermissionCode = permEnumInfo.Code
                        toReturn.PermissionKey = permEnumInfo.Key
                        toReturn.PermissionName = permEnumInfo.Name
                        toReturn.ModuleDefID = moduleDefId
                    Else
                        SetCacheDependant(Of PermissionInfo)(toReturn, glbPermDependency, TimeSpan.FromMinutes(20), permission.ToString, moduleDefId.ToString(CultureInfo.InvariantCulture))
                    End If

                End If

                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' Permission récupérée via la clé de permission
        ''' </summary>
        ''' <param name="permissionKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPermissionEnum(ByVal permissionKey As String) As T

            If _PermissionEnumByKey Is Nothing Then
                _PermissionEnumByKey = New Dictionary(Of String, T)
                For Each objEnum As T In GetEnumMembers(Of T)()
                    _PermissionEnumByKey(Me.PermissionDef(objEnum).Key) = objEnum
                Next
            End If
            If _PermissionEnumByKey.ContainsKey(permissionKey) Then
                Return _PermissionEnumByKey(permissionKey)
            End If
            Return Nothing

        End Function

#End Region

#Region "IModulePermissionsProvider"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="perm"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetPermissionEnumInfo(ByVal perm As T) As PermissionEnumInfo Implements IModulePermissionsProvider(Of T).GetPermissionEnumInfo

#End Region

#Region "IConfigElementInfo"
        ''' <summary>
        ''' Verifies that the permission is installed.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsInstalled() As Boolean Implements IConfigElementInfo.IsInstalled

            If Not Me._PermissionsInstalled Then

                For Each perm As T In GetEnumMembers(Of T)()

                    Dim permEnumInfo As PermissionEnumInfo = Me.PermissionDef(perm)
                    Dim moduleDefIds As List(Of Integer) = Me.ModuleDefIds(perm)

                    For Each mDefId As Integer In moduleDefIds
                        Dim objPermission As PermissionInfo = Me.PermissionInfo(perm, mDefId)
                        If objPermission.PermissionID = 0 Then
                            Me._PermissionsInstalled = False
                            Return Me._PermissionsInstalled
                        End If
                    Next

                Next
                Me._PermissionsInstalled = True
            End If
            Return Me._PermissionsInstalled

        End Function

        ''' <summary>
        ''' Installs the permission
        ''' </summary>
        ''' <param name="actionType"></param>
        ''' <remarks></remarks>
        Public Sub ProcessConfig(ByVal actionType As ConfigActionType) Implements IConfigElementInfo.ProcessConfig
            Select Case actionType
                Case ConfigActionType.Install
                    Me.ResetPermissions(True)
                Case ConfigActionType.Uninstall
                    Me.UninstallPermissions()
            End Select
        End Sub

#End Region

#Region "Private methods"


        ''' <summary>
        ''' Permission reset
        ''' </summary>
        ''' <param name="deleteNonEnumPerms"></param>
        ''' <remarks></remarks>
        Private Sub ResetPermissions(ByVal deleteNonEnumPerms As Boolean)

            Dim overAllMDefIds As New List(Of Integer)
            For Each perm As T In GetEnumMembers(Of T)()

                Dim permEnumInfo As PermissionEnumInfo = Me.PermissionDef(perm)
                Dim moduleDefIds As List(Of Integer) = Me.ModuleDefIds(perm)
                overAllMDefIds.AddRange(moduleDefIds)
                For Each mDefId As Integer In moduleDefIds
                    Dim objPermission As PermissionInfo = Me.PermissionInfo(perm, mDefId)
                    If objPermission.PermissionID = 0 Then
                        _PermissionController.AddPermission(objPermission)
                    Else
                        'other properties should be right since me.PermissionInfo filters by code and key
                        objPermission.PermissionName = permEnumInfo.Name
                        _PermissionController.UpdatePermission(objPermission)
                    End If
                Next

            Next
            If deleteNonEnumPerms Then
                'clean obsolete module permissions
                'module specific permissions should span all permissions
                For Each mDefId As Integer In overAllMDefIds
                    If mDefId <> -1 Then
                        For Each objExPermission As PermissionInfo In _PermissionController.GetPermissionsByModuleDefID(mDefId)
                            If Me.GetPermissionEnum(objExPermission.PermissionKey) Is Nothing Then
                                _PermissionController.DeletePermission(objExPermission.PermissionID)
                            End If
                        Next
                    End If
                Next
            End If
            Me._PermissionsInstalled = True

        End Sub

        ''' <summary>
        ''' Uninstall permission
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub UninstallPermissions()

            For Each perm As T In GetEnumMembers(Of T)()

                Dim permEnumInfo As PermissionEnumInfo = Me.PermissionDef(perm)

                If permEnumInfo.Equals1(PermissionEnumInfo.TabView) OrElse _
                    permEnumInfo.Equals1(PermissionEnumInfo.TabEdit) OrElse _
                    permEnumInfo.Equals1(PermissionEnumInfo.ModuleView) OrElse _
                    permEnumInfo.Equals1(PermissionEnumInfo.ModuleEdit) OrElse _
                    permEnumInfo.Equals1(PermissionEnumInfo.FolderRead) OrElse _
                    permEnumInfo.Equals1(PermissionEnumInfo.FolderWrite) Then
                    Continue For
                End If

                Dim moduleDefIds As List(Of Integer) = Me.ModuleDefIds(perm)
                For Each mDefId As Integer In moduleDefIds
                    Dim objPermission As PermissionInfo = Me.PermissionInfo(perm, mDefId)
                    If objPermission.PermissionID <> -1 Then

                        _PermissionController.DeletePermission(objPermission.PermissionID)

                    End If
                Next

            Next
            Me._PermissionsInstalled = False
        End Sub

        ''' <summary>
        ''' Adds a module permission
        ''' </summary>
        ''' <param name="ModuleId"></param>
        ''' <param name="permInfo"></param>
        ''' <param name="scopeId"></param>
        ''' <param name="scopeName"></param>
        ''' <param name="AddForUser"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddModulePerm(ByVal ModuleId As Integer, ByVal permInfo As PermissionInfo, ByVal scopeId As Integer, ByVal scopeName As String, Optional ByVal AddForUser As Boolean = False) As ModulePermissionInfo
            Dim newPerm As New ModulePermissionInfo
            newPerm.ModuleID = ModuleId
            newPerm.ModulePermissionID = -1
            newPerm.PermissionID = permInfo.PermissionID
            newPerm.PermissionCode = permInfo.PermissionCode
            newPerm.PermissionKey = permInfo.PermissionKey
            newPerm.PermissionName = permInfo.PermissionName
            newPerm.AllowAccess = True
            If AddForUser Then
                newPerm.UserID = scopeId
                newPerm.Username = scopeName
            Else
                newPerm.RoleID = scopeId
                newPerm.RoleName = scopeName
            End If
            Return newPerm
        End Function

        ''' <summary>
        ''' Adds a tab permission
        ''' </summary>
        ''' <param name="tabId"></param>
        ''' <param name="permInfo"></param>
        ''' <param name="scopeId"></param>
        ''' <param name="scopeName"></param>
        ''' <param name="AddForUser"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddTabPerm(ByVal tabId As Integer, ByVal permInfo As PermissionInfo, ByVal scopeId As Integer, ByVal scopeName As String, Optional ByVal AddForUser As Boolean = False) As TabPermissionInfo
            Dim newPerm As New TabPermissionInfo
            newPerm.TabID = tabId
            newPerm.TabPermissionID = -1
            newPerm.PermissionID = permInfo.PermissionID
            newPerm.PermissionCode = permInfo.PermissionCode
            newPerm.PermissionKey = permInfo.PermissionKey
            newPerm.PermissionName = permInfo.PermissionName
            newPerm.AllowAccess = True
            If AddForUser Then
                newPerm.UserID = scopeId
                newPerm.Username = scopeName
            Else
                newPerm.RoleID = scopeId
                newPerm.RoleName = scopeName
            End If
            Return newPerm
        End Function

#End Region

    End Class

End Namespace