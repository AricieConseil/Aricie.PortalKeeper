
Namespace ComponentModel

    Public Class InvertComparer(Of T)
        Implements IComparer(Of T)

        Private _SourceComparer As IComparer(Of T)

        Public Sub New(ByVal sourceComparer As IComparer(Of T))
            Me._SourceComparer = sourceComparer
        End Sub

        Public Function Compare(ByVal x As T, ByVal y As T) As Integer Implements IComparer(Of T).Compare
            Return -_SourceComparer.Compare(x, y)
        End Function
    End Class

End Namespace


