Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
'Imports DotNetNuke.Entities.Profile
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Files

Namespace Aricie.DNN.Modules.PortalKeeper
    '<XmlRoot("UserParam")> _
    '<XmlInclude(GetType(GeneralPropertyDefinition))> _
    <Serializable()> _
    Public Class UserVariableInfo
        Inherits NamedConfig

        Public Sub New()

        End Sub


        'Private _SaveAfterRun As Boolean

        Public Property Mode As UserParameterMode


        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl)), ConditionalVisible("Mode", False, True, UserParameterMode.PropertyDefinition)>
        Public Property PropertyDefinition As New GeneralPropertyDefinition


        Public Property Override As Boolean

        <ConditionalVisible("Override", False, True)>
        Public Property ForceOverride As Boolean


        'Public Property SaveAfterRun() As Boolean
        '    Get
        '        Return _SaveAfterRun
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _SaveAfterRun = value
        '    End Set
        'End Property

        Public Property IsReadOnly As Boolean

        Public Property UseSubStorage As Boolean

        Public Property SubStorageSettings As SmartFileInfo


    End Class
End Namespace