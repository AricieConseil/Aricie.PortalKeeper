Imports Aricie.Services

Namespace ComponentModel
    Public Class ProviderConfigHelper(Of T As {ProviderConfig, New})


        Public Shared Function GetNew(ByVal name As String, ByVal description As String, ByVal providerType As Type) As T
            Dim toReturn As New T
            toReturn.Name = name
            toReturn.Decription = description
            toReturn.TypeName = ReflectionHelper.GetSafeTypeName(providerType)
            Return toReturn
        End Function
    End Class
End Namespace