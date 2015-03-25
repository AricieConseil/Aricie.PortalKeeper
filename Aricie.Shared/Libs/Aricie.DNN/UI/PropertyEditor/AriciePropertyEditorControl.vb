Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Reflection
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Framework
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Utilities
Imports Aricie.Services
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.Controls
Imports Aricie.DNN.Diagnostics
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel
Imports System.Globalization
Imports System.Text.RegularExpressions

<Assembly: WebResource("Aricie.DNN.AriciePropertyEditor.css", "text/css", PerformSubstitution:=True)> 
<Assembly: WebResource("Aricie.DNN.AriciePropertyEditorScripts.js", "text/javascript", PerformSubstitution:=True)> 

Namespace UI.WebControls


    Public Class AriciePropertyEditorControl
        Inherits PropertyEditorControl
        Implements IPostBackEventHandler, IScriptControl, IPathProvider

        Public Event Debug As EventHandler(Of DebugEventArgs)



#Region "Fields"


        Private Shared _VisibleCatsByType As New Dictionary(Of Type, List(Of String))
        Private _VisibleCats As List(Of String)
        Private _SectionsCollapsedByDefault As Boolean = True
        Private _SortedUnderLyingDataSource As PropertyInfo()
        Private _TrialStatus As TrialStatusInfo = TrialStatusInfo.NoTrial(TrialConfigInfo.Empty)
        Private _ValidationGroup As String
        Private _isHidden As Boolean

        Private _ItemChanged As Boolean
        Private _Groups As New Dictionary(Of String, KeyValuePair(Of Control, Control))
        Private _PropertyDepth As Integer
        Private _ParentEditor As PropertyEditorControl

        Private _ParentEditControl As EditControl
        Private _EnabledOnDemandSections As Boolean = True
        Private _RestrictedFields As New Dictionary(Of AricieFieldEditorControl, LimitationAttribute)
        Private _FieldsDictionary As FieldsHierarchy
        Private _Validated As Boolean = False
        Private _IsValid As Boolean = True

        Private _DisableExports As Boolean

        Private _JQueryUIVersion As String = ""
        Private _JQueryVersion As String = ""



        Private _PostBackFields As New List(Of String)

        Private _headerControl As PlaceHolder
        Private _ParentModule As PortalModuleBase
        Private _ParentField As AricieFieldEditorControl

        Private _OnDemandSections As List(Of String)

        Private _CurrentTab As Tab
        Public WithEvents ActionButton As ActionButtonInfo

        'Private Const LOAD_COUNTER As String = "LoadCounter"
        'Private Const PRERENDER_COUNTER As String = "PreRenderCounter"

        'Private ReadOnly Property SessionVisibleCatsDico() As SerializableDictionary(Of String, Boolean)
        '    Get
        '        Dim toReturn As SerializableDictionary(Of String, Boolean) = DirectCast(HttpContext.Current.Session.Item("SessionVisibleCatsDico"), SerializableDictionary(Of String, Boolean))
        '        If toReturn Is Nothing Then
        '            toReturn = New SerializableDictionary(Of String, Boolean)()
        '            HttpContext.Current.Session.Add("SessionVisibleCatsDico", toReturn)
        '        End If
        '        Return toReturn
        '    End Get
        'End Property

#End Region

#Region "Public Properties"

        Private _FriendlyName As String = ""

        Public ReadOnly Property FriendlyName() As String
            Get
                If _FriendlyName = "" Then
                    _FriendlyName = Me.DataSource.GetType.Name
                End If
                Return _FriendlyName
            End Get
        End Property

        Public ReadOnly Property FieldsDictionary As FieldsHierarchy
            Get
                If _FieldsDictionary Is Nothing Then
                    Dim objButtons As List(Of IEnumerable(Of ActionButtonInfo)) = Me.GetCommandButtons()
                    _FieldsDictionary = New FieldsHierarchy(Me.UnderlyingDataSource, objButtons, Me)
                End If
                Return _FieldsDictionary
            End Get
        End Property


        Private _ExceptionsToProcess As New List(Of Exception)

        Public Sub ProcessException(ex As Exception)
            _ExceptionsToProcess.Add(ex)
        End Sub


        Public Property PropertyDepth() As Integer
            Get
                Return _PropertyDepth
            End Get
            Set(ByVal value As Integer)
                _PropertyDepth = value
            End Set
        End Property



        Public Property EnabledOnDemandSections() As Boolean
            Get
                Return _EnabledOnDemandSections
            End Get
            Set(ByVal value As Boolean)
                _EnabledOnDemandSections = value
            End Set
        End Property




        Public WriteOnly Property IsHidden() As Boolean
            Set(value As Boolean)
                _isHidden = value
            End Set
        End Property


        Public ReadOnly Property ParentModule() As PortalModuleBase
            Get
                If _ParentModule Is Nothing Then
                    _ParentModule = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of PortalModuleBase)(Me)
                End If
                Return _ParentModule
            End Get
        End Property

        Public ReadOnly Property ParentAricieModule() As AriciePortalModuleBase
            Get
                Return DirectCast(Me.ParentModule, AriciePortalModuleBase)
            End Get
        End Property

        Public ReadOnly Property ParentEditor() As PropertyEditorControl
            Get
                If Me._ParentEditor Is Nothing Then
                    Dim parentControl As PropertyEditorControl = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of PropertyEditorControl)(Me)
                    If Not parentControl Is Nothing Then
                        Me._ParentEditor = parentControl
                    End If
                End If
                Return Me._ParentEditor
            End Get
        End Property

        Public ReadOnly Property ParentAricieEditor() As AriciePropertyEditorControl
            Get
                If TypeOf Me.ParentEditor Is AriciePropertyEditorControl Then
                    Return DirectCast(ParentEditor, AriciePropertyEditorControl)
                End If
                Return Nothing
            End Get
        End Property

        Private _IsRoot As Nullable(Of Boolean)

        Public ReadOnly Property IsRoot As Boolean
            Get
                If Not _IsRoot.HasValue Then
                    _IsRoot = RootEditor Is Me
                End If
                Return _IsRoot.Value
            End Get
        End Property

        Public ReadOnly Property RootEditor As AriciePropertyEditorControl
            Get
                Dim objParentEditor As AriciePropertyEditorControl = Me
                While objParentEditor.ParentAricieEditor IsNot Nothing
                    objParentEditor = objParentEditor.ParentAricieEditor
                End While
                Return objParentEditor
            End Get
        End Property

        Public ReadOnly Property ParentAricieField As AricieFieldEditorControl
            Get
                If _ParentField Is Nothing Then
                    _ParentField = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of AricieFieldEditorControl)(Me)
                End If
                Return _ParentField
            End Get
        End Property



        Public ReadOnly Property ParentEditControl As EditControl
            Get
                If _ParentEditControl Is Nothing Then
                    _ParentEditControl = Aricie.Web.UI.ControlHelper.FindParentControlRecursive(Of EditControl)(Me)
                End If
                Return _ParentEditControl
            End Get
        End Property


        Public ReadOnly Property OnDemandSections() As List(Of String)
            Get
                If _OnDemandSections Is Nothing Then
                    _OnDemandSections = New List(Of String)
                End If
                Return _OnDemandSections
            End Get
        End Property

        Public Property SectionsCollapsedByDefault() As Boolean
            Get
                Return _SectionsCollapsedByDefault
            End Get
            Set(ByVal value As Boolean)
                _SectionsCollapsedByDefault = value
            End Set
        End Property

        Public Property TrialStatus() As TrialStatusInfo
            Get
                Return _TrialStatus
            End Get
            Set(ByVal value As TrialStatusInfo)
                _TrialStatus = value
            End Set
        End Property

        Public Overloads ReadOnly Property IsValid() As Boolean
            Get
                If Not _Validated Then
                    Validate()
                End If
                Return _IsValid
            End Get
        End Property




        Public Property ValidationGroup() As String
            Get
                Return _ValidationGroup
            End Get
            Set(ByVal value As String)
                _ValidationGroup = value
            End Set
        End Property



        Public Property ItemChanged() As Boolean
            Get
                Return _ItemChanged
            End Get
            Set(ByVal value As Boolean)
                _ItemChanged = value
            End Set
        End Property

        Public Property DisableExports() As Boolean
            Get
                Return _DisableExports
            End Get
            Set(ByVal value As Boolean)
                _DisableExports = value
            End Set
        End Property

        Public Property JQueryVersion() As String
            Get
                Return _JQueryVersion
            End Get
            Set(ByVal value As String)
                _JQueryVersion = value
            End Set
        End Property

        Public Property JQueryUIVersion() As String
            Get
                Return _JQueryUIVersion
            End Get
            Set(ByVal value As String)
                _JQueryUIVersion = value
            End Set
        End Property


        Public Property SubEditorPath As String
            Get
                Return DnnContext.Current.AdvancedClientVariable(Me, "SubEditorPath")
            End Get
            Set(value As String)
                DnnContext.Current.AdvancedClientVariable(Me, "SubEditorPath") = value
                Me._CurrentPath = Nothing
            End Set
        End Property


        Public Property SubEditorFullPath As String
            Get
                Return DnnContext.Current.AdvancedClientVariable(Me, "SubEditorFullPath")
            End Get
            Set(value As String)
                DnnContext.Current.AdvancedClientVariable(Me, "SubEditorFullPath") = value
                SubEditorPath = value
                Dim subPathContainer = TryCast(DataSource, SubPathContainer)
                If (subPathContainer IsNot Nothing) Then
                    subPathContainer.OriginalPath = value
                    subPathContainer.Path = value
                End If
            End Set
        End Property


        Public Sub ClearBackPath()
            Dim strPath = SubEditorPath
            If Not strPath.IsNullOrEmpty() AndAlso strPath <> SubEditorFullPath Then
                SubEditorFullPath = strPath
                Me.ItemChanged = True
            End If
        End Sub

