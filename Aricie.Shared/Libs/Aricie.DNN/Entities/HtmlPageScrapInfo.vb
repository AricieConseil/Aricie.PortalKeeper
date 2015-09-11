Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Filtering
Imports Aricie.Collections
Imports Aricie.ComponentModel
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports System.Globalization
Imports Aricie.DNN.Services

Namespace Entities

    Public Enum PagerQueryType
        PageNb
        ResultIndex
    End Enum

    Public Enum HttpClientMode
        WebClient
        Browser
    End Enum


    <ActionButton(IconName.Search, IconOptions.Normal)> _
    <Serializable()> _
    Public Class HtmlPageScrapInfo
        Inherits NamedConfig

        <ExtendedCategory("Url")> _
        Public Property Requests As New OneOrMore(Of HtmlRequestInfo)

        <ExtendedCategory("Pager")> _
        Public Property UsePager As Boolean

        <ConditionalVisible("UsePager", False, True)> _
        <ExtendedCategory("Pager")> _
        Public Property MaxNbPage As Integer = 0

        <ExtendedCategory("Pager")> _
        <ConditionalVisible("UsePager", False, True)> _
        Public Property QueryType As PagerQueryType = PagerQueryType.PageNb

        <ConditionalVisible("QueryType", False, True, PagerQueryType.ResultIndex)> _
      <ConditionalVisible("UsePager", False, True)> _
      <ExtendedCategory("Pager")> _
        Public Property PageSize As Integer = 20

        <ConditionalVisible("UsePager", False, True)> _
        <ExtendedCategory("Pager")> _
        Public Property QueryParameter As String = "page"

        'todo: remove that obsolete property
        <Browsable(False)>
        Public Property PageParameter As String
            Get
                Return QueryParameter
            End Get
            Set(value As String)
                QueryParameter = value
            End Set
        End Property

        <ConditionalVisible("UsePager", False, True)> _
        <ExtendedCategory("Pager")> _
        Public Property IncludeFirstPage As Boolean

        <ConditionalVisible("UsePager", False, True)> _
        <ExtendedCategory("Pager")> _
        Public Property ZeroBasedIndex As Boolean

        <ExtendedCategory("Scraping")> _
        Public Property XPath As New XPathInfo("//title", True, True)

        <ExtendedCategory("Detail")> _
        Public Property ScrapDetail As Boolean

        <ExtendedCategory("Detail")> _
         <ConditionalVisible("ScrapDetail", False, True)> _
        Public Property ScrapDetails As New SerializableList(Of HtmlScrapDetailRequestInfo)


        Public Overrides Function GetFriendlyDetails() As String
            Return String.Format("{4}{0}{1}{0}{2}{0}{3}", UIConstants.TITLE_SEPERATOR, _
                                 IIf(Me.UsePager, "max " & Me.MaxNbPage & " ", "No ").ToString() & "page" & IIf(Me.MaxNbPage > 1, "s", "").ToString(), _
                                IIf(Me.ScrapDetail, Me.ScrapDetails.Count().ToString(CultureInfo.InvariantCulture) & " ", "No ").ToString() & "detail scrap" & IIf(Me.MaxNbPage > 1, "s", "").ToString(), _
                                Me.Requests.One.ClientMode.ToString(), MyBase.GetFriendlyDetails())

        End Function

        <ExtendedCategory("Custom")> _
        Public Property Custom As New SerializableDictionary(Of String, String)

        '<ExtendedCategory("Custom")> _
        'Public Property CustomVars As New Variables()

    End Class

    <Serializable()> _
    Public Class HtmlRequestInfo

        Public Overridable Property Url As New CData("http://www.google.com")

        Public Property ClientMode As HttpClientMode = HttpClientMode.WebClient

        <ConditionalVisible("ClientMode", False, True, HttpClientMode.Browser)> _
        Public Property FrameId As String = ""


    End Class

    <Serializable()> _
 <DefaultProperty("FriendlyName")> _
    Public Class HtmlScrapRequestInfo
        Inherits HtmlRequestInfo

        Public Property XPath As New XPathInfo("//title", True, True)

    End Class

    Public Enum ScrapUrlMode
        Pattern
        Column
    End Enum


    <Serializable()> _
   <DefaultProperty("FriendlyName")> _
    Public Class HtmlScrapDetailRequestInfo
        Inherits HtmlScrapRequestInfo

        <SortOrder(0)> _
        Public Property UrlMode As ScrapUrlMode = ScrapUrlMode.Column

        <SortOrder(1)> _
        <ConditionalVisible("UrlMode", False, True, ScrapUrlMode.Pattern)> _
        Public Overrides Property Url As CData
            Get
                Return MyBase.Url
            End Get
            Set(value As CData)
                MyBase.Url = value
            End Set
        End Property


        <ConditionalVisible("UrlMode", False, True, ScrapUrlMode.Column)> _
        <SortOrder(1)> _
        Public Property DetailUrlColumn As String = "Url"

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                Return Join({Me.DetailUrlColumn, Me.ClientMode.ToString(), Me.FrameId, Me.XPath.Expression.FriendlyName}, UIConstants.TITLE_SEPERATOR)
            End Get
        End Property

        Public Function GetUrl(columns As IDictionary(Of String, String)) As String
            Select Case Me.UrlMode
                Case ScrapUrlMode.Column
                    If columns.ContainsKey(Me.DetailUrlColumn) Then
                        Return columns(Me.DetailUrlColumn)
                    End If
                Case ScrapUrlMode.Pattern
                    Dim atr As New AdvancedTokenReplace()
                    atr.SetObjectReplace(columns, "Result")
                    Return atr.ReplaceAllTokens(Me.Url.Value)
            End Select
            Throw New ApplicationException("Unable to build Scrap detail url")
        End Function


    End Class
End Namespace