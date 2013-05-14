Imports System.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Lists
Imports System.Web.UI
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls.EditControls
    Public Class AricieRadioButtonListEditContrrol
        Inherits AricieEditControl

        Protected WithEvents _rbList As RadioButtonList
        Private _ListName As String = Null.NullString
        Private _List As ListEntryInfoCollection
        Private _ParentKey As String = Null.NullString

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim strValue As String = ""
            Dim strOldValue As String = CType(OldValue, String)

            strValue = Me._rbList.SelectedValue

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
            Me._rbList.RenderControl(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Me._rbList.Enabled = False
            Me._rbList.RenderControl(writer)
        End Sub

        Private Sub AricieRadioButtonListEditContrrol_Load(ByVal sender As Object, ByVal e As EventArgs) _
            Handles Me.Load
            Me._rbList = New RadioButtonList
            Me._rbList.RepeatLayout = RepeatLayout.Table
            Me._rbList.RepeatDirection = RepeatDirection.Horizontal
            Me._rbList.RepeatColumns = 3
            Me._rbList.CssClass = "enumItem"
            Me._rbList.DataSource = Me.List
            Me._rbList.DataValueField = "Value"
            Me._rbList.DataTextField = "Text"
            Me._rbList.DataBind()

            Try
                Me._rbList.Items.FindByValue(Me.StringValue).Selected = True
            Catch ex As Exception
            End Try

            AddHandler _rbList.SelectedIndexChanged, AddressOf Me._rbList_SelectedIndexChanged

            Me.Controls.Add(Me._rbList)

            Me.EnsureChildControls()

        End Sub

        Private Sub _rbList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _rbList.SelectedIndexChanged
            OnDataChanged(e)
        End Sub
    End Class
End Namespace
