Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Aricie.DNN.UI.Controls
Imports Aricie.DNN.Services
Imports Aricie.Collections
Imports DotNetNuke.Entities.Modules
Imports Aricie.Web.UI
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.UserControls
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.UI.Utilities
Imports System.Globalization
Imports Aricie.Services
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web

Namespace UI
    Public Module FormHelper
        Public Function BindEnumsListControls(ByVal listControls As ListControl(), ByVal correspondingEnums As Type(), _
                                               ByVal resourceFile As String) As ArrayList
            Dim toReturn As New ArrayList
            For i As Integer = 0 To listControls.Length - 1
                toReturn.Add(BindEnumListControl(listControls(i), correspondingEnums(i), resourceFile))
            Next
            Return toReturn
        End Function

        Public Function BindEnumListControl(ByVal listControl As ListControl, ByVal enumType As Type, _
                                             ByVal resourceFile As String) As String()
            Dim enumNames As String() = [Enum].GetNames(enumType)

            Dim dataSource As ListItemCollection = FormatDataSource(Localize(enumNames, resourceFile), enumNames)
            listControl.DataTextField = "Text"
            listControl.DataValueField = "Value"
            listControl.DataSource = dataSource
            listControl.DataBind()

            Return enumNames
        End Function

        Public Function BindEnumListControl(Of T)(ByVal listControl As ListControl, ByVal resourceFile As String) _
            As String()
            Return BindEnumListControl(listControl, GetType(T), resourceFile)
        End Function


        'easier but problem -> type as hash key seems very heavy...
        'Public Function BindEnumsListControls(ByRef enumControlsHash As Hashtable, ByVal resourceFile As String) As Hashtable
        '	Dim toReturn As New Hashtable
        '	For Each entry As DictionaryEntry In enumControlsHash
        '		Dim enumType As Type = entry.Key.GetType
        '		Dim enumNames As String() = System.Enum.GetNames(CType(entry.Key, Type))
        '		toReturn.Add(entry.Key, enumNames)
        '		Dim dataSource As ListItemCollection = FormatDataSource(Localize(enumNames, resourceFile), enumNames)
        '		CType(entry.Value, ListControl).DataSource = dataSource
        '		CType(entry.Value, ListControl).DataBind()
        '	Next
        '	Return toReturn
        'End Function

        Public Function GetParentModuleBase(Of T As PortalModuleBase)(ByVal objControl As Control) As T
            Return DirectCast(FormHelper.GetParentModuleBase(objControl), T)
        End Function


        Public Function GetParentModuleBase(ByVal objControl As Control) As PortalModuleBase
            Return Web.UI.ControlHelper.FindControlRecursive(Of PortalModuleBase)(objControl)
        End Function


        Public Function FormatDataSource(ByVal texts As ArrayList, ByVal values() As String) As ListItemCollection
            Dim toReturn As New ListItemCollection
            For i As Integer = 0 To values.Length - 1
                Dim objListItem As New ListItem(values(i), values(i))
                If i < texts.Count Then
                    objListItem.Text = CType(texts(i), String)
                End If
                toReturn.Add(objListItem)
            Next
            Return toReturn
        End Function


        Public Function Localize(ByVal keys() As String, ByVal resourceFile As String) As ArrayList
            Dim toReturn As New ArrayList
            Dim value As String
            For Each key As String In keys
                value = Localization.GetString(key, resourceFile)
                If value Is Nothing Then
                    toReturn.Add(key)
                Else
                    toReturn.Add(value)
                End If
            Next
            Return toReturn
        End Function

        Public Function LocalizeExplanations(ByVal keys() As String, ByVal resourceFile As String) As Hashtable
            Dim toReturn As New Hashtable
            Dim value As String
            For Each key As String In keys
                value = Localization.GetString(key & ".Explain", resourceFile)
                If value Is Nothing Then
                    toReturn.Add(key, key)
                Else
                    toReturn.Add(key, value)
                End If
            Next
            Return toReturn
        End Function

        <Obsolete("Use Aricie.Core Control Helper function")> _
        Public Function FindControlRecursive(ByVal objControl As Control, ByVal parentType As Type) As Control
            If objControl.Parent Is Nothing Then
                Return Nothing
            Else
                If objControl.Parent.GetType Is parentType _
                   Or objControl.Parent.GetType.IsSubclassOf(parentType) Then
                    Return objControl.Parent
                Else
                    Return Web.UI.ControlHelper.FindControlRecursive(objControl.Parent, parentType)
                End If
            End If
        End Function

        <Obsolete("User the new override")> _
        Public Sub AddSection(ByVal parentControl As Control, ByVal childControl As Control, ByVal resxKey As String)

            Dim sh As SectionHeadControl = DirectCast(parentControl.Page.LoadControl("~/controls/sectionheadcontrol.ascx"), SectionHeadControl)
            sh.ResourceKey = resxKey

            sh.CssClass = "Head"
            If Not parentControl.Page.IsPostBack Then
                sh.IsExpanded = False
            End If

            parentControl.Controls.Add(sh)
            Dim objDiv As Control = AddSubDiv(childControl)
            parentControl.Controls.Add(objDiv)
            sh.Section = objDiv.ID

        End Sub

        Public Function AddSection(ByVal parentControl As Control, ByVal resxKey As String, isExpandedOnFirstLoad As Boolean, ByRef sectionContainer As Control) As Control
            Dim toReturn As Control
            If NukeHelper.DnnVersion.Major < 7 Then
                Dim sh As SectionHeadControl = DirectCast(parentControl.Page.LoadControl("~/controls/sectionheadcontrol.ascx"), SectionHeadControl)
                toReturn = sh
                sh.Text = resxKey
                sh.ResourceKey = resxKey

                sh.CssClass = "Head"

                If Not parentControl.Page.IsPostBack Then
                    sh.IsExpanded = isExpandedOnFirstLoad
                End If
                parentControl.Controls.Add(sh)
                sectionContainer = New HtmlGenericControl("div")
                sectionContainer.ID = "sh" & resxKey.GetHashCode().ToString()
                parentControl.Controls.Add(sectionContainer)
                sh.Section = sectionContainer.ID
            Else
              

                Dim ctlHeader As New HtmlGenericControl("h2")
                toReturn = ctlHeader
                ctlHeader.Attributes.Add("class", "dnnFormSectionHead")
                'ctlHeader.ID = "sh" & resxKey.GetHashCode().ToString().Trim("-"c)
                Dim ctlheaderId As String = "sh" & (parentControl.ClientID & resxKey).GetHashCode().ToString().Trim("-"c)
                ctlHeader.Attributes.Add("id", ctlheaderId)

                Dim ctlA As New HyperLink
                ctlA.Attributes.Add("href", "")
                If isExpandedOnFirstLoad Then
                    ctlA.Attributes.Add("class", "dnnSectionExpanded")
                Else
                    ctlA.Attributes.Add("class", "")
                End If
                ctlA.Attributes.Add("resourcekey", resxKey)
                ctlA.Text = resxKey
                ctlHeader.Controls.Add(ctlA)
                parentControl.Controls.Add(ctlHeader)

                Dim cookieId As String = ctlheaderId
                ' _SectionHeaderCookieRegex.Replace(ctlHeader.ClientID, "")
                Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(cookieId)

                If cookie Is Nothing Then
                    Dim newCookie = New HttpCookie(cookieId)
                    newCookie.Value = isExpandedOnFirstLoad.ToString(CultureInfo.InvariantCulture).ToLower()
                    newCookie.Expires = Now.AddMinutes(2)
                    newCookie.HttpOnly = False
                    parentControl.Page.Response.Cookies.Add(newCookie)
                End If

              
                ''registerCookejs = String.Format("dnn.dom.setCookie({0}, {1}, 0, '/', '', false, 1200000);", cookieId, cookie.Value)



                RegisterDnnJQueryPlugins(parentControl.Page)
                Dim scriptId As String = "loadPanels" & parentControl.ClientID

                Dim scriptLoaded As Object = HttpContext.Current.Items("scriptLoaded" & scriptId)
                If scriptLoaded Is Nothing Then
                    Dim scriptBlock As New StringBuilder()
                    scriptBlock.AppendLine(String.Format("function setupSections{0}() {{jQuery('#{0}').dnnPanels();}}", parentControl.ClientID)) ', registerCookejs))
                    scriptBlock.AppendLine(" jQuery(document).ready( function () {")
                    scriptBlock.AppendLine(String.Format("setupSections{0}();}});", parentControl.ClientID))
                    'Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {{setupSections{0}();}});

                    ScriptManager.RegisterClientScriptBlock(parentControl.Page, parentControl.Page.GetType(), scriptId, scriptBlock.ToString(), True)
                    HttpContext.Current.Items("scriptLoaded" & scriptId) = True
                End If



                sectionContainer = New HtmlGenericControl("fieldset")
                parentControl.Controls.Add(sectionContainer)
            End If
            Return toReturn
        End Function

        Private _SectionHeaderCookieRegex As New Regex("[^a-zA-Z0-9\-]+", RegexOptions.Compiled)


        Public Function AddSubDiv(ByVal childControl As Control, Optional ByVal additionnalCSS As String = "") As Control
            Dim objDiv As New HtmlGenericControl("div")
            'objDiv.ID = "div" & childControl.ID
            objDiv.Attributes.Add("class", String.Format("SubDiv {0}", additionnalCSS))
            'objDiv.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "20px")
            'objDiv.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "20px")
            'objDiv.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "20px")
            objDiv.Controls.Add(childControl)
            Return objDiv
        End Function


        Public Function IsFirstPass(ByVal objControl As Control) As Boolean
            If String.IsNullOrEmpty(DnnContext.Current.AdvancedClientVariable(objControl, "ifp")) Then
                DnnContext.Current.AdvancedClientVariable(objControl, "ifp") = "0"
                Return True
            End If
            Return False
        End Function

        <Obsolete("method related to former sections, see FormHelper.AddSection")> _
        Public Sub FindSectionsUpRecursive(ByVal objControl As Control, ByRef sectionHeads As List(Of SectionHeadControl), ByRef sectionImagesList As List(Of Image))
            If Not objControl.Parent Is Nothing Then
                Dim shToAdd As SectionHeadControl = Nothing
                Dim imgToAdd As Image = Nothing
                For Each child As Control In objControl.Parent.Controls
                    If TypeOf child Is SectionHeadControl Then
                        shToAdd = DirectCast(child, SectionHeadControl)
                    ElseIf TypeOf child Is Panel Then
                        For Each ctl As Control In child.Controls
                            If TypeOf ctl Is Image AndAlso Not String.IsNullOrEmpty(ctl.ID) AndAlso ctl.ID.StartsWith("ico") Then
                                imgToAdd = DirectCast(ctl, Image)
                            End If
                        Next
                    ElseIf child Is objControl Then
                        Exit For
                    End If
                Next
                If shToAdd IsNot Nothing Then
                    sectionHeads.Add(shToAdd)
                End If
                If imgToAdd IsNot Nothing Then
                    sectionImagesList.Add(imgToAdd)
                End If
                FindSectionsUpRecursive(objControl.Parent, sectionHeads, sectionImagesList)
            End If
        End Sub


        Private _DefaultJQueryVersion As String = "1.5.1"
        Private _DefaultJQueryUIVersion As String = "1.8.9"


        Public Sub LoadjQuery(ByVal page As Page)

            LoadjQuery(page, _DefaultJQueryVersion, _DefaultJQueryUIVersion)


        End Sub


        Public Sub LoadjQuery(ByVal page As Page, jqueryVersion As String, jqueryUIVersion As String)

            If NukeHelper.DnnVersion.Major < 5 Then
                If Not page.ClientScript.IsClientScriptIncludeRegistered("jquery") Then
                    Dim url As String = String.Format("https://ajax.googleapis.com/ajax/libs/jquery/{0}/jquery.min.js", jqueryVersion.ToString)
                    page.ClientScript.RegisterClientScriptInclude("jquery", url)
                End If
            End If

            If NukeHelper.DnnVersion.Major < 6 Then
                If Not page.ClientScript.IsClientScriptIncludeRegistered("jquery-ui") Then
                    Dim url As String = String.Format("https://ajax.googleapis.com/ajax/libs/jqueryui/{0}/jquery-ui.min.js", jqueryUIVersion.ToString)
                    page.ClientScript.RegisterClientScriptInclude("jquery-ui", url)
                End If
            End If

            If NukeHelper.DnnVersion.Major < 6 Then
                page.ClientScript.RegisterClientScriptBlock(GetType(Page), "jQuery.Noconflict", "jQuery.noConflict();", True)
            End If


        End Sub

        Public Sub RegisterDnnJQueryPlugins(objPage As Page)
            FormHelper.RegisterDnnJQueryPluginsMethod.Invoke(Nothing, New Object() {objPage})
        End Sub





#Region "Private members"

        Private _DnnJqueryType As Type

        Private ReadOnly Property DnnJqueryType As Type
            Get
                If _DnnJqueryType Is Nothing Then
                    _DnnJqueryType = ReflectionHelper.CreateType("DotNetNuke.Framework.jQuery, DotNetNuke")
                End If
                Return _DnnJqueryType
            End Get
        End Property

        Private _RegisterDnnJQueryPluginsMethod As MethodInfo

        Private ReadOnly Property RegisterDnnJQueryPluginsMethod As MethodInfo
            Get
                If _RegisterDnnJQueryPluginsMethod Is Nothing Then
                    _RegisterDnnJQueryPluginsMethod = DnnJqueryType.GetMethod("RegisterDnnJQueryPlugins")
                End If
                Return _RegisterDnnJQueryPluginsMethod
            End Get
        End Property



#End Region






    End Module
End Namespace


