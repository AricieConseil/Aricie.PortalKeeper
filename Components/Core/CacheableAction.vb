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

        Private _CacheDuration As New STimeSpan(TimeSpan.FromSeconds(10))
        Private _CacheKeyFormat As String = "{0}"

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
        Public Property CacheKeyFormat() As String
            Get
                Return _CacheKeyFormat
            End Get
            Set(ByVal value As String)
                _CacheKeyFormat = value
            End Set
        End Property

        <SortOrder(412)> _
        <ExtendedCategory("Specifics")> _
         <ConditionalVisible("EnableCache", False, True, True)> _
        Public Property ProcessTokens As Boolean


        <SortOrder(413)> _
        <ConditionalVisible("EnableCache", False, True, True), ExtendedCategory("Specifics")>
        Public Property UseSingleton As Boolean

        <SortOrder(414)> _
        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("EnableCache", False, True, True)>
        Public Property CacheDuration() As STimeSpan
            Get
                Return _CacheDuration
            End Get
            Set(ByVal value As STimeSpan)
                _CacheDuration = value
            End Set
        End Property

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
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    Finally
                        If owned Then
                            objSemaphore.Release()
                        End If
                    End Try
                End Using
            Else
                Return RunUnlocked(actionContext, aSync)
            End If
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
            If returnResult Is Nothing Then
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
                                returnResult = BuildResultAndCache(actionContext, key, aSync)
                            Else
                                Return False
                            End If
                        Catch ex As AbandonedMutexException
                            ExceptionHelper.LogException(ex)
                            owned = True
                        Catch ex As Exception
                            ExceptionHelper.LogException(ex)
                        Finally
                            If owned Then
                                objSemaphore.Release()
                            End If
                        End Try
                    End Using
                Else
                    returnResult = BuildResultAndCache(actionContext, key, aSync)
                End If
            End If
            Return Me.RunFromObject(actionContext, aSync, returnResult)
        End Function

        Private Function BuildResultAndCache(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), key As String, ByVal async As Boolean) As Object
            Dim returnResult As Object = Me.BuildResult(actionContext, async)
            If Me.EnableCache Then
                If Me.UseSingleton Then
                    SyncLock _Singletons
                        Dim tempResult As Object = Nothing
                        If Not _Singletons.TryGetValue(key, tempResult) Then
                            _Singletons(key) = returnResult
                        Else
                            returnResult = tempResult
                        End If
                    End SyncLock
                Else
                    SetCacheDependant(key, returnResult, Me._CacheDuration.Value)
                End If
            End If
            Return returnResult
        End Function



        Public MustOverride Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object


        Public MustOverride Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean

        Private Shared _Singletons As New SerializableDictionary(Of String, Object)
    End Class
End Namespace