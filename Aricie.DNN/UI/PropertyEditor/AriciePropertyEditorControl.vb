Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.Collections
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI.WebControls
Imports System.Reflection
Imports Aricie.Services
Imports Aricie.DNN.Services
Imports Aricie.Web.UI
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.Exceptions
Imports System.Web
Imports System.Web.UI
Imports DotNetNuke.UI.UserControls
Imports Aricie.DNN.Security.Trial
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports DotNetNuke.Framework

<Assembly: WebResource("Aricie.DNN.AriciePropertyEditor.css", "text/css", PerformSubstitution:=True)> 
<Assembly: WebResource("Aricie.DNN.AriciePropertyEditorScripts.js", "text/javascript", PerformSubstitution:=True)> 

Namespace UI.WebControls

    Public Class AriciePropertyEditorControl
        Inherits PropertyEditorControl
        Implements System.Web.UI.IPostBackEventHandler, System.Web.UI.IScriptControl


#Region "Fields"


        Private Shared _VisibleCatsByType As New Dictionary(Of Type, List(Of String))
        Private _VisibleCats As List(Of String)
        Private _SectionsCollapsedByDefault As Boolean = True
        Private _SortedUnderLyingDataSource As PropertyInfo()
        Private _TrialStatus As TrialStatusInfo = TrialStatusInfo.NoTrial(TrialConfigInfo.Empty)
        Private _ValidationGroup As String

        Private _ItemChanged As Boolean
        Private _Groups As New Dictionary(Of String, KeyValuePair(Of Control, Control))
        Private _PropertyDepth As Integer
        Private _ParentEditor As PropertyEditorControl
        Private _EnabledOnDemandSections As Boolean = True
        Private _RestrictedFields As New Dictionary(Of AricieFieldEditorControl, TrialLimitedAttribute)
        Private _FieldsDictionary As FieldsHierarchy
        Private _currentSectionName As String = ""
        Private _Validated As Boolean = False
        Private _IsValid As Boolean = True

        Private _DisableExports As Boolean

        Private _JQueryUIVersion As String = ""
        Private _JQueryVersion As String = ""



        Private _PostBackFields As New List(Of String)

        Private _ParentModule As AriciePortalModuleBase

        Private _OnDemandSections As List(Of String)

        Private currentTab As Tab

        Private Const LOAD_COUNTER As String = "LoadCounter"
        Private Const PRERENDER_COUNTER As String = "PreRenderCounter"
        Private Shared _RegisterScriptControlMethod As MethodInfo


        Private Shared ReadOnly Property RegisterScriptControlMethod As MethodInfo
            Get
                If _RegisterScriptControlMethod Is Nothing Then
                    Dim method As MethodInfo = ReflectionHelper.CreateType("System.Web.UI.ScriptManager").GetMethod("RegisterScriptControl")
                    _RegisterScriptControlMethod = method.MakeGenericMethod(GetType(AriciePropertyEditorControl))
                End If
                Return _RegisterScriptControlMethod
            End Get
        End Property




        Private ReadOnly Property SessionVisibleCatsDico() As SerializableDictionary(Of String, Boolean)
            Get
                Dim toReturn As SerializableDictionary(Of String, Boolean) = DirectCast(HttpContext.Current.Session.Item("SessionVisibleCatsDico"), SerializableDictionary(Of String, Boolean))
                If toReturn Is Nothing Then
                    toReturn = New SerializableDictionary(Of String, Boolean)()
                    HttpContext.Current.Session.Add("SessionVisibleCatsDico", toReturn)
                End If
                Return toReturn
            End Get
        End Property





#End Region

#Region "Public Properties"

        Private _FriendlyName As String = ""

        Public ReadOnly Property FriendlyName() As String
            Get
                If _FriendlyName = "" Then
                    _FriendlyName = Me.DataSource.GetType.Name.Replace("`"c, "")
                End If
                Return _FriendlyName
            End Get
        End Property

        Public Property GroupedControls() As Dictionary(Of String, KeyValuePair(Of Control, Control))
            Get
                Return _Groups
            End Get
            Set(ByVal value As Dictionary(Of String, KeyValuePair(Of Control, Control)))
                _Groups = value
            End Set
        End Property


        Public ReadOnly Property FieldsDictionary As FieldsHierarchy
            Get
                If _FieldsDictionary Is Nothing Then
                    _FieldsDictionary = New FieldsHierarchy(Me.UnderlyingDataSource, Me)
                End If

                Return _FieldsDictionary
            End Get
        End Property


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


        Private _isHidden As Boolean

        Public WriteOnly Property IsHidden() As Boolean
            Set(value As Boolean)
                _isHidden = value
            End Set
        End Property



        Public ReadOnly Property ParentModule() As AriciePortalModuleBase
            Get
                If _ParentModule Is Nothing Then
                    _ParentModule = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePortalModuleBase)(Me)
                End If
                Return _ParentModule
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



        Public ReadOnly Property VisibleCats() As List(Of String)
            Get
                If Me._VisibleCats Is Nothing Then
                    If Me.DataSource Is Nothing Then
                        _VisibleCats = New List(Of String)
                    Else
                        Dim dsType As Type = Me.DataSource.GetType
                        If Not _VisibleCatsByType.TryGetValue(dsType, _VisibleCats) Then
                            _VisibleCats = New List(Of String)
                            Dim props As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(dsType)
                            For Each propPair As KeyValuePair(Of String, PropertyInfo) In props
                                If propPair.Value.GetCustomAttributes(GetType(MainCategoryAttribute), False).Length > 0 Then
                                    Dim catAttr As CategoryAttribute = DirectCast(Attribute.GetCustomAttribute(propPair.Value, GetType(CategoryAttribute)), CategoryAttribute)
                                    If catAttr IsNot Nothing Then
                                        If (Not _VisibleCats.Contains(catAttr.Category)) Then
                                            _VisibleCats.Add(catAttr.Category)
                                        End If
                                    End If
                                End If
                            Next
                            _VisibleCatsByType(dsType) = _VisibleCats
                        End If
                    End If
                End If

                Return _VisibleCats
            End Get
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

        Public ReadOnly Property ParentEditor() As PropertyEditorControl
            Get
                If Me._ParentEditor Is Nothing Then
                    Dim parentControl As PropertyEditorControl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of PropertyEditorControl)(Me)
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

