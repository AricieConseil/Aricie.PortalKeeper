Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.Services.Localization
Imports Aricie.Services
Imports Aricie.UI.WebControls.EditControls
Imports Aricie.Web.UI
Imports DotNetNuke.UI.UserControls
Imports Aricie.Web.UI.Controls
Imports System.Globalization
Imports DotNetNuke.UI.Utilities
Imports Aricie.DNN.UI.Controls
Imports System.ComponentModel
Imports System.Reflection
Imports System.Web
Imports Aricie.DNN.UI.WebControls.AriciePropertyEditorControl
Imports Aricie.DNN.Services
Imports System.IO

Namespace UI.WebControls.EditControls

    Public MustInherit Class CollectionEditControl
        Inherits AricieEditControl
        Implements INamingContainer, System.Web.UI.IPostBackEventHandler


        Private Const PAGE_INDEX_KEY As String = "PageIndex"

#Region "Events"

        Public Event MoveUp(ByVal index As Integer)
        Public Event MoveDown(ByVal index As Integer)

#End Region

#Region "Private members"

        'Protected WithEvents dlContentList As DataList
        Protected WithEvents rpContentList As Repeater
        Protected WithEvents addSelector As SelectorControl

        Public WithEvents cmdAddButton As CommandButton

        Public WithEvents ctImportFile As System.Web.UI.HtmlControls.HtmlInputFile
        Public WithEvents txtExportPath As TextBox
        Public WithEvents cmdExportButton As CommandButton
        Public WithEvents cmdCopyButton As CommandButton
        Public WithEvents cmdImportButton As CommandButton

        Protected WithEvents ctlAddContainer As Control
        Protected WithEvents pnPager As Panel

        Public WithEvents ctlPager As Pager
        Public DeleteControls As New List(Of WebControl)



        Private _Ordered As Boolean = True
        Private _AddEntry As Object
        Private _NoAddition As Boolean
        Private _MaxItemNb As Integer
        Private _AddNewEntry As Boolean
        Private _EnableExport As Boolean = True
        Private _HideAddButton As Boolean
        Protected ItemsReadOnly As Boolean


        Private _Paged As Boolean = True
        Private _PageSize As Integer = 30

        Private _DisplayStyle As CollectionDisplayStyle = CollectionDisplayStyle.Accordion
        Private _PageIndex As Integer = -1

        Private _IsPageRequest As Boolean

        Private _ImageSections As List(Of Image)
        Private _SectionHeads As List(Of SectionHeadControl)

        Private _PagedCollection As PagedCollection
        Private _ItemsDictionary As List(Of Element)

        Private _SelectorInfo As SelectorInfo

        Private _PagerDisplayFieldName As String = String.Empty

#End Region

