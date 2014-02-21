Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Aricie.DNN.Services

Namespace UI.WebControls
    <ParseChildren(True)> _
    Public Class IconActionButton
        Inherits IconActionControl
        Implements IPostBackEventHandler
        Implements IButtonControl

        Public Overrides Property CssClass As String
            Get
                If MyBase.CssClass = "" Then
                    MyBase.CssClass = "aricieAction"
                End If
                Return MyBase.CssClass
            End Get
            Set(value As String)
                MyBase.CssClass = value
            End Set
        End Property


        Public Sub RaisePostBackEvent(eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
            If CausesValidation Then
                Me.Page.Validate(Me.ValidationGroup)
            End If

            RaiseEvent Click(Me, EventArgs.Empty)
            RaiseEvent Command(Me, New CommandEventArgs(Me.CommandName, Me.CommandArgument))

        End Sub

        Public Property CausesValidation As Boolean Implements IButtonControl.CausesValidation

        Public Event Click(sender As Object, e As EventArgs) Implements IButtonControl.Click

        Public Event Command(sender As Object, e As CommandEventArgs) Implements IButtonControl.Command

        Public Property CommandArgument As String Implements IButtonControl.CommandArgument

        Public Property CommandName As String Implements IButtonControl.CommandName

        Public Property PostBackUrl As String Implements IButtonControl.PostBackUrl

        Public Overrides Property Text As String Implements IButtonControl.Text

        Public Property ValidationGroup As String Implements IButtonControl.ValidationGroup

        Protected Overrides Sub OnLoad(e As EventArgs)
            Me.Url = DotNetNuke.UI.Utilities.ClientAPI.GetPostBackClientHyperlink(Me, Me.CommandArgument)
        End Sub



    End Class
End NameSpace