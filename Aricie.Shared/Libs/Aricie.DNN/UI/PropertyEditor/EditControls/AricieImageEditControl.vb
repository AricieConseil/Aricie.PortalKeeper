Imports System.Web.UI.WebControls
Imports DotNetNuke.Common
Imports DotNetNuke.UI.UserControls
Imports System.Web.UI
Imports System.Collections.Specialized
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.FileSystem
Imports Aricie.DNN.Services

Namespace UI.WebControls.EditControls

   


    Public Class AricieImageEditControl
        Inherits AricieUrlEditControl


        Protected ImageControl As New Image




        Protected Overrides Sub CreateChildControls()
            Me.AddImageControl(Me.ImageControl)
            MyBase.CreateChildControls()
        End Sub



      

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            Me.ImageControl.ImageUrl = Me.ResolveImageUrl()

        End Sub

        Protected Overrides Sub ResolveEditControl()
            MyBase.ResolveEditControl()

            Me.UrlControl.FileFilter = glbImageFileTypes
            Me.UrlControl.ShowTabs = False

        End Sub
        

        Public Overridable Sub AddImageControl(ByVal img As Image)
            Me.Controls.Add(Me.ImageControl)

        End Sub

        Protected Overrides Sub ResolveNewValue()
            Dim controller As New FileController()
            Dim fi As FileInfo = controller.GetFileById(Integer.Parse(Me.StringValue), PortalSettings.PortalId)
            Me.Value = Me.ResolveUrl("~/" & fi.Folder & fi.FileName)
        End Sub



        Public Overridable Function ResolveImageUrl() As String
            Return Me.StringValue
        End Function



    End Class
End Namespace
