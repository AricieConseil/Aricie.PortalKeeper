Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.Collections
Imports System.Globalization
Imports Google.GData.Client
Imports Google.GData.Spreadsheets
Imports System.Linq
Imports Aricie.DNN.Entities

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum GoogleSpreadSheetMode
        CellCommands
        ListCommands
    End Enum

    Public Enum GoogleSpreadsheetListmode
        ReadList
        UpdateEntry
        InsertEntry
        DeleteEntry
    End Enum


    <ActionButton(IconName.Google, IconOptions.Normal)> _
    <DisplayName("Google Spreadsheets")> _
    <Description("This provider allows to read and write google spreadsheets")> _
    <Serializable()> _
    Public Class GoogleSpreadSheetActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)
        'Implements ISelector

        Public Property SpreadSheetMode As GoogleSpreadSheetMode

        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.CellCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property Commands As New SerializableList(Of SpreadSheetCommand(Of TEngineEvents))

        '<ConditionalVisible("UseExistingFeed", False, True)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.CellCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property CaptureWorksheetEntry As Boolean


        '<ConditionalVisible("UseExistingFeed", False, True)> _
        <ExtendedCategory("Commands")> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.CellCommands)> _
        <ConditionalVisible("CaptureWorksheetEntry", False, True)> _
        Public Property WorksheetEntryName As String = "WorksheetEntry"

        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property ListMode As GoogleSpreadsheetListmode = GoogleSpreadsheetListmode.ReadList

        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
     <ExtendedCategory("Commands")> _
        Public Property Query As New EnabledFeature(Of ListQueryInfo)


        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.UpdateEntry, GoogleSpreadsheetListmode.DeleteEntry)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property ListEntryExpression As New FleeExpressionInfo(Of ListEntry)

        
        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.InsertEntry, GoogleSpreadsheetListmode.UpdateEntry)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property InputDictionaryExpression As New FleeExpressionInfo(Of Dictionary(Of String, String))

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.UpdateEntry)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property UpdateIfChanged As Boolean

        <ConditionalVisible("UpdateIfChanged", False, True)> _
        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.UpdateEntry)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property ExcludedColumns As New List(Of String)


        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
       <ExtendedCategory("Commands")> _
        Public Property CaptureEntries As Boolean

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("CaptureEntries", False, True)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property EntriesVarName As String = "ListEntries"

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property IncludeIndex As Boolean

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("IncludeIndex", False, True)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
      <ExtendedCategory("Commands")> _
        Public Property IndexKey As String = "Index"

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property ReturnDictionary As Boolean

        <ConditionalVisible("ListMode", False, True, GoogleSpreadsheetListmode.ReadList)> _
        <ConditionalVisible("ReturnDictionary", False, True)> _
        <ConditionalVisible("SpreadSheetMode", False, True, GoogleSpreadSheetMode.ListCommands)> _
        <ExtendedCategory("Commands")> _
        Public Property PrimaryKey As New SimpleOrExpression(Of List(Of String))


        <ExtendedCategory("Commands")> _
        Public Property Pace As New STimeSpan(TimeSpan.FromMilliseconds(200))

        <ExtendedCategory("Feed")> _
        Public Property UseExistingFeed As Boolean

        <ExtendedCategory("Feed")> _
        <ConditionalVisible("UseExistingFeed", False, True)> _
        Public Property WorksheetEntryExpression As New SimpleExpression(Of WorksheetEntry)()




        <ExtendedCategory("Feed")> _
        <ConditionalVisible("UseExistingFeed", False, True)> _
        Public Property FeedExpression As New SimpleExpression(Of AbstractFeed)()

        '<AutoPostBack()> _
        '<ExtendedCategory("CellFeed")> _
        ' <ConditionalVisible("UseExistingFeed", False, True)> _
        'Public Property CreateIf As New KeeperCondition(Of TEngineEvents)

        <AutoPostBack()> _
       <ExtendedCategory("Feed")> _
        <ConditionalVisible("UseExistingFeed", False, True)> _
        Public Property CreateIfNull As Boolean

        <Browsable(False)> _
        Public ReadOnly Property ShowFeedInfo As Boolean
            Get
                Return (Not UseExistingFeed) OrElse CreateIfNull 'CreateIf.Instances.Count > 0
            End Get
        End Property

        <ExtendedCategory("Feed")> _
        <ConditionalVisible("ShowFeedInfo", False, True)> _
        Public Property FeedInfo As New SpreadsheetFeedInfo(Of TEngineEvents)



        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object

            Dim objFeed As AbstractFeed = Nothing
            Dim objWorkSheetEntry As WorksheetEntry = Nothing
            If UseExistingFeed Then
                objFeed = FeedExpression.Evaluate(actionContext)
                objWorkSheetEntry = WorksheetEntryExpression.Evaluate(actionContext)
            End If
            If (Not UseExistingFeed) OrElse ((objFeed Is Nothing OrElse objWorkSheetEntry Is Nothing) AndAlso CreateIfNull) Then 'CreateIf.Match(actionContext)) Then
                Dim resultPair As KeyValuePair(Of WorksheetEntry, AbstractFeed)
                If Me.Query.Enabled Then
                    resultPair = Me.FeedInfo.GetWorkSheetAndFeed(actionContext, Me.SpreadSheetMode, Me.Query.Entity)
                Else
                    resultPair = Me.FeedInfo.GetWorkSheetAndFeed(actionContext, Me.SpreadSheetMode, Nothing)
                End If

                If objWorkSheetEntry Is Nothing Then
                    objWorkSheetEntry = resultPair.Key
                    'If Me.CaptureWorksheetEntry Then
                    '    actionContext.Item(Me.WorksheetEntryName) = objWorkSheetEntry
                    'End If
                End If
                If objFeed Is Nothing Then
                    objFeed = resultPair.Value
                End If
            End If

            Dim lastCommandTime As DateTime = Now
            Dim objNow As DateTime
            Dim objService As SpreadsheetsService = DirectCast(objWorkSheetEntry.Service, SpreadsheetsService)
            Select Case Me.SpreadSheetMode
                Case GoogleSpreadSheetMode.CellCommands
                    Dim toReturn As New SerializableDictionary(Of String, String)

                    For Each objCommand As SpreadSheetCommand(Of TEngineEvents) In Commands
                        objNow = Now
                        Dim nextSchedule As DateTime = lastCommandTime.Add(Me.Pace.Value)
                        If nextSchedule > objNow Then
                            Me.Sleep(actionContext, nextSchedule.Subtract(objNow))
                        End If

                        Dim newWorkSheetEntry As WorksheetEntry = objCommand.Execute(objService, objWorkSheetEntry, DirectCast(objFeed, CellFeed), actionContext, toReturn)
                        If newWorkSheetEntry IsNot objWorkSheetEntry Then
                            objWorkSheetEntry = newWorkSheetEntry
                            If Me.CaptureWorksheetEntry Then
                                actionContext.Item(Me.WorksheetEntryName) = objWorkSheetEntry
                            End If
                        End If
                        lastCommandTime = Now
                    Next

                    Return toReturn
                Case Else


                    Dim toReturnList As New List(Of SerializableDictionary(Of String, String))
                    Dim toReturnDictionary As New SerializableDictionary(Of String, SerializableDictionary(Of String, String))(StringComparer.OrdinalIgnoreCase)
                    Dim toReturnEntriesList As New List(Of ListEntry)
                    Dim toReturnEntriesDico As New SerializableDictionary(Of String, ListEntry)

                    Select Case Me.ListMode
                        Case GoogleSpreadsheetListmode.ReadList
                            Dim index As Integer = 1
                            For Each objListEntry As ListEntry In objFeed.Entries
                                objNow = Now
                                Dim nextSchedule As DateTime = lastCommandTime.Add(Me.Pace.Value)
                                If nextSchedule > objNow Then
                                    Me.Sleep(actionContext, nextSchedule.Subtract(objNow))
                                End If
                                index += 1
                                Dim currentRow As New SerializableDictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                                Dim elements As ListEntry.CustomElementCollection = objListEntry.Elements
                                For Each element As ListEntry.Custom In elements
                                    currentRow(element.LocalName) = element.Value
                                Next
                                If IncludeIndex Then
                                    currentRow(Me.IndexKey) = index.ToString(CultureInfo.InvariantCulture)
                                End If
                                Console.WriteLine()
                                If Me.ReturnDictionary Then
                                    Dim tempKey As String = Me.PrimaryKey.GetValue(actionContext, actionContext).Aggregate("", Function(current, header) current & currentRow(header))
                                    toReturnDictionary(tempKey) = currentRow
                                    If Me.CaptureEntries Then
                                        toReturnEntriesDico(tempKey) = objListEntry
                                    End If

                                Else
                                    toReturnList.Add(currentRow)
                                    If Me.CaptureEntries Then
                                        toReturnEntriesList.Add(objListEntry)
                                    End If
                                End If
                                lastCommandTime = Now
                            Next
                        Case Else
                            Dim input As IDictionary(Of String, String)
                            Dim objListEntry As ListEntry
                            If Me.ListMode = GoogleSpreadsheetListmode.DeleteEntry Or Me.ListMode = GoogleSpreadsheetListmode.UpdateEntry Then
                                objListEntry = Me.ListEntryExpression.Evaluate(actionContext, actionContext)
                            Else
                                objListEntry = New ListEntry()
                            End If
                           
                            If Me.ListMode = GoogleSpreadsheetListmode.InsertEntry Or Me.ListMode = GoogleSpreadsheetListmode.UpdateEntry Then
                                input = Me.InputDictionaryExpression.Evaluate(actionContext, actionContext)
                                Dim changed As Boolean
                                If Not UpdateIfChanged Then
                                    changed = True
                                End If
                                For Each objPair As KeyValuePair(Of String, String) In input
                                    Dim objCustom As ListEntry.Custom
                                    If Me.ListMode = GoogleSpreadsheetListmode.UpdateEntry Then
                                        For Each objCustom In objListEntry.Elements
                                            If objCustom.LocalName.ToUpperInvariant = objPair.Key.ToUpperInvariant Then
                                                If UpdateIfChanged Then
                                                    If objCustom.Value <> objPair.Value AndAlso Not Me.ExcludedColumns.Any(Function(objStr) objStr.ToUpperInvariant = objCustom.LocalName.ToUpperInvariant) Then
                                                        changed = True
                                                    End If
                                                End If
                                                objCustom.Value = objPair.Value
                                                Exit For
                                            End If
                                        Next
                                    Else
                                        objCustom = New ListEntry.Custom()
                                        objCustom.LocalName = objPair.Key.ToLower()
                                        objCustom.Value = objPair.Value
                                        objListEntry.Elements.Add(objCustom)
                                    End If
                                Next
                                If Me.ListMode = GoogleSpreadsheetListmode.InsertEntry Then
                                    objService.Insert(objFeed, objListEntry)
                                ElseIf changed OrElse (Not Me.UpdateIfChanged) Then
                                    objListEntry.Update()
                                End If
                            ElseIf Me.ListMode = GoogleSpreadsheetListmode.DeleteEntry Then
                                objService.Delete(objListEntry)
                            End If
                            Return True
                    End Select
                    If Me.ReturnDictionary Then
                        If Me.CaptureEntries Then
                            actionContext.SetVar(Me.EntriesVarName, toReturnEntriesDico)
                        End If
                        Return toReturnDictionary
                    Else
                        If Me.CaptureEntries Then
                            actionContext.SetVar(Me.EntriesVarName, toReturnEntriesList)
                        End If
                        Return toReturnList
                    End If
            End Select
        End Function



        Protected Overrides Function GetOutputType() As Type
            Select Case Me.SpreadSheetMode
                Case GoogleSpreadSheetMode.CellCommands
                    Return GetType(SerializableDictionary(Of String, String))
                Case Else
                    If Me.ReturnDictionary Then
                        Return GetType(SerializableDictionary(Of String, SerializableDictionary(Of String, String)))
                    Else
                        Select Case Me.ListMode
                            Case GoogleSpreadsheetListmode.ReadList
                                Return GetType(List(Of SerializableDictionary(Of String, String)))
                            Case GoogleSpreadsheetListmode.InsertEntry, GoogleSpreadsheetListmode.UpdateEntry
                                Return GetType(ListEntry)
                            Case Else
                                Return GetType(Boolean)
                        End Select
                    End If
            End Select
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
            Me.FeedInfo.AddVariables(existingVars)
            If Me.CaptureEntries Then
                Dim targetType As Type
                If Me.ReturnDictionary Then
                    targetType = GetType(SerializableDictionary(Of String, ListEntry))
                Else
                    targetType = GetType(List(Of ListEntry))
                End If
                existingVars(Me.EntriesVarName) = targetType
            End If
            'If Me.CaptureWorksheetEntry Then
            '    existingVars(Me.WorksheetEntryName) = GetType(WorksheetEntry)
            'End If

            MyBase.AddVariables(currentProvider, existingVars)
        End Sub

    End Class

End Namespace