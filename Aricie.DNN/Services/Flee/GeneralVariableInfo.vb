Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services
Imports Aricie.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Reflection
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Services.Localization

Namespace Services.Flee
    Public Class GeneralVariableInfo
        Inherits VariableInfo
        Implements ISelector(Of MethodInfo)
        Implements IExpressionVarsProvider


        Private _Instance As Object
        Private _SimpleExpression As SimpleExpression(Of Object)
        Private _FleeExpression As New FleeExpressionInfo(Of Object)

        Public Property DotNetType As New DotNetType()

        <Browsable(False)> _
        Public ReadOnly Property HasType As Boolean
            Get
                Return DotNetType.GetDotNetType() IsNot Nothing
            End Get
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property VariableType As String
            Get
                Return DotNetType.TypeName
            End Get
        End Property

        <ConditionalVisible("HasType", False, True)> _
        Public Property VariableMode As VariableMode = VariableMode.Instance

        <ExtendedCategory("", "Evaluation")> _
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)> _
        <ConditionalVisible("HasType", False, True)> _
        Public Property InstanceMode As InstanceMode = InstanceMode.Off

        <ExtendedCategory("", "Evaluation")> _
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)> _
        <ConditionalVisible("HasType", False, True)> _
        Public Overrides Property EvaluationMode() As VarEvaluationMode
            Get
                Return MyBase.EvaluationMode
            End Get
            Set(ByVal value As VarEvaluationMode)
                MyBase.EvaluationMode = value
            End Set
        End Property

        <ExtendedCategory("", "Evaluation")> _
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)> _
        <ConditionalVisible("HasType", False, True)> _
        Public Overrides Property Scope() As VariableScope
            Get
                Return MyBase.Scope
            End Get
            Set(ByVal value As VariableScope)
                MyBase.Scope = value
            End Set
        End Property


        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Instance, VariableMode.Expression)> _
        Public Property UseClone() As Boolean

        ''' <summary>
        ''' Get or sets the instance of the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Instance)> _
        <XmlIgnore()> _
        Public Property Instance As Object
            Get
                If Me.DotNetType.GetDotNetType() IsNot Nothing AndAlso (Me._Instance Is Nothing _
                                                                            OrElse (Me._Instance.GetType() IsNot Me.DotNetType.GetDotNetType() _
                                                                                    AndAlso Not ReflectionHelper.CanConvert(Me._Instance.GetType(), Me.DotNetType.GetDotNetType()))) _
                                                                        AndAlso (Me.VariableMode = Flee.VariableMode.Instance OrElse Me._InstanceMode = Flee.InstanceMode.ContextLess) Then
                    Me._Instance = Me.EvaluateOnce(DnnContext.Current, DnnContext.Current)
                End If
                Return Me._Instance
            End Get
            Set(value As Object)
                If value IsNot Nothing AndAlso value.GetType() IsNot Me.DotNetType.GetDotNetType() Then
                    value = Conversions.ChangeType(value, Me.DotNetType.GetDotNetType())
                End If
                _Instance = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property SerializableInstance() As Serializable(Of Object)
            Get
                If Me.VariableMode = Flee.VariableMode.Instance OrElse Me.InstanceMode <> Flee.InstanceMode.Off Then
                    Return New Serializable(Of Object)(Instance)
                End If
                Return Nothing
            End Get
            Set(ByVal value As Serializable(Of Object))
                Me.Instance = value.Value
            End Set
        End Property




        ''' <summary>
        ''' Is the variable and advanced expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)> _
        Public Property AdvancedExpression() As Boolean

        ''' <summary>
        ''' Gets the flee expression that will be used
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)> _
        <ConditionalVisible("AdvancedExpression", False, True)> _
        Public Property FleeExpression() As FleeExpressionInfo(Of Object)
            Get
                Return _FleeExpression
            End Get
            Set(value As FleeExpressionInfo(Of Object))
                _FleeExpression = value
                _SimpleExpression = Nothing
            End Set
        End Property

        ''' <summary>
        ''' Retrieve the simple expression that will be evaluated
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)> _
        <ConditionalVisible("AdvancedExpression", True, True)> _
        Public Property SimpleExpression() As SimpleExpression(Of Object)
            Get
                If _SimpleExpression Is Nothing Then
                    _SimpleExpression = New SimpleExpression(Of Object)(Me.FleeExpression)
                End If
                Return _SimpleExpression
            End Get
            Set(ByVal value As SimpleExpression(Of Object))
                _SimpleExpression = value
                _SimpleExpression.SlaveExpression = FleeExpression
                Me.FleeExpression.Expression = _SimpleExpression.Expression
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the expression should be compiled or evaluated
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)> _
        Public Property AsCompiledExpression() As Boolean


        <AutoPostBack()> _
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Delegate)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Name", "Name", False, True, "<Select a Method Name>", "", False, True)> _
        Public Property MethodName As String = ""




        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ProvidersSelector("Key", "Value")> _
        <ConditionalVisible("HasType", False, True)> _
        <ConditionalVisible("VariableMode", False, True, VariableMode.Constructor, VariableMode.Delegate)> _
        Public Property MethodIndex As Integer = 1

        <Browsable(False)> _
        Public ReadOnly Property RequiresInstance As Boolean
            Get
                Dim targetMethod As MethodInfo = Me.GetMethodInfo()
                If targetMethod IsNot Nothing Then
                    Return targetMethod.IsStatic
                End If
                Return False
            End Get
        End Property

        <ConditionalVisible("RequiresInstance", False, True)>
        Public Property TargetInstance As New SimpleExpression(Of Object)

        <ConditionalVisible("HasType", False, True)> _
