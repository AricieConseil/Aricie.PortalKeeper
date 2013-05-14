Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Web.UI.Design.WebControls
Imports System.Drawing.Design
Imports System.Runtime.CompilerServices
Imports System.Text

<Assembly: WebResource("Aricie.DNN.DropDownListMulti.js", "text/javascript")> 

Namespace UI.WebControls
    <ParseChildren(True), PersistChildren(True)> _
    Public Class DropDownListMulti
        Inherits Panel
        Implements INamingContainer, IScriptControl

        Public Event SelectedIndexChanged As EventHandler


        Protected WithEvents _tbSelected As TextBox
        Protected WithEvents _myGrid As GridView
        Protected WithEvents _hfValue As HiddenField

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            Dim plh As New PlaceHolder
            plh.ID = "placeH"
            Me.Controls.Add(plh)

            TbSelected = New TextBox
            HfValue = New HiddenField
            Grd = New GridView
            

            'Dim ddlExtender As New AjaxControlToolkit.DropDownExtender
            'ddlExtender.DropDownControlID = "gvDdl"
            'ddlExtender.TargetControlID = "tbSelected"
            '' ddlExtender.DynamicControlID = "gvDdl"

            plh.Controls.Add(TbSelected)
            plh.Controls.Add(Grd)
            plh.Controls.Add(HfValue)
            TbSelected.ID = "tbSelected"
            TbSelected.Attributes.Add("autocomplete", "off")
            Grd.CssClass = "ddlGrid"
            Grd.AllowPaging = False
            Grd.Style.Add("display", "none")
            Grd.ID = "gvDdl"
            Grd.EnableViewState = True
            HfValue.ID = "hfValue"
            ' Me.CssClass = String.Format("{0} {1}", Me.CssClass, "AricieDropDownListMulti")
            AddHandler Grd.RowDataBound, AddressOf GrdRowDataBound
            AddHandler Grd.SelectedIndexChanged, AddressOf GrdSelectedIndexChanged

            'If Not Page Is Nothing AndAlso Page.IsPostBack Then
            Dim currentHFV As New HiddenField
            Dim currentHFT As New HiddenField


            For Each myRow As GridViewRow In Grd.Rows
                myRow.Cells(0).Controls.Add(currentHFV)
                myRow.Cells(0).Controls.Add(currentHFT)
            Next

            'End If
            ' Me.Controls.Add(ddlExtender)
        End Sub
        Public Overrides Property CssClass As String
            Get
                If MyBase.CssClass.Contains("AricieDropDownListMulti") Then
                    Return MyBase.CssClass
                Else
                    Return String.Format("{0} {1}", MyBase.CssClass, "AricieDropDownListMulti")
                End If

            End Get
            Set(ByVal value As String)
                MyBase.CssClass = String.Format("{0} {1}", value, "AricieDropDownListMulti")
            End Set
        End Property


        Protected Sub GrdSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            'recuperation des valeurs
            If (Grd.Rows.Count > 0 AndAlso Grd.SelectedRow IsNot Nothing) Then
                Dim idxHF As Integer = 0
                For Each Control In Grd.SelectedRow.Cells(0).Controls
                    If TypeOf Control Is HiddenField Then
                        If idxHF = 0 Then
                            HfValue.Value = DirectCast(Control, HiddenField).Value
                            idxHF += 1
                        End If
                        If idxHF = 1 Then
                            TbSelected.Text = DirectCast(Control, HiddenField).Value
                        End If
                    End If
                Next
            End If


            RaiseEvent SelectedIndexChanged(sender, e)
        End Sub

        Protected Sub GrdRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
            If e.Row.RowType = DataControlRowType.DataRow Then
                e.Row.Attributes("onmouseover") =
                    "this.style.cursor='pointer';this.originalBackgroundColor=this.style.backgroundColor;this.style.backgroundColor='#bbbbbb';"
                e.Row.Attributes("onmouseout") = "this.style.backgroundColor=this.originalBackgroundColor;"


                Dim values As New StringBuilder("")
                If Not String.IsNullOrEmpty(Me.DataValueField) Then
                    For Each myDataField In Me.DataValueField.Split({","}, StringSplitOptions.RemoveEmptyEntries)
                        values.AppendFormat("{0},", DataBinder.Eval(e.Row.DataItem, myDataField))
                    Next
                End If
                Dim currentHFV As New HiddenField
                currentHFV.ID = "currentHFV"
                currentHFV.Value = values.ToString
                Dim currentHFT As New HiddenField
                currentHFT.ID = "currentHFT"
                If Not String.IsNullOrEmpty(Me.DataTextField) Then
                    currentHFT.Value = DataBinder.Eval(e.Row.DataItem, Me.DataTextField).ToString
                End If
                Dim currentContent As New Label()
                currentContent.Text = e.Row.Cells(0).Text
                e.Row.Cells(0).Controls.Add(currentHFV)
                e.Row.Cells(0).Controls.Add(currentHFT)
                e.Row.Cells(0).Controls.Add(currentContent)
            End If
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            EnsureChildControls()
        End Sub
        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            MyBase.LoadViewState(savedState)
            Dim currentDS As String = CStr(Me.ViewState("CurrentGrdDS"))
            If Not String.IsNullOrEmpty(currentDS) Then
                Dim isDataTable As Boolean = False
                If Not ViewState("IsDataTable") Is Nothing AndAlso DirectCast(ViewState("IsDataTable"), Boolean) = True Then
                    isDataTable = True
                End If
                If isDataTable Then
                    Grd.DataSource = Aricie.Services.ReflectionHelper.Deserialize(Of Aricie.Collections.SerializableList(Of DataTable))(currentDS).First
                Else
                    Grd.DataSource = Aricie.Services.ReflectionHelper.Deserialize(Of Aricie.Collections.SerializableList(Of Object))(currentDS).First
                End If

                Grd.DataBind()
            End If
        End Sub
        Protected Overrides Function SaveViewState() As Object
            'If Not Me.DataSource Is Nothing Then
            '    Me.ViewState.Add("CurrentGrdDS", Grd.DataSource)
            'End If
            Return MyBase.SaveViewState()
        End Function

        'Protected Overrides Sub LoadControlState(ByVal savedState As Object)
        '    MyBase.LoadControlState(savedState)
        'End Sub
        Public Property DataSource As Object
            Get

                Return Grd.DataSource
            End Get
            Set(ByVal value As Object)
                Grd.DataSource = value
                If Not value Is Nothing Then
                    Dim xmlDoc As System.Xml.XmlDocument
                    If Not TypeOf value Is DataView Then
                        Dim ls As New Aricie.Collections.SerializableList(Of Object)()
                        ls.Add(value)
                        xmlDoc = Aricie.Services.ReflectionHelper.Serialize(ls)
                        ViewState.Add("IsDataTable", False)
                    Else
                        Dim ls As New Aricie.Collections.SerializableList(Of DataTable)()
                        Dim dt As DataTable = DirectCast(value, DataView).ToTable
                        If String.IsNullOrEmpty(dt.TableName) Then
                            dt.TableName = "DataSource"
                        End If
                        ls.Add(dt)
                        xmlDoc = Aricie.Services.ReflectionHelper.Serialize(ls)
                        ViewState.Add("IsDataTable", True)
                    End If

                    ViewState.Add("CurrentGrdDS", xmlDoc.OuterXml)
                Else
                    ViewState.Remove("CurrentGrdDS")
                    ViewState.Remove("IsDataTable")
                End If
                'ViewState("CurrentGrdDS") = value
            End Set
        End Property


        Public Property SelectedIndex As Integer
            Get
                Dim toReturn As Integer = -1
                If Not String.IsNullOrEmpty(SelectedValue) AndAlso SelectedValue.Split({","}, StringSplitOptions.RemoveEmptyEntries).Count > 0 Then
                    Dim i As Integer = 0
                    For Each row As GridViewRow In Grd.Rows
                        Dim valueCtl As Control = row.Cells(0).FindControl("currentHFV")
                        If Not valueCtl Is Nothing Then
                            If DirectCast(valueCtl, HiddenField).Value.Trim(","c) = SelectedValue Then
                                toReturn = i
                            End If
                        End If
                        i += 1
                    Next

                End If
                Return toReturn
            End Get
            Set(ByVal value As Integer)
                If value >= 0 AndAlso value < Grd.Rows.Count Then
                    Dim valueCtl As Control = Grd.Rows(value).Cells(0).FindControl("currentHFV")
                    Dim textCtl As Control = Grd.Rows(value).Cells(0).FindControl("currentHFT")
                    If Not valueCtl Is Nothing Then
                        Me.SelectedValue = DirectCast(valueCtl, HiddenField).Value
                    End If
                    If Not textCtl Is Nothing Then
                        Me.Text = DirectCast(textCtl, HiddenField).Value
                    End If
                Else
                    Me.Text = String.Empty
                    Me.SelectedValue = String.Empty
                End If
            End Set
        End Property

