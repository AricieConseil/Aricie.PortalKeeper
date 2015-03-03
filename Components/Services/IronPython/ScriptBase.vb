Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Files
Imports Aricie.ComponentModel
Imports System.ComponentModel
Imports Aricie.Collections
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class ScriptBase


        Public Property Mode As ScriptMode = ScriptMode.Script


        <ConditionalVisible("Mode", False, True, ScriptMode.ScriptFile)> _
        Public Property File As New SimpleFilePathInfo()


        <AutoPostBack()> _
        <Required(True)> _
        <ConditionalVisible("Mode", False, True, ScriptMode.Script)> _
        Public Overridable Property Script As New CData()


        <AutoPostBack()> _
        <Required(True)> _
        <Width(500)> _
        <ConditionalVisible("Mode", False, True, ScriptMode.Command)> _
        Public Overridable Property Command As String

        Private ReadOnly Property HasOutput As Boolean
            Get
                Return Not Output.Value.Trim().IsNullOrEmpty()
            End Get
        End Property

        

        <ExtendedCategory("Test")> _
        <XmlIgnore()> _
        <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
       <ConditionalVisible("HasOutput", False, True)> _
       <LineCount(3)> _
       <Width(600)> _
       <IsReadOnly(True)> _
        Public Overridable Property Output As New CData()


        <ExtendedCategory("Parameters")> _
        Public Property Parameters As New Variables()


        Public Function Run(lookup As IContextLookup) As String
            Return Run(Me.GetInput(), lookup)
        End Function

        Public MustOverride Function Run(ByVal input As String, lookup As IContextLookup) As String

       

        <ExtendedCategory("Test")> _
        <ActionButton(IconName.Code, IconOptions.Normal)> _
        Public Sub Run(ByVal ape As AriciePropertyEditorControl)
            ape.Page.Validate()
            If ape.IsValid Then
                Dim sb As New StringBuilder(Me.Output)
                sb.AppendLine()
                Dim newOutput As String
                Try
                    Dim input As String = GetInput()
                    Dim lookup As New SimpleContextLookup()
                    If Me.Parameters.Instances.Count > 0 Then
                        Dim tempContext As New PortalKeeperContext(Of ScheduleEvent)
                        lookup.Items = Me.Parameters.EvaluateVariables(tempContext, tempContext)
                    End If
                    newOutput = Me.Run(input, lookup)
                    sb.Append(newOutput)
                    Me.Output = sb.ToString()
                    ape.ItemChanged = True
                Catch ex As Exception
                    ape.DisplayMessage(Localization.GetString("ScriptRunFailed.Message", ape.LocalResourceFile).Replace("[Exception]", ex.ToString()), ModuleMessage.ModuleMessageType.YellowWarning)
                End Try
            End If
        End Sub

        <ExtendedCategory("Test")> _
        <ActionButton(IconName.Trash, IconOptions.Normal)> _
        Public Sub ClearOutput(ByVal ape As AriciePropertyEditorControl)
            Me.Output.Value = ""
            ape.ItemChanged = True
        End Sub

        'todo: add context for dynamic expressions
        Public Function GetInput() As String
            Select Case Me.Mode
                Case ScriptMode.ScriptFile
                    Dim objFileInfo As FileInfo = Me.File.DNNFileInfo
                    Return Aricie.DNN.Services.ObsoleteDNNProvider.Instance.GetFileContent(objFileInfo).FromUTF8()
                Case ScriptMode.Command
                    Return Me.Command
                Case Else
                    Return Me.Script
            End Select
        End Function


    End Class
End Namespace