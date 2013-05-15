Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum MultipleActionMode
        Sequence
        ActionTree
        ActionTreeExpression
    End Enum

    <Serializable()> _
        <System.ComponentModel.DisplayName("Multiple Action Provider")> _
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
            Select Case Me.MultipleActionMode
                Case PortalKeeper.MultipleActionMode.Sequence
                    Return Me._KeeperAction.Run(actionContext)
                Case PortalKeeper.MultipleActionMode.ActionTree
                    Return Me.ActionTree.Run(actionContext)
                Case PortalKeeper.MultipleActionMode.ActionTreeExpression
                    Dim objActionTree As ActionTree(Of TEngineEvents) = Me.ActionTreeExpression.Evaluate(actionContext, actionContext)
                    If objActionTree IsNot Nothing Then
                        Return objActionTree.Run(actionContext)
                    End If
            End Select
        End Function

    End Class
End Namespace