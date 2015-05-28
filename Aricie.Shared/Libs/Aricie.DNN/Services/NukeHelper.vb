Imports System.Xml
Imports Aricie.Security.Cryptography
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework.Providers
Imports System.Globalization
Imports Aricie.Services
Imports System.Reflection
Imports DotNetNuke.Security
Imports System.Web
Imports System.Threading

'-------------------------------------------------------------------------------
' 28/03/2011 - [JBB] - Modification du trim pour ne supprimer les \ qu'à la fin et non pas non plus au début (chemin réseau)
'-------------------------------------------------------------------------------
Imports DotNetNuke.Services.Personalization
Imports DotNetNuke.Entities.Profile
Imports System.Security
Imports System.Text
Imports DotNetNuke.Common.Lists

Namespace Services
    Public Module NukeHelper

        Private _WebServerPath As New Dictionary(Of String, String)
        Private _DbParameterAreSet As Boolean
        Private _DnnConnectionString As String
        Private _DnnObjectQualifier As String
        Private _DnnDatabaseOwner As String
        Private _DnnVersion As Version


        Private _TabIdByMid As New Dictionary(Of Integer, Integer)
        Private _PortalIdByMid As New Dictionary(Of Integer, Integer)
        Private _ModuleNameByMid As New Dictionary(Of Integer, String)
        Private _FriendlyModuleNameByMid As New Dictionary(Of Integer, String)

        Private _ModuleDefIdByModuleName As New Dictionary(Of String, Integer)
        Private _DesktopModulesByModuleDefId As New Dictionary(Of Integer, DesktopModuleInfo)
        Private _DesktopModulesLock As New Object


        Private _PidsLock As New Object
        Private _PortalIds As List(Of Integer)

        Private _PaliasLock As New Object
        Private _PortalAliasesByPortalId As Dictionary(Of Integer, List(Of PortalAliasInfo))
        Private _PortalAliasByPortalAliasId As Dictionary(Of Integer, PortalAliasInfo)
        Private _PortalIdByPortalAlias As Dictionary(Of String, Integer)
        Private _PortalController As PortalController

#Region "instanciated controllers"

        Public ReadOnly HostController As New HostSettingsController()
        Public ReadOnly Property PortalController As PortalController
            Get
                If _PortalController Is Nothing Then
                    SyncLock _PidsLock
                        If _PortalController Is Nothing Then
                            _PortalController = New PortalController()
                        End If
                    End SyncLock
                End If
                Return _PortalController
            End Get
        End Property

        Public ReadOnly PortalAliasController As New PortalAliasController()
        Public ReadOnly ModuleController As New ModuleController()
        Private _RoleController As RoleController

        Public ReadOnly ListController As New ListController()
        Public ReadOnly FileController As New FileController()
        Public ReadOnly FolderController As New FolderController()

        Public ReadOnly TabController As New TabController()
        Public ReadOnly LogController As New EventLogController()

        Public ReadOnly DesktopModuleController As New DesktopModuleController()
        Public ReadOnly ModuleDefinitionController As New ModuleDefinitionController()
        Public ReadOnly PersonnalizationController As New PersonalizationController()
        Public ReadOnly ProfileController As New ProfileController()

#End Region

        ''' <summary>
        ''' Gets current RoleController
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RoleController As RoleController
            Get
                If _RoleController Is Nothing Then
                    _RoleController = New RoleController()
                End If

                Return _RoleController
            End Get
        End Property

