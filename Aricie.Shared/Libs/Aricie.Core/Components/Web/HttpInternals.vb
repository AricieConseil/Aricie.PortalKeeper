Imports System.IO
Imports System.Reflection
Imports System.Web
Imports Aricie.Services
Imports System.Text
Imports System.Web.Compilation

Namespace Web




    Public Class HttpInternals


        Private Shared _Instance As New HttpInternals

        Public Shared ReadOnly Property Instance As HttpInternals
            Get

#If DEBUG Then
                If _Instance.GetType() IsNot ReflectionHelper.CreateType(GetType(HttpInternals).AssemblyQualifiedName) Then
                    _Instance = ReflectionHelper.CreateObject(Of HttpInternals)(GetType(HttpInternals).AssemblyQualifiedName)
                End If
#End If
                Return _Instance
            End Get
        End Property



        'Private Sub New()
        'End Sub

        Private Shared ReadOnly _TheRuntimeField As FieldInfo = GetType(HttpRuntime).GetField("_theRuntime", BindingFlags.NonPublic Or BindingFlags.[Static])

        Private Shared ReadOnly _shutDownMessageField As System.Reflection.FieldInfo = GetType(System.Web.HttpRuntime).GetField("_shutDownMessage", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Private Shared ReadOnly _shutDownStackField As System.Reflection.FieldInfo = GetType(System.Web.HttpRuntime).GetField("_shutDownStack", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Private Shared ReadOnly _FileChangesMonitorField As FieldInfo = GetType(HttpRuntime).GetField("_fcm", BindingFlags.NonPublic Or BindingFlags.Instance)
        Private Shared ReadOnly _FileChangesMonitorStopMethod As MethodInfo = _FileChangesMonitorField.FieldType.GetMethod("Stop", BindingFlags.NonPublic Or BindingFlags.Instance)
       
        Private Shared _HttpRuntime As Object

        Private Shared ReadOnly Property HttpRuntime() As Object
            Get
                If _HttpRuntime Is Nothing Then
                    _HttpRuntime = _TheRuntimeField.GetValue(Nothing)
                End If
                Return _HttpRuntime
            End Get
        End Property

        Public Shared ReadOnly Property ShutDownMessage As String
            Get
                Return DirectCast(_shutDownMessageField.GetValue(HttpRuntime), String)
            End Get
        End Property

        Public Shared ReadOnly Property ShutDownReason As String
            Get
                Dim shutdownReasonEnum As System.Web.ApplicationShutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason
                Dim shutdownDetail As String = ""
                Select Case shutdownReasonEnum
                    Case ApplicationShutdownReason.BinDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the Bin folder or files contained in it."
                    Case ApplicationShutdownReason.BrowsersDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Browsers folder or files contained in it."
                    Case ApplicationShutdownReason.ChangeInGlobalAsax
                        shutdownDetail = "The AppDomain shut down because of a change to Global.asax."
                    Case ApplicationShutdownReason.ChangeInSecurityPolicyFile
                        shutdownDetail = "The AppDomain shut down because of a change in the code access security policy file."
                    Case ApplicationShutdownReason.CodeDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Code folder or files contained in it."
                    Case ApplicationShutdownReason.ConfigurationChange
                        shutdownDetail = "The AppDomain shut down because of a change to the application level configuration."
                    Case ApplicationShutdownReason.HostingEnvironment
                        shutdownDetail = "The AppDomain shut down because of the hosting environment."
                    Case ApplicationShutdownReason.HttpRuntimeClose
                        shutdownDetail = "The AppDomain shut down because of a call to Close."
                    Case ApplicationShutdownReason.IdleTimeout
                        shutdownDetail = "The AppDomain shut down because of the maximum allowed idle time limit."
                    Case ApplicationShutdownReason.InitializationError
                        shutdownDetail = "The AppDomain shut down because of an AppDomain initialization error."
                    Case ApplicationShutdownReason.MaxRecompilationsReached
                        shutdownDetail = "The AppDomain shut down because of the maximum number of dynamic recompiles of resources limit."
                    Case ApplicationShutdownReason.PhysicalApplicationPathChanged
                        shutdownDetail = "The AppDomain shut down because of a change to the physical path for the application."
                    Case ApplicationShutdownReason.ResourcesDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_GlobalResources folder or files contained in it."
                    Case ApplicationShutdownReason.UnloadAppDomainCalled
                        shutdownDetail = "The AppDomain shut down because of a call to UnloadAppDomain."
                    Case Else
                        shutdownDetail = "Shutdown reason unknown : " & shutdownReasonEnum.ToString()
                End Select
                Return shutdownDetail
            End Get
        End Property

        Public Shared ReadOnly Property ShutDownStack As String
            Get
                Return DirectCast(_shutDownStackField.GetValue(HttpRuntime), String)
            End Get
        End Property


        Private Shared _FileChangesMonitor As Object


        Public Shared ReadOnly Property FileChangesMonitor() As Object
            Get
                If _FileChangesMonitor Is Nothing Then
                    _FileChangesMonitor = _FileChangesMonitorField.GetValue(HttpRuntime)
                End If
                Return _FileChangesMonitor
            End Get
        End Property


        Public Shared ReadOnly Property FCNMode As Integer
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_FCNMode", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), Integer)
            End Get
        End Property

        Private Shared _DirectoryMonitor As Object
        Public Shared ReadOnly Property DirectoryMonitor As Object
            Get
                If _DirectoryMonitor Is Nothing Then
                    _DirectoryMonitor = FileChangesMonitor.GetType().GetField("_dirMonSubdirs", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor)
                End If
                Return _DirectoryMonitor

            End Get
        End Property

        Private Shared ReadOnly _AliasesField As FieldInfo = FileChangesMonitor.GetType().GetField("_aliases", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase)
        Public Shared ReadOnly Property Aliases As Hashtable
            Get
                Return DirectCast(_AliasesField.GetValue(FileChangesMonitor), Hashtable)
            End Get
        End Property

        Public Shared ReadOnly Property Dirs As Hashtable
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_dirs", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), Hashtable)
            End Get
        End Property

        Public Shared ReadOnly Property SubDirDirMons As Hashtable
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_subDirDirMons", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), Hashtable)
            End Get
        End Property

        Public Shared Property DirMonSpecialDirs As ArrayList
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_dirMonSpecialDirs", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), ArrayList)
            End Get
            Set(value As ArrayList)
                FileChangesMonitor.GetType().GetField("_dirMonSpecialDirs", System.Reflection.BindingFlags.Instance _
                                                                             Or System.Reflection.BindingFlags.NonPublic _
                                                                             Or System.Reflection.BindingFlags.IgnoreCase).SetValue(FileChangesMonitor, value)
            End Set
        End Property

        Private _StartMonitoringFileMethod As MethodInfo

        Private ReadOnly Property StartMonitoringFileMethod As MethodInfo
            Get
                If _StartMonitoringFileMethod Is Nothing Then
                    _StartMonitoringFileMethod = FileChangesMonitor.GetType().GetMethod("StartMonitoringFile", BindingFlags.Instance Or BindingFlags.NonPublic)
                End If
                Return _StartMonitoringFileMethod
            End Get
        End Property
        Public Shared Property CriticalChangeCallBack As MulticastDelegate
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_callbackRenameOrCriticaldirChange", System.Reflection.BindingFlags.Instance _
                                                                              Or System.Reflection.BindingFlags.NonPublic _
                                                                              Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), MulticastDelegate)
            End Get
            Set(value As MulticastDelegate)
                FileChangesMonitor.GetType().GetField("_callbackRenameOrCriticaldirChange", System.Reflection.BindingFlags.Instance _
                                                                              Or System.Reflection.BindingFlags.NonPublic _
                                                                              Or System.Reflection.BindingFlags.IgnoreCase).SetValue(FileChangesMonitor, value)
            End Set
        End Property

       

        Protected Shared ReadOnly _fileMonsField As System.Reflection.FieldInfo = DirectoryMonitor.GetType().GetField("_fileMons", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Protected Shared ReadOnly _anyFileMonField As System.Reflection.FieldInfo = DirectoryMonitor.GetType().GetField("_anyFileMon", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Protected Shared _RemoveTargetMethod As MethodInfo
        Protected Shared _FileNameLongField As FieldInfo

        Private Shared _GetCodeDirectoriesMethod As MethodInfo = GetType(BuildManager).GetMethod("GetCodeDirectories", BindingFlags.Instance Or BindingFlags.NonPublic)
        Private Shared _compileMethod As MethodInfo = GetType(BuildManager).GetMethod("CompileCodeDirectory", BindingFlags.Instance Or BindingFlags.NonPublic)
        Private Shared _theBuildManagerField As FieldInfo = GetType(BuildManager).GetField("_theBuildManager", BindingFlags.Static Or BindingFlags.NonPublic)
        Private Shared _topAssembliesField As FieldInfo = GetType(BuildManager).GetField("_topLevelReferencedAssemblies", BindingFlags.Instance Or BindingFlags.NonPublic)
        Private Shared _codeAssembliesField As FieldInfo = GetType(BuildManager).GetField("_codeAssemblies", BindingFlags.Instance Or BindingFlags.NonPublic)
        Private Shared _simpleCombineMethod As MethodInfo = ApplicationVirtualPath.GetType().GetMethod("SimpleCombineWithDir", BindingFlags.Instance Or BindingFlags.NonPublic)

        Private Shared _TheBuildManager As Object
        Private Shared _applicationVirtualPath As Object

        Public Shared ReadOnly Property TheBuildManager As Object
            Get
                If _TheBuildManager Is Nothing Then
                    _TheBuildManager = _theBuildManagerField.GetValue(Nothing)
                End If
                Return _TheBuildManager
            End Get
        End Property


        Public Shared ReadOnly Property ApplicationVirtualPath As Object
            Get
                If _applicationVirtualPath Is Nothing Then
                    _applicationVirtualPath = GetType(HttpRuntime).GetProperty("CodeDirectoryVirtualPath", BindingFlags.Static Or BindingFlags.NonPublic).GetValue(Nothing, Nothing)
                End If
                Return _applicationVirtualPath
            End Get
        End Property

       

        'Public Shared Sub CompileCodeDirectories()
        '    Dim buildManagerType As Type = GetType(BuildManager)
        '    Dim buildManagerField As FieldInfo = buildManagerType.GetField("_theBuildManager", BindingFlags.Static Or BindingFlags.NonPublic)
        '    buildManagerType.GetMethod("CompileCodeDirectories", BindingFlags.Instance Or BindingFlags.NonPublic).Invoke(buildManagerField.GetValue(Nothing), Nothing)
        'End Sub

        Public Function GetCodeDirectories() As String()
            Dim toReturn As New List(Of String)
            For Each strPath As String In DirectCast(_GetCodeDirectoriesMethod.Invoke(TheBuildManager, Nothing), String())
                strPath = strPath.Trim("/"c).Replace("App_Code", "").TrimStart("/"c)
                If strPath.Length > 0 Then
                    toReturn.Add(strPath)
                End If
            Next
            Return toReturn.ToArray()
        End Function

        Public Function CompileCodeDirectory(directoryName As String) As System.Reflection.Assembly
            Dim assemblyName As String = "App_SubCode_" & directoryName
            Dim topAssemblies As List(Of System.Reflection.Assembly) = DirectCast(_topAssembliesField.GetValue(TheBuildManager), List(Of System.Reflection.Assembly))
            For Each objCodeAssembly As System.Reflection.Assembly In New ArrayList(topAssemblies)
                If objCodeAssembly.GetName().Name.StartsWith(assemblyName) Then
                    topAssemblies.Remove(objCodeAssembly)
                End If
            Next

            Dim codeAssemblies As ArrayList = DirectCast(_codeAssembliesField.GetValue(TheBuildManager), ArrayList)
            For Each objCodeAssembly As System.Reflection.Assembly In New ArrayList(codeAssemblies)
                If objCodeAssembly.GetName().Name.StartsWith(assemblyName) Then
                    codeAssemblies.Remove(objCodeAssembly)
                End If
            Next
            assemblyName = assemblyName & DateTime.UtcNow.ToFileTimeUtc().ToString()

            Dim targetVirtualDir As Object = _simpleCombineMethod.Invoke(ApplicationVirtualPath, {directoryName})
            Dim typeCodeDirectoryType As Type = _compileMethod.GetParameters()(1).ParameterType 'BuildManager.GetType("System.Web.Compilation.CodeDirectoryType, System.Web", False)
            Dim objCodeDirectoryType As Object = [Enum].Parse(typeCodeDirectoryType, "SubCode")


            Dim toReturn As System.Reflection.Assembly = DirectCast(_compileMethod.Invoke(TheBuildManager, {targetVirtualDir, objCodeDirectoryType, assemblyName, Nothing}), System.Reflection.Assembly)
            Aricie.Services.CacheHelper.ClearCache()
            Return toReturn
        End Function

        Public Function StartMonitoringPath(ByVal fullPath As String, ByVal callBack As EventHandler) As DateTime
            Dim toReturn As DateTime = DateTime.MinValue
            Dim targetDelegateType As Type = StartMonitoringFileMethod.GetParameters(1).ParameterType
            Dim wrappedDelegate As [Delegate] = ReflectionHelper.WrapDelegate(targetDelegateType, callBack)
            If Directory.Exists(fullPath) Then
                For Each strFileSystem As String In Directory.GetFileSystemEntries(fullPath)
                    toReturn = New DateTime(Math.Min(toReturn.Ticks, StartMonitoringPath(strFileSystem, callBack).Ticks))
                Next
            End If
            If File.Exists(fullPath) Then
                toReturn = New DateTime(Math.Min(toReturn.Ticks, _
                            DirectCast(StartMonitoringFileMethod.Invoke(FileChangesMonitor, {fullPath, wrappedDelegate}), DateTime).Ticks))
            End If

            Return toReturn
        End Function

        Private Shared _FileChangeActionField As FieldInfo
        Private Shared _AddedFileActionEnum As Object
        Private Shared _ModifiedFileActionEnum As Object
        Public Function StartCompilationMonitoring(ByVal codeDirName As String) As DateTime
            Dim callBack As New EventHandler(Sub(sender As Object, args As EventArgs)
                                                 Try
                                                     If _FileChangeActionField Is Nothing Then
                                                         _FileChangeActionField = args.GetType().GetField("Action", BindingFlags.NonPublic Or BindingFlags.Instance)
                                                         _AddedFileActionEnum = [Enum].Parse(_FileChangeActionField.FieldType, "Added")
                                                         _ModifiedFileActionEnum = [Enum].Parse(_FileChangeActionField.FieldType, "Modified")
                                                     End If
                                                     Dim objAction As Object = _FileChangeActionField.GetValue(args)
                                                     If objAction.Equals(_AddedFileActionEnum) OrElse objAction.Equals(_ModifiedFileActionEnum) Then
                                                         CompileCodeDirectory(codeDirName)
                                                     End If
                                                 Catch ex As Exception
                                                     ExceptionHelper.LogException(ex)
                                                 End Try

                                             End Sub)

            Return StartMonitoringPath(Hosting.HostingEnvironment.MapPath("~/App_Code/" & codeDirName), callBack)
        End Function

        Public Shared Function GetDirMonitorDir(dirMon As Object) As String
            Dim toReturn As String = String.Empty
            If dirMon IsNot Nothing Then
                toReturn = TryCast(dirMon.GetType().GetField("Directory", System.Reflection.BindingFlags.Instance _
                                                                              Or System.Reflection.BindingFlags.NonPublic _
                                                                              Or System.Reflection.BindingFlags.IgnoreCase).GetValue(dirMon), String)
                If toReturn Is Nothing Then
                    toReturn = String.Empty
                End If
            End If
            Return toReturn
        End Function

        Public Overridable Sub StopSystemMonitoring()
            _FileChangesMonitorStopMethod.Invoke(FileChangesMonitor, Nothing)
        End Sub



        Public Overridable Function StopDirectoryCriticalMonitoring(pattern As String) As Integer
            Return Me.StopPathMonitoringByTargetAndPattern(HttpInternals.FileChangesMonitor, pattern)
        End Function



        Public Overridable Function StopPathMonitoringByTargetAndPattern(target As Object, pattern As String) As Integer
            Dim toReturn As Integer = 0
            'for some reason (fcnmode resulting in a single critical directory monitor I guess), there might be duplicates so to speed up with maintain a hashset of what we went through
            Dim knownDirectories As New HashSet(Of Object)
            If pattern.IsNullOrEmpty() Then

                toReturn += RemoveTargets(target, DirectoryMonitor)
            Else
                pattern = pattern.ToUpperInvariant()
            End If

            For Each dirMon As Object In DirMonSpecialDirs

                If dirMon IsNot Nothing Then
                    If Not knownDirectories.Contains(dirMon) Then
                        knownDirectories.Add(dirMon)
                        toReturn += RemoveTargetsWithPattern(target, dirMon, pattern)
                    End If



                End If
            Next

            For Each dirMonPair As DictionaryEntry In Dirs

                If dirMonPair.Value IsNot Nothing Then
                    If Not knownDirectories.Contains(dirMonPair.Value) Then
                        knownDirectories.Add(dirMonPair.Value)
                        toReturn += RemoveTargetsWithPattern(target, dirMonPair.Value, pattern)
                    End If

                End If
            Next

            For Each aliasPair As DictionaryEntry In Aliases

                Dim strAlias As String = DirectCast(aliasPair.Key, String)
                If strAlias.ToUpperInvariant().Contains(pattern) Then
                    If aliasPair.Value IsNot Nothing Then

                        toReturn += RemoveTargetsFromFileMon(target, aliasPair.Value)


                    End If
                End If
            Next


            Return toReturn

        End Function



        Private Function RemoveTargetsWithPattern(objTarget As Object, dirMon As Object, pattern As String) As Integer
            Dim toReturn As Integer = 0
            If pattern.IsNullOrEmpty() OrElse GetDirMonitorDir(dirMon).ToUpperInvariant().Contains(pattern) Then
                toReturn += RemoveTargets(objTarget, dirMon)
            Else
                Dim fileMons As Hashtable = DirectCast(_fileMonsField.GetValue(dirMon), Hashtable)

                For Each objFileMon As Object In fileMons.Values
                    If _FileNameLongField Is Nothing Then
                        _FileNameLongField = _anyFileMonField.FieldType.GetField("_fileNameLong", BindingFlags.NonPublic Or BindingFlags.Instance)
                    End If
                    If pattern.IsNullOrEmpty() OrElse DirectCast(_FileNameLongField.GetValue(objFileMon), String).ToUpperInvariant().Contains(pattern) Then
                        toReturn += RemoveTargetsFromFileMon(objTarget, objFileMon)
                    End If


                Next
            End If

            Return toReturn
        End Function


        Private Function RemoveTargets(objTarget As Object, dirMon As Object) As Integer


            Dim toReturn As Integer = 0

            Dim fileMons As Hashtable = DirectCast(_fileMonsField.GetValue(dirMon), Hashtable)

            For Each objFileMon As Object In fileMons.Values
                toReturn += RemoveTargetsFromFileMon(objTarget, objFileMon)

            Next
            Dim anyFileMon As Object = _anyFileMonField.GetValue(dirMon)
            If anyFileMon IsNot Nothing Then
                toReturn += RemoveTargetsFromFileMon(objTarget, anyFileMon)
            End If
            Return toReturn
        End Function

        Private Function RemoveTargetsFromFileMon(objTarget As Object, fileMon As Object) As Integer
            If _RemoveTargetMethod Is Nothing Then
                _RemoveTargetMethod = _anyFileMonField.FieldType.GetMethod("RemoveTarget", BindingFlags.NonPublic Or BindingFlags.Instance)
            End If
            Return DirectCast(_RemoveTargetMethod.Invoke(fileMon, {objTarget}), Integer)
        End Function






        Public Overridable Sub CombineCriticalChangeCallBack(objHandler As [Delegate])
            If CriticalChangeCallBack IsNot Nothing Then
                CriticalChangeCallBack = ReflectionHelper.CombineDelegate(CriticalChangeCallBack, objHandler)
            Else
                CriticalChangeCallBack = DirectCast(ReflectionHelper.WrapDelegate(FileChangesMonitor.GetType().GetField("_callbackRenameOrCriticaldirChange", System.Reflection.BindingFlags.Instance _
                                                                              Or System.Reflection.BindingFlags.NonPublic _
                                                                              Or System.Reflection.BindingFlags.IgnoreCase).FieldType, objHandler), MulticastDelegate)
            End If
        End Sub

        Public Overridable Function GetDump() As String
            Return GetDump(False)
        End Function

        Public Overridable Function GetDump(includeMonitors As Boolean) As String
            'Return MyBase.GetDump()

            Dim toReturn As New StringBuilder()
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("ShutDownMessage : {0}", HttpInternals.ShutDownMessage))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("ShutDownReason : {0}", HttpInternals.ShutDownReason))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("ShutDownStack : {0}", HttpInternals.ShutDownStack))
            toReturn.AppendLine()

            toReturn.AppendLine(String.Format("FCNMode : {0}", HttpInternals.FCNMode))
            toReturn.AppendLine()
            If includeMonitors Then
                Dim maxDepth As Integer = 1
                toReturn.AppendLine(String.Format("CriticalChangeCallBack : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.CriticalChangeCallBack, maxDepth)).Beautify()))
                toReturn.AppendLine()
                Dim aliasFields As Hashtable = HttpInternals.Aliases
                Dim fields As Object = ReflectionHelper.GetFields(aliasFields, maxDepth)
                toReturn.AppendLine(String.Format("Aliases : {0}", ReflectionHelper.Serialize(fields).Beautify()))
                toReturn.AppendLine()
                toReturn.AppendLine(String.Format("Dirs : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.Dirs, maxDepth)).Beautify()))
                toReturn.AppendLine()
                toReturn.AppendLine(String.Format("SubDirDirMons : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.SubDirDirMons, maxDepth)).Beautify()))
                toReturn.AppendLine()
                toReturn.AppendLine(String.Format("DirMonSpecialDirs : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.DirMonSpecialDirs, maxDepth)).Beautify()))

                toReturn.AppendLine()
            End If


            Return toReturn.ToString()

        End Function



    End Class
End Namespace