Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports System.IO
Imports System.Linq
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public  Enum GetFilesMode
        FileNames
        FileEntries
    End Enum


    <ActionButton(IconName.HddO, IconOptions.Normal)>
    <DisplayName("File Manager Action")>
    <Description("This provider allows to browse or delete files and folders, given a parent path by dynamic expressions")>
    Public Class FileManagerActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileAccessActionProvider(Of TEngineEvents)


        <ExtendedCategory("File")>
        Public Property Mode As FileManagerMode


        <ConditionalVisible("Mode", False, True, FileManagerMode.GetFiles)>
        <ExtendedCategory("File")>
        Public Property GetFilesMode As GetFilesMode = GetFilesMode.FileNames

        <ExtendedCategory("File")>
        <ConditionalVisible("Mode", False, True, FileManagerMode.GetFiles, FileManagerMode.GetDirectories)>
        Public Property Pattern() As String = ""

        <SortOrder(1003)>
        <ExtendedCategory("File")>
        <ConditionalVisible("Mode", False, True, FileManagerMode.Copy, FileManagerMode.Move)>
        Public Property TargetPathExpression() As New FleeExpressionInfo(Of String)("")

        <ExtendedCategory("File")>
        <ConditionalVisible("Mode", False, True, FileManagerMode.Copy)>
        Public Property Overwrite As Boolean = True

        

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim sourceMapPath As String = Me.GetFileMapPath(actionContext)
            Select Case Mode
                Case FileManagerMode.GetFiles
                    If Directory.Exists(sourceMapPath) Then
                        Dim fileNames as String()
                        If Not String.IsNullOrEmpty(Me.Pattern) Then
                            fileNames = Directory.GetFiles(sourceMapPath, Pattern)
                        Else
                            fileNames = Directory.GetFiles(sourceMapPath)
                        End If
                        If GetFilesMode = GetFilesMode.FileNames Then
                            Return fileNames
                        Else
                            dim toReturn as new List(Of System.IO.FileInfo)(fileNames.Length)
                            toReturn.AddRange(From fileName In fileNames Select new System.IO.FileInfo(Path.Combine(sourceMapPath, fileName)))
                            Return toReturn
                        End If
                    Else
                         If GetFilesMode = GetFilesMode.FileNames Then
                           Return New String() {}
                        Else
                          Return New List(Of System.IO.FileInfo)
                        End If
                    End If
                Case FileManagerMode.GetDirectories
                    If Not String.IsNullOrEmpty(Me.Pattern) Then
                        Return Directory.GetDirectories(sourceMapPath, Pattern)
                    Else
                        Return Directory.GetDirectories(sourceMapPath)
                    End If
                Case FileManagerMode.Delete
                    If Directory.Exists(sourceMapPath) Then
                        Dim dir As New DirectoryInfo(sourceMapPath)
                        dir.Delete(True)
                    ElseIf File.Exists(sourceMapPath) Then
                        File.Delete(sourceMapPath)
                    Else
                        Return False
                    End If
                Case FileManagerMode.Copy
                    Dim targetPath As String = Me.TargetPathExpression.Evaluate(actionContext, actionContext)
                    Dim targetMapPath As String = Me.FilePath.GetMapPath(targetPath)
                    If Directory.Exists(sourceMapPath) Then
                        If Not Directory.Exists(targetMapPath) Then
                            Directory.CreateDirectory(targetMapPath)
                        End If
                        CopyDirectory(sourceMapPath, targetMapPath, Me.Overwrite)
                    ElseIf File.Exists(sourceMapPath) Then
                        File.Copy(sourceMapPath, targetMapPath, Me.Overwrite)
                    Else
                        Return False
                    End If
                Case FileManagerMode.Move
                    Dim targetPath As String = Me.TargetPathExpression.Evaluate(actionContext, actionContext)
                    Dim targetMapPath As String = Me.FilePath.GetMapPath(targetPath)
                    If Directory.Exists(sourceMapPath) OrElse File.Exists(sourceMapPath) Then
                        Directory.Move(sourceMapPath, targetMapPath)
                    Else
                        Return False
                    End If
            End Select
            Return True
        End Function

        Protected Overrides Function GetOutputType() As Type
            Select Case Mode
                Case FileManagerMode.GetFiles
                    if GetFilesMode = GetFilesMode.FileEntries Then
                        Return GetType(List(Of System.IO.FileInfo))
                    Else
                        Return GetType(String())
                    End If
                Case FileManagerMode.GetDirectories
                    Return GetType(String())
                Case FileManagerMode.Delete
                    Return GetType(Boolean)
                Case FileManagerMode.Copy
                    Return GetType(Boolean)
                Case FileManagerMode.Move
                    Return GetType(Boolean)
            End Select
            Return GetType(Boolean)
        End Function
    End Class
End Namespace