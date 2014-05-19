Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper


    Public Interface IConditionProvider(Of TEngineEvents As IConvertible)
        Inherits IProvider(Of ConditionProviderConfig(Of TEngineEvents), ConditionProviderSettings(Of TEngineEvents))



        Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean

    End Interface
End Namespace