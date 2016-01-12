Imports Aricie.DNN.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Collections
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Personalization
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Workers
Imports Aricie.DNN.Services.Flee
Imports System.Globalization
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Files
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Users, IconOptions.Normal)>
    <XmlInclude(GetType(UserVariableInfo))>
    Public Class UserBotSettings(Of TEngineEvent As IConvertible)
        Inherits NamedConfig
        Implements ISelector(Of BotInfo(Of TEngineEvent))

#Region "Fields"

        Private _BotName As String = ""

        'Private _Compress As Boolean

        'Private _Encrypt As Boolean = True

        Private _FormerName As String = ""

        Private _DisableTemplateBot As Boolean = True

        'Private _UserBot As BotInfo(Of TEngineEvent)

        Private _UserParameters As New SimpleList(Of UserVariableInfo)

        Private _VariableOverwrites As New Variables

        Private _Bot As BotInfo(Of TEngineEvent)

        Private _Probes As New SimpleList(Of ProbeSettings(Of TEngineEvent))

        Private _ResourceFile As String = "SharedResources"

        'Private _encrypter As IEncrypter

#End Region

#Region "Public properties"

        <ExtendedCategory("MasterBot")>
        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        <ProvidersSelector()>
        <AutoPostBack()>
        Public Property BotName() As String
            Get
                Return _BotName
            End Get
            Set(ByVal value As String)
                _BotName = value
                Me._Bot = Nothing
            End Set
        End Property

        <ExtendedCategory("MasterBot")>
        Public Property DisableTemplateBot() As Boolean
            Get
                Return _DisableTemplateBot
            End Get
            Set(ByVal value As Boolean)
                _DisableTemplateBot = value
            End Set
        End Property

        <Browsable(False)>
        Public ReadOnly Property Bot() As BotInfo(Of TEngineEvent)
            Get
                If _Bot Is Nothing Then
                    For Each webBot As BotInfo(Of TEngineEvent) In PortalKeeperConfig.Instance.GetBotFarm(Of TEngineEvent).Bots.Instances
                        If webBot.Name = Me._BotName Then
                            _Bot = ReflectionHelper.CloneObject(webBot)
                            Exit For ' on a trouvé notre robot, pas la peine de continuer
                        End If
                    Next
                End If
                Return _Bot
            End Get
        End Property




        '<ExtendedCategory("Parameters")> _
        'Public Property Compress() As Boolean
        '    Get
        '        Return _Compress
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _Compress = value
        '    End Set
        'End Property

        '<ExtendedCategory("Parameters")> _
        'Public Property Encrypt() As Boolean
        '    Get
        '        Return _Encrypt
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _Encrypt = value
        '    End Set
        'End Property


        <ExtendedCategory("Parameters")>
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))>
        <Width(300)>
        Public Property FormerName() As String
            Get
                Return _FormerName
            End Get
            Set(ByVal value As String)
                _FormerName = value
            End Set
        End Property

        <ExtendedCategory("Parameters")>
        Public Property ResourceFile() As String
            Get
                Return _ResourceFile
            End Get
            Set(ByVal value As String)
                _ResourceFile = value
            End Set
        End Property


        <ExtendedCategory("Parameters")>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))>
        <LabelMode(LabelMode.Top)>
        Public Property UserParameters() As SimpleList(Of UserVariableInfo)
            Get
                Return _UserParameters
            End Get
            Set(ByVal value As SimpleList(Of UserVariableInfo))
                _UserParameters = value
            End Set
        End Property

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))>
        <LabelMode(LabelMode.Top)>
        <ExtendedCategory("Parameters")>
        Public Property VariableOverwrites() As Variables
            Get
                Return _VariableOverwrites
            End Get
            Set(ByVal value As Variables)
                _VariableOverwrites = value
            End Set
        End Property

        <ExtendedCategory("Probes")>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))>
        <LabelMode(LabelMode.Top)>
        Public Property Probes() As SimpleList(Of ProbeSettings(Of TEngineEvent))
            Get
                Return _Probes
            End Get
            Set(ByVal value As SimpleList(Of ProbeSettings(Of TEngineEvent)))
                _Probes = value
            End Set
        End Property



        <ExtendedCategory("ActionCommands")>
        Public Property ActionCommands() As New SerializableList(Of ActionCommand)

        Private _CurrentUserBots As New Dictionary(Of Integer, UserBotInfo)

        '<XmlIgnore()> _
        '<Selector("", "key", "value", False, True, "Select a UserBot to Display", "", False, False)> _
        '<ExtendedCategory("Management")> _
        'Public Property UserName As String = ""


        <ExtendedCategory("Storage")>
        Public Property Storage As UserBotStorage = UserBotStorage.SmartFiles

        <ConditionalVisible("Storage", False, True, UserBotStorage.SmartFiles)>
        <ExtendedCategory("Storage")>
        Public Property StorageSettings As New SmartFileInfo

        <ExtendedCategory("Storage")>
        Public Property CachePlannedSchedule As New EnabledFeature(Of STimeSpan)(New STimeSpan(TimeSpan.FromMinutes(3)))


        'Public Sub SetEncrypter(encrypter As IEncrypter)
        '    _encrypter = encrypter
        'End Sub


        Private _UserBots As SmartFolder(Of UserBotInfo)


        '<ExtendedCategory("Management")> _
        'Public ReadOnly Property HasEncrypter As Boolean
        '    Get
        '        Return _encrypter IsNot Nothing
        '    End Get
        'End Property

        <XmlIgnore()>
        <ExtendedCategory("Management")>
        Public Property UserBots As SmartFolder(Of UserBotInfo)
            Get
                If _UserBots Is Nothing Then
                    Dim key As EntityKey = GetSampleBotKey(PortalId)
                    Dim strPath As String = Me.StorageSettings.GetFolderPath(key)
                    _UserBots = New SmartFolder(Of UserBotInfo)(Me.StorageSettings.Encryption) With {.FolderPath = New FolderPathInfo() With {.PortalId = PortalId, .Path = New SimpleOrExpression(Of String)(strPath), .PathMode = FilePathMode.AdminPath}}
                End If
                Return _UserBots
            End Get
            Set(value As SmartFolder(Of UserBotInfo))
                value.SetEncrypter(Me.StorageSettings.Encryption)
                _UserBots = value
            End Set
        End Property


        '<NonSerialized()>
        Private lockDefaultVars As New Object
        '<NonSerialized()>
        Private _DefaultVars As Dictionary(Of String, Object)

        <XmlIgnore()>
        <Browsable(False)>
        Private ReadOnly Property DefaultVars As Dictionary(Of String, Object)
            Get
                If Me._DefaultVars Is Nothing Then
                    SyncLock lockDefaultVars
                        If Me._DefaultVars Is Nothing Then
                            Dim pContext As PortalKeeperContext(Of TEngineEvent) = PortalKeeperContext(Of TEngineEvent).Instance
                            Dim tempVars As SerializableDictionary(Of String, Object) = Me.Bot.Variables.EvaluateVariables(pContext, pContext)
                            If Me._VariableOverwrites.Instances.Count > 0 Then
                                Dim overwrites As Dictionary(Of String, Object) = Me._VariableOverwrites.EvaluateVariables(pContext, pContext)
                                For Each overwrite As KeyValuePair(Of String, Object) In overwrites
                                    tempVars(overwrite.Key) = overwrite.Value
                                Next
                            End If
                            Me._DefaultVars = tempVars
                        End If
                    End SyncLock
                End If
                Return _DefaultVars
            End Get
        End Property


        '<NonSerialized()>
        Private Shared WithEvents _AsynchronousProbingTaskQueue As TaskQueue(Of UserBotProbeAccess)

        Private Shared queueLock As New Object


        Private Shared ReadOnly Property AsynchronousProbingTaskQueue() As TaskQueue(Of UserBotProbeAccess)
            Get
                If _AsynchronousProbingTaskQueue Is Nothing Then
                    SyncLock queueLock
                        If _AsynchronousProbingTaskQueue Is Nothing Then
                            Dim objTaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000))
                            _AsynchronousProbingTaskQueue = New TaskQueue(Of UserBotProbeAccess)(AddressOf GetRankingAsync, objTaskQueueInfo)
                        End If
                    End SyncLock
                End If
                Return _AsynchronousProbingTaskQueue
            End Get
        End Property

