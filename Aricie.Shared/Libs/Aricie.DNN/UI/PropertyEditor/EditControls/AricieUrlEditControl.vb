
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.Services.Localization
Imports Microsoft.VisualBasic.CompilerServices
Imports DotNetNuke.UI.UserControls
Imports System.Web.UI
Imports System.Collections.Specialized
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.FileSystem
Imports Aricie.DNN.Services
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.Entities
Imports System.Globalization

Namespace UI.WebControls.EditControls

    Public Class AricieUrlEditControl
        Inherits AricieEditControl
        Implements ILargeEditControl

        Protected UrlControl As UrlControl

        Shared Sub New()
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                Me.EnsureChildControls()
                MyBase.OnInit(e)
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        'Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        '    MyBase.OnLoad(e)
        'End Sub

        Public ReadOnly Property UrlStateKey() As String
            Get
                Return "CtlUrlState-" & Me.ClientID.GetHashCode.ToString
            End Get
        End Property

        Public ReadOnly Property UrlTypeStateKey() As String
            Get
                Return "CtlUrlTypeState-" & Me.ClientID.GetHashCode.ToString
            End Get
        End Property

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)


            If (Page IsNot Nothing) AndAlso Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
                Dim objParentModule As AriciePortalModuleBase = Me.ParentAricieModule

                If objParentModule IsNot Nothing Then
                    AddHandler objParentModule.PreRenderComplete, AddressOf PreRenderComplete
                Else
                    AddHandler Me.Page.PreRenderComplete, AddressOf PreRenderComplete
                End If
            End If

        End Sub

        Private Sub PreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
            Dim objParentModule As AriciePortalModuleBase = Me.ParentAricieModule
            If objParentModule IsNot Nothing Then
                DnnContext.Current.AdvancedClientVariable(Me, UrlStateKey) = Me.UrlControl.Url
            Else
                Me.ViewState(UrlStateKey) = Me.UrlControl.Url
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
            'ResolveNewValue()

            Dim strValue As String = CType(Value, String)
            Dim strOldValue As String = CStr(OldValue)

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = strValue
            args.OldValue = strOldValue
            args.StringValue = StringValue

            MyBase.OnValueChanged(args)

        End Sub

        Protected Overridable Sub ResolveNewValue()
            Dim url As String = Me.UrlControl.Url
            'If Not String.IsNullOrEmpty(url) Then
            Me.Value = url
            'End If
        End Sub




        

        Protected Overrides Sub CreateChildControls()
            Try
                If Me.EditMode = PropertyEditorMode.Edit OrElse (Me.Required AndAlso OldValue.ToString = "") Then
                    Me.ResolveEditControl()
                End If
            Finally
                Me.ChildControlsCreated = True
            End Try
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)

            For Each control As Control In Me.Controls
                control.RenderControl(writer)
            Next
        End Sub


        Protected Overrides Sub RenderViewMode(writer As HtmlTextWriter)
            Dim strUrl As String = Conversions.ToString(Me.Value)
            If DotNetNuke.Common.Globals.GetURLType(strUrl) <> DotNetNuke.Entities.Tabs.TabType.Url Then
                strUrl = DotNetNuke.Common.Globals.LinkClick(Conversions.ToString(Me.Value), -1, -1)
            End If
            Me.ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute("href", strUrl)
            writer.AddAttribute("target", "_blank")
            writer.RenderBeginTag(HtmlTextWriterTag.A)
            Dim navigateLink As String = Localization.GetString(Me.Name & "_Navigate.Text", Me.LocalResourceFile)
            writer.Write(navigateLink)
            writer.RenderEndTag()
        End Sub


        'Protected Overrides Sub LoadControlState(ByVal savedState As Object)
        '    MyBase.LoadControlState(savedState)
        'End Sub

        'Protected Overrides Sub LoadViewState(ByVal savedState As Object)
        '    MyBase.LoadViewState(savedState)
        'End Sub

        Public Property CurrentUrl As String
            Get
                Return DnnContext.Current.AdvancedClientVariable(Me, UrlStateKey)
            End Get
            Set(value As String)
                Me.Value = value
                DnnContext.Current.AdvancedClientVariable(Me, UrlStateKey) = value
            End Set
        End Property

        Public Property CurrentUrlType As String
            Get
                Return DnnContext.Current.AdvancedClientVariable(Me, UrlTypeStateKey)
            End Get
            Set(value As String)
                DnnContext.Current.AdvancedClientVariable(Me, UrlTypeStateKey) = value
            End Set
        End Property

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) _
          As Boolean
            Dim toReturn As Boolean = False
            'GetType(UrlControl).GetMethod("DoRenderTypes", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            'GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            Dim uid As String = UrlControl.UniqueID
            Dim postedUrlType As String = postCollection(uid & "$optType")


            If Not String.IsNullOrEmpty(postedUrlType) Then

                CurrentUrlType = postedUrlType
                UrlControl.UrlType = postedUrlType
                Dim newValue As String = ""
                If postedUrlType = "N" Then
                    newValue = ""
                Else
                    Dim postedRawUrl As String = postCollection(uid & "$txtUrl")
                    If Not String.IsNullOrEmpty(postedRawUrl) AndAlso postedUrlType = "U" Then
                        newValue = postedRawUrl
                    Else
                        Dim postedTabId As String = postCollection(uid & "$cboTabs")
                        If Not String.IsNullOrEmpty(postedTabId) AndAlso postedUrlType = "T" Then
                            newValue = postedTabId
                        Else
                            Dim postedFileId As String = postCollection(uid & "$cboFiles")
                            If Not String.IsNullOrEmpty(postedFileId) AndAlso postedUrlType = "F" Then
                                Dim target As String = postCollection("__EVENTTARGET")
                                Dim uidFolders As String = uid & "$cboFolders"
                                If Not target.IsNullOrEmpty() AndAlso target = uidFolders Then
                                    Dim postedFolderPath As String = postCollection(uidFolders)
                                    If Not postedFolderPath.IsNullOrEmpty() Then
                                        Dim objFolder As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(NukeHelper.PortalId, postedFolderPath)
                                        If objFolder IsNot Nothing Then
                                            Dim objFiles As IEnumerable(Of FileInfo) = ObsoleteDNNProvider.Instance.GetFiles(objFolder)
                                            If objFiles IsNot Nothing AndAlso objFiles.Count > 0 Then
                                                newValue = "FileID=" & objFiles(0).FileId.ToString(CultureInfo.InvariantCulture)
                                            Else
                                                newValue = ""
                                            End If
                                        End If
                                    End If

                                Else
                                    newValue = "FileID=" & postedFileId
                                End If
                            End If
                        End If
                    End If
                End If
                If Me.CurrentUrl <> newValue Then
                    Me.CurrentUrl = newValue
                    toReturn = True
                End If
            End If


            Return toReturn

        End Function

        Protected Overridable Sub ResolveEditControl()

            Me.Controls.Clear()

            Me.UrlControl = CType(Me.Page.LoadControl("~/controls/URLControl.ascx"), UrlControl)

            Me.UrlControl.ID = Me.ID & "Edit"
            If Not Me.Required Then
                Me.UrlControl.ShowNone = True
            End If
            Me.UrlControl.ShowLog = False
            Me.UrlControl.ShowTrack = False
            If TypeOf Me.ParentField.DataSource Is ControlUrlInfo Then
                Dim objUrl As ControlUrlInfo = DirectCast(Me.ParentField.DataSource, ControlUrlInfo)
                'Me.UrlControl.ShowNone = ((objUrl.FilterMode And UrlControlMode.None) = UrlControlMode.None)
                Me.UrlControl.ShowUrls = ((objUrl.FilterMode And UrlControlMode.Url) = UrlControlMode.Url)
                Me.UrlControl.ShowTabs = ((objUrl.FilterMode And UrlControlMode.Tab) = UrlControlMode.Tab)
                Me.UrlControl.ShowFiles = ((objUrl.FilterMode And UrlControlMode.File) = UrlControlMode.File)
                Me.UrlControl.ShowSecure = ((objUrl.FilterMode And UrlControlMode.Secure) = UrlControlMode.Secure)
                Me.UrlControl.ShowDatabase = ((objUrl.FilterMode And UrlControlMode.Database) = UrlControlMode.Database)
                Me.UrlControl.ShowUpLoad = ((objUrl.FilterMode And UrlControlMode.Upload) = UrlControlMode.Upload)
                Me.UrlControl.ShowUsers = ((objUrl.FilterMode And UrlControlMode.Member) = UrlControlMode.Member)
                Me.UrlControl.ShowTrack = ((objUrl.FilterMode And UrlControlMode.Track) = UrlControlMode.Track)
                Me.UrlControl.ShowLog = ((objUrl.FilterMode And UrlControlMode.Log) = UrlControlMode.Log)
                Me.UrlControl.ShowNewWindow = ((objUrl.FilterMode And UrlControlMode.NewWindow) = UrlControlMode.NewWindow)
            End If

            If Not String.IsNullOrEmpty(CurrentUrlType) Then
                Me.UrlControl.UrlType = CurrentUrlType
            End If
            If Not Me.CurrentUrl.IsNullOrEmpty() Then
                Me.UrlControl.Url = Me.CurrentUrl
            End If
           
            'UrlControl.GetType().BaseType.GetField("_doReloadFiles", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).SetValue(UrlControl, True)
            'UrlControl.GetType().BaseType.GetField("_doReloadFolders", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).SetValue(UrlControl, True)
            Me.Controls.Add(Me.UrlControl)

        End Sub

    End Class


End Namespace
