
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Reflection
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper


    <ActionButton(IconName.CloudDownload, IconOptions.Normal)> _
   <Serializable()> _
    Public Class RestServicesSettings

        Public ReadOnly Property DNNServicesAvailable As Boolean
            Get
                Return NukeHelper.DnnVersion.Major >= 7
            End Get
        End Property

        Public Property Enabled As Boolean

        'Public ReadOnly Property OpenRastaIsInstalled As Boolean
        '    Get
        '        Return ConfigHelper.IsInstalled(New PortalKeeperRastaConfigUpdate, True)
        '    End Get
        'End Property

       

        'Public Property EnableOpenRastaLogger As Boolean
        'Public Property EnableDigestAuthentication As Boolean


        'Public Property Services As SimpleList(Of RestService)
        '    Get
        '        Return Nothing
        '    End Get
        '    Set(value As SimpleList(Of RestService))
        '        Me.RestServices = value.Instances
        '    End Set
        'End Property

        Public Property RestServices As New List(Of RestService)



        ' Private _ServicesByControllerName As Dictionary(Of String, RestService)

        ' <XmlIgnore()> _
        '<Browsable(False)> _
        ' Public ReadOnly Property ServicesByControllerName As Dictionary(Of String, RestService)
        '     Get
        '         If _ServicesByControllerName Is Nothing Then
        '             SyncLock Me
        '                 Dim newDico As New Dictionary(Of String, RestService)
        '                 For Each objSettings As RestService In RestServices
        '                     If objSettings.Enabled Then
        '                         For Each objController As DynamicControllerInfo In objSettings.DynamicControllers
        '                             If objController.Enabled Then
        '                                 newDico(objController.Name) = objSettings
        '                             End If
        '                         Next
        '                     End If
        '                 Next
        '                 _ServicesByControllerName = newDico
        '             End SyncLock
        '         End If
        '         Return _ServicesByControllerName
        '     End Get
        ' End Property



        'Private Shared _ReflectedRegisterMethod As MethodInfo

        'Private Shared ReadOnly Property ReflectedRegisterMethod As MethodInfo
        '    Get
        '        If _ReflectedRegisterMethod Is Nothing Then
        '            Dim objModuleType As Type = ReflectionHelper.CreateType("Aricie.PortalKeeper.DNN7.WebAPI.WebApiConfig, Aricie.PortalKeeper.DNN7")
        '            _ReflectedRegisterMethod = DirectCast(ReflectionHelper.GetMember(objModuleType, "RegisterWebHosted"), MethodInfo)
        '        End If
        '        Return _ReflectedRegisterMethod
        '    End Get
        'End Property


        Public Sub RegisteWebAPIServices()
            ObsoleteDotNetProvider.Instance.RegisterWebAPI()

            'ReflectedRegisterMethod.Invoke(Nothing, Nothing)
        End Sub




        'Public Function FindServiceByKey(resourceKey As Object) As RestService
        '    Dim toReturn As RestService = Nothing
        '    Dim resourceType As Type = TryCast(resourceKey, Type)
        '    If resourceType IsNot Nothing Then
        '        For Each objService As RestService In Me.Services.Instances
        '            If objService.ResourceType.GetDotNetType() Is resourceType Then
        '                toReturn = objService
        '            End If
        '        Next
        '    End If
        '    Return toReturn
        'End Function

        'Public Function FindServiceByKey(resourceKey As Object) As RestService
        '    Dim toReturn As RestService = Nothing
        '    Dim resourceType As Type = TryCast(resourceKey, Type)
        '    If resourceType IsNot Nothing Then
        '        For Each objService As RestService In Me.Services.Instances
        '            If objService.ResourceType.GetDotNetType() Is resourceType Then
        '                toReturn = objService
        '            End If
        '        Next
        '    End If
        '    Return toReturn
        'End Function



        '<ConditionalVisible("OpenRastaIsInstalled", True, True)> _
        '<ActionButton(IconName.Anchor, IconOptions.Normal)> _
        'Public Sub InstallOpenRasta(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
        '    SyncLock Me
        '        ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Install, New PortalKeeperRastaConfigUpdate)
        '    End SyncLock
        '    ape.DisplayLocalizedMessage("InstalledOpenRasta.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
        'End Sub

        '<ConditionalVisible("OpenRastaIsInstalled", False, True)> _
        '<ActionButton(IconName.Anchor, IconOptions.Normal)> _
        'Public Sub UninstallOpenRasta(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
        '    SyncLock Me
        '        ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Uninstall, New PortalKeeperRastaConfigUpdate)
        '    End SyncLock
        '    ape.DisplayLocalizedMessage("UninstallOpenRasta.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
        'End Sub


    End Class

End Namespace
