Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel
Imports System.Web.Configuration
Imports System.Security.Cryptography
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities
Imports System.Xml.Serialization
Imports System.IO
Imports System.Xml
Imports System.Text

Namespace Services.Files

    <Serializable()>
    Public Class PathInfo

        <SortOrder(0)> _
        Public Property PathMode As FilePathMode = FilePathMode.AdminPath

        <SortOrder(1)> _
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
        Public Property PortalId As Integer

        Public Overridable Property Path As New SimpleOrExpression(Of String)("")

        Public Overloads Function GetMapPath() As String
            Return GetMapPath(DnnContext.Current, DnnContext.Current)
        End Function


        Public Overloads Function GetMapPath(owner As Object, lookup As IContextLookup) As String
            Dim expressionPath As String = Me.Path.GetValue(owner, lookup)
            Return GetMapPath(expressionPath)
        End Function

        Public Overloads Function GetMapPath(expressionPath As String) As String
            If expressionPath Is Nothing Then
                Throw New ApplicationException(String.Format("path cannot be null"))
            End If
            Dim toReturn As String = expressionPath
            Select Case Me.PathMode
                Case FilePathMode.RootPath
                    toReturn = DotNetNuke.Common.Globals.ApplicationMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.HostPath
                    toReturn = DotNetNuke.Common.Globals.HostMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
                Case FilePathMode.AdminPath
                    toReturn = NukeHelper.PortalInfo(Me.PortalId).HomeDirectoryMapPath.TrimEnd("\"c) & ("\"c) & expressionPath.Replace("/"c, "\"c).TrimStart("\"c)
            End Select
            Return toReturn
        End Function

    End Class

    <Serializable()>
    Public Class FolderPathInfo
        Inherits PathInfo

    End Class

    <Serializable()>
    Public Class FilePathInfo
        Inherits PathInfo

        <SortOrder(2)> _
        Public Property ChooseDnnFile As Boolean


        <ConditionalVisible("ChooseDnnFile", False, True)> _
        Public Property DnnFile As New ControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure Or UrlControlMode.Upload)

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property Path As New SimpleOrExpression(Of String)("")



    End Class
End Namespace