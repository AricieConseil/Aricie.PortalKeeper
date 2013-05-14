Imports System.Web.UI

Imports DotNetNuke.Services.FileSystem

Imports Aricie.Collections
Imports Aricie.DNN.Services
Imports System.IO
Imports System.Web.UI.WebControls

Namespace UI.WebControls
    ''' <summary>
    ''' Permet la selection multiple de dossier
    ''' </summary>
    ''' <remarks>DataTexField=FolderName et DataValueField=FolderId sont requis pour avoir un comportement par defaut</remarks>
    Public Class FolderSelector
        Inherits MultiSelectorControl(Of FolderInfo)

        Private _PortalId As Integer = -1

        Public Property PortalId() As Integer
            Get
                If _PortalId = -1 Then
                    Me._PortalId = NukeHelper.PortalId
                End If
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property

        Public Overrides Function GetEntitiesG() As IList(Of FolderInfo)
            Dim toReturn As New List(Of FolderInfo)
            If String.IsNullOrEmpty(RootFolder) Then
                For Each folder As FolderInfo In NukeHelper.FolderController.GetFolders(PortalSettings.PortalId).Values
                    toReturn.Add(folder)
                Next
            Else
                toReturn = (From myFolder In NukeHelper.FolderController.GetFolders(PortalSettings.PortalId).Values Where myFolder.FolderPath.ToLowerInvariant.Contains(RootFolder.ToLowerInvariant)).ToList
            End If
            
            Return toReturn
        End Function



        Private _RootFolder As String = String.Empty
        ''' <summary>
        ''' Dossier sur lequel filtrer, ex: myFolder/mySubFolder
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RootFolder() As String
            Get
                Return _RootFolder
            End Get
            Set(ByVal value As String)
                _RootFolder = value
            End Set
        End Property

    End Class

End Namespace
