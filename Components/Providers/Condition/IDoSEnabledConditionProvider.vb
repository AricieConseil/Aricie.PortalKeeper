Namespace Aricie.DNN.Modules.PortalKeeper
    Public Interface IDoSEnabledConditionProvider(Of TEngineEvents As IConvertible)


        Property EnableDoSProtection() As Boolean
        Property DoSProtectionDuration() As STimeSpan
        Function FastGetKey(ByVal context As PortalKeeperContext(Of TEngineEvents), ByVal clue As Object) As String
        Overloads Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents), ByRef clue As Object, ByRef key As String) As Boolean

    End Interface
End Namespace