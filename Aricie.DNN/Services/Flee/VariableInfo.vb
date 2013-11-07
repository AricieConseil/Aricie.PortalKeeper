Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    ''' <summary>
    ''' Variable information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public MustInherit Class VariableInfo
        Inherits NamedEntity
        Implements IProviderSettings

        Protected VarConstant As Object

        ''' <summary>
        ''' Gets the variable type as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property VariableType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(Object))
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets Evaluation mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Overridable Property EvaluationMode() As VarEvaluationMode= VarEvaluationMode.Dynamic

        ''' <summary>
        ''' Gets or sets the scope
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Overridable Property Scope() As VariableScope = VariableScope.Local

        ''' <summary>
        ''' Gets ot sets the provider name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
            <XmlIgnore()> _
        <IsReadOnly(True)> _
        Public Property ProviderName() As String = String.Empty Implements ComponentModel.IProviderSettings.ProviderName
          

        Public MustOverride Function EvaluateOnce(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object

        ''' <summary>
        ''' Evaluates the value of the variable
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            Dim toReturn As Object
            Select Case Me._EvaluationMode
                Case VarEvaluationMode.Constant
                    If VarConstant Is Nothing Then
                        VarConstant = Me.EvaluateOnce(owner, globalVars)
                    End If
                    toReturn = VarConstant
                Case Else
                    toReturn = Me.EvaluateOnce(owner, globalVars)
            End Select
            If Me._Scope = VariableScope.Global AndAlso globalVars IsNot Nothing Then
                globalVars.Items(Me.Name) = toReturn
            End If
            Return toReturn
        End Function
    End Class

    ''' <summary>
    ''' Instance modes types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum InstanceMode
        Off
        ContextLess
        InContextEval
    End Enum


    ''' <summary>
    ''' Generics variable information
    ''' </summary>
    ''' <remarks></remarks>
    <DisplayName("Instance")> _
    <Serializable()> _
    Public Class VariableInfo(Of TResult)
        Inherits VariableInfo

        Private _InstanceMode As InstanceMode = GetDefaultInstanceMode()

        Private _Instance As TResult

        ''' <summary>
        ''' Returns context less instance mode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetDefaultInstanceMode() As InstanceMode
            Return Flee.InstanceMode.ContextLess
        End Function

        ''' <summary>
        ''' Gets the type of the generic
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        Public Overrides ReadOnly Property VariableType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(TResult))
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets Instance mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Overridable Property InstanceMode() As InstanceMode
            Get
                Return _InstanceMode
            End Get
            Set(ByVal value As InstanceMode)
                _InstanceMode = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether to use clones
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UseClone() As Boolean

        ''' <summary>
        ''' Get or sets the instance of the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("InstanceMode", True, True, InstanceMode.Off)> _
            <LabelMode(LabelMode.Top)> _
            <Width(500)> _
        Public Property Instance() As TResult
            Get
                If Me._Instance Is Nothing AndAlso Me._InstanceMode = Flee.InstanceMode.ContextLess Then
                    Me._Instance = DirectCast(Me.EvaluateOnce(DnnContext.Current, DnnContext.Current), TResult)
                End If
                Return _Instance
            End Get
            Set(ByVal value As TResult)
                If Me._InstanceMode <> Flee.InstanceMode.Off Then
                    _Instance = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Evaluates the variable 
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            If Me._Instance IsNot Nothing Then
                If Me._InstanceMode <> Flee.InstanceMode.Off Then
                    If Scope = VariableScope.Global AndAlso globalVars IsNot Nothing Then
                        globalVars.Items(Me.Name) = Me._Instance
                    End If
                    Return Me._Instance
                Else
                    Me._Instance = Nothing
                End If
            End If
            Dim toReturn As Object = MyBase.Evaluate(owner, globalVars)
            If Me._InstanceMode = Flee.InstanceMode.InContextEval Then
                Me._Instance = DirectCast(toReturn, TResult)
            End If
            If _UseClone Then
                Return ReflectionHelper.CloneObject(toReturn)
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' Evaluate variable once
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function EvaluateOnce(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            Return ReflectionHelper.CreateObject(GetType(TResult))
        End Function


    End Class
End Namespace