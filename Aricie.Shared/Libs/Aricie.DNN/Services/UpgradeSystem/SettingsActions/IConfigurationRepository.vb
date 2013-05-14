Imports System.Xml.Linq
Imports Aricie.DNN.Settings

Namespace Services.UpgradeSystem.Actions.Settings.Configuration

    ''' <summary>
    ''' Interface de gestion des paramètres pour changements futurs
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IConfigurationRepository

        Function RetrieveConfig(ByVal scope As SettingsScope, ByVal id As Integer, ByVal classKey As String) As XDocument
        Sub StoreConfig(ByVal scope As SettingsScope, ByVal id As Integer, ByVal classKey As String, ByVal d As XDocument)

        Sub CommitConfigChanges()

    End Interface
End Namespace