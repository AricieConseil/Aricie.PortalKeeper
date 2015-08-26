
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
Imports DotNetNuke.UI.Skins.Controls

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



        Public Property RestServices As New List(Of RestService)


        Public Overloads Sub RegisterRestServices()
            ObsoleteDotNetProvider.Instance.RegisterWebAPI()

        End Sub


        'todo: figure out a way to perform the registration again
        '<ActionButton(IconName.Magic, IconOptions.Normal)> _
        'Public Overloads Sub RegisterRestServices(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)



        '    ape.ItemChanged = True
        '    ape.DisplayLocalizedMessage("RestServicesRegistered.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
        'End Sub

       



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
