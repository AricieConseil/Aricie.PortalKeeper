Imports Aricie.Services

Namespace ComponentModel
    Public MustInherit Class ReflectedProviderContainer(Of T)
        Inherits SimpleProviderContainer(Of T)


        Public Overloads Overrides Function GetNewItem(providerId As String) As T
            Return ReflectionHelper.CreateObject(Of T)(providerId)
        End Function

        
    End Class
End NameSpace