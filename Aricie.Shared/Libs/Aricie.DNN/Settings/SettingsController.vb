Imports Aricie.DNN.Services
Imports System.Globalization
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.Entities.Users
Imports Aricie.Services
Imports DotNetNuke.Services.Personalization
Imports System.Web
Imports System.IO
Imports System.Text
Imports DotNetNuke.Entities.Portals
Imports System.Reflection
Imports Aricie.ComponentModel
Imports Aricie.Text
Imports DotNetNuke.Services.Log.EventLog
Imports Newtonsoft.Json


Namespace Settings




    ''' <summary>
    ''' Controller class for settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Module SettingsController

        Private Const glbDependency As String = Constants.Cache.Dependency & "Settings"

        Private loaderLock As New Object

#Region "Personal Settings"

        ''' <summary>
        ''' returns personnal settings for a given user
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPersonalSettings(ByVal portalId As Integer, ByVal userId As Integer) As PersonalSettings
            'Dim strCache As String = Constants.Settings.GetUserKey(portalID, UserController.GetCurrentUserInfo.UserID)
            Dim toReturn As PersonalSettings = Nothing
            If DnnContext.Current.IsAuthenticated Then

                toReturn = GetPersonal(Of PersonalSettings)(portalId, userId.ToString(CultureInfo.InvariantCulture))
                If toReturn Is Nothing Then
                    Dim xmlProfile As String
                    Dim objPersonalizationController As New PersonalizationController

                    xmlProfile = CType(Personalization.GetProfile(objPersonalizationController.LoadProfile(userId, portalId), Constants.Settings.ProfileNamingContainer, Constants.Settings.ProfileKey), String)

                    If Not xmlProfile = "" Then
                        toReturn = ReflectionHelper.Deserialize(Of PersonalSettings)(xmlProfile)
                        SetPersonal(Of PersonalSettings)(toReturn, portalId, userId.ToString)
                    End If

                End If
            End If
            If toReturn Is Nothing Then
                toReturn = New PersonalSettings
            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Returns global personnal settings that can be defined on a portal
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPersonalSettings(ByVal portalId As Integer) As PersonalSettings
            'Dim strCache As String = Constants.Settings.GetUserKey(portalID, UserController.GetCurrentUserInfo.UserID)
            Dim toReturn As PersonalSettings = Nothing
            If DnnContext.Current.IsAuthenticated Then

                toReturn = GetPersonal(Of PersonalSettings)(portalId, UserController.GetCurrentUserInfo.UserID.ToString(CultureInfo.InvariantCulture))
                If toReturn Is Nothing Then
                    Dim xmlProfile As String

                    xmlProfile = CType(Personalization.GetProfile(Constants.Settings.ProfileNamingContainer, Constants.Settings.ProfileKey), String)

                    If Not xmlProfile = "" Then
                        toReturn = ReflectionHelper.Deserialize(Of PersonalSettings)(xmlProfile)
                        SetPersonal(Of PersonalSettings)(toReturn, portalId, UserController.GetCurrentUserInfo.UserID.ToString)
                    End If

                End If
            End If
            If toReturn Is Nothing Then
                toReturn = New PersonalSettings
            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Returns global personnal settings that can be defined on a portal and caches them
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPersonalSettings(ByVal portalId As Integer, ByVal useCache As Boolean) As PersonalSettings
            'Dim strCache As String = Constants.Settings.GetUserKey(portalID, UserController.GetCurrentUserInfo.UserID)
            Dim toReturn As New PersonalSettings
            If HttpContext.Current.Request.IsAuthenticated Then
                If useCache Then
                    toReturn = _
                        GetPersonal(Of PersonalSettings)(portalId, _
                                                           UserController.GetCurrentUserInfo.UserID.ToString( _
                                                                                                              CultureInfo. _
                                                                                                                 InvariantCulture))
                Else
                    toReturn = Nothing
                End If

                If toReturn Is Nothing Then
                    Dim xmlProfile As String

                    xmlProfile = _
                        CType( _
                            Personalization.GetProfile(Constants.Settings.ProfileNamingContainer, _
                                                        Constants.Settings.ProfileKey), String)

                    If Not xmlProfile = "" Then
                        toReturn = ReflectionHelper.Deserialize(Of PersonalSettings)(xmlProfile)
                    End If
                    If useCache Then
                        SetPersonal(Of PersonalSettings)(toReturn, portalId, _
                                                           UserController.GetCurrentUserInfo.UserID.ToString)
                    End If
                End If
            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Sets personnal settings for a given user
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="userId"></param>
        ''' <param name="personalSettings"></param>
        ''' <param name="setCache"></param>
        ''' <remarks></remarks>
        Public Sub SetPersonalSettings(ByVal portalId As Integer, ByVal userId As Integer, ByVal personalSettings As PersonalSettings, _
                                ByVal setCache As Boolean)
            If HttpContext.Current.Request.IsAuthenticated Then

                Dim xmlProfile As String = ReflectionHelper.Serialize(personalSettings).OuterXml
                Dim objPersonalizationController As New PersonalizationController

                Personalization.SetProfile(objPersonalizationController.LoadProfile(userId, portalId), Constants.Settings.ProfileNamingContainer, Constants.Settings.ProfileKey, xmlProfile)
                If setCache Then
                    SetPersonal(Of PersonalSettings)(personalSettings, portalId, userId.ToString(CultureInfo.InvariantCulture))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sets global personnal settings that can be defined on a portal
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <param name="personalSettings"></param>
        ''' <param name="setCache"></param>
        ''' <remarks></remarks>
        Public Sub SetPersonalSettings(ByVal portalId As Integer, ByVal personalSettings As PersonalSettings, _
                                        ByVal setCache As Boolean)
            If HttpContext.Current.Request.IsAuthenticated Then

                Dim xmlProfile As String = ReflectionHelper.Serialize(personalSettings).OuterXml
                Personalization.SetProfile(Constants.Settings.ProfileNamingContainer, Constants.Settings.ProfileKey, _
                                            xmlProfile)
                If setCache Then
                    SetPersonal(Of PersonalSettings)(personalSettings, portalId, _
                                                       UserController.GetCurrentUserInfo.UserID.ToString( _
                                                                                                          CultureInfo. _
                                                                                                             InvariantCulture))
                End If
            End If
        End Sub


#End Region


#Region "File Settings"

       Private  writingJson As Boolean = False


        ''' <summary>
        ''' Loads settings as custom class from a file
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileName"></param>
        ''' <param name="useCache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadFileSettings(Of T As New)(ByVal fileName As String, ByVal useCache As Boolean) As T
            Return LoadFileSettings(Of T)(fileName, useCache, False)
        End Function

        ''' <summary>
        ''' returns the default filename for settings by adding .default before the extension
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        ''' <remarks>If there is no extension, just add .default</remarks>
        Public Function GetDefaultFileName(ByVal filename As String) As String
            Dim dotIdx As Integer = filename.LastIndexOf("."c)
            If dotIdx <> -1 Then
                Return filename.Substring(0, dotIdx) & ".Default" & filename.Substring(dotIdx)
            Else
                Return filename & ".Default"
            End If

        End Function

        ''' <summary>
        ''' Loads settings as custom class from a file
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileName"></param>
        ''' <param name="useCache"></param>
        ''' <param name="useBinarySnapShot"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadFileSettings(Of T As New)(ByVal fileName As String, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As T

            Return DirectCast(LoadFileSettings(fileName, GetType(T), useCache, useBinarySnapShot), T)
        End Function

        Public Function LoadFileSettings(Of T As New)(ByVal fileName As String, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean,ByVal  migrateToJson As Boolean) As T

            Return DirectCast(LoadFileSettings(fileName, GetType(T), useCache, useBinarySnapShot, migrateToJson), T)
        End Function

        Public Function LoadFileSettings(ByVal fileName As String, ByVal targetType As Type, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As Object
            Return LoadFileSettings(fileName, targetType, useCache, useBinarySnapShot, False)
        End Function


        ''' <summary>
        ''' Loads settings as object from a file
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="targetType"></param>
        ''' <param name="useCache"></param>
        ''' <param name="useBinarySnapShot"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadFileSettings(ByVal fileName As String, ByVal targetType As Type, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean,ByVal  migrateToJson As Boolean) As Object
            Dim toReturn As Object = Nothing
            If useCache Then
                toReturn = CacheHelper.GetCache(fileName)
            End If
            If toReturn Is Nothing Then
                Dim fileExists As Boolean
                If useBinarySnapShot Then
                    Dim snapShotName As String = GetSnapShotName(fileName)
                    If System.IO.File.Exists(snapShotName) Then
                        fileExists = True
                        Using reader As FileStream = System.IO.File.OpenRead(snapShotName)
                            Try
                                toReturn = ReflectionHelper.Instance.BinaryFormatter.Deserialize(reader)
                            Catch ex As Exception
                                AsyncLogger.Instance.AddException(ex)
                            Finally
                                reader.Close()
                            End Try
                        End Using
                    End If
                End If

                If toReturn Is Nothing Then
                    Dim objDir As New DirectoryInfo(Path.GetDirectoryName(fileName))
                    If objDir.Exists Then
                        Try
                            'SyncLock loaderLock
                            If Not File.Exists(fileName) Then
                                Dim defaultFileName As String = GetDefaultFileName(fileName)
                                If File.Exists(defaultFileName) Then
                                    File.Copy(defaultFileName, fileName)
                                    Dim objFileInfo As New System.IO.FileInfo(fileName)
                                    If objFileInfo.IsReadOnly Then
                                        objFileInfo.IsReadOnly = False
                                    End If
                                End If
                            End If




                            If File.Exists(fileName) Then
                                fileExists = True

                                Dim jsonFileName As String = GetJsonFileName(fileName)
                                If migrateToJson AndAlso File.Exists(jsonFileName) Then

                                    'File.Delete(jsonFileName)

                                    fileName = jsonFileName
                                    Using fileStream As FileStream = File.OpenRead(fileName)
                                        Using objReader As New StreamReader(fileStream, Encoding.UTF8)
                                            Dim settings As New JsonSerializerSettings() With {.TypeNameHandling = TypeNameHandling.All}
                                            settings.SetWriteOnlySettings()
                                            Dim fileContent As String = objReader.ReadToEnd()
#If DEBUG Then
                                            Dim objStep As New StepInfo("Aricie.Config", "Loading json Config",
                                                                WorkingPhase.InProgress, False, True, -1, DnnContext.Instance.FlowId, New KeyValuePair(Of String, String)("FileName", fileName))
                                            PerformanceLogger.Instance.AddDebugInfo(objStep)
#End If
                                            toReturn = JsonConvert.DeserializeObject(fileContent, targetType, settings)
#If DEBUG Then
                                            Dim objStepEnd As New StepInfo("Aricie.Config", "End Loading json Config",
                                                                    WorkingPhase.InProgress, False, True, -1, DnnContext.Instance.FlowId, New KeyValuePair(Of String, String)("FileName", fileName))
                                            PerformanceLogger.Instance.AddDebugInfo(objStepEnd)
#End If
                                        End Using
                                    End Using





                                Else

                                    Using fileStream As FileStream = File.OpenRead(fileName)
                                        Using objReader As New StreamReader(fileStream)

#If DEBUG Then

                                            Dim objStep As New StepInfo("Aricie.Config", "Loading XML Config",
                                                WorkingPhase.InProgress, False, True, -1, DnnContext.Instance.FlowId, New KeyValuePair(Of String, String)("FileName", fileName))
                                            PerformanceLogger.Instance.AddDebugInfo(objStep)

#End If




                                            toReturn = ReflectionHelper.Deserialize(targetType, objReader)


#If DEBUG Then

                                            Dim objStepEnd As New StepInfo("Aricie.Config", "End Loading XML Config",
                                                WorkingPhase.InProgress, False, True, -1, DnnContext.Instance.FlowId, New KeyValuePair(Of String, String)("FileName", fileName))
                                            PerformanceLogger.Instance.AddDebugInfo(objStepEnd)

#End If



                                        End Using
                                    End Using

                                    If migrateToJson AndAlso Not File.Exists(jsonFileName) Then
                                        If WriteJson(jsonFileName, toReturn) Then
                                            fileName = jsonFileName
                                        End If
                                    End If

                                End If

                            End If

                        Catch ex As Exception
                            AsyncLogger.Instance.AddException(ex)
                        End Try






                    End If
                End If

                If toReturn Is Nothing Then
                    fileExists = False
                    toReturn = Activator.CreateInstance(targetType)
                End If
                If useCache Then
                    If Not fileExists Then
                        CacheHelper.SetCache(fileName, toReturn, Constants.Cache.GlobalExpiration)
                    Else
                        CacheHelper.SetCacheDependant(fileName, toReturn, Constants.Cache.NoExpiration, fileName)
                    End If
                End If
            End If

            Return toReturn
        End Function


        Private Function WriteJson(jsonFileName As String, objToWrite As Object) As Boolean
            If  Not writingJson Then

                Try
                    writingJson = True
                    Dim settings As New JsonSerializerSettings() With {.TypeNameHandling = TypeNameHandling.All}
                    settings.SetWriteOnlySettings()
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                    'Aricie.Common.CallDebuggerBreak()

                    Dim fileContent As String = JsonConvert.SerializeObject(objToWrite, Formatting.Indented, settings)
                    File.WriteAllText(jsonFileName, fileContent, Encoding.UTF8)

                    Return True


                Catch ex As Exception
                    ExceptionHelper.LogException(ex)
                    Return False
                Finally
                    writingJson = False
                End Try


            End If


        End Function

        ''' <summary>
        ''' Saves settings to a file
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileName"></param>
        ''' <param name="settings"></param>
        ''' <remarks></remarks>
        Public Sub SaveFileSettings(Of T)(ByVal fileName As String, ByVal settings As T)
            SaveFileSettings(Of T)(fileName, settings, False)
        End Sub

        ''' <summary>
        ''' Returns filename with ".bin" at the end
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSnapShotName(ByVal fileName As String) As String
            Return fileName & ".bin"
        End Function

        ''' <summary>
        ''' Saves settings to a file
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileName"></param>
        ''' <param name="settings"></param>
        ''' <param name="createBinarySnapShot"></param>
        ''' <remarks></remarks>
        Public Sub SaveFileSettings(Of T)(ByVal fileName As String, ByVal settings As T, ByVal createBinarySnapShot As Boolean)
            SaveFileSettings(fileName, settings, createBinarySnapShot, 0)
        End Sub


      



        ''' <summary>
        ''' Saves settings to a file
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileName"></param>
        ''' <param name="settings"></param>
        ''' <param name="createBinarySnapShot"></param>
        ''' <param name="backupsNb"></param>
        ''' <remarks></remarks>
        Public Sub SaveFileSettings(Of T)(ByVal fileName As String, ByVal settings As T, ByVal createBinarySnapShot As Boolean, ByVal backupsNb As Integer)
            Dim objDir As New DirectoryInfo(Path.GetDirectoryName(fileName))
            If Not objDir.Exists Then
                objDir.Create()
            End If
            Dim saveFile As New System.IO.FileInfo(fileName)

            If saveFile.Exists AndAlso backupsNb > 0 Then
                Dim backupRootDir As New DirectoryInfo(objDir.FullName.TrimEnd("\"c) & "\ConfigBackups")
                If Not backupRootDir.Exists Then
                    backupRootDir.Create()
                End If
                SyncLock loaderLock
                    Dim newBackupFileName As String = backupRootDir.FullName.TrimEnd("\"c) & "\"c & saveFile.LastWriteTime.ToString("yyyy-MM-dd_HH-mm-ss_") & Path.GetFileName(fileName)
                    File.Copy(fileName, newBackupFileName, True)
                    Dim existingBackupFiles As System.IO.FileInfo() = backupRootDir.GetFiles()
                    If existingBackupFiles.Length >= backupsNb Then

                        Array.Sort(Of System.IO.FileInfo)(existingBackupFiles, New Aricie.Business.Filters.SimpleComparer(Of System.IO.FileInfo)("CreationTime", System.ComponentModel.ListSortDirection.Ascending))
                        For i As Integer = 1 To existingBackupFiles.Length - backupsNb
                            If existingBackupFiles(i - 1).IsReadOnly Then
                                existingBackupFiles(i - 1).IsReadOnly = False
                            End If
                            File.Delete(existingBackupFiles(i - 1).FullName)
                        Next
                    End If
                End SyncLock
            End If

            SyncLock loaderLock
                If saveFile.Exists Then
                    If saveFile.IsReadOnly Then
                        saveFile.IsReadOnly = False
                    End If
                    saveFile.Delete()
                End If
                If settings IsNot Nothing Then

                    Dim jsonFileName As String = GetJsonFileName(fileName)
                    If File.Exists(jsonFileName) Then
                        File.Delete(jsonFileName)
                    End If

                    Using writer As New StreamWriter(fileName, False, New UTF8Encoding)
                        ReflectionHelper.Serialize(settings, True, DirectCast(writer, TextWriter))
                    End Using

                End If
            End SyncLock

            CacheHelper.RemoveCache(fileName)
            Dim snapShotName As String = GetSnapShotName(fileName)

            If System.IO.File.Exists(snapShotName) Then
                Dim objFileInfo As New System.IO.FileInfo(snapShotName)
                If objFileInfo.IsReadOnly Then
                    objFileInfo.IsReadOnly = False
                End If
                System.IO.File.Delete(snapShotName)
            End If
            If createBinarySnapShot Then
                Using writer As FileStream = System.IO.File.OpenWrite(snapShotName)
                    ReflectionHelper.Instance.BinaryFormatter.Serialize(writer, settings)
                End Using
            End If

        End Sub

        Private Function GetJsonFileName(fileName As String ) As String
            Return fileName & ".json.resources"
        End Function

        ''' <summary>
        ''' Saves settings to a file
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="settings"></param>
        ''' <param name="createBinarySnapShot"></param>
        ''' <param name="logException"></param>
        ''' <remarks></remarks>
        <Obsolete("user overloads without exception handling")> _
        Public Sub SaveFileSettings(ByVal fileName As String, ByVal settings As Object, ByVal createBinarySnapShot As Boolean, ByVal logException As Boolean)
            SaveFileSettings(fileName, settings, createBinarySnapShot, 0)
        End Sub

        ''' <summary>
        ''' Saves settings to a file
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <param name="settings"></param>
        ''' <param name="createBinarySnapShot"></param>
        ''' <param name="logException"></param>
        ''' <param name="backupsNb"></param>
        ''' <remarks></remarks>
        <Obsolete("user overloads without exception handling")> _
        Public Sub SaveFileSettings(ByVal fileName As String, ByVal settings As Object, ByVal createBinarySnapShot As Boolean, ByVal logException As Boolean, ByVal backupsNb As Integer)
            SaveFileSettings(fileName, settings, createBinarySnapShot, backupsNb)
        End Sub

        Public Function GetBackups(filename As String) As IList(Of System.IO.FileInfo)
            Dim toReturn As New List(Of System.IO.FileInfo)
            Dim objDir As New DirectoryInfo(Path.GetDirectoryName(filename))
            Dim backupRootDir As New DirectoryInfo(objDir.FullName.TrimEnd("\"c) & "\ConfigBackups")
            If backupRootDir.Exists Then
                Dim existingBackupFiles As System.IO.FileInfo() = backupRootDir.GetFiles()
                toReturn.AddRange(existingBackupFiles)
            End If
            Return toReturn
        End Function

        Public Function RestoreBackup(filename As String, backupname As String) As Boolean
            Dim objDir As New DirectoryInfo(Path.GetDirectoryName(filename))
            Dim backupRootDir As New DirectoryInfo(objDir.FullName.TrimEnd("\"c) & "\ConfigBackups")
            If backupRootDir.Exists Then
                Dim existingBackupFiles As System.IO.FileInfo() = backupRootDir.GetFiles()
                For Each existingBackupFile As System.IO.FileInfo In existingBackupFiles
                    If existingBackupFile.Name = backupname Then
                        System.IO.File.Copy(existingBackupFile.FullName, filename, True)
                        Return True
                    End If
                Next
            End If
            Return False
        End Function




#End Region

#Region "Generic Settings"

        ''' <summary>
        ''' Renvoie les settings de type T
        ''' </summary>
        ''' <typeparam name="T">Type des settings</typeparam>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeid">Identifiant lié à la portée des settings(portalid,moduleid ..)</param>
        ''' <returns>Settings de type T</returns>
        ''' <remarks></remarks>
        Public Function GetModuleSettings(Of T As {Class})(ByVal scope As SettingsScope, ByVal scopeid As Integer) As T
            Return GetModuleSettings(Of T)(scope, scopeid, Nothing, True)
        End Function

        ''' <summary>
        ''' Fixes issues with settings saved in previous versions of DNN
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="scope"></param>
        ''' <param name="scopeid"></param>
        ''' <param name="moduleSettings"></param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FixOldModuleSettings(Of T As {Class})(ByVal scope As SettingsScope, _
                                                             ByVal scopeid As Integer, _
                                                             ByVal moduleSettings As Hashtable, _
                                                             ByVal ParamArray args() As String) As T

            Dim toReturn As T = Nothing
            Try
                Dim strKey As String
                Dim xmlGeneralSettings As String

                strKey = Constants.GetKeyDotNet(Of T)(args)
                xmlGeneralSettings = FetchFromModuleSettings(scope, scopeid, strKey, moduleSettings)

                If xmlGeneralSettings = "" Then

                    strKey = Constants.GetKey3(Of T)(args)
                    xmlGeneralSettings = FetchFromModuleSettings(scope, scopeid, strKey, moduleSettings)
                    If xmlGeneralSettings = "" Then

                        strKey = Constants.GetKey2(Of T)()
                        'Il faut vérifier la fonction GetKeyMw, ce serait-elle qui poserait problème
                        xmlGeneralSettings = FetchFromModuleSettings(scope, scopeid, strKey, moduleSettings)

                        'si on ne retrouve toujours pas la clé
                        ' ''on remonte dans les fonctions qui cherchent des keys dans l'historique
                        'If xmlGeneralSettings = "" Then
                        '    ''celle-ci est la dernière version trouvée dans l'historique
                        '    strKey = Constants.GetKey1(Of T)()
                        '    Dim strTerminator As String = Constants.Settings.GetXMLSettingsTerminator(Of T)()
                        '    xmlGeneralSettings = FetchFromModuleSettings(scope, scopeid, strKey, strTerminator, moduleSettings)

                        'End If

                    End If
                End If
                If xmlGeneralSettings <> "" Then

                    Dim endTagPosition As Integer = EvalEndOfValidXMLString(Of T)(xmlGeneralSettings)
                    If endTagPosition > 0 Then
                        ' Nettoyage éventuel du xml récupéré
                        xmlGeneralSettings = xmlGeneralSettings.Substring(0, endTagPosition).Trim
                    End If

                    toReturn = ReflectionHelper.Deserialize(Of T)(xmlGeneralSettings)
                    If toReturn IsNot Nothing Then
                        SetModuleSettings(Of T)(scope, scopeid, toReturn, False)
                        DeleteFromModuleSettings(scope, scopeid, strKey, moduleSettings)
                    End If
                End If
            Catch ex As Exception
                Aricie.Services.ExceptionHelper.LogException(ex)
            End Try

            Return toReturn

        End Function

        ''' <summary>
        ''' Renvoie les settings de type T pour un module donné
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="scope"></param>
        ''' <param name="scopeid"></param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModuleSettingsKey(Of T As {Class})(ByVal scope As SettingsScope, _
                                                             ByVal scopeid As Integer, _
                                                             ByVal ParamArray args() As String) As String
            Dim newArgs As String() = GetKeyArgs(Of T)(scope, scopeid, args)
            Return Constants.GetKey(Of T)(newArgs)
        End Function

        ''' <summary>
        ''' Returns the key built from parameters for cache usage
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="scope"></param>
        ''' <param name="scopeid"></param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetKeyArgs(Of T As {Class})(ByVal scope As SettingsScope, _
                                                             ByVal scopeid As Integer, _
                                                             ByVal ParamArray args() As String) As String()
            Dim newArgs As String() = {}
            Dim tempList As New List(Of String)(args)
            tempList.Add(scope.ToString)
            tempList.Add(scopeid.ToString(CultureInfo.InvariantCulture))
            newArgs = tempList.ToArray
            Return newArgs
        End Function

        ''' <summary>
        ''' Renvoie les settings de type T
        ''' </summary>
        ''' <typeparam name="T">Type des settings</typeparam>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeid">Identifiant lié à la portée des settings(portalid,moduleid ..)</param>
        ''' <param name="moduleSettings">Settings à utiliser</param>
        ''' <param name="useCache">True pour utiliser le cache(valeur par défaut), false sinon</param>
        ''' <param name="args">paramètres additionnels, permettant de qualifier l'objet à retourner</param>
        ''' <returns>Settings de type T</returns>
        ''' <remarks></remarks>
        Public Function GetModuleSettings(Of T As {Class})(ByVal scope As SettingsScope, _
                                                             ByVal scopeid As Integer, _
                                                             ByVal moduleSettings As Hashtable, _
                                                             ByVal useCache As Boolean, _
                                                             ByVal ParamArray args() As String) As T

            Dim toReturn As T = Nothing
            Dim newArgs As String() = {}

            If useCache Then
                newArgs = GetKeyArgs(Of T)(scope, scopeid, args)
                toReturn = GetGlobal(Of T)(newArgs)
            End If

            If toReturn Is Nothing Then

                Dim strKey As String
                strKey = Constants.GetKey(Of T)(args)

                Dim xmlGeneralSettings As String = FetchFromModuleSettings(scope, scopeid, strKey, moduleSettings)

                'todo: remove after the new key has been largely adopted
                If xmlGeneralSettings = "" Then
                    toReturn = FixOldModuleSettings(Of T)(scope, scopeid, moduleSettings, args)
                Else
                    Try
                        toReturn = ReflectionHelper.Deserialize(Of T)(xmlGeneralSettings)
                    Catch ex As Exception
                        Aricie.Services.ExceptionHelper.LogException(ex)
                        toReturn = Nothing
                    End Try

                End If

                If toReturn Is Nothing Then
                    toReturn = Activator.CreateInstance(Of T)()
                ElseIf useCache Then
                    SetCacheDependant(Of T)(toReturn, glbDependency, TimeSpan.FromMinutes(60), newArgs)
                End If


            End If
            Return toReturn
        End Function


        ''' <summary>
        ''' Enregistre les settings
        ''' </summary>
        ''' <typeparam name="T">Type des settings</typeparam>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeID">Identifiant lié à la portée des settings(portalid,moduleid ..)</param>
        ''' <param name="settings">Settings à utiliser</param>
        ''' <remarks></remarks>
        Public Sub SetModuleSettings(Of T As {Class})(ByVal scope As SettingsScope, ByVal scopeId As Integer, _
                                                        ByVal settings As T)


            SetModuleSettings(Of T)(scope, scopeId, settings, True)

        End Sub


        ''' <summary>
        ''' Enregistre les settings
        ''' </summary>
        ''' <typeparam name="T">Type des settings</typeparam>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeID">Identifiant lié à la portée des settings(portalid,moduleid ..)</param>
        ''' <param name="settings">Settings à utiliser</param>
        ''' <param name="setCache">True pour mettre en cache les settings(valeur par défaut),false sinon</param>
        ''' <param name="args">Paramètres additionnel pour qualifier les settings</param>
        ''' <remarks></remarks>
        Public Sub SetModuleSettings(Of T As {Class})(ByVal scope As SettingsScope, ByVal scopeId As Integer, _
                                                        ByVal settings As T, ByVal setCache As Boolean, _
                                                        ByVal ParamArray args() As String)
            Dim strKey As String = Constants.GetKey(Of T)(args)

            DeleteFromModuleSettings(scope, scopeId, strKey)

            Dim xmlGeneralSettings As String = ReflectionHelper.Serialize(settings).OuterXml
            SaveToModuleSettings(scope, scopeId, strKey, xmlGeneralSettings)
            If setCache Then
                Dim newArgs As String() = {}
                newArgs = GetKeyArgs(Of T)(scope, scopeId, args)
                SetCacheDependant(Of T)(settings, glbDependency, TimeSpan.FromMinutes(60), newArgs)
                'Else
                'CacheHelper.ClearCache(scopeID)
            End If

        End Sub


#End Region

#Region "General Methods"
        ''' <summary>
        ''' Cette fonction vérifie qu'il n'y a pas deux fois la balise de fermeture de WrapperSettings
        ''' Cette balise indique la fin des informations utiles
        ''' </summary>
        ''' <param name="xmlSettings"></param>
        ''' <returns>
        ''' il retourne -1 si c'est correct
        ''' sinon
        ''' il retourne la position où la balise se finit
        ''' </returns>
        ''' <remarks></remarks>

        <Obsolete("This function is deprecated ; please use evalEndOfValidXMLString instead of")> _
        Private Function VerifyTagsXML(Of T)(ByVal xmlSettings As String) As Integer

            Dim startPosition As Integer = xmlSettings.IndexOf("?>", System.StringComparison.Ordinal)
            Dim posStartTag As Integer = xmlSettings.IndexOf("<", startPosition, System.StringComparison.Ordinal) + 1
            'posStartTag est maintenant placé sur la première position de la balise de start
            Dim endPosStartTag As Integer = 0
            endPosStartTag = xmlSettings.IndexOf(" ", posStartTag, System.StringComparison.Ordinal)
            If endPosStartTag > xmlSettings.IndexOf(">", posStartTag, System.StringComparison.Ordinal) Then
                endPosStartTag = xmlSettings.IndexOf(">", posStartTag, System.StringComparison.Ordinal)
            End If
            Dim tagSize As Integer = endPosStartTag - posStartTag

            'il faut obtenir le nom de la balise
            Dim tagName As String = xmlSettings.Substring(posStartTag, tagSize)


            Dim strEndTag As String = "</" & tagName & ">"
            Dim position As Integer = xmlSettings.IndexOf(strEndTag, System.StringComparison.Ordinal)
            If (position + tagSize + 3) < xmlSettings.Length() Then
                'il y a des informations après la balise de fermeture 
                Return position + tagSize + 3
            Else
                Return -1
            End If

        End Function


        ''' <summary>
        ''' This function determines end of valid xml content
        ''' </summary>
        ''' <param name="xmlSettings"></param>
        ''' <returns>
        ''' Return position of the latest char of the valid xml
        ''' Return -1 if string doesn't contains valid xml string
        ''' </returns>
        ''' <remarks></remarks>

        Private Function EvalEndOfValidXMLString(Of T)(ByVal xmlSettings As String) As Integer

            Dim toReturn As Integer = -1

            Dim startPosition As Integer = xmlSettings.IndexOf("?>", System.StringComparison.Ordinal)
            If startPosition <> -1 Then
                Dim posStartTag As Integer = xmlSettings.IndexOf("<", startPosition, System.StringComparison.Ordinal) + 1
                ' PosStartTag est maintenant placé sur la première position de la balise de start
                Dim endPosStartTag As Integer = 0
                endPosStartTag = xmlSettings.IndexOf(" ", posStartTag, System.StringComparison.Ordinal)
                If endPosStartTag > xmlSettings.IndexOf(">", posStartTag, System.StringComparison.Ordinal) Then
                    endPosStartTag = xmlSettings.IndexOf(">", posStartTag, System.StringComparison.Ordinal)
                End If
                Dim tagSize As Integer = endPosStartTag - posStartTag

                ' Il faut obtenir le nom de la balise
                Dim tagName As String = xmlSettings.Substring(posStartTag, tagSize)

                If tagName <> "" Then
                    Dim strEndTag As String = "</" & tagName & ">"
                    Dim position As Integer = xmlSettings.IndexOf(strEndTag, System.StringComparison.Ordinal)
                    If (position + tagSize + 3) < xmlSettings.Length() Then
                        ' Il y a des informations après la balise de fermeture 
                        toReturn = position + tagSize + 3
                    Else
                        toReturn = xmlSettings.Length
                    End If
                End If
            End If

            Return toReturn

        End Function


        <Obsolete("Use Overload without a terminator argument")> _
        Public Function FetchFromModuleSettings(ByVal scope As SettingsScope, _
            ByVal scopeid As Integer, ByVal key As String, ByVal terminator As String, _
             Optional ByRef settings As Hashtable = Nothing) As String

            Dim toReturn As String = ""
            If settings Is Nothing Then
                Select Case scope
                    Case SettingsScope.ModuleSettings
                        settings = ModuleController.GetModuleSettings(scopeid)
                    Case SettingsScope.TabModuleSettings
                        settings = ModuleController.GetTabModuleSettings(scopeid)
                End Select
            End If
            If settings.ContainsKey(key) Then
                toReturn = CType(settings(key), String)

                If toReturn <> "" And Not toReturn.EndsWith(terminator) Then
                    Dim j As Integer = 2
                    Dim tempStr As String
                    Do
                        tempStr = ""
                        If settings.ContainsKey(key & "-" & j) Then
                            tempStr = CType(settings(key & "-" & j), String)
                            toReturn += tempStr
                            j += 1
                        End If
                    Loop Until tempStr.EndsWith(terminator) Or tempStr = ""
                End If

            End If
            Return toReturn

        End Function

        ''' <summary>
        ''' Recupere la valeur stockée dans les settings
        ''' </summary>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeid">Identifiant lié à la portée des settings(portalid,moduleid ..)</param>
        ''' <param name="key">Cle des settings</param>
        ''' <param name="settings">Settings à utiliser</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FetchFromModuleSettings(ByVal scope As SettingsScope, _
                                                 ByVal scopeid As Integer, ByVal key As String, _
                                                 Optional ByRef settings As Hashtable = Nothing) As String

            Dim toReturn As New StringBuilder()
            If settings Is Nothing Then
                settings = FetchSettings(scope, scopeid)
            End If
            If settings.ContainsKey(key) Then
                toReturn.Append(DirectCast(settings(key), String))

                Dim j As Integer = 2
                Dim tempStr As String
                ' Do
                While settings.ContainsKey(key & "-" & j)
                    tempStr = DirectCast(settings(key & "-" & j), String)
                    toReturn.Append(tempStr)
                    j += 1
                End While

            End If
            Return toReturn.ToString()

        End Function

        ''' <summary>
        ''' Sauvegarde les settings du module en base
        ''' </summary>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeId">Identifiant lié à la portée des settings(portalid,moduleid ...)</param>
        ''' <param name="key">Cle des settings</param>
        ''' <param name="strXmlToSave">Xml a sauvegarder</param>
        ''' <remarks></remarks>
        Public Sub SaveToModuleSettings(ByVal scope As SettingsScope, ByVal scopeId As Integer, ByVal key As String, _
                                         ByVal strXmlToSave As String)
            DeleteFromModuleSettings(scope, scopeId, key, Nothing)
            'todo: will not work with petapoco, no workaround unless maybe they start supporting system transaction scope
            Using dbt As System.Data.Common.DbTransaction = DotNetNuke.Data.DataProvider.Instance.GetTransaction()

                Dim nbMaxChars As Integer
                Select Case scope
                    Case SettingsScope.HostSettings
                        nbMaxChars = 250
                    Case Else
                        nbMaxChars = 2000
                End Select

                If strXmlToSave.Length < nbMaxChars Then
                    UpdateSettings(scope, scopeId, key, strXmlToSave)
                    DeleteSettings(scope, scopeId, key & "-2")
                Else
                    UpdateSettings(scope, scopeId, key, strXmlToSave.Substring(0, nbMaxChars))
                    Dim nbRows As Integer = Convert.ToInt32(Math.Ceiling(strXmlToSave.Length / nbMaxChars))
                    Dim j As Integer
                    Dim tempStrXml As String
                    For j = 2 To nbRows
                        If j = nbRows Then
                            tempStrXml = strXmlToSave.Substring((j - 1) * nbMaxChars)
                        Else
                            tempStrXml = strXmlToSave.Substring((j - 1) * nbMaxChars, nbMaxChars)
                        End If
                        UpdateSettings(scope, scopeId, key & "-" & j, tempStrXml)

                    Next
                    'TODO : A supprimer une fois que le soucis de récupérer 
                    DeleteSettings(scope, scopeId, key & "-" & j + 1)
                End If
                ' Suppression du cache
                'DotNetNuke.Common.Utilities.DataCache.RemoveCache(String.Format("PortalSettings{0}", PortalId))

                'tout s 'est bien passé, on confirme la transaction
                DotNetNuke.Data.DataProvider.Instance.CommitTransaction(dbt)
            End Using

        End Sub

        ''' <summary>
        ''' Efface une entrée des settings
        ''' </summary>
        ''' <param name="scope">Portée des settings</param>
        ''' <param name="scopeId">Identifiant lié à la portée des settings(portalid,moduleid ...)</param>
        ''' <param name="key">Clé à supprimer</param>
        ''' <param name="Settings">Settings à utiliser</param>
        ''' <remarks></remarks>
        Public Sub DeleteFromModuleSettings(ByVal scope As SettingsScope, ByVal scopeId As Integer, ByVal key As String, _
                                             Optional ByRef settings As Hashtable = Nothing)

            Using dbt As System.Data.Common.DbTransaction = DotNetNuke.Data.DataProvider.Instance.GetTransaction()

                If settings Is Nothing Then
                    settings = FetchSettings(scope, scopeId)
                End If
                If settings.ContainsKey(key) Then
                    Dim strObject As String = CType(settings(key), String)
                    'on supprime le 1er segment de settings
                    DeleteSettings(scope, scopeId, key)
                    settings.Remove(key)

                    Dim j As Integer = 2
                    While settings.ContainsKey(key & "-" & j)
                        'on supprime tous les segments de settings suivants
                        DeleteSettings(scope, scopeId, key & "-" & j)
                        settings.Remove(key & "-" & j)
                        j += 1
                    End While
                End If

                ' tout s'est bien passé, on confirme la transaction
                DotNetNuke.Data.DataProvider.Instance.CommitTransaction(dbt)
            End Using

        End Sub

        Public Function FetchSettings(ByVal scope As SettingsScope, ByVal scopeId As Integer) As Hashtable
            Dim toReturn As Hashtable = Nothing
            Select Case scope
                Case SettingsScope.ModuleSettings
                    toReturn = ModuleController.GetModuleSettings(scopeId)
                Case SettingsScope.TabModuleSettings
                    toReturn = ModuleController.GetTabModuleSettings(scopeId)
                Case SettingsScope.PortalSettings
                    toReturn = PortalSettings.GetSiteSettings(scopeId)
                Case SettingsScope.HostSettings
                    Select Case NukeHelper.DnnVersion.Major
                        Case Is >= 6
                            toReturn = New Hashtable()
                            Dim typeHostController As Type = ReflectionHelper.CreateType("DotNetNuke.Entities.Controllers.HostController, DotNetNuke")
                            Dim hostControllerInstance = typeHostController.GetProperty("Instance", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy).GetValue(Nothing, Nothing)
                            Dim hostSettingsMethod As MethodInfo = DirectCast(ReflectionHelper.GetMember(typeHostController, "GetSettingsDictionary"), MethodInfo)
                            Dim hostSettings As Dictionary(Of String, String) = DirectCast(hostSettingsMethod.Invoke(hostControllerInstance, Nothing), Dictionary(Of String, String))
                            For Each item As KeyValuePair(Of String, String) In hostSettings
                                toReturn.Add(item.Key, item.Value)
                            Next
                            'Case Is = 5
                            '    toReturn = New Hashtable()
                            '    Dim hostCtlrType As Type
                            '    hostCtlrType = ReflectionHelper.CreateType("DotNetNuke.Entities.Controllers.HostController, DotNetNuke")
                            '    Dim hostCtlr As Object = Activator.CreateInstance(hostCtlrType)
                            '    Dim hostSettingsMethod As MethodInfo = DirectCast(ReflectionHelper.GetMember(hostCtlrType, "GetSettingsDictionary"), MethodInfo)
                            '    Dim hostSettings As Dictionary(Of String, String) = DirectCast(hostSettingsMethod.Invoke(hostCtlr, Nothing), Dictionary(Of String, String))
                            '    For Each item As KeyValuePair(Of String, String) In hostSettings
                            '        toReturn.Add(item.Key, item.Value)
                            '    Next
                        Case Else
                            Dim hostCtlrType As Type
                            hostCtlrType = ReflectionHelper.CreateType("DotNetNuke.Entities.Host.HostSettings, DotNetNuke")
                            Dim hostCtlr As Object = Activator.CreateInstance(hostCtlrType)
                            Dim hostSettingsMethod As MethodInfo = DirectCast(ReflectionHelper.GetMember(hostCtlrType, "GetHostSettings"), MethodInfo)
                            toReturn = DirectCast(hostSettingsMethod.Invoke(hostCtlr, Nothing), Hashtable)
                    End Select

            End Select
            Return toReturn
        End Function

        Public Sub UpdateSettings(ByVal scope As SettingsScope, _
                                   ByVal scopeId As Integer, ByVal key As String, ByVal strXmlToSave As String)
            Select Case scope
                Case SettingsScope.ModuleSettings
                    ModuleController.UpdateModuleSetting(scopeId, key, strXmlToSave)
                Case SettingsScope.TabModuleSettings
                    ModuleController.UpdateTabModuleSetting(scopeId, key, strXmlToSave)
                Case SettingsScope.PortalSettings
                    InnerUpdateSiteSettings(scopeId, key, strXmlToSave)

                Case SettingsScope.HostSettings
                    Select Case NukeHelper.DnnVersion.Major
                        Case Is >= 6
                            Dim typeHostController As Type = Type.GetType("DotNetNuke.Entities.Controllers.HostController, DotNetNuke")
                            Dim hostControllerInstance = typeHostController.GetProperty("Instance", BindingFlags.Public Or BindingFlags.Static Or BindingFlags.FlattenHierarchy).GetValue(Nothing, Nothing)
                            Dim hostSettingsMethod As MethodInfo = typeHostController.GetMethod("Update", New Type() {GetType(String), GetType(String)})
                            hostSettingsMethod.Invoke(hostControllerInstance, New Object() {key, strXmlToSave})
                        Case Else
                            HostController.UpdateHostSetting(key, strXmlToSave)
                    End Select



            End Select
        End Sub

        Private _GetPortalDefaultLanguageMethod As MethodInfo

        Public ReadOnly Property GetPortalDefaultLanguageMethod As MethodInfo
            Get
                If _GetPortalDefaultLanguageMethod Is Nothing Then
                    _GetPortalDefaultLanguageMethod = DirectCast(ReflectionHelper.GetMember(GetType(PortalController), "GetPortalDefaultLanguage"), MethodInfo)
                End If
                Return _GetPortalDefaultLanguageMethod
            End Get
        End Property


        Private _DeletePortalsettingMethod As MethodInfo

        Public ReadOnly Property DeletePortalsettingMethod As MethodInfo
            Get
                If _DeletePortalsettingMethod Is Nothing Then
                    Dim portalSettingsMethodDico As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(GetType(PortalController))
                    Dim deletePortalSettingsListMethod As New List(Of MemberInfo)
                    If portalSettingsMethodDico.TryGetValue("DeletePortalSetting", deletePortalSettingsListMethod) Then
                        For Each item As MethodInfo In deletePortalSettingsListMethod.OfType(Of MethodInfo)()
                            If item.GetParameters.Length = 3 Then
                                _DeletePortalsettingMethod = item
                                Exit For
                            End If
                        Next
                    End If
                End If
                Return _DeletePortalsettingMethod
            End Get
        End Property

        Public Sub DeleteSettings(ByVal scope As SettingsScope, _
                                   ByVal scopeId As Integer, ByVal key As String)
            Select Case scope
                Case SettingsScope.ModuleSettings
                    ModuleController.DeleteModuleSetting(scopeId, key)
                Case SettingsScope.TabModuleSettings
                    ModuleController.DeleteTabModuleSetting(scopeId, key)
                Case SettingsScope.PortalSettings
                    Select Case NukeHelper.DnnVersion.Major
                        Case Is >= 6
                            Dim culture As String = ""
                            Dim objItem As MethodInfo = Nothing
                            Try
                                culture = DirectCast(GetPortalDefaultLanguageMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId}), String)
                                DeletePortalsettingMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId, key, culture})
                                DeletePortalsettingMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId, key, String.Empty})

                                'todo: corresponding dnn bug:
                                'https://dnntracker.atlassian.net/browse/DNN-7652
                                Dim dictionaryKey = [String].Format("PortalSettingsDictionary{0}{1}", scopeId, String.Empty)
                                If HttpContext.Current IsNot Nothing Then
                                    HttpContext.Current.Items.Remove(dictionaryKey)
                                    dictionaryKey = [String].Format("PortalSettingsDictionary{0}{1}", scopeId, culture)
                                    HttpContext.Current.Items.Remove(dictionaryKey)
                                End If

                            Catch ex As Exception
                                NukeHelper.LogController.AddLog("DeleteSettingsDebug", String.Format("Erreur d'enregistrement des settings : ScopeId = {0} / key = {1} / culture = {2} /// Exceptions => {3} :", scopeId, key, culture, ex.Message), Nothing, -1, EventLogController.EventLogType.HOST_ALERT)
                            End Try

                        Case Else
                            InnerUpdateSiteSettings(scopeId, key, String.Empty)
                    End Select
                Case SettingsScope.HostSettings
                    HostController.UpdateHostSetting(key, "")

            End Select
        End Sub


        Private _UpdatePortalsettingMethod As MethodInfo

        Public ReadOnly Property UpdatePortalsettingMethod As MethodInfo
            Get
                If _UpdatePortalsettingMethod Is Nothing Then
                    Dim portalSettingsMembersDico As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(GetType(PortalController))
                    Dim updatePortalSettingsListMethod As New List(Of MemberInfo)
                    If portalSettingsMembersDico.TryGetValue("UpdatePortalSetting", updatePortalSettingsListMethod) Then
                        For Each item As MethodInfo In updatePortalSettingsListMethod.OfType(Of MethodInfo)()
                            If item.GetParameters.Length = 5 Then
                                _UpdatePortalsettingMethod = item
                                Exit For
                            End If
                        Next
                    End If
                End If
                Return _UpdatePortalsettingMethod
            End Get
        End Property

        Private Sub InnerUpdateSiteSettings(ByVal scopeId As Integer, ByVal key As String, ByVal value As String)
            ' Modification pour DNN 6.1: MAJ du web.config à chaque appel au UpdateSiteSetting de portalsettings
            While True
                Dim exceptionCounter As Integer = 0 ' on va tenter un nombre limité de fois cette modification
                Try


                    Select Case NukeHelper.DnnVersion.Major
                        Case Is >= 6
                            Dim culture As String = ""
                            Dim objItem As MethodInfo = Nothing
                            Try
                               
                                culture = DirectCast(GetPortalDefaultLanguageMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId}), String)
                                'DeletePortalsettingMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId, key, culture})
                                UpdatePortalsettingMethod.Invoke(NukeHelper.PortalController, New Object() {scopeId, key, value, True, String.Empty})

                            Catch ex As Exception
                                NukeHelper.LogController.AddLog("UpdateSettingsDebug", String.Format("Erreur d'enregistrement des settings : ScopeId = {0} / key = {1} / value = {2} / culture = {3} //// Method : {4}", scopeId, key, value, culture, ex.Message), Nothing, -1, EventLogController.EventLogType.HOST_ALERT)
                            End Try

                        Case Else
                            PortalSettings.UpdateSiteSetting(scopeId, key, value)
                    End Select ' classique
                Catch ioex As IOException ' une exception IO, sans doute le web.config qui n'a pas encore été relaché d'un traitement précédent
                    exceptionCounter += 1
                    If exceptionCounter <= 100 Then
                        Continue While ' Tentons à nouveau
                    Else
                        Throw ' on abandonne
                    End If
                Catch ex As Exception 'on remonte d'autres exceptions (par exemple une exception sql, etc...)
                    Throw
                End Try
                Exit While
            End While
        End Sub

#End Region

    End Module
End Namespace

