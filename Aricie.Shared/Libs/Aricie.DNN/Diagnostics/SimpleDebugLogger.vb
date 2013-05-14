Imports Aricie.Services

Namespace Diagnostics
    ''' <summary>
    ''' Simple concrete implementation of a item based asynchronous logger
    ''' </summary>
    Public Class SimpleDebugLogger
        Inherits DebugLoggerBase(Of DebugInfo)

        Public Shared Function Instance() As SimpleDebugLogger
            Return ReflectionHelper.GetSingleton(Of SimpleDebugLogger)()
        End Function

    End Class
End NameSpace