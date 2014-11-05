Namespace Services

    ''' <summary>
    ''' Interface for context providers: not used
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IContextProvider

        Function HasContect(objType As Type) As Boolean
        Function GetContext(objType As Type) As Object

    End Interface

End Namespace
