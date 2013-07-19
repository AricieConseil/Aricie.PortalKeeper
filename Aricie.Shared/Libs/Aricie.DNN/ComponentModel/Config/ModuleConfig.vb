Imports Aricie.DNN.Services
Imports System.IO
Imports System.Web
Imports Aricie.Services
Imports System.Globalization
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Settings

'-------------------------------------------------------------------------------
' 28/03/2011 - [JBB] - Modification du path.combine pour gérer le cas des chemins UNC
'-------------------------------------------------------------------------------
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
                        'toReturn = Path.Combine(root, GetFileName(moduleName))

                        toReturn = String.Format("{0}\{1}", root.TrimEnd("\"c), GetFileName(moduleName).TrimStart("\"c))
                        _FilePaths(key) = toReturn
                    End If
                End SyncLock
            End If
            Return toReturn
        End Function




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


        Public Shared Sub Save(ByVal moduleName As String, ByVal instance As TConfigClass, ByVal locSettings As LocationSettings, ByVal useBinarySnapShot As Boolean)

            Dim fileName As String = GetFilePath(moduleName, locSettings, False)
            Aricie.DNN.Settings.SettingsController.SaveFileSettings(Of TConfigClass)(fileName, instance, useBinarySnapShot, locSettings.BackupsNb)

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

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Shared Property SharedLocationSettings(ByVal useCache As Boolean, ByVal useBinarySnapShot As Boolean) As LocationSettings
            Get
                Return SettingsController.LoadFileSettings(Of LocationSettings)(GetLocationFileName(useCache), useCache, useBinarySnapShot)
            End Get
            Set(ByVal value As LocationSettings)
                SettingsController.SaveFileSettings(Of LocationSettings)(GetLocationFileName(useCache), value, useBinarySnapShot)
            End Set
        End Property

        <LabelMode(LabelMode.Top)> _
        <Category("LocationSettings")> _
              <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <XmlIgnore()> _
        Public Property LocationSettings() As LocationSettings
            Get
                Return SharedLocationSettings(True, False)
            End Get
            Set(ByVal value As LocationSettings)
                If Not value.Equals(SharedLocationSettings(True, False)) Then
                    SharedLocationSettings(True, False) = value
                End If
            End Set
        End Property



        Private Shared Function GetLocationFileName(ByVal useCache As Boolean) As String
            Dim configFileName As String = GetFilePath(Identity.GetModuleName, LocationSettings.CoreFile, useCache)
            configFileName = configFileName & ".Loc.config"
            Return configFileName
        End Function

        Public Shared ReadOnly Property Instance() As TConfigClass
            Get
                Return Instance(SharedLocationSettings(True, False))
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


        'Public Overloads Sub Save()

        '    Save(Identity.GetModuleName, Instance, SharedLocationSettings)


        'End Sub

        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass)

            Save(objInstance, False)
            'SharedLocationSettings = locSettings

        End Sub

        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal useBinarySnapShot As Boolean)

            Save(objInstance, SharedLocationSettings(True, useBinarySnapShot), useBinarySnapShot)
            'SharedLocationSettings = locSettings

        End Sub


        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal locSettings As LocationSettings)

            Save(objInstance, locSettings, False)


        End Sub

        Public Overloads Shared Sub Save(ByVal objInstance As TConfigClass, ByVal locSettings As LocationSettings, ByVal useBinarySnapShot As Boolean)

            Save(Identity.GetModuleName, objInstance, locSettings, useBinarySnapShot)
            SharedLocationSettings(True, False) = locSettings

        End Sub




        Public Overloads Shared Sub Reset()
            Reset(SharedLocationSettings(True, False))
        End Sub

        Public Overloads Shared Sub Reset(ByVal locSettings As LocationSettings)
            Save(Instance(locSettings).GetDefaultConfig, locSettings)
        End Sub

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


    End Class
End Namespace



