

Namespace ComponentModel
    Public Interface IAutoProvider(Of TConfig As IProviderConfig, TProvider As {IProvider(Of TConfig)})
        Inherits IProviderSettings(Of TProvider)
        Inherits IProvider(Of TConfig)





    End Interface
End Namespace