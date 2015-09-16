Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Globe, IconOptions.Normal)> _
    <Serializable()> _
    Public Class RestService
        Inherits NamedConfig
        Implements IExpressionVarsProvider




        'Public Property ResourceType As New DotNetType

        'Public Property Routes As New OneOrMore(Of DynamicRoute)(New DynamicRoute("default", "{controller}/{action}"))

        Public Property SpecificRoutes As New List(Of DynamicRoute)()


        'Public Property AtUri As String = "/MyResource"

        '<CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List, EnableExport:=False)> _
        'Public Property AlternateUris As New List(Of String)

        'Public Property RestHandlerType As RestHandlerType

        '<ConditionalVisible("RestHandlerType", False, True, RestHandlerType.CustomHandler)> _
        'Public Property HandlerType As New DotNetType

        '<Required(True)> _
        '<ConditionalVisible("RestHandlerType", False, True, RestHandlerType.DynamicHandler)> _
        'Public Property ControllerName As String

        Public Property DynamicControllers As New List(Of DynamicControllerInfo)



        'Private _DynamicMethodsByActionName As IDictionary(Of String, DynamicRestMethod)

        'Public ReadOnly Property DynamicMethodsByActionName As IDictionary(Of String, DynamicRestMethod)
        '    Get
        '        If _DynamicMethodsByActionName Is Nothing Then
        '            SyncLock Me
        '                If _DynamicMethodsByActionName Is Nothing Then
        '                    Dim tempDico As New Dictionary(Of String, DynamicRestMethod)(StringComparer.OrdinalIgnoreCase)
        '                    For Each objMethod As DynamicRestMethod In DynamicMethods
        '                        If objMethod.Enabled AndAlso objMethod.ActionNames.Enabled Then
        '                            For Each strActionName As String In objMethod.ActionNames.Entity
        '                                tempDico(strActionName) = objMethod
        '                            Next
        '                        End If
        '                    Next
        '                    _DynamicMethodsByActionName = tempDico
        '                End If
        '            End SyncLock
        '        End If
        '        Return _DynamicMethodsByActionName
        '    End Get
        'End Property


        <ConditionalVisible("RestHandlerType", False, True, RestHandlerType.DynamicHandler)> _
        Public Property GlobalParameters As New Variables()



        'Public Property AsJsonDataContract As Boolean = True

        'Public Property AsXmlDataContract As Boolean = True

        'Public Property AsXmlSerializer As Boolean = True


        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            Me.GlobalParameters.AddVariables(currentProvider, existingVars)
        End Sub
    End Class
End Namespace