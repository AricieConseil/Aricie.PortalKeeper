Imports System.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Tabs
Imports Aricie.DNN.Entities
Imports DotNetNuke.Entities.Modules
Imports System.Web
Imports System.Globalization
Imports System.Web.SessionState
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Framework
Imports Aricie.Services
Imports System.Threading
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.UI.WebControls
Imports System.Net
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Utilities
Imports System.Web.UI
Imports Aricie.Collections
Imports DotNetNuke.UI.Utilities
Imports System.Net.NetworkInformation
Imports System.Web.Script.Serialization

Namespace Services
    ''' <summary>
    ''' Class that regroups some information about the DotNetNuke instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DnnContext
        Inherits ContextBase(Of DnnContext)

        Public Event Debug As EventHandler(Of DebugEventArgs)

        Public Sub OnDebug(sender As Object, e As DebugEventArgs)
            RaiseEvent Debug(sender, e)
        End Sub

        Public Sub OnDebug()
            RaiseEvent Debug(Me, New DebugEventArgs())
        End Sub

        Private Const CONTEXT_KEY As String = "DnnContext"
        Private Shared _DefaultContext As DnnContext
        Private Shared lock As New Object

        Private _CurrentTabs As Dictionary(Of Integer, TabInfo)
        Private _AdvancedTokenReplace As AdvancedTokenReplace
        Private _Administrator As UserInfo
        Private _MetaData As MetaDataInfo
        Private _ModuleForms As New Dictionary(Of Integer, PortalModuleBase)
        Private _CurrentModuleId As Integer = Null.NullInteger



        'Private _Items As New Dictionary(Of String, Object)
        Private _HttpContext As HttpContext
        Private _IPAddress As IPAddress
        Private _Culture As CultureInfo
        Private Shared _CountryLookup As CountryLookup

        Public Sub New()

        End Sub

        Public Sub New(ByVal context As HttpContext)
            _HttpContext = context
        End Sub

        ''' <summary>
        ''' returns the current DnnContext from the current HttpContext
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetInstance() As DnnContext
            Return Current
        End Function

        ''' <summary>
        ''' returns the current DnnContext from the current HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Shared ReadOnly Property Current() As DnnContext
            Get
                Return Current(HttpContext.Current)
            End Get
        End Property

        ''' <summary>
        ''' returns the current DnnContext from the HttpContext passd as parameters
        ''' </summary>
        ''' <param name="context"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Shared ReadOnly Property Current(ByVal context As HttpContext) As DnnContext
            Get
                Dim toReturn As DnnContext
                If context IsNot Nothing Then
                    toReturn = DirectCast(context.Items(CONTEXT_KEY), DnnContext)
                    If toReturn Is Nothing Then
                        toReturn = New DnnContext(context)
                        context.Items(CONTEXT_KEY) = toReturn
                    End If
                Else
                    toReturn = ReflectionHelper.GetSingleton(Of DnnContext)()
                End If
                Return toReturn
            End Get
        End Property


        ''' <summary>
        ''' Returns authentication information from the HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsAuthenticated() As Boolean
            Get
                Return Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing AndAlso Me.HttpContext.Request.IsAuthenticated
            End Get
        End Property

        ''' <summary>
        ''' Returns session information from the HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Session() As HttpSessionState
            Get
                If Me.HttpContext IsNot Nothing Then
                    Return Me.HttpContext.Session
                End If
                Return Nothing
            End Get
        End Property




        ''' <summary>
        ''' Returns the current HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HttpContext() As HttpContext
            Get
                Return _HttpContext
            End Get
        End Property

        ''' <summary>
        '''  Returns request information from the HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Request() As HttpRequest
            Get
                If _HttpContext IsNot Nothing Then
                    Return _HttpContext.Request
                End If
                Return Nothing
            End Get
        End Property


        Private _AbsoluteUri As String = String.Empty

        Public ReadOnly Property AbsoluteUri As String
            Get
                If _AbsoluteUri.IsNullOrEmpty AndAlso Request IsNot Nothing Then
                    _AbsoluteUri = Request.Url.AbsoluteUri
                End If
                Return _AbsoluteUri
            End Get
        End Property


        ''' <summary>
        '''  Returns response information from the HttpContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Response() As HttpResponse
            Get
                If _HttpContext IsNot Nothing Then
                    Return _HttpContext.Response
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        '''  Returns culture information from the HttpContext, failing that from the thread current culture
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Culture() As CultureInfo
            Get
                If _Culture Is Nothing Then
                    If Me._HttpContext IsNot Nothing Then
                        Dim strLocale As String = Me._HttpContext.Request.Item("language")
                        If Not String.IsNullOrEmpty(strLocale) Then
                            _Culture = New CultureInfo(strLocale)
                        Else
                            Dim cookie As HttpCookie = Me._HttpContext.Response.Cookies.Get("language")
                            If (cookie IsNot Nothing AndAlso Not String.IsNullOrEmpty(cookie.Value)) Then
                                _Culture = New CultureInfo(cookie.Value)
                            End If
                        End If
                    End If
                    If _Culture Is Nothing Then
                        _Culture = Thread.CurrentThread.CurrentCulture
                    End If
                End If
                Return _Culture
            End Get
        End Property

        ''' <summary>
        ''' Returns the administrator id for this portal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Administrator() As UserInfo
            Get
                If _Administrator Is Nothing Then
                    _Administrator = UserController.GetUser(Me.Portal.PortalId, Me.Portal.AdministratorId, True)
                End If
                Return _Administrator
            End Get
        End Property

        ''' <summary>
        '''  Returns DotNetNuke current portal settings
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Portal() As PortalSettings
            Get
                Return NukeHelper.PortalSettings
            End Get
        End Property

        Private _PortalAlias As PortalAliasInfo
        Public ReadOnly Property PortalAlias As PortalAliasInfo
            Get
                If _PortalAlias Is Nothing Then
                    _PortalAlias = Me.Portal.PortalAlias
                    If _PortalAlias.HTTPAlias.IsNullOrEmpty() AndAlso Me.Request IsNot Nothing Then
                        'Dim objDomainName As String = DotNetNuke.Common.Globals.GetDomainName(Me.Request)
                        Dim objDomainName As String = Me.Request.Url.Authority & Me.Request.ApplicationPath
                        _PortalAlias = NukeHelper.PortalAliasController.GetPortalAlias(objDomainName, Me.Portal.PortalId)
                    End If
                End If
                Return _PortalAlias
            End Get
        End Property


        ''' <summary>
        ''' Returns DotNetNuke host settings
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HostSettings() As Hashtable
            Get
                Return DotNetNuke.Common.Globals.HostSettings
            End Get
        End Property

        ''' <summary>
        ''' Returns server name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ServerName() As String
            Get
                Return DotNetNuke.Common.Globals.ServerName
            End Get
        End Property



        Private _Page As Page

        ''' <summary>
        ''' Returns the current page
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Page() As Page
            Get
                If _Page Is Nothing Then
                    If Me.HttpContext IsNot Nothing AndAlso TypeOf Me.HttpContext.CurrentHandler Is Page Then
                        _Page = DirectCast(Me.HttpContext.CurrentHandler, Page)
                    End If
                End If
                Return _Page
            End Get
        End Property


        ''' <summary>
        ''' Returns the current page
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DnnPage() As CDefault
            Get
                If Page IsNot Nothing AndAlso TypeOf Page Is CDefault Then
                    Return DirectCast(Page, CDefault)
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Returns desktopTabs for the current portal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentTabs() As Dictionary(Of Integer, TabInfo)
            Get
                If _CurrentTabs Is Nothing Then
                    If PortalSettings IsNot Nothing Then
                        _CurrentTabs = New Dictionary(Of Integer, TabInfo)
                        For Each objTab As TabInfo In PortalSettings.DesktopTabs
                            _CurrentTabs(objTab.TabID) = objTab
                        Next
                    Else
                        Return New Dictionary(Of Integer, TabInfo)
                    End If
                End If
                Return _CurrentTabs
            End Get
        End Property

        ''' <summary>
        ''' Returns the current User
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property User() As UserInfo
            Get
                Return NukeHelper.User
            End Get
        End Property


        Private _CurrentSkin As DotNetNuke.UI.Skins.Skin

        Public ReadOnly Property CurrentSkin As DotNetNuke.UI.Skins.Skin
            Get
                If _CurrentSkin Is Nothing Then
                    If Me.DnnPage IsNot Nothing Then
                        Dim skinList As IList(Of DotNetNuke.UI.Skins.Skin) = Aricie.Web.UI.ControlHelper.FindControlsRecursive(Of DotNetNuke.UI.Skins.Skin)(Me.DnnPage)
                        If skinList.Count > 0 Then
                            _CurrentSkin = skinList(0)
                        End If
                    End If
                End If
                Return _CurrentSkin
            End Get
        End Property


        ''' <summary>
        ''' Returns the current module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentModule As PortalModuleBase
            Get
                Dim toReturn As PortalModuleBase = Nothing
                If Me._CurrentModuleId <> Null.NullInteger Then
                    Me._ModuleForms.TryGetValue(Me._CurrentModuleId, toReturn)
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' returns the module information from its id
        ''' </summary>
        ''' <param name="mid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property [ModuleForm](ByVal mid As Integer) As PortalModuleBase
            Get
                If _ModuleForms.ContainsKey(mid) Then
                    Return _ModuleForms(mid)
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Collection of mapping between id and module infos
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ModuleForms() As Dictionary(Of Integer, PortalModuleBase)
            Get
                Return _ModuleForms
            End Get
        End Property



      

        ''' <summary>
        ''' Gets or sets the AdvancedTokenReplace
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AdvancedTokenReplace() As AdvancedTokenReplace
            Get
                Return _AdvancedTokenReplace
            End Get
            Set(ByVal value As AdvancedTokenReplace)
                _AdvancedTokenReplace = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Metadata
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MetaData() As MetaDataInfo
            Get
                If _MetaData Is Nothing Then
                    Me.FetchMetaData()
                End If
                Return _MetaData
            End Get
            Set(ByVal value As MetaDataInfo)
                _MetaData = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the requesting IPAdress
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IPAddress() As IPAddress
            Get
                If _IPAddress Is Nothing Then
                    'todo: beware of the racing condition here (the request could be disposed during the parsing)
                    If Me.Request IsNot Nothing AndAlso Not (Me.HttpContext.CurrentNotification >= RequestNotification.EndRequest AndAlso Me.HttpContext.IsPostNotification) Then
                        IPAddress.TryParse(Common.GetExternalIPAddress(Me.Request), _IPAddress)
                    Else
                        For Each netif As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
                            Dim properties As IPInterfaceProperties = netif.GetIPProperties()
                            For Each dns As IPAddress In properties.DnsAddresses
                                _IPAddress = dns
                                Return dns
                            Next
                        Next
                    End If
                End If
                Return _IPAddress
            End Get
        End Property

        ''' <summary>
        ''' returns the requesting country name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CountryName() As String
            Get
                Dim toReturn As String = ""
                Dim ip As IPAddress = Me.IPAddress
                If ip IsNot Nothing Then
                    toReturn = CountryLookup.LookupCountryName(ip)
                Else
                    If Me.Request IsNot Nothing AndAlso Not String.IsNullOrEmpty(Me.Request.UserHostAddress) Then
                        toReturn = CountryLookup.LookupCountryName(Me.Request.UserHostAddress)
                    End If
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' Returns the lookup data for IP localization
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CountryLookup() As CountryLookup
            Get
                If _CountryLookup Is Nothing Then
                    Dim path As String = "~/controls/CountryListBox/Data/GeoIP.dat"
                    _CountryLookup = New CountryLookup(HttpContext.Current.Server.MapPath(path))
                End If
                Return _CountryLookup
            End Get
        End Property

        Public Property AdvancedClientVariable(ByVal localKey As String) As String
            Get
                Return AdvancedClientVariable(Nothing, localKey)
            End Get
            Set(ByVal value As String)
                Me.AdvancedClientVariable(Nothing, localKey) = value
            End Set
        End Property

        Public Property AdvancedClientVariable(ByVal objControl As Control, ByVal localKey As String) As String
            Get
                Dim varKey As String = localKey
                If objControl IsNot Nothing Then
                    varKey &= objControl.ClientID & objControl.ClientID.GetHashCode
                End If
                Dim toReturn As String = Nothing
                If Me.AdvancedClientVars.TryGetValue(varKey, toReturn) Then
                    Return toReturn
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                Dim varKey As String = localKey
                If objControl IsNot Nothing Then
                    varKey &= objControl.ClientID & objControl.ClientID.GetHashCode
                End If
                Me.AdvancedClientVars(varKey) = value
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



        ''' <summary>
        ''' Creates a service
        ''' </summary>
        ''' <param name="serviceType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function CreateService(ByVal serviceType As System.Type) As Object
            Dim toReturn As Object = MyBase.CreateService(serviceType)
            If TypeOf toReturn Is IModuleContext Then
                DirectCast(toReturn, IModuleContext).DnnContext = Me
            End If
            Return toReturn
        End Function







#Region "methods"




        Public Sub FetchMetaData()
            Me._MetaData = New MetaDataInfo()
            If Me.DnnPage IsNot Nothing Then
                Me._MetaData.Title = Me.DnnPage.Title
                Me._MetaData.Description = Me.DnnPage.Description
                Me._MetaData.KeyWords = Me.DnnPage.KeyWords
            End If
        End Sub

        Public Sub SetMetaData()
            If Me.DnnPage IsNot Nothing Then
                If Not String.IsNullOrEmpty(Me._MetaData.Title) Then
                    Me.DnnPage.Title = Me._MetaData.Title
                End If
                If Not String.IsNullOrEmpty(Me._MetaData.Description) Then
                    Me.DnnPage.Description = Me._MetaData.Description
                End If
                If Not String.IsNullOrEmpty(Me._MetaData.KeyWords) Then
                    If Me._MetaData.OverrideKeywords Or Me.DnnPage.KeyWords = "" Then
                        Me.DnnPage.KeyWords = Me._MetaData.KeyWords
                    Else
                        Me.DnnPage.KeyWords &= ", " & Me._MetaData.KeyWords
                    End If
                End If
                If Not String.IsNullOrEmpty(Me._MetaData.MetaTitle) Then
                    Dim meta As New HtmlMeta()
                    meta.Name = "Title"
                    meta.Content = Me._MetaData.MetaTitle
                    Me.DnnPage.Header.Controls.Add(meta)
                End If
            End If
        End Sub

        Public Sub SetModule(ByVal value As PortalModuleBase)
            Me._ModuleForms(value.ModuleId) = value
            Me._CurrentModuleId = value.ModuleId
        End Sub

        Public Sub AddPageMessage(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            DotNetNuke.UI.Skins.Skin.AddPageMessage(Me.CurrentSkin, heading, strMessage, messageType)
        End Sub

        Public Sub AddModuleMessage(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            If Me.CurrentModule IsNot Nothing Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me.CurrentModule, heading, strMessage, messageType)
            Else
                Me.AddPageMessage(strMessage, messageType, heading)
            End If
        End Sub

        Public Function GetModuleMessageControl(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "") As ModuleMessage
            Return DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(heading, strMessage, messageType)
        End Function


        Public Function IsModuleVisible(ByVal portalId As Integer, ByVal tabid As Integer, ByVal mid As Integer) _
           As Boolean


            Dim toReturn As Boolean = False

            'Dim currentUser As UserInfo = UserController.GetCurrentUserInfo
            Dim setNewMap As Boolean = False
            Dim visibilityMap As Dictionary(Of Integer, Object) = CacheHelper.GetPersonal(Of Dictionary(Of Integer, Object))(Me.User.UserID, "VisibilityMap")
            If visibilityMap Is Nothing Then
                visibilityMap = New Dictionary(Of Integer, Object)
                setNewMap = True
            End If
            If Not visibilityMap.ContainsKey(tabid) Then
                Dim objTabInfo As TabInfo = NukeHelper.TabController.GetTab(tabid, portalId, False)
                If Not PortalSecurity.IsInRoles(objTabInfo.AuthorizedRoles) Then
                    visibilityMap(tabid) = False
                Else
                    visibilityMap(tabid) = New Dictionary(Of Integer, Boolean)
                End If
                setNewMap = True
            End If
            If TypeOf visibilityMap(tabid) Is Boolean Then
                toReturn = CType(visibilityMap(tabid), Boolean)
            Else
                Dim _
                    moduleVisib As Dictionary(Of Integer, Boolean) = _
                        DirectCast(visibilityMap(tabid), Dictionary(Of Integer, Boolean))
                If Not moduleVisib.ContainsKey(mid) Then
                    Dim objModuleInfo As ModuleInfo = NukeHelper.ModuleController.GetModule(mid, tabid, False)
                    'moduleVisib(mid) = objModuleInfo.InheritViewPermissions OrElse PortalSecurity.IsInRoles(objModuleInfo.AuthorizedRoles)
                    Dim namespaceModuleConf As String = "DotNetNuke.Entities.Modules.ModuleInfo"

                    Dim dnnVersion As Version = ReflectionHelper.CreateType(namespaceModuleConf).Assembly.GetName.Version
                    Dim modProperties As Dictionary(Of String, System.Reflection.PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(Of ModuleInfo)()
                    If dnnVersion.Major >= 5 Then
                        If dnnVersion.Minor < 1 Then
                            moduleVisib(mid) = objModuleInfo.InheritViewPermissions OrElse PortalSecurity.IsInRoles(Convert.ToString(modProperties("Permissions").GetValue(objModuleInfo, Nothing)))
                        Else
                            moduleVisib(mid) = objModuleInfo.InheritViewPermissions OrElse PortalSecurity.IsInRoles(Convert.ToString(modProperties("AuthorizedViewRoles").GetValue(objModuleInfo, Nothing)))
                        End If
                    Else
                        moduleVisib(mid) = objModuleInfo.InheritViewPermissions OrElse PortalSecurity.IsInRoles(Convert.ToString(modProperties("AuthorizedRoles").GetValue(objModuleInfo, Nothing)))
                    End If
                    setNewMap = True
                End If
                toReturn = moduleVisib(mid)
            End If


            If setNewMap Then

                CacheHelper.SetPersonal(Of Dictionary(Of Integer, Object))(visibilityMap, Me.User.UserID, "VisibilityMap")

            End If

            Return toReturn

        End Function



#End Region

#Region "Private members"

        Private Const AdvancedVarKey As String = "AdvVar"

        Private _AdvancedClientVars As SerializableDictionary(Of String, String)

        Private Shared _JavascriptSerializer As JavaScriptSerializer

        Public Shared ReadOnly Property JavascriptSerializer As JavaScriptSerializer
            Get
                If _JavascriptSerializer Is Nothing Then
                    _JavascriptSerializer = New JavaScriptSerializer()
                End If
                Return _JavascriptSerializer
            End Get
        End Property

        Private ReadOnly Property AdvancedClientVars() As SerializableDictionary(Of String, String)
            Get
                If _AdvancedClientVars Is Nothing Then
                    Dim dnnclientVar As String = ClientAPI.GetClientVariable(Me.DnnPage, AdvancedVarKey)
                    If (String.IsNullOrEmpty(dnnclientVar)) Then
                        'HACK the dnn process to get the dnnvariable (deserialisation pb) - check the HTTP Context
                        Dim dnnVariable As String = Me.HttpContext.Request("__dnnVariable")
                        If (Not String.IsNullOrEmpty(dnnVariable)) Then
                            If (dnnVariable.StartsWith("`")) Then
                                dnnVariable = dnnVariable.Remove(0, 1)
                            End If
                            Dim dnnVariableObj As Dictionary(Of String, String) = JavascriptSerializer.Deserialize(Of Dictionary(Of String, String))(dnnVariable.Replace("`", "'"))
                            If dnnVariableObj.ContainsKey(AdvancedVarKey) Then
                                dnnclientVar = CStr(dnnVariableObj(AdvancedVarKey))
                            End If
                        End If
                    End If
                    If String.IsNullOrEmpty(dnnclientVar) Then
                        _AdvancedClientVars = New SerializableDictionary(Of String, String)
                    Else
                        _AdvancedClientVars = JavascriptSerializer.Deserialize(Of SerializableDictionary(Of String, String))(dnnclientVar)
                    End If
                    AddHandler Me.DnnPage.PreRenderComplete, AddressOf SerializeAdvancedClientVars
                End If
                Return _AdvancedClientVars
            End Get
        End Property

        Private Sub SerializeAdvancedClientVars(ByVal sender As Object, ByVal e As EventArgs)
            Dim dnnclientVar As String = JavascriptSerializer.Serialize(Me.AdvancedClientVars)
            ClientAPI.RegisterClientVariable(Me.DnnPage, AdvancedVarKey, dnnclientVar, True)
        End Sub


#End Region

    End Class

End Namespace


