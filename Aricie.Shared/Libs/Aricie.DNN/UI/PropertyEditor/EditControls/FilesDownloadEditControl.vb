Imports System.Reflection
Imports System.Web.UI
Imports System.Globalization
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.IO
Imports System.Web.UI.WebControls
Imports Aricie.Web.UI
Imports Aricie.Services

Namespace UI.WebControls.EditControls
    Public Class FilesDownloadEditControl
        Inherits AricieEditControl

#Region "Inherits AricieEditControl"
        Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = StringValue
            args.OldValue = StringValue
            args.StringValue = StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            Me.EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Try
                Dim divRoot As New HtmlControls.HtmlGenericControl("div")
                divRoot.Attributes.Add("class", "ctLeft")

                Dim fold As DirectoryInfo = Folder
                If fold.Exists() Then
                    Dim listFiles As FileInfo() = fold.GetFiles()
                    If listFiles.Length > 0 Then
                        Dim hasWriteLockFile As Boolean = False
                        For Each file As FileInfo In listFiles
                            If file.Name.ToLower = "write.lock" Then hasWriteLockFile = True
                        Next

                        If Not hasWriteLockFile Then
                            For Each file As FileInfo In listFiles
                                Dim divChild As New HtmlControls.HtmlGenericControl("div")
                                Dim cmdDownloadFile As New LinkButton()
                                cmdDownloadFile.ID = Me.ID & "cmdDownloadFile" & file.Name
                                cmdDownloadFile.Text = file.Name
                                cmdDownloadFile.CommandArgument = file.FullName
                                AddHandler cmdDownloadFile.Click, AddressOf CmdDownloadFile_Click
                                divChild.Controls.Add(cmdDownloadFile)
                                divRoot.Controls.Add(divChild)
                            Next
                        Else
                            Dim lblWriteLock As New Label()
                            lblWriteLock.ID = Me.ID & "lblWriteLock"
                            lblWriteLock.Text = "Files are locked temporarily"
                            lblWriteLock.Attributes.Add("Resourcekey", lblWriteLock.ID)
                            divRoot.Controls.Add(lblWriteLock)
                        End If

                    Else
                        Dim lblNoFiles As New Label()
                        lblNoFiles.ID = Me.ID & "lblNoFiles"
                        lblNoFiles.Text = "No Files"
                        lblNoFiles.Attributes.Add("Resourcekey", lblNoFiles.ID)
                        divRoot.Controls.Add(lblNoFiles)
                    End If
                Else
                    Dim lblNoFolder As New Label()
                    lblNoFolder.ID = Me.ID & "lblNoFolder"
                    lblNoFolder.Text = "Folder doesn't exist yet"
                    lblNoFolder.Attributes.Add("Resourcekey", lblNoFolder.ID)
                    divRoot.Controls.Add(lblNoFolder)
                End If
                Me.Controls.Add(divRoot)
            Finally
                Me.ChildControlsCreated = True
            End Try

           
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            RenderMode(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            RenderMode(writer)
        End Sub

        Private Sub RenderMode(ByVal writer As HtmlTextWriter)
            For Each ctl As Control In Me.Controls
                ctl.RenderControl(writer)
            Next
        End Sub
#End Region

#Region "Properties"
        Protected Overrides Property StringValue() As String
            Get
                Return Me.Value.ToString
            End Get
            Set(ByVal value As String)
                MyBase.StringValue = value
            End Set
        End Property

        Protected ReadOnly Property Folder() As DirectoryInfo
            Get
                Return New DirectoryInfo(Me.StringValue)
            End Get
        End Property
#End Region

#Region "Protected Methods"
        Protected Sub CmdDownloadFile_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim link As LinkButton = DirectCast(sender, LinkButton)
            Dim pg As Page = Me.Page
            FileHelper.DownloadFile(link.CommandArgument, pg.Response, pg.Server)
        End Sub
#End Region


    End Class
End Namespace
