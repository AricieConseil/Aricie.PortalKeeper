Imports DotNetNuke.Common
Imports System.Web.UI

Namespace UI.WebControls.EditControls
    Public Class GraphEditControl
        Inherits AricieImageEditControl

        Protected EnlargeLink As New System.Web.UI.WebControls.HyperLink


        Public Function GetHandlerName() As String
            Return "GraphImage.ashx?display=" & Me.Name
        End Function


        Public Overrides Function ResolveImageUrl() As String
            Return ApplicationPath & "/"c & Me.GetHandlerName
        End Function


        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

        Public Overrides Sub AddImageControl(ByVal img As System.Web.UI.WebControls.Image)
            Me.EnlargeLink.Controls.Add(img)

            Me.Controls.Add(EnlargeLink)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            Me.ImageControl.AlternateText = "Sequence Graph Preview - Click To Enlarge"
            Me.EnlargeLink.NavigateUrl = Me.ImageControl.ImageUrl & "&width=5000"
            Me.EnlargeLink.Target = "_blank"
        End Sub

    End Class
End Namespace