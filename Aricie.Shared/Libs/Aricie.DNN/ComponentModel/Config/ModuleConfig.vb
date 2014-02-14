Imports Aricie.DNN.Services
Imports System.IO
Imports System.Web
Imports Aricie.Services
Imports System.Globalization
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Settings

'-------------------------------------------------------------------------------
' 28/03/2011 - [JBB] - Modification du path.combine pour gérer le cas des chemins UNC
'-------------------------------------------------------------------------------
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Services.Localization

Namespace ComponentModel


    ''' <summary>
    ''' Base Module configuration class
    ''' </summary>
    ''' <remarks>Basic capabilitites to generate a custom filename</remarks>
    <Serializable()> _
    Public Class ModuleConfig


        <NonSerialized()> _
        Private Shared _FilePaths As New Dictionary(Of String, String)

        Private Shared Function GetFileName(ByVal moduleName As String) As String
            Return moduleName & ".config"
        End Function

        Protected Shared Function GetFilePath(ByVal moduleName As String, ByVal locSettings As LocationSettings, ByVal useCache As Boolean) As String
            Dim toReturn As String = Nothing
            Dim key As String = moduleName & locSettings.UserFile.ToString(CultureInfo.InvariantCulture)
            If Not _FilePaths.TryGetValue(key, toReturn) Then
                SyncLock _FilePaths
                    If Not _FilePaths.TryGetValue(key, toReturn) Then
                        Dim root As String
                        If locSettings.UserFile Then
                            If String.IsNullOrEmpty(locSettings.UserFileName) Then
                                locSettings.UserFileName = moduleName
                            End If
                            root = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(locSettings.UserFileName, True)
                            'root = Path.Combine(DotNetNuke.Common.Globals.HostMapPath, locSettings.UserFileName)
                        Else
                            root = NukeHelper.GetModuleDirectoryMapPath(moduleName, useCache)
                        End If
                        toReturn = String.Format("{0}\{1}", root.TrimEnd("\"c), GetFileName(moduleName).TrimStart("\"c))
                        _FilePaths(key) = toReturn
                    End If
                End SyncLock
            End If
            Return toReturn
        End Function

        Public Overridable Sub Save(ByVal moduleName As String, ByVal locSettings As LocationSettings, ByVal useBinarySnapShot As Boolean)

            Dim fileName As String = GetFilePath(moduleName, locSettings, False)
            Aricie.DNN.Settings.SettingsController.SaveFileSettings(fileName, Me, useBinarySnapShot, locSettings.BackupsNb)

        End Sub


    End Class





    ''' <summary>
    ''' Generic self referencing module configuration class
    ''' </summary>
    ''' <remarks>Has self file load/save capabilities</remarks>
    <Serializable()> _
    Public Class ModuleConfig(Of TConfigClass As {New, ModuleConfig(Of TConfigClass)})
        Inherits ModuleConfig



        Protected Shared Function GetInstance(ByVal moduleName As String, ByVal locSettings As LocationSettings) As TConfigClass
            Return GetInstance(moduleName, locSettings, True, False)
        End Function

        Protected Shared Function GetInstance(ByVal moduleName As String, ByVal locSettings As LocationSettings, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As TConfigClass
            Return Aricie.DNN.Settings.SettingsController.LoadFileSettings(Of TConfigClass)(GetFilePath(moduleName, locSettings, useCache), useCache, useBinarySnapShot)
        End Function


        Public Overloads Shared Sub Save(ByVal moduleName As String, ByVal instance As TConfigClass, ByVal locSettings As LocationSettings, ByVal useBinarySnapShot As Boolean)

            'Dim fileName As String = GetFilePath(moduleName, locSettings, False)
            'Aricie.DNN.Settings.SettingsController.SaveFileSettings(Of TConfigClass)(fileName, instance, useBinarySnapShot, locSettings.BackupsNb)
            instance.Save(moduleName, locSettings, useBinarySnapShot)

        End Sub

        Public Overridable Function GetDefaultConfig() As TConfigClass
            Return New TConfigClass
        End Function


    End Class

    ''' <summary>
    ''' Generic self referencing with identification module configuration class
    ''' </summary>
    ''' <remarks>Has all module configuration capabilities.</remarks>
    <Serializable()> _
    Public Class ModuleConfig(Of TConfigClass As {New, ModuleConfig(Of TConfigClass)}, TModuleIdentity As {IModuleIdentity, New})
        Inherits ModuleConfig(Of TConfigClass)
        Implements ISelector


        <Browsable(False)> _
        <XmlIgnore()> _
        Public Shared Property SharedLocationSettings(ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As LocationSettings
            Get
                Return SettingsController.LoadFileSettings(Of LocationSettings)(GetLocationFileName(useCache), useCache, useBinarySnapShot)
            End Get
            Set(ByVal value As LocationSettings)
                If value IsNot Nothing AndAlso Not value.Equals(SharedLocationSettings(useCache, useBinarySnapShot)) Then
                    SettingsController.SaveFileSettings(Of LocationSettings)(GetLocationFileName(useCache), value, useBinarySnapShot)
                End If
            End Set
        End Property


        Private _LocationSettings As LocationSettings


        <ExtendedCategory("", "LocationSettings")> _
        <XmlIgnore()> _
        Public Property LocationSettings() As LocationSettings
            Get
                If _LocationSettings Is Nothing Then
                    _LocationSettings = ReflectionHelper.CloneObject(Of LocationSettings)(SharedLocationSettings(True, False))
                End If
                Return _LocationSettings
            End Get
            Set(ByVal value As LocationSettings)
                Me._LocationSettings = value
            End Set
        End Property

        <ExtendedCategory("", "LocationSettings")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Text", "Value", False, True, "---", "", False, False)> _
        <XmlIgnore()> _
        Public Property BackupToRestore() As String = ""


        <ConditionalVisible("BackupToRestore", True, True, "")> _
        <ExtendedCategory("", "LocationSettings")> _
        <ActionButton(IconName.Refresh, IconOptions.Normal, "RestoreBackup.Warning")> _
        Public Sub RestoreBackup(pe As AriciePropertyEditorControl)
            If SettingsController.RestoreBackup(GetFilePath(True), BackupToRestore) Then
                pe.ItemChanged = True
                pe.DataSource = Instance
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("BackupRestored.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
            Else
                pe.ItemChanged = True
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("BackupNotRestored.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

        <ExtendedCategory("", "LocationSettings")> _
        <ActionButton(IconName.FloppyO, IconOptions.Normal, "Save Current Form?")> _
        Public Sub SaveLocationSettings(pe As AriciePropertyEditorControl)
            If pe.IsValid Then
                SharedLocationSettings(True, False) = Me.LocationSettings
                Me._LocationSettings = Nothing
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("LocationSettingsSaved.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
                Me.Save(pe)
            End If
        End Sub

        Public Overloads Overrides Sub Save(moduleName As String, locSettings As LocationSettings, useBinarySnapShot As Boolean)
            MyBase.Save(moduleName, locSettings, useBinarySnapShot)
            _Instance = Nothing
        End Sub

        Private Shared _Instance As TConfigClass


        Public Shared ReadOnly Property Instance() As TConfigClass
            Get
                If _Instance Is Nothing Then
                    _Instance = Instance(SharedLocationSettings(True, False), False, False)
                End If
                Return _Instance
            End Get
        End Property

        Public Shared ReadOnly Property Instance(ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As TConfigClass
            Get
                Return Instance(SharedLocationSettings(useCache, useBinarySnapShot), useCache, useBinarySnapShot)
            End Get
        End Property

        Public Shared ReadOnly Property Instance(ByVal locSettings As LocationSettings) As TConfigClass
            Get
                Return Instance(locSettings, True, False)
            End Get
        End Property

        Public Shared ReadOnly Property Instance(ByVal locSettings As LocationSettings, ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As TConfigClass
            Get
                Dim toReturn As TConfigClass = GetInstance(Identity.GetModuleName, locSettings, useCache, useBinarySnapShot)
                Return toReturn
            End Get
        End Property

        <ActionButton(IconName.FloppyO, IconOptions.Normal)> _
        Public Overridable Overloads Sub Save(pe As AriciePropertyEditorControl)
            If pe.IsValid Then
                Me.Save(Identity.GetModuleName(), SharedLocationSettings(True, False), False)
                ReflectionHelper.MergeObjects(Instance, Me)
                pe.ItemChanged = True
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("ModuleConfigSaved.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("ModuleConfigInvalid.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
            End If
        End Sub


        Public Overridable Overloads Sub Save()
            Me.Save(Identity.GetModuleName(), SharedLocationSettings(True, False), False)
        End Sub

        <ActionButton(IconName.FloppyO, IconOptions.Stack1X, IconName.Ban, IconOptions.Stack2X, IconOptions.Normal)> _
        Public Overridable Overloads Sub Cancel(pe As AriciePropertyEditorControl)
            pe.Page.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL())
        End Sub

        <ActionButton(IconName.TrashO, IconOptions.Normal, "Reset.Warning")> _
        Public Overridable Overloads Sub Reset(pe As AriciePropertyEditorControl)
            Reset(SharedLocationSettings(True, False))
            Try
                ReflectionHelper.MergeObjects(Instance, Me)
                pe.ItemChanged = True
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(pe.ParentModule, Localization.GetString("ModuleConfigReset.Message", pe.LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch ex As Exception
                ExceptionHelper.LogException(ex)
                pe.Page.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL())
            End Try
        End Sub

        Public Overloads Shared Sub Reset()
            Reset(SharedLocationSettings(True, False))
        End Sub


        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass)

            Save(objInstance, False)
            'SharedLocationSettings = locSettings

        End Sub

        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal useBinarySnapShot As Boolean)

            Save(objInstance, SharedLocationSettings(True, False), useBinarySnapShot)
            'SharedLocationSettings = locSettings

        End Sub


        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal locSettings As LocationSettings)

            Save(objInstance, locSettings, False)


        End Sub

        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal locSettings As LocationSettings, ByVal useBinarySnapShot As Boolean)

            Save(Identity.GetModuleName, objInstance, locSettings, useBinarySnapShot)


        End Sub




        Public Overloads Shared Sub Reset(ByVal locSettings As LocationSettings)
            Save(Instance(locSettings).GetDefaultConfig, locSettings)
        End Sub



        Private Shared Function GetLocationFileName(ByVal useCache As Boolean) As String
            Dim configFileName As String = GetFilePath(Identity.GetModuleName, LocationSettings.CoreFile, useCache)
            configFileName = configFileName & ".Loc.config"
            Return configFileName
        End Function


        Public Overloads Shared Function GetFilePath(ByVal useCache As Boolean) As String
            Return GetFilePath(SharedLocationSettings(useCache, False), useCache)
        End Function

        Public Overloads Shared Function GetFilePath(ByVal locSettings As LocationSettings, ByVal useCache As Boolean) As String
            Return ModuleConfig.GetFilePath(Identity.GetModuleName, locSettings, useCache)
        End Function


        Protected Shared ReadOnly Property Identity() As TModuleIdentity
            Get
                Return ReflectionHelper.GetSingleton(Of TModuleIdentity)()
            End Get
        End Property

        Private Function GetBackupFiles() As IList(Of FileInfo)
            Return SettingsController.GetBackups(GetFilePath(True))
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "BackupToRestore"
                    Return (From backupfile In Me.GetBackupFiles().OrderBy(Function(objFile As FileInfo) objFile.LastAccessTime) _
                    Select New ListItem(backupfile.Name, backupfile.Name)).ToList()
            End Select
            Return Nothing
        End Function

        Public Overrides Function GetDefaultConfig() As TConfigClass
            Return SettingsController.LoadFileSettings(Of TConfigClass)(SettingsController.GetDefaultFileName(GetFilePath(True)), False)
        End Function

    End Class
End Namespace



