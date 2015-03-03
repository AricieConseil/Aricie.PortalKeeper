Imports Telerik.Web.UI
Imports System.Web.UI.WebControls

Namespace UI.WebControls


    Public Class Grid
        Inherits Telerik.Web.UI.RadGrid

        Private Const DefaultPageSize As Integer = 20
        Public Sub New()
            MyBase.new()
            If Not Me.DesignMode Then
                Me.PageSize = defaultPageSize
                Me.AllowPaging = True
                Me.AllowSorting = True
                Me.AllowFilteringByColumn = True
                Me.EnableAjaxSkinRendering = True
               
                Me.AlternatingItemStyle.CssClass = "alternateGridItem"
                'Me.GroupingSettings.CaseSensitive = False
                With Me.MasterTableView

                    .AllowCustomSorting = True
                    .CommandItemSettings.ShowAddNewRecordButton = True
                    .CommandItemSettings.ShowRefreshButton = True
                    .CommandItemDisplay = GridCommandItemDisplay.Top
                    .AllowSorting = True
                    .OverrideDataSourceControlSorting = True

                    '.ItemStyle.Wrap = False

                End With

                
            End If
        End Sub


        Private _HideExpendColumnIfEmpty As Boolean = True

        <System.ComponentModel.DefaultValue(True)> _
        Public Property HideExpendColumnIfEmpty() As Boolean
            Get
                Return _HideExpendColumnIfEmpty
            End Get
            Set(ByVal value As Boolean)
                _HideExpendColumnIfEmpty = value
            End Set
        End Property


        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            If HideExpendColumnIfEmpty Then
                HideExpandColumnRecursive(Me.MasterTableView)
            End If
        End Sub
      



        Public Sub HideExpandColumnRecursive(ByVal tableView As GridTableView)
            Dim nestedViewItems As GridItem() = tableView.GetItems(GridItemType.NestedView)
            For Each nestedViewItem As GridNestedViewItem In nestedViewItems
                For Each nestedView As GridTableView In nestedViewItem.NestedTableViews
                    If nestedView.Items.Count = 0 Then
                        Dim cell As TableCell = nestedView.ParentItem("ExpandColumn")
                        cell.Controls(0).Visible = False
                        nestedViewItem.Visible = False
                    End If
                    If nestedView.HasDetailTables Then
                        HideExpandColumnRecursive(nestedView)
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace