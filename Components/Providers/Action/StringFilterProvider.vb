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

        Private _XPath As New XPathInfo

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
                _Escaper = Nothing
            End Set
        End Property

        <ExtendedCategory("Filter")> _
       <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)> _
        Public Property XPath() As XPathInfo
            Get
                Return _XPath
            End Get
            Set(ByVal value As XPathInfo)
                _XPath = value
            End Set
        End Property

        <ExtendedCategory("Filter")> _
      <ConditionalVisible("FilterMode", False, True, StringFilterMode.Xpath)> _
        Public Property XPathNavigableVarName As String = ""



        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
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
                Case Else
                    Return Me.Escaper.EscapeString(input)
            End Select
        End Function


        Private _Escaper As StringEscaper

        <XmlIgnore()> _
        <Browsable(False)> _
        Protected ReadOnly Property Escaper() As StringEscaper
            Get
                If _Escaper Is Nothing Then
                    SyncLock Me._Filter
                        If _Escaper Is Nothing Then
                            Me._Escaper = New StringEscaper(Me._Filter)
                        End If
                    End SyncLock
                End If
                Return _Escaper
            End Get
        End Property

        Protected Overrides Function GetOutputType() As Type
            Select Case Me._FilterMode
                Case StringFilterMode.TransformsList
                    Return GetType(String)
                Case StringFilterMode.Xpath
                    Return Me._XPath.GetOutputType()
            End Select
            Return GetType(String)
        End Function
    End Class
End Namespace