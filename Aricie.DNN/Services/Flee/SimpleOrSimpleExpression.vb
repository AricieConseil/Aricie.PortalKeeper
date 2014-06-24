Imports Aricie.DNN.UI.Attributes

Namespace Services.Flee
    <Serializable()>
    Public Class SimpleOrSimpleExpression(Of T)
        Inherits SimpleOrExpressionBase(Of T)

        Public Sub New()

        End Sub

        Public Sub New(value As T)
            Me.New(value, False)
        End Sub


        Public Sub New(value As Object, isExpression As Boolean)
            If isExpression Then
                Me.Expression = New SimpleExpression(Of T)(DirectCast(value, String))
                Me.Mode = SimpleOrExpressionMode.Expression
            Else
                Me.Simple = DirectCast(value, T)
                Me.Mode = SimpleOrExpressionMode.Simple
            End If
        End Sub

        <ConditionalVisible("Mode", False, True, SimpleOrExpressionMode.Expression)>
        Public Property Expression As New SimpleExpression(Of T)

        Public Overrides Function GetExpression() As SimpleExpression(Of T)
            Return Expression
        End Function

    End Class
End NameSpace