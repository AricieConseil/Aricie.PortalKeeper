Imports Aricie.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class MessageEventArgs
        Inherits GenericEventArgs(Of KeyValuePair(Of String, Object))

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal key As String, ByVal value As Object)
            MyBase.New(New KeyValuePair(Of String, Object)(key, value))
        End Sub

    End Class
End NameSpace