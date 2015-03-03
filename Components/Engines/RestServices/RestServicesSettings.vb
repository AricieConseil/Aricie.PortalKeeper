
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Configuration
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper


    <ActionButton(IconName.CloudDownload, IconOptions.Normal)> _
   <Serializable()> _
    Public Class RestServicesSettings

        Public Property Enabled As Boolean

        Public ReadOnly Property OpenRastaIsInstalled As Boolean
            Get
                Return ConfigHelper.IsInstalled(New PortalKeeperRastaConfigUpdate, True)
            End Get
        End Property

        Public ReadOnly Property DNNServicesAvailable As Boolean
            Get
                Return NukeHelper.DnnVersion.Major > 7
            End Get
        End Property

        Public Property EnableOpenRastaLogger As Boolean
        Public Property EnableDigestAuthentication As Boolean


        Public Property Services As New SimpleList(Of RestService)

        Public Function FindServiceByKey(resourceKey As Object) As RestService
            Dim toReturn As RestService = Nothing
            Dim resourceType As Type = TryCast(resourceKey, Type)
            If resourceType IsNot Nothing Then
                For Each objService As RestService In Me.Services.Instances
                    If objService.ResourceType.GetDotNetType() Is resourceType Then
                        toReturn = objService
                    End If
                Next
            End If
            Return toReturn
        End Function



        <ConditionalVisible("OpenRastaIsInstalled", True, True)> _
        <ActionButton(IconName.Anchor, IconOptions.Normal)> _
        Public Sub InstallOpenRasta(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            SyncLock Me
                ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Install, New PortalKeeperRastaConfigUpdate)
            End SyncLock
            ape.DisplayLocalizedMessage("InstalledOpenRasta.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ConditionalVisible("OpenRastaIsInstalled", False, True)> _
        <ActionButton(IconName.Anchor, IconOptions.Normal)> _
        Public Sub UninstallOpenRasta(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            SyncLock Me
                ConfigHelper.ProcessModuleUpdate(Configuration.ConfigActionType.Uninstall, New PortalKeeperRastaConfigUpdate)
            End SyncLock
            ape.DisplayLocalizedMessage("UninstallOpenRasta.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub


    End Class

End Namespace
