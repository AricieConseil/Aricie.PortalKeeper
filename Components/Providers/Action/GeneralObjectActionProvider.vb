Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls
Imports System.Reflection

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Wrench, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Object Action")> _
    <Description("This provider allows to do actions on object by setting properties, calling methods or hooking event handlers")> _
    Public Class GeneralObjectActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)



        <ExtendedCategory("Action")> _
        Public Property ObjectAction As New KeeperObjectAction(Of TEngineEvents)

        <Browsable(False)> _
        Public Overrides ReadOnly Property ShowOutput As Boolean
            Get
                Return ObjectAction.ActionMode = Services.Flee.ObjectActionMode.CallMethod AndAlso ObjectAction.SelectedMember IsNot Nothing AndAlso DirectCast(ObjectAction.SelectedMember, MethodInfo).ReturnType IsNot Nothing
            End Get
        End Property

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Return Me.ObjectAction.Run(actionContext, actionContext)
        End Function

        Protected Overrides Function GetOutputType() As Type
            Return ObjectAction.GetOutputType()
        End Function

        Public Overrides Sub AddVariables(currentProvider As Services.Flee.IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            If Me.ObjectAction.ActionMode = Services.Flee.ObjectActionMode.AddEventHandler _
                AndAlso ObjectAction.EventHandlerType = EventHandlerType.KeeperAction _
                AndAlso ObjectAction.PassArguments Then
                For Each objParam In ObjectAction.AvailableParametersAndTypes()
                    existingVars(objParam.Key) = objParam.Value
                Next
            End If

            MyBase.AddVariables(currentProvider, existingVars)
        End Sub

    End Class
End NameSpace