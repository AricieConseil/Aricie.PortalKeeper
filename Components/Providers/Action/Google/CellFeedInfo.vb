Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Google.GData.Spreadsheets

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class CellFeedInfo(Of TEngineEvents As IConvertible)

        Public Property UseExistingWorksheetEntry As Boolean

        <ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        Public Property WorksheetEntryExpression As New SimpleExpression(Of WorksheetEntry)()

        '<AutoPostBack()> _
        '<ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        'Public Property CreateIf As New KeeperCondition(Of TEngineEvents)

        <AutoPostBack()> _
        <ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property ShowWorksheetEntryInfo As Boolean
            Get
                Return (Not UseExistingWorksheetEntry) OrElse CreateIfNull 'CreateIf.Instances.Count > 0
            End Get
        End Property

        Public Property CaptureCellFeed As Boolean

        <ConditionalVisible("CaptureCellFeed", False, True)> _
        Public Property CellFeedName As String = "Cells"

        <ConditionalVisible("ShowWorksheetEntryInfo", False, True)> _
        Public Property WorksheetInfo As New WorksheetEntryInfo(Of TEngineEvents)

        Public Function GetWorkSheetAndCellFeed(actionContext As PortalKeeperContext(Of TEngineEvents)) As KeyValuePair(Of WorksheetEntry, CellFeed)
            Dim objWorksheetEntry As WorksheetEntry = Nothing
            If Me.UseExistingWorksheetEntry Then
                objWorksheetEntry = WorksheetEntryExpression.Evaluate(actionContext, actionContext)
            End If
            If (Not UseExistingWorksheetEntry) OrElse (objWorksheetEntry Is Nothing AndAlso CreateIfNull) Then ' CreateIf.Match(actionContext)) Then
                objWorksheetEntry = WorksheetInfo.GetWorksheetEntry(actionContext)
            End If
            Dim objCellFeed As CellFeed = objWorksheetEntry.QueryCellFeed()
            If Me.CaptureCellFeed Then
                actionContext.Item(Me.CellFeedName) = objCellFeed
            End If
            Return New KeyValuePair(Of WorksheetEntry, CellFeed)(objWorksheetEntry, objCellFeed)
        End Function

        Public Sub AddVariables(ByRef existingVars As IDictionary(Of String, Type))

            Me.WorksheetInfo.AddVariables(existingVars)

            If Me.CaptureCellFeed Then
                existingVars(Me.CellFeedName) = GetType(CellFeed)
            End If

        End Sub



        'Public Sub UpdateEntries(objWorksheet As WorksheetEntry, objSpreadsheet As SpreadsheetEntry)

        'End Sub


    End Class
End NameSpace