Imports System
Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Reflection


Namespace Web.UI.Controls
    ''' <summary>
    ''' Pager WebControl with multiple capabilities such as thesaurus type pages.
    ''' </summary>
    <ToolboxData("<{0}:Pager runat=""server""></{0}:Pager>")> _
    Public Class Pager
        Inherits WebControl
        Implements IPostBackEventHandler
        Implements INamingContainer

#Region "Life Cycle"
        Protected Overloads Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)
            Page.RegisterRequiresControlState(Me)
        End Sub

        Protected Overloads Overrides Function SaveControlState() As Object
            Dim objState As Object() = New Object(1) {}
            objState(0) = CurrentIndex
            objState(1) = PageSize

            Return objState
        End Function

        Protected Overloads Overrides Sub LoadControlState(ByVal state As Object)
            Dim savedState As Object() = DirectCast(state, Object())
            CurrentIndex = CInt(savedState(0))
            PageSize = CInt(savedState(1))
        End Sub

        Protected Overloads Overrides Sub Render(ByVal writer As HtmlTextWriter)

            If Page IsNot Nothing Then
                Page.VerifyRenderingInServerForm(Me)
            End If

            If Me.PageCount > Me.SmartShortCutThreshold AndAlso GenerateSmartShortCuts Then
                CalculateSmartShortcutAndFillList()
            End If

            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "3")
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "1")
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
            writer.AddAttribute(HtmlTextWriterAttribute.[Class], "PagerContainerTable")
            If RTL Then
                writer.AddAttribute(HtmlTextWriterAttribute.Dir, "rtl")
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Table)
            writer.RenderBeginTag(HtmlTextWriterTag.Tr)

            If GeneratePagerInfoSection Then
                writer.AddAttribute(HtmlTextWriterAttribute.[Class], "PagerInfoCell")
                writer.RenderBeginTag(HtmlTextWriterTag.Td)
                writer.Write((((PageClause & " ") + CurrentIndex.ToString() & " ") + OfClause & " ") + PageCount.ToString())
                writer.RenderEndTag()
            End If

            If GenerateFirstLastSection AndAlso CurrentIndex <> 1 Then
                writer.Write(RenderFirst())
            End If

            If CurrentIndex <> 1 Then
                writer.Write(RenderBack())
            End If

            If CurrentIndex < CompactModePageCount Then

                If CompactModePageCount > PageCount Then
                    CompactModePageCount = PageCount
                End If

                For i As Integer = 1 To CompactModePageCount
                    If i = CurrentIndex Then
                        writer.Write(RenderCurrent())
                    Else
                        writer.Write(RenderOther(i))
                    End If
                Next


                RenderSmartShortCutByCriteria(CompactModePageCount, True, writer)
            ElseIf CurrentIndex >= CompactModePageCount AndAlso CurrentIndex < NormalModePageCount Then

                If NormalModePageCount > PageCount Then
                    NormalModePageCount = PageCount
                End If

                For i As Integer = 1 To NormalModePageCount
                    If i = CurrentIndex Then
                        writer.Write(RenderCurrent())
                    Else
                        writer.Write(RenderOther(i))
                    End If
                Next


                RenderSmartShortCutByCriteria(NormalModePageCount, True, writer)
            ElseIf CurrentIndex >= NormalModePageCount Then
                Dim gapValue As Integer = CInt(NormalModePageCount / 2)
                Dim leftBand As Integer = CurrentIndex - gapValue
                Dim rightBand As Integer = CurrentIndex + gapValue


                RenderSmartShortCutByCriteria(leftBand, False, writer)

                Dim i As Integer = leftBand
                While (i < rightBand + 1) AndAlso i < PageCount + 1
                    If i = CurrentIndex Then
                        writer.Write(RenderCurrent())
                    Else
                        writer.Write(RenderOther(i))
                    End If
                    i += 1
                End While

                If rightBand < Me.PageCount Then

                    RenderSmartShortCutByCriteria(rightBand, True, writer)
                End If
            End If

            If CurrentIndex <> PageCount Then
                writer.Write(RenderNext())
            End If

            If GenerateFirstLastSection AndAlso CurrentIndex <> PageCount Then
                writer.Write(RenderLast())
            End If

            If GenerateGoToSection Then
                writer.Write(RenderGoTo())
            End If

            writer.RenderEndTag()

            writer.RenderEndTag()

            If GenerateGoToSection Then
                writer.Write(RenderGoToScript())
            End If

            If GenerateHiddenHyperlinks Then
                writer.Write(RenderHiddenDiv())
            End If
        End Sub
#End Region

#Region "Command"

        Private Shared ReadOnly EventCommand As New Object()

        Public Custom Event Command As CommandEventHandler
            AddHandler(ByVal value As CommandEventHandler)
                Events.[AddHandler](EventCommand, value)
            End AddHandler
            RemoveHandler(ByVal value As CommandEventHandler)
                Events.[RemoveHandler](EventCommand, value)
            End RemoveHandler
            RaiseEvent()

            End RaiseEvent
        End Event

        Protected Overridable Sub OnCommand(ByVal e As CommandEventArgs)
            Dim clickHandler As CommandEventHandler = DirectCast(Events(EventCommand), CommandEventHandler)
            clickHandler.Invoke(Me, e)
        End Sub

        Private Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
            OnCommand(New CommandEventArgs(Me.UniqueID, Convert.ToInt32(eventArgument)))
        End Sub
#End Region

#Region "Fields and properties"

        Public Property DisplayFieldName As String = String.Empty

        Private _ReflectedProperty As PropertyInfo

        Private _Items As ArrayList


        Public WriteOnly Property Items As ICollection
            Set(value As ICollection)
                Me._Items = New ArrayList(value)
            End Set
        End Property






        ''' <summary>
        ''' Gets or sets total number of rows.
        ''' </summary>
        Private _itemCount As Double
        <Browsable(False)> _
        Public Property ItemCount() As Double
            Get
                Return _itemCount
            End Get
            Set(ByVal value As Double)
                _itemCount = value

                Dim divide As Double = ItemCount / PageSize
                Dim ceiled As Double = System.Math.Ceiling(divide)
                PageCount = Convert.ToInt32(ceiled)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets current page index.
        ''' </summary>
        Private _currentIndex As Integer = 1
        <Browsable(False)> _
        Public Property CurrentIndex() As Integer
            Get
                Return _currentIndex
            End Get
            Set(ByVal value As Integer)
                _currentIndex = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets page size (results per page).
        ''' </summary>
        Private _pageSize As Integer = 15
        <Category("Behavioural")> _
        Public Property PageSize() As Integer
            Get
                Return _pageSize
            End Get
            Set(ByVal value As Integer)
                _pageSize = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the total number of pages.
        ''' </summary>
        Private _pageCount As Integer
        <Browsable(False)> _
        Private Property PageCount() As Integer
            Get
                Return _pageCount
            End Get
            Set(ByVal value As Integer)
                _pageCount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value that indicates whether the Next and Last clause is rendered as UI on page.
        ''' </summary>
        Private _showFirstLast As Boolean = False
        <Category("Behavioural")> _
        Public Property GenerateFirstLastSection() As Boolean
            Get
                Return _showFirstLast
            End Get
            Set(ByVal value As Boolean)
                _showFirstLast = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value that indicates whether the SmartShortcuts are rendered as UI on page.
        ''' </summary>
        Private _enableSSC As Boolean = True
        <Category("Behavioural")> _
        Public Property GenerateSmartShortCuts() As Boolean
            Get
                Return _enableSSC
            End Get
            Set(ByVal value As Boolean)
                _enableSSC = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value that will be used to calculate SmartShortcuts.
        ''' </summary>
        Private _sscRatio As Double = 3.0R
        <Category("Behavioural")> _
        Public Property SmartShortCutRatio() As Double
            Get
                Return _sscRatio
            End Get
            Set(ByVal value As Double)
                _sscRatio = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets maximum number of SmartShortcuts that can be rendered.
        ''' </summary>
        Private _maxSmartShortCutCount As Integer = 6
        <Category("Behavioural")> _
        Public Property MaxSmartShortCutCount() As Integer
            Get
                Return _maxSmartShortCutCount
            End Get
            Set(ByVal value As Integer)
                _maxSmartShortCutCount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that to have the SmartShortcuts rendered, the page count must be greater that this value.
        ''' </summary>
        Private _sscThreshold As Integer = 30
        <Category("Behavioural")> _
        Public Property SmartShortCutThreshold() As Integer
            Get
                Return _sscThreshold
            End Get
            Set(ByVal value As Integer)
                _sscThreshold = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of rendered page numbers in compact mode.
        ''' </summary>
        Private _firstCompactedPageCount As Integer = 10
        <Category("Behavioural")> _
        Public Property CompactModePageCount() As Integer
            Get
                Return _firstCompactedPageCount
            End Get
            Set(ByVal value As Integer)
                _firstCompactedPageCount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of rendered page numbers in standard mode.
        ''' </summary>
        Private _notCompactedPageCount As Integer = 15
        <Category("Behavioural")> _
        Public Property NormalModePageCount() As Integer
            Get
                Return _notCompactedPageCount
            End Get
            Set(ByVal value As Integer)
                _notCompactedPageCount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that indicates whether Pager renders Alt tooltip.
        ''' </summary>
        Private _altEnabled As Boolean = True
        <Category("Behavioural")> _
        Public Property GenerateToolTips() As Boolean
            Get
                Return _altEnabled
            End Get
            Set(ByVal value As Boolean)
                _altEnabled = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that indicates whether Pager information cell is rendered.
        ''' </summary>
        Private _infoCellVisible As Boolean = True
        <Category("Behavioural")> _
        Public Property GeneratePagerInfoSection() As Boolean
            Get
                Return _infoCellVisible
            End Get
            Set(ByVal value As Boolean)
                _infoCellVisible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that indicats whether GoTo section is rendered.
        ''' </summary>
        Private _generateGoToSection As Boolean = False
        <Category("Behavioural")> _
        Public Property GenerateGoToSection() As Boolean
            Get
                Return _generateGoToSection
            End Get
            Set(ByVal value As Boolean)
                _generateGoToSection = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that indicates whether hidden hyperlinks should render.
        ''' </summary>
        Private _generateHiddenHyperlinks As Boolean = False
        <Category("Behavioural")> _
        Public Property GenerateHiddenHyperlinks() As Boolean
            Get
                Return _generateHiddenHyperlinks
            End Get
            Set(ByVal value As Boolean)
                _generateHiddenHyperlinks = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the hidden hyperlinks' QueryString parameter name.
        ''' </summary>
        Private _queryStringParameterName As String = "pagerControlCurrentPageIndex"
        <Category("Behavioural")> _
        Public Property QueryStringParameterName() As String
            Get
                Return _queryStringParameterName
            End Get
            Set(ByVal value As String)
                _queryStringParameterName = value
            End Set
        End Property

#End Region

#Region "Text"

        ''' <summary>
        ''' Gets or sets the text caption displayed as "go" in the pager control.
        ''' Default value: go
        ''' </summary>
        Private _GO As String = "go"
        <Category("Globalization")> _
        Public Property GoClause() As String
            Get
                Return _GO
            End Get
            Set(ByVal value As String)
                _GO = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "of" in the pager control.
        ''' Default value: of
        ''' </summary>
        Private _OF As String = "of"
        <Category("Globalization")> _
        Public Property OfClause() As String
            Get
                Return _OF
            End Get
            Set(ByVal value As String)
                _OF = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "from" in the pager control.
        ''' Default value: From
        ''' </summary>
        Private _FROM As String = "From"
        <Category("Globalization")> _
        Public Property FromClause() As String
            Get
                Return _FROM
            End Get
            Set(ByVal value As String)
                _FROM = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "page" in the pager control.
        ''' Default value: Page
        ''' </summary>
        Private _PAGE As String = "Page"
        <Category("Globalization")> _
        Public Property PageClause() As String
            Get
                Return _PAGE
            End Get
            Set(ByVal value As String)
                _PAGE = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "to" in the pager control.
        ''' Default value: to
        ''' </summary>
        Private _TO As String = "to"
        <Category("Globalization")> _
        Public Property ToClause() As String
            Get
                Return _TO
            End Get
            Set(ByVal value As String)
                _TO = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Showing Results" in the pager control.
        ''' Default value: Showing Results
        ''' </summary>
        Private _SHOWING_RESULT As String = "Showing Results"
        <Category("Globalization")> _
        Public Property ShowingResultClause() As String
            Get
                Return _SHOWING_RESULT
            End Get
            Set(ByVal value As String)
                _SHOWING_RESULT = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Show Result" in the pager control.
        ''' Default value: Show Result
        ''' </summary>
        Private _SHOW_RESULT As String = "Show Result"
        <Category("Globalization")> _
        Public Property ShowResultClause() As String
            Get
                Return _SHOW_RESULT
            End Get
            Set(ByVal value As String)
                _SHOW_RESULT = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "to First Page" in the pager control.
        ''' Default value: to First Page
        ''' </summary>
        Private _BACK_TO_FIRST As String = "to First Page"
        <Category("Globalization")> _
        Public Property BackToFirstClause() As String
            Get
                Return _BACK_TO_FIRST
            End Get
            Set(ByVal value As String)
                _BACK_TO_FIRST = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "to Last Page" in the pager control.
        ''' Default value: to Last Page
        ''' </summary>
        Private _GO_TO_LAST As String = "to Last Page"
        <Category("Globalization")> _
        Public Property GoToLastClause() As String
            Get
                Return _GO_TO_LAST
            End Get
            Set(ByVal value As String)
                _GO_TO_LAST = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Back to Page" in the pager control.
        ''' Default value: Back to Page
        ''' </summary>
        Private _BACK_TO_PAGE As String = "Back to Page"
        <Category("Globalization")> _
        Public Property BackToPageClause() As String
            Get
                Return _BACK_TO_PAGE
            End Get
            Set(ByVal value As String)
                _BACK_TO_PAGE = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Next to Page" in the pager control.
        ''' Default value: Next to Page
        ''' </summary>
        Private _NEXT_TO_PAGE As String = "Next to Page"
        <Category("Globalization")> _
        Public Property NextToPageClause() As String
            Get
                Return _NEXT_TO_PAGE
            End Get
            Set(ByVal value As String)
                _NEXT_TO_PAGE = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Last Page" in the pager control.
        ''' Default value: &gt;&gt;
        ''' </summary>
        Private _LAST As String = "&gt;&gt;"
        <Category("Globalization")> _
        Public Property LastClause() As String
            Get
                Return _LAST
            End Get
            Set(ByVal value As String)
                _LAST = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "First Page" in the pager control.
        ''' Default value: &lt;&lt;
        ''' </summary>
        Private _FIRST As String = "&lt;&lt;"
        <Category("Globalization")> _
        Public Property FirstClause() As String
            Get
                Return _FIRST
            End Get
            Set(ByVal value As String)
                _FIRST = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Previous Page" in the pager control.
        ''' Default value: &lt;
        ''' </summary>
        Private _previous As String = "&lt;"
        <Category("Globalization")> _
        Public Property PreviousClause() As String
            Get
                Return _previous
            End Get
            Set(ByVal value As String)
                _previous = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text caption displayed as "Next Page" in the pager control.
        ''' Default value: &gt;
        ''' </summary>
        Private _next As String = "&gt;"
        <Category("Globalization")> _
        Public Property NextClause() As String
            Get
                Return _next
            End Get
            Set(ByVal value As String)
                _next = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that indicates whether pager control should render RTL or LTR.
        ''' </summary>
        Private _rightToLeft As Boolean = False
        <Category("Globalization")> _
        Public Property RTL() As Boolean
            Get
                Return _rightToLeft
            End Get
            Set(ByVal value As Boolean)
                _rightToLeft = value
            End Set
        End Property


