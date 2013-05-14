Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.Common
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Globalization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities
Imports Aricie.DNN.UI.WebControls.EditControls


Namespace UI.WebControls.EditControls


    <ToolboxData("<{0}:RequiredNumberForAdminControl runat=server></{0}:RequiredNumberForAdminControl>")> _
    Public Class RequiredNumberForAdminControl
        Inherits RequiredTextBoxForAdminControl

        'Private tb As TextBox
        'Private img As Image
        Private validator2 As RegularExpressionValidator

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            'img = New Image
            'MyBase.tb = New TextBox
            validator2 = New RegularExpressionValidator
            CustomMessage = "Information de type numérique obligatoire"
            validator2.ValidationExpression = "^[0-9]+$"
            validator2.ErrorMessage = CustomMessage
            'tb.Width = 80
            'tb.ID = Me.ID + "Text"
            'Me.Controls.Add(Me.tb)

            'img.ID = Me.ID + "img"
            'Me.img.ImageUrl = "~/images/required.gif"
            ' Me.Controls.Add(Me.img)

            Me.validator2.ControlToValidate = tb.ID
            Me.Controls.Add(Me.validator2)
        End Sub


        Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
            Dim args As New PropertyEditorEventArgs(MyBase.Name)
            If IsNumeric(Me.StringValue) Then
                args.Value = Me.StringValue
                args.OldValue = MyBase.OldValue
                args.StringValue = Me.StringValue
            End If
            MyBase.OnValueChanged(args)
        End Sub

    End Class
End Namespace