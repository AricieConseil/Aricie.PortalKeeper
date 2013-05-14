Imports System.Web.UI
Imports System.Web.UI.HtmlControls

Namespace UI.WebControls.EditControls
    Public Class AricieTitleEditControl
        Inherits AricieEditControl
        Implements ILargeEditControl

        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)

        End Sub


        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Dim title As New HtmlGenericControl("div")
            title.Attributes.Add("class", "Head")
            title.InnerText = Me.StringValue

            title.RenderControl(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Dim title As New HtmlGenericControl("div")
            title.Attributes.Add("class", "Head")
            title.InnerText = Me.StringValue

            title.RenderControl(writer)
        End Sub
    End Class
End Namespace
