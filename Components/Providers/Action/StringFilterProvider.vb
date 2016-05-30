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
        StringSplit
        Xpath
    End Enum

    <ActionButton(IconName.Font, IconOptions.Normal)>
    <DisplayName("String Filter Provider")>
    <Description("This provider allows to you to manipulate a string, by either running a series of transformations or performing xpath selects")>
    Public Class StringFilterProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)





        <ExtendedCategory("Filter")>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))>
        <LabelMode(LabelMode.Top)>
        Public Property InputExpression() As New FleeExpressionInfo(Of String)
           


        <ExtendedCategory("Filter")>
        Public Property FilterMode() As StringFilterMode
           


        <ExtendedCategory("Filter")>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.TransformsList)>
        Public Property FilterSource() As New SimpleOrExpression(Of ExpressionFilterInfo)

         <ExtendedCategory("Filter")>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.StringSplit)>
        Public Property Splitter() As New SimpleOrExpression(Of StringSplitInfo)

        <ExtendedCategory("Filter")>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)>
        Public Property XPathSource As New SimpleOrExpression(Of XPathInfo)

      


        <ExtendedCategory("Filter")>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)>
        Public Property XPathNavigableVarName As String = ""


        <ExtendedCategory("Filter")>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.TokenReplace)>
        Public Property UseAdditionalTokens As Boolean

        <ExtendedCategory("Filter")>
        <ConditionalVisible("UseAdditionalTokens", False, True)>
        <ConditionalVisible("FilterMode", False, True, StringFilterMode.TokenReplace)>
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
                        Return tempXpath.DoSelect(navigable, actionContext)
                    End If
                    Return (tempXpath.DoSelect(input, actionContext))
                Case StringFilterMode.TokenReplace
                    Dim atr As AdvancedTokenReplace = actionContext.GetAdvancedTokenReplace()
                    If UseAdditionalTokens Then
                        AdditionalTokens.SetTokens(atr)
                    End If
                    Return atr.ReplaceAllTokens(input)
                Case StringFilterMode.StringSplit
                    Return Me.Splitter.GetValue(actionContext, actionContext).Process(input, actionContext)
                Case Else
                    Return Me.FilterSource.GetValue(actionContext, actionContext).Process(input, actionContext)
            End Select
        End Function


        Protected Overrides Function GetOutputType() As Type
            Select Case Me._FilterMode
                Case StringFilterMode.TransformsList
                    Return GetType(String)
                Case StringFilterMode.Xpath
                    if Me.XPathSource.Simple IsNot nothing
                        Return Me.XPathSource.Simple.GetOutputType()
                    End If
                    Return Nothing
                Case StringFilterMode.StringSplit
                    if Me.Splitter.Simple IsNot nothing
                        Return Me.Splitter.Simple.GetOutputType()
                    End If
                    Return Nothing
                Case StringFilterMode.TokenReplace
                    Return GetType(String)
            End Select
            Return GetType(String)
        End Function
    End Class
End Namespace