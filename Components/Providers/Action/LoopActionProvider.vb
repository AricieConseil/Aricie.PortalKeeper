Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Repeat, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Loop Action Provider")> _
        <Description("This provider allows to loop running a sub bot over a custom collection. On each run, the current item of the collection is affected to a custom variable.")> _
    Public Class LoopActionProvider(Of TEngineEvents As IConvertible)
        Inherits MultipleActionProvider(Of TEngineEvents)

        Private _EnumerableExpression As New FleeExpressionInfo(Of IEnumerable)
        Private _CurrentItemParam As String = "CurrentLoopItem"

        'Private _WaitTime As New STimeSpan(TimeSpan.FromSeconds(1))


        Private _UseCounter As Boolean
        Private _CounterStart As New SimpleExpression(Of Integer)("0")
        Private _CounterUpdate As New SimpleExpression(Of Integer)("CurrentLoopItem + 1")
        Private _CounterEval As New SimpleExpression(Of Boolean)("CurrentLoopItem < 10")


        <ExtendedCategory("LoopAction")> _
            <Required(True)> _
        Public Property CurrentItemParam() As String
            Get
                Return _CurrentItemParam
            End Get
            Set(ByVal value As String)
                _CurrentItemParam = value
            End Set
        End Property

        <ExtendedCategory("LoopAction")> _
        Public Property UseCounter() As Boolean
            Get
                Return _UseCounter
            End Get
            Set(ByVal value As Boolean)
                _UseCounter = value
            End Set
        End Property


        <ExtendedCategory("LoopAction")> _
        <LabelMode(LabelMode.Top)> _
        <ConditionalVisible("UseCounter", False, True)> _
        Public Property CounterStart() As SimpleExpression(Of Integer)
            Get
                Return _CounterStart
            End Get
            Set(ByVal value As SimpleExpression(Of Integer))
                _CounterStart = value
            End Set
        End Property

        <ExtendedCategory("LoopAction")> _
        <LabelMode(LabelMode.Top)> _
        <ConditionalVisible("UseCounter", False, True)> _
        Public Property CounterUpdate() As SimpleExpression(Of Integer)
            Get
                Return _CounterUpdate
            End Get
            Set(ByVal value As SimpleExpression(Of Integer))
                _CounterUpdate = value
            End Set
        End Property

        <ExtendedCategory("LoopAction")> _
        <LabelMode(LabelMode.Top)> _
        <ConditionalVisible("UseCounter", False, True)> _
        Public Property CounterEval() As SimpleExpression(Of Boolean)
            Get
                Return _CounterEval
            End Get
            Set(ByVal value As SimpleExpression(Of Boolean))
                _CounterEval = value
            End Set
        End Property



        <ExtendedCategory("LoopAction")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ConditionalVisible("UseCounter", True, True)> _
        Public Property EnumerableExpression() As FleeExpressionInfo(Of IEnumerable)
            Get
                Return _EnumerableExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of IEnumerable))
                _EnumerableExpression = value
            End Set
        End Property



        ' <ExtendedCategory("LoopAction")> _
        ' <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        '    <LabelMode(LabelMode.Top)> _
        'Public Property WaitTime() As STimeSpan
        '     Get
        '         Return _WaitTime
        '     End Get
        '     Set(ByVal value As STimeSpan)
        '         _WaitTime = value
        '     End Set
        ' End Property

        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            Dim toReturn As Boolean = True
            If Me._UseCounter Then
                actionContext.SetVar(Me._CurrentItemParam, Me._CounterStart.Evaluate(actionContext, actionContext))
                While Me._CounterEval.Evaluate(actionContext, actionContext)
                    toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                    actionContext.SetVar(Me._CurrentItemParam, Me._CounterUpdate.Evaluate(actionContext, actionContext))
                End While
            Else
                Dim objEnumerable As IEnumerable = Me._EnumerableExpression.Evaluate(actionContext, actionContext)
                For Each objCurrent As Object In objEnumerable
                    actionContext.SetVar(Me._CurrentItemParam, objCurrent)
                    'Me.SubBot.ProcessRules(actionContext, actionContext.CurrentEventStep, False)
                    toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                    'Threading.Thread.Sleep(Me._WaitTime.Value)
                Next
            End If

            Return toReturn
        End Function

    End Class
End Namespace