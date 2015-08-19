Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Code, IconOptions.Normal)> _
    Public Class DynamicControllerInfo
        Inherits NamedConfig

        Public Property DynamicAttributes As New ControllerAttributes()

        Public Property DynamicActions As New List(Of DynamicAction)

        Public Property GlobalParameters As New Variables()


    End Class
End Namespace