#Region "web.config"





        ''' <summary>
        ''' Gets current web.config
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property WebConfigDocument() As XmlDocument
            Get
                Dim strFileName As String = NukeHelper.ApplicationMapPath & "\web.config"
                Dim toReturn As XmlDocument = CacheHelper.GetGlobal(Of XmlDocument)(strFileName)
                If toReturn Is Nothing Then
                    'toReturn = Config.Load()
                    toReturn = New XmlDocument
                    toReturn.Load(strFileName)
                    CacheHelper.SetCacheDependant(Of XmlDocument)(toReturn, strFileName, Aricie.Constants.Cache.GlobalExpiration, strFileName)
                End If
                Return toReturn
            End Get
        End Property


        Private _DecryptionKey As SecureString

        Public ReadOnly Property DecryptionKey As SecureString
            Get
                If _DecryptionKey Is Nothing Then
                    _DecryptionKey = New SecureString()
                    Try
                        _DecryptionKey = NukeHelper.WebConfigDocument.SelectSingleNode("configuration/system.web/machineKey").Attributes("decryptionKey").Value.WriteSecureString(True)
                    Catch ex As Exception
                        'Dim objMachine As MachineKeySection = New MachineKeySection

                        _DecryptionKey = Globals.GetHostPortalSettings().GUID.ToByteArray().WriteSecureString(True, Encoding.UTF8, True)
                    End Try
                End If
                Return _DecryptionKey
            End Get
        End Property




        ''' <summary>
        ''' Gets current connection string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DnnConnectionString() As String
            Get
                If Not _DbParameterAreSet Then
                    SetDbParameters()
                End If
                Return _DnnConnectionString
            End Get
        End Property

        ''' <summary>
        ''' Gets current Object qualifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DnnObjectQualifier() As String
            Get
                If Not _DbParameterAreSet Then
                    SetDbParameters()
                End If
                Return _DnnObjectQualifier
            End Get
        End Property

        ''' <summary>
        ''' Gets current database owner
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DnnDatabaseOwner() As String
            Get
                If Not _DbParameterAreSet Then
                    SetDbParameters()
                End If
                Return _DnnDatabaseOwner
            End Get
        End Property

        ''' <summary>
        ''' Initialize database information
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetDbParameters()

            Dim providerConfiguration As ProviderConfiguration = providerConfiguration.GetProviderConfiguration("data")
            Dim provider As Provider = DirectCast(providerConfiguration.Providers.Item(providerConfiguration.DefaultProvider), Provider)
            _DnnConnectionString = Config.GetConnectionString
            If (_DnnConnectionString = "") Then
                _DnnConnectionString = provider.Attributes.Item("connectionString")
            End If
            _DnnObjectQualifier = provider.Attributes.Item("objectQualifier")
            If ((_DnnObjectQualifier <> "") And Not _DnnObjectQualifier.EndsWith("_")) Then
                _DnnObjectQualifier = (_DnnObjectQualifier & "_")
            End If
            _DnnDatabaseOwner = provider.Attributes.Item("databaseOwner")

            _DbParameterAreSet = True

        End Sub

        ''' <summary>
        ''' Gets node value from web.config file
        ''' </summary>
        ''' <param name="elementName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DefineWebServerElementPath(ByVal elementName As String) As String

            Dim toReturn As String = ""
            Dim strTemp As String = ""
            If Not _WebServerPath.TryGetValue(elementName, toReturn) Then
                SyncLock _WebServerPath
                    If Not _WebServerPath.TryGetValue(elementName, toReturn) Then
                        Dim webServerNode As XmlNode
                        webServerNode = WebConfigDocument.SelectSingleNode("//" & elementName)

                        If webServerNode IsNot Nothing Then
                            Dim node As XmlNode = webServerNode
                            While node.NodeType = XmlNodeType.Element
                                If toReturn = "" Then
                                    toReturn = "/" & node.LocalName
                                Else
                                    toReturn = "/" & node.LocalName & toReturn
                                End If
                                node = node.ParentNode
                            End While

                            _WebServerPath.Add(elementName, toReturn)
                        End If
                    End If
                End SyncLock
            End If
            Return toReturn
        End Function

        Public Sub Restart()
            DotNetNuke.Common.Utilities.Config.Touch()
        End Sub

#End Region


#Region "User related members"

        ''' <summary>
        ''' Gets current user info
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property User() As UserInfo
            Get
                Return UserController.GetCurrentUserInfo()
            End Get
        End Property




#End Region

#Region "Instance members"

        ''' <summary>
        ''' Returns DotNetNuke version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DnnVersion() As Version
            Get
                If _DnnVersion Is Nothing Then
                    _DnnVersion = New Version(GetDatabaseVersion("."))
                End If
                Return _DnnVersion

            End Get
        End Property

#End Region


