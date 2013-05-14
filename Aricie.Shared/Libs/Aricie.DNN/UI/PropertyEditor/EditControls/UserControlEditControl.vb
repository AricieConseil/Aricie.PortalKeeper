Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports System.Web.UI
Imports Aricie.DNN.UI.WebControls
Imports DotNetNuke.UI.WebControls

Public Class UserControlEditControl
    Inherits AricieEditControl

    Private _userControlPath As String


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)
        Me.EnsureChildControls()
    End Sub


    Protected Overrides Sub OnDataChanged(ByVal e As System.EventArgs)
        'Dim args As New PropertyEditorEventArgs(Me.Name)
        'args.Value = Me.Value
        'args.OldValue = Me.OldValue
        'args.StringValue = Me.StringValue
        'MyBase.OnValueChanged(args)
    End Sub

    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()


        If Not String.IsNullOrEmpty(_userControlPath) Then
            Dim uc As PropertyEditorUserControlBase = DirectCast(Me.Page.LoadControl(_userControlPath), PropertyEditorUserControlBase)

            uc.ID = "uc"
            uc.EnableViewState = True
            uc.Value = Me.Value

            Me.Controls.Add(uc)
        End If

    End Sub

    Protected Overrides Sub OnAttributesChanged()
        MyBase.OnAttributesChanged()
        If (Not CustomAttributes Is Nothing) Then
            For Each attribute As Attribute In CustomAttributes
                If TypeOf attribute Is UserControlAttribute Then
                    Dim ucAtt As UserControlAttribute = CType(attribute, UserControlAttribute)
                    Me._userControlPath = ucAtt.UserControlPath
                End If
            Next
        End If
    End Sub

    Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
        Return False
    End Function

    Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        MyBase.OnPreRender(e)

        If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
            Me.Page.RegisterRequiresPostBack(Me)
        End If
    End Sub

    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        RenderChildren(writer)
    End Sub

    Public Property TrialMode As TrialModeInformation
        Get
            Return DirectCast(Controls(0), PropertyEditorUserControlBase).TrialMode
        End Get
        Set(value As TrialModeInformation)
            DirectCast(Controls(0), PropertyEditorUserControlBase).TrialMode = value
        End Set
    End Property


    Public Class TrialModeInformation
        Public Property CurrentTrialMode As Security.Trial.TrialPropertyMode
        Public Property Message As String
    End Class

End Class