#End Region

#Region "Event Handlers"


        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
           
            MyBase.OnInit(e)
            If NukeHelper.DnnVersion.Major >= 6 Then
                ' rien à faire pour l'instant
                HelpStyle.CssClass += " dnnFormHelpContent"
            Else
                HelpStyle.CssClass += " Help"
            End If
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            '    If Me.ParentModule IsNot Nothing Then
            '        Me.ParentModule.AdvancedCounter(Me, LOAD_COUNTER) += 1
            '    End If

        End Sub


        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Try
                If Me.DataSource Is Nothing Then
                    Throw New ApplicationException("Data Source missing from property editor")
                End If
                Dim args As String() = Strings.Split(eventArgument, ClientAPI.COLUMN_DELIMITER)
                If args.Length = 2 AndAlso args(0) = "expand" Then
                    Dim sectionName As String = args(1)

                    Dim s As Section = Nothing
                    If Not Me.FieldsDictionary.Sections.TryGetValue(sectionName, s) Then
                        If _CurrentTab IsNot Nothing Then
                            _CurrentTab.Sections.TryGetValue(sectionName, s)
                        End If
                    End If

                    If s IsNot Nothing Then
                        Me.DisplayElement(s, False)
                    End If

                ElseIf args.Length = 2 AndAlso args(0) = "tabChange" Then
                    Dim tabName As String = args(1)


                    Dim t As Tab = Nothing
                    If Me.FieldsDictionary.Tabs.TryGetValue(tabName, t) Then
                        If _CurrentTab IsNot Nothing Then
                            If _CurrentTab Is t Then
                                Exit Sub
                            Else
                                _CurrentTab.HeaderLink.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(Me, "tabChange" & ClientAPI.COLUMN_DELIMITER & _CurrentTab.Name))
                                _CurrentTab.Loaded = False
                                _CurrentTab = t

                            End If
                        End If

                      
                        Dim tIndex As Integer = FieldsDictionary.Tabs.Select(Function(kvp, index) New With {.tab = kvp.Key, .index = index}).Where(Function(s) s.tab = tabName).Select(Function(s) s.index).first()

                        Dim clientVarName As String = GetTabIndexClientVarName() 'Me.GetPath() + "-cookieTab"
                        'Dim clientVarValueStr As String = DnnContext.Current.AdvancedClientVariable(clientVarName)
                        DnnContext.Current.AdvancedClientVariable(clientVarName) = tIndex.ToString(CultureInfo.InvariantCulture)

                        Me.FieldsDictionary.TabsContainer.SetAttribute("data-activetab", tIndex.ToString(CultureInfo.InvariantCulture))

                        'Me.FieldsDictionary.HideAllTabs()
                        t.Loaded = True
                        Me.DisplayElement(t, False)

                        t.HeaderLink.Attributes.Remove("onclick")
                        't.Loaded = True
                    End If

                End If
            Catch ex As Exception
                ProcessException(ex)
            End Try
        End Sub




        Protected Overrides Sub ListItemChanged(ByVal sender As Object, ByVal e As DotNetNuke.UI.WebControls.PropertyEditorEventArgs)
            MyBase.ListItemChanged(sender, e)
            ItemChanged = True
            If Me.ParentAricieEditor IsNot Nothing Then
                Me.ParentAricieEditor.ItemChanged = True
            End If
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            Try

                'needed to account for changes during control events
                If Me._ExceptionsToProcess.Count = 0 AndAlso (ItemChanged OrElse Me.IsDirty) Then


                    Me.ResetDatasourceType()
                    DataBind()
                End If



                If Me.DataSource IsNot Nothing Then
                    For Each field As AricieFieldEditorControl In Me.Fields
                        If Me._PostBackFields.Contains(field.DataField) Then
                            field.AutoPostBack = True
                        End If
                    Next

                    If Not String.IsNullOrEmpty(Me._ValidationGroup) Then
                        Dim vals As List(Of BaseValidator) = Aricie.Web.UI.ControlHelper.FindControlsRecursive(Of BaseValidator)(Me)
                        For Each objVal As BaseValidator In vals
                            objVal.ValidationGroup = Me._ValidationGroup
                        Next
                    End If
                    EnforceTrialMode()
                    For Each t As Tab In Me.FieldsDictionary.Tabs.Values
                        If Not t.Loaded Then
                            t.Container.Visible = False
                        End If
                    Next
                End If

                'Me.PreparePreRenderSections()


                Const resourceName As String = "Aricie.DNN.AriciePropertyEditor.css"
                'Dim cssKey As String = "Aricie.DNN.AriciePropertyEditor"
                Const cssId As String = "AriciePropertyEditorCss"
                Dim url As String = Page.ClientScript.GetWebResourceUrl(Me.GetType, resourceName)
                If (Page IsNot Nothing) AndAlso (Page.Header IsNot Nothing) Then

                    If Page.Header.FindControl(cssId) Is Nothing Then
                        Dim lnk As New HtmlLink
                        lnk.ID = cssId
                        lnk.Href = Page.ResolveUrl(url)
                        lnk.Attributes.Add("type", "text/css")
                        lnk.Attributes.Add("rel", "stylesheet")

                        Dim lnkAricieWeb As New HtmlLink
                        lnkAricieWeb.ID = "AricieWebCss"
                        lnkAricieWeb.Href = Page.ResolveUrl(Page.ClientScript.GetWebResourceUrl(Me.GetType, "Aricie.DNN.Aricie.Web.css"))
                        lnkAricieWeb.Attributes.Add("type", "text/css")
                        lnkAricieWeb.Attributes.Add("rel", "stylesheet")

                        Page.Header.Controls.Add(lnk)
                        Page.Header.Controls.Add(lnkAricieWeb)

                    End If
                End If
            Catch ex As Exception
                Me.ProcessException(ex)
            End Try
            If Not Me.DesignMode Then

                'SB: modification pour fonctionner sous DNN 4.9.3 (version eshop.aricie.info)
                Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
                If (sm Is Nothing AndAlso NukeHelper.DnnVersion.Major < 5) Then
                    sm = DirectCast(AJAX.ScriptManagerControl(Page), ScriptManager)
                End If

                If sm Is Nothing Then
                    Throw New HttpException("A ScriptManager control must exist on the page.")
                End If

                sm.RegisterScriptControl(Of AriciePropertyEditorControl)(Me)

                'RegisterScriptControlMethod.Invoke(sm, New Object() {Me})

            End If

            If Me._ExceptionsToProcess.Count > 0 Then
                Me._ExceptionsToProcess.ForEach(Sub(ex As Exception) DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex))
            End If

            'If NukeHelper.DnnVersion.Major > 6 And Me Is ParentAricieEditor Then
            '    Me.CssClass += " dnnForm"
            'End If
        End Sub

        Protected Overrides Sub Render(ByVal output As System.Web.UI.HtmlTextWriter)
            If Not Me.DesignMode Then
                Dim sm As ScriptManager = ScriptManager.GetCurrent(Me.Page)
                If sm IsNot Nothing Then
                    sm.RegisterScriptDescriptors(Me)
                End If
            End If

            MyBase.Render(output)
        End Sub


        Public Sub ResetDatasourceType()

            Controls.Clear()
            Fields.Clear()
            _FieldsDictionary = Nothing
            _FriendlyName = String.Empty
            _OnDemandSections = Nothing
            _SectionsCollapsedByDefault = Nothing
            _SortedUnderLyingDataSource = Nothing
        End Sub

        Private _QueryStringParsed As Boolean
        Public Overrides Sub DataBind()


            'Invoke OnDataBinding so DataBinding Event is raised
            MyBase.OnDataBinding(EventArgs.Empty)

            'Clear Existing Controls
            Controls.Clear()
            Me.ClearChildViewState()

            'Start Tracking ViewState
            TrackViewState()
            Try
                If Not Me.Page.IsPostBack AndAlso Not _QueryStringParsed Then
                    Me.ParseQueryString()
                    _QueryStringParsed = True
                End If
                ' SB: rajout destiné à fournir le style correct à DNN pour les popups d'aide

                'Create the Editor
                CreateEditor()



            Catch ex As Exception
                Me.ProcessException(ex)
            Finally
                'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
                ChildControlsCreated = True
            End Try
        End Sub

