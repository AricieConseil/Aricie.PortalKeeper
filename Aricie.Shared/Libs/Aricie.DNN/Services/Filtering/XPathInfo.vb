Imports System.Xml.XPath
Imports System.Xml
Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Web.UI.WebControls
Imports Aricie.ComponentModel

Namespace Services.Filtering

    ''' <summary>
    ''' xpath selection helper class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class XPathInfo

        Public Sub New()
        End Sub

        Public Sub New(selectExpression As String, isSingle As Boolean, isHtml As Boolean)
            Me._SelectExpression = selectExpression
            Me._SingleSelect = isSingle
            Me._IsHtmlContent = isHtml
        End Sub

        ''' <summary>
        ''' XPath selection expression
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
           <LineCount(2)> _
           <Width(500)> _
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
        Public Property IsHtmlContent() As Boolean

        ''' <summary>
        ''' Selection of whole tree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("SubSelects")> _
        Public Property SelectTree() As Boolean

        ''' <summary>
        ''' Sub-selection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("SubSelects")> _
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
                Return Me.SelectNavigate(navigator)
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
            If Not String.IsNullOrEmpty(source) And Not String.IsNullOrEmpty(Me._SelectExpression) Then
                Dim navigator As XPathNavigator = Me.GetNavigable(source).CreateNavigator
                Return Me.SelectNavigate(navigator)
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
                    multiString.Add(result.Value)
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
                    For Each subSelect As KeyValuePair(Of String, XPathInfo) In Me._SubSelects
                        Dim objSubResult As Object = subSelect.Value.SelectNavigate(result)
                        If objSubResult IsNot Nothing Then
                            Dim subResult As String = DirectCast(objSubResult, String)
                            currentDico(subSelect.Key) = subResult
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

        ''' <summary>
        ''' Transforms the parameter string into a navigable xpath object
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNavigable(ByVal source As String) As IXPathNavigable
            If _IsHtmlContent Then
                Dim doc As New HtmlAgilityPack.HtmlDocument()
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