#Region "Public properties"

        Public ReadOnly Property ItemsDictionary As List(Of Element)
            Get
                If _ItemsDictionary Is Nothing Then
                    _ItemsDictionary = New List(Of Element)
                End If

                Return _ItemsDictionary
            End Get
        End Property


        Private _SortedCollection As ICollection
        Private _tempOriginal As Object

        Public ReadOnly Property CollectionValue() As ICollection
            Get
                'If Me._SortFieldName = String.Empty Then

                'Else
                '    If _SortedCollection Is Nothing OrElse _tempOriginal IsNot Me.Value Then
                '        Dim tempSortedCollection As New ArrayList(DirectCast(Me.Value, ICollection))
                '        tempSortedCollection.Sort(New Aricie.Business.Filters.SimpleComparer(Me._SortFieldName, System.ComponentModel.ListSortDirection.Ascending))
                '        _SortedCollection = tempSortedCollection
                '        _tempOriginal = Me.Value
                '    End If
                '    Return _SortedCollection
                'End If
                Return DirectCast(Me.Value, ICollection)
            End Get
        End Property

        Public ReadOnly Property PagedCollection() As PagedCollection
            Get
                If _PagedCollection Is Nothing Then
                    _PagedCollection = GetPagedCollection()
                End If
                Return _PagedCollection
            End Get
        End Property

        Public ReadOnly Property OldCollectionValue() As ICollection
            Get
                Return DirectCast(Me.OldValue, ICollection)
            End Get
        End Property

        Public Property AddEntry() As Object
            Get
                If _AddEntry Is Nothing Then
                    _AddEntry = GetNewItem()
                End If
                Return _AddEntry
            End Get
            Set(ByVal value As Object)
                _AddEntry = value
            End Set
        End Property

        Public Property PageIndex() As Integer
            Get
                EnsureChildControls()
                If _PageIndex = -1 Then
                    Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("pagerIndex" & Me.ClientID.GetHashCode())
                    If cookie IsNot Nothing Then
                        Integer.TryParse(cookie.Value, _PageIndex)
                    End If
                    If _PageIndex = -1 Then
                        _PageIndex = 0
                    End If
                End If
                Return _PageIndex
            End Get
            Set(ByVal value As Integer)
                Me._PageIndex = value
                Dim cookieName As String = "pagerIndex" & Me.ClientID.GetHashCode()
                Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(cookieName)
                If cookie IsNot Nothing Then
                    HttpContext.Current.Request.Cookies.Remove(cookieName) 'cookie.Name)
                End If
                cookie = New HttpCookie(cookieName)
                cookie.Value = value.ToString(CultureInfo.InvariantCulture)
                cookie.Expires = Now.AddHours(1)
                Me.Page.Request.Cookies.Add(cookie)
                Me.PagedCollection.PageIndex = value
            End Set
        End Property

        Protected ReadOnly Property ItemIndex(ByVal dataListItemIndex As Integer) As Integer
            Get
                If Me._Paged Then
                    Return Me.PageIndex * Me._PageSize + dataListItemIndex
                Else
                    Return dataListItemIndex
                End If
            End Get
        End Property

        Public ReadOnly Property ImageSections() As List(Of Image)
            Get
                If _ImageSections Is Nothing Then
                    Me._ImageSections = New List(Of Image)
                    Me._SectionHeads = New List(Of SectionHeadControl)
                    FormHelper.FindSectionsUpRecursive(Me, _SectionHeads, _ImageSections)
                End If
                Return _ImageSections
            End Get
        End Property

        Public ReadOnly Property SectionHeads() As List(Of SectionHeadControl)
            Get
                If _SectionHeads Is Nothing Then
                    Me._ImageSections = New List(Of Image)
                    Me._SectionHeads = New List(Of SectionHeadControl)
                    FormHelper.FindSectionsUpRecursive(Me, _SectionHeads, _ImageSections)
                End If
                Return _SectionHeads
            End Get
        End Property

        Public Property PageSize() As Integer
            Get
                Return _PageSize
            End Get
            Set(ByVal value As Integer)
                _PageSize = value
            End Set
        End Property

        Public Property HideAddButton() As Boolean
            Get
                Return _HideAddButton
            End Get
            Set(ByVal value As Boolean)
                _HideAddButton = value
            End Set
        End Property

        Public ReadOnly Property DisplayStyle As CollectionDisplayStyle
            Get
                Return _DisplayStyle
            End Get
        End Property

#End Region