#Region "Controls"

        Protected Property TbSelected As TextBox
            Get
                If _tbSelected Is Nothing Then
                    _tbSelected = DirectCast(Me.FindControl("tbSelected"), TextBox)
                End If

                Return _tbSelected
            End Get
            Set(ByVal value As TextBox)
                _tbSelected = value
            End Set
        End Property

        Protected Property HfValue As HiddenField
            Get
                If _hfValue Is Nothing Then
                    _hfValue = DirectCast(Me.FindControl("hfValue"), HiddenField)
                End If
                Return _hfValue
            End Get
            Set(ByVal value As HiddenField)
                _hfValue = value
            End Set
        End Property

        Protected Property Grd As GridView
            Get
                If _myGrid Is Nothing Then
                    _myGrid = DirectCast(Me.FindControl("gvDdl"), GridView)
                End If
                Return _myGrid
            End Get
            Set(ByVal value As GridView)
                _myGrid = value
            End Set
        End Property

#End Region


        Private _autoPostBack As Boolean = False
        Private _dataTextField As String
        'Private _dataValueField As String


#Region "Public Properties"

        Public Property ShowHeader As Boolean
            Get
                Return Grd.ShowHeader
            End Get
            Set(ByVal value As Boolean)
                Grd.ShowHeader = value
            End Set
        End Property

        ''' <summary>
        ''' If true, controls will postback on selectedindexchanged
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AutoPostBack() As Boolean
            Get
                Return _autoPostBack
            End Get
            Set(ByVal value As Boolean)
                _autoPostBack = value
            End Set
        End Property

        ''' <summary>
        ''' Define the field to show when an item is selected
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DataTextField() As String
            Get
                Return _dataTextField
            End Get
            Set(ByVal value As String)
                _dataTextField = value
            End Set
        End Property

        ''' <summary>
        ''' Define the fields to use as datavalue seperated by ,
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>White-space characters are automaticaly removed</remarks>
        Public Property DataValueField() As String
            Get
                Return Convert.ToString(ViewState("DataValueField")) ' modifié plutôt que le tostring qui plante en cas de valeur nulle
            End Get
            Set(ByVal value As String)
                ViewState("DataValueField") = value.Trim()
            End Set
        End Property

        ''' <summary>
        ''' Colums in the grid, can be used to edit HeaderText only if autogenerateColumns=false. If autogenerated=true, You should use HeaderRow to edit a column header.
        ''' Ie : .Columns(i).HeaderText="NewHeader"
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MergableProperty(False),
            Editor(GetType(DataControlFieldTypeEditor), GetType(UITypeEditor)),
            PersistenceMode(PersistenceMode.InnerProperty)>
        Public ReadOnly Property Columns As DataControlFieldCollection
            Get
                Return Grd.Columns
            End Get
        End Property

        ''' <summary>
        ''' Headerrow of the grid. Can be used to edit a column title specially when autogeneratecolumns=true.
        ''' Ie: .HeaderRow.Cells(i).Text="NewHeader"
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HeaderRow As GridViewRow
            Get
                Return Grd.HeaderRow
            End Get
        End Property

        Public Sub HideColumns(ByVal lstIdxColumn As List(Of Integer))
            For Each myRow As GridViewRow In Grd.Rows
                For Each myIdx In lstIdxColumn
                    myRow.Cells(myIdx).Style.Add("display", "none")
                    Grd.HeaderRow.Cells(myIdx).Style.Add("display", "none")
                Next
                Next
        End Sub
        ''' <summary>
        ''' Text of the selected items
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Text As String
            Get
                Return TbSelected.Text
            End Get
            Set(ByVal value As String)
                TbSelected.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Return the key fields of selected value separated with ,
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SelectedValue As String
            Get
                Return HfValue.Value.Trim(","c)
            End Get
            Set(ByVal value As String)
                HfValue.Value = value
                Me.Text = String.Empty
                'get the corresponding text
                For Each myRow As GridViewRow In Grd.Rows
                    If myRow.RowType = DataControlRowType.DataRow Then
                        Dim valueCtl As Control = myRow.Cells(0).FindControl("currentHFV")
                        Dim textCtl As Control = myRow.Cells(0).FindControl("currentHFT")
                        If Not valueCtl Is Nothing AndAlso DirectCast(valueCtl, HiddenField).Value.Trim(","c) = value AndAlso Not textCtl Is Nothing Then
                            Me.Text = DirectCast(textCtl, HiddenField).Value
                            Exit For
                        End If
                    End If
                Next


            End Set
        End Property

        Public ReadOnly Property SelectedValues As Dictionary(Of String, String)
            Get
                Dim _selectedValues As New Dictionary(Of String, String)
                Dim columnList() As String = DataValueField.Split(","c)
                Dim values() As String = SelectedValue.Split(","c)
                Dim indexCurrentColumn As Integer = 0
                For Each column As String In columnList
                    If indexCurrentColumn < values.Count Then
                        _selectedValues.Add(column, values(indexCurrentColumn))
                    Else
                        _selectedValues.Add(column, String.Empty)
                    End If

                    indexCurrentColumn += 1
                Next
                Return _selectedValues
            End Get
        End Property


        Public Property AutoGenerateColumns() As Boolean
            Get
                Return Grd.AutoGenerateColumns
            End Get
            Set(ByVal value As Boolean)
                Grd.AutoGenerateColumns = value
            End Set
        End Property

