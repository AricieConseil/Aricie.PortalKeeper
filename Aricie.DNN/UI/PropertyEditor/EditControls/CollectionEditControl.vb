Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.Skins.Controls
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
Imports Aricie.DNN.Security.Trial
Imports System.Linq
Imports System.Text

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

        Protected WithEvents cmdAddButton As CommandButton

        Protected WithEvents ctImportFile As System.Web.UI.HtmlControls.HtmlInputFile
        'Public WithEvents txtExportPath As TextBox
        Protected WithEvents cmdExportButton As CommandButton
        Protected WithEvents cmdCopyButton As CommandButton
        Protected WithEvents cmdImportButton As CommandButton

        Protected WithEvents ctlAddContainer As Control
        Protected WithEvents pnPager As Panel

        Protected WithEvents ctlPager As Pager
        Private _DeleteControls As New List(Of WebControl)



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

        'Private _IsPageRequest As Boolean

        'Private _ImageSections As List(Of Image)
        'Private _SectionHeads As List(Of SectionHeadControl)

        Private _PagedCollection As PagedCollection
        Private _ItemsDictionary As List(Of Element)

        Private _SelectorInfo As SelectorInfo

        Private _PagerDisplayFieldName As String = String.Empty

        'Private previousItemHeader As String = ""

        Private _headers As New Dictionary(Of Integer, HtmlGenericControl)

        Private _ItemIndex As Integer = -1
        Private _DataItem As Object

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
                Dim cookie As HttpCookie = HttpContext.Current.Response.Cookies(cookieName)
                If cookie IsNot Nothing Then
                    HttpContext.Current.Response.Cookies.Remove(cookieName)
                End If
                cookie = New HttpCookie(cookieName)
                cookie.Value = value.ToString(CultureInfo.InvariantCulture)
                cookie.Expires = Now.AddHours(1)
                Me.Page.Response.Cookies.Add(cookie)
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

        'Public ReadOnly Property ImageSections() As List(Of Image)
        '    Get
        '        If _ImageSections Is Nothing Then
        '            Me._ImageSections = New List(Of Image)
        '            Me._SectionHeads = New List(Of SectionHeadControl)
        '            FormHelper.FindSectionsUpRecursive(Me, _SectionHeads, _ImageSections)
        '        End If
        '        Return _ImageSections
        '    End Get
        'End Property

        'Public ReadOnly Property SectionHeads() As List(Of SectionHeadControl)
        '    Get
        '        If _SectionHeads Is Nothing Then
        '            Me._ImageSections = New List(Of Image)
        '            Me._SectionHeads = New List(Of SectionHeadControl)
        '            FormHelper.FindSectionsUpRecursive(Me, _SectionHeads, _ImageSections)
        '        End If
        '        Return _SectionHeads
        '    End Get
        'End Property

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

        Public Overrides Sub EnforceTrialMode(ByVal mode As TrialPropertyMode)
            MyBase.EnforceTrialMode(mode)
            If (mode And TrialPropertyMode.NoAdd) = TrialPropertyMode.NoAdd Then
                Me.cmdAddButton.Enabled = False
            End If
            If (mode And TrialPropertyMode.NoDelete) = TrialPropertyMode.NoDelete Then
                For Each deleteControl As WebControl In Me._DeleteControls
                    deleteControl.Enabled = False
                Next
            End If
        End Sub

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)
            EnsureChildControls()
            Page.RegisterRequiresControlState(Me)
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
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Private Sub RegisterControlForPostbackManagement(ctrl As Control)
            For Each c As Control In ctrl.Controls
                If TypeOf (c) Is INamingContainer OrElse TypeOf (c) Is IPostBackDataHandler OrElse TypeOf (c) Is IPostBackEventHandler Then
                    DotNetNuke.Framework.AJAX.RegisterPostBackControl(c)
                End If
            Next
            DotNetNuke.Framework.AJAX.RegisterPostBackControl(ctrl)
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

                    If (Not Me._NoAddition) AndAlso (Me._MaxItemNb = 0 OrElse Me.CollectionValue.Count <= Me._MaxItemNb) Then

                        Dim pnAdd As New Panel

                        pnAdd.CssClass = "AddPanel"
                        pnAdd.EnableViewState = False
                        Controls.Add(pnAdd)

                        If TypeOf Me.ParentField.DataSource Is IProviderContainer AndAlso Me._SelectorInfo IsNot Nothing Then
                            Me.addSelector = Me._SelectorInfo.BuildSelector(Me.ParentField)
                            pnAdd.Controls.Add(Me.addSelector)
                            Me.addSelector.DataBind()
                        End If

                        cmdAddButton = New CommandButton()
                        cmdAddButton.ID = "cmdAdd"
                        'cmdAddButton.CssClass = "dnnTertiaryAction"
                        pnAdd.Controls.Add(cmdAddButton)
                        cmdAddButton.DisplayLink = True
                        cmdAddButton.DisplayIcon = True
                        cmdAddButton.ImageUrl = "~/images/add.gif"
                        cmdAddButton.Text = Localization.GetString(Me.Name + "_AddNew", Me.LocalResourceFile)
                        cmdAddButton.Visible = Not HideAddButton


                        AddHandler cmdAddButton.Click, AddressOf AddClick

                        If Me._AddNewEntry Then
                            Me.ctlAddContainer = pnAdd
                            Me.CreateAddRow(pnAdd)
                        End If
                    End If


                    If Me._EnableExport AndAlso (Me.ParentAricieEditor Is Nothing OrElse Not Me.ParentAricieEditor.DisableExports) Then

                        Dim pnExport As New Panel()
                        pnExport.EnableViewState = False
                        pnExport.CssClass = "ExportPanel"
                        Me.Controls.Add(pnExport)

                        Dim divExport As New HtmlGenericControl("div")
                        pnExport.Controls.Add(divExport)

                        cmdExportButton = New CommandButton
                        cmdExportButton.ID = "cmdExport"
                        'cmdExportButton.CssClass = "dnnTertiaryAction"
                        divExport.Controls.Add(cmdExportButton)
                        cmdExportButton.DisplayLink = True
                        cmdExportButton.DisplayIcon = True
                        cmdExportButton.ImageUrl = "~/images/action_export.gif"
                        cmdExportButton.Text = Localization.GetString(Me.Name + "_Export", Me.LocalResourceFile)
                        AddHandler cmdExportButton.Click, AddressOf ExportClick

                        RegisterControlForPostbackManagement(cmdExportButton)


                        Dim divImport As New HtmlGenericControl("div")
                        pnExport.Controls.Add(divImport)

                        Dim lblImport As LabelControl = DirectCast(Me.ParentModule.LoadControl("~/controls/labelcontrol.ascx"), LabelControl)
                        lblImport.CssClass = "SubHead"
                        lblImport.ResourceKey = "ImportFile"
                        divImport.Controls.Add(lblImport)

                        ctImportFile = New HtmlInputFile
                        ctImportFile.ID = "ctImportFile"
                        divImport.Controls.Add(ctImportFile)

                        cmdImportButton = New CommandButton
                        cmdImportButton.ID = "cmdImport"
                        'cmdImportButton.CssClass = "dnnTertiaryAction"
                        divImport.Controls.Add(cmdImportButton)
                        cmdImportButton.DisplayLink = True
                        cmdImportButton.DisplayIcon = True
                        cmdImportButton.ImageUrl = "~/images/action_import.gif"
                        cmdImportButton.Text = Localization.GetString(Me.Name + "_Import", Me.LocalResourceFile)
                        AddHandler cmdImportButton.Click, AddressOf ImportClick

                        RegisterControlForPostbackManagement(cmdImportButton)
                    End If

                  


                End If
            End If
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)
            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
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

        Private Sub ReapeaterItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
            Try
                Select Case Me._DisplayStyle
                    Case CollectionDisplayStyle.List
                        DisplayListItem(e.Item)
                    Case CollectionDisplayStyle.Accordion
                        DisplayAccordionItem(e.Item)
                End Select
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(e.Item, ex)
            End Try
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Dim args As String() = Strings.Split(eventArgument, ClientAPI.COLUMN_DELIMITER)
            Dim index As Integer = -1

            If args.Length = 2 AndAlso Integer.TryParse(args(1), index) Then

                Dim header As HtmlGenericControl = Nothing
                If _headers.TryGetValue(index, header) Then
                    header.Attributes.Remove("onClick")
                    Dim el As Element = Me.ItemsDictionary(index)
                    Dim dataItem As Object = Me.PagedCollection.CurrentItems(index)
                    Me.DisplayItem(Me.ItemIndex(index), el.Container, dataItem)

                End If

            End If
        End Sub

        Private Sub RepeaterItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
            Try
                If e.CommandArgument.ToString <> "" Then
                    Dim commandIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                    Select Case e.CommandName
                        Case "Focus"
                            Dim toEditor As AriciePropertyEditorControl = Me.ParentAricieEditor.RootEditor
                            If toEditor IsNot Nothing Then
                                Dim path As String = Me.GetSubPath(commandIndex, Me.PagedCollection(commandIndex)).Replace("SubEntity.", "").Replace("SubEntity", "")
                                If Not String.IsNullOrEmpty(toEditor.SubEditorPath) Then
                                    path = toEditor.SubEditorPath & "."c & path
                                End If
                                toEditor.SubEditorFullPath = path
                            End If
                            toEditor.ItemChanged = True
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
                        Case "Export"
                            Dim singleList As ICollection = Me.ExportItem(commandIndex)
                            Me.Download(singleList)
                        Case Else

                    End Select
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try

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

                    Me.Download(Me.CollectionValue)
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

     
        Private Sub ExportClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If Me.Page.IsValid Then

                    Me.Download(CollectionValue)
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub ImportClick(ByVal sender As Object, ByVal e As EventArgs)
            Try




                Dim importCollect As ICollection
                If ctImportFile.PostedFile IsNot Nothing AndAlso ctImportFile.PostedFile.InputStream IsNot Nothing AndAlso ctImportFile.PostedFile.InputStream.Length > 0 Then
                    'DotNetNuke.Common.Utilities.FileSystemUtils.UploadFile(System.IO.Path.GetDirectoryName(path), ctImportFile.PostedFile, System.IO.Path.GetFileName(path))
                    Using objReader As New StreamReader(ctImportFile.PostedFile.InputStream)
                        importCollect = DirectCast(ReflectionHelper.Deserialize(Me.CollectionValue.GetType, objReader), ICollection)
                    End Using
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
                Else
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me.ParentModule, Localization.GetString("MissingFile.Message", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                    '    Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(GetExportFileName(), False)
                    '    importCollect = DirectCast(Aricie.DNN.Settings.SettingsController.LoadFileSettings(path, Me.CollectionValue.GetType, False, False), ICollection)
                End If


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


        'Private Sub CollectionEditControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '    If Me._Paged AndAlso Not Me.Page.IsPostBack Then
        '        If Me._IsPageRequest Then
        '            For Each sh As SectionHeadControl In Me.SectionHeads
        '                sh.IsExpanded = True
        '            Next
        '            For Each img As Image In Me.ImageSections
        '                Dim sectionName As String = img.ID.Substring(3)
        '                Dim propEdit As AriciePropertyEditorControl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePropertyEditorControl)(img)
        '                If propEdit IsNot Nothing Then
        '                    propEdit.VisibleCats.Add(sectionName)
        '                End If
        '            Next
        '            'todo: find out what's wrong in here (does not work)
        '            'Me.ctlPager.Focus()
        '            'Aricie.DNN.Services.DnnContext.Current.DnnPage.ScrollToControl(Me)
        '            'Me.Focus()

        '        End If
        '    End If
        'End Sub


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

        Private Function GetSubPath() As String
            Return Me.GetSubPath(Me._ItemIndex, Me._DataItem)
        End Function

        Private Function GetSubPath(index As Integer, dataItem As Object) As String
            Dim toReturn As New StringBuilder()
            Dim objParent As Control = Me
            Dim parents As New List(Of Control)
            Do
                objParent = Aricie.Web.UI.ControlHelper.FindControlRecursive(objParent, GetType(CollectionEditControl), GetType(PropertyEditorEditControl))
                If objParent IsNot Nothing Then
                    parents.Add(objParent)
                End If
            Loop Until (objParent Is Nothing OrElse TypeOf objParent Is CollectionEditControl)

            Dim previousCol As Boolean
            For i As Integer = parents.Count - 1 To 0 Step -1
                objParent = parents(i)
                If TypeOf objParent Is PropertyEditorEditControl Then
                    If Not previousCol Then
                        toReturn.Append(DirectCast(objParent, PropertyEditorEditControl).ParentAricieField.DataField)
                        toReturn.Append(".")
                    End If
                    previousCol = False
                Else
                    Dim cec As CollectionEditControl = DirectCast(objParent, CollectionEditControl)
                    toReturn.Append(cec.GetSubPath())
                    toReturn.Append(".")
                    previousCol = True
                End If
            Next

            toReturn.Append(Me.ParentAricieField.DataField)
            toReturn.Append("["c)
            If TypeOf Me.CollectionValue Is IDictionary Then
                toReturn.Append(ReflectionHelper.GetProperty(dataItem, "Key").ToString())
            Else
                toReturn.Append(index)
            End If
            toReturn.Append("]"c)

            Return toReturn.ToString
        End Function


        Private Sub AddButtons(actionContainer As Control, commandIndex As Integer)

            If Me.EditMode = PropertyEditorMode.Edit Then
                Dim plAction As New HtmlGenericControl("div")
                plAction.Attributes.Add("class", "ItemAction")

                actionContainer.Controls.Add(plAction)


                'SubPropertyEditor button

                Dim cmdEdit As New ImageButton
                cmdEdit.ID = "cmdFocus"
                plAction.Controls.Add(cmdEdit)
                With cmdEdit
                    .ImageUrl = "~/images/view.gif"
                    .AlternateText = Localization.GetString("Item_Focus", Me.LocalResourceFile)
                    .ToolTip = .AlternateText
                    .CommandName = "Focus"
                    .CommandArgument = commandIndex.ToString()
                End With



                If Me._EnableExport Then

                    Dim export As New ImageButton
                    export.ID = "cmdExport"
                    plAction.Controls.Add(export)
                    With export


                        .ImageUrl = "~/images/action_export.gif"
                        .AlternateText = Localization.GetString("Item_Export", Me.LocalResourceFile)
                        .ToolTip = .AlternateText
                        .CommandName = "Export"
                        .CommandArgument = commandIndex.ToString()
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

                    Dim firstItem As Boolean = (commandIndex = 0)
                    Dim lastItem As Boolean = (commandIndex = CollectionValue.Count - 1)

                    'If Me._Paged AndAlso Me.CollectionValue.Count > Me._PageSize Then
                    '    firstItem = item.ItemIndex + Me.PageIndex = 0
                    '    lastItem = (Me.PageIndex * Me._PageSize) + item.ItemIndex = CollectionValue.Count - 1
                    'Else
                    '    firstItem = item.ItemIndex = 0
                    '    lastItem = item.ItemIndex = CollectionValue.Count - 1
                    'End If
                    If Not firstItem Then
                        Dim up As New ImageButton
                        up.ID = "cmdUp"
                        plAction.Controls.Add(up)
                        With up
                            .ImageUrl = "~/images/up.gif"
                            .AlternateText = Localization.GetString("Item_Up", Me.LocalResourceFile)
                            .ToolTip = .AlternateText
                            .CommandName = "Up"
                            .CommandArgument = commandIndex.ToString()
                        End With


                    End If

                    If Not lastItem Then
                        Dim down As New ImageButton
                        down.ID = "cmdDown"
                        plAction.Controls.Add(down)
                        With down
                            .ImageUrl = "~/images/dn.gif"
                            .AlternateText = Localization.GetString("Item_Down", Me.LocalResourceFile)
                            .ToolTip = .AlternateText
                            .CommandName = "Down"
                            .CommandArgument = commandIndex.ToString()
                        End With

                    End If

                End If



                Dim cmdDelete As New ImageButton
                cmdDelete.ID = "cmdDelete"
                plAction.Controls.Add(cmdDelete)
                With cmdDelete


                    .ImageUrl = "~/images/delete.gif"
                    .AlternateText = Localization.GetString(Me.Name + "_Delete", Me.LocalResourceFile)
                    .ToolTip = .AlternateText
                    .CommandName = "Delete"
                    .CommandArgument = commandIndex.ToString()
                End With
                DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile))
                Me._DeleteControls.Add(cmdDelete)

            End If


        End Sub


        Private Sub DisplayListItem(item As RepeaterItem)

            Dim plItemContainer As New HtmlGenericControl("div")
            Dim oddCss, evenCSS As String
            If Me.ParentAricieEditor IsNot Nothing AndAlso Me.ParentAricieEditor.PropertyDepth Mod 2 = 0 Then
                oddCss = "ItemEven"
                evenCSS = "ItemOdd"
            Else
                oddCss = "ItemOdd"
                evenCSS = "ItemEven"
            End If
            plItemContainer.Attributes.Add("class", "ItemContainer " & IIf((item.ItemIndex Mod 2) = 0, oddCss, evenCSS).ToString())
            item.Controls.Add(plItemContainer)

            Dim commandIndex As Integer = Me.ItemIndex(item.ItemIndex)
            Me.AddButtons(plItemContainer, commandIndex)

            Me.DisplayItem(commandIndex, plItemContainer, item.DataItem)

        End Sub

        Private Sub DisplayAccordionItem(item As RepeaterItem)

            Dim h3 As New HtmlGenericControl("h3")
            Dim plItemContainer As New HtmlGenericControl("div")
            item.Controls.Add(h3)

            item.Controls.Add(plItemContainer)



            Dim accordionHeaderText As String = ReflectionHelper.GetFriendlyName(item.DataItem)

            accordionHeaderText = String.Format("{0}  -  {1}", (item.ItemIndex + 1).ToString(CultureInfo.InvariantCulture), accordionHeaderText)

            Dim commandIndex As Integer = Me.ItemIndex(item.ItemIndex)

            Me.AddButtons(h3, commandIndex)

            Dim headerLink As New HtmlGenericControl("a")

            h3.Controls.Add(headerLink)

            headerLink.InnerText = accordionHeaderText.Replace(" "c, ChrW(160))

            headerLink.Attributes.Add("href", String.Format("#{0}_{1}", Me.ID, commandIndex))

            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("cookieAccordion" & Me.ParentEditor.ClientID.GetHashCode())
            Dim cookieValue As Integer = -1

            If cookie IsNot Nothing Then
                Integer.TryParse(cookie.Value, cookieValue)
            End If

            If cookieValue <> item.ItemIndex Then

                Globals.SetAttribute(headerLink, "onClick", "dnn.vars=null;" & ClientAPI.GetPostBackClientHyperlink(Me, "expand" & ClientAPI.COLUMN_DELIMITER & item.ItemIndex))
                _headers(item.ItemIndex) = headerLink
            Else

                Me.DisplayItem(commandIndex, plItemContainer, item.DataItem)


            End If

            Me.ItemsDictionary.Add(New Element(accordionHeaderText, plItemContainer))



        End Sub

        Private Sub DisplayItem(index As Integer, plItemContainer As Control, item As Object)
            Me._ItemIndex = index
            Me._DataItem = item
            Me.CreateRow(plItemContainer, item)
            Dim emptyDiv As New HtmlGenericControl("div")
            emptyDiv.Attributes.Add("class", "clear")
            plItemContainer.Controls.Add(emptyDiv)
        End Sub

        Private Sub Download(value As ICollection)

            Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(GetExportFileName(), False)
            Aricie.DNN.Settings.SettingsController.SaveFileSettings(path, value, False)
            Aricie.Services.FileHelper.DownloadFile(path, Me.Page.Response, Me.Page.Server)
        End Sub

        Private Function GetExportFileName() As String
            Dim prefix As String = ReflectionHelper.GetCollectionFileName(Me.CollectionValue)
            Return prefix & ".xml"
        End Function


#End Region



        'Protected Overrides Sub LoadControlState(ByVal savedState As Object)

        '    Dim state As Pair = CType(savedState, Pair)

        '    Me._PageIndex = CInt(state.Second)

        '    MyBase.LoadControlState(state.First)
        'End Sub

        'Protected Overrides Function SaveControlState() As Object
        '    Dim state As Pair = New Pair(MyBase.SaveControlState(), Me._PageIndex)
        '    Return state
        'End Function

    End Class
End Namespace
