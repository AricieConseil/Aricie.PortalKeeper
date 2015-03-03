Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
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
        Public Property FilterSource() As New SimpleOrExpression(Of ExpressionFilterInfo)

        'todo: remove obsolete property
       <Browsable(False)> _
        Public Property Filter() As ExpressionFilterInfo
            Get
                Return FilterSource.Simple
            End Get
            Set(ByVal value As ExpressionFilterInfo)
                FilterSource.Simple = value
            End Set
        End Property

        <ExtendedCategory("Filter")> _
       <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)> _
        Public Property XPathSource As New SimpleOrExpression(Of XPathInfo)

        'todo: remove obsolete property
        <Browsable(False)> _
        Public Property XPath() As XPathInfo
            Get
                Return XPathSource.Simple
            End Get
            Set(value As XPathInfo)
                XPathSource.Simple = value
            End Set
        End Property


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
                    Dim tempXpath As XPathInfo = XPathSource.GetValue(actionContext, actionContext)
                    If Not String.IsNullOrEmpty(XPathNavigableVarName) Then
                        Dim navigable As IXPathNavigable = DirectCast(actionContext.GetVar(XPathNavigableVarName), IXPathNavigable)
                        If navigable Is Nothing OrElse navigable.CreateNavigator.InnerXml <> input Then
                            navigable = tempXpath.GetNavigable(input)
                            actionContext.SetVar(XPathNavigableVarName, navigable)
                        End If
                        Return tempXpath.DoSelect(navigable)
                    End If
                    Return (tempXpath.DoSelect(input))
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
                    Return Me.XPathSource.Simple.GetOutputType()
                Case StringFilterMode.TokenReplace
                    Return GetType(String)
            End Select
            Return GetType(String)
        End Function
    End Class
End Namespace