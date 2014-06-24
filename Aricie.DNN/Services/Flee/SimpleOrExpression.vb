Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace Services.Flee
    <Serializable()>
    Public Class SimpleOrExpression(Of T)
        Inherits SimpleOrExpressionBase(Of T)


        Public Sub New()

        End Sub

        Public Sub New(value As T)
            Me.New(value, False)
        End Sub


        Public Sub New(value As Object, isExpression As Boolean)
            If isExpression Then
                Me.Expression = New FleeExpressionInfo(Of T)(DirectCast(value, String))
                Me.Mode = SimpleOrExpressionMode.Expression
            Else
                Me.Simple = DirectCast(value, T)
                Me.Mode = SimpleOrExpressionMode.Simple
            End If
        End Sub

        <ConditionalVisible("Mode", False, True, SimpleOrExpressionMode.Expression)>
        Public Property Expression As New FleeExpressionInfo(Of T)

        Public Overrides Function GetExpression() As SimpleExpression(Of T)
            Return Expression
        End Function
    End Class


    <Serializable()>
    Public MustInherit Class SimpleOrExpressionBase(Of T)



        Public Property Mode As SimpleOrExpressionMode

        <Required(True)> _
        <ConditionalVisible("Mode", False, True, SimpleOrExpressionMode.Simple)>
        Public Overridable Property Simple As T


        Public MustOverride Function GetExpression() As SimpleExpression(Of T)



        Public Function GetValue() As T
            Return GetValue(DnnContext.Current, DnnContext.Current)
        End Function

        Public Shared Function GetValues(sourceCollec As IEnumerable(Of SimpleOrExpressionBase(Of T)), owner As Object, dataContext As IContextLookup) As IEnumerable(Of T)
            Return sourceCollec.Select(Function(objExp) objExp.GetValue(owner, dataContext))
        End Function

        Public Function GetValue(owner As Object, dataContext As IContextLookup) As T
            Select Case Mode
                Case SimpleOrExpressionMode.Simple
                    Return Simple
                Case SimpleOrExpressionMode.Expression
                    Return Me.GetExpression.Evaluate(owner, dataContext)
            End Select
        End Function

    End Class
End Namespace