Imports System.Reflection
Imports System.Web
Imports Aricie.Services
Imports System.Text

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


        Public Shared ReadOnly Property Aliases As Hashtable
            Get
                Return DirectCast(FileChangesMonitor.GetType().GetField("_aliases", System.Reflection.BindingFlags.Instance _
                                                                               Or System.Reflection.BindingFlags.NonPublic _
                                                                               Or System.Reflection.BindingFlags.IgnoreCase).GetValue(FileChangesMonitor), Hashtable)
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

        Public Shared Function GetDirMonitorDir(dirMon As Object) As String
            If dirMon IsNot Nothing Then
                Dim toReturn As String = TryCast(dirMon.GetType().GetField("Directory", System.Reflection.BindingFlags.Instance _
                                                                              Or System.Reflection.BindingFlags.NonPublic _
                                                                              Or System.Reflection.BindingFlags.IgnoreCase).GetValue(dirMon), String)
                If toReturn Is Nothing Then
                    toReturn = String.Empty
                End If
            End If
            Return String.Empty
        End Function

        Public Overridable Sub StopSystemMonitoring()
            _FileChangesMonitorStopMethod.Invoke(FileChangesMonitor, Nothing)
        End Sub

        Protected Shared ReadOnly _fileMonsField As System.Reflection.FieldInfo = DirectoryMonitor.GetType().GetField("_fileMons", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Protected Shared ReadOnly _anyFileMonField As System.Reflection.FieldInfo = DirectoryMonitor.GetType().GetField("_anyFileMon", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
        Protected Shared _RemoveTargetMethod As MethodInfo
        'Protected Shared ReadOnly _targetsField As System.Reflection.FieldInfo = _anyFileMonField.FieldType.GetField("_targets", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)

        Public Overridable Sub StopDirectoryCriticalMonitoring(pattern As String)

            If pattern.IsNullOrEmpty() Then

                RemoveTargets(HttpInternals.FileChangesMonitor, DirectoryMonitor)

            Else

                For Each dirMon As Object In DirMonSpecialDirs
                    If dirMon IsNot Nothing AndAlso GetDirMonitorDir(dirMon).Contains(pattern) Then

                        RemoveTargets(HttpInternals.FileChangesMonitor, dirMon)
                    End If
                Next

                For Each dirMonPair As DictionaryEntry In Dirs
                    If dirMonPair.Value IsNot Nothing AndAlso GetDirMonitorDir(dirMonPair.Value).Contains(pattern) Then 'AndAlso Not GetDirMonitorDir(dirMon).IsNullOrEmpty

                        RemoveTargets(HttpInternals.FileChangesMonitor, dirMonPair.Value)

                    End If
                Next

            End If

        End Sub

        Private Sub RemoveTargets(objTarget As Object, dirMon As Object)

            If _RemoveTargetMethod Is Nothing Then
                _RemoveTargetMethod = _anyFileMonField.FieldType.GetMethod("RemoveTarget", BindingFlags.NonPublic Or BindingFlags.Instance)
            End If


            Dim fileMons As Hashtable = DirectCast(_fileMonsField.GetValue(dirMon), Hashtable)

            For Each objFileMon As Object In fileMons.Values
                Dim count As Integer = DirectCast(_RemoveTargetMethod.Invoke(objFileMon, {objTarget}), Integer)

            Next
            Dim anyFileMon As Object = _anyFileMonField.GetValue(dirMon)
            If anyFileMon IsNot Nothing Then
                Dim count As Integer = DirectCast(_RemoveTargetMethod.Invoke(anyFileMon, {objTarget}), Integer)
            End If
        End Sub





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
            'Return MyBase.GetDump()

            Dim toReturn As New StringBuilder()
            toReturn.AppendLine(String.Format("ShutDownMessage : {0}", HttpInternals.ShutDownMessage))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("ShutDownReason : {0}", HttpInternals.ShutDownReason))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("ShutDownStack : {0}", HttpInternals.ShutDownStack))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("FCNMode : {0}", HttpInternals.FCNMode))
            toReturn.AppendLine()
            Dim maxDepth As Integer = 1
            Dim aliases = HttpInternals.Aliases

            Dim fields As Object = ReflectionHelper.GetFields(aliases, maxDepth)
            toReturn.AppendLine(String.Format("Aliases : {0}", ReflectionHelper.Serialize(fields).Beautify()))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("Dirs : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.Dirs, maxDepth)).Beautify()))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("SubDirDirMons : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.SubDirDirMons, maxDepth)).Beautify()))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("DirMonSpecialDirs : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.DirMonSpecialDirs, maxDepth)).Beautify()))
            toReturn.AppendLine()
            toReturn.AppendLine(String.Format("CriticalChangeCallBack : {0}", ReflectionHelper.Serialize(ReflectionHelper.GetFields(HttpInternals.CriticalChangeCallBack, maxDepth)).Beautify()))
            toReturn.AppendLine()

            Return toReturn.ToString()

        End Function



    End Class
End Namespace