Imports System.Xml
Imports Aricie.DNN.Services
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.Skins.Controls

Namespace Configuration

  
    ''' <summary>
    ''' This is the base element for an Xml based installable component
    ''' </summary>
    ''' <remarks>Uses xml merge files</remarks>
    <Serializable()> _
    Public MustInherit Class XmlConfigElementInfo
        Implements IConfigElementInfo


        Public Overridable ReadOnly Property Installed As Boolean
            Get
                Return IsInstalled()
            End Get
        End Property

        Public MustOverride Overloads Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean


        Public MustOverride Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

        Public Overloads Function IsInstalled() As Boolean Implements IConfigElementInfo.IsInstalled
            Return IsInstalled(NukeHelper.WebConfigDocument)
        End Function

        <ConditionalVisible("Installed", True, True)> _
        <ActionButton(IconName.FloppyO, IconOptions.Normal)> _
        Public Overridable Overloads Sub Install(pe As AriciePropertyEditorControl)
            If pe.IsValid Then
                If Not Me.Installed Then
                    Me.ProcessConfig(ConfigActionType.Install)

                    pe.ItemChanged = True
                    pe.DisplayLocalizedMessage("ConfigElementInstalled.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    pe.DisplayLocalizedMessage("ConfigElementAlreadyInstalled.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                End If

            Else
                pe.DisplayLocalizedMessage("InvalidConfigElement.Message", ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

        <ConditionalVisible("Installed", False, True)> _
        <ActionButton(IconName.Times, IconOptions.Normal)> _
        Public Overridable Overloads Sub Uninstall(pe As AriciePropertyEditorControl)
            If pe.IsValid Then

                If Me.Installed Then
                    Me.ProcessConfig(ConfigActionType.Uninstall)

                    pe.ItemChanged = True
                    pe.DisplayLocalizedMessage("ConfigElementUninstalled.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    pe.DisplayLocalizedMessage("ConfigElementAlreadyUninstalled.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                End If

            Else
                pe.DisplayLocalizedMessage("InvalidConfigElement.Message", ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

        <ConditionalVisible("Installed", False, True)> _
       <ActionButton(IconName.Refresh, IconOptions.Normal)> _
        Public Overridable Overloads Sub Update(pe As AriciePropertyEditorControl)
            If pe.IsValid Then

                If Me.Installed Then
                    Me.ProcessConfig(ConfigActionType.Reinstall)

                    pe.ItemChanged = True
                    pe.DisplayLocalizedMessage("ConfigElementUpdated.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    pe.DisplayLocalizedMessage("ConfigElementAlreadyUninstalled.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
                End If

            Else
                pe.DisplayLocalizedMessage("InvalidConfigElement.Message", ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub


        Public Sub ProcessConfig(ByVal actionType As ConfigActionType) Implements IConfigElementInfo.ProcessConfig

            Dim nodes As New NodesInfo

            Dim addElement As Boolean = True
            If actionType = ConfigActionType.Install Then
                addElement = Not Me.IsInstalled
            Else
                addElement = Me.IsInstalled
            End If
            If addElement Then
                If actionType = ConfigActionType.Reinstall Then
                    Me.AddConfigNodes(nodes, ConfigActionType.Uninstall)
                    Me.AddConfigNodes(nodes, ConfigActionType.Install)
                Else
                    Me.AddConfigNodes(nodes, actionType)
                End If
            End If
            If nodes.Nodes.Count > 0 Then
                Dim webConfigMerge As New XmlConfigInfo
                webConfigMerge.NodesList.Add(nodes)
                ConfigHelper.ProcessConfig(webConfigMerge)
            End If

        End Sub

    End Class
End Namespace


