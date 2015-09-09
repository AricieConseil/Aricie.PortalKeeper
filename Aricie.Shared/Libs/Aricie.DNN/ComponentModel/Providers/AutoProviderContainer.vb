Imports System.Reflection
Imports Aricie.Services

Namespace ComponentModel
    Public Class AutoProviderContainer(Of T)
        Inherits SimpleProviderContainer(Of T)


        Public Overloads Overrides Function GetNewItem(providerId As String) As T
            Return ReflectionHelper.CreateObject(Of T)(providerId)
        End Function

        Public Overrides Function GetProviderIdsByNames() As IEnumerable(Of KeyValuePair(Of String, String))
            Dim toReturn As New Dictionary(Of String, String)
            For Each objAssembly As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                Try
                    For Each objType As Type In objAssembly.GetTypes()
                        Try
                            If objType.IsVisible AndAlso Not objType.IsAbstract AndAlso GetType(T).IsAssignableFrom(objType) Then
                                toReturn.Add(objType.Name, objType.AssemblyQualifiedName)
                            End If
                        Catch ex As Exception
                            'ExceptionHelper.LogException(New ApplicationException("Problem Loading Type " & objType.AssemblyQualifiedName, ex))
                            'swallow exceptions
                            'todo: use obsoletednn provider to make use of cached safedirectorycatalog in recent dnn versions.
                        End Try

                    Next
                Catch ex As Exception
                    'ExceptionHelper.LogException(New ApplicationException("Problem Loading Types from Assembly " & objAssembly.FullName, ex))
                    'swallow exceptions
                    'todo: use obsoletednn provider to make use of cached safedirectorycatalog in recent dnn versions.
                End Try
            Next
            Return toReturn
        End Function
    End Class
End NameSpace