#End Region

#Region "Methods"




        Public Function GetRankings() As List(Of ProbeRanking)

            Dim toReturn As List(Of ProbeRanking) = GetGlobal(Of List(Of ProbeRanking))(Me.Name)
            If toReturn Is Nothing Then
                Dim userBotAccess As New UserBotProbeAccess(Me)
                AsynchronousProbingTaskQueue.EnqueueTask(userBotAccess)
            End If
            Return toReturn


        End Function


        'Public Function GetEnabledBotsUsers() As Dictionary(Of String, UserInfo)
        '    Dim toReturn As Dictionary(Of String, UserInfo) = CacheHelper.GetGlobal(Of Dictionary(Of String, UserInfo))("UserBots", Me.Name)
        '    If toReturn Is Nothing Then
        '        toReturn = New Dictionary(Of String, UserInfo)
        '        For Each pid As Integer In PortalIds
        '            Dim objUsers As IList(Of UserInfo) = New List(Of UserInfo)(GetPortalUsers(pid))
        '            For Each objUser As UserInfo In objUsers
        '                Dim userBotInfo As UserBotInfo = Me.GetUserBotInfo(objUser, False)
        '                If userBotInfo IsNot Nothing AndAlso userBotInfo.Enabled Then
        '                    toReturn(objUser.Username) = objUser
        '                End If
        '            Next
        '        Next
        '        CacheHelper.SetCache(Of Dictionary(Of String, UserInfo))(toReturn, TimeSpan.FromMinutes(5), "UserBots", Me.Name)
        '    End If
        '    Return toReturn
        'End Function


        Friend Function RunUserBots(ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean) As Integer
            Dim toreturn As Integer
            If Me.Bot IsNot Nothing Then
                If (Not forceRun AndAlso Me.Bot.Enabled) OrElse (forceRun AndAlso Me.Bot.ForceRun) Then
                    For Each pid As Integer In PortalIds
                        Dim shuffledUsers As IList(Of UserInfo) = New List(Of UserInfo)(GetPortalUsers(pid))
                        ShuffleList(Of UserInfo)(shuffledUsers)
                        For Each objUser As UserInfo In shuffledUsers
                            If Me.RunUserBot(objUser, events, forceRun) Then
                                toreturn += 1
                            End If
                        Next
                    Next
                End If
            End If
            Return toreturn
        End Function

        Friend Function RunUserBot(objUser As UserInfo, ByVal events As IList(Of TEngineEvent), ByVal forceRun As Boolean) As Boolean
            Dim toreturn As Boolean
            'If Not Me.Bot.AsyncLockBot.ContainsKey(objUser.UserID) Then

            Dim nextSchedulePeek As DateTime = Me.PeekUserBotInfo(objUser)
            If nextSchedulePeek <= Now Then
                'Dim userBotInfo As UserBotInfo = Me.GetUserBotInfo(objUser, objUser.PortalID, False)
                'If userBotInfo IsNot Nothing Then
                'If UserBotInfo.Enabled Then
                Dim runContext As New BotRunContext(Of TEngineEvent)(Me.Bot, nextSchedulePeek)
                runContext.AsyncLockId -= objUser.UserID
                runContext.Events = events
                'runContext.History = userBotInfo.BotHistory
                'runContext.UserParams = userBotInfo.GetParameterValues(objUser)
                Dim persister As New UserBotPersister(Me, objUser)
                AddHandler runContext.Init, AddressOf persister.InitBot
                AddHandler runContext.Finish, AddressOf persister.SaveBot

                toreturn = Me.Bot.RunBot(runContext, forceRun)
            End If

            Return toreturn
        End Function



        'Public Function PeekUserBotInfo(ByVal user As UserInfo) As DateTime
        '    Select Case Storage
        '        Case UserBotStorage.Personalisation
        '            Dim objUserBot As UserBotInfo = GetUserBotInfo(user, False)
        '            If objUserBot IsNot Nothing AndAlso objUserBot.Enabled Then
        '                Return objUserBot.BotHistory.GetNextSchedule(Me.Bot.BotSchedule) ' objUserBot.BotHistory.LastRun.Add(Me.Bot.Schedule.Value)
        '            Else
        '                Return DateTime.MaxValue
        '            End If
        '        Case UserBotStorage.SmartFiles
        '            Dim key As EntityKey = Me.GetUserBotKey(user)
        '            Dim objFileInfo As DotNetNuke.Services.FileSystem.FileInfo = SmartFile.GetFileInfo(key, Me.StorageSettings)
        '            If objFileInfo IsNot Nothing Then
        '                Return Me.Bot.BotSchedule.GetNextSchedule(ObsoleteDNNProvider.Instance.GetFileLastModificationDate(objFileInfo))
        '            Else
        '                Return DateTime.MaxValue
        '            End If
        '    End Select
        '    Return DateTime.MaxValue
        'End Function

        'todo: we need a version that limits db round trips (+split smart files MainParameters vs parameters vs history)
        Public Function PeekUserBotInfo(ByVal user As UserInfo) As DateTime
            Dim toReturn As New Nullable(Of DateTime)
            If Me.CachePlannedSchedule.Enabled Then
                toReturn = CacheHelper.GetGlobal(Of Nullable(Of DateTime))(NextScheduleUserBotKey, user.UserID.ToString(CultureInfo.InvariantCulture), Me.Name)
            End If
            If Not toReturn.HasValue Then
                'Select Case Storage
                '    Case UserBotStorage.Personalisation
                '        Dim objUserBot As UserBotInfo = GetUserBotInfo(user, False)
                '        If objUserBot IsNot Nothing AndAlso objUserBot.Enabled Then
                '            toReturn = objUserBot.BotHistory.GetNextSchedule(Me.Bot.BotSchedule) ' objUserBot.BotHistory.LastRun.Add(Me.Bot.Schedule.Value)
                '        Else
                '            toReturn = DateTime.MaxValue
                '        End If
                '    Case UserBotStorage.SmartFiles
                '        Dim key As EntityKey = Me.GetUserBotKey(user)
                '        Dim objFileInfo As DotNetNuke.Services.FileSystem.FileInfo = SmartFile.GetFileInfo(key, Me.StorageSettings)
                '        If objFileInfo IsNot Nothing Then
                '            toReturn = Me.Bot.BotSchedule.GetNextSchedule(ObsoleteDNNProvider.Instance.GetFileLastModificationDate(objFileInfo))
                '        Else
                '            toReturn = DateTime.MaxValue
                '        End If
                'End Select
                Dim objUserBot As UserBotInfo = GetUserBotInfo(user, False)
                If objUserBot IsNot Nothing AndAlso objUserBot.Enabled Then
                    toReturn = objUserBot.BotHistory.GetNextSchedule(Me.Bot.BotSchedule) ' objUserBot.BotHistory.LastRun.Add(Me.Bot.Schedule.Value)
                Else
                    toReturn = DateTime.MaxValue
                End If
                If Me.CachePlannedSchedule.Enabled Then
                    CacheHelper.SetCache(Of Nullable(Of DateTime))(toReturn.Value, Me.CachePlannedSchedule.Entity.Value, NextScheduleUserBotKey, user.UserID.ToString(CultureInfo.InvariantCulture), Me.Name)
                End If
            End If
            Return toReturn.Value
        End Function

        Public Const NextScheduleUserBotKey As String = "NextScheduleUserBot"

        Public Function GetUserBotInfo(ByVal user As UserInfo, ByVal createIfNull As Boolean) As UserBotInfo

            Dim userBot As UserBotInfo = Nothing '= CacheHelper.GetGlobal(Of UserBotInfo)(Me.Name, user.UserID.ToString(CultureInfo.InvariantCulture))
            If userBot Is Nothing Then
                Try
                    Select Case Storage
                        Case UserBotStorage.Personalisation
                            Dim pc As New PersonalizationController
                            Dim pInfo As PersonalizationInfo = pc.LoadProfile(user.UserID, user.PortalID)

                            Dim strUserDataFormat As String = DirectCast(Personalization.GetProfile(pInfo, "Aricie.PortalKeeper.SmartFile", Me.Name), String)
                            If String.IsNullOrEmpty(strUserDataFormat) AndAlso Not String.IsNullOrEmpty(Me._FormerName) Then
                                strUserDataFormat = DirectCast(Personalization.GetProfile(pInfo, "Aricie.PortalKeeper.SmartFile", Me._FormerName), String)
                                If Not String.IsNullOrEmpty(Me._FormerName) Then
                                    Personalization.SetProfile(pInfo, "Aricie.PortalKeeper.SmartFile", Me.Name, strUserDataFormat)
                                    Personalization.RemoveProfile(pInfo, "Aricie.PortalKeeper.SmartFile", Me._FormerName)
                                End If
                            End If
                            Dim userDataFormat As UserBotFormat
                            If String.IsNullOrEmpty(strUserDataFormat) Then
                                userDataFormat = New UserBotFormat()
                            Else
                                userDataFormat = ReflectionHelper.Deserialize(Of UserBotFormat)(strUserDataFormat)
                            End If

                            Dim userData As Byte() = DirectCast(Personalization.GetProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me.Name), Byte())
                            If userData.Length > 0 AndAlso Not String.IsNullOrEmpty(Me._FormerName) Then
                                userData = DirectCast(Personalization.GetProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me._FormerName), Byte())
                                If Not String.IsNullOrEmpty(Me._FormerName) Then
                                    Personalization.SetProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me.Name, userData)
                                    Personalization.RemoveProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me._FormerName)
                                End If
                            End If
                            If userData.Length > 0 Then
                                If userDataFormat.Encrypted Then
                                    Dim tempData As Byte() = Me.StorageSettings.Encryption.Decrypt(userData, Encoding.Unicode.GetBytes(GetSalt(user)))
                                    userData = tempData
                                End If
                                If userDataFormat.Compressed Then
                                    Dim tempData As Byte() = Decompress(userData, CompressionMethod.Gzip)
                                    userData = tempData
                                End If
                                userBot = ReflectionHelper.Deserialize(Of UserBotInfo)(userData)
                                ' on place le user bot dans le cache après avoir potentiellement complété ses paramètres avec ceux fournis par les paramètres du user bot
                                'CacheHelper.SetCacheDependant(Of UserBotInfo)(userBot, StaticInfo.KeyBucket.PKPCachingUserBotsDependency, TimeSpan.FromMinutes(10), Me.Name, user.UserID.ToString(CultureInfo.InvariantCulture))

                            End If

                        Case UserBotStorage.SmartFiles

                            Dim key As EntityKey = Me.GetUserBotKey(user)
                            'pid, PortalKeeperConfig.Instance.GetModuleName(), user.Username, "UserBots/" & Me.Name, "")
                            Dim sf As SmartFile(Of UserBotInfo) = SmartFile.LoadSmartFile(Of UserBotInfo)(key, Me.StorageSettings)
                            If sf IsNot Nothing AndAlso Me.StorageSettings.CheckSmartFile(sf) Then
                                userBot = sf.TypedValue
                            End If

                    End Select
                Catch ex As Exception
                    Dim newEx As New ApplicationException(String.Format("User Bot Failed to load from persistence storage for user {0}", user.Username), ex)
                    Exceptions.LogException(newEx)
                End Try


                If userBot Is Nothing Then
                    If createIfNull Then
                        userBot = GetNewUserBot()
                    End If
                Else
                    Me.SetDefaultParameters(userBot)
                End If

            End If

            Return userBot
        End Function


        Public Function GetNewUserBot() As UserBotInfo
            Dim userBot As New UserBotInfo
            userBot.Enabled = False
            ' ici on prend les paramètres nom et description du UserBot, pas du MasterBot
            userBot.Name = Name
            userBot.Decription = Decription
            Me.SetDefaultParameters(userBot)
            Return userBot
        End Function

        Public Sub SetUserBotInfo(ByVal user As UserInfo, ByVal pid As Integer, ByVal objUserBotInfo As UserBotInfo)

            Select Case Me.Storage
                Case UserBotStorage.Personalisation
                    Dim pc As New PersonalizationController
                    Dim pInfo As PersonalizationInfo = pc.LoadProfile(user.UserID, pid)
                    If objUserBotInfo IsNot Nothing Then
                        Me.SetDefaultParameters(objUserBotInfo)
                        Dim objUserBotFormat As New UserBotFormat

                        Dim userData As Byte() = ReflectionHelper.SerializeToBytes(objUserBotInfo, False)

                        If Me.StorageSettings.Compress Then
                            objUserBotFormat.Compressed = True
                            userData = Compress(userData, CompressionMethod.Gzip)
                        Else
                            objUserBotFormat.Compressed = False
                        End If

                        If Me.StorageSettings.Encrypt Then
                            objUserBotFormat.Encrypted = True
                            userData = Me.StorageSettings.Encryption.DoEncrypt(userData, Encoding.Unicode.GetBytes(GetSalt(user)))
                        Else
                            objUserBotFormat.Encrypted = False
                        End If

                        Dim strFormat As String = ReflectionHelper.Serialize(objUserBotFormat).OuterXml
                        Personalization.SetProfile(pInfo, "Aricie.PortalKeeper.SmartFile", Me.Name, strFormat)
                        Personalization.SetProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me.Name, userData)
                        pc.SaveProfile(pInfo)
                    Else
                        Personalization.SetProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me.Name, "")
                        Personalization.RemoveProfile(pInfo, "Aricie.PortalKeeper.UserBot", Me.Name)
                        pc.SaveProfile(pInfo)
                    End If
                Case UserBotStorage.SmartFiles
                    'Dim key As EntityKey = Me.GetUserBotKey(user, pid)
                    'Dim objSmartFile As New SmartFile(Of UserBotInfo)(key, objUserBotInfo, Me.StorageSettings, encrypter)
                    'SmartFile.SaveSmartFile(objSmartFile, Me.StorageSettings)
                    If objUserBotInfo IsNot Nothing Then
                        Me.SetDefaultParameters(objUserBotInfo)
                        Me.SaveSmartUserBot(user, objUserBotInfo)
                    Else
                        Me.DeleteSmartUserBot(user)
                    End If
            End Select

            CacheHelper.RemoveCache(Of Nullable(Of DateTime))(NextScheduleUserBotKey, user.UserID.ToString(CultureInfo.InvariantCulture), Me.Name)
            'RemoveCache(Of UserBotInfo)(Me.Name, user.UserID.ToString(CultureInfo.InvariantCulture))
        End Sub




        Private Sub SaveSmartUserBot(user As UserInfo, objUserBotInfo As UserBotInfo)
            Dim key As EntityKey = Me.GetUserBotKey(user)
            Dim objSmartFile As New SmartFile(Of UserBotInfo)(key, objUserBotInfo, Me.StorageSettings)
            SmartFile.SaveSmartFile(objSmartFile, Me.StorageSettings)
        End Sub

        Private Sub DeleteSmartUserBot(user As UserInfo)
            Dim key As EntityKey = Me.GetUserBotKey(user)
            SmartFile.DeleteSmartFile(Of UserBotInfo)(key, Me.StorageSettings)
        End Sub


        Private Function GetSampleBotKey(pid As Integer) As EntityKey
            Dim objSampleUser As UserInfo = New UserController().GetUser(pid, PortalInfo(pid).AdministratorId)
            Return GetUserBotKey(objSampleUser)
        End Function

        Public Function GetUserBotKey(user As UserInfo) As EntityKey
            Dim key As New EntityKey() With {
                               .PortalId = user.PortalID,
                               .Application = PortalKeeperConfig.Instance.GetModuleName(),
                               .UserName = user.Username,
                               .Entity = Me.Name,
                               .Field = ""
                               }
            Return key
        End Function

        <ConditionalVisible("HasEncrypter", False, True, UserBotStorage.Personalisation)>
        <ConditionalVisible("Storage", False, True, UserBotStorage.Personalisation)>
        <ActionButton(IconName.Refresh, IconOptions.Normal)>
        Public Sub SwitchToSmartFiles()
            If Me.Storage = UserBotStorage.Personalisation Then
                Dim shuffledUsers As IList(Of UserInfo) = New List(Of UserInfo)(GetPortalUsers(DnnContext.Current.Portal.PortalId))
                ShuffleList(Of UserInfo)(shuffledUsers)
                For Each objUser As UserInfo In shuffledUsers
                    Dim userBotInfo As UserBotInfo = Me.GetUserBotInfo(objUser, False)
                    If userBotInfo IsNot Nothing Then
                        SaveSmartUserBot(objUser, userBotInfo)
                    End If
                Next
            End If
        End Sub



