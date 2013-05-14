Namespace ComponentModel
    Public Interface IProviderConfig(Of T)
        Inherits IProviderConfig


        Function GetTypedProvider() As T



    End Interface

    Public Interface IProviderConfig

        Function GetProvider() As Object

    End Interface
End Namespace