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

    <ActionButton(IconName.Sitemap, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Multiple Action Provider")> _
        <Description("Runs sub sequences of Actions")> _
    Public Class MultipleActionProvider(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        <ExtendedCategory("Specifics")> _
        Public Property MultipleActionMode As MultipleActionMode = MultipleActionMode.Sequence


        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.Sequence)> _
        Public Property KeeperAction() As New KeeperAction(Of TEngineEvents)

        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.ActionTree)> _
        Public Property ActionTree As New ActionTree(Of TEngineEvents)


        <ExtendedCategory("Specifics")> _
        <ConditionalVisible("MultipleActionMode", False, True, MultipleActionMode.ActionTreeExpression)> _
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