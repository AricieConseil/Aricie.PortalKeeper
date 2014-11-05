Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.Collections
Imports Google.GData.Spreadsheets
Imports System.Threading

Namespace Aricie.DNN.Modules.PortalKeeper



    <ActionButton(IconName.Google, IconOptions.Normal)> _
    <DisplayName("Google Spreadsheets")> _
    <Description("This provider allows to read and write google spreadsheets")> _
    <Serializable()> _
    Public Class GoogleSpreadSheetActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)
        'Implements ISelector

        <ExtendedCategory("Commands")> _
        Public Property Commands As New SerializableList(Of SpreadSheetCommand(Of TEngineEvents))

        <ExtendedCategory("Commands")> _
        Public Property Pace As New STimeSpan(TimeSpan.FromMilliseconds(100))

        <ExtendedCategory("CellFeed")> _
        Public Property UseExistingCellFeed As Boolean

        <ExtendedCategory("CellFeed")> _
        <ConditionalVisible("UseExistingCellFeed", False, True)> _
        Public Property WorksheetEntryExpression As New SimpleExpression(Of WorksheetEntry)()

        <ExtendedCategory("CellFeed")> _
        <ConditionalVisible("UseExistingCellFeed", False, True)> _
        Public Property CellFeedExpression As New SimpleExpression(Of CellFeed)()

        '<AutoPostBack()> _
        '<ExtendedCategory("CellFeed")> _
        ' <ConditionalVisible("UseExistingCellFeed", False, True)> _
        'Public Property CreateIf As New KeeperCondition(Of TEngineEvents)

        <AutoPostBack()> _
       <ExtendedCategory("CellFeed")> _
        <ConditionalVisible("UseExistingCellFeed", False, True)> _
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property ShowCellFeedInfo As Boolean
            Get
                Return (Not UseExistingCellFeed) OrElse CreateIfNull 'CreateIf.Instances.Count > 0
            End Get
        End Property

        <ExtendedCategory("CellFeed")> _
        Public Property CaptureWorksheetEntry As Boolean

        <ExtendedCategory("CellFeed")> _
        <ConditionalVisible("CaptureWorksheetEntry", False, True)> _
         Public Property WorksheetEntryName As String = "WorksheetEntry"

        <ExtendedCategory("CellFeed")> _
        <ConditionalVisible("ShowCellFeedInfo", False, True)> _
        Public Property CellFeedInfo As New CellFeedInfo(Of TEngineEvents)

       

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

            Dim toReturn As New SerializableDictionary(Of String, String)
            Dim objCellFeed As CellFeed = Nothing
            Dim objWorkSheetEntry As WorksheetEntry = Nothing
            If UseExistingCellFeed Then
                objCellFeed = CellFeedExpression.Evaluate(actionContext)
                objWorkSheetEntry = WorksheetEntryExpression.Evaluate(actionContext)
            End If
            If (Not UseExistingCellFeed) OrElse ((objCellFeed Is Nothing OrElse objWorkSheetEntry Is Nothing) AndAlso CreateIfNull) Then 'CreateIf.Match(actionContext)) Then
                Dim resultPair As KeyValuePair(Of WorksheetEntry, CellFeed) = Me.CellFeedInfo.GetWorkSheetAndCellFeed(actionContext)
                If objWorkSheetEntry Is Nothing Then
                    objWorkSheetEntry = resultPair.Key
                    If Me.CaptureWorksheetEntry Then
                        actionContext.Item(Me.WorksheetEntryName) = objWorkSheetEntry
                    End If
                End If
                If objCellFeed Is Nothing Then
                    objCellFeed = resultPair.Value
                End If
            End If

            Dim lastCommandTime As DateTime = DateTime.MinValue
            Dim objNow As DateTime
            Dim objService As SpreadsheetsService = DirectCast(objWorkSheetEntry.Service, SpreadsheetsService)


            For Each objCommand As SpreadSheetCommand(Of TEngineEvents) In Commands
                objNow = Now
                Dim nextSchedule As DateTime = lastCommandTime.Add(Me.Pace.Value)
                If nextSchedule > objNow Then
                    Thread.Sleep(nextSchedule.Subtract(objNow))
                End If
                objWorkSheetEntry = objCommand.Execute(objService, objWorkSheetEntry, objCellFeed, actionContext, toReturn)
                If Me.CaptureWorksheetEntry Then
                    actionContext.Item(Me.WorksheetEntryName) = objWorkSheetEntry
                End If
                lastCommandTime = Now
            Next

            Return toReturn
        End Function



        Protected Overrides Function GetOutputType() As Type
            Return GetType(SerializableDictionary(Of String, String))
        End Function

        'Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
        '    Select Case propertyName
        '        Case "SpreadSheetNameSelection"
        '            If Me.Initialized Then
        '                Return _SpreadSheets.Entries.Select(Function(objAtom) New ListItem(objAtom.Title.Text)).ToList()
        '            End If
        '        Case "WorkSheetNameSelection"
        '            If Me.Initialized Then
        '                If SpreadSheetName <> "" Then
        '                    If Not _SelectedSpreadSheet = SpreadSheetName Then
        '                        Dim sEntry As SpreadsheetEntry = DirectCast(_SpreadSheets.Entries.First(Function(objAtom) objAtom.Title.Text = SpreadSheetName), SpreadsheetEntry)
        '                        _WorkSheets = sEntry.Worksheets
        '                        _SelectedSpreadSheet = SpreadSheetName
        '                    End If
        '                    Return _WorkSheets.Entries.Select(Function(objAtom) New ListItem(objAtom.Title.Text)).ToList()
        '                Else
        '                    _WorkSheets = Nothing
        '                    _SelectedSpreadSheet = ""
        '                    Return New ListItemCollection()
        '                End If
        '            End If
        '    End Select
        '    Return Nothing
        'End Function

        Public Overrides Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            Me.CellFeedInfo.AddVariables(existingVars)

            If Me.CaptureWorksheetEntry Then
                existingVars.Add(Me.WorksheetEntryName, GetType(WorksheetEntry))
            End If

            MyBase.AddVariables(currentProvider, existingVars)
        End Sub

    End Class

End Namespace