#End Region

#Region "Event Handlers"


        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            ' SB: rajout destiné à fournir le style correct à DNN pour les popups d'aide
            If NukeHelper.DnnVersion.Major >= 6 Then
                ' rien à faire pour l'instant
                HelpStyle.CssClass += " dnnFormHelpContent"
            Else
                HelpStyle.CssClass += " Help"
            End If

            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            If Me.ParentModule IsNot Nothing Then
                Me.ParentModule.AdvancedCounter(Me, LOAD_COUNTER) += 1
            End If
        End Sub


        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Try
                Dim args As String() = Strings.Split(eventArgument, ClientAPI.COLUMN_DELIMITER)
                If args.Length = 2 AndAlso args(0) = "expand" Then
                    Dim sectionName As String = args(1)

                    Dim s As Section = Me.FieldsDictionary.GetSectionByName(sectionName)

                    _currentSectionName = s.Name
                    AddColumns(s, Me.FieldsDictionary.Sections(s), s.Container, False)

                ElseIf args.Length = 2 AndAlso args(0) = "tabChange" Then
                    Dim tabName As String = args(1)


                    Dim t As Tab = Me.FieldsDictionary.GetTabByName(tabName)
                    If t IsNot Nothing Then
                        If currentTab IsNot Nothing Then
                            If currentTab Is t Then
                                Exit Sub
                            End If
                            currentTab.HeaderLink.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(Me, "tabChange" & ClientAPI.COLUMN_DELIMITER & currentTab.Name))
                        End If
                        Dim cookieName As String = "cookieTab" & Me.ClientID.GetHashCode()
                        Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(cookieName)
                        If cookie IsNot Nothing Then
                            HttpContext.Current.Request.Cookies.Remove(cookieName)
                        End If
                        currentTab = t
                        cookie = New HttpCookie(cookieName)
                        cookie.Value = Me.FieldsDictionary.Tabs.IndexOf(t).ToString()
                        HttpContext.Current.Request.Cookies.Add(cookie)

                        Me.FieldsDictionary.Tabs.HideAll()

                        AddSections(Me.FieldsDictionary.Tabs(t), t.Container, False)
                        t.HeaderLink.Attributes.Remove("onclick")
                        t.Loaded = True
                    End If

                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Overrides Sub ListItemChanged(ByVal sender As Object, ByVal e As DotNetNuke.UI.WebControls.PropertyEditorEventArgs)
            MyBase.ListItemChanged(sender, e)
            ItemChanged = True
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            Try
                If ItemChanged Or Me.IsDirty Then
                    'quelle utilité ?
                    'Jesse: nécessaire pour prendre en compte les modifications postérieures aux event handlers
                    DataBind()
                End If

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
                If Me._SectionsCollapsedByDefault Then
                    Dim isFirstPass As Boolean = True
                    If Me.ParentModule IsNot Nothing Then
                        Me.ParentModule.AdvancedCounter(Me, PRERENDER_COUNTER) += 1
                        If Me.ParentModule.AdvancedCounter(Me, LOAD_COUNTER) > Me.ParentModule.AdvancedCounter(Me, PRERENDER_COUNTER) Then
                            Me.ParentModule.AdvancedCounter(Me, LOAD_COUNTER) = 1
                            Me.ParentModule.AdvancedCounter(Me, PRERENDER_COUNTER) = 1
                        End If
                        isFirstPass = Me.ParentModule.AdvancedCounter(Me, PRERENDER_COUNTER) = 1
                    End If


                    For Each objSection As KeyValuePair(Of Section, Columns) In Me.FieldsDictionary.Sections
                        Dim section As Section = objSection.Key

                        If Not VisibleCats.Contains(section.Name) Then
                            If isFirstPass Then
                                Select Case NukeHelper.DnnVersion.Major
                                    Case Is > 5
                                        For Each method As MethodInfo In ReflectionHelper.GetFullMembersDictionary(GetType(DNNClientAPI))("MinMaxContentVisibile")
                                            Dim paramList() As ParameterInfo = method.GetParameters
                                            If paramList.Length = 5 Then
                                                Dim param() As Object = {section.Image, -1, True, DNNClientAPI.MinMaxPersistanceType.Page, False}
                                                method.Invoke(Nothing, param)
                                                Exit For
                                            End If
                                        Next
                                    Case Else
                                        Dim clientApiProperty As PropertyInfo = ReflectionHelper.GetPropertiesDictionary(GetType(DNNClientAPI))("MinMaxContentVisibile")
                                        Dim param() As Object = {section.Image, True, DNNClientAPI.MinMaxPersistanceType.Page}
                                        clientApiProperty.SetValue(Nothing, False, param)
                                End Select
                            End If
                        End If
                    Next
                End If

                For Each t As Tab In Me.FieldsDictionary.Tabs.Keys
                    For Each c As Control In t.Container.Controls
                        c.Visible = t.Loaded
                    Next
                Next


                For Each objSection As KeyValuePair(Of Section, Columns) In Me.FieldsDictionary.Sections
                    DNNClientAPI.EnableMinMax(objSection.Key.Image, objSection.Key.Container, False, Me.Page.ResolveUrl("~/images/minus.gif"), Me.Page.ResolveUrl("~/images/plus.gif"), DNNClientAPI.MinMaxPersistanceType.Page)
                Next


                If Me._EnabledOnDemandSections Then
                    For Each sectionName As String In OnDemandSections
                        Dim objSection As KeyValuePair(Of Control, Control) = Me.GroupedControls(sectionName)
                        objSection.Value.Visible = False
                        'needed only on asp.net ajax 1.0 otherwise see aricieportalmodulebase
                        Globals.SetAttribute(objSection.Key, "onClick", "dnn.vars=null;" & ClientAPI.GetPostBackClientHyperlink(Me, "expand" & ClientAPI.COLUMN_DELIMITER & sectionName))
                    Next
                End If


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
                        Page.Header.Controls.Add(lnk)

                    End If
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
            If Not Me.DesignMode Then

                'SB: modification pour fonctionner sous DNN 4.9.3 (version eshop.aricie.info)
                Dim sm As Control = ScriptManager.GetCurrent(Page)
                If (sm Is Nothing AndAlso NukeHelper.DnnVersion.Major < 5) Then
                    sm = AJAX.ScriptManagerControl(Page)
                End If

                If sm Is Nothing Then
                    Throw New HttpException("A ScriptManager control must exist on the page.")
                End If

                RegisterScriptControlMethod.Invoke(sm, New Object() {Me})

            End If

            If NukeHelper.DnnVersion.Major > 6 And Me Is ParentAricieEditor Then
                Me.CssClass += " dnnForm"
            End If
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

        Public Overrides Sub DataBind()

            'Invoke OnDataBinding so DataBinding Event is raised
            MyBase.OnDataBinding(EventArgs.Empty)

            'Clear Existing Controls
            Controls.Clear()

            'Start Tracking ViewState
            TrackViewState()

            'Create the Editor
            CreateEditor()

            'Add the commands buttons
            Me.AddCommandButtons()


            'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
            ChildControlsCreated = True
        End Sub

