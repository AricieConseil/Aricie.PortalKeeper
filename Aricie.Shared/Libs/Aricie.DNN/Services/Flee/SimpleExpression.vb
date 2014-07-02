Imports Aricie.DNN.UI.Attributes
Imports Ciloci.Flee
Imports Aricie.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
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

        Private Shared ReadOnly _CompiledExpressions As New Dictionary(Of String, IGenericExpression(Of TResult))


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
        Public ReadOnly Property ReturnType() As String
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
        Public Property Expression() As String
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


        <ExtendedCategory("", "Help")> _
        Public Property ExpressionBuilder As ExpressionBuilder

        <ConditionalVisible("HasExpressionBuilder", True, True)> _
        <ExtendedCategory("", "Help")> _
        <ActionButton(IconName.Magic, IconOptions.Normal)> _
        Public Sub DisplayAvailableVars(ByVal pe As AriciePropertyEditorControl)
            Dim avVars As IDictionary(Of String, Type) = New Dictionary(Of String, Type)
            Dim currentPe As AriciePropertyEditorControl = pe
            Dim currentProvider As IExpressionVarsProvider
            Dim previousProvider As IExpressionVarsProvider = Nothing
            Do
                If TypeOf currentPe.DataSource Is IExpressionVarsProvider Then
                    currentProvider = DirectCast(currentPe.DataSource, IExpressionVarsProvider)
                    'If previousProvider Is Nothing Then
                    '    previousProvider = currentProvider
                    'End If
                    currentProvider.AddVariables(previousProvider, avVars)
                    previousProvider = currentProvider
                End If
                currentPe = currentPe.ParentAricieEditor

            Loop Until currentPe Is Nothing

            Me.ExpressionBuilder = New ExpressionBuilder(avVars.ToDictionary(Function(objVarPair) objVarPair.Key, Function(objVarPair) New DotNetType(objVarPair.Value)))
            pe.DisplayLocalizedMessage("ExpressionHelper.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
        End Sub


        <ConditionalVisible("HasExpressionBuilder", False, True)> _
           <ExtendedCategory("", "Help")> _
          <ActionButton(IconName.Clipboard, IconOptions.Normal)> _
        Public Sub InsertSelectedVar(ByVal pe As AriciePropertyEditorControl)
            Me.Expression &= Me.ExpressionBuilder.GetInsertString()
            pe.ItemChanged = True
        End Sub





        ''' <summary>
        ''' Evaluates the flee expression
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Evaluate(ByVal owner As Object, ByVal globalVars As IContextLookup) As TResult

            Dim toReturn As TResult
            If Me._Expression <> "" Then

                Dim clone As IGenericExpression(Of TResult) = Me.GetCompiledExpression(owner, globalVars)
                If clone IsNot Nothing Then
                    Using onDemand As New OnDemandVariableLookup(globalVars)
                        AddHandler clone.Context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                        AddHandler clone.Context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        Try
                            toReturn = clone.Evaluate()
                        Catch ex As Exception
                            Dim objFLeeException As New HttpException(String.Format("Flee Expression ""{0}"" failed to run with the inner exception", Me.Expression), ex)
                            Throw objFLeeException
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
        Public Function GetCompiledExpression(ByVal owner As Object, ByVal globalVars As IContextLookup) As IGenericExpression(Of TResult)
            Dim toReturn As IGenericExpression(Of TResult) = Nothing

            If Not _CompiledExpressions.TryGetValue(Me._Expression, toReturn) Then
                SyncLock Me
                    'SyncLock expWriterLock
                    If Not _CompiledExpressions.TryGetValue(Me._Expression, toReturn) Then

                        Dim context As ExpressionContext = Me.GetExpressionContext(owner, globalVars)
                        Dim onDemand As New OnDemandVariableLookup(globalVars)
                        AddHandler context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                        AddHandler context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                        Try
                            toReturn = context.CompileGeneric(Of TResult)(Me._Expression)
                            SyncLock _CompiledExpressions
                                _CompiledExpressions(Me._Expression) = toReturn
                            End SyncLock
                        Catch ex As Ciloci.Flee.ExpressionCompileException
                            Dim objFLeeException As New HttpException(String.Format("Flee Expression ""{0}"" failed to compile with the inner exception", Me.Expression), ex)
                            ExceptionHelper.LogException(objFLeeException)
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
                toReturn = DirectCast(toReturn.Clone, IGenericExpression(Of TResult))
                If Me.InternalVariables.Instances.Count > 0 Then
                    Dim vars As Dictionary(Of String, Object) = Me.InternalVariables.EvaluateVariables(owner, globalVars)
                    For Each objVar As VariableInfo In Me.InternalVariables.Instances
                        If objVar.EvaluationMode = VarEvaluationMode.Dynamic Then
                            toReturn.Context.Variables(objVar.Name) = vars(objVar.Name)
                        End If
                    Next
                End If
                If Not _OwnerProviderSuplied Then
                    If InternalOverrideOwner Then
                        Dim newOwner As Object = Me.InternalNewOwner.Evaluate(owner, globalVars)
                        toReturn.Owner = newOwner
                    Else
                        toReturn.Owner = owner
                    End If
                Else
                    toReturn.Owner = DirectCast(owner, IContextOwnerProvider).ContextOwner
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

            toReturn.ParserOptions.DateTimeFormat = Me.InternalDateTimeFormat
            toReturn.ParserOptions.RequireDigitsBeforeDecimalPoint = Me.InternalRequireDigitsBeforeDecimalPoint
            toReturn.ParserOptions.DecimalSeparator = Me.InternalDecimalSeparator
            toReturn.ParserOptions.FunctionArgumentSeparator = Me.InternalFunctionArgumentSeparator
            toReturn.ParserOptions.RecreateParser()
            Select Case Me.InternalParseCultureMode
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
                        e.VariableType = obj.GetType
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