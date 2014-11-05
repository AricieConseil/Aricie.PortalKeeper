Imports System.Xml.XPath
Imports System.Xml
Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Web.UI.WebControls
Imports Aricie.ComponentModel
Imports System.Web
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls
Imports HtmlAgilityPack

Namespace Services.Filtering

    <Serializable()> _
    Public Class HtmlXPathInfo
        Inherits XPathInfo

        <Browsable(False)> _
        Public Overrides Property IsHtmlContent As Boolean
            Get
                Return True
            End Get
            Set(value As Boolean)
                'donothing
            End Set
        End Property

    End Class

    Public Enum XPathMode
        ReturnResults
        UpdateResults
    End Enum

    Public Enum XPathUpdateOutput
        DocumentString
        SelectionString
    End Enum

    <Serializable()> _
    Public Class UpdateXPathInfo
        Inherits XPathInfo

        <Browsable(False)> _
        Public Overrides Property XPathMode As XPathMode
            Get
                Return Filtering.XPathMode.UpdateResults
            End Get
            Set(value As XPathMode)
                'do nothing
            End Set
        End Property

    End Class


    ''' <summary>
    ''' xpath selection helper class
    ''' </summary>
    ''' <remarks></remarks>
    <ActionButton(IconName.Code, IconOptions.Normal)> _
    <DefaultProperty("SelectExpression")> _
    <Serializable()> _
    Public Class XPathInfo

        Public Sub New()
        End Sub

        Public Sub New(objMode As XPathMode)
            Me.XPathMode = objMode
        End Sub

        Public Sub New(selectExpression As String, isSingle As Boolean, isHtml As Boolean)
            Me._SelectExpression = selectExpression
            Me._SingleSelect = isSingle
            Me._IsHtmlContent = isHtml
        End Sub


        Public Overridable Property XPathMode As XPathMode = XPathMode.ReturnResults

        <ConditionalVisible("XPathMode", False, True, XPathMode.UpdateResults)> _
        Public Property OutputType As XPathUpdateOutput = XPathUpdateOutput.DocumentString

        ''' <summary>
        ''' XPath selection expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
           <LineCount(2)> _
           <Width(500)> _
           <Required(True)> _
        Public Property SelectExpression() As String = ""

        <ConditionalVisible("XPathMode", False, True, XPathMode.UpdateResults)> _
        Public Property Filter As New ExpressionFilterInfo()

        ''' <summary>
        ''' Single node selection 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("XPathSettings")> _
        Public Property SingleSelect() As Boolean

        ''' <summary>
        ''' Selection against Html content
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("XPathSettings")> _
        Public Overridable Property IsHtmlContent() As Boolean

        ''' <summary>
        ''' Selection of whole tree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("XPathSettings")> _
        Public Property SelectTree() As Boolean

        ''' <summary>
        ''' Sub-selection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("XPathSettings")> _
        <ConditionalVisible("SelectTree", False, True)> _
        <LabelMode(LabelMode.Top), Orientation(Orientation.Vertical)> _
        <CollectionEditor(False, True, True, True, 10)> _
        Public Property SubSelects() As New SerializableDictionary(Of String, XPathInfo)

        ''' <summary>
        ''' XPath selection as a simulation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Simulation")> _
        Public Property Simulation() As Boolean

        ''' <summary>
        ''' Simulation data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Simulation")> _
            <Width(500)> _
            <LineCount(8)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <ConditionalVisible("Simulation", False, True, True)> _
        Public Overridable Property SimulationData() As New CData

        ''' <summary>
        ''' Result of the simulation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Simulation")> _
          <ConditionalVisible("Simulation", False, True, True)> _
        Public ReadOnly Property SimulationResult() As String
            Get
                If Me.Simulation Then
                    Try
                        If Not String.IsNullOrEmpty(Me.SimulationData.Value) Then
                            Dim toReturn As Object = DoSelect(Me.SimulationData.Value)
                            If toReturn IsNot Nothing Then
                                Return Aricie.Services.ReflectionHelper.Serialize(toReturn).InnerXml
                            End If
                        End If
                    Catch ex As Exception
                        Return ex.ToString
                    End Try
                End If
                Return String.Empty
            End Get
        End Property

        ''' <summary>
        ''' Select against a navigable entity
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DoSelect(ByVal source As IXPathNavigable) As Object
            If source IsNot Nothing And Not String.IsNullOrEmpty(Me._SelectExpression) Then
                Dim navigator As XPathNavigator = source.CreateNavigator()
                Dim toReturn As Object = Me.SelectNavigate(navigator)
                If Me.XPathMode = Filtering.XPathMode.UpdateResults AndAlso Me.OutputType = XPathUpdateOutput.DocumentString Then
                    navigator.MoveToRoot()
                    Return navigator.OuterXml
                End If
                Return toReturn
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Select against a string that will be converted to HTML
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DoSelect(ByVal source As String) As Object

            If Not String.IsNullOrEmpty(source) AndAlso Not String.IsNullOrEmpty(Me._SelectExpression) Then
                Dim navigable As IXPathNavigable = Me.GetNavigable(source)
                Return DoSelect(navigable)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Runs selection against a navigator
        ''' </summary>
        ''' <param name="navigator"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SelectNavigate(ByVal navigator As XPathNavigator) As Object
            Dim results As XPathNodeIterator = navigator.Select(Me._SelectExpression)

            If Not Me._SelectTree Then
                Dim multiString As New List(Of String)
                For Each result As XPathNavigator In results
                    If XPathMode = Filtering.XPathMode.UpdateResults Then
                        result.SetValue(Me.Filter.Process(result.Value))
                    End If
                    multiString.Add(HttpUtility.HtmlDecode(result.Value))
                    If SingleSelect Then
                        Exit For
                    End If
                Next
                If Not _SingleSelect Then
                    Return multiString
                ElseIf multiString.Count > 0 Then
                    Return multiString(0)
                Else
                    Return String.Empty
                End If
            Else
                Dim multiDico As New List(Of SerializableDictionary(Of String, String))

                For Each result As XPathNavigator In results
                    Dim currentDico As New SerializableDictionary(Of String, String)
                    For Each subSelectPair As KeyValuePair(Of String, XPathInfo) In Me._SubSelects
                        Dim objSubResult As Object = subSelectPair.Value.SelectNavigate(result)
                        If objSubResult IsNot Nothing Then
                            Dim subResult As String = DirectCast(objSubResult, String)
                            currentDico(subSelectPair.Key) = subResult
                        End If
                    Next
                    multiDico.Add(currentDico)
                Next
                If Not _SingleSelect Then
                    Return multiDico
                ElseIf multiDico.Count > 0 Then
                    Return multiDico(0)
                Else
                    Return New SerializableDictionary(Of String, String)
                End If
            End If
        End Function


        Public Function GetOutputType() As Type
            Select Case XPathMode
                Case Filtering.XPathMode.ReturnResults
                    If Not Me._SelectTree Then
                        If Not _SingleSelect Then
                            Return GetType(List(Of String))
                        Else
                            Return GetType(String)
                        End If
                    Else
                        If Not _SingleSelect Then
                            Return GetType(List(Of SerializableDictionary(Of String, String)))
                        Else
                            Return GetType(SerializableDictionary(Of String, String))
                        End If
                    End If
                Case Filtering.XPathMode.UpdateResults
                    Return GetType(String)
            End Select
            Return GetType(String)
        End Function


        ''' <summary>
        ''' Transforms the parameter string into a navigable xpath object
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNavigable(ByVal source As String) As IXPathNavigable
            If IsHtmlContent Then
                Dim doc As New HtmlDocument()
                doc.LoadHtml(source)
                Return doc
            Else
                Dim xmlDoc As New XmlDocument()
                xmlDoc.LoadXml(source)
                Return xmlDoc
            End If
        End Function




    End Class

End Namespace


