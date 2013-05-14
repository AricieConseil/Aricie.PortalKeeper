Namespace ComponentModel


    Public Interface IProviderSettings

        Property ProviderName() As String

        'Function GetProviderConfig() As ProviderConfig



    End Interface

    Public Interface IProviderSettings(Of TProvider As IProvider)
        Inherits IProviderSettings



        Function GetProvider() As TProvider



    End Interface


End Namespace