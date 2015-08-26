Imports System.Xml.Serialization
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Code, IconOptions.Normal)> _
    Public Class DynamicControllerInfo
        Inherits NamedConfig

        

        Public Property DynamicAttributes As New ControllerAttributes()

        Public Property DynamicActions As New List(Of DynamicAction)

        Public Property GlobalParameters As New Variables()

        <CollectionEditor(False, False, False, False, 0, CollectionDisplayStyle.List)> _
        <XmlIgnore()> _
        Public ReadOnly Property RegisteredRoutes As List(Of String)
            Get
                Dim toReturn As New List(Of String)
                Dim baseUrl As String = NukeHelper.BaseUrl()
                toReturn.AddRange(ObsoleteDotNetProvider.Instance.GetSampleRoutes(baseUrl, Me))
                Return toReturn
            End Get
        End Property

    End Class
End Namespace