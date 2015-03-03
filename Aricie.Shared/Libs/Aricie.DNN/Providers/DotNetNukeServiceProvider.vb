Imports Aricie.Providers
Imports System.Web.Caching
Imports DotNetNuke.Common.Utilities
Imports Aricie.Services
Imports Aricie.DNN.Services
Imports DotNetNuke.Entities.Modules

Namespace Providers
    ''' <summary>
    ''' Cache-enhanced SystemServiceProvider class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DotNetNukeServiceProvider
        Inherits SystemServiceProvider

        ''' <summary>
        ''' Returns cached object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetCache(ByVal key As String) As Object
            Return DataCache.GetCache(key)
        End Function

        ''' <summary>
        ''' Removes cached object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Public Overrides Sub RemoveCache(ByVal key As String)
            DataCache.RemoveCache(key)
        End Sub

        ''' <summary>
        ''' Sets cached object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Overrides Sub SetCache(ByVal key As String, ByVal value As Object)
            DataCache.SetCache(key, value)
        End Sub

        ''' <summary>
        ''' Sets cached object with dependency
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <param name="absoluteExpiration"></param>
        ''' <param name="dependencies"></param>
        ''' <remarks></remarks>
        Public Overrides Sub SetCacheDependant(ByVal key As String, ByVal value As Object, ByVal absoluteExpiration As TimeSpan, _
                                                 ByVal ParamArray dependencies() As String)


            If dependencies.Length = 0 Then
                If Not absoluteExpiration.Equals(Constants.Cache.NoExpiration) Then
                    DataCache.SetCache(key, value, Now.Add(absoluteExpiration))
                Else
                    DataCache.SetCache(key, value)
                End If
            Else
                If CacheHelper.GetCache(Constants.Cache.Dependency) Is Nothing Then
                    DataCache.SetCache(Constants.Cache.Dependency, True)
                End If
                'todo remove that horror when dnn is fixed
                Dim prefixDep As String
                If NukeHelper.DnnVersion.Major > 4 Then
                    prefixDep = "DNN_"
                Else
                    prefixDep = ""
                End If
                Dim objDependancy As CacheDependency = New CacheDependency(New String() {}, New String() {prefixDep & Constants.Cache.Dependency})
                Dim fileDeps As List(Of String) = Nothing
                Dim cacheDeps As New List(Of String)()
                For Each dep As String In dependencies
                    If Not String.IsNullOrEmpty(dep) Then
                        If dep.Substring(1).StartsWith(":\") OrElse dep.StartsWith("\\") Then
                            'JBB: Gestion du cas de fichiers sur le réseau
                            If fileDeps Is Nothing Then
                                fileDeps = New List(Of String)
                            End If
                            fileDeps.Add(dep)
                        Else
                            If Not dep.StartsWith("TabModules") AndAlso CacheHelper.GetCache(dep) Is Nothing Then
                                DataCache.SetCache(dep, True, objDependancy)
                            End If
                            cacheDeps.Add(prefixDep & dep)
                        End If
                    End If
                Next
                Dim objDependancy2 As CacheDependency
                If fileDeps Is Nothing Then
                    objDependancy2 = New CacheDependency(Nothing, cacheDeps.ToArray)
                Else
                    objDependancy2 = New CacheDependency(fileDeps.ToArray, cacheDeps.ToArray)
                End If


                If absoluteExpiration.Equals(Constants.Cache.NoExpiration) OrElse absoluteExpiration.Equals(TimeSpan.Zero) Then
                    DataCache.SetCache(key, value, objDependancy2)
                Else
                    DataCache.SetCache(key, value, objDependancy2, Now.Add(absoluteExpiration), Nothing)
                End If

            End If

        End Sub

        ''' <summary>
        ''' Log exception
        ''' </summary>
        ''' <param name="ex"></param>
        ''' <remarks></remarks>
        Public Overrides Sub LogException(ByVal ex As System.Exception)
            Dim currentModule As PortalModuleBase = DnnContext.Current.CurrentModule
            If currentModule IsNot Nothing Then
                If DnnContext.Current.User.IsSuperUser Then
                    DnnContext.Current.AddModuleMessage(ex.ToString, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Else
                    DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(currentModule, ex)
                End If
            Else
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End If
        End Sub
    End Class
End Namespace

