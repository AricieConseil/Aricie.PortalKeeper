Imports System.ComponentModel
Imports DotNetNuke.Framework
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Users
Imports System.Globalization
Imports System.Web.UI
Imports Aricie.Web.UI
Imports Aricie.DNN.Services
Imports DotNetNuke.Services.Localization
Imports System.Threading

Namespace UI.Controls
    Public Class AricieUserControlBase
        Inherits UserControlBase

#Region "private Members"

        Private _MyFileName As String
        Private _ModuleID As Integer = -1
        Private _ModuleSettings As Hashtable
        Private _localResourceFile As String
        Private _ParentModuleBase As PortalModuleBase
        Private _ParentSkinBase As SkinObjectBase

#End Region

#Region "Public Properties"


        Public ReadOnly Property ParentSkinBase() As SkinObjectBase
            Get
                If Me._ParentSkinBase Is Nothing Then
					Dim parentControl As Control = Web.UI.ControlHelper.FindControlRecursive(Me, GetType(SkinObjectBase))
                    If Not parentControl Is Nothing Then
                        Me._ParentSkinBase = CType(parentControl, SkinObjectBase)
                    End If
                End If
                Return Me._ParentSkinBase
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BasePage() As CDefault
            Get
                Return DirectCast(Me.Page, CDefault)
            End Get
        End Property

        Public ReadOnly Property TabId() As Integer
            Get
                Return NukeHelper.PortalSettings.ActiveTab.TabID
            End Get
        End Property

        Public ReadOnly Property UserInfo() As UserInfo
            Get
                Return UserController.GetCurrentUserInfo
            End Get
        End Property

        Public ReadOnly Property UserId() As Integer
            Get
                Return UserController.GetCurrentUserInfo.UserID
            End Get
        End Property

        Public ReadOnly Property IsEditable() As Boolean
            Get
                Return Me.ParentModuleBase.IsEditable
            End Get
        End Property

        Public ReadOnly Property PortalId() As Integer
            Get
                Return NukeHelper.PortalId
            End Get
        End Property



        Public Property MyFileName() As String
            Get
                If Me._MyFileName = "" Then
                    Me.MyFileName = Me.GetType.BaseType.Name & ".ascx"
                    'Me.GetType.Name.Replace("_ascx", ".ascx")
                End If
                Return Me._MyFileName
            End Get
            Set(ByVal Value As String)
                Me._MyFileName = Value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                If Me._localResourceFile = "" Then
                    Me._localResourceFile = Localization.GetResourceFile(Me, Me.MyFileName)
                End If
                Return Me._localResourceFile
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property

        Public ReadOnly Property ParentModuleBase() As PortalModuleBase
            Get
                If Me._ParentModuleBase Is Nothing Then
                    Dim parentControl As Control = GetParentModuleBase(Me)
                    If Not parentControl Is Nothing Then
                        Me._ParentModuleBase = CType(parentControl, PortalModuleBase)
                    End If
                End If
                Return Me._ParentModuleBase
            End Get
        End Property

        Public ReadOnly Property ModuleID() As Integer
            Get
                If Me._ModuleID = -1 Then
                    If Not Me.ParentModuleBase Is Nothing Then
                        If TypeOf Me._ParentModuleBase Is ModuleSettingsBase Then
                            Me._ModuleID = CType(Me.ParentModuleBase, ModuleSettingsBase).ModuleId
                            Me._ModuleSettings = CType(Me.ParentModuleBase, ModuleSettingsBase).Settings
                        Else
                            Me._ModuleID = Me.ParentModuleBase.ModuleId
                            Me._ModuleSettings = Me.ParentModuleBase.Settings
                        End If
                    End If
                End If
                Return Me._ModuleID
            End Get
        End Property

        Public ReadOnly Property ModuleSettings() As Hashtable
            Get
                If Me._ModuleSettings Is Nothing Then
                    If Not Me.ParentModuleBase Is Nothing Then
                        If TypeOf Me._ParentModuleBase Is ModuleSettingsBase Then
                            Me._ModuleSettings = CType(Me.ParentModuleBase, ModuleSettingsBase).Settings
                        Else
                            Me._ModuleSettings = Me.ParentModuleBase.Settings
                        End If
                    End If
                End If
                Return Me._ModuleSettings
            End Get
        End Property

        Public ReadOnly Property SharedResourceFile() As String
            Get
                Return _
                    Me.Parent.TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/" & _
                    Localization.LocalSharedResourceFile
            End Get
        End Property

        Public ReadOnly Property CurrentCulture() As CultureInfo
            Get
                Return Thread.CurrentThread.CurrentCulture
            End Get
        End Property

        Public ReadOnly Property IsFirstLoad() As Boolean
            Get
                Return String.IsNullOrEmpty(CStr(Me.ViewState("IsFirstLoad")))
            End Get
        End Property
#End Region

#Region "public methods"

        Public Sub SetParentModuleBase(ByVal control As PortalModuleBase)

            Me._ParentModuleBase = control

        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            Me.ViewState("IsFirstLoad") = "False"
        End Sub

#End Region
    End Class
End Namespace


