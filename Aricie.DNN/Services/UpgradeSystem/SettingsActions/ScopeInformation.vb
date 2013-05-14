Imports Aricie.DNN.Settings

Namespace Services.UpgradeSystem.Actions.Settings
    Public Class ScopeInformation
        Public Sub New(SS As SettingsScope, SID As Integer)
            Scope = SS
            ScopeId = SID
        End Sub
        Public Property Scope As SettingsScope
        Public Property ScopeId As Integer
    End Class
End Namespace