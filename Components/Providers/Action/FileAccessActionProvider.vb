Imports Aricie.DNN.UI.Attributes
Imports Aricie.Text
Imports System.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.ComponentModel
Imports System.IO

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum FilePathMode
        AbsoluteMapPath
        RootPath
        HostPath
        AdminPath
    End Enum

    Public Enum FileAccessMode
        StringReadWrite
        FileHelperCSV
    End Enum

   


    Public Enum FileManagerMode
        GetFiles
        GetDirectories
        Delete
        Copy
        Move
    End Enum

    <DisplayName("File Manager Action")> _
       <Description("This provider allows to browse or delete files and folders, given a parent path by dynamic expressions")> _
   <Serializable()> _
    Public Class FileManagerActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileAccessActionProvider(Of TEngineEvents)

        <ExtendedCategory("File")> _
        Public Property Mode As FileManagerMode

        <ExtendedCategory("File")> _
        <ConditionalVisible("Mode", False, True, FileManagerMode.GetFiles, FileManagerMode.GetDirectories)> _
        Public Property Pattern() As String = ""

        <SortOrder(1003)> _
        <ExtendedCategory("File")> _
        <ConditionalVisible("Mode", False, True, FileManagerMode.Copy, FileManagerMode.Move)> _
        Public Property TargetPathExpression() As New FleeExpressionInfo(Of String)("")

        <ExtendedCategory("File")> _
        <ConditionalVisible("Mode", False, True, FileManagerMode.Copy)> _
        Public Property Overwrite As Boolean = True

        Public Overrides Function BuildResult(actionContext As PortalKeeperContext(Of TEngineEvents), async As Boolean) As Object
            Dim sourceMapPath As String = Me.GetFileMapPath(actionContext)
            Select Case Mode
                Case FileManagerMode.GetFiles
                    If Not String.IsNullOrEmpty(Me.Pattern) Then
                        Return System.IO.Directory.GetFiles(sourceMapPath, Pattern)
                    Else
                        Return System.IO.Directory.GetFiles(sourceMapPath)
                    End If
                Case FileManagerMode.GetDirectories
                    If Not String.IsNullOrEmpty(Me.Pattern) Then
                        Return System.IO.Directory.GetDirectories(sourceMapPath, Pattern)
                    Else
                        Return System.IO.Directory.GetDirectories(sourceMapPath)
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
                    Dim targetMapPath As String = Me.GetFileMapPath(actionContext, targetPath)
                    If Directory.Exists(sourceMapPath) Then
                        If Not Directory.Exists(targetMapPath) Then
                            Directory.CreateDirectory(targetMapPath)
                        End If
                        Aricie.Common.CopyDirectory(sourceMapPath, targetMapPath, Me.Overwrite)
                    ElseIf File.Exists(sourceMapPath) Then
                        File.Copy(sourceMapPath, targetMapPath, Me.Overwrite)
                    Else
                        Return False
                    End If
                Case FileManagerMode.Move
                    Dim targetPath As String = Me.TargetPathExpression.Evaluate(actionContext, actionContext)
                    Dim targetMapPath As String = Me.GetFileMapPath(actionContext, targetPath)
                    If Directory.Exists(sourceMapPath) OrElse File.Exists(sourceMapPath) Then
                        System.IO.Directory.Move(sourceMapPath, targetMapPath)
                    Else
                        Return False
                    End If
            End Select
            Return True
        End Function
    End Class


    <Serializable()> _
    Public MustInherit Class FileReadWriteActionProvider(Of TEngineEvents As IConvertible)
        Inherits FileAccessActionProvider(Of TEngineEvents)

        'Private _AccessMode As FileAccessMode

        'Private _EntityType As New DotNetType
        Private _Encoding As SimpleEncoding

        Private _UseCompression As Boolean

        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
       <ExtendedCategory("File")> _
        Public Property Encoding() As SimpleEncoding
            Get
                Return _Encoding
            End Get
            Set(ByVal value As SimpleEncoding)
                _Encoding = value
            End Set
        End Property

        <ConditionalVisible("AccessMode", False, True, FileAccessMode.StringReadWrite)> _
        <ExtendedCategory("File")> _
        Public Property UseCompression() As Boolean
            Get
                Return _UseCompression
            End Get
            Set(ByVal value As Boolean)
                _UseCompression = value
            End Set
        End Property

        '<ExtendedCategory("File")> _
        'Public Property AccessMode() As FileAccessMode
        '    Get
        '        Return _AccessMode
        '    End Get
        '    Set(ByVal value As FileAccessMode)
        '        _AccessMode = value
        '    End Set
        'End Property




        '<ConditionalVisible("AccessMode", False, True, FileAccessMode.FileHelperCSV)> _
        '<ExtendedCategory("File")> _
        'Public Property EntityType() As DotNetType
        '    Get
        '        Return _EntityType
        '    End Get
        '    Set(ByVal value As DotNetType)
        '        _EntityType = value
        '    End Set
        'End Property

    End Class

    <Serializable()> _
    Public MustInherit Class FileAccessActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        Protected Shared RWLock As New ReaderWriterLock
        Protected Shared LockTimeSpan As TimeSpan = TimeSpan.FromSeconds(5)





        Private _PathExpression As New FleeExpressionInfo(Of String)


        Private _PathMode As FilePathMode
        Private _PortalId As Integer




        <SortOrder(1000)> _
        <ExtendedCategory("File")> _
        Public Property PathMode() As FilePathMode
            Get
                Return _PathMode
            End Get
            Set(ByVal value As FilePathMode)
                _PathMode = value
            End Set
        End Property

        <SortOrder(1001)> _
        <ExtendedCategory("File")> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property


        <SortOrder(1002)> _
        <ExtendedCategory("File")> _
        Public Property PathExpression() As FleeExpressionInfo(Of String)
            Get
                Return _PathExpression
            End Get
            Set(ByVal value As FleeExpressionInfo(Of String))
                _PathExpression = value
            End Set
        End Property




        Protected Function GetFileMapPath(actionContext As PortalKeeperContext(Of TEngineEvents)) As String
            Dim expressionPath As String = Me._PathExpression.Evaluate(actionContext, actionContext)
            Return GetFileMapPath(actionContext, expressionPath)
        End Function

        Protected Function GetFileMapPath(actionContext As PortalKeeperContext(Of TEngineEvents), expressionPath As String) As String
            Dim toReturn As String = expressionPath
            Select Case Me._PathMode
                Case FilePathMode.RootPath
                    toReturn = DotNetNuke.Common.Globals.ApplicationMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.HostPath
                    toReturn = DotNetNuke.Common.Globals.HostMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.AdminPath
                    toReturn = NukeHelper.PortalInfo(Me._PortalId).HomeDirectoryMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
            End Select
            Return toReturn
        End Function


    End Class
End Namespace