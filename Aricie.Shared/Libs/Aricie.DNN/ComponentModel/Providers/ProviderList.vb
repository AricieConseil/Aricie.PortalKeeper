Imports Aricie.Collections
Imports System.ComponentModel

Namespace ComponentModel
    <Serializable()> _
    Public Class ProviderList(Of T As {NamedConfig})
        Inherits SerializableList(Of T)

        <Browsable(False)> _
        Public ReadOnly Property Available() As IDictionary(Of String, T)
            Get
                Return GetAvailable(Me)
            End Get
        End Property

        Public Shared Function GetAvailable(ByVal items As IList(Of T)) As IDictionary(Of String, T)
            Dim toReturn As New SerializableDictionary(Of String, T)
            For Each objItem As T In items
                If objItem.Enabled Then
                    toReturn(objItem.Name) = objItem
                End If
            Next
            Return toReturn
        End Function


    End Class
End Namespace