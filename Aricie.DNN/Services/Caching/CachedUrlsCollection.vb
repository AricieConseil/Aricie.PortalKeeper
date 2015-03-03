
Imports System.Configuration

Namespace Services.Caching
    <ConfigurationCollection(GetType(CachedUrlsElement))> _
    Public Class CachedUrlsCollection
        Inherits ConfigurationElementCollection
        ' Methods
        Public Sub Add(ByVal element As CachedUrlsElement)
            Me.BaseAdd(element)
        End Sub

        Public Sub Clear()
            MyBase.BaseClear()
        End Sub

        Protected Overrides Function CreateNewElement() As ConfigurationElement
            Return New CachedUrlsElement
        End Function

        Protected Overrides Function GetElementKey(ByVal element As ConfigurationElement) As Object
            Return DirectCast(element, CachedUrlsElement).Path
        End Function

        Public Sub Remove(ByVal key As String)
            MyBase.BaseRemove(key)
        End Sub

    End Class
End Namespace

