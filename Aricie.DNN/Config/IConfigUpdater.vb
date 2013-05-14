
Namespace Configuration
    ''' <summary>
    ''' Base interface for configuration dedicated providers
    ''' </summary>
    Public Interface IUpdateProvider

        'Function GetUpdateConfig(ByVal action As ConfigActionType) As ConfigInfo

        Function GetConfigElements() As List(Of IConfigElementInfo)

        'Function GetHttpModules() As List(Of HttpModuleInfo)

        'Function GetProviders() As List(Of ProviderInfo)

        'Function GetSchedulerClients() As System.Collections.Generic.Dictionary(Of Type, TimeSpan)

        'Function GetAuthenticationServices() As List(Of AuthenticationServiceInfo)


        'Function GetPermissionHelpers() As List(Of Security.PermissionHelper)



    End Interface
End Namespace


