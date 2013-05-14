Namespace Services

    ''' <summary>
    ''' Interface for context providers: not used
    ''' TODO: delete
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IContextProvider

        Function GetCurrentContext() As Object

        Function GetAvailiableContexts() As List(Of Object)

    End Interface

End Namespace
