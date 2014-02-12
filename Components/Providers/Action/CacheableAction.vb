Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class CacheableAction(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        Private _EnableCache As Boolean
        Private _CacheDuration As New STimeSpan(TimeSpan.FromSeconds(10))
        Private _CacheKeyFormat As String = "{0}"


        <ExtendedCategory("Specifics")> _
        Public Property EnableCache() As Boolean
            Get
                Return _EnableCache
            End Get
            Set(ByVal value As Boolean)
                _EnableCache = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
            <ConditionalVisible("EnableCache", False, True, True)> _
            <TrialLimited(Security.Trial.TrialPropertyMode.Disable)> _
        Public Property CacheKeyFormat() As String
            Get
                Return _CacheKeyFormat
            End Get
            Set(ByVal value As String)
                _CacheKeyFormat = value
            End Set
        End Property


        Private _UseSingleton As Boolean

        <ExtendedCategory("Specifics")> _
           <ConditionalVisible("EnableCache", False, True, True)> _
        Public Property UseSingleton() As Boolean
            Get
                Return _UseSingleton
            End Get
            Set(ByVal value As Boolean)
                _UseSingleton = value
            End Set
        End Property


        <ExtendedCategory("Specifics")> _
            <ConditionalVisible("EnableCache", False, True, True)> _
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



        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            Dim returnResult As Object = Nothing
            Dim key As String = ""
            If Me._EnableCache Then
                key = actionContext.CurrentEngine.Name & "."c & String.Format(Me._CacheKeyFormat, Me.Name)
                If Me._UseSingleton Then
                    _Singletons.TryGetValue(key, returnResult)
                Else
                    returnResult = Aricie.Services.CacheHelper.GetCache(key)
                End If
            End If
            If returnResult Is Nothing Then
                returnResult = Me.BuildResult(actionContext, aSync)
                If Me._EnableCache Then
                    If Me._UseSingleton Then
                        SyncLock _Singletons
                            Dim tempResult As Object = Nothing
                            If Not _Singletons.TryGetValue(key, tempResult) Then
                                _Singletons(key) = returnResult
                            Else
                                returnResult = tempResult
                            End If
                        End SyncLock
                    Else
                        Aricie.Services.CacheHelper.SetCacheDependant(key, returnResult, Me._CacheDuration.Value)
                    End If
                End If
            End If
            Return Me.RunFromObject(actionContext, aSync, returnResult)
        End Function

        Public MustOverride Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object


        Public MustOverride Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean

        Private Shared _Singletons As New SerializableDictionary(Of String, Object)


    End Class
End Namespace