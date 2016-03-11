Imports System.Xml.Serialization
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Code, IconOptions.Normal)> _
    Public Class DynamicControllerInfo
        Inherits NamedConfig
        Implements IExpressionVarsProvider

        <ExtendedCategory("Global")> _
        Public Property DynamicAttributes As New ControllerAttributes()

        <ExtendedCategory("Global")> _
        Public Property GlobalParameters As New Variables()

        Public Property DynamicActions As New List(Of DynamicAction)

        <ExtendedCategory("Routes")> _
        Public Property SpecificRoutes As New List(Of DynamicRoute)()

        <ExtendedCategory("Routes")> _
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

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            existingVars("Controller") = ReflectionHelper.CreateType("Aricie.PortalKeeper.DNN7.DynamicController, Aricie.PortalKeeper.DNN7")
            existingVars("Request") = ReflectionHelper.CreateType("System.Net.Http.HttpRequestMessage, System.Net.Http")
            existingVars("RouteData") = ReflectionHelper.CreateType("System.Web.Http.Routing.IHttpRouteData, System.Web.Http")
            Me.GlobalParameters.AddVariables(currentProvider, existingVars)
        End Sub
    End Class
End Namespace