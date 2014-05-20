Imports Aricie.DNN.ComponentModel
Imports Ciloci.Flee
Imports Aricie.Services
Imports Aricie.Collections

Namespace Services.Flee

   
    ''' <summary>
    ''' Base class for variables 
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class VariablesBase
        Inherits ProviderHost(Of DotNetType(Of VariableInfo), VariableInfo, IGenericizer(Of VariableInfo))


        Protected _ExpressionTypes As New List(Of DotNetType)
        Private Shared ReadOnly _GenerictypeLessTypeExpressionVar As Type = GetType(ExpressionVariableInfo(Of ))
        Private Shared ReadOnly _GenerictypeLessTypeCTorVar As Type = GetType(CtorVariableInfo(Of ))
        Private Shared ReadOnly _GenerictypeLessTypeVar As Type = GetType(VariableInfo(Of ))

        <Obsolete("user other signature")> _
        Public Function EvaluateVariables(ByVal owner As Object, ByVal globalVars As IContextLookup, forceStatic As Boolean) As SerializableDictionary(Of String, Object)
            Return EvaluateVariables(owner, globalVars)
        End Function

        ''' <summary>
        ''' Returns collection of evaluated variables
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EvaluateVariables(ByVal owner As Object, ByVal globalVars As IContextLookup) As SerializableDictionary(Of String, Object)
            Dim toReturn As New SerializableDictionary(Of String, Object)
            Dim siblingsContext As SimpleContextLookup = Nothing
            For Each objVar As VariableInfo In Me.Instances
                Dim varValue As Object
                If siblingsContext Is Nothing Then
                    varValue = objVar.Evaluate(owner, globalVars)
                Else
                    varValue = objVar.Evaluate(owner, siblingsContext)
                End If

                toReturn(objVar.Name) = varValue
                If objVar.Scope = VariableScope.LocalAndSiblings Then
                    If siblingsContext Is Nothing Then
                        siblingsContext = New SimpleContextLookup(globalVars)
                    End If
                    siblingsContext.Items(objVar.Name) = varValue
                End If
            Next
            Return toReturn
        End Function

        ''' <summary>
        ''' Returns the available providers for the variables
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetAvailableProviders() As System.Collections.Generic.IDictionary(Of String, DotNetType(Of VariableInfo))
            If Me._ExpressionTypes.Count = 0 Then
                Me._ExpressionTypes.AddRange(GetInitialTypes())
            End If

            Return (From simpleDotNetType In Me._ExpressionTypes From objKeyPair In Me.Genericize(simpleDotNetType) Select objKeyPair).ToDictionary(Function(objKeyPair) objKeyPair.Key, Function(objKeyPair) objKeyPair.Value)
        End Function


        Protected Overridable Function Genericize(simpleDotNetType As DotNetType) As Dictionary(Of String, DotNetType(Of VariableInfo))
            Dim toReturn As New Dictionary(Of String, DotNetType(Of VariableInfo))
            Dim toAddVar As New DotNetType(Of VariableInfo)(_GenerictypeLessTypeVar, simpleDotNetType)
            toReturn.Add(toAddVar.Name, toAddVar)
            Dim simpleType As Type = simpleDotNetType.GetDotNetType
            If simpleType IsNot Nothing Then
                If simpleType.GetConstructors.Length > 0 Then
                    If simpleType.Name <> "String" Then
                        Dim toAddCtor As New DotNetType(Of VariableInfo)(_GenerictypeLessTypeCTorVar, simpleDotNetType)
                        toReturn.Add(toAddCtor.Name, toAddCtor)
                    End If
                End If
                Dim toAddExp As New DotNetType(Of VariableInfo)(_GenerictypeLessTypeExpressionVar, simpleDotNetType)
                toReturn.Add(toAddExp.Name, toAddExp)
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Returns a list containing the object type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetInitialTypes() As IList(Of DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(ReflectionHelper.GetSafeTypeName(GetType(Object))))
            Return toReturn
        End Function

       

    End Class
End Namespace