#End Region

#Region "Overrides"


        Protected Overrides ReadOnly Property UnderlyingDataSource() As System.Collections.IEnumerable
            Get
                If _SortedUnderLyingDataSource Is Nothing Then
                    _SortedUnderLyingDataSource = DirectCast(MyBase.UnderlyingDataSource, PropertyInfo())
                    If _SortedUnderLyingDataSource IsNot Nothing Then
                        _SortedUnderLyingDataSource = Array.FindAll(Of PropertyInfo)(_SortedUnderLyingDataSource, Function(objProp As PropertyInfo) As Boolean
                                                                                                                      Return objProp.GetIndexParameters().Length = 0
                                                                                                                  End Function)
                        Array.Sort(_SortedUnderLyingDataSource, New AriciePropertySortOrderComparer(_SortedUnderLyingDataSource))
                    End If
                End If
                Return _SortedUnderLyingDataSource
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
            Dim toReturn As Boolean = MyBase.GetRowVisibility(obj)
            If toReturn Then
                Dim info As PropertyInfo = DirectCast(obj, PropertyInfo)
                'If ReflectionHelper.IsTrueReferenceType(info.PropertyType) AndAlso info.GetValue(Me.DataSource, Nothing) Is Nothing Then
                If info.GetValue(Me.DataSource, Nothing) Is Nothing Then
                    Return False
                Else
                    Dim flag2 As Boolean = True
                    Dim customAttributes As Object() = info.GetCustomAttributes(GetType(ConditionalVisibleAttribute), True)
                    If (customAttributes.Length > 0) Then
                        For Each condAttribute As ConditionalVisibleAttribute In customAttributes
                            Dim props As Dictionary(Of String, PropertyInfo) = ReflectionHelper.GetPropertiesDictionary(Me.DataSource.GetType)
                            Dim objPropInfo As PropertyInfo = Nothing
                            If props.TryGetValue(condAttribute.MasterPropertyName, objPropInfo) Then
                                customAttributes = objPropInfo.GetCustomAttributes(GetType(ConditionalVisibleAttribute), True)
                                If (customAttributes.Length > 0) AndAlso Not Me.GetRowVisibility(objPropInfo) Then
                                    toReturn = False
                                Else
                                    Dim value As Object = objPropInfo.GetValue(Me.DataSource, Nothing)
                                    toReturn = condAttribute.MatchValue(value)
                                    If toReturn AndAlso condAttribute.SecondaryPropertyName <> "" Then
                                        If props.TryGetValue(condAttribute.SecondaryPropertyName, objPropInfo) Then
                                            value = objPropInfo.GetValue(Me.DataSource, Nothing)
                                            toReturn = condAttribute.MatchSecondary(value)
                                        End If
                                    End If
                                End If
                                If condAttribute.EnforceAutoPostBack Then
                                    _PostBackFields.Add(condAttribute.MasterPropertyName)
                                    If condAttribute.SecondaryPropertyName <> "" Then
                                        _PostBackFields.Add(condAttribute.SecondaryPropertyName)
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If
            Return toReturn
        End Function


        Protected Overrides Sub CreateEditor()
            Me.CssClass = "dnnForm"
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
                            lnk.Href = "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.9/themes/flick/jquery-ui.css"
                            lnk.Attributes.Add("type", "text/css")
                            lnk.Attributes.Add("rel", "stylesheet")
                            Page.Header.Controls.Add(lnk)
                        End If

                End If
                AddTabs()

                'Me.AddCommandButtons()
            End If




            Validate()
        End Sub

       

