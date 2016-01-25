Namespace Business.Filters
    Public Class InvariantCharComparer
        Implements IEqualityComparer(Of Char)

        Public Function Equals1(x As Char, y As Char) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Char).Equals
            Return Char.ToUpperInvariant(x) = Char.ToUpperInvariant(y)
        End Function

        Public Function GetHashCode1(obj As Char) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Char).GetHashCode
            Return char.ToUpperInvariant(obj).GetHashCode()
        End Function
    End Class
End NameSpace