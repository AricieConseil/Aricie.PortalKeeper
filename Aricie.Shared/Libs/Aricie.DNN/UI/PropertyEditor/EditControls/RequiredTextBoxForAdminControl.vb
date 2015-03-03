
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace UI.WebControls.EditControls
    <ToolboxData("<{0}:RequiredTextBoxForAdminControl runat=server></{0}:RequiredTextBoxForAdminControl>")> _
    Public Class RequiredTextBoxForAdminControl
        Inherits EditControl

        Public tb As TextBox
        Public img As Image
        Public validator As RequiredFieldValidator

        Private _customMessage As String

        Property CustomMessage As String
            Get
                Return _customMessage
            End Get
            Set(ByVal value As String)
                _customMessage = value
            End Set
        End Property



        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            img = New Image
            tb = New TextBox
            validator = New RequiredFieldValidator

            If String.IsNullOrEmpty(CustomMessage) Then
                validator.ErrorMessage = "Information obligatoire"
            Else
                validator.ErrorMessage = CustomMessage
            End If
            validator.Display = ValidatorDisplay.Dynamic
            tb.Width = 140
            tb.ID = Me.ID + "Text"
            Me.Controls.Add(Me.tb)

            img.ID = Me.ID + "img"
            Me.img.ImageUrl = "~/images/required.gif"
            Me.Controls.Add(Me.img)

            Me.validator.ControlToValidate = Me.ID + "Text"
            Me.Controls.Add(Me.validator)



        End Sub

        Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
            Dim args As New PropertyEditorEventArgs(MyBase.Name)
            args.Value = Me.StringValue
            args.OldValue = MyBase.OldValue
            args.StringValue = Me.StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) _
            As Boolean
            Me.EnsureChildControls()
            Return MyBase.LoadPostData(postDataKey + "Text", postCollection)
        End Function

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            Me.EnsureChildControls()
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            MyBase.RenderChildren(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)

        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            If Not StringValue = String.Empty Then
                tb.Text = StringValue
            End If

            If ((Not Me.Page Is Nothing) AndAlso (MyBase.EditMode = PropertyEditorMode.Edit)) Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If

        End Sub

        Protected Overrides Property StringValue As String
            Get
                If (MyBase.Value Is Nothing) Then
                    Return String.Empty
                End If
                Return MyBase.Value.ToString
            End Get
            Set(ByVal value As String)
                MyBase.Value = value
            End Set
        End Property
    End Class
End Namespace
