Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports Aricie.ComponentModel
Imports Aricie.Collections
Imports Google.GData.Client
Imports Google.GData.Spreadsheets
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Table, IconOptions.Normal)> _
    <Serializable()> _
    Public Class SpreadSheetCommand(Of TEngineEvents As IConvertible)
        Inherits NamedConfig

        Public Property Operation As SpreadSheetOperation

        Public Property Row As New SimpleOrExpression(Of Integer)(1)

        Public Property Column As New SimpleOrExpression(Of Integer)(1)

        <ConditionalVisible("Operation", False, True, SpreadSheetOperation.UpdateCell, SpreadSheetOperation.InsertCell)> _
        Public Property Value As New SimpleOrExpression(Of CData)

        <ConditionalVisible("Operation", False, True, SpreadSheetOperation.ReadCell)> _
        Public Property ReadInput As Boolean

        <ConditionalVisible("Operation", False, True, SpreadSheetOperation.ReadCell)> _
        Public Property Key As New SimpleOrExpression(Of String)("<cell Name>")

        Public Property Condition As New KeeperCondition(Of TEngineEvents)

        Public Function Execute(objService As SpreadsheetsService, ByVal objWorksheetEntry As WorksheetEntry, objCellFeed As CellFeed, actionContext As PortalKeeperContext(Of TEngineEvents), ByRef reader As SerializableDictionary(Of String, String)) As WorksheetEntry
            If Me.Enabled AndAlso Condition.Match(actionContext) Then
                Dim intRow As UInt32 = Convert.ToUInt32(Me.Row.GetValue(actionContext, actionContext))
                Dim intColumn As UInt32 = Convert.ToUInt32(Me.Column.GetValue(actionContext, actionContext))
                Select Case Operation
                    Case SpreadSheetOperation.ReadCell
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)
                        Dim objKey As String = Key.GetValue(actionContext, actionContext)
                        Dim objValue As String
                        If ReadInput Then
                            objValue = entry.Cell.InputValue
                        Else
                            objValue = entry.Cell.Value
                        End If
                        If objValue Is Nothing Then
                            objValue = ""
                        End If
                        reader(objKey) = objValue
                    Case SpreadSheetOperation.InsertCell
                        Dim entry As New CellEntry()
                        Dim cell As New CellEntry.CellElement()
                        cell.InputValue = Me.Value.GetValue(actionContext, actionContext)
                        cell.Row = intRow
                        cell.Column = intColumn
                        entry.Cell = cell
                        If cell.Row > objWorksheetEntry.Rows OrElse cell.Column > objWorksheetEntry.Cols Then
                            Dim wsLink As AtomLink = objWorksheetEntry.Links.FindService(GDataSpreadsheetsNameTable.ServiceSelf, AtomLink.ATOM_TYPE)
                            Dim wsQuery As New WorksheetQuery(wsLink.HRef.Content)
                            Dim objWFeed As WorksheetFeed = DirectCast(objWorksheetEntry.Service.Query(wsQuery), WorksheetFeed)
                            For Each newWorksheetEntry As WorksheetEntry In objWFeed.Entries
                                If newWorksheetEntry.Title.Text = objWorksheetEntry.Title.Text Then
                                    objWorksheetEntry = newWorksheetEntry
                                    objWorksheetEntry.Rows = CUInt(Math.Max(cell.Row, objWorksheetEntry.Rows))
                                    objWorksheetEntry.Cols = CUInt(Math.Max(cell.Column, objWorksheetEntry.Cols))
                                    objWorksheetEntry = DirectCast(objWorksheetEntry.Update(), WorksheetEntry)
                                    Exit For
                                End If
                            Next
                        End If
                        objService.Insert(objCellFeed, entry)
                    Case SpreadSheetOperation.UpdateCell
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)
                        entry.Cell.Value = Nothing
                        entry.Cell.NumericValue = Nothing
                        entry.Cell.InputValue = Me.Value.GetValue(actionContext, actionContext)
                        objService.Update(entry)
                    Case SpreadSheetOperation.DeleteCell
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)
                        objService.Delete(entry)
                End Select
            End If
            Return objWorksheetEntry
        End Function

    End Class
End NameSpace