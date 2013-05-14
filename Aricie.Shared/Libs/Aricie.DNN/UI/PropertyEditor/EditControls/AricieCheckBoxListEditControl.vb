Imports System.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Lists
Imports System.Web.UI
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls
    Public Class AricieCheckBoxListEditControl
        Inherits AricieEditControl

        Protected WithEvents _ckList As CheckBoxList
        Private _ListName As String = Null.NullString
        Private _List As ListEntryInfoCollection
        Private _ParentKey As String = Null.NullString
        Private _alreadyChanged As Boolean = False


        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim strValue As String = ""
            Dim strOldValue As String = CType(OldValue, String)

            For Each i As ListItem In Me._ckList.Items
                If i.Selected Then
                    strValue &= i.Value & ";"
                End If
            Next
            strValue = strValue.TrimEnd(";"c)

            Value = strValue

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = strValue
            args.OldValue = strOldValue
            args.StringValue = StringValue

            MyBase.OnValueChanged(args)

        End Sub


        Protected ReadOnly Property List() As ListEntryInfoCollection
            Get
                If _List Is Nothing Then
                    Dim objListController As New ListController
                    _List = objListController.GetListEntryInfoCollection(ListName, ParentKey)
                End If
                Return _List
            End Get
        End Property

        Protected Overridable Property ListName() As String
            Get
                If _ListName = Null.NullString Then
                    _ListName = Me.Name
                End If
                Return _ListName
            End Get
            Set(ByVal Value As String)
                _ListName = Value
            End Set
        End Property

        Protected Overridable Property ParentKey() As String
            Get
                Return _ParentKey
            End Get
            Set(ByVal Value As String)
                _ParentKey = Value
            End Set
        End Property

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Me._ckList.RenderControl(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Me._ckList.Enabled = False
            Me._ckList.RenderControl(writer)
        End Sub


        Private Sub AricieCheckBoxListEditControl_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            Me._ckList = New CheckBoxList
            Me._ckList.RepeatLayout = RepeatLayout.Table
            Me._ckList.RepeatDirection = RepeatDirection.Horizontal
            Me._ckList.RepeatColumns = 3
            Me._ckList.CssClass = "enumItem"
            Me._ckList.DataSource = Me.List
            Me._ckList.DataValueField = "Value"
            Me._ckList.DataTextField = "Text"
            Me._ckList.DataBind()
            Dim strValue As String = Me.StringValue
            Dim strArray() As String = strValue.Split(";"c)

            For Each s As String In strArray
                Try
                    Me._ckList.Items.FindByValue(s).Selected = True
                Catch ex As Exception
                End Try
            Next

            AddHandler _ckList.SelectedIndexChanged, AddressOf Me._ckList_SelectedIndexChanged

            Me.Controls.Add(Me._ckList)

            Me.EnsureChildControls()
        End Sub

        Private Sub _ckList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _ckList.SelectedIndexChanged
            If Not _alreadyChanged Then
                OnDataChanged(e)
                _alreadyChanged = True
            End If
        End Sub
    End Class
End Namespace
