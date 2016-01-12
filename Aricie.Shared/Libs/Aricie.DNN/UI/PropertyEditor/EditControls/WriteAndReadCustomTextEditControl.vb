Imports System.Web.UI

Namespace UI.WebControls.EditControls
    Public Class WriteAndReadCustomTextEditControl
        Inherits CustomTextEditControl

        Protected Overrides Sub RenderViewMode(writer As HtmlTextWriter)
            RenderMode(writer, True)
        End Sub


    End Class
End NameSpace