#Region "Portal members"

        ''' <summary>
        ''' gets current portal id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalId() As Integer
            Get
                Return PortalSettings.PortalId
            End Get
        End Property

        ''' <summary>
        ''' Gets a list of portal ids on the server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalIds() As List(Of Integer)
            Get
                If _PortalIds Is Nothing Then
                    SyncLock _PidsLock
                        If _PortalIds Is Nothing Then
                            Dim tempList As New List(Of Integer)
                            Dim pc As New PortalController
                            For Each objPortal As PortalInfo In PortalController.GetPortals
                                Try
                                    PortalSettings.GetSiteSettings(objPortal.PortalID)
                                    tempList.Add(objPortal.PortalID)
                                Catch ex As Exception
                                    DotNetNuke.Services.Exceptions.LogException(New ApplicationException( _
                                        "There is a ghost portal in your instance (with no page), please clean your portals table", ex))
                                End Try
                            Next
                            _PortalIds = tempList
                        End If
                    End SyncLock
                End If
                Return _PortalIds
            End Get
        End Property

        Private _PortalSettingsDebug As Boolean


        Private _HostPortalsettings As PortalSettings

        ''' <summary>
        ''' Gets current portal settings
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalSettings() As PortalSettings
            Get
                Dim toReturn As PortalSettings = Nothing
                If HttpContext.Current IsNot Nothing Then
                    toReturn = DirectCast(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                End If
                If toReturn Is Nothing Then
                    If _HostPortalsettings Is Nothing Then
                        _HostPortalsettings = Globals.GetHostPortalSettings()
                    End If
                    toReturn = _HostPortalsettings
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' Gets portal alias information by portal alias id
        ''' </summary>
        ''' <param name="portalAliasId"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalAliasByPortalAliasId(ByVal portalAliasId As Integer) As PortalAliasInfo
            Get
                If _PortalAliasByPortalAliasId Is Nothing Then
                    BuildPortalAliasDictionaries()
                End If
                Return _PortalAliasByPortalAliasId(portalAliasId)
            End Get
        End Property

        ''' <summary>
        ''' Gets portal id by portal alias
        ''' </summary>
        ''' <param name="portalAlias"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalIdByPortalAlias(ByVal portalAlias As String) As Integer
            Get
                If _PortalIdByPortalAlias Is Nothing Then
                    BuildPortalAliasDictionaries()
                End If
                Return _PortalIdByPortalAlias(portalAlias)
            End Get
        End Property

        ''' <summary>
        ''' gets list of portal alias info for one portal
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalAliasesByPortalId(ByVal portalId As Integer) As List(Of PortalAliasInfo)
            Get
                If _PortalAliasesByPortalId Is Nothing Then
                    BuildPortalAliasDictionaries()
                End If
                Return _PortalAliasesByPortalId(portalId)
            End Get
        End Property

        ''' <summary>
        ''' Gets portal info according to portal id
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PortalInfo(ByVal portalId As Integer) As PortalInfo
            Get
                Dim toReturn As PortalInfo = GetGlobal(Of PortalInfo)(portalId.ToString(CultureInfo.InvariantCulture))
                If toReturn Is Nothing Then
                    toReturn = PortalController.GetPortal(portalId)
                    SetGlobal(Of PortalInfo)(toReturn, portalId.ToString(CultureInfo.InvariantCulture))
                End If
                Return toReturn
            End Get
        End Property

        <Obsolete("Cette fonction est obsolete, elle a été remplacée par la propriété du meme nom.")> _
        Public Function GetPortalIds() As List(Of Integer)
            Return PortalIds
        End Function

        ''' <summary>
        ''' Gets portal id according to moduleid
        ''' </summary>
        ''' <param name="moduleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPortalIdByModuleId(ByVal moduleId As Integer) As Integer

            ''TODO renvoie une valeur par défaut si le moduleID est 0
            If moduleId > 0 Then
                If Not _PortalIdByMid.ContainsKey(moduleId) Then
                    Dim myModuleInfo As ModuleInfo = ModuleController.GetModule(moduleId, -1, False)
                    If myModuleInfo IsNot Nothing Then
                        _PortalIdByMid(moduleId) = myModuleInfo.PortalID
                        Return _PortalIdByMid(moduleId)
                    Else
                        Return 0
                    End If
                Else
                    Return _PortalIdByMid(moduleId)
                End If
            Else
                Return 0
            End If

        End Function

        ''' <summary>
        ''' Builds the portal alias matching dictionary
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BuildPortalAliasDictionaries()
            SyncLock _PaliasLock
                If _PortalAliasByPortalAliasId Is Nothing OrElse _PortalIdByPortalAlias Is Nothing Then

                    Dim tempPortalAliasesByPortalAliasId As New Dictionary(Of Integer, PortalAliasInfo)
                    Dim tempPortalAliasesByPortalId As New Dictionary(Of Integer, List(Of PortalAliasInfo))
                    Dim tempPortalIdByPortalAlias As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                    Dim pAliasCollec As PortalAliasCollection = PortalAliasController.GetPortalAliases()
                    For Each strPortalAlias As String In pAliasCollec.Keys
                        Dim pAliasInfo As PortalAliasInfo = pAliasCollec(strPortalAlias)
                        tempPortalAliasesByPortalAliasId(pAliasInfo.PortalAliasID) = pAliasInfo
                        tempPortalIdByPortalAlias(strPortalAlias) = pAliasInfo.PortalID
                        If Not tempPortalAliasesByPortalId.ContainsKey(pAliasInfo.PortalID) Then
                            tempPortalAliasesByPortalId(pAliasInfo.PortalID) = New List(Of PortalAliasInfo)
                        End If
                        tempPortalAliasesByPortalId(pAliasInfo.PortalID).Add(pAliasInfo)
                    Next
                    _PortalAliasByPortalAliasId = tempPortalAliasesByPortalAliasId
                    _PortalIdByPortalAlias = tempPortalIdByPortalAlias
                    _PortalAliasesByPortalId = tempPortalAliasesByPortalId
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Gets the SSL enabled value
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSSLEnabled(ByVal portalId As Integer) As Boolean
            Return PortalSettings.GetSiteSetting(portalId, "SSLEnabled") = "True"
        End Function

        ''' <summary>
        ''' Gets the SSL Enforced value
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSSLEnforced(ByVal portalId As Integer) As Boolean
            Return PortalSettings.GetSiteSetting(portalId, "SSLEnforced") = "True"
        End Function

        ''' <summary>
        ''' Refreshes the portal settings
        ''' </summary>
        ''' <param name="targetTabId"></param>
        ''' <param name="context"></param>
        ''' <remarks></remarks>
        Public Sub RenewPortalSettings(ByVal targetTabId As Integer, ByVal context As HttpContext)
            Dim domainName As String = DotNetNuke.Common.Globals.GetDomainName(context.Request, True)
            Dim portalAlias As String = PortalSettings.GetPortalByTab(targetTabId, domainName)
            If String.IsNullOrEmpty(portalAlias) Then
                portalAlias = domainName
            End If
            Dim objPortalAliasInfo As PortalAliasInfo = PortalSettings.GetPortalAliasInfo(portalAlias)

            Dim settings As New PortalSettings(targetTabId, objPortalAliasInfo)
            context.Items("PortalSettings") = settings
        End Sub

        ''' <summary>
        ''' Gets a list of users for a portal
        ''' </summary>
        ''' <param name="pid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPortalUsers(ByVal pid As Integer) As List(Of UserInfo)
            Dim toReturn As List(Of UserInfo) = CacheHelper.GetGlobal(Of List(Of UserInfo))("AllUsers", pid.ToString)
            If toReturn Is Nothing Then
                Dim tempArrayList As ArrayList = UserController.GetUsers(pid)
                toReturn = New List(Of UserInfo)(tempArrayList.Count)
                For Each objUser As UserInfo In tempArrayList
                    toReturn.Add(objUser)
                Next
                CacheHelper.SetCacheDependant(Of List(Of UserInfo))(toReturn, "", TimeSpan.FromMinutes(1), "AllUsers", pid.ToString)
            End If
            Return toReturn
        End Function

#End Region

#Region " Message methods "

        <Obsolete("Use AdvancedTokenReplace instead")> _
        Public Function PersonalizeSystemMessage(Of T As {Class})(ByVal scope As String, ByVal prefix As String, _
                                                                    ByVal objObject As Object) As String
            Dim replace As New AdvancedTokenReplace(objObject, prefix)
            Return replace.ReplaceAllTokens(scope)
        End Function

        <Obsolete("Use AdvancedTokenReplace instead")> _
        Public Function PersonalizeSystemMessage(ByVal scope As String, ByVal prefix As String, _
                                                  ByVal objObject As Object, ByVal objType As Type, _
                                                  Optional ByVal htmlEncode As Boolean = True) As String

            If Not objObject Is Nothing Then

                Dim strPropertyName As String = ""
                Dim propValue As Object
                Dim strPropertyValue As String = ""

                Dim toReturn As String = scope
                Dim searchTag As String
                Dim _
                    objProperties As Dictionary(Of String, PropertyInfo) = _
                        ReflectionHelper.GetPropertiesDictionary(objType)
                Dim objProperty As PropertyInfo
                'For intProperty = 0 To objProperties.Count - 1
                For Each objPropertyName As String In objProperties.Keys

                    objProperty = CType(objProperties(objPropertyName), PropertyInfo)

                    strPropertyName = objProperty.Name
                    searchTag = GetTag(prefix, ":"c, strPropertyName, False)
                    If toReturn.IndexOf(searchTag, System.StringComparison.InvariantCultureIgnoreCase) <> -1 Then

                        Dim objIndexParameters() As ParameterInfo = objProperty.GetIndexParameters

                        If Not objIndexParameters.Length > 0 Then
                            searchTag = GetTag(prefix, ":"c, strPropertyName, True)
                            propValue = objProperty.GetValue(objObject, Nothing)
                            If propValue IsNot Nothing Then
                                strPropertyValue = propValue.ToString()
                            End If

                            ' special case for encrypted passwords
                            If _
                                (prefix & strPropertyName = "Membership:Password") And _
                                Convert.ToString(Globals.HostSettings("EncryptionKey")) <> "" Then
                                Dim objSecurity As New PortalSecurity
                                strPropertyValue = _
                                    objSecurity.Decrypt(Globals.HostSettings("EncryptionKey").ToString, strPropertyValue)
                            End If
                            If htmlEncode Then
                                strPropertyValue = HttpUtility.HtmlEncode(strPropertyValue)
                            End If
                            toReturn = Replace(toReturn, searchTag, strPropertyValue, , , CompareMethod.Text)
                        Else
                            ' Indexed property : fetch property values from index
                            searchTag = searchTag.TrimEnd("]"c) & ":"
                            Dim index As Integer = toReturn.IndexOf(searchTag, 0)

                            While index <> -1

                                Dim endTagIndex As Integer = toReturn.IndexOf("]"c, index)
                                Dim currentSearchTag As String = toReturn.Substring(index, endTagIndex - index + 1)
                                Dim currentIndex As Integer = searchTag.Length
                                Dim paramValues As New List(Of Object)
                                For i As Integer = 0 To objIndexParameters.Length - 1
                                    Dim objIndexParameter As ParameterInfo = objIndexParameters(i)
                                    Dim tempIndex As Integer
                                    If i = objIndexParameters.Length - 1 Then
                                        tempIndex = currentSearchTag.Length - 1
                                    Else
                                        tempIndex = currentSearchTag.IndexOf(":", currentIndex)
                                    End If
                                    Dim strParameterValue As String
                                    Try
                                        strParameterValue = _
                                            currentSearchTag.Substring(currentIndex, tempIndex - currentIndex)
                                        Dim _
                                            objParamValue As Object = _
                                                Convert.ChangeType(strParameterValue, objIndexParameter.ParameterType)
                                        paramValues.Add(objParamValue)
                                    Catch ex As Exception
                                        Throw _
                                            New Exception( _
                                                           "Referenced property by token """ & currentSearchTag & _
                                                           """ is not correctly indexed. Please rebuild the index.", ex)
                                    End Try
                                    currentIndex = tempIndex + 1

                                Next
                                Try
                                    propValue = objProperty.GetValue(objObject, paramValues.ToArray())
                                Catch
                                    propValue = ""
                                End Try


                                If propValue IsNot Nothing Then
                                    strPropertyValue = propValue.ToString()
                                End If

                                toReturn = _
                                    Replace(toReturn, currentSearchTag, strPropertyValue, , , CompareMethod.Text)
                                index = toReturn.IndexOf(searchTag, index)
                            End While

                        End If
                    End If



                Next

                Return toReturn

            End If

            Return scope

        End Function

        ''' <summary>
        ''' Gets a string for advanced token replacement
        ''' </summary>
        ''' <param name="prefix">Prefix value</param>
        ''' <param name="middlefix">Separator</param>
        ''' <param name="suffix">Suffix value</param>
        ''' <param name="endTag"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetTag(ByVal prefix As String, ByVal middlefix As Char, ByVal suffix As String, _
                                 Optional ByVal endTag As Boolean = True) As String
            Dim _
                toReturn As String = "[" & prefix.TrimStart("["c).TrimEnd(middlefix) & middlefix & _
                                     suffix.TrimStart(middlefix).TrimEnd("]"c)
            If endTag Then
                toReturn = toReturn & "]"
            End If
            Return toReturn
        End Function


#End Region

#Region " Time methods "

        ''' <summary>
        ''' Returns time adjusted to user timezone
        ''' </summary>
        ''' <param name="sourceDate"></param>
        ''' <param name="format"></param>
        ''' <param name="timeZone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAdjustedUserTime(ByVal sourceDate As String, ByVal format As String, _
                                             ByVal timeZone As Integer) As String

            Dim dateToFormat As DateTime = DateTime.Parse(sourceDate, Thread.CurrentThread.CurrentCulture)
            Dim objUserTime As New UserTime

            Return _
                objUserTime.ConvertToUserTime(dateToFormat, objUserTime.ClientToServerTimeZoneFactor).ToString(format)

        End Function

        ''' <summary>
        ''' Returns time adjusted to user timezone
        ''' </summary>
        ''' <param name="dateString"></param>
        ''' <param name="format"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAdjustedUserTime(ByVal dateString As String, ByVal format As String) As String

            Return GetAdjustedUserTime(dateString, format)

        End Function

        ''' <summary>
        ''' Returns time adjusted to user timezone
        ''' </summary>
        ''' <param name="objDateTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAdjustedUserTime(ByVal objDateTime As DateTime) As DateTime

            Dim dateToFormat As DateTime = objDateTime
            Dim objUserTime As New UserTime

            Return objUserTime.ConvertToUserTime(dateToFormat, objUserTime.ClientToServerTimeZoneFactor)

        End Function

        ''' <summary>
        ''' Returns time adjusted to server timezone
        ''' </summary>
        ''' <param name="objDateTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAdjustedServerTime(ByVal objDateTime As DateTime) As DateTime

            Dim dateToFormat As DateTime = objDateTime
            Dim objUserTime As New UserTime

            Return objUserTime.ConvertToServerTime(dateToFormat, objUserTime.ClientToServerTimeZoneFactor())

        End Function

#End Region

#Region "Module Methods"

        ''' <summary>
        ''' Returns the shared resource file for a module definition id
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleSharedResourceFile(ByVal moduleDefId As Integer) As String

            Dim toReturn As String = GetGlobal(Of String)("SharedResourceFile", moduleDefId.ToString(CultureInfo.InvariantCulture))
            If toReturn Is Nothing OrElse toReturn = "" Then
                toReturn = GetModuleDirectoryPath(moduleDefId) & Localization.LocalResourceDirectory & "/" & _
                           Localization.LocalSharedResourceFile
                SetGlobal(Of String)(toReturn, "SharedResourceFile", moduleDefId.ToString(CultureInfo.InvariantCulture))
            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Returns the shared resource file for a module definition id
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleSharedResourceFile(ByVal moduleName As String) As String
            Return GetModuleSharedResourceFile(GetModuleDefIdByModuleName(moduleName))
        End Function

        ''' <summary>
        ''' Returns the DesktopModuleInfo for a module definition id
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDesktopModuleByModuleDefId(ByVal moduleDefId As Integer) As DesktopModuleInfo
            If Not _DesktopModulesByModuleDefId.ContainsKey(moduleDefId) Then
                Dim mdc As New ModuleDefinitionController
                Dim dmc As New DesktopModuleController
                Dim def As ModuleDefinitionInfo = mdc.GetModuleDefinition(moduleDefId)
                Dim dm As DesktopModuleInfo = dmc.GetDesktopModule(def.DesktopModuleID)
                _DesktopModulesByModuleDefId(moduleDefId) = dm
                If Not _ModuleDefIdByModuleName.ContainsKey(dm.ModuleName) Then
                    _ModuleDefIdByModuleName(dm.ModuleName) = moduleDefId
                End If
            End If
            Return _DesktopModulesByModuleDefId(moduleDefId)
        End Function

        ''' <summary>
        ''' Returns boolean indicating whether the module is installed
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsModuleInstalled(ByVal moduleName As String) As Boolean
            Return GetModuleDefIdByModuleName(moduleName) <> -1
        End Function

        ''' <summary>
        ''' Returns module definition id according to module name
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDefIdByModuleName(ByVal moduleName As String) As Integer
            Dim toReturn As Integer = Nothing
            If Not _ModuleDefIdByModuleName.TryGetValue(moduleName, toReturn) Then
                toReturn = -1
                Dim dm As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(moduleName)
                If dm IsNot Nothing Then
                    Dim moduleDefs As ArrayList = ModuleDefinitionController.GetModuleDefinitions(dm.DesktopModuleID)
                    Dim moduleDef As ModuleDefinitionInfo = DirectCast(moduleDefs(0), ModuleDefinitionInfo)
                    If moduleDef IsNot Nothing Then
                        toReturn = moduleDef.ModuleDefID
                        If Not _DesktopModulesByModuleDefId.ContainsKey(toReturn) Then
                            _DesktopModulesByModuleDefId(toReturn) = dm
                        End If
                    End If
                End If
                _ModuleDefIdByModuleName(moduleName) = toReturn
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Returns module directory path
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryPath(ByVal moduleName As String) As String
            Return GetModuleDirectoryPath(GetModuleDefIdByModuleName(moduleName), True)
        End Function

        ''' <summary>
        ''' Returns module directory path
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryPath(ByVal moduleDefId As Integer) As String

            Return GetModuleDirectoryPath(moduleDefId, True)

        End Function

        ''' <summary>
        ''' Returns module directory path
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryPath(ByVal moduleDefId As Integer, ByVal useCache As Boolean) As String

            Return DotNetNuke.Common.Globals.ApplicationPath & GetModuleDirectorySubPath(moduleDefId, useCache)

        End Function

        ''' <summary>
        ''' Returns module directory sub path
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectorySubPath(ByVal moduleDefId As Integer, ByVal useCache As Boolean) As String


            Dim toReturn As String = ""
            If useCache Then
                toReturn = GetGlobal(Of String)("ModuleDirectorySubPath", moduleDefId.ToString(CultureInfo.InvariantCulture))
            End If
            If String.IsNullOrEmpty(toReturn) Then
                Dim moduleDef As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinition(moduleDefId)
                If moduleDef IsNot Nothing Then
                    Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModule(moduleDef.DesktopModuleID)
                    toReturn = "/DesktopModules/" & desktopModule.FolderName & "/"
                    If useCache Then
                        SetGlobal(Of String)(toReturn, "ModuleDirectorySubPath", moduleDefId.ToString(CultureInfo.InvariantCulture))
                    End If
                End If
            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Returns module directory path mapped to physical folder
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryMapPath(ByVal moduleName As String, ByVal useCache As Boolean) As String
            Return GetModuleDirectoryMapPath(GetModuleDefIdByModuleName(moduleName), useCache)
        End Function

        ''' <summary>
        ''' Returns module directory path mapped to physical folder
        ''' </summary>
        ''' <param name="moduleName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryMapPath(ByVal moduleName As String) As String
            Return GetModuleDirectoryMapPath(moduleName, True)
        End Function

        ''' <summary>
        ''' Returns module directory path mapped to physical folder
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryMapPath(ByVal moduleDefId As Integer) As String
            Return GetModuleDirectoryMapPath(moduleDefId, True)
        End Function

        ''' <summary>
        ''' Returns module directory path mapped to physical folder
        ''' </summary>
        ''' <param name="moduleDefId"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleDirectoryMapPath(ByVal moduleDefId As Integer, ByVal useCache As Boolean) As String

            Dim dirPath As String = GetModuleDirectorySubPath(moduleDefId, useCache)
            Return NukeHelper.ApplicationMapPath & dirPath.Replace("/", "\")

        End Function


        ''' <summary>
        ''' Returns the tab id for a module
        ''' </summary>
        ''' <param name="moduleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTabIdByModuleId(ByVal moduleId As Integer) As Integer
            Dim toReturn As Integer = Nothing
            If Not _TabIdByMid.TryGetValue(moduleId, toReturn) Then
                Dim myModuleInfo As ModuleInfo = ModuleController.GetModule(moduleId, -1, False)
                If myModuleInfo IsNot Nothing Then
                    _TabIdByMid(moduleId) = myModuleInfo.TabID
                    Return myModuleInfo.TabID
                End If
                Return -1
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' returns the name of a module
        ''' </summary>
        ''' <param name="moduleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleNameByModuleId(ByVal moduleId As Integer) As String
            Dim toReturn As String = Nothing
            If Not _ModuleNameByMid.TryGetValue(moduleId, toReturn) Then
                Dim myModuleInfo As ModuleInfo = ModuleController.GetModule(moduleId, -1, False)
                If myModuleInfo IsNot Nothing Then
                    _FriendlyModuleNameByMid(moduleId) = myModuleInfo.FriendlyName
                    _ModuleNameByMid(moduleId) = myModuleInfo.ModuleName
                    Return myModuleInfo.ModuleName
                End If
                Return String.Empty
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' returns the friendly name of a module
        ''' </summary>
        ''' <param name="moduleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleFriendlyNameByModuleId(ByVal moduleId As Integer) As String
            Dim toReturn As String = Nothing
            If Not _FriendlyModuleNameByMid.TryGetValue(moduleId, toReturn) Then
                Dim myModuleInfo As ModuleInfo = ModuleController.GetModule(moduleId, -1, False)
                If myModuleInfo IsNot Nothing Then
                    _FriendlyModuleNameByMid(moduleId) = myModuleInfo.FriendlyName
                    _ModuleNameByMid(moduleId) = myModuleInfo.ModuleName
                    Return myModuleInfo.FriendlyName
                End If
                Return String.Empty
            End If
            Return toReturn
        End Function


#End Region

#Region "Path and File"

        Private _ApplicationMapPath As String

        ''' <summary>
        ''' Returns application mapped path
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ApplicationMapPath As String
            Get
                If String.IsNullOrEmpty(_ApplicationMapPath) Then
                    _ApplicationMapPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd("\"c)
                End If
                Return _ApplicationMapPath
            End Get
        End Property

        ''' <summary>
        ''' Returns path from a control url
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="controlUrl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPathFromCtrUrl(ByVal portalId As Integer, ByVal controlUrl As String) As String

            Return GetPathFromCtrUrl(portalId, controlUrl, False)

        End Function

        ''' <summary>
        ''' Returns path from a control url
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="controlUrl"></param>
        ''' <param name="track"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPathFromCtrUrl(ByVal portalId As Integer, ByVal controlUrl As String, ByVal track As Boolean) As String

            Dim toreturn As String = controlUrl
            Dim uRLType As TabType = Globals.GetURLType(controlUrl)

            If (uRLType = TabType.File) AndAlso Not track Then

                Dim fileId As Integer = -1
                If controlUrl.ToLower.StartsWith("fileid=", StringComparison.OrdinalIgnoreCase) Then
                    fileId = Integer.Parse(UrlUtils.GetParameterValue(controlUrl), CultureInfo.InvariantCulture)
                End If
                If fileId > -1 Then
                    Return FileHelper.GetFileUrl(fileId, portalId)
                Else
                    Return Globals.ApplicationPath & "/"c & controlUrl
                End If

            End If

            Return LinkClick(toreturn, -1, -1, track)

        End Function

        ''' <summary>
        ''' Returns file information from control url
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="controlUrl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFileInfoFromCtrUrl(ByVal portalId As Integer, ByVal controlUrl As String) As FileInfo

            Dim toreturn As FileInfo = Nothing
            Dim uRLType As TabType = GetURLType(controlUrl)

            If (uRLType = TabType.File) Then

                Dim fc As New FileController
                Dim fileId As Integer
                If controlUrl.ToLower(CultureInfo.InvariantCulture).StartsWith("fileid=", _
                                                                                  StringComparison.OrdinalIgnoreCase) Then
                    fileId = Integer.Parse(UrlUtils.GetParameterValue(controlUrl), CultureInfo.InvariantCulture)


                Else
                    fileId = fc.ConvertFilePathToFileId(controlUrl, portalId)
                End If

                toreturn = fc.GetFileById(fileId, portalId)
            End If

            Return toreturn

        End Function

        ''' <summary>
        ''' Returns mapped folder path
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="folderPath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFolderMapPath(ByVal portalId As Integer, ByVal folderPath As String) As String

            Dim folderController As New FolderController

            Dim toReturn As String

            toReturn = folderController.GetFolder(portalId, folderPath, False).PhysicalPath

            Return toReturn
        End Function

        ''' <summary>
        ''' Returns mapped folder path
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="folderId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFolderMapPath(ByVal portalId As Integer, ByVal folderId As Integer) As String

            Dim pc As New PortalController
            Dim folderController As New FolderController

            Dim toReturn As String
            If folderId = -1 Then
                toReturn = pc.GetPortal(portalId).HomeDirectoryMapPath
            Else
                toReturn = folderController.GetFolderInfo(portalId, folderId).PhysicalPath
            End If

            Return toReturn
        End Function




#End Region

        ''' <summary>
        ''' Returns locale from neutral language
        ''' </summary>
        ''' <param name="language"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLocaleFromNeutralLangauge(ByVal language As String) As Locale

            For Each objLocale As Locale In Localization.GetEnabledLocales()
                If objLocale.Code.StartsWith(language) Then
                    If Not objLocale.Fallback.StartsWith(language) Then
                        Return objLocale
                    End If
                End If
            Next
            Return Nothing

        End Function


        Private _SingletonListEntries As New Dictionary(Of Integer, ListEntryInfo)

        Public Function GetListEntrySingleton(id As Integer) As ListEntryInfo
            Dim toReturn As ListEntryInfo = Nothing
            If Not _SingletonListEntries.TryGetValue(id, toReturn) Then
                toReturn = ListController.GetListEntryInfo(id)
                SyncLock _SingletonListEntries
                    _SingletonListEntries(id) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function


    End Module
End Namespace
