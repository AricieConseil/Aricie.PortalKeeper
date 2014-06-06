Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.Services.FileSystem
Imports System.Xml.Serialization
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports System.Globalization

Namespace Services.Files
    <Serializable()> _
    Public Class SmartFolder(Of T As New)
        Implements ISelector(Of FileInfo)

        Private _Encrypter As IEncrypter


        Public Sub New()

        End Sub

        Public Sub New(encrypter As IEncrypter)
            Me._Encrypter = encrypter
        End Sub


        <IsReadOnly(True)> _
        Public Overridable Property FolderPath As New FolderPathInfo()

        <XmlIgnore()> _
        <AutoPostBack()> _
        <Editor(GetType(Aricie.DNN.UI.WebControls.EditControls.SelectorEditControl), GetType(EditControl))> _
        <Selector("FileName", "FileId", False, True, "Select a file", "", False, True)>
        Public Property SelectedFile As String = ""

        Public Property NoCache As Boolean

        Private _SmartFile As SmartFile(Of T)


        Public ReadOnly Property HasEncrypter As Boolean
            Get
                Return _Encrypter IsNot Nothing
            End Get
        End Property

        <ConditionalVisible("SelectedFile", True, True, "")> _
        <XmlIgnore()> _
        Public Property CurrentFile As SmartFile(Of T)
            Get
                If _SmartFile Is Nothing Then
                    If Not String.IsNullOrEmpty(SelectedFile) Then

                        Dim intFileId As Integer = Integer.Parse(Me.SelectedFile)
                        Dim objFile As SmartFile(Of T) = Nothing
                        If Not NoCache Then
                            objFile = CacheHelper.GetGlobal(Of SmartFile(Of T))(intFileId.ToString(CultureInfo.InvariantCulture))
                        End If
                        If objFile Is Nothing Then
                            Dim objFileInfo As FileInfo = New FileController().GetFileById(intFileId, FolderPath.PortalId)
                            objFile = SmartFile.LoadSmartFile(Of T)(objFileInfo)
                            If Not NoCache Then
                                CacheHelper.SetCacheDependant(Of SmartFile(Of T))(objFile, "", TimeSpan.FromMinutes(5), intFileId.ToString(CultureInfo.InvariantCulture))
                            End If
                        End If
                        If Me.HasEncrypter AndAlso objFile IsNot Nothing Then
                            objFile.SetEncrypter(_Encrypter)
                        End If
                        Me._SmartFile = objFile
                    End If
                Else
                    If String.IsNullOrEmpty(SelectedFile) Then
                        _SmartFile = Nothing
                    End If
                End If
                Return _SmartFile
            End Get
            Set(value As SmartFile(Of T))
                _SmartFile = value
            End Set
        End Property

        Public Sub SetEncrypter(ByVal encrypter As IEncrypter)
            _encrypter = encrypter
        End Sub


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