#End Region

        ''' <summary>
        ''' Bind the Dropdownlist
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub DataBind()
            Grd.DataBind()

        End Sub


        Public Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor) _
            Implements IScriptControl.GetScriptDescriptors
            Dim toReturn As New List(Of ScriptDescriptor)
            Dim myScriptD As New ScriptControlDescriptor("Aricie.DNN.DropDownListMulti", Me.ClientID)
            myScriptD.AddProperty("ClientId", Me.ClientID)
            myScriptD.AddProperty("AutoPostBack", Me.AutoPostBack)
            myScriptD.AddProperty("tbClientId", TbSelected.ClientID)
            myScriptD.AddProperty("grdClientId", Grd.ClientID)
            myScriptD.AddProperty("hfValueClientId", HfValue.ClientID)
            toReturn.Add(myScriptD)
            Return toReturn
        End Function

        Public Function GetScriptReferences() As IEnumerable(Of ScriptReference) _
            Implements IScriptControl.GetScriptReferences
            Dim toReturn As New List(Of ScriptReference)
            toReturn.Add(New ScriptReference("Aricie.DNN.DropDownListMulti.js",
                                             GetType(DropDownListMulti).Assembly.FullName))
            'toReturn.Add(New ScriptReference(Aricie.DNN.UI.WebControls.ResourcesUtils.getWebResourceUrl(Me.Page, GetType(DropDownListMulti), "Aricie.DNN.DropDownListMulti.js")))
            Return toReturn
        End Function


        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptDescriptors(Me)
                If Me.AutoPostBack Then
                    Dim r As GridViewRow
                    For Each r In Grd.Rows
                        If r.RowType = DataControlRowType.DataRow Then
                            'Page.ClientScript.RegisterForEventValidation(r.UniqueID + "$LinkButtonSelectRow")
                            r.Attributes.Add("onclick",
                                             Page.ClientScript.GetPostBackEventReference(Grd, "Select$" & r.RowIndex,
                                                                                         True))
                            r.Style("cursor") = "pointer"
                        End If
                    Next
                End If
            End If
            MyBase.Render(writer)
        End Sub

        'Private Sub DropDownListMulti_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'End Sub


        Private Sub DropDownListMultiPreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
            ResourcesUtils.registerStylesheet(Me.Page, "Aricie.Web",
                                              ResourcesUtils.getWebResourceUrl(Me.Page, GetType(DropDownListMulti),
                                                                               "Aricie.DNN.Aricie.Web.css"), False)
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptControl(Me)

            End If
        End Sub
    End Class

    Public Module DataControlFieldCollectionExtension
        <Extension()>
        Public Function FromKey(ByVal columns As DataControlFieldCollection, ByVal fieldName As String) As BoundField
            Dim toReturn As BoundField = Nothing
            For Each myCol As BoundField In columns

                If myCol.DataField.ToLowerInvariant = fieldName.ToLowerInvariant() Then
                    toReturn = myCol
                End If
            Next
            Return toReturn
        End Function
    End Module
End Namespace