#End Region

#Region "Inner methods"


        Protected Sub AddTabs()

            Dim propertyContainer As Control = Me
            AddSections(Me.FieldsDictionary.Tabs.NotInTab, propertyContainer, False)
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("cookieTab" & Me.ClientID.GetHashCode())

            If Me.FieldsDictionary.Tabs.Count > 0 Then
                Dim loopTabIndex = 0
                Dim currentTabIndex As Integer = 0
                If cookie IsNot Nothing Then
                    Integer.TryParse(cookie.Value, currentTabIndex)
                End If

                Dim tabsContainer As New HtmlGenericControl("div")
                'tabsContainer.EnableViewState = False
                Dim tabsHeader As New HtmlGenericControl("ul")
                tabsHeader.EnableViewState = False

                tabsContainer.Attributes.Add("class", "aricie_pe_tabs-" & Me.ClientID)
                tabsContainer.Attributes.Add("hash", Me.ClientID.GetHashCode().ToString())

                Me.Controls.Add(tabsContainer)
                tabsContainer.Controls.Add(tabsHeader)
                Dim tabIshidden As Boolean
                Dim isFirstPass As Boolean = FormHelper.IsFirstPass(Me, Me.ParentModule)

                For Each tabSectionsPair As KeyValuePair(Of Tab, Sections) In Me.FieldsDictionary.Tabs

                    Dim tabContainer As New HtmlGenericControl("div")
                    'tabContainer.EnableViewState = False
                    tabContainer.ID = String.Format("{0}_{1}", Me.FriendlyName, tabSectionsPair.Key.Name.Replace(" ", ""))
                    tabsContainer.Controls.Add(tabContainer)
                    tabContainer.ID = tabContainer.ClientID.GetHashCode.ToString
                    'Dim tabId As String = String.Format("{0}_{1}", Me.FriendlyName, tabSectionsPair.Key.Name.Replace(" ", ""))
                    'tabContainer.Attributes.Add("id", tabId)



                    tabSectionsPair.Key.Container = tabContainer


                    Dim headerLink As New System.Web.UI.HtmlControls.HtmlGenericControl("a")
                    headerLink.EnableViewState = False
                    headerLink.Attributes.Add("href", "#" & tabContainer.ClientID)

                    Dim loadTab As Boolean = False
                    Dim displayTab As Boolean = False
                    If currentTabIndex = loopTabIndex Then
                        tabIshidden = Me._isHidden
                        displayTab = True
                        loadTab = True
                        currentTab = tabSectionsPair.Key
                    Else
                        tabIshidden = True
                        loadTab = False ' (Not _EnabledOnDemandSections) OrElse (Not _isHidden AndAlso isFirstPass)
                        ' désactivation du chargement transversal des onglets
                        displayTab = Me.HasVisibleContent(tabSectionsPair.Value)
                    End If

                    If displayTab Then
                        If loadTab Then
                            tabSectionsPair.Key.Loaded = True
                            propertyContainer = tabContainer
                            AddSections(tabSectionsPair.Value, propertyContainer, tabIshidden)
                        Else
                            headerLink.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(Me, "tabChange" & ClientAPI.COLUMN_DELIMITER & tabSectionsPair.Key.Name))
                        End If
                    End If

                    'quoi qu'il arrive, on ajoute l'onglet à la collection et on le passe en caché (mais dans le html) si nécessaire

                    Dim resKey As String = tabSectionsPair.Key.ResourceKey
                    If String.IsNullOrEmpty(resKey) Then
                        resKey = Me.FriendlyName & "_" & tabSectionsPair.Key.Name & ".Header"
                    End If

                    headerLink.InnerText = Localization.GetString(resKey, Me.LocalResourceFile)

                    Dim li = New HtmlGenericControl("li")
                    If Not displayTab Then
                        tabContainer.Visible = False
                        li.Style("display") = "none"
                    End If
                    tabsHeader.Controls.Add(li)
                    li.Controls.Add(headerLink)
                    tabSectionsPair.Key.HeaderLink = headerLink

                    loopTabIndex += 1
                Next
            End If



        End Sub

        Protected Sub AddCommandButtons()
            If Me.DataSource IsNot Nothing Then
                Dim buttonMethods As Dictionary(Of MethodInfo, ActionButtonAttribute) = CacheHelper.GetGlobal(Of Dictionary(Of MethodInfo, ActionButtonAttribute))(Me.DataSource.GetType.FullName)
                If buttonMethods Is Nothing Then
                    buttonMethods = New Dictionary(Of MethodInfo, ActionButtonAttribute)
                    Dim members As Dictionary(Of String, MemberInfo) = ReflectionHelper.GetMembersDictionary(Me.DataSource.GetType())

                    For Each member As KeyValuePair(Of String, MemberInfo) In members
                        If TypeOf member.Value Is MethodInfo Then
                            Dim method As MethodInfo = DirectCast(member.Value, MethodInfo)
                            If method IsNot Nothing AndAlso method.IsPublic Then
                                Dim attrs As Attribute() = CType(method.GetCustomAttributes(GetType(ActionButtonAttribute), True), Attribute())
                                If attrs.Length > 0 Then
                                    Dim attr As ActionButtonAttribute = DirectCast(attrs(0), ActionButtonAttribute)
                                    buttonMethods(method) = attr
                                End If
                            End If
                        End If
                    Next
                    CacheHelper.SetGlobal(Of Dictionary(Of MethodInfo, ActionButtonAttribute))(buttonMethods, Me.DataSource.GetType.FullName)
                End If
                Dim buttonC As Panel = Nothing
                For Each buttonMethod As KeyValuePair(Of MethodInfo, ActionButtonAttribute) In buttonMethods
                    Dim method As MethodInfo = buttonMethod.Key
                    If buttonC Is Nothing Then
                        buttonC = New Panel
                        buttonC.EnableViewState = False
                        buttonC.ID = "divCmdButtons"
                        buttonC.CssClass = "CommandsButtons DNNAligncenter"
                        Me.Controls.Add(buttonC)
                    End If
                    Dim btn As New CommandButton()
                    btn.ID = "cmd" & method.Name

                    btn.ResourceKey = Me.DataSource.GetType().Name & "_" & method.Name & ".Text"
                    If buttonMethod.Value.IconPath <> "" Then
                        btn.ImageUrl = buttonMethod.Value.IconPath
                    End If
                    buttonC.Controls.Add(btn)

                    Dim params As ParameterInfo() = method.GetParameters()
                    Dim paramInstance As Object = Nothing
                    If params.Length = 1 Then
                        Dim objParam As ParameterInfo = params(0)
                        If objParam.ParameterType Is GetType(AriciePortalModuleBase) Then
                            paramInstance = Me.ParentModule
                        ElseIf objParam.ParameterType Is GetType(AriciePropertyEditorControl) Then
                            paramInstance = Me
                        End If
                    End If

                    AddHandler btn.Click, Sub(s As Object, e As EventArgs)
                                              If paramInstance Is Nothing Then
                                                  method.Invoke(Me.DataSource, Nothing)
                                              Else
                                                  method.Invoke(Me.DataSource, New Object() {paramInstance})
                                              End If
                                          End Sub


                Next
            End If


        End Sub

        Protected Function AddSections(ByVal sections As Sections, ByVal container As Control, ByVal isHidden As Boolean) As Integer

            Dim nbControls As Integer
            nbControls += AddColumns(Nothing, sections.NotInSection, container, isHidden)

            If sections.Count > 0 Then
                Dim sectionContainer As New HtmlGenericControl("div")
                'sectionContainer.EnableViewState = False
                'sectionContainer.Attributes.Add("class", "aricie_pe_sections")
                container.Controls.Add(sectionContainer)
                container = sectionContainer
            End If

            For Each s As KeyValuePair(Of Section, Columns) In sections


                nbControls += AddColumns(s.Key, s.Value, container, isHidden)

            Next
            Return nbControls
        End Function


        Protected Function AddColumns(ByVal section As Section, ByVal columns As Columns, ByVal container As Control, ByVal isHidden As Boolean) As Integer
            ' on va sauvegarder l'élément racine pour pouvoir insérer le header si nécessaire
            Dim rootContainer As Control = container
            ' et le header pour pouvoir l'effacer si nécessaire
            Dim headerControl As Control = Nothing

            ' ajout de colonnes multiples
            Dim multipleColumnsExist As Boolean = columns.Count > 1
            If section IsNot Nothing Then

                ' commençons avant toute chose par rajouter le header control, on le retirera plus tard si nécessaire
                AddHeaderCt(rootContainer, section.Name)
                headerControl = rootContainer.Controls(rootContainer.Controls.Count - 1)

                Dim div As New HtmlGenericControl("div")
                'div.EnableViewState = False
                container.Controls.Add(div)
                section.Container = div
                section.Image = CType(container.FindControl("ico" & section.Name), Image)

                container = div
            End If

            Dim counter As Integer = 0
            Dim columnMaxWidth As Integer = CInt(Math.Floor(100 / Math.Max(columns.Count, 1)))

            For Each c As Integer In columns.Keys
                Dim currentContainer As Control = container
                ' si on a plusieurs colonnes, le container doit être modifié pour prendre un div de la taille de la colonne (width / nb de colonnes)
                If (multipleColumnsExist) Then

                    Dim col As New HtmlGenericControl("div")
                    'col.EnableViewState = False
                    col.Style.Add("float", "left")
                    col.Style.Add("Width", String.Format("{0}%", columnMaxWidth.ToString))

                    currentContainer = DirectCast(col, Control)
                End If

                For Each p As PropertyInfo In columns(c)
                    If Me.GetRowVisibility(p) Then
                        Me.AddEditorCtl(currentContainer, p, isHidden)
                        counter += 1
                    End If
                Next

                ' et une fois le container colonne rempli, remettons le dans le contrôle originel
                If multipleColumnsExist Then
                    container.Controls.Add(currentContainer)
                End If
            Next

            ' on a x contrôles rajoutés, mais si il n'y a eu qu'un header rajouté, on le retire, il n'a aucun enfant.
            If (counter = 0 AndAlso headerControl IsNot Nothing) Then
                rootContainer.Controls.Remove(headerControl)
            End If

            Return counter
        End Function






        Protected Sub AddHeaderCt(ByVal container As Control, ByVal header As String)

            If header <> "" Then
                Dim child As New Panel
                child.CssClass = "HeaderPl"
                child.EnableViewState = False
                Dim image As New Image
                image.ID = ("ico" & header)
                image.EnableViewState = False
                Dim literal As New Literal
                literal.Text = " "
                literal.EnableViewState = False
                Dim label As New Label
                label.ID = ("lbl" & header)
                label.Attributes.Item("resourcekey") = (Me.FriendlyName & "_" & header & ".Header")
                label.Text = header
                label.EnableViewState = False
                label.ControlStyle.CopyFrom(Me.GroupHeaderStyle)
                child.Controls.Add(image)
                child.Controls.Add(literal)
                child.Controls.Add(label)
                If Me.GroupHeaderIncludeRule Then
                    child.Controls.Add(New LiteralControl("<hr noshade=""noshade"" size=""1""/>"))
                End If
                container.Controls.Add(child)
            End If
        End Sub

        Protected Sub AddEditorCtl(ByRef container As Control, ByVal obj As Object, ByVal isHidden As Boolean)
            Dim objPropertyInfo As PropertyInfo = DirectCast(obj, PropertyInfo)
            Try

                Me.AddEditorCtl(container, objPropertyInfo.Name, New AricieStandardEditorInfoAdapter(Me.DataSource, objPropertyInfo.Name), isHidden)
                Dim customAttributes As Object()
                If Me._TrialStatus.IsLimited Then
                    customAttributes = objPropertyInfo.GetCustomAttributes(GetType(TrialLimitedAttribute), True)
                    If (customAttributes.Length > 0) Then
                        Dim objAttr As TrialLimitedAttribute = Nothing
                        objAttr = DirectCast(customAttributes(0), TrialLimitedAttribute)
                        If objAttr IsNot Nothing Then
                            Dim objField As AricieFieldEditorControl = DirectCast(Me.Fields(Me.Fields.Count - 1), AricieFieldEditorControl)
                            Me._RestrictedFields(objField) = objAttr

                        End If
                    End If
                End If
            Catch ex As Exception
                ProcessModuleLoadException("Empty Reference Property: " & objPropertyInfo.Name, Me, ex)
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

        Protected Sub AddEditorCtl(ByRef container As Control, ByVal name As String, ByVal editorAdapter As IEditorInfoAdapter, ByVal isHidden As Boolean)
            Dim myCtDiv As New HtmlControls.HtmlGenericControl("div")
            'myCtDiv.EnableViewState = False
            myCtDiv.Attributes.Add("class", "fieldC")
            container.Controls.Add(myCtDiv)
            Dim control As New AricieFieldEditorControl
            'control.ID = "field" + name
            'control.ID = name.Substring(0, 3) & name.Substring(name.Length - 2)
            control.ID = name ' la méthode au dessus ne marche pas, EnableTheseSettings et EnableTheseOtherSettings provoque une collision
            control.IsHidden = isHidden
            control.DataSource = Me.DataSource
            control.EditorInfoAdapter = editorAdapter
            control.DataField = name
            control.EditorDisplayMode = Me.DisplayMode
            control.EnableClientValidation = Me.EnableClientValidation
            control.EditMode = Me.EditMode
            control.HelpDisplayMode = Me.HelpDisplayMode
            control.LabelWidth = Me.LabelWidth
            control.LabelMode = LabelMode.Left
            control.LabelStyle.CopyFrom(Me.LabelStyle)
            control.HelpStyle.CopyFrom(Me.HelpStyle)
            control.ErrorStyle.CopyFrom(Me.ErrorStyle)
            control.VisibilityStyle.CopyFrom(Me.VisibilityStyle)
            control.EditControlStyle.CopyFrom(Me.EditControlStyle)
            control.EditControlWidth = Me.EditControlWidth
            control.LocalResourceFile = Me.LocalResourceFile
            control.RequiredUrl = Me.RequiredUrl
            control.ShowRequired = Me.ShowRequired
            control.ShowVisibility = Me.ShowVisibility
            AddHandler control.ItemChanged, New DotNetNuke.UI.WebControls.PropertyChangedEventHandler(AddressOf Me.ListItemChanged)
            AddHandler control.ItemCreated, New EditorCreatedEventHandler(AddressOf Me.EditorItemCreated)


            Me.Fields.Add(control)
            myCtDiv.Controls.Add(control)
            control.DataBind()

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

        Public Shared Function GetParentSection(ByVal childControl As Control) As KeyValuePair(Of Control, Control)
            Dim parentPH As AriciePropertyEditorControl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of AriciePropertyEditorControl)(childControl)
            If parentPH Is Nothing Then
                Return Nothing
            End If
            Dim tbl As HtmlGenericControl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of HtmlGenericControl)(childControl)
            If tbl Is Nothing Then
                Return Nothing
            End If
            While tbl.Parent IsNot parentPH
                tbl = Aricie.Web.UI.ControlHelper.FindControlRecursive(Of HtmlGenericControl)(tbl)
                If tbl Is Nothing Then
                    Return Nothing
                End If
            End While
            If tbl.ID Is Nothing Then
                Return Nothing
            End If
            If tbl.ID.StartsWith("pl") AndAlso tbl.ID.Length > 2 Then
                Dim parentSectionName As String = tbl.ID.Substring(2)
                Return parentPH.GroupedControls(parentSectionName)
            End If
            Return GetParentSection(parentPH)

        End Function

        Public Sub LoadJQuery()
            If Me._JQueryVersion <> "" AndAlso Me._JQueryUIVersion <> "" Then
                FormHelper.LoadjQuery(Me.Page, Me._JQueryVersion, Me._JQueryUIVersion)
            Else
                FormHelper.LoadjQuery(Me.Page)
            End If
        End Sub

        Private Sub EnforceTrialMode()
            Dim WarningMessage As String = Localization.GetString(TrialLimitedAttribute.TrialModeLimitedKey, LocalResourceFile)
            If String.IsNullOrEmpty(WarningMessage) Then
                WarningMessage = TrialLimitedAttribute.TrialModeLimitedKey
            End If

            For Each restrictedField As KeyValuePair(Of AricieFieldEditorControl, TrialLimitedAttribute) In Me._RestrictedFields
                Dim objAttr As TrialLimitedAttribute = restrictedField.Value
                Dim objField As AricieFieldEditorControl = restrictedField.Key

                If TypeOf objField.Editor Is UserControlEditControl Then
                    ' si c'est un contrôle utilisateur dans le propety editor, on lui passe juste l'information nécessaire
                    DirectCast(objField.Editor, UserControlEditControl).TrialMode = New UserControlEditControl.TrialModeInformation() With {.CurrentTrialMode = objAttr.TrialPropertyMode, .Message = WarningMessage}
                Else
                    'sinon on effectue le traitement classique
                    objField.ToolTip = WarningMessage ' on fixe le tooltip sur toute la ligne
                    objField.CssClass = objField.CssClass & " PETrialDisabled" ' on rajoute une classe

                    objField.Editor.CssClass = objField.Editor.CssClass & " PETrialEditorDisabled" ' on rajoute une classe sur l'éditeur uniquement
                    If objField.ShowRequired AndAlso objField.Editor.Required AndAlso objField.Editor.Parent.Controls.Count >= 2 AndAlso TypeOf objField.Editor.Parent.Controls(1) Is Image Then
                        objField.Editor.Parent.Controls(1).Visible = False ' on a pas besoin de l'image puisqu'on a désactivé l'éditeur
                    End If


                    If (objAttr.TrialPropertyMode And TrialPropertyMode.Hide) = TrialPropertyMode.Hide Then
                        objField.Visible = False
                    ElseIf (objAttr.TrialPropertyMode And TrialPropertyMode.Disable) = TrialPropertyMode.Disable Then
                        objField.Enabled = False
                        objField.EditMode = PropertyEditorMode.View
                        objField.Editor.EditMode = PropertyEditorMode.View
                        objField.Editor.Required = False
                    Else
                        If (objAttr.TrialPropertyMode And TrialPropertyMode.NoAdd) = TrialPropertyMode.NoAdd _
                                OrElse (objAttr.TrialPropertyMode And TrialPropertyMode.NoDelete) = TrialPropertyMode.NoDelete Then
                            Dim collectEditCtl As CollectionEditControl = Nothing
                            If TypeOf objField.Editor Is Aricie.DNN.UI.WebControls.EditControls.CollectionEditControl Then
                                collectEditCtl = DirectCast(objField.Editor, CollectionEditControl)
                            ElseIf TypeOf objField.Editor Is Aricie.DNN.UI.WebControls.EditControls.PropertyEditorEditControl Then
                                For Each subField As AricieFieldEditorControl In DirectCast(objField.Editor, PropertyEditorEditControl).InnerEditor.Fields
                                    If TypeOf subField.Editor Is Aricie.DNN.UI.WebControls.EditControls.CollectionEditControl Then
                                        collectEditCtl = DirectCast(subField.Editor, CollectionEditControl)
                                    End If
                                Next
                            End If
                            If collectEditCtl IsNot Nothing Then
                                If (objAttr.TrialPropertyMode And TrialPropertyMode.NoAdd) = TrialPropertyMode.NoAdd Then
                                    collectEditCtl.cmdAddButton.Enabled = False
                                End If
                                If (objAttr.TrialPropertyMode And TrialPropertyMode.NoDelete) = TrialPropertyMode.NoDelete Then
                                    For Each deleteControl As WebControl In collectEditCtl.DeleteControls
                                        deleteControl.Enabled = False
                                    Next
                                End If
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        Private Function HasVisibleContent(ByVal objSections As Sections) As Boolean
            For Each objProps As IList(Of PropertyInfo) In objSections.NotInSection.Values
                For Each objProp As PropertyInfo In objProps
                    If Me.GetRowVisibility(objProp) Then
                        Return True
                    End If
                Next

            Next

            For Each objSection As Columns In objSections.Values
                For Each objProps As IList(Of PropertyInfo) In objSection.Values
                    For Each objProp As PropertyInfo In objProps
                        If Me.GetRowVisibility(objProp) Then
                            Return True
                        End If
                    Next
                Next
            Next
        End Function
