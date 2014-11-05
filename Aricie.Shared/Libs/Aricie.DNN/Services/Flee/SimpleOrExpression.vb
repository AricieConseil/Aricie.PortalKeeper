Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

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
        Inherits SimpleOrExpressionBase(Of T, T)


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
                    Dim toReturn As T = Me.GetExpression.Evaluate(owner, dataContext)
                    If toReturn Is Nothing AndAlso CreateIfNull Then
                        toReturn = Simple
                    End If
                    Return toReturn
            End Select
        End Function



    End Class



    <DefaultProperty("FriendlyName")> _
    <Serializable()>
    Public MustInherit Class SimpleOrExpressionBase(Of TSimple, TExpression)

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                If Mode = SimpleOrExpressionMode.Simple Then
                    Return ReflectionHelper.GetFriendlyName(Simple)
                Else
                    Return GetExpression.Expression
                End If
            End Get
        End Property

        Public Property Mode As SimpleOrExpressionMode

        <AutoPostBack()> _
        <ConditionalVisible("Mode", False, True, SimpleOrExpressionMode.Expression)>
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property DisplaySimple As Boolean
            Get
                Return Mode = SimpleOrExpressionMode.Simple OrElse CreateIfNull
            End Get
        End Property

        <SortOrder(100)> _
        <Width(500)> _
        <Required(True)> _
        <ConditionalVisible("DisplaySimple", False, True)>
        Public Overridable Property Simple As TSimple = ReflectionHelper.CreateObject(Of TSimple)()


        Public MustOverride Function GetExpression() As SimpleExpression(Of TExpression)





    End Class
End Namespace