#Region "overrides"


        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)
            Page.RegisterRequiresControlState(Me)
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Me.EnsureChildControls()
            MyBase.OnLoad(e)
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean
            Return False
        End Function

        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()

            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In CustomAttributes
                    If TypeOf attribute Is CollectionEditorAttribute Then
                        Dim collecAttribute As CollectionEditorAttribute = DirectCast(attribute, CollectionEditorAttribute)
                        Me._AddNewEntry = collecAttribute.ShowAddItem
                        Me._Ordered = collecAttribute.Ordered
                        Me._NoAddition = collecAttribute.NoAdd
                        Me._MaxItemNb = collecAttribute.MaxItemNb
                        Me._EnableExport = collecAttribute.EnableExport
                        Me._Paged = collecAttribute.Paged
                        Me._PageSize = collecAttribute.PageSize
                        Me._DisplayStyle = collecAttribute.DisplayStyle
                        Me._PagerDisplayFieldName = collecAttribute.PagerDisplayFieldName
                        Me.ItemsReadOnly = collecAttribute.ItemsReadOnly
                    ElseIf TypeOf attribute Is SelectorAttribute Then
                        Dim selAtt As SelectorAttribute = CType(attribute, SelectorAttribute)
                        Me._SelectorInfo = selAtt.SelectorInfo
                    End If

                Next
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Me.Name)
            args.Value = Me.CollectionValue
            args.OldValue = Me.OldCollectionValue
            'args.Changed = (Not args.Value Is args.OldValue)
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub



        Protected Overrides Sub CreateChildControls()

            Select Case Me._DisplayStyle
                Case CollectionDisplayStyle.Accordion

                    If Me.ParentAricieEditor IsNot Nothing Then
                        Me.ParentAricieEditor.LoadJQuery()
                    Else
                        FormHelper.LoadjQuery(Me.Page)
                    End If

                    If (Page IsNot Nothing) AndAlso (Page.Header IsNot Nothing) AndAlso NukeHelper.DnnVersion.Major < 6 Then
                        Dim cssId As String = "JqueryUiCss"

                        If Page.Header.FindControl(cssId) Is Nothing Then
                            Dim lnk As New HtmlControls.HtmlLink
                            lnk.ID = cssId
                            lnk.Href = "http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/themes/flick/jquery-ui.css"
                            lnk.Attributes.Add("type", "text/css")
                            lnk.Attributes.Add("rel", "stylesheet")
                            Page.Header.Controls.Add(lnk)

                        End If
                    End If

            End Select


            Me.BindData()


            If Me.EditMode = PropertyEditorMode.Edit Then


                If Not Me._NoAddition OrElse Me._EnableExport Then

                    If Me._EnableExport AndAlso (Me.ParentAricieEditor Is Nothing OrElse Not Me.ParentAricieEditor.DisableExports) Then

                        Dim pnExport As New Panel()

                        pnExport.EnableViewState = False
                        pnExport.CssClass = "ExportPanel"

                        Controls.Add(pnExport)

                        Dim sm As ScriptManager = DirectCast(DotNetNuke.Framework.AJAX.ScriptManagerControl(Me.Page), ScriptManager)


                        cmdExportButton = New CommandButton
                        cmdExportButton.ID = "cmdExport"
                        pnExport.Controls.Add(cmdExportButton)
                        cmdExportButton.DisplayLink = True
                        cmdExportButton.DisplayIcon = True
                        cmdExportButton.ImageUrl = "~/images/action_export.gif"
                        cmdExportButton.Text = Localization.GetString(Me.Name + "_Export", Me.LocalResourceFile)

                        AddHandler cmdExportButton.Click, AddressOf ExportClick
                        If sm IsNot Nothing Then
                            sm.RegisterPostBackControl(cmdExportButton)
                        End If


                        pnExport.Controls.Add(New LiteralControl("&nbsp;|&nbsp;"))

                        Dim spanExport As New HtmlGenericControl("span")
                        spanExport.Style.Add("display", "inline")
                        pnExport.Controls.Add(spanExport)
                        Dim spanLabel As New HtmlGenericControl("span")
                        spanExport.Controls.Add(spanLabel)

                        Dim labelPath As LabelControl = DirectCast(Me.ParentModule.LoadControl("~/controls/labelcontrol.ascx"), LabelControl)
                        labelPath.CssClass = "SubHead"
                        labelPath.ResourceKey = "ImportFile"

                        spanLabel.Controls.Add(labelPath)

                        txtExportPath = New TextBox
                        txtExportPath.Width = Unit.Pixel(280)
                        txtExportPath.MaxLength = 256
                        txtExportPath.Visible = False

                        spanExport.Controls.Add(txtExportPath)

                        ctImportFile = New HtmlInputFile
                        ctImportFile.ID = "ctImportFile"
                        spanExport.Controls.Add(ctImportFile)

                        cmdImportButton = New CommandButton
                        cmdImportButton.ID = "cmdImport"
                        pnExport.Controls.Add(cmdImportButton)
                        cmdImportButton.DisplayLink = True
                        cmdImportButton.DisplayIcon = True
                        cmdImportButton.ImageUrl = "~/images/action_import.gif"
                        cmdImportButton.Text = Localization.GetString(Me.Name + "_Import", Me.LocalResourceFile)

                        AddHandler cmdImportButton.Click, AddressOf ImportClick
                        If sm IsNot Nothing Then
                            sm.RegisterPostBackControl(cmdImportButton)
                        End If

                    End If

                    If (Not Me._NoAddition) AndAlso (Me._MaxItemNb = 0 OrElse Me.CollectionValue.Count <= Me._MaxItemNb) Then

                        Dim pnAdd As New Table()
                        Dim rAdd As New TableRow()
                        Dim cAdd As New TableCell()


                        pnAdd.EnableViewState = False
                        pnAdd.Rows.Add(rAdd)
                        rAdd.Cells.Add(cAdd)

                        Controls.Add(pnAdd)

                        If TypeOf Me.ParentField.DataSource Is IProviderContainer AndAlso Me._SelectorInfo IsNot Nothing Then
                            Me.addSelector = Me._SelectorInfo.BuildSelector(Me.ParentField)
                            cAdd.Controls.Add(Me.addSelector)
                            Me.addSelector.DataBind()
                        End If

                        cmdAddButton = New CommandButton()
                        cmdAddButton.ID = "cmdAdd"
                        cAdd.Controls.Add(cmdAddButton)
                        cmdAddButton.DisplayLink = True
                        cmdAddButton.DisplayIcon = True
                        cmdAddButton.ImageUrl = "~/images/add.gif"
                        cmdAddButton.Text = Localization.GetString(Me.Name + "_AddNew", Me.LocalResourceFile)
                        cmdAddButton.Visible = Not HideAddButton


                        AddHandler cmdAddButton.Click, AddressOf AddClick

                        If Me._AddNewEntry Then
                            Dim cAddNew As New TableCell()
                            rAdd.Cells.Add(cAddNew)
                            Me.ctlAddContainer = cAddNew
                            Me.CreateAddRow(cAddNew)
                        End If
                    End If


                End If
            End If
            MyBase.CreateChildControls()
        End Sub



        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)
            Me.SetExportFileName()
            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub


        Private Sub SetExportFileName()
            If Me.txtExportPath IsNot Nothing AndAlso Me.txtExportPath.Text = "" Then
                If Me.CollectionValue IsNot Nothing Then
                    Dim suffix As String = ".xml"
                    If Me.CollectionValue.GetType.IsGenericType Then
                        suffix = Me.CollectionValue.GetType.GetGenericArguments(0).Name.Replace("`", "") & suffix
                    Else
                        If Me.CollectionValue.Count > 0 Then
                            Dim collecEnum As IEnumerator = Me.CollectionValue.GetEnumerator
                            If collecEnum.MoveNext Then
                                Dim itemType As Type = collecEnum.Current.GetType
                                If itemType.IsGenericType Then
                                    Dim subType As Type = itemType.GetGenericArguments(0)
                                    suffix = subType.Name.Replace("`", "") & suffix
                                End If
                                suffix = itemType.Name.Replace("`", "") & suffix

                            End If
                        End If
                    End If
                    Me.txtExportPath.Text = Me.CollectionValue.GetType.Name.Replace("`"c, "") & "_" & suffix
                End If
            End If
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

