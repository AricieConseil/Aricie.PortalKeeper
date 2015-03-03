Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Reflection

Namespace Web.UI.Controls

    Public Class ForeignKeyBoundField
        Inherits BoundField

        Private _textField As String

        Protected Overrides Sub InitializeDataCell(ByVal cell As System.Web.UI.WebControls.DataControlFieldCell, ByVal rowState As System.Web.UI.WebControls.DataControlRowState)
            Dim child As Control = Nothing
            Dim fk As New DropDownListForeignKeyControl(Me.CollectionDataSourceID, rowState)
            fk.ID = Me.DataField & "_fk"
            fk.TextField = Me.TextField
            fk.ValueField = Me.DataField

            cell.Controls.Add(fk)

            If MyBase.Visible Then
                AddHandler fk.DataBinding, New EventHandler(AddressOf Me.OnDataBindField)
            End If
        End Sub

        Protected Overrides Sub OnDataBindField(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim control As Control = DirectCast(sender, Control)
            Dim namingContainer As Control = control.NamingContainer
            Dim dataValue As Object = Me.GetValue(namingContainer)

            If TypeOf control Is ForeignKeyControl Then
                DirectCast(control, ForeignKeyControl).Value = dataValue
            End If
        End Sub

        Public Property CollectionDataSourceID() As String
            Get
                Return CStr(ViewState("CollectionDataSourceID"))
            End Get
            Set(ByVal value As String)
                ViewState("CollectionDataSourceID") = value
            End Set
        End Property

        Public Property TextField() As String
            Get
                Return _textField
            End Get
            Set(ByVal value As String)
                _textField = value
            End Set
        End Property

    End Class

    Public MustInherit Class ForeignKeyControl
        Inherits DataBoundControl

        Dim _value As Object
        Dim _rowState As DataControlRowState
        Dim _valueField As String
        Dim _textField As String


        Sub New(ByVal dataSourceID As String, ByVal rowState As DataControlRowState)
            Me.DataSourceID = dataSourceID
            Me.RowState = rowState
        End Sub

        Public Property Value() As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
            End Set
        End Property

        Public Property RowState() As DataControlRowState
            Get
                Return _rowState
            End Get
            Set(ByVal value As DataControlRowState)
                _rowState = value
            End Set
        End Property

        Public Property TextField() As String
            Get
                Return _textField
            End Get
            Set(ByVal value As String)
                _textField = value
            End Set
        End Property

        Public Property ValueField() As String
            Get
                Return _valueField
            End Get
            Set(ByVal value As String)
                _valueField = value
            End Set
        End Property

        Public Overrides Sub DataBind()
            MyBase.DataBind()

            Dim view As DataSourceView = GetData()

            view.Select(Me.CreateDataSourceSelectArguments, New DataSourceViewSelectCallback(AddressOf Me.OnDataSourceViewSelectCallback))

        End Sub

        Protected Overrides Sub PerformDataBinding(ByVal data As System.Collections.IEnumerable)

            If (Me.RowState And DataControlRowState.Edit) <> DataControlRowState.Normal Then
                'mode edit
                BindEdit(data)
            Else
                'mode view
                BindView(data)
            End If
        End Sub

        Protected Overridable Sub BindView(ByVal data As System.Collections.IEnumerable)

            Dim dict As New Dictionary(Of Object, Object)

            For Each element As Object In data

                Dim type As Type = element.GetType()
                Dim valueProp As PropertyInfo = type.GetProperty(Me.ValueField)
                Dim textProp As PropertyInfo = type.GetProperty(Me.TextField)

                Dim value As Object = valueProp.GetValue(element, Nothing)
                Dim text As Object = textProp.GetValue(element, Nothing)

                dict.Add(value, text)

            Next

            Dim label As Label = CType(Me.FindControl(Me.ID & "_label"), Label)

            If Me.Value IsNot Nothing AndAlso dict.ContainsKey(Me.Value) Then
                label.Text = dict(Me.Value).ToString()
            Else
                label.Text = "&nbsp;"
            End If



        End Sub

        Protected MustOverride Sub BindEdit(ByVal data As System.Collections.IEnumerable)


        Private Sub OnDataSourceViewSelectCallback(ByVal data As IEnumerable)
            Me.PerformDataBinding(data)
        End Sub


        Protected Overrides Sub CreateChildControls()

            If (Me.RowState And DataControlRowState.Edit) <> DataControlRowState.Normal Then
                'mode edit
                CreateEditControls()
            Else
                'mode view
                CreateViewControls()
            End If

        End Sub

        Protected Overridable Sub CreateViewControls()
            Dim label As New Label

            label.ID = Me.ID & "_label"

            Me.Controls.Add(label)

        End Sub

        Protected MustOverride Sub CreateEditControls()

    End Class

    Public Class DropDownListForeignKeyControl
        Inherits ForeignKeyControl

        Public Sub New(ByVal dataSourceID As String, ByVal rowState As DataControlRowState)
            MyBase.New(dataSourceID, rowState)
        End Sub

        Protected Overrides Sub CreateEditControls()

            Dim ddl As New DropDownList

            ddl.ID = Me.ID & "_ddl"
            ddl.DataTextField = Me.TextField
            ddl.DataValueField = Me.ValueField

            Me.Controls.Add(ddl)

        End Sub


        Protected Overrides Sub BindEdit(ByVal data As System.Collections.IEnumerable)
            Dim ddl As DropDownList = CType(Me.FindControl(Me.ID & "_ddl"), DropDownList)

            ddl.DataSource = data
            ddl.DataBind()

            ddl.SelectedValue = CStr(Me.Value)

        End Sub

    End Class


End Namespace
