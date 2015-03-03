Imports System.ComponentModel
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Settings
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Framework
Imports System.Threading
Imports DotNetNuke.Services.Localization
Imports Aricie.DNN.Services
Imports System.Web.UI.HtmlControls
Imports System.Web.UI
Imports System.Reflection
Imports DotNetNuke.UI.Utilities
Imports System.Web.UI.WebControls


Namespace UI.Controls
    Public Class AriciePortalModuleBase
        Inherits PortalModuleBase


#Region "private Members"

        '       Private _MapID As Integer = -2
        '       Private _HotSpotID As Integer = -2

        Protected _GeneralSettings As GeneralSettings

        Protected _PersonalSettings As PersonalSettings

        Private _Tab As TabInfo

        Protected _CanPerformAction As Hashtable = New Hashtable


#End Region

#Region "Protected members"

        '   Public objMap As ModuleWorkflowsInfo

#End Region

#Region "Public Properties"

        Public ReadOnly Property CurrentCulture() As String
            Get
                Return Thread.CurrentThread.CurrentCulture.ToString
                ' _CurrentCulture
            End Get
        End Property

        Public ReadOnly Property Tab() As TabInfo
            Get
                If Me._Tab Is Nothing Then
                    Dim tc As New TabController
                    Me._Tab = tc.GetTab(Me.TabId, Me.PortalId, False)
                End If
                Return Me._Tab
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property BasePage() As CDefault
            Get
                Return DirectCast(Me.Page, CDefault)
            End Get
        End Property






#Region "Settings Properties"

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Overridable Property PersonalSettings() As PersonalSettings
            Get
                If Me._PersonalSettings Is Nothing Then
                    Me._PersonalSettings = SettingsController.GetPersonalSettings(Me.PortalId)
                End If
                Return Me._PersonalSettings
            End Get
            Set(ByVal Value As PersonalSettings)
                Me._PersonalSettings = Value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Overridable Property GeneralSettings() As GeneralSettings
            Get
                If Me._GeneralSettings Is Nothing Then
                    Me._GeneralSettings = _
                       SettingsController.GetModuleSettings(Of GeneralSettings)(SettingsScope.ModuleSettings, Me.ModuleId)
                End If
                Return Me._GeneralSettings
            End Get
            Set(ByVal Value As GeneralSettings)
                Me._GeneralSettings = Value
                SettingsController.SetModuleSettings(Of GeneralSettings)(SettingsScope.ModuleSettings, Me.ModuleId, Value)
            End Set
        End Property

        Public Function GetTabModuleSettings(Of T As Class)() As T
            Return SettingsController.GetModuleSettings(Of T)(SettingsScope.TabModuleSettings, Me.TabModuleId)
        End Function

        Public Sub SetTabModuleSettings(Of T As Class)(value As T)
            SettingsController.SetModuleSettings(Of T)(SettingsScope.TabModuleSettings, Me.TabModuleId, value)
        End Sub

        Public Function GetModuleSettings(Of T As Class)() As T
            Return SettingsController.GetModuleSettings(Of T)(SettingsScope.ModuleSettings, Me.ModuleId)
        End Function

        Public Sub SetModuleSettings(Of T As Class)(value As T)
            SettingsController.SetModuleSettings(Of T)(SettingsScope.ModuleSettings, Me.ModuleId, value)
        End Sub


        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ParentViewstate(ByVal key As String) As Object
            Get
                Return Me.ViewState(key)
            End Get
            Set(ByVal value As Object)
                Me.ViewState(key) = value
            End Set
        End Property


        Private _IsAdmin As Nullable(Of Boolean)

        Public ReadOnly Property IsAdmin() As Boolean
            Get
                If Not _IsAdmin.HasValue Then
                    _IsAdmin = Me.UserInfo.IsInRole(Me.PortalSettings.AdministratorRoleName) OrElse Me.UserInfo.IsSuperUser
                End If
                Return _IsAdmin.Value
            End Get
        End Property


#End Region


        Public ReadOnly Property SharedResourceFile() As String
            Get

                Return Me.TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/" & _
                    Localization.LocalSharedResourceFile

            End Get
        End Property

        Private _ParentSkin As DotNetNuke.UI.Skins.Skin

        Public ReadOnly Property ParentSkin As DotNetNuke.UI.Skins.Skin
            Get
                If _ParentSkin Is Nothing Then
                    _ParentSkin = DotNetNuke.UI.Skins.Skin.GetParentSkin(Me)
                End If
                Return _ParentSkin
            End Get
        End Property

