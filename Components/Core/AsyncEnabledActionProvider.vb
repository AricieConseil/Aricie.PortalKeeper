Imports Aricie.DNN.Services.Workers
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper



    <Serializable()> _
    Public MustInherit Class AsyncEnabledActionProvider(Of TEngineEvents As IConvertible)
        Inherits ActionProvider(Of TEngineEvents)

        Private _TaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
        <NonSerialized()> _
        Private WithEvents _AsynchronousRunTaskQueue As TaskQueue(Of PortalKeeperContext(Of TEngineEvents))

        <AutoPostBack()> _
        <SortOrder(900)> _
        <ExtendedCategory("TechnicalSettings")> _
        Public Property UseTaskQueue() As Boolean


        <SortOrder(900)> _
        <ExtendedCategory("TechnicalSettings")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ConditionalVisible("UseTaskQueue", False, True)> _
        Public Property TaskQueueInfo() As TaskQueueInfo
            Get
                If UseTaskQueue Then
                    Return _TaskQueueInfo
                End If
                Return Nothing
            End Get
            Set(ByVal value As TaskQueueInfo)
                _TaskQueueInfo = value
            End Set
        End Property

        Private ReadOnly Property AsynchronousRunTaskQueue() As TaskQueue(Of PortalKeeperContext(Of TEngineEvents))
            Get
                If _AsynchronousRunTaskQueue Is Nothing Then
                    _AsynchronousRunTaskQueue = New TaskQueue(Of PortalKeeperContext(Of TEngineEvents))(AddressOf RunAsync, Me._TaskQueueInfo)
                    'AddHandler _PersisterTaskQueue.ActionPerformed, AddressOf Me.PersisterTaskQueue_ActionPerformed
                End If
                Return _AsynchronousRunTaskQueue
            End Get
        End Property


        Public Sub RunAsync(ByVal actionContext As PortalKeeperContext(Of TEngineEvents))
            Me.Run(actionContext, True)
        End Sub

        Public NotOverridable Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Not Me._UseTaskQueue Then
                Return Me.Run(actionContext, False)
            Else
                Me.AsynchronousRunTaskQueue.EnqueueTask(actionContext)
            End If
            Return True
        End Function


        Protected MustOverride Overloads Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean


    End Class
End Namespace