Imports System.Web.UI.WebControls
Imports System.Web.UI

Namespace Web.UI.Controls.GridViewGrouping
    ''' <summary>
    ''' Helper class for implementing GridView items grouping
    ''' </summary>
    Public Class GridViewHelper
        ' Events
        Public Event FooterDataBound As FooterEvent
        Public Event GeneralSummary As FooterEvent
        Public Event GroupEnd As GroupEvent
        Public Event GroupHeader As GroupEvent
        Public Event GroupStart As GroupEvent
        Public Event GroupSummary As GroupEvent

        ' Methods
        Public Sub New(ByVal grd As GridView)
            Me.New(grd, False, SortDirection.Ascending)
        End Sub

        Public Sub New(ByVal grd As GridView, ByVal useFooterForGeneralSummaries As Boolean)
            Me.New(grd, useFooterForGeneralSummaries, SortDirection.Ascending)
        End Sub

        Public Sub New(ByVal grd As GridView, ByVal useFooterForGeneralSummaries As Boolean, _
                        ByVal groupSortDirection As SortDirection)
            Me.mGrid = grd
            Me.useFooter = useFooterForGeneralSummaries
            Me.groupSortDir = groupSortDirection
            Me.mGeneralSummaries = New GridViewSummaryList
            Me.mGroups = New GridViewGroupList
            AddHandler Me.mGrid.RowDataBound, New GridViewRowEventHandler(AddressOf Me.RowDataBoundHandler)
        End Sub

        Public Sub ApplyGroupSort()
            Me.mGrid.Sort(Me.GetSequentialGroupColumns, Me.groupSortDir)
        End Sub

        Private Function ColumnHasSummary(ByVal colindex As Integer, ByVal g As GridViewGroup) As Boolean
            Dim list As List(Of GridViewSummary)
            Dim column As String = Me.GetDataFieldName(Me.mGrid.Columns.Item(colindex))
            If (g Is Nothing) Then
                list = Me.mGeneralSummaries
            Else
                list = g.Summaries
            End If
            Dim s As GridViewSummary
            For Each s In list
                If (column.ToLower = s.Column.ToLower) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Function ColumnHasSummary(ByVal column As String, ByVal g As GridViewGroup) As Boolean
            Dim list As List(Of GridViewSummary)
            If (g Is Nothing) Then
                list = Me.mGeneralSummaries
            Else
                list = g.Summaries
            End If
            Dim s As GridViewSummary
            For Each s In list
                If (column.ToLower = s.Column.ToLower) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Function EvaluateEquals(ByVal g As GridViewGroup, ByVal dataitem As Object) As Boolean
            If (g.ActualValues Is Nothing) Then
                Return False
            End If
            Dim i As Integer
            For i = 0 To g.Columns.Length - 1
                If ((g.ActualValues(i) Is Nothing) AndAlso (Not DataBinder.Eval(dataitem, g.Columns(i)) Is Nothing)) _
                    Then
                    Return False
                End If
                If ((Not g.ActualValues(i) Is Nothing) AndAlso (DataBinder.Eval(dataitem, g.Columns(i)) Is Nothing)) _
                    Then
                    Return False
                End If
                If Not g.ActualValues(i).Equals(DataBinder.Eval(dataitem, g.Columns(i))) Then
                    Return False
                End If
            Next i
            Return True
        End Function

        Private Sub GenerateGeneralSummaries(ByVal e As GridViewRowEventArgs)
            If Not Me.HasAutoSummary(Me.mGeneralSummaries) Then
                RaiseEvent GeneralSummary(e.Row)
            Else
                Dim row As GridViewRow
                If Me.useFooter Then
                    row = e.Row
                Else
                    row = Me.InsertGridRow(e.Row, Nothing)
                End If
                Dim s As GridViewSummary
                For Each s In Me.mGeneralSummaries
                    Dim colIndex As Integer
                    If Not s.Automatic Then
                        Continue For
                    End If
                    If Me.useFooter Then
                        colIndex = Me.GetColumnIndex(s.Column)
                    Else
                        colIndex = Me.GetVisibleColumnIndex(s.Column)
                    End If
                    colIndex = Me.ResolveCellIndex(row, colIndex)
                    row.Cells.Item(colIndex).Text = _
                        Me.GetFormatedString(s.FormatString, Me.GetColumnFormat(Me.GetColumnIndex(s.Column)), s.Value)
                Next
                RaiseEvent GeneralSummary(row)
            End If
        End Sub

        Private Sub GenerateGroupSummary(ByVal g As GridViewGroup, ByVal row As GridViewRow)
            If (Me.HasAutoSummary(g.Summaries) OrElse Me.HasSuppressGroup) Then
                Dim colIndex As Integer
                Dim newRow As GridViewRow = Me.InsertGridRow(row, g)
                Dim s As GridViewSummary
                For Each s In g.Summaries
                    If s.Automatic Then
                        colIndex = Me.GetVisibleColumnIndex(s.Column)
                        colIndex = Me.ResolveCellIndex(newRow, colIndex)
                        newRow.Cells.Item(colIndex).Text = _
                            Me.GetFormatedString(s.FormatString, Me.GetColumnFormat(Me.GetColumnIndex(s.Column)), _
                                                  s.Value)
                    End If
                Next
                If g.IsSuppressGroup Then
                    Dim i As Integer
                    For i = 0 To g.Columns.Length - 1
                        Dim colValue As Object = g.ActualValues(i)
                        If (Not colValue Is Nothing) Then
                            colIndex = Me.GetVisibleColumnIndex(g.Columns(i))
                            colIndex = Me.ResolveCellIndex(newRow, colIndex)
                            newRow.Cells.Item(colIndex).Text = colValue.ToString
                        End If
                    Next i
                End If
                RaiseEvent GroupSummary(g.Name, g.ActualValues, newRow)
            End If
        End Sub

        Public Function GetColumnFormat(ByVal colIndex As Integer) As String
            If TypeOf Me.mGrid.Columns.Item(colIndex) Is BoundField Then
                Return TryCast(Me.mGrid.Columns.Item(colIndex), BoundField).DataFormatString
            End If
            Return String.Empty
        End Function

        Public Function GetColumnIndex(ByVal columnName As String) As Integer
            Dim i As Integer
            For i = 0 To Me.mGrid.Columns.Count - 1
                Dim dfield As String = Me.GetDataFieldName(Me.mGrid.Columns.Item(i)).ToLower
                If dfield <> "" Then
                    If columnName.ToLower.Contains(dfield) Then
                        Return i
                    End If
                End If
            Next i
            Return -1
        End Function

        Public Function GetDataFieldName(ByVal field As DataControlField) As String
            If TypeOf field Is BoundField Then
                Return TryCast(field, BoundField).DataField
            End If
            Return field.SortExpression
        End Function

        Private Function GetFormatedString(ByVal preferredFormat As String, ByVal secondFormat As String, _
                                            ByVal value As Object) As String
            Dim format As String = preferredFormat
            If (format.Length = 0) Then
                format = secondFormat
            End If
            If (format.Length > 0) Then
                Return String.Format(format, value)
            End If
            Return value.ToString
        End Function

        Private Function GetGroupRowValues(ByVal g As GridViewGroup, ByVal dataitem As Object) As Object()
            Dim values As Object() = New Object(g.Columns.Length - 1) {}
            Dim i As Integer
            For i = 0 To g.Columns.Length - 1
                values(i) = DataBinder.Eval(dataitem, g.Columns(i))
            Next i
            Return values
        End Function

        Public Function GetRealIndexFromVisibleColumnIndex(ByVal visibleIndex As Integer) As Integer
            Dim visibles As Integer = 0
            Dim i As Integer
            For i = 0 To Me.mGrid.Columns.Count - 1
                If Me.mGrid.Columns.Item(i).Visible Then
                    If (visibleIndex = visibles) Then
                        Return i
                    End If
                    visibles += 1
                End If
            Next i
            Return -1
        End Function

        Private Function GetSequentialGroupColumns() As String
            Dim ret As String = String.Empty
            Dim g As GridViewGroup
            For Each g In Me.mGroups
                ret = (ret & g.Name.Replace("+"c, ","c) & ",")
            Next
            Return ret.Substring(0, (ret.Length - 1))
        End Function

        Public Function GetVisibleColumnCount() As Integer
            Dim ret As Integer = 0
            Dim i As Integer
            For i = 0 To Me.mGrid.Columns.Count - 1
                If Me.mGrid.Columns.Item(i).Visible Then
                    ret += 1
                End If
            Next i
            Return ret
        End Function

        Public Function GetVisibleColumnIndex(ByVal columnName As String) As Integer
            Dim visibles As Integer = 0
            Dim i As Integer
            For i = 0 To Me.mGrid.Columns.Count - 1
                Dim dfield As String = Me.GetDataFieldName(Me.mGrid.Columns.Item(i)).ToLower
                If dfield <> "" Then
                    If columnName.ToLower.Contains(dfield) Then
                        Return visibles
                    End If
                End If
                If Me.mGrid.Columns.Item(i).Visible Then
                    visibles += 1
                End If
            Next i
            Return -1
        End Function

        Private Function HasAutoSummary(ByVal list As List(Of GridViewSummary)) As Boolean
            Dim s As GridViewSummary
            For Each s In list
                If s.Automatic Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Function HasSuppressGroup() As Boolean
            Dim g As GridViewGroup
            For Each g In Me.mGroups
                If g.IsSuppressGroup Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub HideColumnsWithoutGroupSummary()
            Dim dcf As DataControlField
            For Each dcf In Me.mGrid.Columns
                Dim colChecked As Boolean = False
                Dim colname As String = Me.GetDataFieldName(dcf).ToLower
                Dim g As GridViewGroup
                For Each g In Me.mGroups
                    Dim j As Integer
                    For j = 0 To g.Columns.Length - 1
                        If (colname = g.Columns(j).ToLower) Then
                            colChecked = True
                            Exit For
                        End If
                    Next j
                    If colChecked Then
                        Exit For
                    End If
                    colChecked = Me.ColumnHasSummary(colname, g)
                    If colChecked Then
                        Exit For
                    End If
                Next
                If Not colChecked Then
                    dcf.Visible = False
                End If
            Next
        End Sub

        Public Function InsertGridRow(ByVal beforeRow As GridViewRow) As GridViewRow
            Dim visibleColumns As Integer = Me.GetVisibleColumnCount
            Dim tbl As Table = DirectCast(Me.mGrid.Controls.Item(0), Table)
            Dim newRowIndex As Integer = tbl.Rows.GetRowIndex(beforeRow)
            Dim _
                newRow As _
                    New GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal)
            newRow.Cells.Add(New TableCell)
            If (visibleColumns > 1) Then
                newRow.Cells.Item(0).ColumnSpan = visibleColumns
            End If
            tbl.Controls.AddAt(newRowIndex, newRow)
            Return newRow
        End Function

        Private Function InsertGridRow(ByVal beforeRow As GridViewRow, ByVal g As GridViewGroup) As GridViewRow
            Dim cell As TableCell
            Dim tcArray As TableCell()
            Dim i As Integer
            Dim visibleColumns As Integer = Me.GetVisibleColumnCount
            Dim tbl As Table = DirectCast(Me.mGrid.Controls.Item(0), Table)
            Dim newRowIndex As Integer = tbl.Rows.GetRowIndex(beforeRow)
            Dim _
                newRow As _
                    New GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal)
            If ((Not g Is Nothing) AndAlso (g.IsSuppressGroup OrElse g.GenerateAllCellsOnSummaryRow)) Then
                tcArray = New TableCell(visibleColumns - 1) {}
                For i = 0 To visibleColumns - 1
                    cell = New TableCell
                    cell.ApplyStyle(Me.mGrid.Columns.Item(Me.GetRealIndexFromVisibleColumnIndex(i)).ItemStyle)
                    cell.Text = "&nbsp;"
                    tcArray(i) = cell
                Next i
            Else
                Dim colspan As Integer = 0
                Dim tcc As New List(Of TableCell)
                For i = 0 To Me.mGrid.Columns.Count - 1
                    If Me.ColumnHasSummary(i, g) Then
                        If (colspan > 0) Then
                            cell = New TableCell
                            cell.Text = "&nbsp;"
                            cell.ColumnSpan = colspan
                            tcc.Add(cell)
                            colspan = 0
                        End If
                        cell = New TableCell
                        cell.ApplyStyle(Me.mGrid.Columns.Item(i).ItemStyle)
                        tcc.Add(cell)
                    ElseIf Me.mGrid.Columns.Item(i).Visible Then
                        colspan += 1
                    End If
                Next i
                If (colspan > 0) Then
                    cell = New TableCell
                    cell.Text = "&nbsp;"
                    cell.ColumnSpan = colspan
                    tcc.Add(cell)
                    colspan = 0
                End If
                tcArray = New TableCell(tcc.Count - 1) {}
                tcc.CopyTo(tcArray)
            End If
            newRow.Cells.AddRange(tcArray)
            tbl.Controls.AddAt(newRowIndex, newRow)
            Return newRow
        End Function

        Private Sub ProcessGroup(ByVal g As GridViewGroup, ByVal e As GridViewRowEventArgs)
            Dim groupHeaderText As String = String.Empty
            If Not Me.EvaluateEquals(g, e.Row.DataItem) Then
                If (Not g.ActualValues Is Nothing) Then
                    g.CalculateSummaries()
                    Me.GenerateGroupSummary(g, e.Row)
                    RaiseEvent GroupEnd(g.Name, g.ActualValues, e.Row)
                End If
                g.Reset()
                g.SetActualValues(Me.GetGroupRowValues(g, e.Row.DataItem))
                If g.Automatic Then
                    Dim v As Integer
                    For v = 0 To g.ActualValues.Length - 1
                        If (Not g.ActualValues(v) Is Nothing) Then
                            groupHeaderText = (groupHeaderText & g.ActualValues(v).ToString)
                            If ((g.ActualValues.Length - v) > 1) Then
                                groupHeaderText = (groupHeaderText & " - ")
                            End If
                        End If
                    Next v
                    Dim newRow As GridViewRow = Me.InsertGridRow(e.Row)
                    newRow.Cells.Item(0).Text = groupHeaderText
                    RaiseEvent GroupHeader(g.Name, g.ActualValues, newRow)
                End If
                RaiseEvent GroupStart(g.Name, g.ActualValues, e.Row)
            End If
            g.AddValueToSummaries(e.Row.DataItem)
        End Sub

        Public Function RegisterGroup(ByVal column As String, ByVal [auto] As Boolean, _
                                       ByVal hideGroupColumns As Boolean) As GridViewGroup
            Dim cols As String() = New String() {column}
            Return Me.RegisterGroup(cols, [auto], hideGroupColumns)
        End Function

        Public Function RegisterGroup(ByVal columns As String(), ByVal [auto] As Boolean, _
                                       ByVal hideGroupColumns As Boolean) As GridViewGroup
            If Me.HasSuppressGroup Then
                Throw _
                    New Exception( _
                                   "A suppress group is already defined. You can't define suppress AND summary groups simultaneously")
            End If
            Dim g As New GridViewGroup(columns, [auto], hideGroupColumns)
            Me.mGroups.Add(g)
            If hideGroupColumns Then
                Dim i As Integer
                For i = 0 To Me.mGrid.Columns.Count - 1
                    Dim j As Integer
                    For j = 0 To columns.Length - 1
                        If (Me.GetDataFieldName(Me.mGrid.Columns.Item(i)).ToLower = columns(j).ToLower) Then
                            Me.mGrid.Columns.Item(i).Visible = False
                        End If
                    Next j
                Next i
            End If
            Return g
        End Function

        Public Function RegisterSummary(ByVal s As GridViewSummary) As GridViewSummary
            If Not s.Validate Then
                Throw New Exception("Invalid summary.")
            End If
            If (s.Group Is Nothing) Then
                If Me.useFooter Then
                    Me.mGrid.ShowFooter = True
                End If
                Me.mGeneralSummaries.Add(s)
                Return s
            End If
            If Not s.Group.ContainsSummary(s) Then
                s.Group.AddSummary(s)
            End If
            Return s
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal operation As SummaryOperation) As GridViewSummary
            Return Me.RegisterSummary(column, String.Empty, operation)
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal operation As CustomSummaryOperation, _
                                         ByVal getResult As SummaryResultMethod) As GridViewSummary
            Return Me.RegisterSummary(column, String.Empty, operation, getResult)
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal operation As SummaryOperation, _
                                         ByVal groupName As String) As GridViewSummary
            Return Me.RegisterSummary(column, String.Empty, operation, groupName)
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal formatString As String, _
                                         ByVal operation As SummaryOperation) As GridViewSummary
            If (operation = SummaryOperation.Custom) Then
                Throw New Exception("Use adequate method to register a summary with custom operation.")
            End If
            Dim s As New GridViewSummary(column, formatString, operation, Nothing)
            Me.mGeneralSummaries.Add(s)
            If Me.useFooter Then
                Me.mGrid.ShowFooter = True
            End If
            Return s
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal operation As CustomSummaryOperation, _
                                         ByVal getResult As SummaryResultMethod, ByVal groupName As String) _
            As GridViewSummary
            Return Me.RegisterSummary(column, String.Empty, operation, getResult, groupName)
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal formatString As String, _
                                         ByVal operation As CustomSummaryOperation, _
                                         ByVal getResult As SummaryResultMethod) As GridViewSummary
            Dim s As New GridViewSummary(column, formatString, operation, getResult, Nothing)
            Me.mGeneralSummaries.Add(s)
            If Me.useFooter Then
                Me.mGrid.ShowFooter = True
            End If
            Return s
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal formatString As String, _
                                         ByVal operation As SummaryOperation, ByVal groupName As String) _
            As GridViewSummary
            If (operation = SummaryOperation.Custom) Then
                Throw New Exception("Use adequate method to register a summary with custom operation.")
            End If
            Dim group As GridViewGroup = Me.mGroups.Item(groupName)
            If (group Is Nothing) Then
                Throw _
                    New Exception( _
                                   String.Format("Group {0} not found. Please register the group before the summary.", _
                                                  groupName))
            End If
            Dim s As New GridViewSummary(column, formatString, operation, group)
            group.AddSummary(s)
            Return s
        End Function

        Public Function RegisterSummary(ByVal column As String, ByVal formatString As String, _
                                         ByVal operation As CustomSummaryOperation, _
                                         ByVal getResult As SummaryResultMethod, ByVal groupName As String) _
            As GridViewSummary
            Dim group As GridViewGroup = Me.mGroups.Item(groupName)
            If (group Is Nothing) Then
                Throw _
                    New Exception( _
                                   String.Format("Group {0} not found. Please register the group before the summary.", _
                                                  groupName))
            End If
            Dim s As New GridViewSummary(column, formatString, operation, getResult, group)
            group.AddSummary(s)
            Return s
        End Function

        Private Function ResolveCellIndex(ByVal row As GridViewRow, ByVal colIndex As Integer) As Integer
            Dim colspansum As Integer = 0
            Dim i As Integer
            For i = 0 To row.Cells.Count - 1
                Dim realIndex As Integer = (i + colspansum)
                If (realIndex = colIndex) Then
                    Return i
                End If
                If (row.Cells.Item(i).ColumnSpan > 1) Then
                    colspansum = ((colspansum + row.Cells.Item(i).ColumnSpan) - 1)
                End If
            Next i
            Return -1
        End Function

        Private Sub RowDataBoundHandler(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
            Dim g As GridViewGroup
            For Each g In Me.mGroups
                If (e.Row.RowType = DataControlRowType.Footer) Then
                    g.CalculateSummaries()
                    Me.GenerateGroupSummary(g, e.Row)
                    RaiseEvent GroupEnd(g.Name, g.ActualValues, e.Row)
                ElseIf (e.Row.RowType = DataControlRowType.DataRow) Then
                    Me.ProcessGroup(g, e)
                    If g.IsSuppressGroup Then
                        e.Row.Visible = False
                    End If
                ElseIf (e.Row.RowType = DataControlRowType.Pager) Then
                    Dim originalCell As TableCell = e.Row.Cells.Item(0)
                    Dim newCell As New TableCell
                    newCell.Visible = False
                    e.Row.Cells.AddAt(0, newCell)
                    originalCell.ColumnSpan = Me.GetVisibleColumnCount
                End If
            Next
            Dim s As GridViewSummary
            For Each s In Me.mGeneralSummaries
                If (e.Row.RowType = DataControlRowType.Header) Then
                    s.Reset()
                ElseIf (e.Row.RowType = DataControlRowType.DataRow) Then
                    s.AddValue(DataBinder.Eval(e.Row.DataItem, s.Column))
                ElseIf (e.Row.RowType = DataControlRowType.Footer) Then
                    s.Calculate()
                End If
            Next
            If (e.Row.RowType = DataControlRowType.Footer) Then
                Me.GenerateGeneralSummaries(e)
                RaiseEvent FooterDataBound(e.Row)
            End If
        End Sub

        Public Sub SetInvisibleColumnsWithoutGroupSummary()
            Me.HideColumnsWithoutGroupSummary()
        End Sub

        Public Function SetSuppressGroup(ByVal columns As String()) As GridViewGroup
            If (Me.mGroups.Count > 0) Then
                Throw _
                    New Exception( _
                                   "At least a group is already defined. A suppress group can't coexist with other groups")
            End If
            Dim g As New GridViewGroup(columns, True, False, False, False)
            Me.mGroups.Add(g)
            Me.mGrid.AllowPaging = False
            Return g
        End Function

        Public Function SetSuppressGroup(ByVal column As String) As GridViewGroup
            Dim cols As String() = New String() {column}
            Return Me.SetSuppressGroup(cols)
        End Function


        ' Properties
        Public ReadOnly Property GeneralSummaries() As GridViewSummaryList
            Get
                Return Me.mGeneralSummaries
            End Get
        End Property

        Public ReadOnly Property Groups() As GridViewGroupList
            Get
                Return Me.mGroups
            End Get
        End Property


        ' Fields
        Private Const GROUP_NOT_FOUND As String = "Group {0} not found. Please register the group before the summary."
        Private groupSortDir As SortDirection
        Private Const INVALID_SUMMARY As String = "Invalid summary."
        Private mGeneralSummaries As GridViewSummaryList
        Private mGrid As GridView
        Private mGroups As GridViewGroupList

        Private _
            Const _
            ONE_GROUP_ALREADY_REGISTERED As String = _
            "At least a group is already defined. A suppress group can't coexist with other groups"

        Private _
            Const _
            SUPPRESS_GROUP_ALREADY_DEFINED As String = _
            "A suppress group is already defined. You can't define suppress AND summary groups simultaneously"

        Private _
            Const _
            USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY As String = _
            "Use adequate method to register a summary with custom operation."

        Private useFooter As Boolean
    End Class
End Namespace


