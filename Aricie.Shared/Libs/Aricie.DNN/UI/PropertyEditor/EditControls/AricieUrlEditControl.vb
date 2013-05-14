Imports System.Web.UI.WebControls
Imports System.Web.UI.Adapters
Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.Common
Imports DotNetNuke.UI.UserControls
Imports System.Web.UI
Imports System.Collections.Specialized
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.FileSystem
Imports Aricie.DNN.Services
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.UI.Utilities

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

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Try
                MyBase.OnLoad(e)
                'If Not Me.UrlControl.IsPostBack Then
                '    Me.UrlControl.Url = Me.StringValue
                'End If
            Catch ex As Exception
                Exceptions.ProcessModuleLoadException(Me, ex)
            End Try
            MyBase.OnLoad(e)
        End Sub

        Public ReadOnly Property UrlStateKey() As String
            Get
                Return "CtlUrlState-" & Me.ClientID.GetHashCode.ToString
            End Get
        End Property

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)


            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
                Dim parentModule As AriciePortalModuleBase = Me.ParentAricieModule
                If parentModule IsNot Nothing Then
                    AddHandler parentModule.PreRenderComplete, AddressOf PreRenderComplete
                Else
                    AddHandler Me.Page.PreRenderComplete, AddressOf PreRenderComplete
                End If
            End If

        End Sub

        Private Sub PreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
            Dim parentModule As AriciePortalModuleBase = Me.ParentAricieModule
            If parentModule IsNot Nothing Then
                Me.ParentAricieModule.AdvancedClientVariable(Me, UrlStateKey) = Me.UrlControl.Url
            Else
                Me.ViewState(UrlStateKey) = Me.UrlControl.Url
            End If
        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
            'ResolveNewValue()

            Dim strValue As String = CType(Value, String)
            Dim strOldValue As String = DirectCast(IIf(OldValue Is Nothing, "", CType(OldValue, String)), String)

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

        Protected Overridable Sub ResolveEditControl()

            Me.Controls.Clear()

            Me.UrlControl = CType(Me.Page.LoadControl("~/controls/URLControl.ascx"), UrlControl)

            Me.UrlControl.ID = Me.ID & "Edit"
            If Not Me.Required Then
                Me.UrlControl.ShowNone = True
            End If
            Me.UrlControl.ShowLog = False
            Me.UrlControl.ShowTrack = False
           
            If Me.ParentAricieModule IsNot Nothing Then
                Dim existingUrl As String = Me.ParentAricieModule.AdvancedClientVariable(Me, UrlStateKey)
                If String.IsNullOrEmpty(existingUrl) Then
                    Me.UrlControl.Url = Me.StringValue
                Else
                    Me.ParentAricieModule.AdvancedClientVariable(Me, UrlStateKey) = ""
                End If
            Else
                'on est pas dans le cas d'un portalmodulebase(ex:settings)
                Dim existingUrl As String = CStr(Me.ViewState(UrlStateKey))
                If String.IsNullOrEmpty(existingUrl) Then
                    Me.UrlControl.Url = Me.StringValue
                Else
                    Me.ViewState(UrlStateKey) = ""
                End If
            End If
            'UrlControl.GetType().BaseType.GetField("_doReloadFiles", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).SetValue(UrlControl, True)
            'UrlControl.GetType().BaseType.GetField("_doReloadFolders", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).SetValue(UrlControl, True)
            Me.Controls.Add(Me.UrlControl)
            
        End Sub

        Protected Overrides Sub CreateChildControls()
            If Me.EditMode = PropertyEditorMode.Edit OrElse (Me.Required AndAlso OldValue.ToString = "") Then
                Me.ResolveEditControl()
            End If
            MyBase.CreateChildControls()

        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)

            For Each control As Control In Me.Controls
                control.RenderControl(writer)
            Next
        End Sub

        Protected Overrides Sub LoadControlState(ByVal savedState As Object)
            MyBase.LoadControlState(savedState)
        End Sub

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            MyBase.LoadViewState(savedState)
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) _
          As Boolean

            Dim currentUrl As String = Me.UrlControl.Url

            'GetType(UrlControl).GetMethod("DoRenderTypes", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            'GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            Dim uid As String = UrlControl.UniqueID
            Dim postedUrlType As String = postCollection(uid & "$optType")
            If Not String.IsNullOrEmpty(postedUrlType) Then
                UrlControl.UrlType = postedUrlType
                If postedUrlType = "N" Then
                    UrlControl.Url = ""
                    Me.Value = ""
                Else
                    Dim postedRawUrl As String = postCollection(uid & "$txtUrl")
                    If Not String.IsNullOrEmpty(postedRawUrl) AndAlso postedUrlType = "U" Then
                        UrlControl.Url = postedRawUrl
                        Me.Value = postedRawUrl
                    Else
                        Dim postedTabId As String = postCollection(uid & "$cboTabs")
                        If Not String.IsNullOrEmpty(postedTabId) AndAlso postedUrlType = "T" Then
                            UrlControl.Url = postedTabId
                            Me.Value = postedTabId
                        Else
                            Dim postedFileId As String = postCollection(uid & "$cboFiles")
                            If Not String.IsNullOrEmpty(postedFileId) AndAlso postedUrlType = "F" Then
                                Me.Value = "FileID=" & postedFileId
                                UrlControl.Url = Me.Value.ToString
                            End If
                        End If
                    End If
                End If
            End If
            


            


            'Dim opt As RadioButtonList = DirectCast(UrlControl.GetType().GetField("optType", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).GetValue(UrlControl), RadioButtonList)
            'opt.SelectedValue = UrlControl.UrlType

            'If CurrentUrlType = "T" Then
            '    GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            '    Dim CurrentTab As String = postCollection(UrlControl.UniqueID & "$cboTabs")
            '    Dim cboTabs As DropDownList = DirectCast(UrlControl.GetType().GetField("cboTabs", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).GetValue(UrlControl), DropDownList)
            '    cboTabs.SelectedValue = CurrentTab
            'End If

            'Dim UrlControlViewState As StateBag = DirectCast(GetType(UrlControl).GetProperty("ViewState", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).GetValue(UrlControl, Nothing), StateBag)

            'TODO: revenir supprimer cet affreux bloc de violence sur un UrlControl.

            'If CurrentUrlType = "F" Then
            '    ' on est obligé de lui donner le portail pour qu'il trouve les fichiers
            '    UrlControl.GetType().BaseType.GetField("_objPortal", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic).SetValue(UrlControl, PortalInfo(PortalId))
            '    GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            '    Dim CurrentFolder As String = postCollection(UrlControl.UniqueID & "$cboFolders")
            '    Dim cboFolder As DropDownList = DirectCast(UrlControl.GetType().GetField("cboFolders", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).GetValue(UrlControl), DropDownList)
            '    cboFolder.SelectedValue = CurrentFolder
            '    UrlControlViewState("LastFolderPath") = CurrentFolder

            '    'GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            '    Dim CurrentFile As String = postCollection(UrlControl.UniqueID & "$cboFiles")
            '    Dim cboFiles As DropDownList = DirectCast(UrlControl.GetType().GetField("cboFiles", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).GetValue(UrlControl), DropDownList)
            '    cboFiles.SelectedValue = CurrentFile
            '    UrlControlViewState("LastFileName") = CurrentFile

            '    If (Not String.IsNullOrEmpty(CurrentFolder) AndAlso String.IsNullOrEmpty(CurrentFile)) Then
            '        ' CurrentInterestingFolder = CurrentFolder
            '    End If

            '    'GetType(UrlControl).GetMethod("DoRenderTypeControls", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).Invoke(UrlControl, Nothing)
            'End If

            'UrlControlViewState("Url") = UrlControl.Url
            'UrlControlViewState("UrlType") = UrlControl.UrlType


            Return True

        End Function



    End Class


End Namespace
