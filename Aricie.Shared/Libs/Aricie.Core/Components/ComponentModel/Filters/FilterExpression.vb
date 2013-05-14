Imports System.CodeDom
Imports Aricie.Services

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    ''' <summary>
    ''' IFilter compund Boolean expression
    ''' </summary>
    Public Class FilterExpression
        Inherits List(Of FilterInExpression)
        Implements IFilter

#Region "cTors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal left As IFilter)
            MyBase.New()
            Me.Add(left, OperatorFilterExp.And)
        End Sub

        Public Sub New(ByVal objOperator As OperatorFilterExp, ByVal ParamArray filters As IFilter())

            Me.New()
            For Each Filter As IFilter In filters
                Me.Add(New FilterInExpression(Filter, objOperator))
            Next


        End Sub

        Public Sub New(ByVal subFilter As List(Of FilterInExpression))
            MyBase.New(subFilter)
        End Sub

#End Region


#Region "Public Properties"

        Public ReadOnly Property IsSimpleMatch() As Boolean Implements IFilter.IsSimpleMatch
            Get
                Return Me.TrueForAll(New Predicate(Of FilterInExpression)(AddressOf IsSimpleMatchFilter))
            End Get
        End Property

#End Region

        Public Shared Function IsSimpleMatchFilter(ByVal inFilter As FilterInExpression) As Boolean
            Return inFilter.Filter.IsSimpleMatch
        End Function


        Public Overloads Sub Add(ByVal filter As IFilter, ByVal op As OperatorFilterExp)
            Me.Add(New FilterInExpression(filter, op))
        End Sub


        Public Overridable Function Match(Of T)(ByVal content As T) As Boolean Implements IFilter.Match

            Dim toReturn As Boolean = False
            For i As Integer = 0 To Me.Count - 1

                Dim inFilter As FilterInExpression = Me(i)
                If inFilter.OperatorFilterExp = OperatorFilterExp.And Then
                    If Not inFilter.Filter.Match(content) Then
                        Return False
                    Else
                        toReturn = True
                    End If
                Else
                    If inFilter.Filter.Match(content) Then
                        Return True
                    End If
                End If
            Next

            Return toReturn
            'Return content.Evaluate(Me)


        End Function

        Public Overridable Function GetCodeExpression() As CodeExpression Implements IFilter.GetCodeExpression

            Dim cacheKey As String = Me.GetArgs()
            Dim codeDomExp As CodeExpression = GetGlobal(Of CodeBinaryOperatorExpression)(cacheKey)
            If (codeDomExp Is Nothing) Then

                If Me.Count > 1 Then
                    Dim left As CodeExpression = Me(0).Filter.GetCodeExpression
                    Dim op As CodeBinaryOperatorType
                    Select Case Me(1).OperatorFilterExp
                        Case OperatorFilterExp.And
                            op = CodeBinaryOperatorType.BooleanAnd
                        Case OperatorFilterExp.Or
                            op = CodeBinaryOperatorType.BooleanOr
                    End Select
                    Dim right As CodeExpression = New FilterExpression(Me.GetRange(1, Me.Count - 1)).GetCodeExpression
                    codeDomExp = New CodeBinaryOperatorExpression(left, op, right)

                ElseIf Me.Count = 0 Then
                    codeDomExp = Me(0).Filter.GetCodeExpression
                Else
                    codeDomExp = New CodePrimitiveExpression(True)
                End If

                SetGlobal(Of CodeBinaryOperatorExpression)(codeDomExp, cacheKey)

            End If

            Return codeDomExp

        End Function


        Public Function GetArgs() As String Implements IFilter.GetArgs

            Dim toReturn As String = "fe"
            For i As Integer = 0 To Me.Count - 1
                toReturn &= "-f-" & i & "-" & Me(i).OperatorFilterExp.ToString & "-" & Me(i).Filter.GetArgs
            Next
            toReturn &= "-fe"
            Return toReturn
        End Function
    End Class
End Namespace
