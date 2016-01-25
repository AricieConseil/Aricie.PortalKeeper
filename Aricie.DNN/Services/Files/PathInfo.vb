Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee

Namespace Services.Files

    <DefaultProperty("CurrentMapPath")> _
    <Serializable()>
    Public Class PathInfo


        Public Overridable Property PathMode As FilePathMode = FilePathMode.AdminPath

        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
        Public Overridable Property PortalId As Integer

        Public Overridable Property Path As New SimpleOrExpression(Of String)("")

        <XmlIgnore()> _
        Public Overridable ReadOnly Property CurrentMapPath As String
            Get
                Return GetMapPath()
            End Get
        End Property
        Public Overloads Function GetMapPath() As String
            Return GetMapPath(DnnContext.Current, DnnContext.Current)
        End Function


        Public Overridable Overloads Function GetMapPath(owner As Object, lookup As IContextLookup) As String
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
End NameSpace