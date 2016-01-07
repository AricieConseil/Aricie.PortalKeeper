Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Web.Compilation
Imports System.Xml
Imports DotNetNuke.Common
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.WebControls
Imports Aricie.Collections
Imports Aricie.Services
Imports Aricie.DNN.UI.Controls
Imports Aricie.DNN.Settings
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.Services
Imports Aricie.DNN.Diagnostics
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.TestSurrogates
Imports Aricie.Web


Namespace Aricie.DNN.Modules.PortalKeeper.UI
    Partial Class KeeperConfig
        Inherits AriciePortalModuleBase

        Public Sub New()
            MyBase.New()

#If DEBUG Then
            'the following mechansim illustrates bringing a compiled coding section into edit and continue domain within this code behind file
            AddHandler DnnContext.Current.Debug, AddressOf Me.OnDebug
            RegisterDebugSurrogates()
#End If

        End Sub




        Private _KeeperConfig As PortalKeeperConfig
        Private _KeeperSettings As FirewallSettings

       

        

        Protected ReadOnly Property KeeperConfig() As PortalKeeperConfig
            Get

                If _KeeperConfig Is Nothing Then
                    If Me.IsPostBack AndAlso Me.UserInfo.IsSuperUser Then
                        _KeeperConfig = DirectCast(Session("KeeperConfig"), PortalKeeperConfig)
                        If _KeeperConfig Is Nothing Then
                            _KeeperConfig = ReflectionHelper.CloneObject(Of PortalKeeperConfig)(PortalKeeperConfig.Instance)
                            Session("KeeperConfig") = _KeeperConfig
                        End If
                    Else
                        Dim objConfig As PortalKeeperConfig = PortalKeeperConfig.Instance
                        _KeeperConfig = objConfig
                        Session.Remove("KeeperConfig")
                        If Me.UserInfo.IsSuperUser Then
                            Dim cloningJob As New SessionCloningJob(Of PortalKeeperConfig)(Me.Session, objConfig, "KeeperConfig")
                            cloningJob.Enqueue()
                        End If
                    End If
                End If

                Return _KeeperConfig
            End Get
        End Property

        Protected ReadOnly Property KeeperSettings() As FirewallSettings
            Get
                If _KeeperSettings Is Nothing Then
                    If Not Me.IsPostBack OrElse Session("KeeperSettings") Is Nothing Then
                        Session("KeeperSettings") = SettingsController.GetModuleSettings(Of FirewallSettings)(SettingsScope.PortalSettings, NukeHelper.PortalId)
                    End If
                    _KeeperSettings = DirectCast(Session("KeeperSettings"), FirewallSettings)
                End If
                Return _KeeperSettings
            End Get
        End Property


        Private _UserBotSettings As UserBotSettings(Of ScheduleEvent)

        Protected ReadOnly Property UserBotSettings As UserBotSettings(Of ScheduleEvent)
            Get
                If _UserBotSettings Is Nothing Then
                    Me.KeeperConfig.SchedulerFarm.AvailableUserBots.TryGetValue(Me.KeeperModuleSettings.UserBotName, _UserBotSettings)
                End If
                Return _UserBotSettings
            End Get
        End Property


        Private _UserBot As UserBotInfo

        Public Property UserBot() As UserBotInfo
            Get
                If _UserBot Is Nothing Then

                    If Me.UserBotSettings IsNot Nothing Then
                        Dim key As String = "UserBot" & Me.UserBotSettings.Name
                        If Not Me.IsPostBack OrElse Session(key) Is Nothing Then

                            Session(key) = Me.UserBotSettings.GetUserBotInfo(Me.UserInfo, True)
                        End If
                        _UserBot = DirectCast(Session(key), UserBotInfo)
                        If Me.KeeperModuleSettings.DisplayRankings Then
                            Dim objRankings As List(Of ProbeRanking) = Me.UserBotSettings.GetRankings()
                            If objRankings IsNot Nothing Then
                                _UserBot.Rankings = objRankings
                            End If
                        End If
                    End If
                End If

                Return _UserBot
            End Get
            Set(value As UserBotInfo)
                Dim userSettings As UserBotSettings(Of ScheduleEvent) = Nothing
                If Me.KeeperConfig.SchedulerFarm.AvailableUserBots.TryGetValue(Me.KeeperModuleSettings.UserBotName, userSettings) Then
                    userSettings.SetUserBotInfo(Me.UserInfo, Me.PortalId, value)
                    Dim key As String = "UserBot" & userSettings.Name
                    Session.Remove(key)
                    _UserBot = Nothing
                End If

            End Set
        End Property


        Protected ReadOnly Property KeeperModuleSettings() As KeeperModuleSettings
            Get
                Return Me.GetModuleSettings(Of KeeperModuleSettings)()
            End Get
        End Property

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Try

                AddHandler Me.KC.Debug, AddressOf Me.OnDebug
                If Not Me.IsPostBack Then
                    AssertInstalled()
