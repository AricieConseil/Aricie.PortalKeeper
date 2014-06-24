Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Services
Imports System.Reflection
Imports Ciloci.Flee
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Security

Namespace Services.Flee

    ''' <summary>
    ''' Helper for flee manipulation
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Class FleeHelper(Of T)

        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of Object, T)()) As IDictionary(Of Object, T)
            Dim toReturn As New Dictionary(Of Object, T)
            For Each objPair As KeyValuePair(Of Object, T) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function
        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of String, T)()) As IDictionary(Of String, T)
            Dim toReturn As New Dictionary(Of String, T)
            For Each objPair As KeyValuePair(Of String, T) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function



    End Class



    Public Module FleeHelper

 
        ''' <summary>
        ''' makes a dictionary from key values
        ''' </summary>
        ''' <param name="pairs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeDictionary(ByVal ParamArray pairs As KeyValuePair(Of Object, Object)()) As IDictionary
            Dim toReturn As New Dictionary(Of Object, Object)
            For Each objPair As KeyValuePair(Of Object, Object) In pairs
                toReturn(objPair.Key) = objPair.Value
            Next
            Return toReturn
        End Function

        ''' <summary>
        ''' Sets properties on an object through reflexion
        ''' </summary>
        ''' <param name="objTarget"></param>
        ''' <param name="prop1"></param>
        ''' <param name="obj1"></param>
        ''' <param name="prop2"></param>
        ''' <param name="obj2"></param>
        ''' <param name="prop3"></param>
        ''' <param name="obj3"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetProperties(ByVal objTarget As Object, ByVal prop1 As String, ByVal obj1 As Object, Optional ByVal prop2 As String = "", Optional ByVal obj2 As Object = Nothing, Optional ByVal prop3 As String = "", Optional ByVal obj3 As Object = Nothing) As Object
            Dim toReturn As Object = objTarget
            If toReturn IsNot Nothing Then
                Dim propDico As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(toReturn.GetType)
                Dim currentPropInfo As PropertyInfo = Nothing
                If Not String.IsNullOrEmpty(prop1) Then
                    If propDico.TryGetValue(prop1, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj1, Nothing)
                    End If
                End If
                If Not String.IsNullOrEmpty(prop2) Then
                    If propDico.TryGetValue(prop2, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj2, Nothing)
                    End If
                End If
                If Not String.IsNullOrEmpty(prop3) Then
                    If propDico.TryGetValue(prop3, currentPropInfo) Then
                        currentPropInfo.SetValue(toReturn, obj3, Nothing)
                    End If
                End If
            End If
            Return toReturn
        End Function

        Public Function Format(ByVal tempalte As String, value As String) As String
            Return String.Format(tempalte, value)
        End Function

        Public Function Format(ByVal tempalte As String, value1 As String, value2 As String) As String
            Return String.Format(tempalte, value1, value2)
        End Function



    End Module

    ''' <summary>
    ''' Flee expression, wrapper around SimpleExpressionInfo
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <ActionButton(IconName.Code, IconOptions.Normal)> _
    <Serializable()> _
    Public Class FleeExpressionInfo(Of TResult)
        Inherits SimpleExpression(Of TResult)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(expressionText As String)
            MyBase.New(expressionText)
        End Sub

        <ExtendedCategory("ExpressionOwner")> _
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

        <ExtendedCategory("ExpressionOwner")> _
        <ConditionalVisible("OverrideOwner", False, True)> _
Public Property OwnerMemberAccess As BindingFlags
            Get
                Return InternalOwnerMemberAccess
            End Get
            Set(value As BindingFlags)
                InternalOwnerMemberAccess = value
            End Set
        End Property

        <ExtendedCategory("ExpressionOwner")> _
        <ConditionalVisible("OverrideOwner", False, True)> _
        Public Property NewOwner As FleeExpressionInfo(Of Object)
            Get
                Return InternalNewOwner
            End Get
            Set(value As FleeExpressionInfo(Of Object))
                InternalNewOwner = value
            End Set
        End Property
       
        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalVariables
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Variables")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        Public Property Variables() As Variables
            Get
                Return InternalVariables
            End Get
            Set(ByVal value As Variables)
                InternalVariables = value
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

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalDateTimeFormat
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
            <Required(True)> _
        Public Property DateTimeFormat() As String
            Get
                Return InternalDateTimeFormat
            End Get
            Set(ByVal value As String)
                InternalDateTimeFormat = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalRequireDigitsBeforeDecimalPoint
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
        Public Property RequireDigitsBeforeDecimalPoint() As Boolean
            Get
                Return InternalRequireDigitsBeforeDecimalPoint
            End Get
            Set(ByVal value As Boolean)
                InternalRequireDigitsBeforeDecimalPoint = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalDecimalSeparator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
            <Required(True)> _
        Public Property DecimalSeparator() As Char
            Get
                Return InternalDecimalSeparator
            End Get
            Set(ByVal value As Char)
                InternalDecimalSeparator = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalFunctionArgumentSeparator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
            <Required(True)> _
        Public Property FunctionArgumentSeparator() As Char
            Get
                Return InternalFunctionArgumentSeparator
            End Get
            Set(ByVal value As Char)
                InternalFunctionArgumentSeparator = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalParseCultureMode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
        Public Property ParseCultureMode() As CultureInfoMode
            Get
                Return InternalParseCultureMode
            End Get
            Set(ByVal value As CultureInfoMode)
                InternalParseCultureMode = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalCustomCultureLocale
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("ParseCultureMode", False, True, CultureInfoMode.Custom)> _
        Public Property CustomCultureLocale() As String
            Get
                Return InternalCustomCultureLocale
            End Get
            Set(ByVal value As String)
                InternalCustomCultureLocale = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets SimpleExpressionInfo.InternalRealLiteralDataType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("TechnicalSettings")> _
        Public Property RealLiteralDataType() As RealLiteralDataType
            Get
                Return InternalRealLiteralDataType
            End Get
            Set(ByVal value As RealLiteralDataType)
                InternalRealLiteralDataType = value
            End Set
        End Property



    End Class
End Namespace