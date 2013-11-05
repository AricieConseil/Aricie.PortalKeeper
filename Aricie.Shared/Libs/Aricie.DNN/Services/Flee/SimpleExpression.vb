Imports Aricie.DNN.UI.Attributes
Imports Ciloci.Flee
Imports Aricie.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports System.Globalization
Imports Aricie.Services
Imports System.Threading
Imports System.Xml.Serialization

Namespace Services.Flee

    ''' <summary>
    ''' Simple flee expression
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class SimpleExpression(Of TResult)
        Private _Expression As String = ""

        Protected InternalStaticImports As New List(Of FleeImportInfo)
        Protected InternalImportBuiltinTypes As Boolean = True

        Protected InternalVariables As New Variables


        Protected InternalDateTimeFormat As String = "dd/MM/yyyy"
        Protected InternalRequireDigitsBeforeDecimalPoint As Boolean = False
        Protected InternalDecimalSeparator As Char = "."c
        Protected InternalFunctionArgumentSeparator As Char = ","c
        Protected InternalParseCultureMode As CultureInfoMode = CultureInfoMode.Invariant
        Protected InternalCustomCultureLocale As String = "en-US"
        Protected InternalRealLiteralDataType As RealLiteralDataType = RealLiteralDataType.Decimal

        Private Shared _CompiledExpressions As New Dictionary(Of String, IGenericExpression(Of TResult))


        Public Sub New()
        End Sub

        Public Sub New(ByVal expression As String)
            Me._Expression = expression
        End Sub

        Public Sub New(masterExpression As FleeExpressionInfo(Of TResult))
            Me.New(masterExpression.Expression)
            Me._MasterExpression = masterExpression
        End Sub

        ''' <summary>
        ''' Gets the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        Public ReadOnly Property ReturnType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(TResult))
            End Get
        End Property


        Private _MasterExpression As FleeExpressionInfo(Of TResult)

        ''' <summary>
        ''' Gets or sets the Flee expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        <Browsable(False)> _
        Public Property MasterExpression() As FleeExpressionInfo(Of TResult)
            Get
                Return _MasterExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of TResult))
                _MasterExpression = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the expression as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
            <MainCategory()> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
            LineCount(8), Width(500)> _
        Public Property Expression() As String
            Get
                Return _Expression
            End Get
            Set(ByVal value As String)
                If value IsNot Nothing AndAlso value <> _Expression Then
                    SyncLock Me
                        _Expression = value
                        If _MasterExpression IsNot Nothing Then
                            Me._MasterExpression.Expression = value
                        End If
                    End SyncLock
                End If
            End Set
        End Property

        Private Shared expWriterLock As New Object

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
                SyncLock Me._Expression
                    SyncLock expWriterLock
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
                                DotNetNuke.Services.Exceptions.Exceptions.LogException(objFLeeException)
                                toReturn = Nothing
                            Finally
                                RemoveHandler context.Variables.ResolveVariableType, AddressOf onDemand.ResolveVariableType
                                RemoveHandler context.Variables.ResolveVariableValue, AddressOf onDemand.ResolveVariableValue
                            End Try
                        End If
                    End SyncLock
                End SyncLock
            End If
            If toReturn IsNot Nothing Then
                toReturn = DirectCast(toReturn.Clone, IGenericExpression(Of TResult))

                Dim vars As Dictionary(Of String, Object) = Me.InternalVariables.EvaluateVariables(owner, globalVars)
                For Each objVar As VariableInfo In Me.InternalVariables.Instances
                    If objVar.EvaluationMode = VarEvaluationMode.Dynamic Then
                        toReturn.Context.Variables(objVar.Name) = vars(objVar.Name)
                    End If
                Next

                toReturn.Owner = owner
            End If

            Return toReturn
        End Function


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
        ''' Returns the expression context
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)> _
        Private Function GetExpressionContext(ByVal owner As Object, ByVal globalVars As IContextLookup) As ExpressionContext

            Dim tempContext As New ExpressionContext(owner)

            tempContext.ParserOptions.DateTimeFormat = Me.InternalDateTimeFormat
            tempContext.ParserOptions.RequireDigitsBeforeDecimalPoint = Me.InternalRequireDigitsBeforeDecimalPoint
            tempContext.ParserOptions.DecimalSeparator = Me.InternalDecimalSeparator
            tempContext.ParserOptions.FunctionArgumentSeparator = Me.InternalFunctionArgumentSeparator
            tempContext.ParserOptions.RecreateParser()
            Select Case Me.InternalParseCultureMode
                Case CultureInfoMode.Current
                    tempContext.Options.ParseCulture = DirectCast(Thread.CurrentThread.CurrentCulture.Clone, CultureInfo)
                Case CultureInfoMode.CurrentUI
                    tempContext.Options.ParseCulture = DirectCast(Thread.CurrentThread.CurrentUICulture.Clone, CultureInfo)
                Case CultureInfoMode.Custom
                    tempContext.Options.ParseCulture = New CultureInfo(Me.InternalCustomCultureLocale)
            End Select
            tempContext.Options.RealLiteralDataType = Me.InternalRealLiteralDataType
            If Me.InternalStaticImports.Count = 0 Then
                tempContext.Imports.AddType(GetType(System.Math), "")
            Else
                For Each staticImport As FleeImportInfo In Me.InternalStaticImports
                    tempContext.Imports.AddType(staticImport.DotNetType.GetDotNetType, staticImport.CustomNamespace)
                Next
            End If
            If Me.InternalImportBuiltinTypes Then
                tempContext.Imports.ImportBuiltinTypes()
            End If
            tempContext.Imports.AddType(GetType(FleeHelper), "")

            Dim vars As Dictionary(Of String, Object) = Me.InternalVariables.EvaluateVariables(owner, globalVars)
            For Each objVar As KeyValuePair(Of String, Object) In vars
                tempContext.Variables.Add(objVar.Key, objVar.Value)
            Next

            Return tempContext
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