Imports System.Web.UI.WebControls
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Web.UI

Namespace Web.UI.Controls
    ''' <summary>
    ''' Enhanced gridview with advanced grouping capabilities additional creation row.
    ''' </summary>
    Public Class AdvancedGridView
        Inherits GridView


        ' Fields
        Private _InsertValues As IOrderedDictionary
        Private _DynamicBoundFieldValues As OrderedDictionary
        Protected gvFooterRow As GridViewRow


        ' Events
        <Category("Action")> _
        Public Event RowInserted As GridViewUpdatedEventHandler

        <Category("Action")> _
        Public Event RowInserting As GridViewUpdateEventHandler


#Region "inner Methods"

        ' Methods

        Protected Overrides Function CreateChildControls(ByVal dataSource As IEnumerable, ByVal dataBinding As Boolean) _
            As Integer
            Dim ret As Integer = MyBase.CreateChildControls(dataSource, dataBinding)
            If (((Me.Columns.Count <> 0) AndAlso Not MyBase.DesignMode) AndAlso Me.AllowInsert) Then
                Me.ShowFooter = True
                Dim flds As DataControlField() = New DataControlField(Me.Columns.Count - 1) {}
                Me.Columns.CopyTo(flds, 0)
                If (ret = 0) Then
                    Me.Controls.Clear()
                    Dim t As New Table
                    Me.Controls.Add(t)
                    If Me.ShowHeader Then
                        Dim _
                            r As GridViewRow = _
                                Me.CreateRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal)
                        Me.InitializeRow(r, flds)
                        t.Rows.Add(r)
                    End If

                    Me.gvFooterRow = Me.CreateRow(-1, -1, DataControlRowType.Footer, DataControlRowState.Insert)
                    Me.InitializeRow(Me.gvFooterRow, flds)
                    t.Rows.Add(Me.gvFooterRow)
                Else
                    Me.gvFooterRow = MyBase.FooterRow
                End If
                Me.FooterRow.RowState = DataControlRowState.Insert
                Me.FooterRow.Visible = (MyBase.EditIndex < 0)

                'set insert command controls
                Dim i As Integer
                For i = 0 To Me.Columns.Count - 1
                    Dim cell As DataControlFieldCell = DirectCast(Me.FooterRow.Cells.Item(i), DataControlFieldCell)
                    Dim fld As DataControlField = Me.Columns.Item(i)
                    fld.InsertVisible = fld.Visible
                    If TypeOf fld Is CommandField Then
                        Dim cf As CommandField = DirectCast(fld, CommandField)
                        Dim ins As New CommandField
                        ins.ButtonType = cf.ButtonType
                        ins.InsertImageUrl = cf.InsertImageUrl
                        ins.InsertText = cf.InsertText
                        ins.CancelImageUrl = cf.CancelImageUrl
                        ins.CancelText = cf.CancelText
                        ins.InsertVisible = True
                        ins.ShowInsertButton = True
                        ins.Initialize(False, Me)
                        ins.InitializeCell(cell, DataControlCellType.DataCell, DataControlRowState.Insert, -1)
                    ElseIf (cell.Controls.Count = 0) Then
                        fld.Initialize(MyBase.AllowSorting, Me)
                        fld.InitializeCell(cell, DataControlCellType.DataCell, _
                                            (DataControlRowState.Insert Or DataControlRowState.Edit), -1)
                        Dim buttons As List(Of IButtonControl) = FindControlsRecursive(Of IButtonControl)(cell)
                        For Each button As IButtonControl In buttons
                            If button.CommandArgument = "" Then
                                button.CommandArgument = "-1"
                            End If
                            If button.CommandName = "Update" Then
                                button.CommandName = "Insert"
                            End If
                        Next

                    End If
                Next i

                Dim dict As New OrderedDictionary
                MyBase.ExtractRowValues(dict, Me.FooterRow, True, True)
                Dim dataItem As Object = Nothing

                Dim tbl As New DataTable
                Dim row As DataRow = tbl.Rows.Add()
                Dim k As String
                For Each k In dict.Keys
                    tbl.Columns.Add(New DataColumn(k))
                    row.Item(k) = dict.Item(k)
                Next
                dataItem = New DataView(tbl).Item(0)

                Me.FooterRow.DataItem = dataItem
                Dim args1 As New GridViewRowEventArgs(Me.FooterRow)
                Me.OnRowCreated(args1)
                Me.FooterRow.DataBind()
                Me.OnRowDataBound(args1)
                Me.FooterRow.DataItem = Nothing
                Dim f As DataControlField
                For Each f In Me.Columns
                    f.Initialize(Me.AllowSorting, Me)
                Next
            End If
            Return ret
        End Function

        Protected Overrides Sub OnRowCommand(ByVal e As GridViewCommandEventArgs)

            If ("-1".Equals(e.CommandArgument)) Then
                If (e.CommandName = "Cancel") Then
                    Me.Page.Trace.Warn("Canceling insert...")
                ElseIf (e.CommandName = "Insert") Or (e.CommandName = "Update") Then
                    If Me.Page.IsValid Then

                        Dim args As New GridViewUpdateEventArgs(-1)
                        MyBase.ExtractRowValues(args.NewValues, Me.FooterRow, True, True)
                        Me.InsertValues = args.NewValues
                        Dim allEmpty As Boolean = True
                        'Dim v As String
                        'For Each v In Me.InsertValues.Values
                        '    allEmpty = ((v Is Nothing) OrElse (v.Trim.Length = 0))
                        '    If Not allEmpty Then
                        '        Exit For
                        '    End If
                        'Next
                        'If Not allEmpty Then
                        Me.OnRowInserting(args)
                        If Not args.Cancel AndAlso Me.GetData.CanInsert Then
                            Me.GetData.Insert(args.NewValues, AddressOf SetEventHandler)
                        End If
                        'End If
                    End If
                End If
            Else
                MyBase.OnRowCommand(e)
            End If
            If Me.AllowInsert Then
                'Me.AllowInsert = False
                'Me.DataBind()
            End If

            'End If


        End Sub

        Private Function SetEventHandler(ByVal affectedRows As Integer, ByVal ex As Exception) As Boolean

            Dim evt As New GridViewUpdatedEventArgs(affectedRows, ex)
            evt.NewValues.Clear()
            Dim v As String
            For Each v In Me.InsertValues.Keys
                evt.NewValues.Add(v, Me.InsertValues.Item(v))
            Next
            Me.OnRowInserted(evt)
            If ((Not ex Is Nothing) AndAlso Not evt.ExceptionHandled) Then
                Return False
            End If
            MyBase.RequiresDataBinding = True
            Return True
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If (Not savedState Is Nothing) AndAlso Not Me.IsBoundUsingDataSourceID Then
                Dim objArray As Object() = DirectCast(savedState, Object())
                If (Not objArray(10) Is Nothing) Then
                    'TODO: Décommenter la ligne suivante et retrouver la classe OrderedDictionaryStateHelper ?
                    Me.LoadViewState(DirectCast(Me.DynamicBoundFieldValues, OrderedDictionary), _
                                      DirectCast(objArray(10), ArrayList))
                End If
            End If

            MyBase.LoadViewState(savedState)
        End Sub

        Public Overloads Sub LoadViewState(ByVal dictionary As IOrderedDictionary, ByVal state As ArrayList)
            If (dictionary Is Nothing) Then
                Throw New ArgumentNullException("dictionary")
            End If
            If (state Is Nothing) Then
                Throw New ArgumentNullException("state")
            End If
            If (Not state Is Nothing) Then
                Dim i As Integer
                For i = 0 To state.Count - 1
                    Dim pair As Pair = DirectCast(state.Item(i), Pair)
                    dictionary.Add(pair.First, pair.Second)
                Next i
            End If
        End Sub


        Protected Overrides Sub OnRowUpdating(ByVal e As GridViewUpdateEventArgs)
            If Not Me.IsBoundUsingDataSourceID Then
                Dim entry As DictionaryEntry
                For Each entry In Me.DynamicBoundFieldValues
                    e.OldValues.Add(entry.Key, entry.Value)
                Next
                If (Me.DataKeys.Count > e.RowIndex) Then
                    Dim entry2 As DictionaryEntry
                    For Each entry2 In Me.DataKeys.Item(e.RowIndex).Values
                        e.Keys.Add(entry2.Key, entry2.Value)
                    Next
                End If
                Dim row As GridViewRow
                If (Me.Rows.Count > e.RowIndex) Then
                    row = Me.Rows.Item(e.RowIndex)
                    Me.ExtractRowValues(e.NewValues, row, False, True)
                End If
            End If
            MyBase.OnRowUpdating(e)
        End Sub

        Protected Overridable Sub OnRowInserted(ByVal e As GridViewUpdatedEventArgs)
            RaiseEvent RowInserted(Me, e)
        End Sub

        Protected Overridable Sub OnRowInserting(ByVal e As GridViewUpdateEventArgs)
            RaiseEvent RowInserting(Me, e)
        End Sub

#End Region

#Region "Public Properties"


        ' Properties
        <Browsable(True), DefaultValue("true"), Category("Behavior")> _
        Public Property AllowInsert() As Boolean
            Get
                Dim o As Object = Me.ViewState.Item("_InsOk")
                If (Not o Is Nothing) Then
                    Return CBool(o)
                End If
                Return True
            End Get
            Set(ByVal value As Boolean)
                Me.ViewState.Item("_InsOk") = value
                Me.ShowFooter = value
            End Set
        End Property

        Protected ReadOnly Property DynamicBoundFieldValues() As IOrderedDictionary
            Get
                If (Me._DynamicBoundFieldValues Is Nothing) Then
                    Dim count As Integer = Me.Columns.Count
                    If Me.AutoGenerateColumns Then
                        count = (count + 10)
                    End If
                    Me._DynamicBoundFieldValues = New OrderedDictionary(count)
                End If
                Return Me._DynamicBoundFieldValues
            End Get
        End Property

        Public Overrides ReadOnly Property FooterRow() As GridViewRow
            Get
                If (Not Me.gvFooterRow Is Nothing) Then
                    Return Me.gvFooterRow
                End If
                Return MyBase.FooterRow
            End Get
        End Property

        Protected Property InsertValues() As IOrderedDictionary
            Get
                Return Me._InsertValues
            End Get
            Set(ByVal value As IOrderedDictionary)
                Me._InsertValues = value
            End Set
        End Property

        Public ReadOnly Property HumanPageIndex() As Integer
            Get
                Return Me.PageIndex + 1
            End Get
        End Property


#End Region
    End Class
End Namespace