#End Region

#Region "Public Methods"


        'Public Function GetSharedResource(ByVal key As String) As String

        '    Dim path As String = Me.TemplateSourceDirectory & "/" & DotNetNuke.Services.Localization.Localization.LocalResourceDirectory & "/" & DotNetNuke.Services.Localization.Localization.LocalSharedResourceFile

        '    path = "~" & path.Substring(path.IndexOf("/DesktopModules/"), path.Length - path.IndexOf("/DesktopModules/"))

        '    Return DotNetNuke.Services.Localization.Localization.GetString(key, path)

        'End Function


        Public Sub SavePersonalSettings()
            SetPersonalSettings(Me.PortalId, Me.PersonalSettings, True)
        End Sub

        Public Sub AddConfirm(ctl As WebControl)
            Dim key As String = ctl.ID & ".Confirm"
            Dim message As String = Localization.GetString(key, Me.LocalResourceFile)
            If message = "" Then
                message = key
            End If
            ClientAPI.AddButtonConfirm(ctl, message)
        End Sub

        <Obsolete("Use DnnContext instead")> _
        Public Sub AddPageMessage(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            DotNetNuke.UI.Skins.Skin.AddPageMessage(Me.ParentSkin, heading, strMessage, messageType)
        End Sub

        <Obsolete("Use DnnContext instead")> _
        Public Sub AddModuleMessage(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, heading, strMessage, messageType)
        End Sub

        <Obsolete("Use DnnContext instead")> _
        Public Function GetModuleMessageControl(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "") As ModuleMessage
            Return DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(heading, strMessage, messageType)
        End Function



#End Region

#Region "Protected Methods"


        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            Me.CheckInstall()
            Me.ParseRequest()
            AjaxifyClientVariable()
            DnnContext.Current.SetModule(Me)
            AddHandler Me.Page.PreRenderComplete, AddressOf PagePreRenderComplete
        End Sub

        Private Sub PagePreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent PreRenderComplete(Me, e)
            RaiseEvent AfterPreRenderComplete(Me, e)
        End Sub

        Private Sub AjaxifyClientVariable()
          
            If DotNetNuke.Framework.AJAX.IsEnabled AndAlso ModuleConfiguration IsNot Nothing AndAlso ModuleConfiguration.SupportsPartialRendering Then
                If Me.Context.Items("AjaxifyClientVariable") Is Nothing Then 'AndAlso Not Me.IsPostBack 
                    Dim clientVarControl As HtmlInputHidden = DotNetNuke.UI.Utilities.ClientAPI.RegisterDNNVariableControl(Me)
                    Dim updatePanel As Control = DotNetNuke.Framework.AJAX.WrapUpdatePanelControl(clientVarControl, False)
                    clientVarControl.EnableViewState = False
                    Dim updatePanelType As Type = DotNetNuke.Framework.Reflection.CreateType("System.Web.UI.UpdatePanel")
                    Dim updatePanelModeType As Type = DotNetNuke.Framework.Reflection.CreateType("System.Web.UI.UpdatePanelUpdateMode")
                    Dim updateProp As PropertyInfo = Aricie.Services.ReflectionHelper.GetPropertiesDictionary(updatePanelType)("UpdateMode")
                    'Reflection.SetProperty(updatePanelType, "UpdateMode", updatePanel, New Object() {System.Enum.Parse(updatePanelModeType, "0")})
                    updateProp.SetValue(updatePanel, System.Enum.Parse(updatePanelModeType, "0"), Nothing)
                    'AddHandler clientVarControl.Load, AddressOf ClientVarControl_Load
                    DotNetNuke.UI.Utilities.DNNClientAPI.AddBodyOnloadEventHandler(Me.Page, "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args){dnn.vars=null;});")
                    Me.Context.Items("AjaxifyClientVariable") = True
                End If
            End If
        End Sub

        'Private Sub ClientVarControl_Load(ByVal sender As Object, ByVal e As EventArgs)
        '    Throw New NotImplementedException()
        'End Sub

        Protected Overridable Sub ParseRequest()


        End Sub

        Protected Overridable Sub CheckInstall()

        End Sub

        Public Event PreRenderComplete As EventHandler
        Public Event AfterPreRenderComplete As EventHandler


#End Region
    End Class
End Namespace


