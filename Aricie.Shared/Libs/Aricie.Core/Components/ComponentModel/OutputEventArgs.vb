Imports System

Namespace ComponentModel
    Public Class OutputEventArgs
        Inherits EventArgs
        Public Property Value As String

        Public Sub New(ByVal value As String)
            MyBase.New()
            Me.Value = value
        End Sub
    End Class
End Namespace