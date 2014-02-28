Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class CacheableAction(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        Private _CacheDuration As New STimeSpan(TimeSpan.FromSeconds(10))
        Private _CacheKeyFormat As String = "{0}"


        <ExtendedCategory("Specifics")>
        Public Property EnableCache As Boolean

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

        <ExtendedCategory("Specifics")> _
         <ConditionalVisible("EnableCache", False, True, True)> _
        Public Property ProcessTokens As Boolean



        <ConditionalVisible("EnableCache", False, True, True), ExtendedCategory("Specifics")>
        Public Property UseSingleton As Boolean


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


        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
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
                returnResult = Me.BuildResult(actionContext, aSync)
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
            End If
            Return Me.RunFromObject(actionContext, aSync, returnResult)
        End Function

        Public MustOverride Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object


        Public MustOverride Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean

        Private Shared _Singletons As New SerializableDictionary(Of String, Object)
    End Class
End Namespace