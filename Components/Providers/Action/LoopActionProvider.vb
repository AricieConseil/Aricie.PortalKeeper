Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports System.Xml.Serialization
Imports System.Linq

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Repeat, IconOptions.Normal)> _
    <Serializable()> _
    <DisplayName("Loop")> _
        <Description("This provider allows to loop running a sub bot over a custom collection. On each run, the current item of the collection is affected to a custom variable.")> _
    Public Class LoopActionProvider(Of TEngineEvents As IConvertible)
        Inherits MultipleActionProvider(Of TEngineEvents)



        Private _CurrentItemParam As String = "CurrentLoopItem"

        'Private _WaitTime As New STimeSpan(TimeSpan.FromSeconds(1))


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
        

        <ExtendedCategory("LoopAction")>
        Public Property UseCounter As Boolean

        <ExtendedCategory("LoopAction")>
        Public Property MaxNbIterations As Integer = 0


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
            <ConditionalVisible("UseCounter", True, True)> _
        Public Property UseCloneList As Boolean

        <ExtendedCategory("LoopAction")> _
         <ConditionalVisible("UseCloneList", False, True)> _
            <ConditionalVisible("UseCounter", True, True)> _
        Public Property Reverse As Boolean

        <ConditionalVisible("UseCounter", True, True)> _
      <ExtendedCategory("LoopAction")> _
        Public Property CaptureCounter As Boolean

        <Required(True)> _
        <ConditionalVisible("UseCounter", True, True)> _
        <ConditionalVisible("CaptureCounter", False, True)> _
        <ExtendedCategory("LoopAction")> _
        Public Property CounterVarName As String = "CurrentLoopIndex"

        <ConditionalVisible("UseCounter", True, True)> _
        <ConditionalVisible("CaptureCounter", False, True)> _
        <ExtendedCategory("LoopAction")> _
        Public Property CounterStartsAt1 As Boolean

        <ExtendedCategory("LoopAction")> _
           <ConditionalVisible("UseCounter", True, True)> _
        Public Property EnumerableExpression() As New FleeExpressionInfo(Of IEnumerable)
          

        <XmlIgnore()> _
        <ConditionalVisible("DisablePerformanceLogger", True, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1001)> _
        Public Overridable Property AgregateLogSteps() As Boolean


        <XmlIgnore()> _
        <ConditionalVisible("DisablePerformanceLogger", True, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1001)> _
        Public Property LogParticularSteps() As Boolean = True

        <ConditionalVisible("LogParticularSteps", False, True)> _
        <ExtendedCategory("TechnicalSettings")> _
        <SortOrder(1001)> _
        Public Property StepsToLogAsString As String = "0;"


        Private _StepsToLog As List(Of Integer)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property StepsToLog As List(Of Integer)
            Get
                If _StepsToLog Is Nothing Then
                    Dim toReturn As New List(Of Integer)
                    Dim toAdds As String() = StepsToLogAsString.Trim().Trim(";"c).Split(";"c)
                    For Each objAdd As String In toAdds
                        Dim toAdd As Integer
                        If Integer.TryParse(objAdd, toAdd) Then
                            toReturn.Add(toAdd)
                        End If
                    Next
                    _StepsToLog = toReturn
                End If
                Return _StepsToLog
            End Get
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
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Boolean = True

            If Me.AgregateLogSteps Then
                PerformanceLogger.Instance.AgregateLogs(actionContext.FlowId)
            End If


            Dim maxIterations As Integer = Integer.MaxValue
            If Me.MaxNbIterations > 0 Then
                maxIterations = MaxNbIterations
            End If
            Dim counter As Integer = 0
            If Me.UseCounter Then
                actionContext.SetVar(Me._CurrentItemParam, Me._CounterStart.Evaluate(actionContext, actionContext))
                While Me._CounterEval.Evaluate(actionContext, actionContext) AndAlso counter < maxIterations
                    If LogParticularSteps AndAlso Not StepsToLog.Contains(counter) Then
                        PerformanceLogger.Instance().DisableLog(actionContext.FlowId)
                    End If
                    toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                    If LogParticularSteps AndAlso Not StepsToLog.Contains(counter) Then
                        PerformanceLogger.Instance().EnableLog(actionContext.FlowId)
                    End If
                    actionContext.SetVar(Me._CurrentItemParam, Me._CounterUpdate.Evaluate(actionContext, actionContext))
                    counter += 1
                End While
            Else
                Dim objEnumerable As IEnumerable = Me._EnumerableExpression.Evaluate(actionContext, actionContext)
                If objEnumerable IsNot Nothing Then
                    If UseCloneList Then
                        Dim newList = objEnumerable.Cast(Of Object)()
                        If Reverse Then
                            objEnumerable = newList.Reverse().ToList()
                        Else
                            objEnumerable = newList.ToList()
                        End If
                    End If
                    For Each objCurrent As Object In objEnumerable
                        If counter = maxIterations Then
                            Exit For
                        End If
                        actionContext.SetVar(Me._CurrentItemParam, objCurrent)
                        If CaptureCounter Then
                            If CounterStartsAt1 Then
                                actionContext.SetVar(Me.CounterVarName, counter + 1)
                            Else
                                actionContext.SetVar(Me.CounterVarName, counter)
                            End If
                        End If
                        If LogParticularSteps AndAlso Not StepsToLog.Contains(counter) Then
                            PerformanceLogger.Instance().DisableLog(actionContext.FlowId)
                        End If
                        toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                        If LogParticularSteps AndAlso Not StepsToLog.Contains(counter) Then
                            PerformanceLogger.Instance().EnableLog(actionContext.FlowId)
                        End If
                        counter += 1
                    Next
                Else
                    Throw New ApplicationException(String.Format("Expression {0} does not evaluate to an Enumerable entity", Me._EnumerableExpression.Expression))
                End If
            End If

            If Me.AgregateLogSteps Then
                PerformanceLogger.Instance.DisableAregates(actionContext.FlowId)
            End If

            Return toReturn
        End Function

        Public Overrides Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            If Not Me.CurrentItemParam.IsNullOrEmpty() Then
                existingVars.Add(Me.CurrentItemParam, GetType(Object))
            End If
            MyBase.AddVariables(currentProvider, existingVars)
        End Sub


    End Class
End Namespace