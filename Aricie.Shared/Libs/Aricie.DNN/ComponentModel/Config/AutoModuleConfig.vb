Imports Aricie.DNN.Settings

Namespace ComponentModel
    ''' <summary>
    ''' Generic self referencing self identifying module configuration class
    ''' </summary>
    ''' <remarks>Contains its own identifying implementation</remarks>
    <Serializable()> _
    Public MustInherit Class AutoModuleConfig(Of TConfigClass As {New, ModuleConfig(Of TConfigClass), IModuleIdentity})
        Inherits ModuleConfig(Of TConfigClass, TConfigClass)
        Implements IModuleIdentity

        'Public Overloads Sub Save()

        '    Save(Identity.GetModuleName, Me, SharedLocationSettings)


        'End Sub

        Public Property ModuleInstance(mid As Integer) As TConfigClass
            Get
                Return SettingsController.GetModuleSettings(Of TConfigClass)(SettingsScope.ModuleSettings, mid)
            End Get
            Set(value As TConfigClass)
                SettingsController.SetModuleSettings(Of TConfigClass)(SettingsScope.ModuleSettings, mid, value)
            End Set
        End Property


        Public MustOverride Function GetModuleName() As String Implements IModuleIdentity.GetModuleName

    End Class
End Namespace