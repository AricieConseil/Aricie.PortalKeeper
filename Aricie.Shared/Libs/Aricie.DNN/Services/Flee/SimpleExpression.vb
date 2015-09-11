Imports Aricie.DNN.UI.Attributes
Imports Ciloci.Flee
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports System.Globalization
Imports Aricie.Services
Imports System.Threading
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls

Namespace Services.Flee
    ''' <summary>
    ''' Simple flee expression
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <ActionButton(IconName.Code, IconOptions.Normal)> _
    <DefaultProperty("Expression")> _
    <Serializable()> _
    Public Class SimpleExpression(Of TResult)




        Friend Const DefaultDateTimeFormat As String = "dd/MM/yyyy"
        Friend Const DefaultRequireDigitsBeforeDecimalPoint As Boolean = False
        Friend Const DefaultDecimalSeparator As Char = "."c
        Friend Const DefaultFunctionArgumentSeparator As Char = ","c
        Friend Const DefaultParseCultureMode As CultureInfoMode = CultureInfoMode.Invariant
        Friend Const DefaultCustomCultureLocale As String = "en-US"
        Friend Const DefaultRealLiteralDataType As RealLiteralDataType = RealLiteralDataType.Decimal
        Friend Const DefaultOwnerMemberAccess As Reflection.BindingFlags = Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance _
                                                                         Or Reflection.BindingFlags.Static Or Reflection.BindingFlags.IgnoreCase


        Private _Expression As String = ""

        Protected InternalStaticImports As New List(Of FleeImportInfo)
        Protected InternalImportBuiltinTypes As Boolean = True

        Protected InternalNoCloning As Boolean
        Protected InternalKeepCloneExpression As Boolean
        Protected InternalOverrideOwner As Boolean
        Protected InternalNewOwner As FleeExpressionInfo(Of Object)


        Protected InternalVariables As New Variables


        Protected InternalDateTimeFormat As String = DefaultDateTimeFormat
        Protected InternalRequireDigitsBeforeDecimalPoint As Boolean = DefaultRequireDigitsBeforeDecimalPoint
        Protected InternalDecimalSeparator As Char = DefaultDecimalSeparator
        Protected InternalFunctionArgumentSeparator As Char = DefaultFunctionArgumentSeparator
        Protected InternalParseCultureMode As CultureInfoMode = DefaultParseCultureMode
        Protected InternalCustomCultureLocale As String = DefaultCustomCultureLocale
        Protected InternalRealLiteralDataType As RealLiteralDataType = DefaultRealLiteralDataType
        Protected InternalOwnerMemberAccess As Reflection.BindingFlags = DefaultOwnerMemberAccess
        Protected InternalBreakOnException As Boolean
        Protected InternalBreakAtCompileTime As Boolean
        Protected InternalBreakAtEvaluateTime As Boolean
        Protected InternalLogCompileExceptions As Boolean = True
        Protected InternalLogEvaluateExceptions As Boolean
        Protected InternalThrowCompileExceptions As Boolean = True
        Protected InternalThrowEvaluateExceptions As Boolean = True

        'todo: move to individual compiled expressions when needed (possible ambiguity now)
        Private Shared ReadOnly _CompiledExpressions As New Dictionary(Of String, IGenericExpression(Of TResult))
        'Private ReadOnly _CompiledExpressions As IGenericExpression(Of TResult)

        Public Sub New()
        End Sub

        Public Sub New(ByVal expression As String)
            Me._Expression = expression
        End Sub

        Public Sub New(slaveExpression As FleeExpressionInfo(Of TResult))
            Me.New(slaveExpression.Expression)
            Me.SlaveExpression = slaveExpression
        End Sub

        ''' <summary>
        ''' Gets the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ReturnType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(TResult))
            End Get
        End Property


        ''' <summary>
        ''' Gets or sets the Flee expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False), XmlIgnore()>
        Public Property SlaveExpression As SimpleExpression(Of TResult)

        ''' <summary>
        ''' Gets or sets the expression as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Required(True)> _
        <LineCount(4), Width(500)> _
        Public Overridable Property Expression() As String
            Get
                Return _Expression
            End Get
            Set(ByVal value As String)
                If value IsNot Nothing AndAlso value <> _Expression Then
                    SyncLock Me
                        _Expression = value
                        If SlaveExpression IsNot Nothing Then
                            SlaveExpression.Expression = value
                        End If
                    End SyncLock
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property HasExpressionBuilder As Boolean
            Get
                Return ExpressionBuilder IsNot Nothing
            End Get
        End Property

        <XmlIgnore()> _
        Public Overridable Property ExpressionBuilder As FleeExpressionBuilder

        <ConditionalVisible("HasExpressionBuilder", True, True)> _
        <ActionButton(IconName.Magic, IconOptions.Normal)> _
        Public Overridable Sub DisplayAvailableVars(ByVal pe As AriciePropertyEditorControl)

            Dim fullAccess As Boolean = Me.InternalOverrideOwner AndAlso ((Me.InternalOwnerMemberAccess And Reflection.BindingFlags.NonPublic) = Reflection.BindingFlags.NonPublic)

            Me.ExpressionBuilder = FleeExpressionBuilder.GetExpressionBuilder(pe, fullAccess)
            pe.DisplayLocalizedMessage("ExpressionHelper.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            'pe.DisplayMessage(Me.ExpressionBuilder.GetType.AssemblyQualifiedName, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
        End Sub

        <ConditionalVisible("HasExpressionBuilder", False, True)> _
        <ActionButton(IconName.Undo, IconOptions.Normal)> _
        Public Overridable Sub RemoveExpressionBuilder(ByVal pe As AriciePropertyEditorControl)

            Me.ExpressionBuilder = Nothing
            pe.ItemChanged = True
        End Sub


        <ConditionalVisible("HasExpressionBuilder", False, True)> _
          <ActionButton(IconName.Clipboard, IconOptions.Normal)> _
        Public Overridable Sub InsertSelectedVar(ByVal pe As AriciePropertyEditorControl)
            Me.Expression &= Me.ExpressionBuilder.InsertString
            Me.ExpressionBuilder = Nothing
            pe.ItemChanged = True
        End Sub


        Public Overloads Function Evaluate(ByVal globalVars As IContextLookup) As TResult
            Return Me.Evaluate(globalVars, globalVars, Nothing)
        End Function


        ''' <summary>
        ''' Evaluates the flee expression
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As TResult
            Return Evaluate(owner, globalVars, Nothing)
        End Function


        ''' <summary>
        ''' Evaluates the flee expression
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Overloads Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup, objType As Type) As TResult

            Dim toReturn As TResult
            If Me._Expression <> "" Then

                Dim clone As IGenericExpression(Of TResult) = Me.GetCompiledExpression(owner, globalVars, objType)
                If clone IsNot Nothing Then
                    Using onDemand As New OnDemandVariableLookup(globalVars)
                        AddHandler clone.Context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                        AddHandler clone.Context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        Try
                            If Me.InternalBreakAtEvaluateTime Then
                                Common.CallDebuggerBreak()
                            End If
                            toReturn = clone.Evaluate()
                        Catch ex As Exception
                            If Me.InternalBreakOnException Then
                                Common.CallDebuggerBreak()
                            End If

                            Dim objFLeeException As New HttpException(String.Format("Flee Expression ""{0}"" failed to run with the inner exception", Me.Expression), ex)

                            If Me.InternalLogEvaluateExceptions Then
                                ExceptionHelper.LogException(objFLeeException)
                            End If
                            If Me.InternalThrowEvaluateExceptions Then
                                Throw objFLeeException
                            End If

                        Finally
                            RemoveHandler clone.Context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                            RemoveHandler clone.Context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        End Try
                    End Using
                End If
            End If
            Return toReturn

        End Function


        ''' <summary>
        ''' Returns the compiled flee expression
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Public Function GetCompiledExpression(ByVal owner As Object, ByVal globalVars As IContextLookup, objType As Type) As IGenericExpression(Of TResult)
            Dim toReturn As IGenericExpression(Of TResult) = Nothing
            If objType Is Nothing Then
                objType = GetType(TResult)
            End If
            Dim key As String = Me._Expression & objType.AssemblyQualifiedName
            If Not _CompiledExpressions.TryGetValue(key, toReturn) Then
                SyncLock Me
                    'SyncLock expWriterLock
                    If Not _CompiledExpressions.TryGetValue(key, toReturn) Then

                        If Me.InternalBreakAtCompileTime Then
                            Common.CallDebuggerBreak()
                        End If

                        Dim context As ExpressionContext = Me.GetExpressionContext(owner, globalVars)
                        Dim onDemand As New OnDemandVariableLookup(globalVars)
                        AddHandler context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                        AddHandler context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        Try
                            toReturn = context.CompileGeneric(Of TResult)(Me._Expression)
                            SyncLock _CompiledExpressions
                                _CompiledExpressions(key) = toReturn
                            End SyncLock
                        Catch ex As Ciloci.Flee.ExpressionCompileException
                            If Me.InternalBreakOnException Then
                                Common.CallDebuggerBreak()
                            End If
                            Dim objFLeeException As New HttpException(String.Format("Flee Expression ""{0}"" failed to compile with the inner exception", Me.Expression), ex)
                            If Me.InternalLogCompileExceptions Then
                                ExceptionHelper.LogException(objFLeeException)
                            End If
                            If Me.InternalThrowCompileExceptions Then
                                Throw objFLeeException
                            End If
                            toReturn = Nothing
                        Finally
                            RemoveHandler context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                            RemoveHandler context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        End Try
                    End If
                    'End SyncLock
                End SyncLock
            End If
            If toReturn IsNot Nothing Then
                If InternalKeepCloneExpression Then
                    Dim cloneExp As Object = Nothing
                    globalVars.Items.TryGetValue("CloneExpression:" & Expression, cloneExp)
                    If cloneExp IsNot Nothing Then
                        toReturn = DirectCast(cloneExp, IGenericExpression(Of TResult))
                    End If
                End If
                If (Not InternalNoCloning) AndAlso owner IsNot Nothing AndAlso toReturn.Owner IsNot owner Then
                    toReturn = DirectCast(toReturn.Clone, IGenericExpression(Of TResult))
                    If Not _OwnerProviderSuplied Then
                        If Not InternalOverrideOwner Then
                            toReturn.Owner = owner
                        Else
                            Dim newOwner As Object = Me.InternalNewOwner.Evaluate(owner, globalVars)
                            toReturn.Owner = newOwner
                        End If
                    Else
                        toReturn.Owner = DirectCast(owner, IContextOwnerProvider).ContextOwner
                    End If
                    If InternalKeepCloneExpression Then
                        globalVars.Items("CloneExpression:" & Expression) = toReturn
                    End If
                End If
                If Me.InternalVariables.Instances.Count > 0 Then
                    Dim vars As Dictionary(Of String, Object) = Nothing
                    For Each objVar As VariableInfo In Me.InternalVariables.Instances
                        If objVar.EvaluationMode = VarEvaluationMode.Dynamic Then
                            If vars Is Nothing Then
                                vars = Me.InternalVariables.EvaluateVariables(owner, globalVars)
                            End If
                            toReturn.Context.Variables(objVar.Name) = vars(objVar.Name)
                        End If
                    Next
                End If
            End If

            Return toReturn
        End Function

        Private _OwnerProviderSuplied As Boolean

        ''' <summary>
        ''' Returns the expression context
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Private Function GetExpressionContext(ByVal owner As Object, ByVal globalVars As IContextLookup) As ExpressionContext

            Dim toReturn As ExpressionContext
            If owner IsNot Nothing Then
                If TypeOf owner Is IContextOwnerProvider Then
                    Dim ownerProvider As IContextOwnerProvider = DirectCast(owner, IContextOwnerProvider)
                    If ownerProvider.ContextOwner IsNot Nothing Then
                        toReturn = New ExpressionContext(ownerProvider.ContextOwner)
                        _OwnerProviderSuplied = True
                    Else
                        If InternalOverrideOwner Then
                            Dim newOwner As Object = Me.InternalNewOwner.Evaluate(owner, globalVars)
                            toReturn = New ExpressionContext(newOwner)
                        Else
                            toReturn = New ExpressionContext(owner)
                        End If

                    End If
                Else
                    If InternalOverrideOwner Then
                        Dim newOwner As Object = Me.InternalNewOwner.Evaluate(owner, globalVars)
                        toReturn = New ExpressionContext(newOwner)
                    End If
                    toReturn = New ExpressionContext(owner)
                End If
            Else
                toReturn = New ExpressionContext()
            End If


            toReturn.ParserOptions.DateTimeFormat = Me.InternalDateTimeFormat
            toReturn.ParserOptions.RequireDigitsBeforeDecimalPoint = Me.InternalRequireDigitsBeforeDecimalPoint
            toReturn.ParserOptions.DecimalSeparator = Me.InternalDecimalSeparator
            toReturn.ParserOptions.FunctionArgumentSeparator = Me.InternalFunctionArgumentSeparator
            toReturn.ParserOptions.RecreateParser()
            Select Case Me.InternalParseCultureMode
                Case CultureInfoMode.Invariant
                    toReturn.Options.ParseCulture = CultureInfo.InvariantCulture
                Case CultureInfoMode.Current
                    toReturn.Options.ParseCulture = DirectCast(Thread.CurrentThread.CurrentCulture.Clone, CultureInfo)
                Case CultureInfoMode.CurrentUI
                    toReturn.Options.ParseCulture = DirectCast(Thread.CurrentThread.CurrentUICulture.Clone, CultureInfo)
                Case CultureInfoMode.Custom
                    toReturn.Options.ParseCulture = New CultureInfo(Me.InternalCustomCultureLocale)
            End Select
            toReturn.Options.RealLiteralDataType = Me.InternalRealLiteralDataType
            toReturn.Options.OwnerMemberAccess = Me.InternalOwnerMemberAccess
            'If Me.InternalStaticImports.Count = 0 Then
            '    tempContext.Imports.AddType(GetType(System.Math), "")
            'Else
            If Me.InternalImportBuiltinTypes Then
                toReturn.Imports.ImportBuiltinTypes()
                toReturn.Imports.AddType(GetType(System.Math), "")
                toReturn.Imports.AddType(GetType(System.Linq.Enumerable), "Enumerable")
                toReturn.Imports.AddType(GetType(DateTime), "DateTime")
                toReturn.Imports.AddType(GetType(HttpUtility), "HttpUtility")
                toReturn.Imports.AddType(GetType(Common), "Common")
                'toReturn.Imports.AddType(GetType(ReflectionHelper), "ReflectionHelper")
                'toReturn.Imports.AddType(GetType(System.Linq.Expressions.Expression), "Expression")
            End If
            For Each staticImport As FleeImportInfo In Me.InternalStaticImports
                toReturn.Imports.AddType(staticImport.DotNetType.GetDotNetType, staticImport.CustomNamespace)
            Next
            'End If

            toReturn.Imports.AddType(GetType(FleeHelper), "")

            Dim vars As Dictionary(Of String, Object) = Me.InternalVariables.EvaluateVariables(owner, globalVars)
            For Each objVar As KeyValuePair(Of String, Object) In vars
                If (objVar.Value IsNot Nothing) Then
                    toReturn.Variables.Add(objVar.Key, objVar.Value)
                Else
                    Throw New ApplicationException(String.Format("The var {0} value should not be null", objVar.Key))
                End If
            Next

            Return toReturn
        End Function

        ''' <summary>
        ''' Lazy variable evaluation class
        ''' </summary>
        ''' <remarks></remarks>
        Public Class OnDemandVariableLookup
            Implements IDisposable


            Public Sub New(ByVal context As IContextLookup)
                Me._Context = context
            End Sub


            Private _Context As IContextLookup

            ''' <summary>
            ''' Gets or sets the context lookup
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Items() As IContextLookup
                Get
                    Return _Context
                End Get
                Set(ByVal value As IContextLookup)
                    _Context = value
                End Set
            End Property

            ''' <summary>
            ''' Calculates the variable type
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            ''' <remarks></remarks>
            Public Sub ResolveVariableType(ByVal sender As Object, ByVal e As ResolveVariableTypeEventArgs)
                Dim obj As Object = Nothing
                If _Context IsNot Nothing AndAlso _Context.Items.TryGetValue(e.VariableName, obj) Then
                    If obj IsNot Nothing Then
                        e.VariableType = obj.GetType()
                        While Not e.VariableType.IsVisible
                            e.VariableType = e.VariableType.BaseType
                        End While
                    Else
                        e.VariableType = GetType(Object)
                    End If
                Else
                    e.VariableType = GetType(Object)
                End If

            End Sub

            ''' <summary>
            ''' Calculates the variable value
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            ''' <remarks></remarks>
            Public Sub ResolveVariableValue(ByVal sender As Object, ByVal e As ResolveVariableValueEventArgs)
                Dim obj As Object = Nothing
                If _Context IsNot Nothing AndAlso _Context.Items.TryGetValue(e.VariableName, obj) Then
                    If obj IsNot Nothing Then
                        e.VariableValue = obj
                    Else
                        e.VariableValue = Nothing
                    End If
                Else
                    e.VariableValue = Nothing
                End If

            End Sub

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                        Me._Context = Nothing
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class


    End Class
End Namespace