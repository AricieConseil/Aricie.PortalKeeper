Imports System.CodeDom
Imports System.Reflection
Imports Aricie.Services

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    ''' <summary>
    ''' IFilter Implementation with a Predicate
    ''' </summary>
    Public Class PredicateFilter(Of T)
        Implements IFilter


        Public Sub New(ByVal propName As IConvertible, _
                        ByVal objPredicate As Predicate(Of T))

            Me._PropertyName = propName
            Me._Predicate = objPredicate

        End Sub

#Region "private members"

        Private _PropertyName As IConvertible = ""

        Private _Predicate As Predicate(Of T)


#End Region

#Region "Public Properties"

        Public Property PropertyName() As IConvertible
            Get
                Return Me._PropertyName
            End Get
            Set(ByVal value As IConvertible)
                Me._PropertyName = value
            End Set
        End Property


        Public Property Predicate() As Predicate(Of T)
            Get
                Return _Predicate
            End Get
            Set(ByVal value As Predicate(Of T))
                _Predicate = value
            End Set
        End Property


        Public ReadOnly Property IsSimpleMatch() As Boolean Implements IFilter.IsSimpleMatch
            Get
                Return True
            End Get
        End Property

#End Region


        Protected Overridable Function GetSimpleMatch(Of Y)(ByVal content As Y) As Boolean
            Dim _
                myProperty As PropertyInfo = _
                    ReflectionHelper.GetPropertiesDictionary(GetType(Y))(Me.PropertyName.ToString(Nothing))

            Dim toReturn As Boolean = False
            Return Me.Predicate.Invoke(DirectCast(myProperty.GetValue(content, Nothing), T))


        End Function

        Public Function Match(Of Y)(ByVal content As Y) As Boolean Implements IFilter.Match

            Dim toReturn As Boolean

            If Me.IsSimpleMatch Then
                toReturn = Me.GetSimpleMatch(Of Y)(content)

            Else
                'toReturn = content.Evaluate(Me)


            End If

            Return toReturn
        End Function

        'Public Function Evaluate(ByVal obj As Object) As Object
        '    'Dim codeExp As CodeExpression = Me.GetCodeExpression()
        '    'Dim args As New Dictionary(Of String, String)
        '    'args.Add("f-", Me.GetArgs())
        '    'Dim ruleValid As RuleValidation = CacheHelper.GetGlobal(Of RuleValidation)(args)
        '    'Dim ruleExec As RuleExecution
        '    'If (ruleValid Is Nothing) Then
        '    '    ruleValid = New RuleValidation(GetType(T), Nothing)
        '    '    RuleExpressionWalker.Validate(ruleValid, codeExp, False)
        '    '    CacheHelper.SetGlobal(Of RuleValidation)(ruleValid, args)
        '    'End If
        '    'ruleExec = New RuleExecution(ruleValid, Me)
        '    'Return RuleExpressionWalker.Evaluate(ruleExec, codeExp).Value
        'End Function


        Public Overridable Function GetCodeExpression() As CodeExpression Implements IFilter.GetCodeExpression
            Throw New NotImplementedException("No Code exp yet for Predicate Filters")
        End Function

        Public Overridable Function GetTempArgs() As String

            Return "f" & (Me.PropertyName.ToString(Nothing) & "-" & Me.Predicate.ToString)

        End Function


        Public Function GetArgs() As String Implements IFilter.GetArgs

            Return GetTempArgs()

        End Function
    End Class
End Namespace
