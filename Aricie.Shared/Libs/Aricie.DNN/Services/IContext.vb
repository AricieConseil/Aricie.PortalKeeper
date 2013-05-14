Namespace Services
    ''' <summary>
    ''' Interfaces for context
    ''' </summary>
    ''' <typeparam name="TContext"></typeparam>
    ''' <remarks></remarks>
    Public Interface IContext(Of TContext)

        Function GetInstance() As TContext

    End Interface
End Namespace