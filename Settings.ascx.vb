Imports DotNetNuke.Entities.Modules
Imports Aricie.DNN.Settings
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions

Namespace Aricie.DNN.Modules.PortalKeeper.UI

    Partial Public Class Settings
        Inherits ModuleSettingsBase

        Protected Property KeeperModuleSettings() As KeeperModuleSettings
            Get
                Return GetModuleSettings(Of KeeperModuleSettings)(SettingsScope.TabModuleSettings, Me.TabModuleId)
            End Get
            Set(ByVal value As KeeperModuleSettings)
                SetModuleSettings(Of KeeperModuleSettings)(SettingsScope.TabModuleSettings, Me.TabModuleId, value)
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


            Dim resultSettings As KeeperModuleSettings = DirectCast(Me.ctS.DataSource, KeeperModuleSettings)


            Me.KeeperModuleSettings = resultSettings


        End Sub



#End Region
    End Class
End Namespace