#If DEBUG Then
                                        Me.divDebug.Visible = True
#End If
                End If
                EnforceFreeVersion()
                BindSettings()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub


        Private Sub EnforceFreeVersion()

            Dim tc As Aricie.DNN.Security.Trial.TrialController = Aricie.DNN.Security.Trial.TrialController.Instance(New TrialProvider())
            Dim objTrialStatus As TrialStatusInfo = tc.LimitModule(Me, True)
            Me.KC.TrialStatus = objTrialStatus
            Me.KS.TrialStatus = objTrialStatus
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Try
                If Not Me.IsPostBack Then
                    ManageVisibility()
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        'Private Sub cmdSaveConfig_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveConfig.Click
        '    Try
        '        If Me.UserInfo.IsSuperUser AndAlso Me.KC.IsValid Then
        '            Dim objSettings As PortalKeeperConfig = DirectCast(Me.KC.DataSource, PortalKeeperConfig)
        '            PortalKeeperConfig.Save(objSettings)
        '            Skin.AddModuleMessage(Me, Localization.GetString("HostConfigSaved.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
        '        Else
        '            Skin.AddModuleMessage(Me, Localization.GetString("HostConfigInvalid.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
        '        End If
        '    Catch ex As Exception
        '        ProcessModuleLoadException(Me, ex)
        '    End Try
        'End Sub

        Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
            Try
                If Me.IsAdmin AndAlso Me.KS.IsValid Then
                    Dim objSettings As FirewallSettings = DirectCast(Me.KS.DataSource, FirewallSettings)
                    SettingsController.SetModuleSettings(Of FirewallSettings)(SettingsScope.PortalSettings, Me.PortalId, objSettings)
                    Skin.AddModuleMessage(Me, Localization.GetString("ConfigSaved.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    Skin.AddModuleMessage(Me, Localization.GetString("ConfigInvalid.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        'Protected Sub cmdRunBots_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRunBots.Click
        '    Try
        '        'Dim flowid As String = Guid.NewGuid.ToString
        '        'If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

        '        '    Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run Start", _
        '        '                                WorkingPhase.InProgress, False, False, -1, flowid)
        '        '    PerformanceLogger.Instance.AddDebugInfo(objStep)
        '        'End If
        '        'Dim nbRuns As Integer = PortalKeeperConfig.Instance.SchedulerFarm.RunBots(PortalKeeperSchedule.ScheduleEventList, True, Guid.NewGuid.ToString)

        '        'If PortalKeeperConfig.Instance.SchedulerFarm.EnableLogs Then

        '        '    Dim objStep As New StepInfo(Debug.PKPDebugType, "Manual Run End", _
        '        '                                WorkingPhase.InProgress, True, False, -1, flowid)
        '        '    PerformanceLogger.Instance.AddDebugInfo(objStep)
        '        'End If

        '        'Skin.AddModuleMessage(Me, String.Format(Localization.GetString("ManualRun.Message", Me.LocalResourceFile), nbRuns), ModuleMessage.ModuleMessageType.GreenSuccess)
        '        PortalKeeperConfig.Instance.SchedulerFarm.RunForcedBots(Me)
        '        Me.BindSettings()
        '    Catch ex As Exception
        '        ProcessModuleLoadException(Me, ex)
        '    End Try
        'End Sub

        Protected Sub cmdInstall_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdInstall.Click
            Try
                Configuration.ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Install, New PortalKeeperConfigUpdate)
                Response.Redirect(NavigateURL())
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub cmdUninstall_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUninstall.Click
            Try
                Dim uninstallProcedure As IUpdateProvider
                Dim errorsUpdater As IUpdateProvider = KeeperConfig.ApplicationSettings.CustomErrorsConfig.GetUpdateProvider()
                If ConfigHelper.IsInstalled(errorsUpdater, True) Then
                    uninstallProcedure = New MultiUpdateProvider(errorsUpdater, New PortalKeeperConfigUpdate)
                Else
                    uninstallProcedure = New PortalKeeperConfigUpdate
                End If
                ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Uninstall, uninstallProcedure)
                Response.Redirect(Me.Request.Url.AbsoluteUri)
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub cmdClearProbes_Click(sender As Object, e As System.EventArgs) Handles cmdClearProbes.Click
            Try

                CacheHelper.RemoveCache("PKPProbes")
                Skin.AddModuleMessage(Me, Localization.GetString("ProbesCleared.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub cmdDebug_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDebug.Click
            Try
                
                DnnContext.Current.OnDebug()
                'To remove the context routing replace with:
                'Me.OnCmdDebug()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub cmdCancelSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancelSettings.Click
            Try
                Me.Response.Redirect(NavigateURL)
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        'Private Sub cmdCancelUserBot_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancelUserBot.Click
        '    Try
        '        Me.Response.Redirect(NavigateURL)
        '    Catch ex As Exception
        '        ProcessModuleLoadException(Me, ex)
        '    End Try
        'End Sub

        'Private Sub cmdSaveUserBot_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveUserBot.Click
        '    Try
        '        If Me.ctUserBotEntities.IsValid Then
        '            Dim userSettings As UserBotSettings(Of ScheduleEvent) = Nothing
        '            If Me.KeeperConfig.SchedulerFarm.AvailableUserBots.TryGetValue(Me.KeeperModuleSettings.UserBotName, userSettings) Then
        '                Dim userBotEntity As UserBotInfo = DirectCast(Me.ctUserBotEntities.DataSource, UserBotInfo)
        '                Dim readOnlyUserBot As UserBotInfo = userSettings.GetUserBotInfo(Me.KeeperConfig.SchedulerFarm, Me.UserInfo, Me.PortalId, True)
        '                userBotEntity.RevertReadonlyParameters(readOnlyUserBot)
        '                Me.UserBot = userBotEntity
        '                Me.BindSettings()
        '                Skin.AddModuleMessage(Me, Localization.GetString("UserBotSaved.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
        '            End If

        '        End If
        '    Catch ex As Exception
        '        ProcessModuleLoadException(Me, ex)
        '    End Try
        'End Sub

        'Private Sub cmdDeleteUserBot_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDeleteUserBot.Click
        '    Try
        '        Dim userSettings As UserBotSettings(Of ScheduleEvent) = Nothing
        '        If Me.KeeperConfig.SchedulerFarm.AvailableUserBots.TryGetValue(Me.KeeperModuleSettings.UserBotName, userSettings) Then
        '            Me.UserBot = Nothing
        '            Me.BindSettings()
        '            Skin.AddModuleMessage(Me, Localization.GetString("UserBotDeleted.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
        '        End If
        '    Catch ex As Exception
        '        ProcessModuleLoadException(Me, ex)
        '    End Try
        'End Sub

#End Region

#Region "Private methods"

        Private Sub BindSettings()
            If Me.UserInfo.IsSuperUser Then
                Me.divUserBot.Controls.Clear()
                Me.KC.LocalResourceFile = Me.SharedResourceFile
                Me.KC.DataSource = Me.KeeperConfig
                Me.KC.DataBind()
            Else
                Me.divHostConfig.Controls.Clear()
                If Me.KeeperConfig.SchedulerFarm.EnableUserBots AndAlso Not String.IsNullOrEmpty(Me.KeeperModuleSettings.UserBotName) Then
                    If Me.KeeperModuleSettings.UserBot IsNot Nothing AndAlso Me.KeeperModuleSettings.UserBot.Bot IsNot Nothing Then
                        Dim userSettings As UserBotSettings(Of ScheduleEvent) = Nothing
                        If Me.KeeperConfig.SchedulerFarm.AvailableUserBots.TryGetValue(Me.KeeperModuleSettings.UserBotName, userSettings) Then

                            If Me.KeeperModuleSettings.EnableActionCommands AndAlso Me.KeeperModuleSettings.UserBot.ActionCommands.Count > 0 Then
                                Me.divActionCommands.Visible = True
                                If Me.divActionCommands.Controls.Count = 0 Then
                                    For Each objActionCommand As ActionCommand In Me.KeeperModuleSettings.UserBot.ActionCommands
                                        Dim cb As New CommandButton()
                                        Me.divActionCommands.Controls.Add(cb)
                                        cb.ID = "cmd" & objActionCommand.Name
                                        cb.Text = objActionCommand.Name
                                        If Not String.IsNullOrEmpty(objActionCommand.ResxKey) Then
                                            cb.ResourceKey = objActionCommand.ResxKey
                                        End If
                                        cb.ImageUrl = objActionCommand.IconPath
                                        cb.CommandName = objActionCommand.Name
                                        AddHandler cb.Click, AddressOf ActionCommandClick
                                    Next
                                End If
                            End If

                            Me.ctUserBotEntities.LocalResourceFile = Me.SharedResourceFile
                            Me.ctUserBotEntities.DataSource = Me.UserBot

                            Me.ctUserBotEntities.DataBind()
                            If Me.KeeperModuleSettings.DisplayBotConfig Then
                                Me.lblBotDefinition.Visible = True
                                Me.ctlUserBot.LocalResourceFile = Me.SharedResourceFile.Replace("SharedResources", Me.KeeperModuleSettings.UserBot.ResourceFile)
                                Me.ctlUserBot.EditMode = DotNetNuke.UI.WebControls.PropertyEditorMode.View
                                Me.ctlUserBot.DataSource = Me.KeeperModuleSettings.UserBot.Bot
                                Me.ctlUserBot.DataBind()
                            Else
                                Me.lblBotDefinition.Visible = False
                            End If
                        End If
                    End If
                Else
                    Me.divUserBot.Controls.Clear()
                End If
            End If
            If Me.KeeperConfig.FirewallConfig.EnablePortalLevelSettings AndAlso Me.IsAdmin Then
                Me.KS.LocalResourceFile = Me.SharedResourceFile
                Me.KS.DataSource = Me.KeeperSettings
                Me.KS.DataBind()
            Else
                Me.divPortalSettings.Controls.Clear()
            End If
        End Sub


        Private Sub ActionCommandClick(sender As Object, e As EventArgs)
            Dim cb As CommandButton = DirectCast(sender, CommandButton)
            Dim commandName As String = cb.CommandName
            For Each objActionCommand As ActionCommand In Me.KeeperModuleSettings.UserBot.ActionCommands
                If objActionCommand.Name = commandName Then
                    Try
                        Dim resultContext As New PortalKeeperContext(Of ScheduleEvent)
                        resultContext.Init(Me.UserBotSettings.Bot)
                        resultContext.InitParams(Me.UserBot.GetParameterValues(Me.UserInfo))
                        resultContext.Init(objActionCommand)
                        objActionCommand.BatchRun(PortalKeeperSchedule.ScheduleEventList, resultContext)

                        'Dim resultVars As SerializableDictionary(Of String, Object) = resultContext.ComputeDynamicExpressions(objActionCommand.ComputedVars, True)

                        Dim objDumpSettings As New DumpSettings()
                        objDumpSettings.EnableDump = True
                        objDumpSettings.DumpAllVars = False
                        objDumpSettings.DumpVariables = objActionCommand.ComputedVars
                        objDumpSettings.SkipNull = False
                        Dim resultVars As SerializableDictionary(Of String, Object) = resultContext.GetDump(objDumpSettings)

                        If resultVars.Count > 0 Then
                            UserBot.ComputedEntities = resultVars
                        End If
                        Me.BindSettings()
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                    End Try
                End If
            Next
        End Sub

        Private Sub AssertInstalled()
            'HttpContext.Current.Response.Write("test")

            If Me.UserInfo.IsSuperUser Then
                Me.divInstall.Visible = True
                Configuration.ConfigHelper.AssertIsInstalled(Me, New PortalKeeperConfigUpdate, Me.cmdInstall, New Control() {Me.cmdUninstall, Me.divConfig})
                Me.AddConfirm(Me.cmdUninstall)
            Else
                Me.divInstall.Visible = False
            End If
        End Sub

        Private Sub ManageVisibility()
            If Me.UserInfo.IsSuperUser Then
                Me.divHostConfig.Visible = True
                Me.divPortalSettings.Visible = Me.KeeperConfig.FirewallConfig.EnablePortalLevelSettings
            Else
                Me.divHostConfig.Visible = False
                If Me.IsAdmin Then
                    If Not Me.KeeperConfig.FirewallConfig.EnablePortalLevelSettings Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("PortalLevelSettingsDisabled.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                    Else
                        Me.divPortalSettings.Visible = True
                    End If
                End If
                If Me.KeeperConfig.SchedulerFarm.EnableUserBots AndAlso Not String.IsNullOrEmpty(Me.KeeperModuleSettings.UserBotName) Then
                    If Me.KeeperModuleSettings.UserBot IsNot Nothing Then
                        Me.divUserBot.Visible = True
                        If Not DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") Then
                            Me.ctUserBotEntities.EditMode = DotNetNuke.UI.WebControls.PropertyEditorMode.View
                            Me.divUserBotCmds.Visible = False
                        Else
                            Me.ctUserBotEntities.EditMode = DotNetNuke.UI.WebControls.PropertyEditorMode.Edit
                            Me.divUserBotCmds.Visible = True
                        End If
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("NoUserBot.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                    End If
                End If
            End If

        End Sub


#End Region


#Region "Debug"


        Private Sub OnCmdDebug()



            Dim sw As New Stopwatch()
            sw.Start()

            DemonstrateSurrogates()


            DnnContext.Instance.AddModuleMessage(String.Format("Time Elapsed: {0}", Common.FormatTimeSpan(sw.Elapsed)), ModuleMessage.ModuleMessageType.GreenSuccess)

            
        End Sub

        'Public Shared Function CompileCodeDirectory(directoryName As String) As System.Reflection.Assembly
        '    Dim buildManagerType As Type = GetType(BuildManager)
        '    Dim assemblyName As String = "App_SubCode_" & directoryName
        '    Dim compileMethod As MethodInfo = buildManagerType.GetMethod("CompileCodeDirectory", BindingFlags.Instance Or BindingFlags.NonPublic)
        '    Dim theBuildManager As Object = buildManagerType.GetField("_theBuildManager", BindingFlags.Static Or BindingFlags.NonPublic).GetValue(Nothing)
        '    Dim topAssemblies As List(Of System.Reflection.Assembly) = DirectCast(buildManagerType.GetField("_topLevelReferencedAssemblies", BindingFlags.Instance Or BindingFlags.NonPublic).GetValue(theBuildManager), List(Of System.Reflection.Assembly))
        '    For Each objCodeAssembly As System.Reflection.Assembly In New ArrayList(topAssemblies)
        '        If objCodeAssembly.GetName().Name.StartsWith(assemblyName) Then
        '            topAssemblies.Remove(objCodeAssembly)
        '        End If
        '    Next

        '    Dim codeAssemblies As ArrayList = DirectCast(buildManagerType.GetField("_codeAssemblies", BindingFlags.Instance Or BindingFlags.NonPublic).GetValue(theBuildManager), ArrayList)
        '    For Each objCodeAssembly As System.Reflection.Assembly In New ArrayList(codeAssemblies)
        '        If objCodeAssembly.GetName().Name.StartsWith(assemblyName) Then
        '            codeAssemblies.Remove(objCodeAssembly)
        '        End If
        '    Next
        '    assemblyName = assemblyName & DateTime.UtcNow.ToFileTimeUtc().ToString()
        '    Dim appVirtualPath As Object = GetType(HttpRuntime).GetProperty("CodeDirectoryVirtualPath", BindingFlags.Static Or BindingFlags.NonPublic).GetValue(Nothing, Nothing)
        '    Dim virtualPathType As Type = appVirtualPath.GetType()
        '    Dim targetVirtualDir As Object = virtualPathType.GetMethod("SimpleCombineWithDir", BindingFlags.Instance Or BindingFlags.NonPublic).Invoke(appVirtualPath, {directoryName})
        '    Dim typeCodeDirectoryType As Type = compileMethod.GetParameters()(1).ParameterType 'BuildManager.GetType("System.Web.Compilation.CodeDirectoryType, System.Web", False)
        '    Dim objCodeDirectoryType As Object = [Enum].Parse(typeCodeDirectoryType, "SubCode")


        '    Dim toReturn As System.Reflection.Assembly = DirectCast(compileMethod.Invoke(theBuildManager, {targetVirtualDir, objCodeDirectoryType, assemblyName, Nothing}), System.Reflection.Assembly)
        '    DotNetNuke.Common.Utilities.DataCache.ClearCache()
        '    Return toReturn
        'End Function

        Private Sub OnDebug(sender As Object, e As DebugEventArgs)
            Try

                DnnContext.Instance.AddModuleMessage("Event routed from DNN Context", ModuleMessage.ModuleMessageType.GreenSuccess)
                OnCmdDebug()

            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub


        'Private Function TestReddit() As String
        '    Dim objReddit As New RedditSharp.Reddit(RedditSharp.WebAgent.RateLimitMode.Burst)
        '    'objReddit.User = objReddit.LogIn("Jessynoo", "cetia540", True)
        '    Dim toreturn As String = ""
        '    Dim botName As String = "Alfred_Centworth"
        '    objReddit.User = objReddit.LogIn(botName, "cetia540", True)


        '    Dim messages As IEnumerable(Of Comment) = objReddit.GetUser("Jessynoo").Comments.Take(5)
        '    'Dim messages As IEnumerable(Of Comment) = objReddit.User.Inbox()
        '    For Each objMessage In messages
        '        Dim answered As Boolean = objMessage.Comments.Any(Function(tmpMessage) tmpMessage.Author.AuthorName = botName)
        '        If Not answered Then
        '            toreturn += HttpUtility.HtmlEncode(ReflectionHelper.Serialize(messages).Beautify())
        '        End If
        '        'objMessage.Reply("")
        '    Next
        '    Return toreturn
        'End Function

        Private Sub RegisterDebugSurrogates()
            ReflectionHelper.RegisterDebugSurrogate(AddressOf Me.SurrogateCallback, True)
        End Sub
        Public Function SurrogateCallback(objType As Type) As Object
            Dim toReturn As Object = Nothing
            Select Case objType.FullName
                Case GetType(TestSurrogateOriginal).FullName
                    toReturn = New TestSurrogateReplacement()
            End Select
            Return toReturn

        End Function

        Private Sub DemonstrateSurrogates()

            Dim message As String

            'We first instantiate the original Type. It can be a static or a dynamic Type

            Dim testSurrogate As New TestSurrogateOriginal()

            message = testSurrogate.TestFunction()

            DnnContext.Instance.AddModuleMessage(String.Format("Debug: {0}", message.ToString()), ModuleMessage.ModuleMessageType.GreenSuccess)

            'We then use the surrogate mechanism through the CreateObject method. The surrogate was registered in the user control constructor

            testSurrogate = ReflectionHelper.CreateObject(Of TestSurrogateOriginal)()

            message = testSurrogate.TestFunction()

            DnnContext.Instance.AddModuleMessage(String.Format("Debug: {0}", message.ToString()), ModuleMessage.ModuleMessageType.GreenSuccess)

            'We then use the Automatic Surrogate mechanism through the CreateType method. Adding Debug to the Namespace of an original Type let you define a surrogate either in App_Code or in the calling dynamic code file

            'Note this will work if the corresponding application action is enabled or by setting:
            'ReflectionHelper.EnableAutoDebugSurrogates = True

            testSurrogate = ReflectionHelper.CreateObject(Of TestSurrogateOriginal)(ReflectionHelper.CreateType(GetType(TestSurrogateOriginal).AssemblyQualifiedName, False))

            message = testSurrogate.TestFunction()

            DnnContext.Instance.AddModuleMessage(String.Format("Debug: {0}", message.ToString()), ModuleMessage.ModuleMessageType.GreenSuccess)

        End Sub

#End Region


    End Class

   

End Namespace

#Region "Surrogates Demonstration"



Namespace Aricie.DNN.TestSurrogates

    Public Class TestSurrogateOriginal

        Public Overridable Function TestFunction() As String
            Return "Hello World from original: " & Me.GetType.AssemblyQualifiedName
        End Function

    End Class

    Public Class TestSurrogateReplacement
        Inherits TestSurrogateOriginal

        Public Overrides Function TestFunction() As String
            Return "Hello World from explicit surrogate: " & Me.GetType.AssemblyQualifiedName
        End Function

    End Class

End Namespace

Namespace Aricie.DNN.TestSurrogates.Debug

    Public Class TestSurrogateReplacement
        Inherits Aricie.DNN.TestSurrogates.TestSurrogateReplacement

        Public Overrides Function TestFunction() As String
            Return "Hello World from automatic surrogate to explicit surrogate: " & Me.GetType.AssemblyQualifiedName
        End Function

    End Class

End Namespace

#End Region




