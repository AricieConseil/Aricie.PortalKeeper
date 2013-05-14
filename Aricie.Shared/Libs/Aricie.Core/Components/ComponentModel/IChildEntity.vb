Namespace ComponentModel

    ''' <summary>
    ''' Interface for a Simple Hierarchy
    ''' </summary>
    Public Interface IChildEntity
        Sub SetParent(ByVal parent As Object)


    End Interface

    ''' <summary>
    ''' Simple Interface for a simple Hierarchy
    ''' </summary>
    Public Interface IChildEntity(Of ParentType)
        Inherits IChildEntity

        Property Parent() As ParentType

    End Interface
End Namespace