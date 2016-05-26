Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities
Imports System.Xml.Serialization
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
    Public Class SimpleFilePathInfo
        Inherits FilePathInfo

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property ChooseDnnFile As Boolean
            Get
                Return True
            End Get
            Set(value As Boolean)
                MyBase.ChooseDnnFile = value
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overrides Property PathMode As FilePathMode
            Get
                Return MyBase.PathMode
            End Get
            Set(value As FilePathMode)
                MyBase.PathMode = value
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overrides Property Path As SimpleOrExpression(Of String)
            Get
                Return MyBase.Path
            End Get
            Set(value As SimpleOrExpression(Of String))
                MyBase.Path = value
            End Set
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides ReadOnly Property CurrentMapPath As String
            Get
                Return MyBase.CurrentMapPath
            End Get
        End Property

    End Class


    <Serializable()>
    Public Class FilePathInfo
        Inherits PathInfo

        Private _ChooseDnnFile As Boolean

        <SortOrder(0)>
        Public Overridable Property ChooseDnnFile As Boolean
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
                                    Me.PortalId = objFile.PortalId
                                End If
                            End If
                        Else
                            Me.PathMode = FilePathMode.AdminPath
                            Me.PortalId = NukeHelper.PortalId
                        End If
                    Else
                        If Not Me.DnnFile.UrlPath.IsNullOrEmpty() Then
                            Me.PathMode = FilePathMode.AdminPath
                            Me.Path.Simple = GetAdminPathFromControlUrl(Me.DnnFile.UrlPath)
                            Me.DnnFile.Url = ""
                            Me.PortalId = NukeHelper.PortalId
                        End If
                    End If
                    _ChooseDnnFile = value
                End If
            End Set
        End Property


        Private Function GetAdminPathFromControlUrl(ctrUrlPath As String) As String
            Return ctrUrlPath.Replace(DnnContext.Current.Portal.HomeDirectory, "")
        End Function

        <ConditionalVisible("ChooseDnnFile", False, True)> _
        Public Property DnnFile As New SimpleControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure)


        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property DNNFileInfo As DotNetNuke.Services.FileSystem.FileInfo
            Get
                Dim toReturn As DotNetNuke.Services.FileSystem.FileInfo = Nothing
                If Me.ChooseDnnFile Then
                    toReturn = NukeHelper.GetFileInfoFromCtrUrl(Me.PortalId, Me.DnnFile.Url)
                Else
                    'todo
                    Throw New NotImplementedException()
                End If
                Return toReturn
            End Get
        End Property


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


        Public Overrides Function GetMapPath(owner As Object, lookup As IContextLookup) As String
            If Not ChooseDnnFile Then
                Return MyBase.GetMapPath(owner, lookup)
            Else
                Return Me.GetMapPath(GetAdminPathFromControlUrl(Me.DnnFile.UrlPath))
            End If
        End Function


    End Class
End Namespace