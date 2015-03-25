Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.Services.Localization
Imports Aricie.Services
Imports Aricie.UI.WebControls.EditControls
Imports Aricie.Web.UI.Controls
Imports System.Globalization
Imports DotNetNuke.UI.Utilities
Imports System.Web
Imports Aricie.DNN.UI.WebControls.AriciePropertyEditorControl
Imports Aricie.DNN.Services
Imports System.IO
Imports Aricie.DNN.Security.Trial
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

Namespace UI.WebControls.EditControls
    Public MustInherit Class CollectionEditControl
        Inherits AricieEditControl
        Implements INamingContainer, IPostBackEventHandler, IPathProvider



        Private Const PAGE_INDEX_KEY As String = "PageIndex"

#Region "Events"

        Public Event MoveUp(ByVal index As Integer)
        Public Event MoveDown(ByVal index As Integer)

#End Region

#Region "Private members"

        'Protected WithEvents dlContentList As DataList
        Protected WithEvents rpContentList As Repeater
        Protected WithEvents addSelector As SelectorControl

        Protected WithEvents cmdAddButton As IconActionButton
        Protected WithEvents cmdClearButton As IconActionButton


        Protected WithEvents ctImportFile As System.Web.UI.HtmlControls.HtmlInputFile
        'Public WithEvents txtExportPath As TextBox
        Protected WithEvents cmdExportButton As IconActionButton
        Protected WithEvents cmdCopyButton As IconActionButton
        Protected WithEvents cmdPasteButton As IconActionButton
        Protected WithEvents cmdImportButton As IconActionButton

        Protected WithEvents ctlAddContainer As Control
        Protected WithEvents pnPager As Panel

        Protected WithEvents ctlPager As Pager
        Private _DeleteControls As New List(Of WebControl)



        Private _Ordered As Boolean = True
        Private _AddEntry As Object
        Private _NoAddition As Boolean
        Private _NoDeletion As Boolean
        Private _MaxItemNb As Integer
        Private _AddNewEntry As Boolean
        Private _EnableExport As Boolean = True
        Private _HideAddButton As Boolean
        Protected ItemsReadOnly As Boolean


        Private _Paged As Boolean = True
        Private _PageSize As Integer = 30

        Private _DisplayStyle As Nullable(Of CollectionDisplayStyle)
        Private _PageIndex As Integer = -1

        'Private _IsPageRequest As Boolean

        'Private _ImageSections As List(Of Image)
        'Private _SectionHeads As List(Of SectionHeadControl)

        Private _PagedCollection As PagedCollection
        Private _ItemsDictionary As List(Of Element)

        Private _SelectorInfo As SelectorInfo

        Private _PagerDisplayFieldName As String = String.Empty

        'Private previousItemHeader As String = ""

        Private _headers As New Dictionary(Of Integer, WebControl)

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
                    Dim cookieName As String = "pagerIndex" & Me.ClientID.GetHashCode()
                    Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(cookieName)
                    If cookie IsNot Nothing Then
                        Integer.TryParse(cookie.Value, _PageIndex)
                    End If
                    If _PageIndex = -1 Then
                        _PageIndex = 0
                    ElseIf _PageIndex > Me.CollectionValue.Count \ _PageSize Then
                        Me.PageIndex = _PageIndex - 1
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
                Return _DisplayStyle.Value
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
                        Me._NoDeletion = collecAttribute.NoDeletion
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

            If Not _DisplayStyle.HasValue Then
                Dim objetType As Type = ReflectionHelper.GetCollectionElementType(Me.CollectionValue, False)
                'If objetType IsNot Nothing AndAlso ReflectionHelper.IsTrueReferenceType(objetType) AndAlso objetType IsNot GetType(CData) Then
                If objetType IsNot Nothing AndAlso (Not ReflectionHelper.IsSimpleType(objetType)) Then
                    Me._DisplayStyle = CollectionDisplayStyle.Accordion
                Else
                    Me._DisplayStyle = CollectionDisplayStyle.List
                End If
            End If
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

            AddFooter()

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

            If args.Length = 2 AndAlso args(0) = "navigate" AndAlso Integer.TryParse(args(1), index) Then

                Dim toEditor As AriciePropertyEditorControl = Me.ParentAricieEditor.RootEditor
                If toEditor IsNot Nothing Then
                    Dim path As String = Me.GetSubPath(index, Me.PagedCollection(index))
                    '.Replace("SubEntity.", "").Replace("SubEntity", "")
                    'If Not String.IsNullOrEmpty(toEditor.SubEditorPath) Then
                    '    path = toEditor.SubEditorPath & "."c & path
                    'End If
                    toEditor.SubEditorFullPath = path
                    toEditor.ItemChanged = True
                End If


            End If
        End Sub

        Private Sub RepeaterItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
            Try
                If e.CommandArgument.ToString <> "" Then
                    Dim commandIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                    Select Case e.CommandName
                        Case "Expand"
                            Dim header As WebControl = Nothing
                            If _headers.TryGetValue(commandIndex, header) Then
                                header.Attributes.Remove("onClick")
                                Dim el As Element = Me.ItemsDictionary(commandIndex)
                                Dim dataItem As Object = Me.PagedCollection.CurrentItems(commandIndex)
                                Me.DisplaySubItems(Me.ItemIndex(commandIndex), el.Container, dataItem)

                            End If
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
                            Me.ParentAricieEditor.RootEditor.ClearBackPath()
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
                        Case "Copy"
                            Me.Copy(Me.ExportItem(commandIndex))
                        Case "Export"
                            Dim singleList As ICollection = Me.ExportItem(commandIndex)
                            Me.Download(singleList)
                        Case "Copy"
                            Dim singleList As ICollection = Me.ExportItem(commandIndex)
                            Me.Copy(singleList)
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
                Page.Validate()
                If Me.Page.IsValid Then
                    Me.AddNewItem(Me.AddEntry)

                    addEvent.Value = Me.CollectionValue
                    addEvent.Changed = True
                    Me.OnValueChanged(addEvent)

                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try


        End Sub

        Private Sub ClearClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                Dim clearEvent As New PropertyEditorEventArgs(Me.Name)
                clearEvent.OldValue = New ArrayList(Me.CollectionValue)
                Page.Validate()
                If Me.Page.IsValid Then

                    For i As Integer = Me.CollectionValue.Count - 1 To 0 Step -1
                        Me.DeleteItem(i)
                    Next

                    clearEvent.Value = Me.CollectionValue
                    clearEvent.Changed = True
                    Me.OnValueChanged(clearEvent)
                    Me.ParentAricieEditor.RootEditor.ClearBackPath()
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub



        Private Sub CopyClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                Page.Validate()
                If Me.Page.IsValid Then

                    Me.Copy(Me.CollectionValue)
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub


        Private Sub ExportClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                Page.Validate()
                If Me.Page.IsValid Then

                    Me.Download(CollectionValue)
                End If

            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub PasteClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If CopiedCollection IsNot Nothing Then
                    ImportItems(CopiedCollection)
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
                    ImportItems(importCollect)
                Else
                    Me.ParentAricieEditor.DisplayLocalizedMessage("MissingFile.Message", ModuleMessage.ModuleMessageType.YellowWarning)
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
                    'accordion.Attributes.Add("data-activeaccordion", Me.GetCurrentAccordionIndex().ToString(CultureInfo.InvariantCulture))
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


        Private _CollectionSubPath As String = ""


        Private Function GetSubPath(index As Integer, dataItem As Object) As String
            Dim objToReturn As String = GetPath()
            objToReturn &= GetIndexPath(GetIndexKey(index, dataItem))
            Return objToReturn

        End Function

        Public Shared Function GetIndexPath(index As String) As String
            If index.IsNumber Then
                Return String.Format("[{0}]", index)
            Else
                Return String.Format("[""{0}""]", index)
            End If

        End Function


        Private Function GetIndexKey(index As Integer, dataitem As Object) As String
            If TypeOf Me.CollectionValue Is IDictionary Then
                Return ReflectionHelper.GetProperty(dataitem, "Key").ToString()
            Else
                Return index.ToString(CultureInfo.InvariantCulture)
            End If
        End Function


        Public Function GetPath() As String Implements IPathProvider.GetPath
            If _CollectionSubPath.IsNullOrEmpty Then
                Dim parentPath As String = GetParentPath(Me)
                If parentPath.IsNullOrEmpty() Then
                    If ParentAricieField.Editor Is Me AndAlso Me.ParentAricieField.DataField <> "SubEntity" AndAlso Me.ParentAricieField.DataField <> "ReadOnlySubEntity" Then
                        _CollectionSubPath = ParentAricieField.DataField
                    Else
                        _CollectionSubPath = ""
                    End If
                Else
                    If ParentAricieField.Editor Is Me AndAlso Me.ParentAricieField.DataField <> "SubEntity" AndAlso Me.ParentAricieField.DataField <> "ReadOnlySubEntity" Then
                        _CollectionSubPath = String.Format("{0}.{1}", parentPath, Me.ParentAricieField.DataField)
                    Else
                        _CollectionSubPath = parentPath
                    End If
                End If
            End If
            Return _CollectionSubPath
        End Function


        Public Shared Function GetParentPath(ctl As Control) As String
            Dim parentCt As IPathProvider = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of IPathProvider)(ctl)
            If (parentCt IsNot Nothing) Then
                Return parentCt.GetPath().Replace(".SubEntity", "").Replace("SubEntity", "")
            End If
            Return ""
        End Function


        Private Sub DisplayListItem(item As RepeaterItem)

            Dim idxKey As String = Me.GetIndexKey(Me.ItemIndex(item.ItemIndex), item.DataItem)
            Dim plItemContainer As New IndexedDiv(idxKey) 'New HtmlGenericControl("div")
            Dim oddCss, evenCSS As String
            If Me.ParentAricieEditor IsNot Nothing AndAlso Me.ParentAricieEditor.PropertyDepth Mod 2 = 0 Then
                oddCss = "ItemEven"
                evenCSS = "ItemOdd"
            Else
                oddCss = "ItemOdd"
                evenCSS = "ItemEven"
            End If
            plItemContainer.Attributes.Add("class", "ListItem ItemContainer " & IIf((item.ItemIndex Mod 2) = 0, oddCss, evenCSS).ToString())
            item.Controls.Add(plItemContainer)

            Dim commandIndex As Integer = Me.ItemIndex(item.ItemIndex)
            Me.AddItemButtons(plItemContainer, Nothing, commandIndex)

            Me.DisplaySubItems(commandIndex, plItemContainer, item.DataItem)

        End Sub

        Private Sub DisplayAccordionItem(item As RepeaterItem)

            Dim h3 As New HtmlGenericControl("h3")


            Dim idxKey As String = Me.GetIndexKey(Me.ItemIndex(item.ItemIndex), item.DataItem)
            Dim plItemContainer As New IndexedDiv(idxKey) 'New HtmlGenericControl("div")
            item.Controls.Add(h3)

            item.Controls.Add(plItemContainer)



            Dim accordionHeaderText As String = ReflectionHelper.GetFriendlyName(item.DataItem)

            Dim commandIndex As Integer = Me.ItemIndex(item.ItemIndex)

            accordionHeaderText = String.Format("{0} {2} {1}", (commandIndex + 1).ToString(CultureInfo.InvariantCulture), accordionHeaderText, UIConstants.TITLE_SEPERATOR)
            Dim accordeonSB = New StringBuilder()
            Dim lstTerms() As String = accordionHeaderText.Split(New String() {UIConstants.TITLE_SEPERATOR}, StringSplitOptions.None)

            For Each myAccordeonItem As String In lstTerms
                accordeonSB.AppendFormat("<span>{0}</span>", myAccordeonItem.Trim())
            Next

            accordionHeaderText = accordeonSB.ToString()
            Dim headerLink As New IconActionControl  'HtmlGenericControl("a")
            headerLink.EnableViewState = False

            If item.DataItem IsNot Nothing Then
                Dim objActionButtonInfo As ActionButtonInfo = ActionButtonInfo.FromMember(item.DataItem.GetType)
                If objActionButtonInfo IsNot Nothing Then
                    headerLink.ActionItem = objActionButtonInfo.IconAction
                End If
            End If


            h3.Controls.Add(headerLink)




            headerLink.Text = accordionHeaderText


            headerLink.Url = String.Format("#{0}_{1}", Me.ID, commandIndex)


            'Dim currentAccordionIndex = GetCurrentAccordionIndex()
            Dim displaySubItem As Boolean = True
            'If currentAccordionIndex <> item.ItemIndex AndAlso item.DataItem IsNot Nothing Then
            If item.DataItem IsNot Nothing Then
                Dim objDataItemType As Type = item.DataItem.GetType()
                If Not ReflectionHelper.IsSimpleType(objDataItemType) Then
                    If objDataItemType.IsGenericType AndAlso objDataItemType.GetGenericTypeDefinition Is GetType(KeyValuePair(Of ,)) Then
                        Dim valueType As Type = objDataItemType.GetGenericArguments()(1)
                        If valueType Is GetType(Object) Then
                            valueType = ReflectionHelper.GetProperty(objDataItemType, "Value", item.DataItem).GetType()
                        End If
                        If Not ReflectionHelper.IsSimpleType(valueType) Then
                            If Not (valueType.IsGenericType _
                                    AndAlso valueType.GetGenericTypeDefinition Is GetType(List(Of )) _
                                    AndAlso ReflectionHelper.IsSimpleType(valueType.GetGenericArguments()(0))) Then
                                displaySubItem = False
                            End If
                        End If
                    Else
                        displaySubItem = False
                    End If
                End If
            End If
            If Not displaySubItem Then

                Globals.SetAttribute(headerLink, "onClick", "dnn.vars=null;" & ClientAPI.GetPostBackClientHyperlink(Me, "navigate" & ClientAPI.COLUMN_DELIMITER & commandIndex))
                _headers(item.ItemIndex) = headerLink
            Else

                Me.DisplaySubItems(commandIndex, plItemContainer, item.DataItem)


            End If

            Me.AddItemButtons(h3, headerLink, commandIndex)

            Me.ItemsDictionary.Add(New Element(accordionHeaderText, plItemContainer))



        End Sub

        'Private Function GetCurrentAccordionIndex() As Integer
        '    Dim toReturn As Integer = -1
        '    Dim clientVarName As String = Me.GetAccordionIndexClientVarName()
        '    Dim advStringValue As String = DnnContext.Current.AdvancedClientVariable(clientVarName)
        '    If (Not String.IsNullOrEmpty(advStringValue)) Then
        '        Integer.TryParse(advStringValue, toReturn)
        '    End If
        '    Return toReturn
        'End Function


        'Private Function GetAccordionIndexClientVarName() As String
        '    Return Me.GetPath() & "-cookieAccordion"
        'End Function

        Private Sub DisplaySubItems(index As Integer, plItemContainer As Control, item As Object)
            Me._ItemIndex = index
            Me._DataItem = item
            Me.CreateRow(plItemContainer, item)
            Dim emptyDiv As New HtmlGenericControl("div")
            emptyDiv.Attributes.Add("class", "clear")
            plItemContainer.Controls.Add(emptyDiv)
        End Sub





        Private Sub AddItemButtons(actionContainer As Control, headerLink As Control, commandIndex As Integer)

            If Me.EditMode = PropertyEditorMode.Edit Then
                Dim plAction As New HtmlGenericControl("div")
                plAction.Attributes.Add("class", "ItemActions")

                actionContainer.Controls.Add(plAction)

                Dim sm As ScriptManager = DirectCast(DotNetNuke.Framework.AJAX.ScriptManagerControl(Me.Page), ScriptManager)
                'SubPropertyEditor button
                If headerLink IsNot Nothing Then

                    Dim toEditor As AriciePropertyEditorControl = Me.ParentAricieEditor.RootEditor
                    If toEditor IsNot Nothing Then
                        Dim path As String = Me.GetSubPath(commandIndex, Me.PagedCollection(commandIndex))
                        '.Replace("SubEntity.", "").Replace("SubEntity", "")
                        'If Not String.IsNullOrEmpty(toEditor.SubEditorPath) Then
                        '    path = toEditor.SubEditorPath & "."c & path
                        'End If

                        Dim newUrl As New UriBuilder(Me.Context.Request.Url)
                        Dim query As NameValueCollection = HttpUtility.ParseQueryString(newUrl.Query)
                        query(SubPathQuery) = path
                        newUrl.Query = query.ToString()

                        Dim cmdLink As New IconActionControl()
                        plAction.Controls.Add(cmdLink)
                        With cmdLink
                            .CssClass = "aricieAction"
                            .ActionItem.IconName = IconName.Link
                            .Url = newUrl.ToString()
                        End With
                    End If




                    Dim cmdFocus As New IconActionButton
                    plAction.Controls.Add(cmdFocus)
                    With cmdFocus
                        .ActionItem.IconName = IconName.SearchPlus
                        .CommandName = "Expand"
                        .CommandArgument = commandIndex.ToString()
                        '.Attributes.Add("onclick", String.Format("jQuery('#{0}').attr('onclick','');jQuery('#{0}').click();", headerLink.ClientID))
                        '   .Attributes.Add("onclick", String.Format("jQuery('#{0}').click(function(e){{return false;}});jQuery('#{0}').unbind('click');jQuery('#{0}').click();", headerLink.ClientID))
                        .Attributes.Add("onclick", "SelectAndActivateParentHeader(this);")
                        AddHandler cmdFocus.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                    End With

                    sm.RegisterPostBackControl(cmdFocus)

                End If

                If Me._EnableExport Then


                    Dim cmdExport As New IconActionButton
                    plAction.Controls.Add(cmdExport)
                    With cmdExport
                        .ActionItem.IconName = IconName.Download
                        .CommandName = "Export"
                        .CommandArgument = commandIndex.ToString()
                    End With
                    AddHandler cmdExport.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                    sm.RegisterPostBackControl(cmdExport)


                    Dim cmdCopy As New IconActionButton
                    With cmdCopy
                        .ActionItem.IconName = IconName.FilesO
                        .CommandName = "Copy"
                        .CommandArgument = commandIndex.ToString()
                    End With
                    AddHandler cmdCopy.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                    plAction.Controls.Add(cmdCopy)

                    sm.RegisterPostBackControl(cmdCopy)

                End If

                If _Ordered Then

                    Dim firstItem As Boolean = (commandIndex = 0)
                    Dim lastItem As Boolean = (commandIndex = CollectionValue.Count - 1)



                    If Not lastItem Then
                        Dim cmdDown As New IconActionButton
                        plAction.Controls.Add(cmdDown)
                        With cmdDown
                            .ActionItem.IconName = IconName.ArrowDown
                            .CommandName = "Down"
                            .CommandArgument = commandIndex.ToString()
                        End With
                        AddHandler cmdDown.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                        sm.RegisterPostBackControl(cmdDown)

                    End If

                    If Not firstItem Then
                        Dim cmdUp As New IconActionButton
                        plAction.Controls.Add(cmdUp)
                        With cmdUp
                            .ActionItem.IconName = IconName.ArrowUp
                            .CommandName = "Up"
                            .CommandArgument = commandIndex.ToString()
                        End With
                        AddHandler cmdUp.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                        sm.RegisterPostBackControl(cmdUp)
                    End If

                End If
                If Not Me._NoDeletion Then
                    Dim cmdDelete As New IconActionButton
                    plAction.Controls.Add(cmdDelete)
                    With cmdDelete
                        .ActionItem.IconName = IconName.TrashO
                        .CommandName = "Delete"
                        .CommandArgument = commandIndex.ToString()
                    End With
                    AddHandler cmdDelete.Command, Sub(sender, e) RepeaterItemCommand(sender, New RepeaterCommandEventArgs(Nothing, sender, e))
                    sm.RegisterPostBackControl(cmdDelete)
                    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile))
                    Me._DeleteControls.Add(cmdDelete)
                End If


            End If


        End Sub

        Private Sub AddFooter()
            If Me.EditMode = PropertyEditorMode.Edit Then


                If Not Me._NoAddition OrElse Me._EnableExport Then

                    Dim pnAdd As New Panel
                    pnAdd.CssClass = "aricieActions dnnActions dnnClear"
                    pnAdd.EnableViewState = False
                    Controls.Add(pnAdd)
                    If (Not Me._NoAddition) AndAlso (Me._MaxItemNb = 0 OrElse Me.CollectionValue.Count <= Me._MaxItemNb) Then

                        If TypeOf Me.ParentField.DataSource Is IProviderContainer AndAlso Me._SelectorInfo IsNot Nothing Then
                            Dim ctrAddSelector As SelectorControl = Me._SelectorInfo.BuildSelector(Me.ParentField)
                            If ctrAddSelector.AllItems.Count > 0 Then
                                Me.addSelector = ctrAddSelector
                                pnAdd.Controls.Add(Me.addSelector)
                                Me.addSelector.DataBind()
                                If Me.addSelector.Items.Count < 2 Then
                                    Me.addSelector.Visible = False
                                End If
                            End If
                        End If

                        If Me._AddNewEntry OrElse TypeOf Me.CollectionValue Is IDictionary Then
                            Me.ctlAddContainer = pnAdd
                            Me.CreateAddRow(pnAdd)
                        End If

                        cmdAddButton = New IconActionButton()
                        pnAdd.Controls.Add(cmdAddButton)
                        cmdAddButton.ActionItem.IconName = IconName.Magic
                        cmdAddButton.Text = "Add " & Name
                        cmdAddButton.ResourceKey = "AddNew.Command"
                        cmdAddButton.Visible = Not HideAddButton
                        AddHandler cmdAddButton.Click, AddressOf AddClick


                        cmdClearButton = New IconActionButton()
                        pnAdd.Controls.Add(cmdClearButton)
                        cmdClearButton.ActionItem.IconName = IconName.TrashO
                        cmdClearButton.Text = "Clear " & Name
                        cmdClearButton.ResourceKey = "ClearItems.Command"
                        cmdClearButton.Visible = Not HideAddButton
                        AddHandler cmdClearButton.Click, AddressOf ClearClick


                       
                    End If


                    If Me._EnableExport AndAlso (Me.ParentAricieEditor Is Nothing OrElse Not Me.ParentAricieEditor.DisableExports) Then

                        If Me.CollectionValue.Count > 0 Then
                            cmdCopyButton = New IconActionButton
                            pnAdd.Controls.Add(cmdCopyButton)
                            cmdCopyButton.ActionItem.IconName = IconName.FilesO
                            cmdCopyButton.Text = "Copy " & Name
                            cmdCopyButton.ResourceKey = "Copy.Command"
                            AddHandler cmdCopyButton.Click, AddressOf CopyClick
                            RegisterControlForPostbackManagement(cmdCopyButton)
                        End If

                        If CopiedCollection IsNot Nothing AndAlso Me.CollectionValue.GetType().IsInstanceOfType(CopiedCollection) Then

                            cmdPasteButton = New IconActionButton
                            pnAdd.Controls.Add(cmdPasteButton)
                            cmdPasteButton.ActionItem.IconName = IconName.Clipboard
                            cmdPasteButton.Text = "Paste " & Name
                            cmdPasteButton.ResourceKey = "Paste.Command"
                            AddHandler cmdPasteButton.Click, AddressOf PasteClick
                            RegisterControlForPostbackManagement(cmdPasteButton)
                        End If



                        cmdExportButton = New IconActionButton
                        pnAdd.Controls.Add(cmdExportButton)
                        cmdExportButton.ActionItem.IconName = IconName.Download
                        cmdExportButton.Text = "Export " & Name
                        cmdExportButton.ResourceKey = "Export.Command"
                        AddHandler cmdExportButton.Click, AddressOf ExportClick
                        RegisterControlForPostbackManagement(cmdExportButton)



                        ctImportFile = New HtmlInputFile
                        ctImportFile.ID = "ctImportFile"
                        pnAdd.Controls.Add(ctImportFile)

                        cmdImportButton = New IconActionButton

                        pnAdd.Controls.Add(cmdImportButton)
                        cmdImportButton.ActionItem.IconName = IconName.Upload
                        cmdImportButton.Text = "Import " & Name
                        cmdImportButton.ResourceKey = "Import.Command"
                        AddHandler cmdImportButton.Click, AddressOf ImportClick

                        RegisterControlForPostbackManagement(cmdImportButton)
                    End If




                End If
            End If
        End Sub




        Private Property CopiedCollection As ICollection
            Get
                Return DirectCast(Me.Page.Session("AricieCopy"), ICollection)
            End Get
            Set(value As ICollection)
                Me.Page.Session("AricieCopy") = value
            End Set
        End Property



        Private Sub Copy(value As ICollection)

            CopiedCollection = ReflectionHelper.CloneObject(value)
            Me.ParentAricieEditor.ItemChanged = True
        End Sub

        Private Sub Download(value As ICollection)

            Dim path As String = Aricie.DNN.Services.FileHelper.GetAbsoluteMapPath(GetExportFileName(), False)
            Aricie.DNN.Settings.SettingsController.SaveFileSettings(path, value, False)
            Aricie.Services.FileHelper.DownloadFile(path, Me.Page.Response, Me.Page.Server)
        End Sub

        Private Sub ImportItems(items As ICollection)
            Dim addEvent As New PropertyEditorEventArgs(Me.Name)
            If TypeOf Me.CollectionValue Is IList AndAlso ReflectionHelper.CanCreateObject(Me.CollectionValue.GetType()) Then
                Dim oldValue As IList = DirectCast(ReflectionHelper.CreateObject(Me.CollectionValue.GetType()), IList)
                For Each obj As Object In Me.CollectionValue
                    oldValue.Add(obj)
                Next
                addEvent.OldValue = oldValue
            Else
                addEvent.OldValue = New ArrayList(Me.CollectionValue)
            End If
            
            Me.AddItems(items)

            'If Me._PageSize > 0 Then
            '    Me.PageIndex = CInt(Math.Floor((Me.CollectionValue.Count - 1) / PageSize))
            'End If

            'Me.BindData()

            addEvent.Value = Me.CollectionValue
            addEvent.Changed = True
            Me.OnValueChanged(addEvent)

        End Sub

        Protected Overridable Sub AddItems(items As ICollection)
            For Each newItem As Object In items
                Me.AddNewItem(newItem)
            Next
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
