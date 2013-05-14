Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports DotNetNuke.UI.WebControls
Imports Calendar = DotNetNuke.Common.Utilities.Calendar

Namespace UI.WebControls.EditControls
    Public Class AricieCalendarEditControl
        Inherits AricieEditControl

        Protected WithEvents _txt As New TextBox
        Protected WithEvents _cal As New HyperLink


        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim strValue As String = CType(Value, String)
            Dim strOldValue As String = CType(OldValue, String)

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = strValue
            args.OldValue = strOldValue
            args.StringValue = StringValue

            MyBase.OnValueChanged(args)
        End Sub


        Private Sub AricieCalendarEditControl_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            Me.Controls.Add(_txt)
            Me.Controls.Add(_cal)

            _cal.NavigateUrl = Calendar.InvokePopupCal(_txt)
            _cal.ImageUrl = "~/DesktopModules/Aricie.DynamicForms/images/Calendar-16x16.gif"
            'cal.Text = "Cal"

        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            _txt.Text = Me.StringValue

            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)

            For Each control As Control In Me.Controls
                control.RenderControl(writer)
            Next
        End Sub
    End Class
End Namespace
