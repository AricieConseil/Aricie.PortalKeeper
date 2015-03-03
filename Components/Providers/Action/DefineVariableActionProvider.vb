Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Magic, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Define Variable")> _
    <Description("This provider allows to declare and instanciate a single variable, which will be stored in the context ""Item"" dictionary")> _
    Public Class DefineVariableActionProvider(Of TEngineEvents As IConvertible)
        Inherits CacheableAction(Of TEngineEvents)
        Implements IExpressionVarsProvider


        Public Property Variable As New GeneralVariableInfo()

        Public Property GetFromHistory As Boolean

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Object = Me.Variable.Evaluate(actionContext, actionContext)
            If Me._GetFromHistory Then
                Dim newValue As Object = Nothing
                If actionContext.Items.TryGetValue("Last" & Me.Variable.Name, newValue) Then
                    toReturn = newValue
                End If
            End If
            Return toReturn
        End Function

        Public Overrides Function RunFromObject(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean, cachedObject As Object) As Boolean
            actionContext.SetVar(Me.Variable.Name, cachedObject)
            Return True
        End Function

        Public Overrides Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            existingVars(Me.Variable.Name) = Me.Variable.DotNetType.GetDotNetType()
            MyBase.AddVariables(currentProvider, existingVars)
        End Sub
    End Class
End NameSpace