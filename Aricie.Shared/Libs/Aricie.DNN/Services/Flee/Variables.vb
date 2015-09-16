Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports System.Reflection
Imports Aricie.DNN.Entities

Namespace Services.Flee

   

    ''' <summary>
    ''' Generics version of VariablesBase
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class Variables(Of TResult)
        Inherits VariablesBase

        ''' <summary>
        ''' Returns a list containing the generics type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetInitialTypes() As System.Collections.Generic.IList(Of ComponentModel.DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(GetType(TResult)))
            Return toReturn
        End Function

        ''' <summary>
        ''' Evaluate the variable
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EvaluateGeneric(ByVal owner As Object, ByVal globalVars As IContextLookup) As SerializableDictionary(Of String, TResult)
            Dim tempDico As SerializableDictionary(Of String, Object) = Me.EvaluateVariables(owner, globalVars)
            Dim toReturn As New SerializableDictionary(Of String, TResult)(tempDico.Count)
            For Each tempPair As KeyValuePair(Of String, Object) In tempDico
                toReturn.Add(tempPair.Key, DirectCast(tempPair.Value, TResult))
            Next
            Return toReturn
        End Function

        <Obsolete("user other signature")> _
        Public Function EvaluateGeneric(ByVal owner As Object, ByVal globalVars As IContextLookup, forceStatic As Boolean) As SerializableDictionary(Of String, TResult)
            Return EvaluateGeneric(owner, globalVars)
        End Function

        Public Overrides Function GetAvailableProviders() As System.Collections.Generic.IDictionary(Of String, DotNetType(Of VariableInfo))

            Dim toReturn As New Dictionary(Of String, DotNetType(Of VariableInfo))
            Dim objGeneralDotNetType As New DotNetType(Of VariableInfo)(GetType(GeneralVariableInfo(Of )), New DotNetType(GetType(TResult)))
            toReturn.Add(ReflectionHelper.GetSimpleTypeName(objGeneralDotNetType.GetDotNetType()), objGeneralDotNetType)

            Return toReturn
        End Function


    End Class

    ''' <summary>
    ''' Variables in flee
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class Variables
        Inherits VariablesBase


        Public Shared Function GetFromParameters(objParameters As ParameterInfo()) As Variables
            Dim toReturn As New Variables()
            For Each objParameter As ParameterInfo In objParameters
                Dim newVar As New GeneralVariableInfo()
                newVar.Name = objParameter.Name
                newVar.DotNetType = New DotNetType(objParameter.ParameterType)
                If Not ReflectionHelper.CanCreateObject(objParameter.ParameterType) Then
                    newVar.VariableMode = VariableMode.Expression
                    newVar.FleeExpression.Expression = "null"
                Else
                    newVar.VariableMode = VariableMode.Instance
                    'If objParameter.DefaultValue IsNot Nothing AndAlso Not IsDBNull(objParameter.DefaultValue) Then
                    '    newVar.Instance = objParameter.DefaultValue
                    'End If
                End If
                toReturn.Instances.Add(newVar)
            Next
            Return toReturn
        End Function


        Private Shared ReadOnly _GenerictypeLessTypeDelegateVar As Type = GetType(DelegateVariableInfo(Of ))

        '<Category("")> _
        'Public Property ShowAvailableTypes() As Boolean

        '<AutoPostBack()> _
        '<ConditionalVisible("ShowAvailableTypes", False, True)> _
        'Public Property AddDelegates As Boolean

        ' ''' <summary>
        ' ''' Gets or sets expression types
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<Editor(GetType(ListEditControl), GetType(EditControl))> _
        '<CollectionEditor(False, False, True, True, 5, CollectionDisplayStyle.List)> _
        '<LabelMode(LabelMode.Top)> _
        '<ConditionalVisible("ShowAvailableTypes", False, True)> _
        'Public Property ExpressionTypes() As List(Of DotNetType)
        '    Get
        '        Return _ExpressionTypes
        '    End Get
        '    Set(ByVal value As List(Of DotNetType))
        '        _ExpressionTypes = value
        '    End Set
        'End Property

        ''' <summary>
        ''' Returns a list containing types String and boolean
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetInitialTypes() As IList(Of DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(GetType(String)))
            toReturn.Add(New DotNetType(GetType(Boolean)))
            Return toReturn
        End Function

        Protected Overrides Function Genericize(simpleDotNetType As DotNetType) As Dictionary(Of String, DotNetType(Of VariableInfo))
            Dim toReturn As Dictionary(Of String, DotNetType(Of VariableInfo)) = MyBase.Genericize(simpleDotNetType)
            'If Me.AddDelegates Then
            '    Dim simpleType As Type = simpleDotNetType.GetDotNetType()
            '    If simpleType IsNot Nothing Then
            '        Dim toAddCtor As New DotNetType(Of VariableInfo)(_GenerictypeLessTypeDelegateVar, simpleDotNetType)
            '        toReturn.Add(toAddCtor.Name, toAddCtor)
            '    End If
            'End If
            Return toReturn
        End Function

        '<ConditionalVisible("ShowAvailableTypes", False, True)> _
        '<ActionButton("~/images/action_refresh.gif")> _
        'Public Sub ApplyUpdates(ByVal pe As AriciePropertyEditorControl)
        '    If pe IsNot Nothing Then
        '        pe.ItemChanged = True
        '        'Me.ShowAvailableTypes = False
        '    End If
        'End Sub

    End Class


End Namespace