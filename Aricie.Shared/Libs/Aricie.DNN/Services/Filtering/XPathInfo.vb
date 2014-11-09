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

    Public Enum XPathOutputMode
        Selection
        DocumentString
        DocumentNavigable
    End Enum

    Public Enum XPathSelectMode
        SelectionString
        SelectionNodes
    End Enum



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


        Public Sub New(selectExpression As String, isSingle As Boolean, isHtml As Boolean)
            Me._SelectExpression = selectExpression
            Me._SingleSelect = isSingle
            Me._IsHtmlContent = isHtml
        End Sub


        'Public Overridable Property XPathMode As XPathMode = XPathMode.ReturnResults
        Public Property OutputMode As XPathOutputMode = XPathOutputMode.Selection

        <ConditionalVisible("OutputMode", False, True, XPathOutputMode.Selection)> _
        Public Property SelectMode As XPathSelectMode = XPathSelectMode.SelectionString

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

        <ExtendedCategory("Filter")> _
        Public Property ApplyFilter As Boolean

        <ConditionalVisible("ApplyFilter", False, True)> _
        <ExtendedCategory("Filter")> _
        Public Property UpdateNodes As Boolean

        Private _Filter As ExpressionFilterInfo

        <ConditionalVisible("ApplyFilter", False, True)> _
        <ExtendedCategory("Filter")> _
        Public Property Filter As ExpressionFilterInfo
            Get
                If Not Me.ApplyFilter Then
                    Return Nothing
                End If
                If _Filter Is Nothing Then
                    _Filter = New ExpressionFilterInfo
                End If
                Return _Filter
            End Get
            Set(value As ExpressionFilterInfo)
                _Filter = value
            End Set
        End Property


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

        Private _SimulationResult As String = ""

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
                Return _SimulationResult
            End Get
        End Property

        <ExtendedCategory("Simulation")> _
          <ConditionalVisible("Simulation", False, True, True)> _
        <ActionButton(IconName.Refresh, IconOptions.Normal)> _
        Public Sub RunSimulation(ByVal pe As AriciePropertyEditorControl)
            If Not String.IsNullOrEmpty(Me.SimulationData.Value) Then
                Dim toReturn As Object = DoSelect(Me.SimulationData.Value)
                If toReturn IsNot Nothing Then
                    Me._SimulationResult = Aricie.Services.ReflectionHelper.Serialize(toReturn).InnerXml
                End If
                pe.ItemChanged = True
            End If
        End Sub

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
                Select Case OutputMode
                    Case XPathOutputMode.DocumentNavigable
                        toReturn = source
                    Case XPathOutputMode.DocumentString
                        If TypeOf source Is HtmlDocument Then
                            Return DirectCast(source, HtmlDocument).DocumentNode.OuterHtml
                        Else
                            navigator = source.CreateNavigator()
                            navigator.MoveToRoot()
                            Return navigator.OuterXml()
                        End If
                End Select
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
            Dim results As XPathNodeIterator
            results = navigator.Select(Me._SelectExpression)
           

            If Not Me._SelectTree Then
                Dim multiString As List(Of String) = Nothing
                Dim resultList As List(Of XPathNavigator) = Nothing
                If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                    resultList = New List(Of XPathNavigator)
                Else
                    multiString = New List(Of String)
                End If
                Dim multiNodes As New List(Of XPathNavigator)()
                For Each result As XPathNavigator In results
                   
                    Dim resultValue As String = result.Value
                    If Me.ApplyFilter Then
                        resultValue = Me.Filter.Process(resultValue)
                        If Me.UpdateNodes Then
                            If TypeOf result Is HtmlNodeNavigator Then
                                DirectCast(result, HtmlAgilityPack.HtmlNodeNavigator).CurrentNode.InnerHtml = resultValue
                            Else
                                result.SetValue(resultValue)
                            End If
                        End If
                    End If
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        resultList.Add(result)
                    Else
                        multiString.Add(resultValue)
                    End If

                    If SingleSelect Then
                        Exit For
                    End If
                Next
                If Not _SingleSelect Then
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        Return resultList
                    Else
                        Return multiString
                    End If

                ElseIf (Me.SelectMode = XPathSelectMode.SelectionNodes) Then
                    If resultList.Count > 0 Then
                        Return resultList(0)
                    Else
                        Return Nothing
                    End If

                ElseIf (Me.SelectMode = XPathSelectMode.SelectionString) Then
                    If multiString.Count > 0 Then
                        Return multiString(0)
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            Else
                Dim multiDico As List(Of SerializableDictionary(Of String, String)) = Nothing
                Dim multiDicoNodes As List(Of SerializableDictionary(Of String, Object)) = Nothing
                If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                    multiDicoNodes = New List(Of SerializableDictionary(Of String, Object))
                Else
                    multiDico = New List(Of SerializableDictionary(Of String, String))
                End If
                For Each result As XPathNavigator In results
                    Dim currentDico As SerializableDictionary(Of String, String) = Nothing
                    Dim currentDicoNodes As SerializableDictionary(Of String, Object) = Nothing
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        currentDicoNodes = New SerializableDictionary(Of String, Object)
                    Else
                        currentDico = New SerializableDictionary(Of String, String)
                    End If

                    For Each subSelectPair As KeyValuePair(Of String, XPathInfo) In Me._SubSelects
                        Dim objSubResult As Object = subSelectPair.Value.SelectNavigate(result)
                        If objSubResult IsNot Nothing Then
                            If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                                currentDicoNodes(subSelectPair.Key) = objSubResult
                            Else
                                Dim subResult As String = DirectCast(objSubResult, String)
                                currentDico(subSelectPair.Key) = subResult
                            End If
                        End If
                    Next
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        multiDicoNodes.Add(currentDicoNodes)
                    Else
                        multiDico.Add(currentDico)
                    End If
                Next
                If Not _SingleSelect Then
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        Return multiDicoNodes
                    Else
                        Return multiDico
                    End If

                ElseIf multiDico.Count > 0 Then
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        Return multiDicoNodes(0)
                    Else
                        Return multiDico(0)
                    End If
                Else
                    If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                        Return New SerializableDictionary(Of String, Object)
                    Else
                        Return New SerializableDictionary(Of String, String)
                    End If
                End If
            End If
        End Function


        Public Function GetOutputType() As Type
            Select Case Me.OutputMode
                Case XPathOutputMode.DocumentString
                    Return GetType(String)
                Case XPathOutputMode.DocumentNavigable
                    Return GetType(IXPathNavigable)
                Case Else
                    If Not Me._SelectTree Then
                        If Not _SingleSelect Then
                            If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                                Return GetType(List(Of XPathNavigator))
                            Else
                                Return GetType(List(Of String))
                            End If

                        Else
                            If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                                Return GetType(XPathNavigator)
                            Else
                                Return GetType(String)
                            End If
                        End If
                    Else
                        If Not _SingleSelect Then
                            If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                                Return GetType(List(Of SerializableDictionary(Of String, Object)))
                            Else
                                Return GetType(List(Of SerializableDictionary(Of String, String)))
                            End If
                        Else
                            If Me.SelectMode = XPathSelectMode.SelectionNodes Then
                                Return GetType(SerializableDictionary(Of String, Object))
                            Else
                                Return GetType(SerializableDictionary(Of String, String))
                            End If
                        End If
                    End If
            End Select
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


