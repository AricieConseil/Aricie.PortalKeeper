Namespace Diagnostics
    Public Class DebugEventArgs
        Inherits EventArgs

        Public Sub New()

        End Sub

        Public Sub New(key As String, value As Object)
            Me.Context.Add(key, value)
        End Sub

        Public Property Context As New Dictionary(Of String, Object)

        Public Function GetAs(Of T)(key As String) As T
            Dim toReturn As Object = Nothing
            If Context.TryGetValue(key, toReturn) Then
                Return DirectCast(toReturn, T)
            End If
            Return Nothing
        End Function

    End Class
End NameSpace