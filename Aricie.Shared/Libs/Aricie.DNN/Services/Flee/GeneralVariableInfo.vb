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
Imports System.Text
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Services.Localization
Imports System.Linq
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Files

Namespace Services.Flee

     Public Class AnonymousGeneralVariableInfo
        Inherits GeneralVariableInfo

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property

    End Class

    Public Class AnonymousGeneralVariableInfo(Of T)
        Inherits GeneralVariableInfo(Of T)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property

    End Class

    Public Class GeneralVariableInfo(Of T)
        Inherits GeneralVariableInfo

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(strName As String)
            MyBase.New(strName)
        End Sub

        Private _ConstDotNetType As New DotNetType(GetType(T))

        <SortOrder(0)> _
        Public Overrides ReadOnly Property VariableType As String
            Get
                Return MyBase.VariableType
            End Get
        End Property


        <SortOrder(0)> _
        Public Property SubType() As New EnabledFeature(Of SubDotNetType(Of T))


        <Browsable(False)>
        Public Overrides Property DotNetType As DotNetType
            Get
                If SubType.Enabled
                    Return SubType.Entity
                End If
                Return _ConstDotNetType
            End Get
            Set(value As DotNetType)
                'do nothing
            End Set
        End Property

        Public  Function EvaluateTyped(ByVal owner As Object, ByVal globalVars As IContextLookup) As T
            Return DirectCast(Me.Evaluate(owner, globalVars), T)
        End Function


    End Class

    Public Class GeneralVariableInfo
        Inherits VariableInfo
        Implements ISelector(Of MemberInfo)
        Implements IExpressionVarsProvider


        Public Overrides Function GetFriendlyDetails() As String
            Dim toReturn As String = MyBase.GetFriendlyDetails()
            Dim typeName As String = ReflectionHelper.GetSimpleTypeName(Me.DotNetType.GetDotNetType())
            Dim nextSegment As String = ""
            Select Case Me.VariableMode
                Case VariableMode.Constructor
                    nextSegment = "cTor " & Me.MethodName
                Case VariableMode.Delegate
                    nextSegment = "Delegate: " & Me.TargetInstance.Expression
                Case VariableMode.Expression
                    nextSegment = "Expression: " & Me.SimpleExpression.Expression
                Case VariableMode.Instance
                    nextSegment = "Instance: " & ReflectionHelper.GetFriendlyName(Me.Instance)
            End Select
            toReturn = String.Format("{0}{1}{2}{1}{3}", toReturn, UIConstants.TITLE_SEPERATOR, typeName, nextSegment)
            Return toReturn
        End Function


        Private _Instance As Object
        Private _SimpleExpression As SimpleExpression(Of Object)
        Private _FleeExpression As New FleeExpressionInfo(Of Object)()



        Public Sub New()

        End Sub

        Public Sub New(strName As String)
            Me.Name = strName
        End Sub


        Public Overridable Property DotNetType As New DotNetType()

        <Browsable(False)>
        Public ReadOnly Property HasType As Boolean
            Get
                Return DotNetType.GetDotNetType() IsNot Nothing
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property HasTargetType As Boolean
            Get
                If DotNetType.GetDotNetType() IsNot Nothing Then
                    Return (Not VariableMode = VariableMode.StaticMember) OrElse StaticMemberType.GetDotNetType() IsNot Nothing
                End If
                Return False
            End Get
        End Property

        <Browsable(False)>
        Public Overrides ReadOnly Property VariableType As String
            Get
                Return DotNetType.TypeName
            End Get
        End Property

        <ConditionalVisible("HasType", False, True)>
        Public Property VariableMode As VariableMode = VariableMode.Expression


        <ConditionalVisible("VariableMode", False, True, VariableMode.SmartFile)>
        Public Property SmartFileKey() As New EntityKeyInfo()

        <ConditionalVisible("VariableMode", False, True, VariableMode.SmartFile)>
        Public Property SetForSave() As Boolean = True

        <ConditionalVisible("VariableMode", False, True, VariableMode.SmartFile)>
        Public Property UseCustomFileSettings() As New EnabledFeature(Of SmartFileInfo)

        'Static Member

        <ConditionalVisible("HasType")>
        <ConditionalVisible("VariableMode", False, True, VariableMode.StaticMember)>
        Public Overridable Property StaticMemberType As New DotNetType()



        ''' <summary>
        ''' Get or sets the instance of the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Instance)>
        <XmlIgnore()>
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

        'Instance

        <ExtendedCategory("", "Evaluation")>
        <ConditionalVisible("HasTargetType", False, True)>
        <ConditionalVisible("VariableMode", True, True, VariableMode.Constructor, VariableMode.Delegate)>
        Public Property UseClone() As Boolean

        <ExtendedCategory("", "Evaluation")>
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)>
        <ConditionalVisible("HasType", False, True)>
        Public Property InstanceMode As InstanceMode = InstanceMode.Off

        <ExtendedCategory("", "Evaluation")>
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)>
        <ConditionalVisible("HasType", False, True)>
        Public Overrides Property EvaluationMode() As VarEvaluationMode
            Get
                Return MyBase.EvaluationMode
            End Get
            Set(ByVal value As VarEvaluationMode)
                MyBase.EvaluationMode = value
            End Set
        End Property

        <ExtendedCategory("", "Evaluation")>
        <ConditionalVisible("VariableMode", True, True, VariableMode.Instance)>
        <ConditionalVisible("HasType", False, True)>
        Public Overrides Property Scope() As VariableScope
            Get
                Return MyBase.Scope
            End Get
            Set(ByVal value As VariableScope)
                MyBase.Scope = value
            End Set
        End Property

        <Browsable(False)>
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

        'Expression


        ''' <summary>
        ''' Is the variable and advanced expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)>
        Public Property AdvancedExpression() As Boolean

        ''' <summary>
        ''' Gets the flee expression that will be used
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)>
        <ConditionalVisible("AdvancedExpression", False, True)>
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
        <XmlIgnore()>
        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)>
        <ConditionalVisible("AdvancedExpression", True, True)>
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
        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Expression)>
        Public Property AsCompiledExpression() As Boolean





        'Delegate
        'todo: should be member
        <AutoPostBack()>
        <ConditionalVisible("HasTargetType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Delegate, VariableMode.StaticMember)>
        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        <Selector("Name", "Name", False, True, "<Select a Member Name>", "", False, True)>
        Public Property MethodName As String = ""

        <Browsable(False)> _
        Public ReadOnly Property HasSeveralCandidates() As Boolean
            Get
                'Dim candidates As IList = GetSelector("MethodIndex")
                Return GetSelector("MethodIndex").Count > 0
            End Get
        End Property

        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        <ProvidersSelector("Key", "Value")>
        <ConditionalVisible("HasTargetType")>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Constructor, VariableMode.Delegate, VariableMode.StaticMember)>
        <ConditionalVisible("HasSeveralCandidates")>
        Public Property MethodIndex As Integer = 1

        <Browsable(False)>
        Public ReadOnly Property RequiresInstance As Boolean
            Get
                'Dim targetMethod As MethodInfo = Me.GetMethodInfo()
                'If targetMethod IsNot Nothing Then
                '    Return targetMethod.IsStatic
                'End If
                If Me.SelectedMember IsNot Nothing Then
                    Return Not ReflectionHelper.IsStatic(Me.SelectedMember)
                End If

                Return False
            End Get
        End Property

        <ConditionalVisible("HasTargetType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Delegate)>
        <ConditionalVisible("RequiresInstance", False, True)>
        Public Property TargetInstance As New SimpleExpression(Of Object)

        <Browsable(False)>
        Public ReadOnly Property HasParameters() As Boolean
            Get
                Select Case Me.VariableMode
                    Case VariableMode.Constructor, VariableMode.StaticMember
                        If SelectedMember IsNot Nothing Then
                            Dim objParameters As ParameterInfo()
                            If TypeOf SelectedMember Is MethodBase Then
                                objParameters = DirectCast(SelectedMember, MethodBase).GetParameters()
                            ElseIf TypeOf SelectedMember Is PropertyInfo Then
                                objParameters = DirectCast(SelectedMember, PropertyInfo).GetIndexParameters()
                            Else
                                Throw New ApplicationException("Only ctors, Methods and Property parameters supported")
                            End If
                            Return objParameters.Length > 0
                        End If
                        Return False
                    Case Else
                        Return False
                End Select
            End Get
        End Property


        <ConditionalVisible("HasTargetType", False, True)>
        <ConditionalVisible("HasParameters")>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Constructor, VariableMode.StaticMember)>
        Public Property Parameters() As New Variables



        <ConditionalVisible("HasType", False, True)>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Instance)>
        <ActionButton(IconName.Refresh, IconOptions.Normal)>
        Public Sub ResetInstance(ape As AriciePropertyEditorControl)
            Me._Instance = Nothing
            ape.ItemChanged = True
            Dim message As String = Localization.GetString("InstanceReset.Message", ape.LocalResourceFile)
            ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess)
        End Sub

        <ConditionalVisible("HasTargetType", False, True)>
        <ConditionalVisible("HasParameters")>
        <ConditionalVisible("VariableMode", False, True, VariableMode.Constructor, VariableMode.StaticMember)>
        <ActionButton(IconName.Key, IconOptions.Normal)>
        Public Sub SetParameters(ape As AriciePropertyEditorControl)
            If Me.VariableMode = VariableMode.Constructor OrElse VariableMode = VariableMode.StaticMember Then
                Dim objParameters As ParameterInfo()
                If TypeOf SelectedMember Is MethodBase Then
                    objParameters = DirectCast(SelectedMember, MethodBase).GetParameters()
                ElseIf TypeOf SelectedMember Is PropertyInfo Then
                    objParameters = DirectCast(SelectedMember, PropertyInfo).GetIndexParameters()
                Else
                    Throw New ApplicationException("Only ctors, Methods and Property parameters supported")
                End If

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
            'If Me.UseSmartFile Then
            '  toReturn=  Aricie.DNN.Services.Files.SmartFile.LoadSmartFile(Me.SmartFileKey.Evaluate(owner, globalVars), TryCast(globalVars.Items("SmartFileInfo"), SmartFileInfo))
            'End If
            If Me._Instance IsNot Nothing Then
                If Me.VariableMode = Flee.VariableMode.Instance OrElse Me._InstanceMode <> Flee.InstanceMode.Off Then
                    toReturn = Me._Instance
                    If _UseClone Then
                        toReturn = ReflectionHelper.CloneObject(toReturn)
                    End If
                    If Scope = VariableScope.Global AndAlso globalVars IsNot Nothing Then
                        globalVars.Items(Me.Name) = toReturn
                    End If

                    Return toReturn
                Else
                    Me._Instance = Nothing
                End If
            End If
            toReturn = MyBase.Evaluate(owner, globalVars)
            If _UseClone Then
                toReturn = ReflectionHelper.CloneObject(toReturn)
            End If
            If Me._InstanceMode = InstanceMode.InContextEval Then
                Me._Instance = toReturn
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
                    Dim args As Object()
                    args = (From objParam In Me.Parameters.EvaluateVariables(owner, globalVars) Select objParam.Value).ToArray()
                    Try
                        toReturn = System.Activator.CreateInstance(Me.DotNetType.GetDotNetType(), args)
                    Catch ex As Exception
                        Dim newEx As ApplicationException
                        If args IsNot Nothing Then
                            Dim argsList As New StringBuilder()
                            For Each obj As Object In args
                                If obj Is Nothing Then
                                    argsList.Append("Null")
                                Else
                                    argsList.Append(ReflectionHelper.GetFriendlyName(obj))
                                End If
                                argsList.Append(", ")
                            Next
                            Dim strArgs As String = argsList.ToString()
                            strArgs = strArgs.Substring(0, strArgs.Length - 2)
                            newEx = New ApplicationException(String.Format("Error calling constructor for type {0} with args {1}", Me.DotNetType.GetDotNetType, strArgs), ex)
                        Else
                            newEx = New ApplicationException(String.Format("Error calling constructor for type {0} ", Me.DotNetType.GetDotNetType), ex)
                        End If

                        Throw newEx
                    End Try
                Case Flee.VariableMode.Expression
                    If Me._AsCompiledExpression Then
                        toReturn = Me.FleeExpression.GetCompiledExpression(owner, globalVars, Me.DotNetType.GetDotNetType())
                    Else
                        toReturn = Me.FleeExpression.Evaluate(owner, globalVars, Me.DotNetType.GetDotNetType())
                    End If
                Case Flee.VariableMode.Delegate
                    Dim potentialsMembers As List(Of MemberInfo) = Nothing
                    Dim targetMethod As MethodInfo = DirectCast(Me.SelectedMember, MethodInfo)
                    If targetMethod IsNot Nothing Then
                        If RequiresInstance Then
                            Dim target As Object = Me.TargetInstance.Evaluate(owner, globalVars, Me.DotNetType.GetDotNetType())
                            toReturn = [Delegate].CreateDelegate(Me.DotNetType.GetDotNetType(), target, targetMethod)
                        Else
                            toReturn = [Delegate].CreateDelegate(Me.DotNetType.GetDotNetType(), targetMethod)
                        End If
                    Else
                        Throw New Exception(String.Format("Can't create Delegate for method {0} in type {1}", targetMethod.Name, Me.DotNetType.GetDotNetType().FullName))
                    End If
                Case Flee.VariableMode.StaticMember
                    Dim targetMember As MemberInfo = Me.SelectedMember
                    If targetMember IsNot Nothing Then
                        If TypeOf targetMember Is FieldInfo Then
                            toReturn = DirectCast(targetMember, FieldInfo).GetValue(Nothing)
                        Else
                            Dim args As Object()
                            args = (From objParam In Me.Parameters.EvaluateVariables(owner, globalVars) Select objParam.Value).ToArray()
                            If TypeOf targetMember Is PropertyInfo Then
                                toReturn = DirectCast(targetMember, PropertyInfo).GetValue(Nothing, args)
                            ElseIf TypeOf targetMember Is MethodBase Then
                                toReturn = DirectCast(targetMember, MethodBase).Invoke(Nothing, args)
                            End If
                        End If
                    End If
                Case VariableMode.SmartFile
                    Dim smartKey As EntityKey = Me.SmartFileKey.Evaluate(owner, globalVars)
                    If UseCustomFileSettings.Enabled Then
                        toReturn = Aricie.DNN.Services.Files.SmartFile.LoadSmartFile(smartKey, globalVars, Me.SetForSave, UseCustomFileSettings.Entity)
                    Else
                        toReturn =  Aricie.DNN.Services.Files.SmartFile.LoadSmartFile(smartKey, globalVars, Me.SetForSave)
                    End If
            End Select
            Return toReturn
        End Function


        'Public Function GetMethodInfo() As MethodInfo
        '    Dim toReturn As MethodInfo = Nothing
        '    If Me.DotNetType.GetDotNetType() IsNot Nothing Then
        '        Dim potentialsMembers As List(Of MemberInfo) = Nothing
        '        If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType).TryGetValue(Me._MethodName, potentialsMembers) Then
        '            Dim index As Integer = 0
        '            For Each potentialMember As MemberInfo In potentialsMembers
        '                If TypeOf potentialMember Is MethodInfo Then
        '                    index += 1
        '                    If index = MethodIndex Then
        '                        toReturn = DirectCast(potentialMember, MethodInfo)
        '                    End If
        '                End If
        '            Next
        '        End If
        '    End If
        '    Return toReturn
        'End Function


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "MethodIndex"
                    Dim toReturn As New Dictionary(Of String, Integer)
                    Dim index As Integer = 1
                    For Each objMember As MemberInfo In Me.SelectedMembers
                        toReturn.Add(ReflectionHelper.GetMemberSignature(objMember), index)
                        index += 1
                    Next
                    Return toReturn.ToList()
                Case "MethodName"
                    Return DirectCast(GetSelectorG(propertyName), IList)
            End Select
            Return New ArrayList
        End Function


        <XmlIgnore()>
        <Browsable(False)>
        Public ReadOnly Property SelectedMembers As List(Of MemberInfo)
            Get

                Dim toReturn As New List(Of MemberInfo)
                If Me.DotNetType.GetDotNetType() IsNot Nothing Then
                    Dim members As List(Of MemberInfo) = Nothing
                    Select Case Me.VariableMode
                        Case VariableMode.Constructor
                            toReturn.AddRange(Me.DotNetType.GetDotNetType.GetConstructors())
                        Case VariableMode.StaticMember
                            If ReflectionHelper.GetFullMembersDictionary(Me.StaticMemberType.GetDotNetType(), True, False).TryGetValue(Me.MethodName, members) Then
                                toReturn.AddRange(members)
                            End If
                        Case Else
                            If ReflectionHelper.GetFullMembersDictionary(Me.DotNetType.GetDotNetType(), True, False).TryGetValue(Me.MethodName, members) Then
                                toReturn.AddRange(members)
                            End If

                    End Select

                End If
                Return toReturn
            End Get
        End Property

        <XmlIgnore()>
        <Browsable(False)>
        Public ReadOnly Property SelectedMember As MemberInfo
            Get
                Dim tmpMembers As List(Of MemberInfo) = SelectedMembers
                If tmpMembers.Count >= MethodIndex Then
                    Return SelectedMembers(MethodIndex - 1)
                End If
                Return Nothing
            End Get
        End Property

        Public Function GetSelectorG(propertyName As String) As IList(Of MemberInfo) Implements ISelector(Of MemberInfo).GetSelectorG
            Select Case Me.VariableMode
                Case VariableMode.StaticMember
                    Return ReflectionHelper.GetMembersDictionary( _
                        StaticMemberType.GetDotNetType()).Values _
                        .Where(Function(objMember) ReflectionHelper.IsStatic(objMember) _
                             AndAlso DotNetType.GetDotNetType().IsAssignableFrom(ReflectionHelper.GetMemberReturnType(objMember))) _
                        .ToList()
                Case VariableMode.Delegate
                    Return ReflectionHelper.GetMembersDictionary(Me.DotNetType.GetDotNetType()).Values.Where(Function(objMember) TypeOf objMember Is MethodInfo).ToList()
            End Select
            Return New List(Of MemberInfo)
        End Function

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            If VariableMode = VariableMode.Expression AndAlso AdvancedExpression Then
                FleeExpression.AddVariables(currentProvider, existingVars)
            End If
        End Sub
    End Class
End Namespace