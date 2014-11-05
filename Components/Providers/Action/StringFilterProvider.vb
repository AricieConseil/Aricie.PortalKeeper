Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports System.Xml.XPath
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum StringFilterMode
        TokenReplace
        TransformsList
        Xpath
    End Enum

    <ActionButton(IconName.Font, IconOptions.Normal)> _
   <Serializable()> _
   <DisplayName("String Filter Provider")> _
       <Description("This provider allows to you to manipulate a string, by either running a series of transformations or performing xpath selects")> _
    Public Class StringFilterProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)


        Private _InputExpression As New FleeExpressionInfo(Of String)

        Private _FilterMode As StringFilterMode

        Private _Filter As New ExpressionFilterInfo

        <ExtendedCategory("Filter")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property InputExpression() As FleeExpressionInfo(Of String)
            Get
                Return _InputExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _InputExpression = value
            End Set
        End Property


        <ExtendedCategory("Filter")> _
        Public Property FilterMode() As StringFilterMode
            Get
                Return _FilterMode
            End Get
            Set(ByVal value As StringFilterMode)
                _FilterMode = value
            End Set
        End Property


        <ExtendedCategory("Filter")> _
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.TransformsList)> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property Filter() As ExpressionFilterInfo
            Get
                Return _Filter
            End Get
            Set(ByVal value As ExpressionFilterInfo)
                _Filter = value
            End Set
        End Property

        <ExtendedCategory("Filter")> _
       <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)> _
        Public Property XPath() As New XPathInfo()
          

        <ExtendedCategory("Filter")> _
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)> _
        Public Property XPathNavigableVarName As String = ""


        <ExtendedCategory("Filter")> _
       <ConditionalVisible("FilterMode", False, True, StringFilterMode.TokenReplace)> _
        Public Property UseAdditionalTokens As Boolean

        <ExtendedCategory("Filter")> _
        <ConditionalVisible("UseAdditionalTokens", False, True)> _
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.TokenReplace)> _
        Public Property AdditionalTokens As New TokenSourceInfo()

        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim input As String = Me._InputExpression.Evaluate(actionContext, actionContext)
            Select Case Me._FilterMode
                Case StringFilterMode.Xpath
                    If Not String.IsNullOrEmpty(XPathNavigableVarName) Then
                        Dim navigable As IXPathNavigable = DirectCast(actionContext.GetVar(XPathNavigableVarName), IXPathNavigable)
                        If navigable Is Nothing OrElse navigable.CreateNavigator.InnerXml <> input Then
                            navigable = _XPath.GetNavigable(input)
                            actionContext.SetVar(XPathNavigableVarName, navigable)
                        End If
                        Return Me._XPath.DoSelect(navigable)
                    End If
                    Return (Me._XPath.DoSelect(input))
                Case StringFilterMode.TokenReplace
                    Dim atr As AdvancedTokenReplace = actionContext.GetAdvancedTokenReplace()
                    If UseAdditionalTokens Then
                        AdditionalTokens.SetTokens(atr)
                    End If
                    Return atr.ReplaceAllTokens(input)
                Case Else
                    Return Me.Filter.Process(input)
            End Select
        End Function


        Protected Overrides Function GetOutputType() As Type
            Select Case Me._FilterMode
                Case StringFilterMode.TransformsList
                    Return GetType(String)
                Case StringFilterMode.Xpath
                    Return Me._XPath.GetOutputType()
                Case StringFilterMode.TokenReplace
                    Return GetType(String)
            End Select
            Return GetType(String)
        End Function
    End Class
End Namespace