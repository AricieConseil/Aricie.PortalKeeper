Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.Services.FileSystem
Imports System.Xml.Serialization

Namespace Services.Files
    <Serializable()> _
    Public Class SmartFolder(Of T As New)
        Implements ISelector(Of FileInfo)


        <Browsable(False)> _
        Public Overridable Property FolderPath As New FolderPathInfo()


        <Selector("FileName", "FileId", False, True, "Select a file", "", False, True)>
        Public Property SelectedFile As String = ""


        Private _SmartFile As SmartFile(Of T)

        <XmlIgnore()> _
        Public Property CurrentFile As SmartFile(Of T)
            Get
                If _SmartFile Is Nothing Then
                    If Not String.IsNullOrEmpty(SelectedFile) Then
                        Dim intFileId As Integer = Integer.Parse(Me.SelectedFile)
                        Dim objFileInfo As FileInfo = New FileController().GetFileById(intFileId, FolderPath.PortalId)
                        Me._SmartFile = SmartFile.LoadSmartFile(Of T)(objFileInfo)
                    End If
                End If
                Return _SmartFile
            End Get
            Set(value As SmartFile(Of T))
                _SmartFile = value
            End Set
        End Property


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function

        Public Function GetSelectorG(propertyName As String) As IList(Of FileInfo) Implements ISelector(Of FileInfo).GetSelectorG
            Dim toReturn As New List(Of FileInfo)
            Select Case propertyName
                Case "SelectedFile"
                    Dim objFolder As FolderInfo = ObsoleteDNNProvider.Instance.GetFolderFromPath(FolderPath.PortalId, FolderPath.Path.GetValue())
                    If objFolder IsNot Nothing Then
                        Dim objFiles As IEnumerable(Of FileInfo) = ObsoleteDNNProvider.Instance.GetFiles(objFolder)
                        toReturn.AddRange(objFiles)
                    End If
            End Select
            Return toReturn
        End Function
    End Class
End NameSpace