

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    ''' <summary>
    ''' General Purpose Generics filtering Interface
    ''' </summary>
    Public Interface IFilter
        Inherits IDescriptor

        Function Match(Of T)(ByVal content As T) As Boolean

        ReadOnly Property IsSimpleMatch() As Boolean
    End Interface
End Namespace
