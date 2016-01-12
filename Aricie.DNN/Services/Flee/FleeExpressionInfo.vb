Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Reflection
Imports Ciloci.Flee
Imports Aricie.DNN.UI.WebControls
Imports System.Xml.Serialization

Namespace Services.Flee

   


    ''' <summary>
    ''' Flee expression, wrapper around SimpleExpressionInfo
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <ActionButton(IconName.Code, IconOptions.Normal)> _
    Public Class FleeExpressionInfo(Of TResult)
        Inherits SimpleExpression(Of TResult)
        Implements IExpressionVarsProvider

        Private _NewOwnerType As DotNetType



        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(expressionText As String)
            MyBase.New(expressionText)
        End Sub

        <ExtendedCategory("Expression")> _
        Public Overrides ReadOnly Property ReturnType As String
            Get
                Return MyBase.ReturnType
            End Get
        End Property

        <Required(True)> _
       <LineCount(5), Width(500)> _
       <ExtendedCategory("Expression")> _
        Public Overrides Property Expression As String
            Get
                Return MyBase.Expression
            End Get
            Set(value As String)
                MyBase.Expression = value
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Expression")> _
        Public Overrides Property ExpressionBuilder As FleeExpressionBuilder
            Get
                Return MyBase.ExpressionBuilder
            End Get
            Set(value As FleeExpressionBuilder)
                MyBase.ExpressionBuilder = value
            End Set
        End Property

        <ExtendedCategory("Expression")> _
        Public Overrides Sub DisplayAvailableVars(pe As AriciePropertyEditorControl)
            MyBase.DisplayAvailableVars(pe)
        End Sub

        <ExtendedCategory("Expression")> _
        Public Overrides Sub RemoveExpressionBuilder(pe As AriciePropertyEditorControl)
            MyBase.RemoveExpressionBuilder(pe)
        End Sub

        <ExtendedCategory("Expression")> _
        Public Overrides Sub InsertSelectedVar(pe As AriciePropertyEditorControl)
            MyBase.InsertSelectedVar(pe)
        End Sub


        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalVariables
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Variables")> _
        Public Property Variables() As Variables
            Get
                Return InternalVariables
            End Get
            Set(ByVal value As Variables)
                InternalVariables = value
            End Set
        End Property

        Private _ConstantIsSet As Boolean = False
        Private _Constant As TResult = Nothing

        <ExtendedCategory("Evaluation")> _
        Public Property MakeConstant As Boolean

        <ConditionalVisible("MakeConstant", True)> _
        <ExtendedCategory("Evaluation")> _
        Public Property NoCloning As Boolean
            Get
                Return InternalNoCloning
            End Get
            Set(value As Boolean)
                InternalNoCloning = value
            End Set
        End Property

        <ConditionalVisible("MakeConstant", True)> _
        <ConditionalVisible("NoCloning", True)> _
        <ExtendedCategory("Evaluation")> _
        Public Property KeepCloneExpression As Boolean
            Get
                Return InternalKeepCloneExpression
            End Get
            Set(value As Boolean)
                InternalKeepCloneExpression = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        <AutoPostBack()> _
        Public Property OverrideOwner As Boolean
            Get
                Return InternalOverrideOwner
            End Get
            Set(value As Boolean)
                If value <> InternalOverrideOwner Then
                    If value Then
                        InternalNewOwner = New FleeExpressionInfo(Of Object)
                    Else
                        InternalNewOwner = Nothing
                    End If
                End If
                InternalOverrideOwner = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        <ConditionalVisible("OverrideOwner", False, True)> _
        Public Property OwnerMemberAccess As BindingFlags
            Get
                Return InternalOwnerMemberAccess
            End Get
            Set(value As BindingFlags)
                If value <> BindingFlags.Default Then
                    InternalOwnerMemberAccess = value
                End If
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        <ConditionalVisible("OverrideOwner", False, True)> _
        Public Property NewOwner As FleeExpressionInfo(Of Object)
            Get
                Return InternalNewOwner
            End Get
            Set(value As FleeExpressionInfo(Of Object))
                InternalNewOwner = value
            End Set
        End Property

        <ExtendedCategory("Evaluation")> _
        <ConditionalVisible("OverrideOwner", False, True)> _
        Public Property NewOwnerType As DotNetType
            Get
                If OverrideOwner Then
                    If _NewOwnerType Is Nothing Then
                        _NewOwnerType = New DotNetType(GetType(Object))
                    End If
                    Return _NewOwnerType
                End If
                Return Nothing
            End Get
            Set(value As DotNetType)
                Me._NewOwnerType = value
            End Set
        End Property



        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalStaticImports
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("StaticImports")> _
            <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 5)> _
            <LabelMode(LabelMode.Top)> _
        Public Property StaticImports() As List(Of FleeImportInfo)
            Get
                Return InternalStaticImports
            End Get
            Set(ByVal value As List(Of FleeImportInfo))
                InternalStaticImports = value
            End Set
        End Property

        


        <ExtendedCategory("Debug")> _
        Public Property BreakOnException As Boolean
            Get
                Return InternalBreakOnException
            End Get
            Set(value As Boolean)
                InternalBreakOnException = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property BreakAtCompileTime As Boolean
            Get
                Return InternalBreakAtCompileTime
            End Get
            Set(value As Boolean)
                InternalBreakAtCompileTime = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property BreakAtEvaluateTime As Boolean
            Get
                Return InternalBreakAtEvaluateTime
            End Get
            Set(value As Boolean)
                InternalBreakAtEvaluateTime = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property LogCompileExceptions As Boolean
            Get
                Return InternalLogCompileExceptions
            End Get
            Set(value As Boolean)
                InternalLogCompileExceptions = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property ThrowCompileExceptions As Boolean
            Get
                Return InternalThrowCompileExceptions
            End Get
            Set(value As Boolean)
                InternalThrowCompileExceptions = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property LogEvaluateExceptions As Boolean
            Get
                Return InternalLogEvaluateExceptions
            End Get
            Set(value As Boolean)
                InternalLogEvaluateExceptions = value
            End Set
        End Property

        <ExtendedCategory("Debug")> _
        Public Property ThrowEvaluateExceptions As Boolean
            Get
                Return InternalThrowEvaluateExceptions
            End Get
            Set(value As Boolean)
                InternalThrowEvaluateExceptions = value
            End Set
        End Property




        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalImportBuiltTypes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("StaticImports")> _
        Public Property ImportBuiltinTypes() As Boolean
            Get
                Return InternalImportBuiltinTypes
            End Get
            Set(ByVal value As Boolean)
                InternalImportBuiltinTypes = value
            End Set
        End Property




        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalDateTimeFormat
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        '    <Required(True)> _
        'Public Property DateTimeFormat() As String
        '    Get
        '        Return InternalDateTimeFormat
        '    End Get
        '    Set(ByVal value As String)
        '        InternalDateTimeFormat = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalRequireDigitsBeforeDecimalPoint
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        'Public Property RequireDigitsBeforeDecimalPoint() As Boolean
        '    Get
        '        Return InternalRequireDigitsBeforeDecimalPoint
        '    End Get
        '    Set(ByVal value As Boolean)
        '        InternalRequireDigitsBeforeDecimalPoint = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalDecimalSeparator
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        '    <Required(True)> _
        'Public Property DecimalSeparator() As Char
        '    Get
        '        Return InternalDecimalSeparator
        '    End Get
        '    Set(ByVal value As Char)
        '        InternalDecimalSeparator = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalFunctionArgumentSeparator
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        '    <Required(True)> _
        'Public Property FunctionArgumentSeparator() As Char
        '    Get
        '        Return InternalFunctionArgumentSeparator
        '    End Get
        '    Set(ByVal value As Char)
        '        InternalFunctionArgumentSeparator = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalParseCultureMode
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        'Public Property ParseCultureMode() As CultureInfoMode
        '    Get
        '        Return InternalParseCultureMode
        '    End Get
        '    Set(ByVal value As CultureInfoMode)
        '        InternalParseCultureMode = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalCustomCultureLocale
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        '<ConditionalVisible("ParseCultureMode", False, True, CultureInfoMode.Custom)> _
        'Public Property CustomCultureLocale() As String
        '    Get
        '        Return InternalCustomCultureLocale
        '    End Get
        '    Set(ByVal value As String)
        '        InternalCustomCultureLocale = value
        '    End Set
        'End Property

        ' ''' <summary>
        ' ''' gets or sets SimpleExpressionInfo.InternalRealLiteralDataType
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        '<ExtendedCategory("TechnicalSettings")> _
        'Public Property RealLiteralDataType() As RealLiteralDataType
        '    Get
        '        Return InternalRealLiteralDataType
        '    End Get
        '    Set(ByVal value As RealLiteralDataType)
        '        InternalRealLiteralDataType = value
        '    End Set
        'End Property

        Public Overrides Function Evaluate(owner As Object, globalVars As IContextLookup, objType As Type) As TResult
            If Me.MakeConstant Then
                If Not _ConstantIsSet Then
                    _Constant = MyBase.Evaluate(owner, globalVars, objType)
                    _ConstantIsSet = True
                End If
                Return _Constant
            End If
            Return MyBase.Evaluate(owner, globalVars, objType)
        End Function

        Public Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type)) Implements IExpressionVarsProvider.AddVariables
            Me.Variables.AddVariables(currentProvider, existingVars)
            If Me.OverrideOwner Then
                existingVars(FleeExpressionBuilder.ExpressionOwnerVar) = Me.NewOwnerType.GetDotNetType()
            End If
        End Sub


    End Class
End Namespace