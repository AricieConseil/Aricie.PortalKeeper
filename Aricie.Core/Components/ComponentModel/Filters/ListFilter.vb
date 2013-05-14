Imports System.CodeDom
Imports System.Reflection
Imports Aricie.Services

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    Public Enum ScopeOperator
        Any
        All
    End Enum

    Public Class ListFilter(Of T)
        Implements IFilter


        Public Sub New(ByVal propName As IConvertible, ByVal innerFilter As IFilter, ByVal scope As ScopeOperator)
            Me._PropertyName = propName
            Me._Scope = scope
            Me._InnerFilter = innerFilter

        End Sub


#Region "private members"

        Private _PropertyName As IConvertible = ""
        Private _Scope As ScopeOperator
        Private _InnerFilter As IFilter

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

        Public Property Scope() As ScopeOperator
            Get
                Return Me._Scope
            End Get
            Set(ByVal value As ScopeOperator)
                Me._Scope = value
            End Set
        End Property


        Public Property InnerFilter() As IFilter
            Get
                Return _InnerFilter
            End Get
            Set(ByVal value As IFilter)
                _InnerFilter = value
            End Set
        End Property

#End Region

        Public ReadOnly Property IsSimpleMatch() As Boolean Implements IFilter.IsSimpleMatch
            Get
                Return True
            End Get
        End Property

        Public Function Match(Of T1)(ByVal content As T1) As Boolean Implements IFilter.Match

            If IsSimpleMatch Then
                Return GetSimpleMatch(Of T1)(content)
            End If

        End Function


        Protected Function GetSimpleMatch(Of Y)(ByVal content As Y) As Boolean

            Dim _
                myProperty As PropertyInfo = _
                    ReflectionHelper.GetPropertiesDictionary(GetType(Y)).Item(Me.PropertyName.ToString(Nothing))

            Dim innerList As IList = CType(myProperty.GetValue(Me, Nothing), IList)

            Dim match As Boolean
            Select Case Scope
                Case ScopeOperator.All
                    match = False
                Case ScopeOperator.Any
                    match = True
            End Select
            Dim tempMatch As Boolean
            For Each innerObj As T In innerList

                tempMatch = _InnerFilter.Match(content)

                If tempMatch = match Then
                    Return match
                End If

            Next

            Return tempMatch

        End Function


        Public Function GetCodeExpression() As CodeExpression Implements IDescriptor.GetCodeExpression
            'If Me.Scope = ScopeOperator.All Or Not Me.IsSimpleMatch Then
            Throw _
                New NotSupportedException( _
                                           "scope ""All"" not supported in complex expressions with list filters nor complex inner filters")
            'End If

            'Dim args As String = GetTempArgs()

            'Dim containsInvokeCodeDomExp As CodeMethodInvokeExpression = CacheHelper.GetGlobal(Of CodeMethodInvokeExpression)(args)
            'If (containsInvokeCodeDomExp Is Nothing) Then

            '    'Dim primCodeDomExp As New System.CodeDom.CodePrimitiveExpression(Me.Value)
            '    Dim thisCodeDomExp As New System.CodeDom.CodeThisReferenceExpression()
            '    Dim propCodeDomExp As New System.CodeDom.CodePropertyReferenceExpression(thisCodeDomExp, Me.PropertyName.ToString(Nothing))
            '    Dim containsRefCodeDomExp As New System.CodeDom.CodeMethodReferenceExpression(propCodeDomExp, "Contains")
            '    containsInvokeCodeDomExp = New System.CodeDom.CodeMethodInvokeExpression(containsRefCodeDomExp, primCodeDomExp)

            '    CacheHelper.SetGlobal(Of CodeMethodInvokeExpression)(containsInvokeCodeDomExp, args)
            'Else

            '    CType(containsInvokeCodeDomExp.Parameters(0), CodePrimitiveExpression).Value = Me.Value
            'End If

            'Return containsInvokeCodeDomExp
        End Function


        Public Function GetArgs() As String Implements IDescriptor.GetArgs
            Return "f" & Me.Scope.ToString & "-" & Me._InnerFilter.GetArgs
        End Function
    End Class
End Namespace
