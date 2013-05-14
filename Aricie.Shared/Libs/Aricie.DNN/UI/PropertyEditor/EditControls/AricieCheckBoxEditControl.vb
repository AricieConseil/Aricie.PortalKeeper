Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Globalization

Namespace UI.WebControls.EditControls

    Public Class AricieCheckBoxEditControl
        Inherits AricieEditControl

        Protected WithEvents _Checkbox As CheckBox

        'Private _ListName As String = Null.NullString
        'Private _List As ListEntryInfoCollection
        'Private _ParentKey As String = Null.NullString
        'Private _alreadyChanged As Boolean = False

        Protected Overrides Property StringValue() As String
            Get
                Return Me._Checkbox.Checked.ToString(CultureInfo.InvariantCulture)
            End Get
            Set(ByVal value As String)
                Me._Checkbox.Checked = Boolean.Parse(value)
            End Set
        End Property

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            'Dim strValue As String = ""
            'Dim strOldValue As String = CType(OldValue, String)

            'For Each i As ListItem In Me._Checkbox.Items
            '    If i.Selected Then
            '        strValue &= i.Value & ";"
            '    End If
            'Next
            'strValue = strValue.TrimEnd(";"c)

            'Value = strValue

            'Dim args As New PropertyEditorEventArgs(Name)
            'args.Value = strValue
            'args.OldValue = strOldValue
            'args.StringValue = StringValue

            'MyBase.OnValueChanged(args)

        End Sub


        'Protected ReadOnly Property List() As ListEntryInfoCollection
        '    Get
        '        If _List Is Nothing Then
        '            Dim objListController As New ListController
        '            _List = objListController.GetListEntryInfoCollection(ListName, ParentKey)
        '        End If
        '        Return _List
        '    End Get
        'End Property

        'Protected Overridable Property ListName() As String
        '    Get
        '        If _ListName = Null.NullString Then
        '            _ListName = Me.Name
        '        End If
        '        Return _ListName
        '    End Get
        '    Set(ByVal Value As String)
        '        _ListName = Value
        '    End Set
        'End Property

        'Protected Overridable Property ParentKey() As String
        '    Get
        '        Return _ParentKey
        '    End Get
        '    Set(ByVal Value As String)
        '        _ParentKey = Value
        '    End Set
        'End Property

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Me._Checkbox.RenderControl(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Me._Checkbox.Enabled = False
            Me._Checkbox.RenderControl(writer)
        End Sub


        Private Sub AricieCheckBoxListEditControl_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            Me._Checkbox = New CheckBox
            'Me._Checkbox.RepeatLayout = RepeatLayout.Table
            'Me._Checkbox.RepeatDirection = RepeatDirection.Horizontal
            'Me._Checkbox.RepeatColumns = 3
            'Me._Checkbox.CssClass = "enumItem"
            'Me._Checkbox.DataSource = Me.List
            'Me._Checkbox.DataValueField = "Value"
            'Me._Checkbox.DataTextField = "Text"
            'Me._Checkbox.DataBind()
            Dim strValue As String = Me.StringValue
            'Dim strArray() As String = strValue.Split(";"c
            Dim boolValue As Boolean = Boolean.Parse(strValue)

            'For Each s As String In strArray
            '    Try
            '        Me._Checkbox.Items.FindByValue(s).Selected = True
            '    Catch ex As Exception
            '    End Try
            'Next
            If boolValue Then
                Me._Checkbox.Checked = True
            Else
                Me._Checkbox.Checked = False
            End If

            'AddHandler _Checkbox.SelectedIndexChanged, AddressOf Me._ckList_SelectedIndexChanged

            Me.Controls.Add(Me._Checkbox)

            Me.EnsureChildControls()
        End Sub



        Private Sub _Checkbox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _Checkbox.CheckedChanged
            OnDataChanged(e)
        End Sub

    End Class
End Namespace