#End Region

#Region "IScriptControl"


        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors

            Dim toReturn As New List(Of ScriptDescriptor)
            Dim descriptor As ScriptControlDescriptor = New ScriptControlDescriptor("Aricie.DNN.AriciePropertyEditorScripts", Me.ClientID)

            descriptor.AddProperty("clientId", Me.ClientID)
            descriptor.AddProperty("hash", Me.ClientID.GetHashCode().ToString())

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
            Private _Loaded As Boolean = False
            Private _ResourceKey As String

            Public Sub New(ByVal name As String, ByVal container As Control)
                _Name = name
                _Container = container
            End Sub

            Public Sub New(ByVal name As String, ByVal prop As PropertyInfo)
                _Name = name
                If prop IsNot Nothing Then
                    Me._ResourceKey = String.Format("{0}_{1}.Header", prop.DeclaringType.Name, name)
                End If
            End Sub

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal value As String)
                    _Name = value
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
                Get
                    Return _Loaded
                End Get
                Set(ByVal value As Boolean)
                    _Loaded = value
                End Set
            End Property



            Public Property ResourceKey() As String
                Get
                    Return _ResourceKey
                End Get
                Set(ByVal value As String)
                    _ResourceKey = value
                End Set
            End Property


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

            Sub New(ByVal name As String, ByVal prop As PropertyInfo)
                MyBase.New(name, prop)
            End Sub


            Public Shared ReadOnly Empty As New Tab("", Nothing)

            Public Shared Function FromName(ByVal name As String, ByVal prop As PropertyInfo) As Tab
                Return New Tab(name, prop)
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

            Private _Image As Image

            Public Sub New(ByVal name As String, ByVal prop As PropertyInfo)
                MyBase.New(name, prop)
            End Sub

            Public Property Image() As Image
                Get
                    Return _Image
                End Get
                Set(ByVal value As Image)
                    _Image = value
                End Set
            End Property

            Public Shared ReadOnly Empty As New Section("", Nothing)

            Public Shared Function FromName(ByVal name As String, ByVal prop As PropertyInfo) As Section
                Return New Section(name, prop)
            End Function

        End Class

        Public Class Tabs
            Inherits Dictionary(Of Tab, Sections)

            Private _NotInTab As New Sections

            Public ReadOnly Property NotInTab() As Sections
                Get
                    Return _NotInTab
                End Get
            End Property

            Public Sub HideAll()
                For Each t As Tab In Me.Keys
                    t.Loaded = False
                Next
            End Sub

            Public Function IndexOf(ByVal t As Tab) As Integer

                Dim list As New List(Of Tab)(Me.Keys)

                Return list.IndexOf(t)

            End Function

        End Class


        Public Class Sections
            Inherits Dictionary(Of Section, Columns)

            Private _NotInSection As New Columns

            Public ReadOnly Property NotInSection() As Columns
                Get
                    Return _NotInSection
                End Get
            End Property
        End Class

        Public Class Columns
            Inherits Dictionary(Of Integer, List(Of PropertyInfo))
        End Class


        Public Class FieldsHierarchy

            Private _Tabs As New Tabs
            Private _Sections As New Sections
            Private _Editor As AriciePropertyEditorControl


            Public Sub New(ByVal underlyingDatasource As IEnumerable, ByVal editor As AriciePropertyEditorControl)
                Me._Editor = editor
                If underlyingDatasource IsNot Nothing Then
                    For Each p As PropertyInfo In underlyingDatasource
                        'If _Editor.IsPropertyVisible(p) Then
                        Dim added As Boolean = False

                        Dim customAttributes As Object() = p.GetCustomAttributes(GetType(CategoryAttribute), True)
                        If customAttributes.Length > 0 Then
                            Dim categoryAttr As CategoryAttribute = CType(customAttributes(0), CategoryAttribute)

                            Dim columns As Columns = _Tabs.NotInTab.NotInSection



                            If Not _Tabs.NotInTab.ContainsKey(Section.FromName(categoryAttr.Category, p)) And Not String.IsNullOrEmpty(categoryAttr.Category) Then

                                Dim s As Section = Section.FromName(categoryAttr.Category, p)
                                _Tabs.NotInTab.Add(s, New Columns)
                                _Sections.Add(s, _Tabs.NotInTab(s))
                            End If

                            If Not String.IsNullOrEmpty(categoryAttr.Category) Then
                                columns = _Tabs.NotInTab(Section.FromName(categoryAttr.Category, p))
                            End If

                            If Not columns.ContainsKey(0) Then
                                columns.Add(0, New List(Of PropertyInfo))
                            End If

                            columns(0).Add(p)

                            added = True
                        End If


                        customAttributes = p.GetCustomAttributes(GetType(ExtendedCategoryAttribute), True)
                        If customAttributes.Length > 0 Then
                            Dim categoryAttr As ExtendedCategoryAttribute = CType(customAttributes(0), ExtendedCategoryAttribute)

                            Dim sections As Sections = _Tabs.NotInTab
                            Dim columns As Columns = Nothing

                            If Not Me.Tabs.ContainsKey(Tab.FromName(categoryAttr.TabName, p)) And Not Tab.FromName(categoryAttr.TabName, p).Equals(Tab.Empty) Then
                                Me.Tabs.Add(Tab.FromName(categoryAttr.TabName, p), New Sections)
                            End If

                            If Not Tab.FromName(categoryAttr.TabName, p).Equals(Tab.Empty) Then
                                sections = Me.Tabs(Tab.FromName(categoryAttr.TabName, p))
                            End If

                            If Not String.IsNullOrEmpty(categoryAttr.SectionName) Then
                                Dim s As Section = Section.FromName(categoryAttr.SectionName, p)

                                If Not sections.ContainsKey(Section.FromName(categoryAttr.SectionName, p)) Then
                                    sections.Add(s, New Columns)
                                    _Sections.Add(s, sections(s))
                                End If

                                columns = sections(s)
                            Else
                                columns = sections.NotInSection
                            End If



                            If Not columns.ContainsKey(categoryAttr.Column) Then
                                columns.Add(categoryAttr.Column, New List(Of PropertyInfo))
                            End If

                            columns(categoryAttr.Column).Add(p)

                            added = True
                        End If

                        If Not added Then
                            If Not _Tabs.NotInTab.NotInSection.ContainsKey(0) Then
                                _Tabs.NotInTab.NotInSection.Add(0, New List(Of PropertyInfo))
                            End If

                            _Tabs.NotInTab.NotInSection(0).Add(p)

                        End If


                        'End If


                    Next
                End If


            End Sub

            Public ReadOnly Property Tabs() As Tabs
                Get
                    Return _Tabs
                End Get
            End Property

            Public ReadOnly Property Sections() As Sections
                Get
                    Return _Sections
                End Get
            End Property

            Public Function GetTabByName(ByVal name As String) As Tab

                For Each t As Tab In Me._Tabs.Keys
                    If t.Name = name Then
                        Return t
                    End If
                Next

                Return Nothing

            End Function

            Public Function GetSectionByName(ByVal name As String) As Section

                For Each s As Section In Me._Sections.Keys
                    If s.Name = name Then
                        Return s
                    End If
                Next

                Return Nothing

            End Function

        End Class


#End Region


    End Class

End Namespace
