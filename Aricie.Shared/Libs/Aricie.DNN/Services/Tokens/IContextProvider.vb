Namespace Services


    ''' <summary>
    ''' Interface for context providers: obsolete (restored for compatibility)
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("User IContextSource instead")> _
    Public Interface IContextProvider

        Function GetCurrentContext() As Object

        Function GetAvailiableContexts() As List(Of Object)

    End Interface




    ''' <summary>
    ''' Interface for context providing contexts by type
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IContextSource

        Function HasContext(objType As Type) As Boolean
        Function GetContext(objType As Type) As Object

    End Interface

End Namespace
