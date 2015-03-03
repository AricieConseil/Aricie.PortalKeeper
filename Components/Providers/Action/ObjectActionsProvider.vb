Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Wrench, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Object Actions Provider")> _
        <Description("This provider allows to call object methods or set object properties")> _
    Public Class ObjectActionsProvider(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)



        Private _ObjectActions As New ObjectActions


        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ExtendedCategory("Specifics")> _
            <MainCategory()> _
        Public Property ObjectActions() As ObjectActions
            Get
                Return _ObjectActions
            End Get
            Set(ByVal value As ObjectActions)
                _ObjectActions = value
            End Set
        End Property


        Protected Overloads Overrides Function Run(actionContext As PortalKeeperContext(Of TEngineEvents), aSync As Boolean) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            'Return MyBase.Run(actionContext)
            Me._ObjectActions.Run(actionContext, actionContext)
            Return True
        End Function
    End Class
End Namespace