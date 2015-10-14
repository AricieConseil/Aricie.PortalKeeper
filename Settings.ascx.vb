Imports System.Web.Compilation
Imports Aricie.DNN.Services
Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Settings
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services
Imports DotNetNuke.UI.Skins.Controls

Namespace Aricie.DNN.Modules.PortalKeeper.UI

    Partial Public Class Settings
        Inherits ModuleSettingsBase

        Protected Property KeeperModuleSettings() As KeeperModuleSettings
            Get
                Return GetModuleSettings(Of KeeperModuleSettings)(SettingsScope.ModuleSettings, Me.ModuleId)
            End Get
            Set(ByVal value As KeeperModuleSettings)
                SetModuleSettings(Of KeeperModuleSettings)(SettingsScope.ModuleSettings, Me.ModuleId, value)
            End Set
        End Property

        Public ReadOnly Property SharedResourceFile() As String
            Get

                Return Me.TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/" & Localization.LocalSharedResourceFile

            End Get
        End Property

#Region "Settings methods"

        Public Overrides Sub LoadSettings()
            Try



            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try


        End Sub

        ''' <summary>
        ''' Save the module settings in the database
        ''' </summary>
        Public Overrides Sub UpdateSettings()
            Try
                Me.SaveSettings()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try

        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            Try

                Me.ctS.LocalResourceFile = Me.SharedResourceFile
                Me.ctS.DataSource = Me.KeeperModuleSettings
                Me.ctS.DataBind()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub




#End Region

#Region "Private methods"

        Private Sub SaveSettings()
            Dim resultSettings As KeeperModuleSettings = Nothing
            If ctS.DataSource IsNot Nothing Then
                If TypeOf ctS.DataSource Is KeeperModuleSettings Then
                    resultSettings = DirectCast(Me.ctS.DataSource, KeeperModuleSettings)
                ElseIf TypeOf ctS.DataSource Is SubPathContainer Then
                    resultSettings = DirectCast(DirectCast(Me.ctS.DataSource, SubPathContainer).OriginalEntity, KeeperModuleSettings)
                End If
            End If

            If resultSettings IsNot Nothing Then
                Me.KeeperModuleSettings = resultSettings
            End If

        End Sub



#End Region
    End Class
End Namespace
