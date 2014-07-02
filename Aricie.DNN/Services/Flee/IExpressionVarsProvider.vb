Namespace Services.Flee
    Public Interface IExpressionVarsProvider

        Sub AddVariables(ByVal currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))

    End Interface
End NameSpace