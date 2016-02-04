Imports System.ComponentModel
Imports System.Linq
Imports System.Xml.Serialization
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Globe, IconOptions.Normal)>
    <DisplayName("Call Web Service")>
    <Description("Performs a call to one of the dynamic web API services, hosted on the platform, internally")>
    Public Class CallServiceAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)
        Implements ISelector(Of RestService)
        Implements ISelector(Of DynamicControllerInfo)
        Implements ISelector(Of DynamicAction)

        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        <Selector("Name", "Name", False, True, "<Select a Service>", "", False, True)>
        Property SelectedServiceName() As String = ""


        <XmlIgnore()>
        <Browsable(False)>
        Public ReadOnly Property SelectedService() As RestService
            Get
                Return GetServices("SelectedServiceName").FirstOrDefault(Function(objService) objService.Name = SelectedServiceName)
            End Get
        End Property

        <ConditionalVisible("SelectedServiceName", True, True, "")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Name", "Name", False, True, "<Select a Controller>", "", False, True)>
        Property SelectedControllerName() As String = ""

        <XmlIgnore()> _
        <Browsable(False)>
        Public ReadOnly Property SelectedController() As DynamicControllerInfo
            Get
                dim objService as RestService = SelectedService
                if objService IsNot Nothing
                    Return objService.DynamicControllers.FirstOrDefault(Function(objController) objController.Name = SelectedControllerName)
                End If
                Return Nothing
            End Get
        End Property

        <ConditionalVisible("SelectedControllerName", True, True, "")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Name", "Name", False, True, "<Select an Action>", "", False, True)>
        Property SelectedActionName() As String = ""

        <XmlIgnore()> _
        <Browsable(False)>
        Public ReadOnly Property SelectedAction() As DynamicAction
            Get
                Dim objController As DynamicControllerInfo = SelectedController
                If objController IsNot Nothing Then
                    Return objController.DynamicActions.FirstOrDefault(Function(objAction) objAction.Name = SelectedActionName)
                End If
                Return Nothing
            End Get
        End Property

        <ConditionalVisible("SelectedActionName", True, True, "")> _
        Property Verb() As WebMethod = WebMethod.Get

        <ConditionalVisible("SelectedActionName", True, True, "")> _
        Public Property Parameters() As New Variables()


        <ConditionalVisible("SelectedActionName", True, True, "")> _
        <ActionButton(IconName.Key, IconOptions.Normal)>
        Public Sub SetParameters(ape As AriciePropertyEditorControl)
            Dim objAction As DynamicAction = SelectedAction
            If objAction IsNot Nothing Then
                Dim params As New Variables
                For Each objDynamicParam As DynamicParameter In SelectedAction.Parameters
                    Dim newParam As New GeneralVariableInfo(objDynamicParam.Name)
                    newParam.DotNetType = New DotNetType(objDynamicParam.EditableType.GetDotNetType())
                    params.Instances.Add(newParam)
                Next
                Me.Parameters = params
                ape.ItemChanged = True
                ape.DisplayLocalizedMessage("ParametersCreated.Message", ModuleMessage.ModuleMessageType.GreenSuccess)
            End If
        End Sub

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Dim objAction As DynamicAction = SelectedAction
            If objAction IsNot Nothing Then
                Dim args As IDictionary(Of String, Object) = Me.Parameters.EvaluateVariables(actionContext, actionContext)
                Return objAction.Run(SelectedService, SelectedController, Verb, args)
            End If
            Return Nothing
        End Function

        Protected Overrides Function GetOutputType() As Type
           Return GetType(Object)
        End Function

    
        Public Function GetServices(propertyName As String) As IList(Of RestService) Implements ISelector(Of RestService).GetSelectorG
            Return PortalKeeperConfig.Instance.RestServices.RestServices
        End Function

        Public Function GetControllers(propertyName As String) As IList(Of DynamicControllerInfo) Implements ISelector(Of DynamicControllerInfo).GetSelectorG
            Dim objService As RestService = SelectedService
            If objService IsNot Nothing Then
                Return objService.DynamicControllers
            End If
            Return New List(Of DynamicControllerInfo)
        End Function

        Public Function GetActions(propertyName As String) As IList(Of DynamicAction) Implements ISelector(Of DynamicAction).GetSelectorG
             Dim objController As DynamicControllerInfo = SelectedController
                If objController IsNot Nothing Then
                    Return objController.DynamicActions
                End If
                 Return New List(Of DynamicAction)
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "SelectedServiceName"
                    Return DirectCast(GetServices(propertyName), IList)
                Case "SelectedControllerName"
                    Return DirectCast(GetControllers(propertyName), IList)
                Case "SelectedActionName"
                    Return DirectCast(GetActions(propertyName), IList)
            End Select
            Return New ArrayList
        End Function
    End Class
End NameSpace