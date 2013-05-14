Namespace ComponentModel
    Public Interface IMergeable(Of T, Y)

        Function GetMerge(ByVal source As T) As Y

    End Interface
End Namespace