#End Region

#Region "Public Methods"

        Public Overloads Function GetRowVisibility(ByVal member As MemberInfo) As Boolean
            Dim info As PropertyInfo = TryCast(member, PropertyInfo)
            If (info IsNot Nothing) Then
                If Not MyBase.GetRowVisibility(info) Then
                    Return False
                End If
                'Jesse: I don't understand that rollback: why should null strings ("not true" reference type)  be hidden?
                'Jesse: reapplying the original full test
                If ReflectionHelper.IsTrueReferenceType(info.PropertyType) AndAlso info.GetValue(Me.DataSource, Nothing) Is Nothing Then
                    'If info.GetValue(Me.DataSource, Nothing) Is Nothing Then
                    Return False
                End If
            End If
            Dim condVisibles As IList(Of ConditionalVisibleInfo) = ConditionalVisibleInfo.FromMember(member)
            Return ComputeVisibility(condVisibles, member)
        End Function

        Public Sub OnDebug(sender As Object, e As DebugEventArgs)
            If Me.ParentAricieEditor IsNot Nothing Then
                Me.ParentAricieEditor.OnDebug(sender, e)
            Else
                RaiseEvent Debug(sender, e)
            End If
        End Sub

        Public Sub OnDebug()
            RaiseEvent Debug(Me, New DebugEventArgs())
        End Sub

        Public Sub DisplayLocalizedMessage(strKey As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            Dim localized As String = Localization.GetString(strKey, Me.LocalResourceFile)
            Dim moduleMessage As ModuleMessage = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(heading, localized, messageType)
            moduleMessage.EnableViewState = False
            _headerControl.Controls.Add(moduleMessage)
        End Sub


        Public Sub DisplayMessage(strMessage As String, messageType As DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType, Optional heading As String = "")
            Dim moduleMessage As ModuleMessage = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(heading, strMessage, messageType)
            moduleMessage.EnableViewState = False
            _headerControl.Controls.Add(moduleMessage)
        End Sub

#End Region



#Region "Overrides"


        Protected Overrides ReadOnly Property UnderlyingDataSource() As System.Collections.IEnumerable
            Get
                Dim toReturn As PropertyInfo() = Nothing
                If Me.DataSource IsNot Nothing Then
                    Dim key As String = Me.DataSource.GetType.AssemblyQualifiedName
                    toReturn = CacheHelper.GetGlobal(Of PropertyInfo())(key, Me.SortMode.ToString())
                    If toReturn Is Nothing Then
                        toReturn = Me.GetProperties()
                        Array.Sort(toReturn, New AriciePropertySortOrderComparer(toReturn))
                        CacheHelper.SetGlobal(Of PropertyInfo())(toReturn, key, Me.SortMode.ToString())
                    End If
                End If
                Return toReturn
            End Get
        End Property

        ''' <summary>
        ''' This override set the empty group at the first index of the array.
        ''' </summary>
        ''' <param name="arrObjects"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetGroups(ByVal arrObjects As System.Collections.IEnumerable) As String()
            Dim toReturn As String() = MyBase.GetGroups(arrObjects)
            Dim idxDefault As Integer = Array.IndexOf(toReturn, "")
            If idxDefault > 0 Then
                Dim tmpCat As String = toReturn(0)
                toReturn(0) = toReturn(idxDefault)
                toReturn(idxDefault) = tmpCat
            End If
            Return toReturn
        End Function

        Protected Overrides Function GetRowVisibility(ByVal obj As Object) As Boolean
            Return Me.GetRowVisibility(DirectCast(obj, MemberInfo))
        End Function

        Protected Overrides Sub CreateEditor()
            Me.CssClass = String.Format("{0} dnnForm", Me.CssClass)
            'Me.CssClass &= " aricie_pe_depth" ' & PropertyDepth
            If Not Me.AutoGenerate Then
                Dim mainC As New HtmlControls.HtmlGenericControl("div")
                mainC.ID = "main"
                'mainC.EnableViewState = False
                Me.AddCtFields(mainC)
                Me.Controls.Add(mainC)
            Else
                Me.Fields.Clear()
                Me.LoadJQuery()

                If (Page IsNot Nothing) AndAlso (Page.Header IsNot Nothing) AndAlso NukeHelper.DnnVersion.Major > 6 OrElse NukeHelper.DnnVersion.Major < 6 Then
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
                CreateHeader()
                ProcessSubPath()
                DisplayHierarchy()

            End If

            Validate()

        End Sub



#End Region

#Region "Inner methods"

        Private Sub CreateHeader()
            If _headerControl Is Nothing Then
                Me._headerControl = New PlaceHolder
            End If
            Me.Controls.Add(_headerControl)

        End Sub

        Private Sub ProcessSubPath()
            If Me.DataSource IsNot Nothing AndAlso Me.IsRoot Then
                If Not String.IsNullOrEmpty(Me.SubEditorFullPath) Then
                    'If Me.Page.IsPostBack Then
                    If Me.DataSource.GetType() IsNot GetType(SubPathContainer) Then
                        Dim newContainerEntity As New SubPathContainer()
                        newContainerEntity.OriginalEntity = Me.DataSource
                        newContainerEntity.OriginalPath = SubEditorFullPath
                        newContainerEntity.Path = SubEditorPath
                        'If Not String.IsNullOrEmpty(SubEditorPath) Then
                        '    newContainerEntity.Path = SubEditorPath
                        'Else
                        '    newContainerEntity.Path = newContainerEntity.OriginalPath
                        'End If
                        Me.DataSource = newContainerEntity
                        'Me._FieldsDictionary = Nothing
                        Me.ResetDatasourceType()
                    Else
                        Dim newPath As String = DirectCast(Me.DataSource, SubPathContainer).Path
                        If newPath <> Me.SubEditorPath Then
                            If Not String.IsNullOrEmpty(SubEditorPath) Then
                                Me._FieldsDictionary = Nothing
                            End If
                            Me.SubEditorPath = newPath
                        End If
                    End If
                    'Else
                    '    Me.SubEditorFullPath = ""
                    '    Me.SubEditorPath = ""
                    'End If
                Else
                    CloseSubEditor()
                End If
            End If
        End Sub

        Friend Sub CloseSubEditor()
            If Me.DataSource.GetType() Is GetType(SubPathContainer) Then
                Me.DataSource = DirectCast(Me.DataSource, SubPathContainer).OriginalEntity
                Me._FieldsDictionary = Nothing
                Me.SubEditorFullPath = ""
                Me.SubEditorPath = ""
                Me.ItemChanged = True
                Me.DataBind()
            End If
        End Sub

        Private Function GetCommandButtons() As List(Of IEnumerable(Of ActionButtonInfo))
            Dim toReturn As New List(Of IEnumerable(Of ActionButtonInfo))
            If Me.DataSource IsNot Nothing Then
                toReturn.Add(GetCommandButtons(Me.DataSource.GetType()))
                Dim subPathContainer = TryCast(Me.DataSource, SubPathContainer)
                If (subPathContainer IsNot Nothing AndAlso subPathContainer.OriginalEntity IsNot subPathContainer.SubEntity) Then
                    'toReturn = New List(Of ActionButtonInfo)(toReturn)
                    Dim dico = subPathContainer.GetParentEntities()
                    For Each objEntityPair As KeyValuePair(Of String, Object) In dico
                        Dim subEntity As Object = objEntityPair.Value
                        If subEntity IsNot Nothing Then
                            Dim additionalButtons As List(Of ActionButtonInfo) = GetCommandButtons(subEntity.GetType)
                            Dim toAdd As IEnumerable(Of ActionButtonInfo) = From tmpActionButton In additionalButtons _
                                                                            Where String.IsNullOrEmpty(tmpActionButton.ExtendedCategory.TabName) _
                                                                                AndAlso String.IsNullOrEmpty(tmpActionButton.ExtendedCategory.SectionName) _
                                                                                AndAlso ComputeVisibility(subEntity, tmpActionButton.ConditionalVisibles, tmpActionButton.Method)
                            If toAdd.Count > 0 Then
                                toReturn.Add(toAdd)
                            End If
                        End If
                    Next
                End If
            End If
            toReturn.Reverse()
            Return toReturn
        End Function

        Private Shared Function GetCommandButtons(objType As Type) As List(Of ActionButtonInfo)
            Dim toReturn As List(Of ActionButtonInfo) = CacheHelper.GetGlobal(Of List(Of ActionButtonInfo))(objType.FullName)
            If toReturn Is Nothing Then
                Dim membersDico As Dictionary(Of String, List(Of MemberInfo)) = ReflectionHelper.GetFullMembersDictionary(objType)
                toReturn = (From objMethod In (From memberPair In membersDico From objMember In memberPair.Value Select objMember).OfType(Of MethodInfo)() Where objMethod IsNot Nothing Select toAdd = ActionButtonInfo.FromMethod(objMethod) Where toAdd IsNot Nothing).ToList()
                CacheHelper.SetGlobal(Of List(Of ActionButtonInfo))(toReturn, objType.FullName)
            End If
            Return toReturn
        End Function

        Private Sub DisplayHierarchy()
            If Me.DataSource IsNot Nothing Then
                If ActionButton IsNot Nothing Then
                    Dim myPeIco As New IconActionControl()
                    myPeIco.ActionItem = _ActionButton.IconAction
                    'todo: insert a text with apropriate format and layout.
                    'myPeIco.Text = Me.DataSource.GetType.Name
                    'myPeIco.ResourceKey = Me.DataSource.GetType.Name & "_Title"
                    ' myPeIco.CssClass = "aricieIcon"
                    Me._headerControl.Controls.Add(myPeIco)
                End If
                Me.DisplayElement(Me.FieldsDictionary, False)
            End If
        End Sub

        Protected Function DisplayElement(objElement As Element, keepHidden As Boolean) As Integer

            objElement.Container.Visible = True
            Dim nbControls As Integer
            nbControls += AddColumns(objElement, objElement.Container, keepHidden)

            nbControls += AddSections(objElement, objElement.Container, keepHidden)

            nbControls += AddTabs(objElement, keepHidden)

            If Me.EditMode = PropertyEditorMode.Edit Then
                nbControls += AddActionButtons(objElement, keepHidden)

            End If

            Return nbControls
        End Function

        ''' <summary>
        ''' Retrieves the value of a cookie from the response or the request. Tries the response first to see if we already set the value server-side
        ''' </summary>
        ''' <param name="cookieName">Name of the cookie</param>
        ''' <returns></returns>
        ''' <remarks>If no cookie with this name exists in the response, cookies from the request are used</remarks>
        Private Function RetrieveCookieValue(cookieName As String) As HttpCookie
            Dim cookie As HttpCookie = HttpContext.Current.Response.Cookies(cookieName)
            If (cookie Is Nothing OrElse cookie.Value Is Nothing) Then
                cookie = HttpContext.Current.Request.Cookies(cookieName)
            End If
            Return cookie
        End Function

        Protected Function AddTabs(objElement As Element, ByVal keepHidden As Boolean) As Integer
            Dim nbControls As Integer
            If objElement.Tabs.Count > 0 Then
                Dim clientVarName As String = GetTabIndexClientVarName()
                Dim advStr As String = DnnContext.Current.AdvancedClientVariable(clientVarName)

                Dim loopTabIndex = 0
                Dim currentTabIndex As Integer = 0
                If (Not String.IsNullOrEmpty(advStr)) Then
                    Integer.TryParse(advStr, currentTabIndex)
                End If

                If currentTabIndex > objElement.Tabs.Count - 1 Then
                    currentTabIndex = 0
                End If
                Dim tabsContainer As New HtmlGenericControl("div")
                Dim tabsHeader As New HtmlGenericControl("ul")
                tabsHeader.EnableViewState = False

                tabsContainer.Attributes.Add("class", "aricie_pe_tabs-" & Me.ClientID)
                tabsContainer.Attributes.Add("hash", Me.ClientID.GetHashCode().ToString())
                'tabsContainer.Attributes.Add("data-entitypath", Regex.Escape(GetPath()))
                DirectCast(tabsContainer, IAttributeAccessor).SetAttribute("data-activetab", currentTabIndex.ToString(CultureInfo.InvariantCulture))
                objElement.Container.Controls.Add(tabsContainer)
                objElement.TabsContainer = tabsContainer
                tabsContainer.Controls.Add(tabsHeader)
                Dim tabIshidden As Boolean

                For Each objTab As Tab In objElement.Tabs.Values

                    objTab.Container = New HtmlGenericControl("div")
                    'tabContainer.EnableViewState = False
                    objTab.Container.ID = String.Format("{0}_{1}", Me.FriendlyName, objTab.Name.Replace(" ", ""))
                    tabsContainer.Controls.Add(objTab.Container)
                    objTab.Container.ID = objTab.Container.ClientID.GetHashCode.ToString
                    'Dim tabId As String = String.Format("{0}_{1}", Me.FriendlyName, tabSectionsPair.Key.Name.Replace(" ", ""))
                    'tabContainer.Attributes.Add("id", tabId)


                    Dim loadTab As Boolean = False
                    Dim displayTab As Boolean = False
                    If currentTabIndex = loopTabIndex Then
                        tabIshidden = Me._isHidden
                        displayTab = True
                        loadTab = True
                        _CurrentTab = objTab
                    Else
                        tabIshidden = True
                        loadTab = False ' (Not _EnabledOnDemandSections) OrElse (Not _isHidden AndAlso isFirstPass)
                        ' désactivation du chargement transversal des onglets
                        displayTab = Me.HasVisibleContent(objTab)
                    End If

                    Dim li = New HtmlGenericControl("li")
                    tabsHeader.Controls.Add(li)

                    Dim headerLink As New HtmlGenericControl("a")
                    objTab.HeaderLink = headerLink
                    headerLink.EnableViewState = False
                    headerLink.Attributes.Add("href", "#" & objTab.Container.ClientID)


                    Dim resKey As String = objTab.ResourceKey
                    If String.IsNullOrEmpty(resKey) Then
                        resKey = Me.FriendlyName & "_" & objTab.Name & ".Header"
                    End If
                    Dim headerText As String = Localization.GetString(resKey, Me.LocalResourceFile)
                    If headerText = "" Then
                        headerText = resKey
                    End If
                    headerLink.InnerText = headerText
                    li.Controls.Add(headerLink)

                    If displayTab Then
                        If loadTab Then
                            objTab.Loaded = True
                            nbControls += Me.DisplayElement(objTab, tabIshidden)
                        Else
                            headerLink.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(Me, "tabChange" & ClientAPI.COLUMN_DELIMITER & objTab.Name))
                        End If
                    Else
                        objTab.Container.Visible = False
                        li.Style("display") = "none"
                    End If


                    loopTabIndex += 1
                Next
            End If

            Return nbControls

        End Function

        Private _CurrentPath As String = Nothing

        Public Function GetPath() As String Implements IPathProvider.GetPath
            If _CurrentPath Is Nothing Then
                Dim parentPath As String = CollectionEditControl.GetParentPath(Me)
                If parentPath.IsNullOrEmpty() Then
                    If Me.ParentAricieField Is Nothing Then
                        If Me.IsRoot AndAlso Not Me.SubEditorPath.IsNullOrEmpty Then
                            _CurrentPath = Me.SubEditorPath
                        Else
                            _CurrentPath = String.Empty
                        End If
                    Else
                        If Me.ParentAricieField.DataField <> "SubEntity" AndAlso Me.ParentAricieField.DataField <> "ReadOnlySubEntity" Then
                            _CurrentPath = Me.ParentAricieField.DataField
                        Else
                            _CurrentPath = String.Empty
                        End If
                    End If
                Else
                    If Me.ParentAricieField.DataField <> "SubEntity" AndAlso Me.ParentAricieField.DataField <> "ReadOnlySubEntity" Then
                        _CurrentPath = String.Format("{0}.{1}", parentPath, Me.ParentAricieField.DataField)
                    Else
                        _CurrentPath = parentPath
                    End If
                End If
            End If

            Return _CurrentPath
        End Function

       

        Protected Function AddSections(ByVal element As Element, ByVal container As Control, ByVal keepHidden As Boolean) As Integer

            Dim nbControls As Integer

            For Each s As Section In element.Sections.Values

                Dim sectionContainer As Control = Nothing
                Dim sectionControl As Control = FormHelper.AddSection(container, s.ResourceKey, False, sectionContainer)
                s.Container = sectionContainer
                Dim nbSectionControls As Integer = DisplayElement(s, keepHidden)
                If nbSectionControls = 0 Then
                    container.Controls.Remove(sectionControl)
                End If
                nbControls += nbSectionControls

            Next
            Return nbControls
        End Function

        Protected Function AddColumns(ByVal element As Element, ByVal container As Control, ByVal keepHidden As Boolean) As Integer
            ' on va sauvegarder l'élément racine pour pouvoir insérer le header si nécessaire
            Dim rootContainer As Control = container
            ' et le header pour pouvoir l'effacer si nécessaire
            'Dim headerControl As Control = Nothing

            ' ajout de colonnes multiples
            Dim multipleColumnsExist As Boolean = element.Columns.Count > 1
            'Dim objSection As Section = TryCast(element, Section)
            'If (objSection IsNot Nothing) Then

            '    ' commençons avant toute chose par rajouter le header control, on le retirera plus tard si nécessaire
            '    AddSectionHeader(rootContainer, objSection.Name)
            '    headerControl = rootContainer.Controls(rootContainer.Controls.Count - 1)

            '    Dim sectionContainer As New HtmlGenericControl("fieldset")
            '    'div.EnableViewState = False
            '    container.Controls.Add(sectionContainer)
            '    objSection.Container = sectionContainer
            '    objSection.Image = CType(container.FindControl("ico" & objSection.Name), Image)

            '    container = sectionContainer
            'End If

            Dim counter As Integer = 0
            Dim columnMaxWidth As Integer = CInt(Math.Floor(100 / Math.Max(element.Columns.Count, 1)))

            For Each c As Integer In element.Columns.Keys
                Dim currentContainer As Control = container
                ' si on a plusieurs colonnes, le container doit être modifié pour prendre un div de la taille de la colonne (width / nb de colonnes)
                If (multipleColumnsExist) Then

                    Dim col As New HtmlGenericControl("div")
                    col.Attributes.Add("class", String.Format("peCol{0} peCol", element.Columns.Count))
                    'col.EnableViewState = False
                    'col.Style.Add("float", "left")
                    'col.Style.Add("Width", String.Format("{0}%", columnMaxWidth.ToString))

                    currentContainer = DirectCast(col, Control)
                End If

                For Each p As PropertyInfo In element.Columns(c)
                    If Me.GetRowVisibility(p) Then
                        Me.AddEditorCtl(currentContainer, p, keepHidden)
                        counter += 1
                    End If
                Next

                ' et une fois le container colonne rempli, remettons le dans le contrôle originel
                If multipleColumnsExist Then
                    container.Controls.Add(currentContainer)
                End If
            Next

            ' on a x contrôles rajoutés, mais si il n'y a eu qu'un header rajouté, on le retire, il n'a aucun enfant.
            'If (counter = 0 AndAlso headerControl IsNot Nothing) Then
            '    rootContainer.Controls.Remove(headerControl)
            'End If

            Return counter
        End Function

        Protected Function AddActionButtons(ByVal element As Element, ByVal keepHidden As Boolean) As Integer
            Dim nbControls As Integer
            Dim buttonContainer As Panel = Nothing
            For Each buttonList As List(Of ActionButtonInfo) In element.ActionButtons
                For Each objActionButton As ActionButtonInfo In buttonList
                    If Me.ComputeVisibility(objActionButton.ConditionalVisibles, objActionButton.Method) Then
                        If buttonContainer Is Nothing Then
                            buttonContainer = New Panel()
                            buttonContainer.EnableViewState = False
                            'buttonContainer.ID = "divCmdButtons" & element.Name
                            buttonContainer.CssClass = "aricieActions"
                            element.Container.Controls.Add(buttonContainer)
                        End If
                        Me.AddActionButton(objActionButton, buttonContainer)
                        nbControls += 1
                    End If
                Next
                buttonContainer = Nothing
            Next
            Return nbControls
        End Function


        Public Overridable Function GetContext(contextType As Type) As Object
            Dim currentPe As AriciePropertyEditorControl = Me
            Dim currentProvider As IContextProvider
            Do
                If TypeOf currentPe.DataSource Is IContextProvider Then
                    currentProvider = DirectCast(currentPe.DataSource, IContextProvider)
                    If currentProvider.HasContect(contextType) Then
                        Return currentProvider.GetContext(contextType)
                    End If
                End If
                currentPe = currentPe.ParentAricieEditor
            Loop Until currentPe Is Nothing
            Return Nothing
        End Function



        Private Sub AddActionButton(objButtonInfo As ActionButtonInfo, container As Control)

            Dim btn As WebControl = Nothing
            Select Case objButtonInfo.Mode
                Case ActionButtonMode.CommandButton
                    Dim cmdbtn As New CommandButton()
                    btn = cmdbtn
                    'If NukeHelper.DnnVersion.Major > 6 Then
                    '    cmdbtn.CssClass = "dnnTertiaryAction aricieActions"
                    'Else
                    cmdbtn.CssClass = "aricieAction"
                    'End If

                    cmdbtn.Text = objButtonInfo.Method.Name
                    cmdbtn.ResourceKey = objButtonInfo.Method.GetBaseDefinition().DeclaringType.Name & "_" & objButtonInfo.Method.Name & ".Text"
                    If objButtonInfo.IconPath <> "" Then
                        cmdbtn.ImageUrl = objButtonInfo.IconPath
                    End If

                Case ActionButtonMode.IconButton
                    Dim iconbtn As New IconActionButton()
                    btn = iconbtn
                    'iconbtn.CssClass = "dnnTertiaryAction"
                    iconbtn.ID = objButtonInfo.Method.GetBaseDefinition().DeclaringType.Name & "_" & objButtonInfo.Method.Name
                    iconbtn.ActionItem = objButtonInfo.IconAction
                    iconbtn.Text = objButtonInfo.Method.Name
                    iconbtn.ResourceKey = iconbtn.ID & ".Text"
            End Select

            container.Controls.Add(btn)
            If objButtonInfo.AlertKey <> "" Then
                Dim message As String = Localization.GetString(objButtonInfo.AlertKey, Me.LocalResourceFile)
                If message = "" Then
                    message = objButtonInfo.AlertKey
                End If
                ClientAPI.AddButtonConfirm(btn, message)
            End If
            'todo: there is redundancy in the anonymous methods
            If TypeOf btn Is IconActionButton Then
                AddHandler DirectCast(btn, IconActionButton).Click, Sub(s As Object, e As EventArgs)
                                                                        Try
                                                                            Dim params As ParameterInfo() = objButtonInfo.Method.GetParameters()
                                                                            Dim paramInstances As New List(Of Object)
                                                                            For Each objParam As ParameterInfo In params
                                                                                Dim paramType As Type = objParam.ParameterType
                                                                                If paramType Is GetType(AriciePortalModuleBase) Then
                                                                                    paramInstances.Add(Me.ParentModule)
                                                                                ElseIf paramType Is GetType(AriciePropertyEditorControl) Then
                                                                                    paramInstances.Add(Me)
                                                                                ElseIf paramType Is GetType(AricieFieldEditorControl) Then
                                                                                    paramInstances.Add(Me.ParentAricieField)
                                                                                Else
                                                                                    Dim contextParam As Object = Me.GetContext(paramType)
                                                                                    paramInstances.Add(contextParam)
                                                                                End If
                                                                            Next
                                                                            Dim targetEntity As Object = Me.DataSource

                                                                            If TypeOf Me.DataSource Is SubPathContainer Then
                                                                                Dim objContainer As SubPathContainer = DirectCast(Me.DataSource, SubPathContainer)
                                                                                Dim targetType As Type = objButtonInfo.Method.GetBaseDefinition().DeclaringType
                                                                                If targetType IsNot GetType(SubPathContainer) Then
                                                                                    targetEntity = DirectCast(Me.DataSource, SubPathContainer).OriginalEntity
                                                                                    For Each objEntity As Object In objContainer.GetParentEntities().Values.Reverse()
                                                                                        If targetType.IsInstanceOfType(objEntity) Then
                                                                                            targetEntity = objEntity
                                                                                            Exit For
                                                                                        End If
                                                                                    Next
                                                                                End If
                                                                            End If
                                                                            objButtonInfo.Method.Invoke(targetEntity, paramInstances.ToArray)
                                                                        Catch ex As Exception
                                                                            Me.ProcessException(ex)
                                                                        End Try
                                                                    End Sub
            ElseIf TypeOf btn Is CommandButton Then
                AddHandler DirectCast(btn, CommandButton).Click, Sub(s As Object, e As EventArgs)
                                                                     Try
                                                                         Dim params As ParameterInfo() = objButtonInfo.Method.GetParameters()
                                                                         Dim paramInstances As New List(Of Object)
                                                                         For Each objParam As ParameterInfo In params
                                                                             Dim paramType As Type = objParam.ParameterType
                                                                             If paramType Is GetType(AriciePortalModuleBase) Then
                                                                                 paramInstances.Add(Me.ParentModule)
                                                                             ElseIf paramType Is GetType(AriciePropertyEditorControl) Then
                                                                                 paramInstances.Add(Me)
                                                                             ElseIf paramType Is GetType(AricieFieldEditorControl) Then
                                                                                 paramInstances.Add(Me.ParentAricieField)
                                                                             Else
                                                                                 Dim contextParam As Object = Me.GetContext(paramType)
                                                                                 paramInstances.Add(contextParam)
                                                                             End If
                                                                         Next
                                                                         Dim targetEntity As Object = Me.DataSource

                                                                         If TypeOf Me.DataSource Is SubPathContainer Then
                                                                             Dim objContainer As SubPathContainer = DirectCast(Me.DataSource, SubPathContainer)
                                                                             Dim targetType As Type = objButtonInfo.Method.GetBaseDefinition().DeclaringType
                                                                             If targetType IsNot GetType(SubPathContainer) Then
                                                                                 targetEntity = DirectCast(Me.DataSource, SubPathContainer).OriginalEntity
                                                                                 For Each objEntity As Object In objContainer.GetParentEntities().Values.Reverse()
                                                                                     If targetType.IsInstanceOfType(objEntity) Then
                                                                                         targetEntity = objEntity
                                                                                         Exit For
                                                                                     End If
                                                                                 Next
                                                                             End If
                                                                         End If
                                                                         objButtonInfo.Method.Invoke(targetEntity, paramInstances.ToArray)
                                                                     Catch ex As Exception
                                                                         Me.ProcessException(ex)
                                                                     End Try
                                                                 End Sub

            End If


        End Sub

        Protected Sub AddEditorCtl(ByRef container As Control, ByVal obj As Object, ByVal keepHidden As Boolean)
            Dim objPropertyInfo As PropertyInfo = DirectCast(obj, PropertyInfo)
            Try

                Me.AddEditorCtl(container, objPropertyInfo.Name, New AricieStandardEditorInfoAdapter(Me.DataSource, objPropertyInfo.Name), keepHidden)
                Dim customAttributes As Object()

                customAttributes = ReflectionHelper.GetCustomAttributes(objPropertyInfo).Where(Function(objAttribute) TypeOf objAttribute Is LimitationAttribute).ToArray()
                If (customAttributes.Length > 0) Then
                    Dim objAttr As LimitationAttribute = DirectCast(customAttributes(0), LimitationAttribute)
                    If objAttr.IsLimited(Me) Then
                        Dim objField As AricieFieldEditorControl = DirectCast(Me.Fields(Me.Fields.Count - 1), AricieFieldEditorControl)
                        Me._RestrictedFields(objField) = objAttr
                    End If
                End If

            Catch ex As Exception
                Me.ProcessException(ex)
            End Try

        End Sub

        Protected Sub AddCtFields(ByVal Container As Control)
            For Each myField As AricieFieldEditorControl In Fields
                Dim myDivCt As New HtmlControls.HtmlGenericControl("div")
                myDivCt.EnableViewState = False
                myDivCt.Attributes.Add("class", "fieldCt")
                Container.Controls.Add(myDivCt)

                With myField
                    .EditorDisplayMode = Me.DisplayMode
                    .EnableClientValidation = Me.EnableClientValidation
                    .EditMode = Me.EditMode
                    .LabelWidth = Me.LabelWidth
                    .LabelStyle.CopyFrom(Me.LabelStyle)
                    .HelpStyle.CopyFrom(Me.HelpStyle)
                    .ErrorStyle.CopyFrom(Me.ErrorStyle)
                    .EditControlStyle.CopyFrom(Me.EditControlStyle)
                    .EditControlWidth = Me.EditControlWidth
                    .RequiredUrl = Me.RequiredUrl
                    .ShowRequired = Me.ShowRequired
                    .ShowVisibility = Me.ShowVisibility
                    .Width = Me.Width
                    .DataSource = Me.DataSource
                    .DataBind()
                End With

                myDivCt.Controls.Add(myField)
            Next

        End Sub

        Protected Sub AddEditorCtl(ByRef container As Control, ByVal name As String, ByVal editorAdapter As IEditorInfoAdapter, ByVal keepHidden As Boolean)
            Dim myCtDiv As New HtmlControls.HtmlGenericControl("div")
            'myCtDiv.EnableViewState = False
            myCtDiv.Attributes.Add("class", "fieldC")
            container.Controls.Add(myCtDiv)
            Dim fieldControl As New AricieFieldEditorControl
            'control.ID = "field" + name
            'control.ID = name.Substring(0, 3) & name.Substring(name.Length - 2)
            fieldControl.ID = name ' la méthode au dessus ne marche pas, EnableTheseSettings et EnableTheseOtherSettings provoque une collision
            fieldControl.IsHidden = keepHidden
            fieldControl.DataSource = Me.DataSource
            fieldControl.EditorInfoAdapter = editorAdapter
            fieldControl.DataField = name
            fieldControl.EditorDisplayMode = Me.DisplayMode
            fieldControl.EnableClientValidation = Me.EnableClientValidation
            fieldControl.EditMode = Me.EditMode
            fieldControl.HelpDisplayMode = Me.HelpDisplayMode
            fieldControl.LabelWidth = Me.LabelWidth
            fieldControl.LabelMode = LabelMode.Left
            fieldControl.LabelStyle.CopyFrom(Me.LabelStyle)
            fieldControl.HelpStyle.CopyFrom(Me.HelpStyle)
            fieldControl.ErrorStyle.CopyFrom(Me.ErrorStyle)
            fieldControl.VisibilityStyle.CopyFrom(Me.VisibilityStyle)
            fieldControl.EditControlStyle.CopyFrom(Me.EditControlStyle)
            fieldControl.EditControlWidth = Me.EditControlWidth
            fieldControl.LocalResourceFile = Me.LocalResourceFile
            fieldControl.RequiredUrl = Me.RequiredUrl
            fieldControl.ShowRequired = Me.ShowRequired
            fieldControl.ShowVisibility = Me.ShowVisibility
            AddHandler fieldControl.ItemChanged, New DotNetNuke.UI.WebControls.PropertyChangedEventHandler(AddressOf Me.ListItemChanged)
            AddHandler fieldControl.ItemCreated, New EditorCreatedEventHandler(AddressOf Me.EditorItemCreated)


            Me.Fields.Add(fieldControl)
            myCtDiv.Controls.Add(fieldControl)
            fieldControl.DataBind()

        End Sub

        Protected Sub Validate()
            For Each editor As AricieFieldEditorControl In Fields
                If editor.Visible AndAlso Not editor.IsValid Then
                    _IsValid = False
                    Exit For
                End If
            Next
            _Validated = True

            If Me.ParentAricieEditor IsNot Nothing Then
                Me.ParentAricieEditor.ValidationNeeded()
            End If

        End Sub

        Protected Sub ValidationNeeded()
            _Validated = False

            For Each field As AricieFieldEditorControl In Fields
                field.ValidationNeeded()
            Next
        End Sub

        Public Sub LoadJQuery()
            If Me._JQueryVersion <> "" AndAlso Me._JQueryUIVersion <> "" Then
                FormHelper.LoadjQuery(Me.Page, Me._JQueryVersion, Me._JQueryUIVersion)
            Else
                FormHelper.LoadjQuery(Me.Page)
            End If
        End Sub

        Private Sub EnforceTrialMode()
            Dim warningMessage As String = Localization.GetString(TrialLimitedAttribute.TrialModeLimitedKey, LocalResourceFile)
            If String.IsNullOrEmpty(warningMessage) Then
                warningMessage = TrialLimitedAttribute.TrialModeLimitedKey
            End If

            For Each restrictedField As KeyValuePair(Of AricieFieldEditorControl, LimitationAttribute) In Me._RestrictedFields
                Dim objAttr As LimitationAttribute = restrictedField.Value
                Dim objField As AricieFieldEditorControl = restrictedField.Key

                If TypeOf objField.Editor Is UserControlEditControl Then
                    ' si c'est un contrôle utilisateur dans le propety editor, on lui passe juste l'information nécessaire
                    DirectCast(objField.Editor, UserControlEditControl).TrialMode = New UserControlEditControl.TrialModeInformation() With {.CurrentTrialMode = objAttr.LimitationMode, .Message = warningMessage}
                Else
                    'sinon on effectue le traitement classique
                    objField.ToolTip = warningMessage ' on fixe le tooltip sur toute la ligne
                    objField.CssClass = objField.CssClass & " PETrialDisabled" ' on rajoute une classe

                    objField.Editor.CssClass = objField.Editor.CssClass & " PETrialEditorDisabled" ' on rajoute une classe sur l'éditeur uniquement
                    If objField.ShowRequired AndAlso objField.Editor.Required AndAlso objField.Editor.Parent.Controls.Count >= 2 AndAlso TypeOf objField.Editor.Parent.Controls(1) Is Image Then
                        objField.Editor.Parent.Controls(1).Visible = False ' on a pas besoin de l'image puisqu'on a désactivé l'éditeur
                    End If


                    If (objAttr.LimitationMode And TrialPropertyMode.Hide) = TrialPropertyMode.Hide Then
                        objField.Visible = False
                    ElseIf (objAttr.LimitationMode And TrialPropertyMode.Disable) = TrialPropertyMode.Disable Then
                        objField.Enabled = False
                        objField.EditMode = PropertyEditorMode.View
                        objField.Editor.EditMode = PropertyEditorMode.View
                        objField.Editor.Required = False
                    Else
                        If (objAttr.LimitationMode And TrialPropertyMode.NoAdd) = TrialPropertyMode.NoAdd _
                                OrElse (objAttr.LimitationMode And TrialPropertyMode.NoDelete) = TrialPropertyMode.NoDelete Then

                            If TypeOf objField.Editor Is AricieEditControl Then
                                DirectCast(objField.Editor, AricieEditControl).EnforceTrialMode(objAttr.LimitationMode)
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        Private Function HasVisibleContent(ByVal objElement As Element) As Boolean
            For Each objProps As IList(Of PropertyInfo) In objElement.Columns.Values
                For Each objProp As PropertyInfo In objProps
                    If Me.GetRowVisibility(objProp) Then
                        Return True
                    End If
                Next
            Next

            For Each objSection As Section In objElement.Sections.Values
                If HasVisibleContent(objSection) Then
                    Return True
                End If
            Next

            For Each objTab As Tab In objElement.Tabs.Values
                If HasVisibleContent(objTab) Then
                    Return True
                End If
            Next

        End Function

        Private Function GetProperties() As PropertyInfo()

            If Not DataSource Is Nothing Then
                'TODO:  We need to add code to support using the cache in the future

                'Dim Bindings As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.FlattenHierarchy

                Dim objProps As PropertyInfo() = ReflectionHelper.GetPropertiesDictionary(DataSource.GetType()).Values.Where(Function(objProp As PropertyInfo) _
                                                                                                                                 (Not objProp.GetAccessors(True)(0).IsStatic) _
                                                                                                                                 AndAlso objProp.GetIndexParameters().Length = 0).ToArray()

                'Apply sort method
                Select Case SortMode
                    Case PropertySortType.Alphabetical
                        Array.Sort(objProps, New PropertyNameComparer)
                    Case PropertySortType.Category
                        Array.Sort(objProps, New PropertyCategoryComparer)
                    Case PropertySortType.SortOrderAttribute
                        Array.Sort(objProps, New PropertySortOrderComparer)
                End Select

                Return objProps
            Else
                Return Nothing
            End If

        End Function

        Private Overloads Function ComputeVisibility(conditionalVisibles As IList(Of ConditionalVisibleInfo), objMember As MemberInfo) As Boolean
            Return ComputeVisibility(Me.DataSource, conditionalVisibles, objMember, Me)
        End Function

        Private Overloads Shared Function ComputeVisibility(objDataSource As Object, conditionalVisibles As IList(Of ConditionalVisibleInfo), objMember As MemberInfo, Optional pe As AriciePropertyEditorControl = Nothing) As Boolean
            Dim toReturn As Boolean = True
            Dim props As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(objDataSource.GetType)
            Dim objPropInfo As PropertyInfo = Nothing
            For Each condVisibility As ConditionalVisibleInfo In conditionalVisibles
                If props.TryGetValue(condVisibility.MasterPropertyName, objPropInfo) Then
                    Dim subVis As IList(Of ConditionalVisibleInfo) = ConditionalVisibleInfo.FromMember(objPropInfo)
                    If subVis.Count > 0 AndAlso objMember IsNot objPropInfo _
                        AndAlso ((pe IsNot Nothing AndAlso Not pe.GetRowVisibility(objPropInfo)) _
                                 OrElse (pe Is Nothing AndAlso ComputeVisibility(objDataSource, subVis, objPropInfo))) Then
                        'todo: figure out if correct
                        toReturn = False
                    Else
                        Dim value As Object = objPropInfo.GetValue(objDataSource, Nothing)
                        toReturn = toReturn AndAlso condVisibility.MatchValue(value)
                        If toReturn AndAlso condVisibility.SecondaryPropertyName <> "" Then
                            If props.TryGetValue(condVisibility.SecondaryPropertyName, objPropInfo) Then
                                value = objPropInfo.GetValue(objDataSource, Nothing)
                                toReturn = toReturn AndAlso condVisibility.MatchSecondary(value)
                            End If
                        End If
                    End If
                    If condVisibility.EnforceAutoPostBack AndAlso pe IsNot Nothing Then
                        If Not pe._PostBackFields.Contains(condVisibility.MasterPropertyName) Then
                            pe._PostBackFields.Add(condVisibility.MasterPropertyName)
                        End If
                        If condVisibility.SecondaryPropertyName <> "" Then
                            If Not pe._PostBackFields.Contains(condVisibility.SecondaryPropertyName) Then
                                pe._PostBackFields.Add(condVisibility.SecondaryPropertyName)
                            End If
                        End If
                    End If
                End If
            Next
            Return toReturn
        End Function



        Private Sub ParseQueryString()
            If Me.IsRoot Then
                Dim pathQuery As String = Me.Context.Request.QueryString(SubPathQuery)
                If Not String.IsNullOrEmpty(pathQuery) Then
                    Me.SubEditorFullPath = pathQuery
                End If
            End If
        End Sub

        Private Function GetTabIndexClientVarName() As String
            Return Me.GetPath() & "-cookieTab"
        End Function

