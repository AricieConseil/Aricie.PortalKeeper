Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.Web.Configuration
Imports System.Security.Cryptography
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities
Imports System.Xml.Serialization
Imports System.IO
Imports System.Xml
Imports System.Text
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.Services.FileSystem
Imports System.Globalization

Namespace Services.Files
    <Serializable()>
    Public Class FolderPathInfo
        Inherits PathInfo

    End Class

    <Serializable()>
    Public Class FilePathInfo
        Inherits PathInfo

        Private _ChooseDnnFile As Boolean

        <SortOrder(0)>
        Public Property ChooseDnnFile As Boolean
            Get
                Return _ChooseDnnFile
            End Get
            Set(value As Boolean)
                If value <> _ChooseDnnFile Then
                    If value Then
                        If Me.PathMode = FilePathMode.AdminPath AndAlso Not Me.Path.Simple.IsNullOrEmpty() Then
                            Dim strFolder As String = System.IO.Path.GetDirectoryName(Me.Path.Simple)
                            Dim objFolder As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(NukeHelper.PortalId, strFolder)
                            If objFolder IsNot Nothing Then
                                Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = ObsoleteDNNProvider.Instance.GetFile(objFolder, System.IO.Path.GetFileName(Me.Path.Simple))
                                If objFile IsNot Nothing Then
                                    Me.DnnFile.Url = "FileID=" & objFile.FileId.ToString(CultureInfo.InvariantCulture)
                                    Me.Path.Simple = ""
                                End If
                            End If
                        End If
                    Else
                        If Not Me.DnnFile.UrlPath.IsNullOrEmpty() Then
                            Me.PathMode = FilePathMode.AdminPath
                            Me.Path.Simple = Me.DnnFile.UrlPath
                            Me.DnnFile.Url = ""
                        End If
                    End If
                    _ChooseDnnFile = value
                End If
            End Set
        End Property

        <ConditionalVisible("ChooseDnnFile", False, True)> _
        Public Property DnnFile As New ControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure)

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property PathMode As FilePathMode
            Get
                Return MyBase.PathMode
            End Get
            Set(value As FilePathMode)
                MyBase.PathMode = value
            End Set
        End Property

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
         Public Overrides Property PortalId As Integer
            Get
                Return MyBase.PortalId
            End Get
            Set(value As Integer)
                MyBase.PortalId = value
            End Set
        End Property

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property Path As SimpleOrExpression(Of String)
            Get
                Return MyBase.Path
            End Get
            Set(value As SimpleOrExpression(Of String))
                MyBase.Path = value
            End Set
        End Property





    End Class
End Namespace