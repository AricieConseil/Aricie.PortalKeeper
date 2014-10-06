Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Web.Compilation
Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports System.Web.UI.WebControls
Imports Aricie.Services
Imports System.Text
Imports Aricie.DNN.UI.WebControls

Namespace Services.Flee

    <DefaultProperty("InsertString")> _
    <Serializable()> _
    Public MustInherit Class FleeExpressionBuilderBase


        <XmlIgnore()> _
        Public AvailableVariables As New Dictionary(Of String, DotNetType)

        <ConditionalVisible("InsertString", True, True, "")>
        Public MustOverride ReadOnly Property InsertString As String


    End Class


    '<Serializable()> _
    'Public Class OperatorExpression
    '    Inherits ExpressionBuilderBase
    '    Implements ISelector


    '    Public Sub New()

    '    End Sub

    '    Public Sub New(vars As IDictionary(Of String, DotNetType))
    '        Me.SubExpression = New ExpressionBuilder(vars)
    '    End Sub

    '    <AutoPostBack()> _
    '    <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
    '   <ProvidersSelector("Text", "Value")>
    '    Public Property SelectedOperator As String = ""


    '    Public Property SubExpression As ExpressionBuilder

    '    Public Overrides ReadOnly Property InsertString As String
    '        Get
    '            Return String.Format(" {0} {1}", SelectedOperator, SubExpression.InsertString)
    '        End Get
    '    End Property

    '    Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
    '        Select Case propertyName
    '            Case "SelectedOperator"
    '                Return FleeBinaryOperators.Select(Function(objString) New ListItem(objString)).ToList()
    '        End Select
    '    End Function


    '    Private Shared ReadOnly FleeBinaryOperators As New List(Of String) From {"+", "-", "*", "/", "^", "%", "=", "<>", "<", "<=", "and", "or", "xor", "<<", ">>", "In"}
    '    'Private Shared ReadOnly FleeUnaryOperators As New List(Of String) From {"Not"}

    'End Class
    <Flags()> _
    Public Enum ExpressionFeature
        None = 0
        Negate = 1
        Parenthesis = 2
        SubMember = 4
        Binary = 8
        Cast = 16
    End Enum


    <Serializable()>
    Public Class FleeExpressionBuilder
        Inherits FleeExpressionBuilderBase
        Implements ISelector

        Private Shared ReadOnly FleeBinaryOperators As New List(Of String) From {"+", "-", "*", "/", "^", "%", "=", "<>", "<", "<=", "and", "or", "xor", "<<", ">>", "In"}

        Public Sub New()

        End Sub

        Public Sub New(vars As Dictionary(Of String, DotNetType))
            AvailableVariables = vars
        End Sub



        Private _SelectedVariable As String = ""
        Private _Features As ExpressionFeature
        Private _SelectedOperator As String

       


       

        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Text", "Value", False, True, "<-- Select a Variable -->", "", False, True)>
        Public Property SelectedVariable As String
            Get
                Return _SelectedVariable
            End Get
            Set(value As String)
                If _SelectedVariable <> value Then
                    _SelectedVariable = value
                    VariableMember = Nothing
                End If
            End Set
        End Property

        <ConditionalVisible("SelectedVariable", True, True, "")>
       <AutoPostBack()> _
        Public Property Features As ExpressionFeature
            Get
                Return _Features
            End Get
            Set(value As ExpressionFeature)
                If value <> _Features Then

                    If (value And ExpressionFeature.Binary) <> ExpressionFeature.Binary Then
                        Me.SubExpression = Nothing
                    ElseIf Me.SubExpression Is Nothing Then
                        'Me.SubExpression = New ExpressionBuilder(Me.AvailableVariables)
                        CreateSubExpression()
                    End If
                    If (value And ExpressionFeature.SubMember) <> ExpressionFeature.SubMember Then
                        Me.VariableMember = Nothing
                    ElseIf Me.VariableMember Is Nothing Then
                        Me.VariableMember = New MemberDrillDown(SelectedVariableType)
                    End If
                    _Features = value
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property SelectedVariableType As DotNetType
            Get
                If Not _SelectedVariable.IsNullOrEmpty() Then
                    Return AvailableVariables(SelectedVariable)
                End If
                Return Nothing
            End Get
        End Property


        <ConditionalVisible("SelectedVariable", True, True, "")>
        Public Property VariableMember As MemberDrillDown


        <ConditionalVisible("SelectedVariable", True, True, "")>
        Public ReadOnly Property ResultingTypeName As String
            Get
                If Not SelectedVariable.IsNullOrEmpty() Then

                    If VariableMember IsNot Nothing Then
                        Dim objResultingMember As MemberInfo = Me.VariableMember.ResultingMemberInfo
                        If objResultingMember IsNot Nothing Then
                            Return ReflectionHelper.GetMemberReturnType(objResultingMember).AssemblyQualifiedName
                        End If
                    Else
                        Return SelectedVariableType.GetDotNetType().AssemblyQualifiedName
                    End If
                End If
                Return Nothing
            End Get
        End Property


        <ConditionalVisible("Features", False, True, ExpressionFeature.Binary)> _
        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Text", "Value", False, True, "<-- Select an Operator -->", "", False, True)> _
        <ConditionalVisible("SelectedVariable", True, True, "")>
        Public Property SelectedOperator As String
            Get
                Return _SelectedOperator
            End Get
            Set(value As String)
                If value <> _SelectedOperator Then
                    If value.IsNullOrEmpty() Then
                        Me.SubExpression = Nothing
                    ElseIf Me.SubExpression Is Nothing Then
                        CreateSubExpression()
                    End If
                    _SelectedOperator = value
                End If
            End Set
        End Property

        <ConditionalVisible("Features", False, True, ExpressionFeature.Binary)> _
        <ConditionalVisible("SelectedVariable", True, True, "")> _
        <ConditionalVisible("SelectedOperator", True, True, "")>
        Public Property SubExpression As FleeExpressionBuilder


        Public Const ExpressionOwnerVar As String = "<Expression Owner>"

        <ConditionalVisible("SelectedVariable", True, True, "")> _
        Public Overrides ReadOnly Property InsertString As String
            Get
                Dim toReturn As New StringBuilder()

                If Not Me.SelectedVariable.IsNullOrEmpty() Then
                    If (Me.Features And ExpressionFeature.Parenthesis) = ExpressionFeature.Parenthesis Then
                        toReturn.Append("(")
                    End If
                    If (Me.Features And ExpressionFeature.Negate) = ExpressionFeature.Negate Then
                        toReturn.Append("Not ")
                    End If
                    If SelectedVariable = ExpressionOwnerVar Then
                        If Me.VariableMember IsNot Nothing Then
                            toReturn.Append(Me.VariableMember.InsertString.TrimStart("."c))
                        End If
                    Else

                        toReturn.Append(Me.SelectedVariable)
                        If Me.VariableMember IsNot Nothing Then
                            toReturn.Append(Me.VariableMember.InsertString)
                        End If
                    End If

                    
                    If Not SelectedOperator.IsNullOrEmpty() Then
                        toReturn.Append(String.Format(" {0} {1}", SelectedOperator, SubExpression.InsertString)) 'OperatorExpression.InsertString)
                    End If
                    If (Me.Features And ExpressionFeature.Parenthesis) = ExpressionFeature.Parenthesis Then
                        toReturn.Append(")")
                    End If
                End If
                Return toReturn.ToString()
            End Get
        End Property



        Public Overridable Function GetAvailableVars(pe As AriciePropertyEditorControl) As Dictionary(Of String, Type)
            Dim avVars As New Dictionary(Of String, Type)
            Dim currentPe As AriciePropertyEditorControl = pe
            Dim currentProvider As IExpressionVarsProvider
            Dim previousProvider As IExpressionVarsProvider = Nothing
            Do
                If TypeOf currentPe.DataSource Is IExpressionVarsProvider Then
                    currentProvider = DirectCast(currentPe.DataSource, IExpressionVarsProvider)
                    'If previousProvider Is Nothing Then
                    '    previousProvider = currentProvider
                    'End If
                    currentProvider.AddVariables(previousProvider, DirectCast(avVars, IDictionary(Of String, Type)))
                    previousProvider = currentProvider
                End If
                currentPe = currentPe.ParentAricieEditor

            Loop Until currentPe Is Nothing
            Return avVars
        End Function



    Private Sub CreateSubExpression()
            Me.SubExpression = DirectCast(ReflectionHelper.CreateObject(GetType(FleeExpressionBuilder).AssemblyQualifiedName), FleeExpressionBuilder)
            Me.SubExpression.AvailableVariables = Me.AvailableVariables
        End Sub


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "SelectedVariable"
                    Dim vars = (AvailableVariables.Keys).ToList()
                    vars.Sort()
                    Return vars.Select(Function(objString) New ListItem(objString & " ( " & ReflectionHelper.GetSimpleTypeName(AvailableVariables(objString).GetDotNetType()) & " )", objString)).ToList()
                Case "SelectedOperator"
                    Return FleeBinaryOperators.Select(Function(objString) New ListItem(objString)).ToList()
            End Select
            Return Nothing
        End Function


    End Class
End Namespace