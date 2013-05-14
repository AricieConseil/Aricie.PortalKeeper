Imports System.CodeDom
Imports System.Reflection
Imports Aricie.Services

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    ''' <summary>
    ''' Simple Reflection based IFilter
    ''' </summary>
    Public Class SimpleFilter(Of T As {IComparable})
        Implements IFilter


        Public Sub New(ByVal propName As IConvertible, _
                        ByVal op As CodeBinaryOperatorType, _
                        ByVal objValue As T)

            Me.PropertyName = propName
            Me.Operator = op
            Me.Value = objValue

        End Sub

#Region "private members"

        Private _PropertyName As IConvertible = ""
        Private _Operator As CodeBinaryOperatorType = CodeBinaryOperatorType.ValueEquality
        Private _Value As T = Nothing


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

        Public Property [Operator]() As CodeBinaryOperatorType
            Get
                Return Me._Operator
            End Get
            Set(ByVal value As CodeBinaryOperatorType)
                Me._Operator = value
            End Set
        End Property

        Public Property Value() As T
            Get
                Return Me._Value
            End Get
            Set(ByVal value As T)
                Me._Value = value
            End Set
        End Property

        Public ReadOnly Property IsSimpleMatch() As Boolean Implements IFilter.IsSimpleMatch
            Get
                'Return Me.Operator = CodeBinaryOperatorType.ValueEquality
                Return True
            End Get
        End Property

#End Region


        Protected Overridable Function GetSimpleMatch(Of Y)(ByVal content As Y) As Boolean
            Dim _
                myProperty As PropertyInfo = _
                    ReflectionHelper.GetPropertiesDictionary(GetType(Y))(Me.PropertyName.ToString(Nothing))

            Dim toReturn As Boolean = False
            Select Case Me.Operator
                Case CodeBinaryOperatorType.ValueEquality
                    Dim Com As T = CType(myProperty.GetValue(content, Nothing), T)
                    toReturn = (Com.CompareTo(Me.Value) = 0)
                Case CodeBinaryOperatorType.GreaterThan
                    toReturn = (CType(myProperty.GetValue(content, Nothing), T).CompareTo(Me.Value) > 0)
                Case CodeBinaryOperatorType.GreaterThanOrEqual
                    toReturn = (CType(myProperty.GetValue(content, Nothing), T).CompareTo(Me.Value) >= 0)
                Case CodeBinaryOperatorType.LessThan
                    toReturn = (CType(myProperty.GetValue(content, Nothing), T).CompareTo(Me.Value) < 0)
                Case CodeBinaryOperatorType.LessThanOrEqual
                    toReturn = (CType(myProperty.GetValue(content, Nothing), T).CompareTo(Me.Value) <= 0)
                Case CodeBinaryOperatorType.IdentityEquality
                    toReturn = (CType(myProperty.GetValue(content, Nothing), T).Equals(Me.Value))
                Case Else
                    Throw _
                        New NotImplementedException( _
                                                     "no complex filter implemented until reference is made to .Net 3.0 workflow foundation")
            End Select
            Return toReturn

        End Function

        Public Function Match(Of Y)(ByVal content As Y) As Boolean Implements IFilter.Match

            'Dim toReturn As Boolean

            'If Me.IsSimpleMatch Then
            '    toReturn = Me.GetSimpleMatch(Of Y)(content)

            'Else
            'toReturn = content.Evaluate(Me)


            'End If

            'Return toReturn
            Return Me.GetSimpleMatch(Of Y)(content)
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
            Dim args As String = GetArgs()


            Dim binCodeDomExp As CodeBinaryOperatorExpression = GetGlobal(Of CodeBinaryOperatorExpression)(args)
            If (binCodeDomExp Is Nothing) Then

                Dim primCodeDomExp As New CodePrimitiveExpression(Me.Value)
                Dim thisCodeDomExp As New CodeThisReferenceExpression()
                Dim _
                    propCodeDomExp As _
                        New CodePropertyReferenceExpression(thisCodeDomExp, Me.PropertyName.ToString(Nothing))

                binCodeDomExp = New CodeBinaryOperatorExpression(propCodeDomExp, Me.Operator, primCodeDomExp)
                SetGlobal(Of CodeBinaryOperatorExpression)(binCodeDomExp, args)
            Else

                CType(binCodeDomExp.Right, CodePrimitiveExpression).Value = Me.Value
            End If

            Return binCodeDomExp
        End Function

        Public Overridable Function GetTempArgs() As String

            Return "f" & (Me.PropertyName.ToString(Nothing) & "-" & Me.Operator.ToString)

        End Function


        Public Function GetArgs() As String Implements IFilter.GetArgs

            Return GetTempArgs() & "-" & Me.Value.ToString

        End Function
    End Class
End Namespace
