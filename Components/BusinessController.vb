Imports Aricie.DNN.Services
Imports Aricie.Services
Imports Aricie.DNN.Settings
Imports DotNetNuke.Entities.Modules
Imports System.Xml

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class BusinessController
        Implements IPortable

        Public Const MODULE_NAME As String = "Aricie.PortalKeeper"

        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements IPortable.ExportModule
            'Dim pid As Integer = NukeHelper.GetPortalIdByModuleId(ModuleID)
            'Dim params As FirewallSettings = SettingsController.GetModuleSettings(Of FirewallSettings)(SettingsScope.PortalSettings, pid)
            Dim params As PortalKeeperConfig = PortalKeeperConfig.Instance
            Dim doc As XmlDocument = ReflectionHelper.Serialize(params, True)
            Return doc.OuterXml
        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements IPortable.ImportModule
            Dim settings As PortalKeeperConfig = ReflectionHelper.Deserialize(Of PortalKeeperConfig)(Content)
            'Dim pid As Integer = NukeHelper.GetPortalIdByModuleId(ModuleID)
            'Aricie.DNN.Settings.SettingsController.SetModuleSettings(Of FirewallSettings)(SettingsScope.PortalSettings, pid, settings)
            PortalKeeperConfig.Save(settings)
        End Sub


    End Class

End Namespace