#End Region

#Region "ISelector"

        Public Function GetSelector(ByVal propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function



        Public Function GetSelectorG(ByVal propertyName As String) As IList(Of BotInfo(Of TEngineEvent)) Implements ISelector(Of BotInfo(Of TEngineEvent)).GetSelectorG
            Return PortalKeeperConfig.Instance.GetBotFarm(Of TEngineEvent).Bots.Instances
        End Function

#End Region

#Region "Private methods"

        Private Function GetSalt(ByVal user As UserInfo) As String
            Return user.Email & "-"c & user.PortalID
        End Function

        Private Shared objRankingLock As New Object

        Private Shared Sub GetRankingAsync(objProbeAccess As UserBotProbeAccess)
            SyncLock objRankingLock
                Dim toReturn As List(Of ProbeRanking) = GetGlobal(Of List(Of ProbeRanking))(objProbeAccess.UserBotSettings.Name)
                If toReturn Is Nothing Then
                    toReturn = New List(Of ProbeRanking)
                    For Each objParamProb As ProbeSettings(Of TEngineEvent) In objProbeAccess.UserBotSettings.Probes.Instances
                        If objParamProb.Enabled Then
                            Dim toAdd As New ProbeRanking
                            toAdd.Name = objParamProb.Name
                            toAdd.Decription = objParamProb.Decription
                            For Each pid As Integer In PortalIds
                                Dim users As List(Of UserInfo) = GetPortalUsers(pid)
                                For Each objUser As UserInfo In users
                                    If objUser.Membership.Approved Then
                                        Dim objUserBotInfo As UserBotInfo = objProbeAccess.UserBotSettings.GetUserBotInfo(objUser, False)
                                        If objUserBotInfo IsNot Nothing AndAlso objUserBotInfo.Enabled Then
                                            Dim objProbeInstance As ProbeInstance = objParamProb.GetProbe(objUserBotInfo, objUser)
                                            If objProbeInstance IsNot Nothing Then
                                                toAdd.Items.Instances.Add(objProbeInstance)
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                            If toAdd.Items.Instances.Count > 0 Then
                                toAdd.Items.Instances.Sort()
                                If objParamProb.SortDirection = SortDirection.Descending Then
                                    toAdd.Items.Instances.Reverse()
                                End If
                                If toAdd.Items.Instances.Count > objParamProb.RankingsSize Then
                                    toAdd.Items.Instances = New SerializableList(Of ProbeInstance)(toAdd.Items.Instances.GetRange(0, objParamProb.RankingsSize))
                                End If
                            End If
                            If toAdd IsNot Nothing Then
                                toReturn.Add(toAdd)
                            End If
                        End If
                    Next
                    SetCache(Of List(Of ProbeRanking))(toReturn, TimeSpan.FromMinutes(60), objProbeAccess.UserBotSettings.Name)
                End If
            End SyncLock
        End Sub


        Private Sub SetDefaultParameters(userBot As UserBotInfo)
            For Each userParameter As UserVariableInfo In UserParameters.Instances
                Dim targetName As String = userParameter.Name
                Dim targetMode As UserParameterMode = userParameter.Mode

                Dim userBotParameterTarget = userBot.UserParameters.Instances.Find(Function(ovi) ovi.Name = targetName AndAlso ovi.Mode = targetMode)

                Dim parameterMatchesInUserBotSettings = userBotParameterTarget IsNot Nothing
                Dim userBotSettingsOverwritable As Boolean = userParameter.Override AndAlso ((Not userBot.NoOverride) OrElse userParameter.ForceOverride)

                If (Not parameterMatchesInUserBotSettings OrElse userBotSettingsOverwritable) Then
                    Dim value As Object = Nothing
                    Select Case userParameter.Mode
                        Case UserParameterMode.PropertyDefinition
                            Dim propDef As GeneralPropertyDefinition = userParameter.PropertyDefinition.Clone
                            If String.IsNullOrEmpty(propDef.DefaultValue) Then
                                If DefaultVars.TryGetValue(userParameter.Name, value) Then
                                    propDef.PropertyValue = value.ToString
                                Else
                                    Throw New Exception(String.Format("Missing UserParameter {0}", userParameter.Name))
                                End If
                            Else
                                propDef.PropertyValue = propDef.DefaultValue
                            End If
                            userBot.SetPropertyDefinition(userParameter.Name, propDef)
                        Case Else
                            If Not DefaultVars.TryGetValue(userParameter.Name, value) Then
                                Throw New Exception(String.Format("Missing UserParameter {0}", userParameter.Name))
                            Else
                                userBot.SetEntity(userParameter.Name, ReflectionHelper.CloneObject(value))
                            End If
                    End Select
                End If
            Next
            ' on ne veut que les paramètres activés au niveau du user bot
            Dim enabledList As New SimpleList(Of UserVariableInfo)
            For Each uvi In UserParameters.Instances
                If uvi.Enabled Then
                    enabledList.Instances.Add(uvi)
                End If
            Next
            userBot.UserParameters = enabledList
        End Sub


#End Region

#Region "Inner Classes"


        Public Class UserBotFormat


            Private _Encrypted As Boolean = True
            Private _Compressed As Boolean


            Public Property Encrypted() As Boolean
                Get
                    Return _Encrypted
                End Get
                Set(ByVal value As Boolean)
                    _Encrypted = value
                End Set
            End Property


            Public Property Compressed() As Boolean
                Get
                    Return _Compressed
                End Get
                Set(ByVal value As Boolean)
                    _Compressed = value
                End Set
            End Property

        End Class



        Private Class UserBotProbeAccess

            Public Sub New(objUserBotSettings As UserBotSettings(Of TEngineEvent))
                Me._UserBotSettings = objUserBotSettings

            End Sub

            Private _UserBotSettings As UserBotSettings(Of TEngineEvent)
            Public Property UserBotSettings() As UserBotSettings(Of TEngineEvent)
                Get
                    Return _UserBotSettings
                End Get
                Set(ByVal value As UserBotSettings(Of TEngineEvent))
                    _UserBotSettings = value
                End Set
            End Property


        End Class



        Private Class UserBotPersister

            Public Sub New(ByVal userSettings As UserBotSettings(Of TEngineEvent), ByVal objUser As UserInfo)
                Me._UserBotSettings = userSettings
                Me._User = objUser


            End Sub


            Private _UserBotSettings As UserBotSettings(Of TEngineEvent)


            Public Property UserBotSettings() As UserBotSettings(Of TEngineEvent)
                Get
                    Return _UserBotSettings
                End Get
                Set(ByVal value As UserBotSettings(Of TEngineEvent))
                    _UserBotSettings = value
                End Set
            End Property

            Private _User As UserInfo

            Public Property User() As UserInfo
                Get
                    Return _User
                End Get
                Set(ByVal value As UserInfo)
                    _User = value
                End Set
            End Property


            Private _UserBot As UserBotInfo

            Public Property UserBot() As UserBotInfo
                Get
                    Return _UserBot
                End Get
                Set(ByVal value As UserBotInfo)
                    _UserBot = value
                End Set
            End Property


            Public Sub InitBot(ByVal sender As Object, e As GenericEventArgs(Of BotRunContext(Of TEngineEvent)))
                Dim runContext = e.Item
                Dim objUserBotInfo As UserBotInfo = Me.UserBotSettings.GetUserBotInfo(Me.User, False)
                If objUserBotInfo.Enabled Then
                    Dim nextSchedule As DateTime = objUserBotInfo.BotHistory.GetNextSchedule(Me.UserBotSettings.Bot.BotSchedule)
                    If nextSchedule <= Now Then '.Add(Me.UserBotSettings.Bot.Schedule.Value) <= Now Then
                        runContext.Enabled = True
                        Me._UserBot = objUserBotInfo
                        runContext.History = Me._UserBot.BotHistory
                        runContext.UserParams = Me._UserBot.GetParameterValues(Me.User)
                        If Me.UserBotSettings.CachePlannedSchedule.Enabled Then
                            CacheHelper.RemoveCache(Of Nullable(Of DateTime))(NextScheduleUserBotKey, User.UserID.ToString(CultureInfo.InvariantCulture), Me.UserBotSettings.Name)
                        End If
                    Else
                        runContext.Enabled = False
                        If Me.UserBotSettings.CachePlannedSchedule.Enabled Then
                            CacheHelper.SetCache(Of Nullable(Of DateTime))(nextSchedule, Me.UserBotSettings.CachePlannedSchedule.Entity.Value, NextScheduleUserBotKey, User.UserID.ToString(CultureInfo.InvariantCulture), Me.UserBotSettings.Name)
                        End If
                    End If
                Else
                    runContext.Enabled = False
                    If UserBotSettings.CachePlannedSchedule.Enabled Then
                        CacheHelper.SetCache(Of Nullable(Of DateTime))(DateTime.MaxValue, Me.UserBotSettings.CachePlannedSchedule.Entity.Value, NextScheduleUserBotKey, User.UserID.ToString(CultureInfo.InvariantCulture), Me.UserBotSettings.Name)
                    End If
                End If
            End Sub
            Public Sub SaveBot(ByVal sender As Object, e As GenericEventArgs(Of BotRunContext(Of TEngineEvent)))
                Me._UserBot.BotHistory = e.Item.History
                Me._UserBotSettings.SetUserBotInfo(Me._User, Me._User.PortalID, Me._UserBot)
                If UserBotSettings.CachePlannedSchedule.Enabled Then
                    Dim nextSchedule As DateTime = Me.UserBot.BotHistory.GetNextSchedule(Me.UserBotSettings.Bot.BotSchedule)

                    CacheHelper.SetCache(Of Nullable(Of DateTime))(nextSchedule, Me.UserBotSettings.CachePlannedSchedule.Entity.Value, NextScheduleUserBotKey, User.UserID.ToString(CultureInfo.InvariantCulture), Me.UserBotSettings.Name)
                End If
            End Sub

        End Class

#End Region

    End Class
End Namespace