#End Region

#Region "IScriptControl"


        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors

            Dim toReturn As New List(Of ScriptDescriptor)
            Dim strId As String = ClientID
            Dim descriptor As ScriptControlDescriptor = New ScriptControlDescriptor("Aricie.DNN.AriciePropertyEditorScripts", strId)

            descriptor.AddProperty("clientId", strId)
            'descriptor.AddProperty("hash", strId.GetHashCode().ToString())
            'descriptor.AddProperty("friendlyName", Me.FriendlyName)

            toReturn.Add(descriptor)


            Return toReturn
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim toReturn As New List(Of ScriptReference)
            Dim cookieJsUrl As String = Page.ClientScript.GetWebResourceUrl(Me.GetType, "Aricie.DNN.jquery.cookie.js")
            Dim scriptsUrl As String = Page.ClientScript.GetWebResourceUrl(Me.GetType, "Aricie.DNN.AriciePropertyEditorScripts.js")

            toReturn.Add(New ScriptReference(cookieJsUrl))
            toReturn.Add(New ScriptReference(scriptsUrl))
            Return toReturn
        End Function


#End Region

#Region "Inner classes"

        Public Class Element
            Private _Name As String
            Private _Container As Control

            Private _Prefix As String


            Private _ResourceKey As String

            Public Sub New()

            End Sub

            Public Sub New(ByVal name As String, ByVal container As Control)
                _Name = name
                _Container = container
            End Sub

            Public Sub New(ByVal name As String, ByVal prefix As String)
                _Name = name
                Me.Prefix = prefix
                'Me._ResourceKey = String.Format("{0}_{1}.Header", prefix, name)
            End Sub

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
                End Set
            End Property


            Public Property Prefix As String
                Get
                    Return _Prefix
                End Get
                Set(value As String)
                    _Prefix = value
                End Set
            End Property

            Public Property ResourceKey() As String
                Get
                    If String.IsNullOrEmpty(Me._ResourceKey) Then
                        Me._ResourceKey = String.Format("{0}_{1}.Header", Me.Prefix, Name)
                    End If
                    Return _ResourceKey
                End Get
                Set(ByVal value As String)
                    _ResourceKey = value
                End Set
            End Property


            Public Property Container() As Control
                Get
                    Return _Container
                End Get
                Set(ByVal value As Control)
                    _Container = value
                End Set
            End Property

            Public Property Loaded() As Boolean
           


            Public Property Columns As New Dictionary(Of Integer, List(Of PropertyInfo))

            Public Property Sections As New Dictionary(Of String, Section)

            Public Property TabsContainer As IAttributeAccessor

            Public Property Tabs As New Dictionary(Of String, Tab)

            Public Property ActionButtons As New List(Of List(Of ActionButtonInfo))

            'Public Sub HideAllTabs()
            '    For Each t As Tab In Me.Tabs.Values
            '        t.Loaded = False
            '    Next
            'End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Me.Name.GetHashCode()
            End Function

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                Dim e As Element = CType(obj, Element)
                Return Me.Name.Equals(e.Name)
            End Function
        End Class

        Public Class Tab
            Inherits Element

            Sub New(ByVal name As String, ByVal prefix As String)
                MyBase.New(name, prefix)
            End Sub


            Public Shared ReadOnly Empty As New Tab("", Nothing)

            Public Shared Function FromName(ByVal name As String, ByVal prefix As String) As Tab
                Return New Tab(name, prefix)
            End Function

            Private _HeaderLink As HtmlGenericControl

            Public Property HeaderLink() As HtmlGenericControl
                Get
                    Return _HeaderLink
                End Get
                Set(ByVal value As HtmlGenericControl)
                    _HeaderLink = value
                End Set
            End Property


        End Class

        Public Class Section
            Inherits Element

            'Private _Image As Image

            Public Sub New(ByVal name As String, ByVal prefix As String)
                MyBase.New(name, prefix)
            End Sub

            'Public Property Image() As Image
            '    Get
            '        Return _Image
            '    End Get
            '    Set(ByVal value As Image)
            '        _Image = value
            '    End Set
            'End Property

            Public Shared ReadOnly Empty As New Section("", Nothing)

            Public Shared Function FromName(ByVal name As String, ByVal prefix As String) As Section
                Return New Section(name, prefix)
            End Function

        End Class

        Public Class FieldsHierarchy
            Inherits Element


            Public Sub New(ByVal underlyingDatasource As IEnumerable, objButtons As List(Of IEnumerable(Of ActionButtonInfo)), ByVal editor As AriciePropertyEditorControl)
                MyBase.New(editor.FriendlyName, editor)
                Me.ParseProperties(underlyingDatasource)
                Me.ParseActionButtons(objButtons)
            End Sub



            Private Sub ParseProperties(ByVal underlyingDataSource As IEnumerable)
                If underlyingDataSource IsNot Nothing Then
                    For Each p As PropertyInfo In underlyingDataSource
                        'If _Editor.IsPropertyVisible(p) Then
                        'Dim added As Boolean = False

                        Dim extCategory As ExtendedCategory = ExtendedCategory.FromMember(p)
                        Dim target As Element = Me.ProcessExtendedCategory(extCategory)
                        target.Columns(extCategory.Column).Add(p)


                        'Dim customAttributes As Object() = p.GetCustomAttributes(GetType(CategoryAttribute), True)
                        'If customAttributes.Length > 0 Then
                        '    Dim categoryAttr As CategoryAttribute = CType(customAttributes(0), CategoryAttribute)

                        '    Dim columns As Columns = _Tabs.NotInTab.NotInSection



                        '    If Not _Tabs.NotInTab.ContainsKey(Section.FromName(categoryAttr.Category, p)) And Not String.IsNullOrEmpty(categoryAttr.Category) Then

                        '        Dim s As Section = Section.FromName(categoryAttr.Category, p)
                        '        _Tabs.NotInTab.Add(s, New Columns)
                        '        _Sections.Add(s, _Tabs.NotInTab(s))
                        '    End If

                        '    If Not String.IsNullOrEmpty(categoryAttr.Category) Then
                        '        columns = _Tabs.NotInTab(Section.FromName(categoryAttr.Category, p))
                        '    End If

                        '    If Not columns.ContainsKey(0) Then
                        '        columns.Add(0, New List(Of PropertyInfo))
                        '    End If

                        '    columns(0).Add(p)

                        '    added = True
                        'End If


                        'customAttributes = p.GetCustomAttributes(GetType(ExtendedCategoryAttribute), True)
                        'If customAttributes.Length > 0 Then
                        '    Dim categoryAttr As ExtendedCategoryAttribute = CType(customAttributes(0), ExtendedCategoryAttribute)

                        '    Dim sections As Sections = _Tabs.NotInTab
                        '    Dim columns As Columns = Nothing

                        '    If Not Me.Tabs.ContainsKey(Tab.FromName(categoryAttr.ExtendedCategory.TabName, p)) And Not Tab.FromName(categoryAttr.ExtendedCategory.TabName, p).Equals(Tab.Empty) Then
                        '        Me.Tabs.Add(Tab.FromName(categoryAttr.ExtendedCategory.TabName, p), New Sections)
                        '    End If

                        '    If Not Tab.FromName(categoryAttr.ExtendedCategory.TabName, p).Equals(Tab.Empty) Then
                        '        sections = Me.Tabs(Tab.FromName(categoryAttr.ExtendedCategory.TabName, p))
                        '    End If

                        '    If Not String.IsNullOrEmpty(categoryAttr.ExtendedCategory.SectionName) Then
                        '        Dim s As Section = Section.FromName(categoryAttr.ExtendedCategory.SectionName, p)

                        '        If Not sections.ContainsKey(Section.FromName(categoryAttr.ExtendedCategory.SectionName, p)) Then
                        '            sections.Add(s, New Columns)
                        '            _Sections.Add(s, sections(s))
                        '        End If

                        '        columns = sections(s)
                        '    Else
                        '        columns = sections.NotInSection
                        '    End If



                        '    If Not columns.ContainsKey(categoryAttr.ExtendedCategory.Column) Then
                        '        columns.Add(categoryAttr.ExtendedCategory.Column, New List(Of PropertyInfo))
                        '    End If

                        '    columns(categoryAttr.ExtendedCategory.Column).Add(p)

                        '    added = True
                        'End If

                        'If Not added Then
                        '    If Not _Tabs.NotInTab.NotInSection.ContainsKey(0) Then
                        '        _Tabs.NotInTab.NotInSection.Add(0, New List(Of PropertyInfo))
                        '    End If

                        '    _Tabs.NotInTab.NotInSection(0).Add(p)

                        'End If


                        ''End If


                    Next
                End If
            End Sub


            Private Function ProcessExtendedCategory(exCat As ExtendedCategory) As Element
                Dim toReturnElt As Element = Me

                If Not String.IsNullOrEmpty(exCat.TabName) Then

                    Dim objTab As Tab = Nothing
                    If Not Me.Tabs.TryGetValue(exCat.TabName, objTab) Then
                        objTab = New Tab(exCat.TabName, exCat.Prefix)
                        toReturnElt.Tabs.Add(exCat.TabName, objTab)
                    End If
                    toReturnElt = objTab
                End If
                If Not String.IsNullOrEmpty(exCat.SectionName) Then
                    Dim objSection As Section = Nothing
                    If Not toReturnElt.Sections.TryGetValue(exCat.SectionName, objSection) Then
                        'toReturnColumns = New Columns
                        'objSections.Add(objSection, toReturnColumns)
                        objSection = New Section(exCat.SectionName, exCat.Prefix)
                        toReturnElt.Sections.Add(exCat.SectionName, objSection)
                    End If
                    toReturnElt = objSection

                End If
                If Not toReturnElt.Columns.ContainsKey(exCat.Column) Then
                    toReturnElt.Columns.Add(exCat.Column, New List(Of PropertyInfo))
                End If
                Return toReturnElt
            End Function

            Private Sub ParseActionButtons(ByVal objActionButtons As List(Of IEnumerable(Of ActionButtonInfo)))
                Dim idx As Integer = 0
                For Each actionButtonList As IEnumerable(Of ActionButtonInfo) In objActionButtons
                    For Each objActionButton As ActionButtonInfo In actionButtonList
                        Dim target As Element = Me.ProcessExtendedCategory(objActionButton.ExtendedCategory)
                        If target.ActionButtons.Count <= idx Then
                            target.ActionButtons.Add(New List(Of ActionButtonInfo))
                        End If
                        target.ActionButtons.Last.Add(objActionButton)
                    Next
                    idx += 1
                Next
            End Sub


        End Class


#End Region


        Public Const SubPathQuery As String = "SubPath"

    End Class

End Namespace
