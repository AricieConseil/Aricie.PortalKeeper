Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Workers
Imports System.Threading

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class CacheableAction(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)


        <SortOrder(410)> _
        <ExtendedCategory("Specifics")>
        Public Property EnableCache As Boolean

        <SortOrder(950)> _
       <ConditionalVisible("UseSemaphore", False, True)> _
       <ExtendedCategory("TechnicalSettings")> _
        Public Property SemaphoreAppliesToCache As Boolean

        <SortOrder(411)> _
        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("EnableCache", False, True, True)> _
        <TrialLimited(TrialPropertyMode.Disable)>
        Public Property CacheKeyFormat() As String  = "{0}"

        <SortOrder(412)> _
        <ExtendedCategory("Specifics")> _
         <ConditionalVisible("EnableCache", False, True)> _
        Public Property ProcessTokens As Boolean


        <SortOrder(413)> _
        <ConditionalVisible("EnableCache", False, True)> _
        <ExtendedCategory("Specifics")>
        Public Property UseSingleton As Boolean

        <SortOrder(414)> _
        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("EnableCache", False, True)>
        <ConditionalVisible("UseSingleton", True, True)>
        Public Property CacheDuration() As New STimeSpan(TimeSpan.FromSeconds(10))
         
        <SortOrder(415)> _
        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("EnableCache", False, True)>
        Public Property PreCacheInsertActions As New KeeperAction(Of TEngineEvents)


        '<ExtendedCategory("Specifics")> _
        '    <ConditionalVisible("EnableCache", False, True, True)> _
        'Public Property EnableFileCaching As Boolean

        '<ExtendedCategory("Specifics")> _
        '    <ConditionalVisible("EnableFileCaching", False, True, True)> _
        'Public Property EnableSerialization As Boolean


        Public Overrides Function RunAndSleep(actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return MyBase.RunAndSleepUnlocked(actionContext)
        End Function

        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            If Me.UseSemaphore AndAlso SemaphoreAppliesToCache Then
                Dim owned As Boolean
                'Dim semaphoreId As String = "AsyncBot" & botContext.AsyncLockId.ToString(CultureInfo.InvariantCulture)
                'todo: check if a global semaphore is necessary (see the code below for security access)
                'semaphoreId = String.Format("Global\{0}", semaphoreId)
                'Using objSemaphore As New Semaphore(0, Me.NbConcurrentThreads, Me.SemaphoreName)
                Using objSemaphore As New SafeSemaphore(Me.NbConcurrentThreads, Me.SemaphoreName)
                    Try
                        'Dim allowEveryoneRule As New MutexAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), MutexRights.FullControl, AccessControlType.Allow)
                        'Dim securitySettings As New MutexSecurity()
                        'securitySettings.AddAccessRule(allowEveryoneRule)
                        'objMutex.SetAccessControl(securitySettings)
                        If (Me.SynchronisationTimeout.Value <> TimeSpan.Zero AndAlso objSemaphore.Wait(Me.SynchronisationTimeout.Value)) OrElse (Me.SynchronisationTimeout.Value = TimeSpan.Zero AndAlso objSemaphore.Wait()) Then
                            owned = True
                            Return RunUnlocked(actionContext, aSync)
                        Else
                            Return False
                        End If
                    Catch ex As AbandonedMutexException
                        ExceptionHelper.LogException(ex)
                        owned = True
                    Finally
                        If owned Then
                            objSemaphore.Release()
                        End If
                    End Try
                End Using
            Else
                Return RunUnlocked(actionContext, aSync)
            End If
            Return False
        End Function


        Private Function RunUnlocked(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            Dim returnResult As Object = Nothing
            Dim key As String = ""
            If Me.EnableCache Then
                key = Me._CacheKeyFormat
                If Me.ProcessTokens Then
                    Dim atc As AdvancedTokenReplace = Me.GetAdvancedTokenReplace(actionContext)
                    key = atc.ReplaceAllTokens(key)
                End If
                key = actionContext.CurrentEngine.Name & "."c & String.Format(key, Me.Name)
                If Me.UseSingleton Then
                    _Singletons.TryGetValue(key, returnResult)
                Else
                    returnResult = CacheHelper.GetCache(key)
                End If
            End If
            Dim shouldInsertInCache As Boolean
            If returnResult Is Nothing Then
                If Me.EnableCache Then
                    shouldInsertInCache = True
                End If
                If UseSemaphore AndAlso Not SemaphoreAppliesToCache Then
                    Dim owned As Boolean
                    'Dim semaphoreId As String = "AsyncBot" & botContext.AsyncLockId.ToString(CultureInfo.InvariantCulture)
                    'todo: check if a global semaphore is necessary (see the code below for security access)
                    'semaphoreId = String.Format("Global\{0}", semaphoreId)
                    'Using objSemaphore As New Semaphore(0, Me.NbConcurrentThreads, Me.SemaphoreName)
                    Using objSemaphore As New SafeSemaphore(Me.NbConcurrentThreads, Me.SemaphoreName)
                        Try
                            'Dim allowEveryoneRule As New MutexAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), MutexRights.FullControl, AccessControlType.Allow)
                            'Dim securitySettings As New MutexSecurity()
                            'securitySettings.AddAccessRule(allowEveryoneRule)
                            'objMutex.SetAccessControl(securitySettings)
                            If (Me.SynchronisationTimeout.Value <> TimeSpan.Zero AndAlso objSemaphore.Wait(Me.SynchronisationTimeout.Value)) OrElse (Me.SynchronisationTimeout.Value = TimeSpan.Zero AndAlso objSemaphore.Wait()) Then
                                owned = True
                                returnResult = BuildResult(actionContext, aSync)
                            Else
                                Return False
                            End If
                        Catch ex As AbandonedMutexException
                            ExceptionHelper.LogException(ex)
                            owned = True
                        Finally
                            If owned Then
                                objSemaphore.Release()
                            End If
                        End Try
                    End Using
                Else
                    returnResult = BuildResult(actionContext, aSync)
                End If
            End If
            Dim toReturn As Boolean = Me.RunFromObject(actionContext, aSync, returnResult)
            If shouldInsertInCache AndAlso toReturn AndAlso returnResult IsNot Nothing Then
                If _PreCacheInsertActions.Instances.Count > 0 Then
                    _PreCacheInsertActions.Run(actionContext)
                End If
                InsertInCache(key, returnResult)
            End If
            Return toReturn
        End Function

        'Private Function BuildResultAndCache(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), key As String, ByVal async As Boolean) As Object
        '    Dim returnResult As Object = Me.BuildResult(actionContext, async)

        '    Return returnResult
        'End Function




        Private Sub InsertInCache(key As String, returnResult As Object)
            If Me.EnableCache Then
                If Me.UseSingleton Then
                    SyncLock _Singletons
                        _Singletons(key) = returnResult
                    End SyncLock
                Else
                    SetCacheDependant(key, returnResult, Me._CacheDuration.Value)
                End If
            End If
        End Sub

        Public MustOverride Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object


        Public MustOverride Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean

        Private Shared _Singletons As New SerializableDictionary(Of String, Object)
    End Class
End Namespace