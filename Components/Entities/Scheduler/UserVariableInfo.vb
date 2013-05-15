Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
'Imports DotNetNuke.Entities.Profile
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    '<XmlRoot("UserParam")> _
    '<XmlInclude(GetType(GeneralPropertyDefinition))> _
    <Serializable()> _
    Public Class UserVariableInfo
        Inherits NamedConfig

        Public Sub New()

        End Sub


        Private _Mode As UserParameterMode
        Private _PropertyDefinition As New GeneralPropertyDefinition
        Private _Override As Boolean
        Private _ForceOverride As Boolean
        Private _IsReadOnly As Boolean

        'Private _SaveAfterRun As Boolean

        Public Property Mode() As UserParameterMode
            Get
                Return _Mode
            End Get
            Set(ByVal value As UserParameterMode)
                _Mode = value
            End Set
        End Property



        <ConditionalVisible("Mode", False, True, UserParameterMode.PropertyDefinition)> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        Public Property PropertyDefinition() As GeneralPropertyDefinition
            Get
                Return _PropertyDefinition
            End Get
            Set(ByVal value As GeneralPropertyDefinition)
                _PropertyDefinition = value
            End Set
        End Property



        Public Property Override() As Boolean
            Get
                Return _Override
            End Get
            Set(ByVal value As Boolean)
                _Override = value
            End Set
        End Property

        <ConditionalVisible("Override", False, True)> _
        Public Property ForceOverride() As Boolean
            Get
                Return _ForceOverride
            End Get
            Set(ByVal value As Boolean)
                _ForceOverride = value
            End Set
        End Property


        'Public Property SaveAfterRun() As Boolean
        '    Get
        '        Return _SaveAfterRun
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _SaveAfterRun = value
        '    End Set
        'End Property


        Public Property IsReadOnly() As Boolean
            Get
                Return _IsReadOnly
            End Get
            Set(ByVal value As Boolean)
                _IsReadOnly = value
            End Set
        End Property




    End Class
End Namespace