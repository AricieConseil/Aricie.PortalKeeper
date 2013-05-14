Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace UI.WebControls


    Public Class YesNoCtl
        Inherits RadioButtonList

        Public Sub New()
            MyBase.New()
            Dim itemYes As New ListItem
            itemYes.Text = "oui"
            itemYes.Value = "1"
            Dim itemNo As New ListItem
            itemNo.Text = "non"
            itemNo.Value = "0"
            itemNo.Selected = True

            MyBase.Items.Add(itemYes)
            MyBase.Items.Add(itemNo)
            MyBase.RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Horizontal
        End Sub

    End Class
End Namespace
