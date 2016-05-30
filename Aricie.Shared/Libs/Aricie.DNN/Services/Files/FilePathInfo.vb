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
Imports System.Web
Imports Newtonsoft.Json

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
               'do nothing
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

        Private _dnnFile As  SimpleControlUrlInfo

        <SortOrder(0)>
        Public Overridable Property ChooseDnnFile As Boolean
           


        Private Function GetAdminPathFromControlUrl(byval ctrUrlPath As String) As String
            dim toReturn  as String = ctrUrlPath
            toReturn = toReturn.Substring(DotNetNuke.Common.ApplicationPath.Length).TrimStart("/"c)
            Dim portalHome As String = NukeHelper.PortalInfo(Me.PortalId).HomeDirectory
            toReturn = toReturn.Substring(portalHome.Length).TrimStart("/"c)
            Return toReturn
        End Function

        <ConditionalVisible("ChooseDnnFile", False, True)>
        Public Property DnnFile As SimpleControlUrlInfo

            Get
                If ChooseDnnFile Then
                    If _dnnFile Is Nothing Then
                        _dnnFile = New SimpleControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure)
                        Dim found As Boolean
                        If Me.PathMode = FilePathMode.AdminPath AndAlso Not Me.Path.Simple.IsNullOrEmpty() Then
                            Dim strFolder As String = System.IO.Path.GetDirectoryName(Me.Path.Simple)
                            Dim objFolder As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(NukeHelper.PortalId, strFolder)
                            If objFolder IsNot Nothing Then
                                Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = ObsoleteDNNProvider.Instance.GetFile(objFolder, System.IO.Path.GetFileName(Me.Path.Simple))
                                If objFile IsNot Nothing Then
                                    Me._dnnFile.Url = "FileID=" & objFile.FileId.ToString(CultureInfo.InvariantCulture)
                                    Me.Path.Simple = ""
                                    Me.PortalId = objFile.PortalId
                                    found = True
                                End If
                            End If
                        End If
                        If Not found Then
                            Me.PathMode = FilePathMode.AdminPath
                            Me.PortalId = NukeHelper.PortalId
                        End If
                    End If
                End If
                Return _dnnFile
            End Get
            Set
                _dnnFile = Value
            End Set
        End Property

        Public Function ShouldSerializeDnnFile() As Boolean
            Return ChooseDnnFile
        End Function



        <JsonIgnore()> _
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

        <ConditionalVisible("ChooseDnnFile", True, True)>
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)>
        <Editor(GetType(SelectorEditControl), GetType(EditControl))>
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
        Public Overrides Property PortalId As Integer
            Get
                Return MyBase.PortalId
            End Get
            Set(value As Integer)
                MyBase.PortalId = value
            End Set
        End Property


        Public Overrides Function ShouldSerializePortalId() As Boolean
            If PathMode = FilePathMode.AdminPath Then
                If ChooseDnnFile Then
                    Return DnnFile IsNot Nothing AndAlso Not DnnFile.Url.IsNullOrEmpty()
                Else
                    If Path.Mode = SimpleOrExpressionMode.Expression Then
                        Return Not Path.Expression.Expression.IsNullOrEmpty()
                    Else
                        Return Not Path.Simple.IsNullOrEmpty()
                    End If
                End If
            End If
            Return False
        End Function

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property Path As SimpleOrExpression(Of String)
            Get
                If Not ChooseDnnFile AndAlso _dnnFile IsNot Nothing Then
                    If Not Me._dnnFile.Url.IsNullOrEmpty() Then
                        Me.PathMode = FilePathMode.AdminPath
                        MyBase.Path.Simple = GetAdminPathFromControlUrl(Me._dnnFile.UrlPath)
                        Me.PortalId = _dnnFile.PortalId
                    End If
                    _dnnFile = Nothing
                End If
                Return MyBase.Path
            End Get
            Set(value As SimpleOrExpression(Of String))
                MyBase.Path = value
            End Set
        End Property


        Public Function ShouldSerializePath() As Boolean
            Return Not ChooseDnnFile
        End Function


        Public Overrides Function GetMapPath(owner As Object, lookup As IContextLookup) As String
            If Not ChooseDnnFile Then
                Return MyBase.GetMapPath(owner, lookup)
            Else
                Dim toReturn As String = Me.DnnFile.UrlPath
                If Not toReturn.IsNullOrEmpty() Then
                    toReturn = GetAdminPathFromControlUrl(toReturn)
                End If
                Return Me.GetMapPath(toReturn)
            End If
        End Function


    End Class
End Namespace