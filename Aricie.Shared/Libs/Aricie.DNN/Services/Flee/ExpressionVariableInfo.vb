Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization

Namespace Services.Flee

    ''' <summary>
    ''' Class to use flee variables
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <DisplayName("Dynamic expression")> _
    <Serializable()> _
    Public Class ExpressionVariableInfo(Of TResult)
        Inherits InstanceVariableInfo(Of TResult)

        Private _SimpleExpression As SimpleExpression(Of TResult)
        Private _FleeExpression As New FleeExpressionInfo(Of TResult)


        ''' <summary>
        ''' Is the variable and advanced expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Definition")> _
        Public Property AdvancedExpression() As Boolean

        ''' <summary>
        ''' Gets the flee expression that will be used
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("AdvancedExpression", False, True)> _
        <ExtendedCategory("Definition")>
        Public Property FleeExpression() As FleeExpressionInfo(Of TResult)
            Get
                Return _FleeExpression
            End Get
            Set(value As FleeExpressionInfo(Of TResult))
                _FleeExpression = value
                _SimpleExpression = Nothing
            End Set
        End Property

        ''' <summary>
        ''' Retrieve the simple expression that will be evaluated
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        <ConditionalVisible("AdvancedExpression", True, True)> _
       <ExtendedCategory("Definition")> _
        Public Property SimpleExpression() As SimpleExpression(Of TResult)
            Get
                If _SimpleExpression Is Nothing Then
                    _SimpleExpression = New SimpleExpression(Of TResult)(Me.FleeExpression)
                End If
                Return _SimpleExpression
            End Get
            Set(ByVal value As SimpleExpression(Of TResult))
                _SimpleExpression = value
                _SimpleExpression.SlaveExpression = FleeExpression
                Me.FleeExpression.Expression = _SimpleExpression.Expression
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the expression should be compiled or evaluated
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Evaluation")> _
        <SortOrder(104)> _
        Public Property AsCompiledExpression() As Boolean

        ''' <summary>
        ''' Evaluates the variable value
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function EvaluateOnce(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            If Me._AsCompiledExpression Then
                Return Me.FleeExpression.GetCompiledExpression(owner, globalVars)
            Else
                Return Me.FleeExpression.Evaluate(owner, globalVars)
            End If
        End Function
    End Class
End Namespace