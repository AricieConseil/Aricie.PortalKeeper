Imports Aricie.DNN.Settings
Imports System.Xml.Linq
Imports System.Linq
Imports System.Text

Namespace Services.UpgradeSystem.Actions.Settings.Configuration

    ''' <summary>
    ''' Classe de gestion de la configuration pour les actions se rapportant aux paramètres: implémentation actuelle de la gestion des paramètres dans Aricie.Shared
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class ConfigurationRepository
        Implements IConfigurationRepository

        Private ScopeConfigs As New List(Of ScopeConfig)

        Private Class ScopeConfig
            Public Property Scope As SettingsScope
            Public Property Id As Integer
            Public Property ClassKey As String
            Public Property Document As XDocument
        End Class

        Private Function GetConfig(ByVal scope As SettingsScope, ByVal id As Integer, ByVal classKey As String) As ScopeConfig
            Dim config = ScopeConfigs.FirstOrDefault(Function(sc) sc.Scope = scope AndAlso sc.Id = id AndAlso sc.ClassKey = classKey)

            If (config Is Nothing) Then
                Dim XmlValue = SettingsController.FetchFromModuleSettings(scope, id, classKey)
                Dim Document = XDocument.Parse(XmlValue.ToString())
                config = New ScopeConfig() With {.Scope = scope, .Id = id, .Document = Document, .ClassKey = classKey}
                ScopeConfigs.Add(config)
            End If

            Return config
        End Function

        Protected Friend Function RetrieveConfig(ByVal scope As SettingsScope, ByVal id As Integer, ByVal classKey As String) As XDocument Implements IConfigurationRepository.RetrieveConfig
            Return GetConfig(scope, id, classKey).Document
        End Function

        Protected Friend Sub StoreConfig(ByVal scope As SettingsScope, ByVal id As Integer, ByVal classKey As String, ByVal d As XDocument) Implements IConfigurationRepository.StoreConfig
            GetConfig(scope, id, classKey).Document = d
        End Sub

        Protected Friend Sub CommitConfigChanges() Implements IConfigurationRepository.CommitConfigChanges
            For Each sc In ScopeConfigs
                Dim builder As New StringBuilder()
                Using writer As New System.IO.StringWriter(builder)
                    sc.Document.Save(writer)
                End Using

                SettingsController.SaveToModuleSettings(sc.Scope, sc.Id, sc.ClassKey, builder.ToString())
            Next
        End Sub
    End Class
End Namespace
