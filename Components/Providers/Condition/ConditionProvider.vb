Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ConditionProvider(Of TEngineEvents As IConvertible)
        Inherits ConditionProviderSettings(Of TEngineEvents)
        Implements IConditionProvider(Of TEngineEvents)




        Public Overridable Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean Implements IConditionProvider(Of TEngineEvents).Match
            Return False

        End Function
    End Class
End Namespace