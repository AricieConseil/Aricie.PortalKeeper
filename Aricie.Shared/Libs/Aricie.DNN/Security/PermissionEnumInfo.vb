
Namespace Security
    ''' <summary>
    ''' Class allowing quick access to the permission info for DotNetNuke entities
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class PermissionEnumInfo
        Implements IEquatable(Of PermissionEnumInfo)

        ''' <summary>
        ''' Tab view permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared TabView As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.Tab, "SYSTEM_TAB", "VIEW", "View Tab")
        ''' <summary>
        ''' Tab edition permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared TabEdit As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.Tab, "SYSTEM_TAB", "EDIT", "Edit Tab")
        ''' <summary>
        ''' Module view permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ModuleView As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.ModuleGlobal, "SYSTEM_MODULE_DEFINITION", "VIEW", "View")
        ''' <summary>
        ''' Module edition permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ModuleEdit As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.ModuleGlobal, "SYSTEM_MODULE_DEFINITION", "EDIT", "Edit")
        ''' <summary>
        ''' Folder read permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared FolderRead As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.Folder, "SYSTEM_FOLDER", "READ", "View Folder")
        ''' <summary>
        ''' Folder write permission structure
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared FolderWrite As PermissionEnumInfo = New PermissionEnumInfo(Security.PermissionType.Folder, "SYSTEM_FOLDER", "WRITE", "Write to Folder")

        Private _PermissionType As PermissionType

        Private _Code As String = String.Empty

        Private _Key As String = String.Empty

        Private _Name As String = String.Empty

        Private _ModuleNames As List(Of String)

#Region "cTor"

        Public Sub New(ByVal objPermissionType As PermissionType, ByVal permCode As String, ByVal permKey As String, ByVal permName As String, ByVal ParamArray moduleNames() As String)
            Me._PermissionType = objPermissionType
            Me._Code = permCode
            Me._Key = permKey
            Me._Name = permName
            If moduleNames IsNot Nothing Then
                Me._ModuleNames = New List(Of String)(moduleNames)
            Else
                Me._ModuleNames = New List(Of String)
            End If
        End Sub

#End Region

        ''' <summary>
        ''' Type de la permission
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PermissionType() As PermissionType
            Get
                Return _PermissionType
            End Get
            Set(ByVal value As PermissionType)
                _PermissionType = value
            End Set
        End Property

        ''' <summary>
        ''' Code permission
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Code() As String
            Get
                Return _Code
            End Get
            Set(ByVal value As String)
                _Code = value
            End Set
        End Property

        ''' <summary>
        ''' Clé permission
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Key() As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                _Key = value
            End Set
        End Property

        ''' <summary>
        ''' Nom de la permission
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        ''' <summary>
        ''' Liste de modules
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ModuleNames() As List(Of String)
            Get
                Return _ModuleNames
            End Get
            Set(ByVal value As List(Of String))
                _ModuleNames = value
            End Set
        End Property

        ''' <summary>
        ''' Egalité entre deux structures de permission
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Equals1(ByVal other As PermissionEnumInfo) As Boolean Implements IEquatable(Of PermissionEnumInfo).Equals
            Return Me.PermissionType = other.PermissionType AndAlso Me.Code = other.Code AndAlso Me.Key = other.Key
        End Function

    End Class

End Namespace