#End Region

#Region "Rendering"
        Private Function GenerateAltMessage(ByVal pageNumber As Integer) As String
            Dim altGen As New StringBuilder()
            altGen.Append(If(pageNumber = CurrentIndex, ShowingResultClause, ShowResultClause))
            altGen.Append(" ")
            altGen.Append(((pageNumber - 1) * PageSize) + 1)
            altGen.Append(" ")
            altGen.Append(ToClause)
            altGen.Append(" ")
            altGen.Append(If(pageNumber = PageCount, ItemCount, pageNumber * PageSize))
            altGen.Append(" ")
            altGen.Append(OfClause)
            altGen.Append(" ")
            altGen.Append(ItemCount)

            Return altGen.ToString()
        End Function

        Private Function GetAlternativeText(ByVal pageNumber As Integer) As String
            Return If(GenerateToolTips, String.Format(" title=""{0}""", GenerateAltMessage(pageNumber)), "")
        End Function

        Private Function RenderFirst() As String
            Dim templateCell As String = (("<td class=""PagerOtherPageCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" title=""" & " ") + BackToFirstClause & " " & """> ") + GetClause(1, FirstClause) & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, "1"))
        End Function

        Private Function RenderLast() As String
            Dim templateCell As String = (("<td class=""PagerOtherPageCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" title=""" & " ") + GoToLastClause & " " & """> ") + LastClause & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, PageCount.ToString()))
        End Function

        Private Function RenderBack() As String
            Dim templateCell As String = ((("<td class=""PagerOtherPageCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" title=""" & " ") + BackToPageClause & " ") + (CurrentIndex - 1).ToString() & """> ") + PreviousClause & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, (CurrentIndex - 1).ToString()))
        End Function

        Private Function RenderNext() As String
            Dim templateCell As String = ((("<td class=""PagerOtherPageCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" title=""" & " ") + NextToPageClause & " ") + (CurrentIndex + 1).ToString() & """> ") + NextClause & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, (CurrentIndex + 1).ToString()))
        End Function

        Private Function RenderCurrent() As String
            Return ("<td class=""PagerCurrentPageCell""><span class=""PagerHyperlinkStyle"" " & GetAlternativeText(CurrentIndex) & " ><strong> ") + GetClause(CurrentIndex, CurrentIndex.ToString()) & " </strong></span></td>"
        End Function

        Private Function RenderOther(ByVal pageNumber As Integer) As String
            Dim templateCell As String = ("<td class=""PagerOtherPageCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" " & GetAlternativeText(pageNumber) & " > ") + GetClause(pageNumber, pageNumber.ToString()) & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, pageNumber.ToString()))
        End Function

        Private Function RenderSSC(ByVal pageNumber As Integer) As String
            Dim templateCell As String = ("<td class=""PagerSSCCells""><a class=""PagerHyperlinkStyle"" href=""{0}"" " & GetAlternativeText(pageNumber) & " > ") + GetClause(pageNumber, pageNumber.ToString()) & " </a></td>"
            Return [String].Format(templateCell, Page.ClientScript.GetPostBackClientHyperlink(Me, pageNumber.ToString()))
        End Function

        Private Function RenderGoTo() As String
            Dim templateCell As String = "<td style=""padding:1px 1px 1px 1px;"" class=""PagerOtherPageCells""><div onclick=""handleGoToVisibility()"" class=""GoToLabel"">&nbsp;Go To&nbsp;</div><img id=""goto_img"" onclick=""handleGoToVisibility()"" src=""" & Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "ASPnetPagerV2_8.Images.arr_right.gif") & """ alt="""" class=""GoToArrow""/>&nbsp;<div id=""div_goto"" style=""display:none;""><select class=""GoToSelect"" name=""ddlTes"" id=""ddlTes"" onchange=""javascript:handleGoto(this);"">{0}</select></div></td>"
            Dim listItemTemplate As String = "<option {0} value=""{1}"">{2}</option>"

            Dim sb As New StringBuilder()
            For i As Integer = 1 To Me.PageCount
                sb.Append(String.Format(listItemTemplate, If(i = CurrentIndex, "selected=""selected"" class=""GoToSelectedOption""", ""), Page.ClientScript.GetPostBackClientHyperlink(Me, i.ToString()), i))
            Next
            Return String.Format(templateCell, sb.ToString())
        End Function

        Private Function RenderGoToScript() As String
            Dim sb As New StringBuilder()

            sb.Append(vbCr & vbLf & "                                function handleGoto(selectObj) {" & vbCr & vbLf & "                                    eval(selectObj.options[selectObj.selectedIndex].value);" & vbCr & vbLf & "                                }" & vbCr & vbLf & vbCr & vbLf & "                                function handleGoToVisibility() {" & vbCr & vbLf & "                                    var gotoElem = document.getElementById('div_goto');" & vbCr & vbLf & "                                    gotoElem.style.display = gotoElem.style.display == 'none' ? 'inline' : 'none';" & vbCr & vbLf & "                                    var gotoImg = document.getElementById('goto_img');" & vbCr & vbLf & "                                    " & vbCr & vbLf & "                                ")

            sb.AppendFormat("gotoImg.src = gotoElem.style.display == 'none' ? '{0}' : '{1}';", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "ASPnetPagerV2_8.Images.arr_right.gif"), Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "ASPnetPagerV2_8.Images.arr_left.gif"))
            sb.Append("}")



            Dim goToScript As String = "<script type=""text/javascript"">{0}</script>"

            Return String.Format(goToScript, sb.ToString())
        End Function

        Private Function RenderHiddenDiv() As String
            Dim regEx As System.Text.RegularExpressions.Regex
            Dim theURL As Uri = System.Web.HttpContext.Current.Request.Url
            Dim hasQueryStringParam As Boolean = If(Not String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables("QUERY_STRING")), True, False)
            Dim tempHyperlink As String = "<a href=""{0}"">page {1}</a>"
            Dim tempDiv As String = "<div style=""display:none;"">{0}</div>"
            Dim sb As New StringBuilder()

            If hasQueryStringParam AndAlso System.Web.HttpContext.Current.Request.QueryString(Me.QueryStringParameterName) IsNot Nothing Then
                regEx = New Regex(Me.QueryStringParameterName & "\=\d*", RegexOptions.Compiled Or RegexOptions.Singleline)
                For i As Integer = 0 To Me.NormalModePageCount - 1
                    sb.Append(String.Format(tempHyperlink, regEx.Replace(theURL.ToString(), (Me.QueryStringParameterName & "=") & (i + Me.CurrentIndex)), i + Me.CurrentIndex))
                Next
            Else
                Dim qsParameterName As String = ""
                For i As Integer = 0 To Me.NormalModePageCount - 1
                    qsParameterName = String.Format("{0}={1}", Me.QueryStringParameterName, i + Me.CurrentIndex)
                    sb.Append(String.Format(tempHyperlink, If(hasQueryStringParam, (theURL.ToString() & "&") + qsParameterName, (theURL.ToString() & "?") + qsParameterName), i + Me.CurrentIndex))

                Next
            End If

            Return String.Format(tempDiv, sb.ToString())
        End Function
#End Region

#Region "Smart shortcut"

        Private _smartShortCutList As List(Of Integer)
        Private Property SmartShortCutList() As List(Of Integer)
            Get
                Return _smartShortCutList
            End Get
            Set(ByVal value As List(Of Integer))
                _smartShortCutList = value
            End Set
        End Property

        Private Sub CalculateSmartShortcutAndFillList()
            _smartShortCutList = New List(Of Integer)()
            Dim shortCutCount As Double = Me.PageCount * SmartShortCutRatio / 100
            Dim shortCutCountRounded As Double = System.Math.Round(shortCutCount, 0)
            If shortCutCountRounded > MaxSmartShortCutCount Then
                shortCutCountRounded = MaxSmartShortCutCount
            End If
            If shortCutCountRounded = 1 Then
                shortCutCountRounded += 1
            End If

            For i As Integer = 1 To CInt(shortCutCountRounded)
                Dim calculatedValue As Integer = CInt((System.Math.Round((Me.PageCount * (100 / shortCutCountRounded) * i / 100) * 0.1, 0) * 10))
                If calculatedValue >= Me.PageCount Then
                    Exit For
                End If
                SmartShortCutList.Add(calculatedValue)
            Next
        End Sub

        ' smart shortcut list calculator and list 

        Private Sub RenderSmartShortCutByCriteria(ByVal basePageNumber As Integer, ByVal getRightBand As Boolean, ByVal writer As HtmlTextWriter)
            If IsSmartShortCutAvailable() Then

                Dim lstSSC As List(Of Integer) = Me.SmartShortCutList

                Dim rVal As Integer = -1
                If getRightBand Then
                    For i As Integer = 0 To lstSSC.Count - 1
                        If lstSSC(i) > basePageNumber Then
                            rVal = i
                            Exit For
                        End If
                    Next
                    If rVal >= 0 Then
                        For i As Integer = rVal To lstSSC.Count - 1
                            If lstSSC(i) <> basePageNumber Then
                                writer.Write(RenderSSC(lstSSC(i)))
                            End If
                        Next
                    End If
                ElseIf Not getRightBand Then

                    For i As Integer = 0 To lstSSC.Count - 1
                        If basePageNumber > lstSSC(i) Then
                            rVal = i
                        End If
                    Next

                    If rVal >= 0 Then
                        For i As Integer = 0 To rVal
                            If lstSSC(i) <> basePageNumber Then
                                writer.Write(RenderSSC(lstSSC(i)))
                            End If
                        Next
                    End If
                End If
            End If
        End Sub

        Private Function IsSmartShortCutAvailable() As Boolean
            Return Me.GenerateSmartShortCuts AndAlso Me.SmartShortCutList IsNot Nothing AndAlso Me.SmartShortCutList.Count <> 0
        End Function
#End Region

#Region "HiddenDiv"

#End Region

#Region "FieldPager"



        Private Function GetClause(pageNumber As Integer, defaultClause As String) As String
            If Me.DisplayFieldName <> String.Empty AndAlso _Items IsNot Nothing Then
                Dim targetPageFirstItem As Object = Me._Items(Me._pageSize * (pageNumber - 1))
                If _ReflectedProperty Is Nothing Then
                    If Not Aricie.Services.ReflectionHelper.GetPropertiesDictionary(targetPageFirstItem.GetType).TryGetValue(Me.DisplayFieldName, Me._ReflectedProperty) Then
                        Throw New ApplicationException(String.Format("Pager's configured DisplayField does not correspond to a collection item property"))
                    End If
                End If
                Dim targetPageField As String = CType(_ReflectedProperty.GetValue(targetPageFirstItem, Nothing), String)
                If targetPageField <> String.Empty Then
                    Dim idx As Integer = 0
                    If pageNumber > 1 Then
                        Dim previousItem As Object = Me._Items((Me._pageSize * (pageNumber - 1)) - 1)
                        Dim previousItemField As String = CType(_ReflectedProperty.GetValue(previousItem, Nothing), String)
                        Dim minLength As Integer = Math.Min(targetPageField.Length, previousItemField.Length)
                        While idx < minLength - 1 AndAlso targetPageField(idx) = previousItemField(idx)
                            idx += 1
                        End While
                    End If
                    Return targetPageField.Substring(0, idx + 1)
                End If
            End If
            Return defaultClause
        End Function


#End Region
    End Class
End Namespace
