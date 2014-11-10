Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Google.GData.Spreadsheets
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports System.Linq

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class WorksheetEntryInfo(Of TEngineEvents As IConvertible)
        Implements ISelector

        Private _WorkSheets As WorksheetFeed
        Private _SelectedSpreadSheet As String = ""

        Public Property UseExistingSpreadSheetEntry As Boolean

        <ConditionalVisible("UseExistingSpreadSheetEntry", False, True)> _
        Public Property SpreadSheetEntryExpression As New SimpleExpression(Of SpreadsheetEntry)

        '<AutoPostBack()> _
        '<ConditionalVisible("UseExistingSpreadSheetEntry", False, True)> _
        'Public Property CreateIf As New KeeperCondition(Of TEngineEvents)

        <AutoPostBack()> _
      <ConditionalVisible("UseExistingSpreadSheetEntry", False, True)> _
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property ShowSpreadsheetEntryInfo As Boolean
            Get
                Return (Not UseExistingSpreadSheetEntry) OrElse CreateIfNull
            End Get
        End Property

       


        <Browsable(False)> _
        Public ReadOnly Property Initialized As Boolean
            Get
                Return Spreadsheet.Initialized
            End Get
        End Property

        Private _Worksheet As WorksheetEntry

        <ConditionalVisible("Initialized", True, True)> _
        Public Property WorkSheetName As String = ""

        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("Initialized", False, True)> _
        <Selector("Text", "Value", False, True, "<Select WorkSheet>", "", False, True)> _
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property WorkSheetNameSelection As String
            Get
                Return WorkSheetName
            End Get
            Set(value As String)
                WorkSheetName = value
            End Set
        End Property

        Private ReadOnly Property WorkSheet As WorksheetEntry
            Get
                If _Worksheet Is Nothing OrElse _Worksheet.Title.Text <> WorkSheetNameSelection Then
                    _Worksheet = DirectCast(_WorkSheets.Entries.First(Function(objAtom) objAtom.Title.Text = WorkSheetNameSelection), WorksheetEntry)
                End If
                Return _Worksheet
            End Get
        End Property



        <ConditionalVisible("WorkSheetNameSelection", True, True, "")> _
        <ConditionalVisible("Initialized", False, True)> _
        Public ReadOnly Property WorkSheetInfo As String
            Get
                If WorkSheet IsNot Nothing Then
                    Return String.Format("Rows: {0}, Columns: {1}, Last Updated: {2}", WorkSheet.Rows, WorkSheet.Cols, WorkSheet.Updated.ToShortDateString())
                End If
                Return ""
            End Get
        End Property


        Public Property CaptureWorksheetFeed As Boolean


        <ConditionalVisible("CaptureWorksheetFeed", False, True)> _
        Public Property WorksheetFeedName As String = "Worksheets"


        'Public Property CaptureWorksheetEntry As Boolean


        '<ConditionalVisible("CaptureWorksheetEntry", False, True)> _
        'Public Property WorksheetEntryName As String = "WorksheetEntry"

        <Browsable(False)> _
        Public ReadOnly Property ServiceReady As Boolean
            Get
                Return Spreadsheet.ServiceReady
            End Get
        End Property

        'Public Sub UpdateEntries(objWorksheet As WorksheetEntry, objSpreadsheet As SpreadsheetEntry, actionContext As PortalKeeperContext(Of TEngineEvents))
        '    If Me.CaptureWorksheetEntry Then
        '        actionContext.Item(Me.WorksheetEntryName) = objWorksheet
        '    End If
        'End Sub

        <ConditionalVisible("ShowSpreadsheetEntryInfo", False, True)> _
        Public Property Spreadsheet As New SpreadSheetEntryInfo(Of TEngineEvents)


        Public Function GetWorksheetEntry(actionContext As PortalKeeperContext(Of TEngineEvents)) As WorksheetEntry
            Dim toReturn As WorksheetEntry = Nothing
            Dim objSpreadSheetEntry As SpreadsheetEntry = Nothing
            If Me.UseExistingSpreadSheetEntry Then
                objSpreadSheetEntry = SpreadSheetEntryExpression.Evaluate(actionContext, actionContext)
            End If
            If (Not UseExistingSpreadSheetEntry) OrElse (objSpreadSheetEntry Is Nothing AndAlso CreateIfNull) Then
                objSpreadSheetEntry = Spreadsheet.GetSpreadSheetEntry(actionContext)
            End If
            If Me.CaptureWorksheetFeed Then
                actionContext.Item(Me.WorksheetFeedName) = objSpreadSheetEntry.Worksheets
            End If
            For Each objWorkSheetEntry As WorksheetEntry In objSpreadSheetEntry.Worksheets.Entries
                If objWorkSheetEntry.Title.Text = WorkSheetName Then
                    toReturn = objWorkSheetEntry
                    'If Me.CaptureWorksheetEntry Then
                    '    actionContext.Item(Me.WorksheetEntryName) = objWorkSheetEntry
                    'End If
                    Exit For
                End If
            Next
            Return toReturn
        End Function

        Public Sub AddVariables(ByRef existingVars As IDictionary(Of String, Type))

            Me.Spreadsheet.AddVariables(existingVars)

            If Me.CaptureWorksheetFeed Then
                existingVars(Me.WorksheetFeedName) = GetType(WorksheetFeed)
            End If
            'If Me.CaptureWorksheetEntry Then
            '    existingVars.Add(Me.WorksheetEntryName, GetType(WorksheetEntry))
            'End If

        End Sub

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "WorkSheetNameSelection"
                    If Me.Initialized Then
                        If Spreadsheet.SpreadSheetNameSelection <> "" Then
                            If Not _SelectedSpreadSheet = Spreadsheet.SpreadSheetNameSelection Then
                                'Dim sEntry As SpreadsheetEntry = DirectCast(Spreadsheet.SpreadSheets.Entries.First(Function(objAtom) objAtom.Title.Text = SpreadSheetName), SpreadsheetEntry)
                                _WorkSheets = Spreadsheet.SpreadSheet.Worksheets
                                _SelectedSpreadSheet = Spreadsheet.SpreadSheetNameSelection
                            End If
                            Return _WorkSheets.Entries.Select(Function(objAtom) New ListItem(objAtom.Title.Text)).ToList()
                        Else
                            _WorkSheets = Nothing
                            _SelectedSpreadSheet = ""
                            Return New ListItemCollection()
                        End If
                    End If
            End Select
            Return Nothing
        End Function

    End Class
End NameSpace