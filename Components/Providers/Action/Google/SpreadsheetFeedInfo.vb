Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports Google.GData.Client
Imports Google.GData.Spreadsheets

Namespace Aricie.DNN.Modules.PortalKeeper
    
    Public Class SpreadsheetFeedInfo(Of TEngineEvents As IConvertible)

        <ExtendedCategory("Worksheet")> _
        Public Property UseExistingWorksheetEntry As Boolean

        <ExtendedCategory("Worksheet")> _
        <ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        Public Property WorksheetEntryExpression As New SimpleExpression(Of WorksheetEntry)()

        '<AutoPostBack()> _
        '<ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        'Public Property CreateIf As New KeeperCondition(Of TEngineEvents)

        <ExtendedCategory("Worksheet")> _
        <AutoPostBack()> _
        <ConditionalVisible("UseExistingWorksheetEntry", False, True)> _
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property ShowWorksheetEntryInfo As Boolean
            Get
                Return (Not UseExistingWorksheetEntry) OrElse CreateIfNull 'CreateIf.Instances.Count > 0
            End Get
        End Property

        <ExtendedCategory("Worksheet")> _
        <ConditionalVisible("ShowWorksheetEntryInfo", False, True)> _
        Public Property WorksheetInfo As New WorksheetEntryInfo(Of TEngineEvents)

        <ExtendedCategory("Feed")> _
        Public Property CaptureFeed As Boolean

        <ExtendedCategory("Feed")> _
        <ConditionalVisible("CaptureFeed", False, True)> _
        Public Property FeedName As String = "Feed"



        Public Function GetWorkSheetAndFeed(actionContext As PortalKeeperContext(Of TEngineEvents), mode As GoogleSpreadSheetMode, feedQueryInfo As FeedQueryInfo) As KeyValuePair(Of WorksheetEntry, AbstractFeed)
            Dim objWorksheetEntry As WorksheetEntry = Nothing
            If Me.UseExistingWorksheetEntry Then
                objWorksheetEntry = WorksheetEntryExpression.Evaluate(actionContext, actionContext)
            End If
            If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, "CellFeed Query Start", _
                                            WorkingPhase.EndOverhead, False, False, -1, actionContext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            If (Not UseExistingWorksheetEntry) OrElse (objWorksheetEntry Is Nothing AndAlso CreateIfNull) Then ' CreateIf.Match(actionContext)) Then
                objWorksheetEntry = WorksheetInfo.GetWorksheetEntry(actionContext)
            End If

            Dim objFeed As AbstractFeed

            If mode = GoogleSpreadSheetMode.CellCommands Then
                objFeed = objWorksheetEntry.QueryCellFeed()
            Else
                Dim listFeedLink As AtomLink = objWorksheetEntry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, Nothing)
                Dim query As New ListQuery(listFeedLink.HRef.ToString())
                If feedQueryInfo IsNot Nothing Then
                    feedQueryInfo.SetQuery(query, actionContext)
                End If
                objFeed = DirectCast(objWorksheetEntry.Service, SpreadsheetsService).Query(query)
            End If

            If actionContext.LoggingLevel = LoggingLevel.Detailed Then
                Dim objStep As New StepInfo(Debug.PKPDebugType, "End CellFeed Query", _
                                            WorkingPhase.InProgress, False, False, -1, actionContext.FlowId)
                PerformanceLogger.Instance.AddDebugInfo(objStep)
            End If
            If Me.CaptureFeed Then
                actionContext.Item(Me.FeedName) = objFeed
            End If
            Return New KeyValuePair(Of WorksheetEntry, AbstractFeed)(objWorksheetEntry, objFeed)
        End Function

        Public Sub AddVariables(ByRef existingVars As IDictionary(Of String, Type))

            Me.WorksheetInfo.AddVariables(existingVars)

            If Me.CaptureFeed Then
                existingVars(Me.FeedName) = GetType(CellFeed)
            End If

        End Sub


        'Public Sub UpdateEntries(objWorksheet As WorksheetEntry, objSpreadsheet As SpreadsheetEntry)

        'End Sub


    End Class
End Namespace