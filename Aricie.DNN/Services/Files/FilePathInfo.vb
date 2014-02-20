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
    Public Class FilePathInfo

        Public Property PathMode As FilePathMode

        Public Property ChooseDnnFile As Boolean

        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
        Public Property PortalId As Integer

        <ConditionalVisible("ChooseDnnFile", False, True)> _
        Public Property DnnFile As New ControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure Or UrlControlMode.Upload)

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Property Path As New SimpleOrExpression(Of String)("")


        Public Function GetFileMapPath(owner As Object, lookup As IContextLookup) As String
            Dim expressionPath As String = Me.Path.GetValue(owner, lookup)
            Return GetFileMapPath(expressionPath)
        End Function

        Public Function GetFileMapPath(expressionPath As String) As String
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
End Namespace