Namespace ComponentModel
    Public Interface IProvider(Of TConfig As IProviderConfig, TSettings)
        Inherits IProvider(Of TConfig)
        'Inherits ISimpleProvider(Of TConfig, TSettings)


        'Property Settings() As TSettings

        Function GetNewProviderSettings() As TSettings


    End Interface

    'Public Interface ISimpleProvider(Of TConfig As IProviderConfig, TSettings)
    '    Inherits IProvider(Of TConfig)


    '    Function GetNewProviderSettings() As TSettings



    'End Interface


    Public Interface IProvider(Of TConfig As IProviderConfig)
        Inherits IProvider


        Property Config() As TConfig

    End Interface


    Public Interface IProvider
        Sub SetConfig(ByVal config As IProviderConfig)

        'Sub SetSettings(ByVal settings As IProviderSettings)

    End Interface

End Namespace