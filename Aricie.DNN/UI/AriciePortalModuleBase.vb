Imports System.ComponentModel
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
Imports Aricie.Collections
Imports DotNetNuke.UI.Utilities
Imports Aricie.Services
Imports System.Globalization


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
            End Set
        End Property



        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ParentViewstate(ByVal key As String) As Object
            Get
                Return Me.ViewState(key)
            End Get
            Set(ByVal value As Object)
                Me.ViewState(key) = value
            End Set
        End Property


        Private Const AdvancedVarKey As String = "AdvVar"

        Private _AdvancedClientVars As SerializableDictionary(Of String, String)

        Private ReadOnly Property AdvancedClientVars() As SerializableDictionary(Of String, String)
            Get
                'Dim toReturn As SerializableDictionary(Of String, String) = DnnContext.Current.GetItem(Of SerializableDictionary(Of String, String))(AdvancedVarKey)
                If _AdvancedClientVars Is Nothing Then
                    Dim dnnclientVar As String = ClientAPI.GetClientVariable(Me.Page, AdvancedVarKey)
                    If String.IsNullOrEmpty(dnnclientVar) Then
                        _AdvancedClientVars = New SerializableDictionary(Of String, String)
                    Else
                        _AdvancedClientVars = ReflectionHelper.Deserialize(Of SerializableDictionary(Of String, String))(dnnclientVar)
                    End If
                    'DnnContext.Current.SetItem(Of SerializableDictionary(Of String, String))(toReturn, AdvancedVarKey)
                    AddHandler Me.AfterPreRenderComplete, AddressOf SerializeAdvancedClientVars
                End If
                Return _AdvancedClientVars
            End Get
        End Property

        Private Sub SerializeAdvancedClientVars(ByVal sender As Object, ByVal e As EventArgs)
            Dim dnnclientVar As String = ReflectionHelper.Serialize(Me.AdvancedClientVars).OuterXml
            ClientAPI.RegisterClientVariable(Me.Page, AdvancedVarKey, dnnclientVar, True)
        End Sub

        Public Property AdvancedClientVariable(ByVal objControl As Control, ByVal localKey As String) As String
            Get
                Dim varKey As String = localKey & objControl.ID & objControl.ClientID.GetHashCode
                Dim toReturn As String = Nothing
                If Me.AdvancedClientVars.TryGetValue(varKey, toReturn) Then
                    Return toReturn
                Else
                    Return ""
                End If
                'Dim toReturn As String = ClientAPI.GetClientVariable(objControl.Page, varKey)
                'If String.IsNullOrEmpty(toReturn) Then
                '    Dim parentModule As AriciePortalModuleBase = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePortalModuleBase)(objControl)
                '    If parentModule IsNot Nothing Then
                '        toReturn = DirectCast(parentModule.ParentViewstate(varKey), String)
                '    End If
                'End If
                'Return toReturn
            End Get
            Set(ByVal value As String)
                Dim varKey As String = localKey & objControl.ID & objControl.ClientID.GetHashCode
                Me.AdvancedClientVars(varKey) = value
                'ClientAPI.RegisterClientVariable(objControl.Page, varKey, value, True)
                'Dim parentModule As AriciePortalModuleBase = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePortalModuleBase)(objControl)
                'If parentModule IsNot Nothing Then
                '    parentModule.ParentViewstate(varKey) = value
                'End If
            End Set
        End Property

        Public Property AdvancedCounter(ByVal objControl As Control, ByVal localKey As String) As Integer
            Get
                Dim toReturn As Integer = 0
                Dim strExisting As String = AdvancedClientVariable(objControl, localKey)
                If Not String.IsNullOrEmpty(strExisting) Then
                    toReturn = Integer.Parse(strExisting, CultureInfo.InvariantCulture)
                End If
                Return toReturn
            End Get
            Set(ByVal value As Integer)
                AdvancedClientVariable(objControl, localKey) = value.ToString(CultureInfo.InvariantCulture)
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