<ConditionalVisible("VariableMode", False, True, VariableMode.Constructor)> _
        Public Property Parameters() As New Variables

        <ConditionalVisible("VariableMode", False, True, VariableMode.Constructor)> _
       <ActionButton(IconName.Key, IconOptions.Normal)> _
        Public Sub SetParameters(ape As AriciePropertyEditorControl)
            If Me.VariableMode = VariableMode.Constructor Then
                Dim objParameters As ParameterInfo()

                objParameters = SelectedMember.GetParameters()
                Me.Parameters = Variables.GetFromParameters(objParameters)
                ape.ItemChanged = True
                Dim message As String = Localization.GetString("ParametersCreated.Message", ape.LocalResourceFile)
                ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
            End If
        End Sub


        ''' <summary>
        ''' Evaluates the variable 
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            Dim toReturn As Object
            'Select Case Me.VariableMode
            '    Case Flee.VariableMode.Constructor, Flee.VariableMode.Expression, Flee.VariableMode.Delegate
            If Me._Instance IsNot Nothing Then
                If Me.VariableMode = Flee.VariableMode.Instance OrElse Me._InstanceMode <> Flee.InstanceMode.Off Then
                    If Scope = VariableScope.Global AndAlso globalVars IsNot Nothing Then
                        globalVars.Items(Me.Name) = Me._Instance
                    End If
                    Return Me._Instance
                Else
                    Me._Instance = Nothing
                End If
            End If
            toReturn = MyBase.Evaluate(owner, globalVars)
            If Me._InstanceMode = InstanceMode.InContextEval Then
                Me._Instance = toReturn
            End If
            'Case Else
            'toReturn = MyBase.Evaluate(owner, globalVars)
            'End Select
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
            Dim toReturn As Object = Nothing
            Select Case Me.VariableMode
                Case Flee.VariableMode.Instance
                    If ReflectionHelper.CanCreateObject(Me.DotNetType.GetDotNetType()) Then
                        toReturn = ReflectionHelper.CreateObject(Me.DotNetType.GetDotNetType())
                    End If
                Case Flee.VariableMode.Constructor
                    Dim args As New List(Of Object)
                    For Each objParam As KeyValuePair(Of String, Object) In Me.Parameters.EvaluateVariables(owner, globalVars)
                        args.Add(objParam.Value)
                    Next
                    toReturn = System.Activator.CreateInstance(Me.DotNetType.GetDotNetType(), args.ToArray)
                Case Flee.VariableMode.Expression
                    If Me._AsCompiledExpression Then
                        toReturn = Me.FleeExpression.GetCompiledExpression(owner, globalVars)
                    Else
                        toReturn = Me.FleeExpression.Evaluate(owner, globalVars)
                    End If
                Case Flee.VariableMode.Delegate
                    Dim potentialsMembers As List(Of MemberInfo) = Nothing
                    Dim targetMethod As MethodInfo = Me.GetMethodInfo()
                    If targetMethod IsNot Nothing Then
                        If RequiresInstance Then
                            Dim target As Object = Me.TargetInstance.Evaluate(owner, globalVars)
                            toReturn = [Delegate].CreateDelegate(Me.DotNetType.GetDotNetType(), target, targetMethod)
                        Else
                            toReturn = [Delegate].CreateDelegate(Me.DotNetType.GetDotNetType(), targetMethod)
                        End If

                    Else
                        Throw New Exception(String.Format("Can't create Delegate for method {0} in type {1}", targetMethod.Name, Me.DotNetType.GetDotNetType().FullName))
                    End If
            End Select
            Return toReturn
        End Function


        Public Function GetMethodInfo() As MethodInfo
            Dim toReturn As MethodInfo = Nothing
            If Me.DotNetType.GetDotNetType() IsNot Nothing Then
                Dim potentialsMembers As List(Of MemberInfo) = Nothing
                If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType).TryGetValue(Me._MethodName, potentialsMembers) Then
                    Dim index As Integer = 0
                    For Each potentialMember As MemberInfo In potentialsMembers
                        If TypeOf potentialMember Is MethodInfo Then
                            index += 1
                            If index = MethodIndex Then
                                toReturn = DirectCast(potentialMember, MethodInfo)
                            End If
                        End If
                    Next
                End If
            End If
            Return toReturn
        End Function


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "MethodIndex"
                    Dim toReturn As New Dictionary(Of String, Integer)
                    Dim index As Integer = 1
                    For Each objMember As MemberInfo In Me.SelectedMembers
                        toReturn.Add(ReflectionHelper.GetMemberSignature(objMember), index)
                        index += 1
                    Next
                    If toReturn.Count = 0 Then
                        toReturn.Add("", 1)
                    End If
                    Return toReturn.ToList()
                Case "MethodName"
                    Return DirectCast(GetSelectorG(propertyName), IList)
            End Select
            Return New ArrayList
        End Function


        <XmlIgnore()> _
       <Browsable(False)> _
        Public ReadOnly Property SelectedMembers As List(Of MethodBase)
            Get

                Dim toReturn As New List(Of MethodBase)
                If Me.DotNetType.GetDotNetType() IsNot Nothing Then
                    Dim members As List(Of MemberInfo) = Nothing
                    If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType()).TryGetValue(Me.MethodName, members) Then
                        For Each member As MemberInfo In members
                            If TypeOf member Is MethodBase Then
                                toReturn.Add(DirectCast(member, MethodBase))
                            End If
                        Next
                    ElseIf Me.VariableMode = Flee.VariableMode.Constructor Then
                        toReturn.AddRange(Me.DotNetType.GetDotNetType.GetConstructors())
                    End If

                End If

                Return toReturn
            End Get
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property SelectedMember As MethodBase
            Get
                Dim tmpMembers As List(Of MethodBase) = SelectedMembers
                If tmpMembers.Count >= MethodIndex Then
                    Return SelectedMembers(MethodIndex - 1)
                End If
                Return Nothing
            End Get
        End Property

        Public Function GetSelectorG(propertyName As String) As IList(Of MethodInfo) Implements ISelector(Of MethodInfo).GetSelectorG
            Dim toReturn As New List(Of MethodInfo)
            For Each objMember As MemberInfo In ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType()).Values
                If TypeOf objMember Is MethodInfo Then
                    toReturn.Add(DirectCast(objMember, MethodInfo))
                End If
            Next
            Return toReturn
        End Function

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            If VariableMode = VariableMode.Expression AndAlso AdvancedExpression Then
                FleeExpression.AddVariables(currentProvider, existingVars)
            End If
        End Sub
    End Class
End Namespace