#End Region

#Region "Dynamic Handlers"

        Private previousItemHeader As String = ""

        Private Sub ReapeaterItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)

            Try
                Dim emptyDiv As New HtmlGenericControl("div")
                emptyDiv.Attributes.Add("class", "clear")
                Select Case Me._DisplayStyle
                    Case CollectionDisplayStyle.List



                        Dim plItemContainer As New HtmlGenericControl("div")

                        Dim plItem As New HtmlGenericControl("div")
                        ' Dim emptyDiv As New HtmlGenericControl("div")
                        ' emptyDiv.Attributes.Add("class", "clear")
                        Dim plAction As New HtmlGenericControl("div")
                        plAction.Attributes.Add("class", "ItemAction")
                        plItemContainer.Controls.Add(plAction)
                        plItemContainer.Controls.Add(plItem)
                        plItemContainer.Controls.Add(emptyDiv)

                        Dim oddCss, evenCSS As String
                        If Me.ParentAricieEditor IsNot Nothing AndAlso Me.ParentAricieEditor.PropertyDepth Mod 2 = 0 Then
                            oddCss = "ItemEven"
                            evenCSS = "ItemOdd"
                        Else
                            oddCss = "ItemOdd"
                            evenCSS = "ItemEven"
                        End If

                        plItemContainer.Attributes.Add("class", "ItemContainer " & IIf((e.Item.ItemIndex Mod 2) = 0, oddCss, evenCSS).ToString())
                        e.Item.Controls.Add(plItemContainer)

                        If Me.EditMode = PropertyEditorMode.Edit Then
                            Dim commandIndex As String = Me.ItemIndex(e.Item.ItemIndex).ToString
                            Dim delete As New ImageButton
                            delete.ID = "cmdDelete"
                            plAction.Controls.Add(delete)
                            With delete
                                .ImageUrl = "~/images/delete.gif"
                                .AlternateText = Localization.GetString(Me.Name + "_Delete", Me.LocalResourceFile)
                                .ToolTip = .AlternateText
                                .CommandName = "Delete"
                                .CommandArgument = commandIndex
                            End With

                            DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(delete, Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile))
                            Me.DeleteControls.Add(delete)

                            If _Ordered Then

                                Dim firstItem As Boolean
                                Dim lastItem As Boolean

                                If Me._Paged AndAlso Me.CollectionValue.Count > Me._PageSize Then
                                    firstItem = e.Item.ItemIndex + Me.PageIndex = 0
                                    lastItem = (Me.PageIndex * Me._PageSize) + e.Item.ItemIndex = CollectionValue.Count - 1
                                Else
                                    firstItem = e.Item.ItemIndex = 0
                                    lastItem = e.Item.ItemIndex = CollectionValue.Count - 1
                                End If
                                If Not firstItem Then
                                    Dim up As New ImageButton
                                    up.ID = "cmdUp"
                                    plAction.Controls.Add(up)
                                    With up
                                        .ImageUrl = "~/images/up.gif"
                                        .AlternateText = Localization.GetString(Me.Name + "_Up", Me.LocalResourceFile)
                                        .ToolTip = up.AlternateText
                                        .CommandName = "Up"
                                        .CommandArgument = commandIndex
                                    End With
                                End If

                                If Not lastItem Then
                                    Dim down As New ImageButton
                                    down.ID = "cmdDown"
                                    plAction.Controls.Add(down)
                                    With down
                                        .ImageUrl = "~/images/dn.gif"
                                        .AlternateText = Localization.GetString(Me.Name + "_Down", Me.LocalResourceFile)
                                        .ToolTip = down.AlternateText
                                        .CommandName = "Down"
                                        .CommandArgument = commandIndex
                                    End With
                                End If

                            End If
                        End If

                        Me.CreateRow(plItem, e.Item.DataItem)


                    Case CollectionDisplayStyle.Accordion

                        Dim accordionHeaderText As String = ReflectionHelper.GetFriendlyName(e.Item.DataItem)
                        'Dim localizedname As String = Localization.GetString(accordionHeaderText, Me.LocalResourceFile)
                        'If localizedname <> "" AndAlso Not localizedname.ToLower.StartsWith("resx:") Then
                        '    accordionHeaderText = localizedname
                        'End If

                        If e.Item.ItemIndex > 0 Then
                            If Me.previousItemHeader.StartsWith(accordionHeaderText) Then
                                accordionHeaderText = String.Format("{0} - {1}", (e.Item.ItemIndex + 1).ToString(CultureInfo.InvariantCulture), accordionHeaderText)
                            End If
                        End If

                        'If accordionHeaderText = "" Then
                        '    accordionHeaderText = " "
                        'End If
                        Me.previousItemHeader = accordionHeaderText

                        Dim commandIndex As String = Me.ItemIndex(e.Item.ItemIndex).ToString


                        Dim h3 As New HtmlGenericControl("h3")
                        Dim headerLink As New HtmlGenericControl("a")
                        Dim plItemContainer As New HtmlGenericControl("div")



                        If Me.EditMode = PropertyEditorMode.Edit Then
                            Dim plAction As New HtmlGenericControl("div")
                            plAction.Attributes.Add("class", "ItemAction")

                            h3.Controls.Add(plAction)

                            If Me._EnableExport Then

                                Dim export As New ImageButton
                                export.ID = "cmdExport"
                                plAction.Controls.Add(export)
                                With export


                                    .ImageUrl = "~/images/action_export.gif"
                                    .AlternateText = Localization.GetString(Me.Name + "_Export", Me.LocalResourceFile)
                                    .ToolTip = .AlternateText
                                    .CommandName = "Export"
                                    .CommandArgument = commandIndex
                                End With

                                Dim sm As ScriptManager = DirectCast(DotNetNuke.Framework.AJAX.ScriptManagerControl(Me.Page), ScriptManager)
                                sm.RegisterPostBackControl(export)

                                'Dim copy As New ImageButton
                                'copy.ID = "cmdCopy"
                                'plAction.Controls.Add(copy)
                                'With copy


                                '    .ImageUrl = "~/images/copy.gif"
                                '    .AlternateText = Localization.GetString(Me.Name + "_Copy", Me.LocalResourceFile)
                                '    .ToolTip = .AlternateText
                                '    .CommandName = "Copy"
                                '    .CommandArgument = commandIndex
                                'End With
                            End If

                            If _Ordered Then

                                Dim firstItem As Boolean
                                Dim lastItem As Boolean

                                If Me._Paged AndAlso Me.CollectionValue.Count > Me._PageSize Then
                                    firstItem = e.Item.ItemIndex + Me.PageIndex = 0
                                    lastItem = (Me.PageIndex * Me._PageSize) + e.Item.ItemIndex = CollectionValue.Count - 1
                                Else
                                    firstItem = e.Item.ItemIndex = 0
                                    lastItem = e.Item.ItemIndex = CollectionValue.Count - 1
                                End If
                                If Not firstItem Then
                                    Dim up As New ImageButton
                                    up.ID = "cmdUp"
                                    plAction.Controls.Add(up)
                                    With up
                                        .ImageUrl = "~/images/up.gif"
                                        .AlternateText = Localization.GetString(Me.Name + "_Up", Me.LocalResourceFile)
                                        .ToolTip = .AlternateText
                                        .CommandName = "Up"
                                        .CommandArgument = commandIndex
                                    End With


                                End If

                                If Not lastItem Then
                                    Dim down As New ImageButton
                                    down.ID = "cmdDown"
                                    plAction.Controls.Add(down)
                                    With down
                                        .ImageUrl = "~/images/dn.gif"
                                        .AlternateText = Localization.GetString(Me.Name + "_Down", Me.LocalResourceFile)
                                        .ToolTip = .AlternateText
                                        .CommandName = "Down"
                                        .CommandArgument = commandIndex
                                    End With

                                End If

                            End If



                            Dim delete As New ImageButton
                            delete.ID = "cmdDelete"
                            plAction.Controls.Add(delete)
                            With delete


                                .ImageUrl = "~/images/delete.gif"
                                .AlternateText = Localization.GetString(Me.Name + "_Delete", Me.LocalResourceFile)
                                .ToolTip = .AlternateText
                                .CommandName = "Delete"
                                .CommandArgument = commandIndex
                            End With
                            DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(delete, Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile))
                            Me.DeleteControls.Add(delete)

                        End If

                        Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("cookieAccordion" & Me.ParentEditor.ClientID.GetHashCode())
                        Dim cookieValue As Integer = -1

                        If cookie IsNot Nothing Then
                            Integer.TryParse(cookie.Value, cookieValue)
                        End If

                        h3.Controls.Add(headerLink)

                        headerLink.InnerText = accordionHeaderText.Replace(" "c, ChrW(160))
                        headerLink.Attributes.Add("href", String.Format("#{0}_{1}", Me.ID, commandIndex))

                        e.Item.Controls.Add(h3)

                        e.Item.Controls.Add(plItemContainer)

                        If cookieValue <> e.Item.ItemIndex Then
                            Globals.SetAttribute(headerLink, "onClick", "dnn.vars=null;" & ClientAPI.GetPostBackClientHyperlink(Me, "expand" & ClientAPI.COLUMN_DELIMITER & e.Item.ItemIndex))
                            _headers(e.Item.ItemIndex) = headerLink
                        Else
                            Me.CreateRow(plItemContainer, e.Item.DataItem)

                            plItemContainer.Controls.Add(emptyDiv)
                        End If

                        Me.ItemsDictionary.Add(New Element(accordionHeaderText, plItemContainer))

                End Select
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(e.Item, ex)
            End Try


        End Sub

        Private _headers As New Dictionary(Of Integer, HtmlGenericControl)



        Private Sub RepeaterItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
            If e.CommandArgument.ToString <> "" Then
                Dim commandIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                Select Case e.CommandName
                    Case "Delete"

                        Dim delEvent As New PropertyEditorEventArgs(Me.Name)
                        'todo: should try with ReflectionHelper.CloneObject()
                        delEvent.OldValue = New ArrayList(Me.CollectionValue)

                        Me.DeleteItem(commandIndex)

                        'If Me._Paged AndAlso Me.CollectionValue.Count > Me._PageSize Then
                        '    Me.ctlPager.ItemCount = Me.CollectionValue.Count
                        '    Me.PageIndex = Math.Min(Me.PageIndex, CInt(Math.Floor((Me.CollectionValue.Count - 1) / PageSize)))
                        'End If

                        'Me.BindData()

                        delEvent.Value = Me.CollectionValue
                        delEvent.Changed = True
                        Me.OnValueChanged(delEvent)
                    Case "Up"
                        RaiseEvent MoveUp(commandIndex)
                        Dim addEvent As New PropertyEditorEventArgs(Me.Name)
                        addEvent.OldValue = Me.CollectionValue
                        addEvent.Value = Me.CollectionValue
                        addEvent.Changed = True
                        Me.OnValueChanged(addEvent)
                        'Me.BindData()

                    Case "Down"
                        RaiseEvent MoveDown(commandIndex)
                        Dim addEvent As New PropertyEditorEventArgs(Me.Name)
                        addEvent.OldValue = Me.CollectionValue
                        addEvent.Value = Me.CollectionValue
                        addEvent.Changed = True
                        Me.OnValueChanged(addEvent)
                        'Me.BindData()
                    Case "Export"
                        Dim singleList As ICollection = Me.ExportItem(commandIndex)
                        SetExportFileName()
                        Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(Me.txtExportPath.Text, False)
                        Aricie.DNN.Settings.SettingsController.SaveFileSettings(path, singleList, False)
                        DotNetNuke.Common.Utilities.FileSystemUtils.DownloadFile(path)
                    Case "Copy"
                        Dim singleList As ICollection = Me.ExportItem(commandIndex)
                        SetExportFileName()
                        Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(Me.txtExportPath.Text, False)
                        Aricie.DNN.Settings.SettingsController.SaveFileSettings(path, singleList, False)

                    Case Else

                End Select
            End If


        End Sub

        Private Sub AddClick(ByVal sender As Object, ByVal e As EventArgs)

            Try
                Dim addEvent As New PropertyEditorEventArgs(Me.Name)
                addEvent.OldValue = New ArrayList(Me.CollectionValue)

                If Me.Page.IsValid Then
                    Me.AddNewItem(Me.AddEntry)

                    'If Me._AddNewEntry Then
                    '    Me.ctlAddContainer.Controls.Clear()
                    '    Me._AddEntry = Nothing
                    'End If

                    'If Me._PageSize > 0 Then
                    '    Me.PageIndex = CInt(Math.Floor((Me.CollectionValue.Count - 1) / PageSize))
                    'End If

                    ''Me.BindData()

                    'If Me._AddNewEntry Then
                    '    Me.CreateAddRow(Me.ctlAddContainer)
                    'End If

                    addEvent.Value = Me.CollectionValue
                    addEvent.Changed = True
                    Me.OnValueChanged(addEvent)

                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try


        End Sub

        Private Sub CopyClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If Me.Page.IsValid Then

                    Me.Copy()
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Function Copy() As String
            SetExportFileName()
            Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(Me.txtExportPath.Text, False)
            Aricie.DNN.Settings.SettingsController.SaveFileSettings(path, Me.CollectionValue, False)
            Return path
        End Function

        Private Sub ExportClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If Me.Page.IsValid Then

                    Dim path As String = Me.Copy
                    DotNetNuke.Common.Utilities.FileSystemUtils.DownloadFile(path)
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub ImportClick(ByVal sender As Object, ByVal e As EventArgs)
            Try




                Dim importCollect As ICollection
                If ctImportFile.PostedFile IsNot Nothing AndAlso ctImportFile.PostedFile.InputStream IsNot Nothing Then
                    'DotNetNuke.Common.Utilities.FileSystemUtils.UploadFile(System.IO.Path.GetDirectoryName(path), ctImportFile.PostedFile, System.IO.Path.GetFileName(path))
                    Using objReader As New StreamReader(ctImportFile.PostedFile.InputStream)
                        importCollect = DirectCast(ReflectionHelper.Deserialize(Me.CollectionValue.GetType, objReader), ICollection)
                    End Using
                Else
                    SetExportFileName()
                    Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(Me.txtExportPath.Text, False)
                    importCollect = DirectCast(Aricie.DNN.Settings.SettingsController.LoadFileSettings(path, Me.CollectionValue.GetType, False, False), ICollection)
                End If



                Dim addEvent As New PropertyEditorEventArgs(Me.Name)
                addEvent.OldValue = New ArrayList(Me.CollectionValue)

                For Each newItem As Object In importCollect
                    Me.AddNewItem(newItem)
                Next

                'If Me._PageSize > 0 Then
                '    Me.PageIndex = CInt(Math.Floor((Me.CollectionValue.Count - 1) / PageSize))
                'End If

                'Me.BindData()

                addEvent.Value = Me.CollectionValue
                addEvent.Changed = True
                Me.OnValueChanged(addEvent)

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub ctlPager_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles ctlPager.Command

            Try
                Me.PageIndex = CInt(e.CommandArgument) - 1
                Me.BindData()
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try




        End Sub


        Private Sub CollectionEditControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Me._Paged AndAlso Not Me.Page.IsPostBack Then
                If Me._IsPageRequest Then
                    For Each sh As SectionHeadControl In Me.SectionHeads
                        sh.IsExpanded = True
                    Next
                    For Each img As Image In Me.ImageSections
                        Dim sectionName As String = img.ID.Substring(3)
                        Dim propEdit As AriciePropertyEditorControl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePropertyEditorControl)(img)
                        If propEdit IsNot Nothing Then
                            propEdit.VisibleCats.Add(sectionName)
                        End If
                    Next
                    'todo: find out what's wrong in here (does not work)
                    'Me.ctlPager.Focus()
                    'Aricie.DNN.Services.DnnContext.Current.DnnPage.ScrollToControl(Me)
                    'Me.Focus()

                End If
            End If
        End Sub


