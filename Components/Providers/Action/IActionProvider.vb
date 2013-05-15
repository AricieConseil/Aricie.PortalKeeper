Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    'Public Interface IAutoActionProvider
    '    Inherits IAutoProvider(Of IAutoActionProvider, ActionProvider)
    '    Inherits IActionProvider





    'End Interface

    Public Interface IActionProvider(Of TEngineEvents As IConvertible)
        Inherits IProvider(Of ActionProviderConfig(Of TEngineEvents), ActionProviderSettings(Of TEngineEvents))



        Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean


    End Interface
End Namespace