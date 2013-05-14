Imports System.CodeDom

'Imports System.Workflow.Activities.Rules

Namespace Business.Filters
    Public Interface IDescriptor
        Function GetCodeExpression() As CodeExpression

        Function GetArgs() As String
    End Interface
End Namespace