#End Region

#Region "MustOverrides"

        Protected MustOverride Sub CreateRow(ByVal container As Control, ByVal value As Object)

        Protected MustOverride Sub CreateAddRow(ByVal container As Control)

        Protected MustOverride Sub DeleteItem(ByVal index As Integer)

        Protected MustOverride Function ExportItem(index As Integer) As ICollection


        Protected Overridable Function GetNewItem() As Object
            Dim toReturn As Object = Nothing
            If TypeOf Me.ParentField.DataSource Is ITypedContainer Then
                toReturn = DirectCast(Me.ParentField.DataSource, ITypedContainer).GetNewItem(Me.ParentField.DataField)
            ElseIf TypeOf Me.ParentField.DataSource Is IProviderContainer AndAlso Me.addSelector IsNot Nothing Then
                toReturn = DirectCast(Me.ParentField.DataSource, IProviderContainer).GetNewItem(Me.ParentField.DataField, Me.addSelector.SelectedValue)
            End If
            If toReturn Is Nothing Then
                Dim objetType As Type = ReflectionHelper.GetCollectionElementType(Me.CollectionValue)
                toReturn = ReflectionHelper.CreateObject(objetType.AssemblyQualifiedName)
            End If

            Return toReturn
        End Function

        Protected MustOverride Sub AddNewItem(ByVal item As Object)

        Protected Overridable Function GetPagedCollection() As PagedCollection
            Return New PagedCollection(Me.CollectionValue, Me._PageSize, Me.PageIndex) ', Me._SortFieldName)
        End Function

