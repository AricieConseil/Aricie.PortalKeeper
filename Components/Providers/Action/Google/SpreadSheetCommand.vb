Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports Aricie.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.Diagnostics
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

        <ConditionalVisible("Operation", False, True, SpreadSheetOperation.UpdateCell, SpreadSheetOperation.InsertCell)> _
        Public Property CheckForChanges As Boolean

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

                        Dim objKey As String = Key.GetValue(actionContext, actionContext)
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Spreadsheet Command Start", _
                                                        WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)

                        Dim objValue As String
                        If ReadInput Then
                            objValue = entry.Cell.InputValue
                        Else
                            objValue = entry.Cell.Value
                        End If
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "End Spreadsheet Command", _
                                                        WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
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
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Spreadsheet Command Start", _
                                                        WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
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
                        Dim isNew As Boolean = True
                        If Me.CheckForChanges Then
                            Dim existingEntry As CellEntry = objCellFeed(intRow, intColumn)
                            If existingEntry IsNot Nothing AndAlso existingEntry.Cell.Value = entry.Cell.Value Then
                                isNew = False
                            End If
                        End If
                        If isNew Then
                            objService.Insert(objCellFeed, entry)
                        End If

                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "End Spreadsheet Command", _
                                                        WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                    Case SpreadSheetOperation.UpdateCell
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Spreadsheet Command Start", _
                                                        WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)
                        Dim olValue As String = entry.Cell.Value
                        entry.Cell.Value = Nothing
                        entry.Cell.NumericValue = Nothing
                        entry.Cell.InputValue = Me.Value.GetValue(actionContext, actionContext)
                        Dim isNew As Boolean = True
                        If Me.CheckForChanges Then
                            If olValue = entry.Cell.Value Then
                                isNew = False
                            End If
                        End If
                        If isNew Then
                            objService.Update(entry)
                        End If
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "End Spreadsheet Command", _
                                                        WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                    Case SpreadSheetOperation.DeleteCell
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "Spreadsheet Command Start", _
                                                        WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                        Dim entry As CellEntry = objCellFeed(intRow, intColumn)
                        objService.Delete(entry)
                        If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                            Dim objStep As New StepInfo(Debug.PKPDebugType, "End Spreadsheet Command", _
                                                        WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                            PerformanceLogger.Instance.AddDebugInfo(objStep)
                        End If
                End Select
            End If
            Return objWorksheetEntry
        End Function

    End Class
End Namespace