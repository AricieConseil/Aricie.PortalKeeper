Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class DumpSettings

        <DefaultValue(False)> _
        Public Property EnableDump As Boolean

        <DefaultValue(True)> _
        <ConditionalVisible("EnableDump", False, True)> _
        Public Property DumpAllVars() As Boolean = True

        <DefaultValue(True)> _
        <ConditionalVisible("EnableDump", False, True)> _
        Public Property SkipNull As Boolean = True

        
        <ConditionalVisible("SkipNull", True, True)> _
        <ConditionalVisible("EnableDump", False, True)> _
        Public Property DefaultValue As New SimpleExpression(Of Object)("""""")

        <ConditionalVisible("EnableDump", False, True)> _
        <ConditionalVisible("DumpAllVars", True, True)> _
        Public Property DumpVariables() As New CData("")

        <Browsable(False)> _
        Public ReadOnly Property HasExpressionBuilder As Boolean
            Get
                Return ExpressionBuilder IsNot Nothing
            End Get
        End Property

        <ConditionalVisible("EnableDump", False, True)> _
         <ConditionalVisible("DumpAllVars", True, True)> _
        <XmlIgnore()> _
        Public Overridable Property ExpressionBuilder As FleeExpressionBuilder

        <ConditionalVisible("EnableDump", False, True)> _
         <ConditionalVisible("DumpAllVars", True, True)> _
        <ConditionalVisible("HasExpressionBuilder", True, True)> _
        <ActionButton(IconName.Magic, IconOptions.Normal)> _
        Public Overridable Sub DisplayAvailableVars(ByVal pe As AriciePropertyEditorControl)

            'Dim fullAccess As Boolean = Me.InternalOverrideOwner AndAlso ((Me.InternalOwnerMemberAccess And Reflection.BindingFlags.NonPublic) = Reflection.BindingFlags.NonPublic)

            Me.ExpressionBuilder = FleeExpressionBuilder.GetExpressionBuilder(pe, False)
            pe.DisplayLocalizedMessage("ExpressionHelper.Message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            'pe.DisplayMessage(Me.ExpressionBuilder.GetType.AssemblyQualifiedName, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            pe.ItemChanged = True
        End Sub

        <ConditionalVisible("EnableDump", False, True)> _
         <ConditionalVisible("DumpAllVars", True, True)> _
        <ConditionalVisible("HasExpressionBuilder", False, True)> _
        <ActionButton(IconName.Undo, IconOptions.Normal)> _
        Public Overridable Sub RemoveExpressionBuilder(ByVal pe As AriciePropertyEditorControl)

            Me.ExpressionBuilder = Nothing
            pe.ItemChanged = True
        End Sub


        <ConditionalVisible("EnableDump", False, True)> _
        <ConditionalVisible("DumpAllVars", True, True)> _
       <ConditionalVisible("HasExpressionBuilder", False, True)> _
         <ActionButton(IconName.Clipboard, IconOptions.Normal)> _
        Public Overridable Sub InsertSelectedVar(ByVal pe As AriciePropertyEditorControl)
            Me.DumpVariables.Value &= Me.ExpressionBuilder.InsertString
            Me.ExpressionBuilder = Nothing
            pe.ItemChanged = True
        End Sub


       



        Private _dumpVarLock As New Object
        Private _DumpVarList As List(Of String)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property DumpVarList() As List(Of String)
            Get
                If _DumpVarList Is Nothing Then
                    SyncLock _dumpVarLock
                        If _DumpVarList Is Nothing Then
                            '_DumpVarList = New List(Of String)
                            'Dim strVars As String() = Me._DumpVariables.Split(","c)
                            'For Each strVar As String In strVars
                            '    If strVar.Trim <> "" Then
                            '        _DumpVarList.Add(strVar.Trim())
                            '    End If
                            'Next
                            _DumpVarList = Common.ParseStringList(Me._DumpVariables)
                        End If
                    End SyncLock
                End If
                Return _DumpVarList
            End Get
        End Property

    End Class
End Namespace