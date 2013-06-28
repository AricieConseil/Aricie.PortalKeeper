Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Modules
Imports System.Web.UI
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins
Imports Aricie.Services
Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports System.Globalization
Imports Aricie.DNN.Services
Imports System.Reflection


Namespace Configuration

    Public Enum TrustLevel
        Full
        Medium
    End Enum



    ''' <summary>
    ''' The main Helper for configuration operations.
    ''' </summary>
    ''' <remarks>Can be fed a configuration provider, combines the Xml operations and executes all operations in sequence.</remarks>
    Public Module ConfigHelper

        Public Sub AssertIsInstalled(ByVal moduleBase As PortalModuleBase, ByVal updateProvider As IUpdateProvider, ByVal installControl As Control, ByVal uninstallControl As Control)
            Dim controlsToShowForUnInstall As New List(Of Control)
            controlsToShowForUnInstall.Add(uninstallControl)
            AssertIsInstalled(moduleBase, updateProvider, installControl, controlsToShowForUnInstall)
        End Sub

        Public Sub AssertIsInstalled(ByVal moduleBase As PortalModuleBase, ByVal updateProvider As IUpdateProvider, ByVal installControl As Control, ByVal controlsToShowForUnInstall As IList(Of Control))
            Dim controlsToShowForInstall As New List(Of Control)
            controlsToShowForInstall.Add(installControl)
            AssertIsInstalled(moduleBase, updateProvider, controlsToShowForInstall, controlsToShowForUnInstall)
        End Sub

        Public Sub AssertIsInstalled(ByVal moduleBase As PortalModuleBase, ByVal updateProvider As IUpdateProvider, ByVal controlsToShowForInstall As IList(Of Control), ByVal controlsToShowForUnInstall As IList(Of Control))
            If IsInstalled(updateProvider, True) Then
                If moduleBase.Session("ComponentsInstalled" & moduleBase.ModuleId.ToString(CultureInfo.InvariantCulture)) Is Nothing Then
                    moduleBase.Session("ComponentsInstalled" & moduleBase.ModuleId.ToString(CultureInfo.InvariantCulture)) = True
                    Skin.AddModuleMessage(moduleBase, Localization.GetString("ComponentsInstalled.Message", moduleBase.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
                End If

                For Each installControl As Control In controlsToShowForInstall
                    installControl.Visible = False
                Next
                For Each uninstallControl As Control In controlsToShowForUnInstall
                    uninstallControl.Visible = True
                Next
            Else
                Skin.AddModuleMessage(moduleBase, Localization.GetString("ComponentsNotInstalled.Message", moduleBase.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                For Each installControl As Control In controlsToShowForInstall
                    installControl.Visible = True
                Next
                For Each uninstallControl As Control In controlsToShowForUnInstall
                    uninstallControl.Visible = False
                Next
            End If
        End Sub


        Public Function IsInstalled(ByVal updateProvider As IUpdateProvider, ByVal cacheResult As Boolean) As Boolean
            Dim dimDico As Dictionary(Of String, Boolean) = Nothing
            If cacheResult Then
                dimDico = GetGlobal(Of Dictionary(Of String, Boolean))("updateProvider")
                If dimDico IsNot Nothing Then
                    If dimDico.ContainsKey(updateProvider.GetType.FullName) Then
                        Return dimDico(updateProvider.GetType.FullName)
                    End If
                End If

            End If

            Dim toReturn As Boolean = True

            Dim configElements As List(Of IConfigElementInfo) = updateProvider.GetConfigElements
            If configElements IsNot Nothing Then
                For Each configElement As IConfigElementInfo In configElements
                    If Not configElement.IsInstalled() Then
                        toReturn = False
                        Exit For
                    End If
                Next
            End If


            If cacheResult Then
                If dimDico Is Nothing Then
                    dimDico = New Dictionary(Of String, Boolean)
                End If
                dimDico(updateProvider.GetType.FullName) = toReturn
                CacheHelper.SetCacheDependant(Of Dictionary(Of String, Boolean))(dimDico, Constants.Cache.Dependency, Constants.Cache.GlobalExpiration, "updateProvider")
            End If


            Return toReturn
        End Function


        Public Sub ProcessModuleUpdate(ByVal action As ConfigActionType, ByVal updateProvider As IUpdateProvider)


            Dim nodes As New NodesInfo
            Dim configElements As List(Of IConfigElementInfo) = updateProvider.GetConfigElements
            If configElements IsNot Nothing Then
                For Each configElement As IConfigElementInfo In configElements
                    If TypeOf configElement Is XmlConfigElementInfo Then
                        Dim xmlConfig As XmlConfigElementInfo = DirectCast(configElement, XmlConfigElementInfo)
                        Dim addElement As Boolean = True
                        If action = ConfigActionType.Uninstall Then
                            addElement = configElement.IsInstalled
                        Else
                            addElement = Not configElement.IsInstalled
                        End If
                        If addElement Then
                            xmlConfig.AddConfigNodes(nodes, action)
                        End If
                    Else
                        configElement.ProcessConfig(action)
                    End If
                Next
            End If
            If nodes.Nodes.Count > 0 Then
                Dim webConfigMerge As New XmlConfigInfo
                webConfigMerge.NodesList.Add(nodes)
                ProcessConfig(webConfigMerge)
            End If

            RemoveCache(Constants.Cache.Dependency)
        End Sub

        Public Sub SwitchTrust(ByVal level As TrustLevel, Optional ByVal originUrl As String = ".*")

            Dim xmlConfig As XmlDocument = Config.Load()

            Dim trustModuleNode As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/trust[@originUrl='" & originUrl & "']")
            If trustModuleNode IsNot Nothing Then

                trustModuleNode.Attributes("level").Value = level.ToString

            Else
                Dim trust As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web")
                Dim trustNode As XmlNode = xmlConfig.CreateNode(XmlNodeType.Element, "", "trust", "")
                Dim levelAttr As XmlAttribute = xmlConfig.CreateAttribute("level")
                levelAttr.Value = level.ToString
                Dim originUrlAttr As XmlAttribute = xmlConfig.CreateAttribute("originUrl")
                originUrlAttr.Value = ".*"
                trustNode.Attributes.Append(levelAttr)
                trustNode.Attributes.Append(originUrlAttr)
                trust.AppendChild(trustNode)

            End If

            Config.Save(xmlConfig)

        End Sub

        Public Sub SwitchRunAllManagedModulesForAllRequests(ByVal enable As Boolean)

            Dim xmlConfig As XmlDocument = Config.Load()

            Dim modulesNode As XmlNode = xmlConfig.SelectSingleNode("configuration/system.webServer/modules")
            If modulesNode IsNot Nothing Then
                Dim objAttr As XmlAttribute = modulesNode.Attributes("runAllManagedModulesForAllRequests")
                If objAttr Is Nothing Then
                    objAttr = xmlConfig.CreateAttribute("runAllManagedModulesForAllRequests")
                    objAttr.Value = enable.ToString(CultureInfo.InvariantCulture)
                    modulesNode.Attributes.Append(objAttr)
                Else
                    objAttr.Value = enable.ToString(CultureInfo.InvariantCulture)
                End If
            End If


            Config.Save(xmlConfig)


        End Sub

#Region "Private methods"



        Public Sub ProcessConfig(ByVal objConfig As XmlConfigInfo)

            Dim strConfig As XmlDocument = ReflectionHelper.Serialize(objConfig, True)
            Dim xmlMergeType As Type
            Select Case DnnVersion.Major
                Case Is > 4
                    xmlMergeType = ReflectionHelper.CreateType("DotNetNuke.Services.Installer.XmlMerge, DotNetNuke")
                Case Else
                    xmlMergeType = ReflectionHelper.CreateType("DotNetNuke.Services.Packages.XmlMerge, DotNetNuke")
            End Select

            Dim merger As Object = Activator.CreateInstance(xmlMergeType, strConfig, "", GetType(ConfigHelper).FullName)
            Dim updateConfigMethod As MethodInfo = DirectCast(ReflectionHelper.GetMember(xmlMergeType, "UpdateConfigs"), MethodInfo)
            updateConfigMethod.Invoke(merger, Nothing)
        End Sub

#End Region















    End Module


End Namespace
