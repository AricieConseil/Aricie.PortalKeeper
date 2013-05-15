Imports OpenRasta.Pipeline.Contributors
Imports OpenRasta.DI
Imports OpenRasta.Configuration.Fluent
Imports OpenRasta.Diagnostics
Imports OpenRasta.Hosting.AspNet
Imports OpenRasta.Configuration
Imports OpenRasta.Security

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class RestServicesConfiguration
        Implements IConfigurationSource

        Public Sub Configure() Implements OpenRasta.Configuration.IConfigurationSource.Configure
            Try
                Dim restSettings As RestServicesSettings = PortalKeeperConfig.Instance.FirewallConfig.RestServices
                If restSettings.Enabled Then
                    Using (OpenRastaConfiguration.Manual)
                        If restSettings.EnableDigestAuthentication Then
                            'ResourceSpace.Uses.CustomDependency(Of IAuthenticationProvider, DNNAuthenticationProvider)(DependencyLifetime.Singleton)
                            'ResourceSpace.Uses.PipelineContributor(Of AuthenticationContributor)()
                        End If
                        If restSettings.EnableOpenRastaLogger Then
                            ResourceSpace.Uses.CustomDependency(Of ILogger(Of AspNetLogSource), OpenRastaLogger(Of AspNetLogSource))(DependencyLifetime.Singleton)
                            ResourceSpace.Uses.CustomDependency(Of ILogger, OpenRastaLogger)(DependencyLifetime.Singleton)
                        End If
                        For Each objService As RestService In restSettings.Services.Instances
                            If objService.Enabled Then
                                Dim serviceDef As IUriDefinition = ResourceSpace.Has.ResourcesOfType(objService.ResourceType.GetDotNetType).AtUri(objService.AtUri)
                                For Each alternateUri As String In objService.AlternateUris
                                    serviceDef.And.AtUri(alternateUri)
                                Next
                                Dim handlerDef As IHandlerForResourceWithUriDefinition = serviceDef.HandledBy(objService.HandlerType.GetDotNetType)
                                If objService.AsJsonDataContract Then
                                    handlerDef.AsJsonDataContract()
                                End If
                                If objService.AsXmlDataContract Then
                                    handlerDef.AsXmlDataContract()
                                End If
                                If objService.AsXmlSerializer Then
                                    handlerDef.AsXmlSerializer()
                                End If
                            End If
                        Next
                    End Using
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
        End Sub


    End Class

    Public Class DependencyResolverAccessor
        Implements IDependencyResolverAccessor




        Public ReadOnly Property Resolver As OpenRasta.DI.IDependencyResolver Implements OpenRasta.DI.IDependencyResolverAccessor.Resolver
            Get
                Dim toReturn As New InternalDependencyResolver
                toReturn.AddDependency(Of IDependencyRegistrar, AricieDependencyRegistrar)()
                Return toReturn
            End Get
        End Property
    End Class

    Public Class AricieDependencyRegistrar
        Inherits DefaultDependencyRegistrar

        Protected Overrides Sub RegisterLogging(resolver As OpenRasta.DI.IDependencyResolver)
            resolver.AddDependency(GetType(ILogger), GetType(OpenRastaLogger), DependencyLifetime.Singleton)
        End Sub

        Protected Overrides Sub AddLogSources()
            MyBase.AddLogSources()
            LogSourcedLoggerType = GetType(OpenRastaLogger(Of ))
        End Sub

    End Class

End Namespace