#End Region

#Region "Private methods"




        Private Sub BindData()
            If Me.rpContentList Is Nothing Then
                Me.rpContentList = New Repeater()

                If Me._DisplayStyle = CollectionDisplayStyle.Accordion Then
                    Dim accordion As New HtmlGenericControl("div")
                    accordion.Attributes.Add("class", "aricie_pe_accordion-" & Me.ParentEditor.ClientID)
                    accordion.Attributes.Add("hash", Me.ParentEditor.ClientID.GetHashCode().ToString())

                    Me.Controls.Add(accordion)
                    accordion.Controls.Add(Me.rpContentList)
                Else
                    Me.Controls.Add(Me.rpContentList)
                End If

                AddHandler Me.rpContentList.ItemDataBound, AddressOf ReapeaterItemDataBound
                AddHandler Me.rpContentList.ItemCommand, AddressOf RepeaterItemCommand
            End If

            If Me.PagedCollection.IsPaginated Then
                Me.InjectPager()
            End If

            Me.rpContentList.DataSource = Me.PagedCollection

            Me.rpContentList.Controls.Clear()
            Me.rpContentList.ID = "rp" & Me.ParentField.ID & Me.PageIndex

            Me.rpContentList.DataBind()
        End Sub

        Private Sub InjectPager()
            If Me.pnPager Is Nothing Then
                Me.pnPager = New Panel
                Me.Controls.Add(Me.pnPager)
                Me.pnPager.ID = "pnPager" & Me.ParentField.ID
            End If

            If Me.ctlPager Is Nothing Then
                Me.ctlPager = New Pager
                Me.pnPager.Controls.Add(Me.ctlPager)
            End If
            Me.ctlPager.CurrentIndex = Me.PageIndex + 1
            Me.ctlPager.PageSize = Me._PageSize
            Me.ctlPager.ItemCount = Me.CollectionValue.Count
            If Me._PagerDisplayFieldName <> String.Empty Then
                Me.ctlPager.DisplayFieldName = Me._PagerDisplayFieldName
                Me.ctlPager.Items = Me.CollectionValue
            End If
        End Sub

#End Region

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Dim args As String() = Strings.Split(eventArgument, ClientAPI.COLUMN_DELIMITER)
            Dim index As Integer = -1

            If args.Length = 2 AndAlso Integer.TryParse(args(1), index) Then

                Dim header As HtmlGenericControl = Nothing
                If _headers.TryGetValue(index, header) Then
                    header.Attributes.Remove("onClick")
                    Dim el As Element = Me.ItemsDictionary(index)
                    Dim dataItem As Object = Me.PagedCollection.CurrentItems(index)
                    Me.CreateRow(el.Container, dataItem)
                    Dim emptyDiv As New HtmlGenericControl("div")
                    emptyDiv.Attributes.Add("class", "clear")
                    el.Container.Controls.Add(emptyDiv)
                End If

            End If
        End Sub

        Protected Overrides Sub LoadControlState(ByVal savedState As Object)

            Dim state As Pair = CType(savedState, Pair)

            Me._PageIndex = CInt(state.Second)

            MyBase.LoadControlState(state.First)
        End Sub

        Protected Overrides Function SaveControlState() As Object
            Dim state As Pair = New Pair(MyBase.SaveControlState(), Me._PageIndex)
            Return state
        End Function

    End Class
End Namespace
