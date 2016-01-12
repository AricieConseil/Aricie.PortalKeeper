Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum MultipleActionMode
        Sequence
        ActionTree
        ActionTreeExpression
    End Enum

    <ActionButton(IconName.Sitemap, IconOptions.Normal)>
    <DisplayName("Multiple Action Provider")>
    <Description("Runs sub sequences of Actions")>
    Public Class MultipleActionProvider(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        <ExtendedCategory("Specifics")>
        Public Property MultipleActionMode As MultipleActionMode = MultipleActionMode.Sequence


        <ExtendedCategory("Specifics")>
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.Sequence)>
        Public Property KeeperAction() As New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("Specifics")>
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.ActionTree)>
        Public Property ActionTree As New ActionTree(Of TEngineEvents)


        <ExtendedCategory("Specifics")>
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.ActionTreeExpression)>
        Public Property ActionTreeExpression As New SimpleExpression(Of ActionTree(Of TEngineEvents))




        'Public Overrides Function Run(ByVal actionContext As ActionContext) As Boolean

        'End Function


        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Select Case Me.MultipleActionMode
                Case MultipleActionMode.Sequence
                    Return Me._KeeperAction.Run(actionContext)
                Case MultipleActionMode.ActionTree
                    Return Me.ActionTree.Run(actionContext)
                Case MultipleActionMode.ActionTreeExpression
                    Dim objActionTree As ActionTree(Of TEngineEvents) = Me.ActionTreeExpression.Evaluate(actionContext, actionContext)
                    If objActionTree IsNot Nothing Then
                        Return objActionTree.Run(actionContext)
                    End If
            End Select
        End Function


        Public Overrides Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            Me.KeeperAction.AddVariables(currentProvider, existingVars)
            MyBase.AddVariables(currentProvider, existingVars)
        End Sub


    End Class
End Namespace