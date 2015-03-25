Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports Google.GData.Spreadsheets
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls
Imports System.Linq

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class SpreadSheetEntryInfo(Of TEngineEvents As IConvertible)
        Implements ISelector


        Private _TempServiceInfo As GDataServiceInfo(Of SpreadsheetsService, TEngineEvents)
        Private _SpreadSheets As SpreadsheetFeed


        <ExtendedCategory("Service")> _
        Public Property Service As New SimpleOrExpression(Of GDataServiceInfo(Of SpreadsheetsService, TEngineEvents))( _
            New GDataServiceInfo(Of SpreadsheetsService, TEngineEvents)("https://spreadsheets.google.com/feeds https://docs.google.com/feeds"))

        <ExtendedCategory("SpreadSheet")> _
        Public ReadOnly Property ServiceReady As Boolean
            Get
                Dim objService = Me.Service.GetValue()
                If objService IsNot Nothing Then
                    Return objService.Authorized
                End If
                Return False
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property Initialized As Boolean
            Get
                Return _TempServiceInfo IsNot Nothing
            End Get
        End Property


        <ExtendedCategory("SpreadSheet")> _
        <ConditionalVisible("Initialized", True, True)> _
        Public Property SpreadSheetName As String = ""

        <ExtendedCategory("SpreadSheet")> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("Initialized", False, True)> _
        <Selector("Text", "Value", False, True, "<Select SpreadSheet>", "", False, True)> _
        <AutoPostBack> _
        <XmlIgnore()> _
        Public Property SpreadSheetNameSelection As String
            Get
                Return SpreadSheetName
            End Get
            Set(value As String)
                SpreadSheetName = value
            End Set
        End Property

        <ExtendedCategory("SpreadSheet")> _
        Public Property CaptureSpreadsheetFeed As Boolean

        <ExtendedCategory("SpreadSheet")> _
        <ConditionalVisible("CaptureSpreadsheetFeed", False, True)> _
        Public Property SpreadSheetFeedName As String = "SpreadSheets"

        <ExtendedCategory("SpreadSheet")> _
        Public Property CaptureSpreadsheetEntry As Boolean


        <ExtendedCategory("SpreadSheet")> _
        <ConditionalVisible("CaptureSpreadsheetEntry", False, True)> _
        Public Property SpreadsheetEntryName As String = "SpreadsheetEntry"

        <Browsable(False)> _
        Public ReadOnly Property SpreadSheet As SpreadsheetEntry
            Get
                If _SpreadSheets IsNot Nothing Then
                    Return DirectCast(_SpreadSheets.Entries.First(Function(objAtom) objAtom.Title.Text = SpreadSheetName), SpreadsheetEntry)
                End If
                Return Nothing
            End Get
        End Property


       




        <ExtendedCategory("SpreadSheet")> _
        <ConditionalVisible("ServiceReady", False, True)> _
        <ActionButton(IconName.Certificate, IconOptions.Normal)> _
        Public Sub Initialize(ByVal pe As AriciePropertyEditorControl, actionContext As PortalKeeperContext(Of TEngineEvents))
            Me._TempServiceInfo = GetService(actionContext)
            Dim sheetQuery As New SpreadsheetQuery()
            Try
                _SpreadSheets = Me._TempServiceInfo.Service.Query(sheetQuery)
            Catch
                _TempServiceInfo = Nothing
                Throw
            End Try
            pe.DisplayLocalizedMessage("SpreadsheetServiceInitialized.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
            If pe.ParentAricieEditor IsNot Nothing Then
                pe.ParentAricieEditor.ItemChanged = True
            End If
        End Sub

        Private Function GetService(actionContext As PortalKeeperContext(Of TEngineEvents)) As GDataServiceInfo(Of SpreadsheetsService, TEngineEvents)
            Dim objServiceInfo As GDataServiceInfo(Of SpreadsheetsService, TEngineEvents) = Me.Service.GetValue(actionContext, actionContext)
            If objServiceInfo.Service Is Nothing Then
                objServiceInfo.Init(actionContext)
            End If
            Return objServiceInfo
        End Function


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "SpreadSheetNameSelection"
                    If Me.Initialized Then
                        Return _SpreadSheets.Entries.Select(Function(objAtom) New ListItem(objAtom.Title.Text)).ToList()
                    End If
            End Select
            Return Nothing
        End Function


        Public Function GetSpreadSheetEntry(actionContext As PortalKeeperContext(Of TEngineEvents)) As SpreadsheetEntry
            Dim toReturn As SpreadsheetEntry = Nothing
            Dim objServiceInfo As GDataServiceInfo(Of SpreadsheetsService, TEngineEvents) = GetService(actionContext)
            Dim sheetQuery As New SpreadsheetQuery()
            Dim objSpreadSheetFeed As SpreadsheetFeed = objServiceInfo.Service.Query(sheetQuery)
            If Me.CaptureSpreadsheetFeed Then
                actionContext.Item(Me.SpreadSheetFeedName) = objSpreadSheetFeed
            End If
            For Each objSpreadSheetEntry As SpreadsheetEntry In objSpreadSheetFeed.Entries
                If objSpreadSheetEntry.Title.Text = SpreadSheetName Then
                    toReturn = objSpreadSheetEntry
                    If Me.CaptureSpreadsheetEntry Then
                        actionContext.Item(Me.SpreadsheetEntryName) = objSpreadSheetEntry
                    End If
                    Exit For
                End If
            Next
            Return toReturn
        End Function


        Public Sub AddVariables(ByRef existingVars As IDictionary(Of String, Type))
            If Me.CaptureSpreadsheetFeed Then
                existingVars(Me.SpreadSheetFeedName) = GetType(SpreadsheetFeed)
            End If

        End Sub

    End Class
End NameSpace