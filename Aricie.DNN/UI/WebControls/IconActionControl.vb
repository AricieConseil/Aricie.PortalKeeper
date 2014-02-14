Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Text

Namespace UI.WebControls
    <ParseChildren(True)> _
    Public Class IconActionControl
        Inherits System.Web.UI.WebControls.WebControl

        Public Property Url() As String = String.Empty

        Public Overridable Property Text() As String = String.Empty

        Public Property ResourceKey As String = ""

        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property ActionItem() As New IconActionInfo



        Protected Overrides Sub OnPreRender(e As System.EventArgs)
            MyBase.OnPreRender(e)




            If (Not Me.ActionItem Is Nothing) Then
                Dim currentControl As Control = Me



                'Dim htmlToAdd As New System.Text.StringBuilder()
                If Me.Enabled AndAlso (Not String.IsNullOrEmpty(Me.Url)) Then
                    Dim hl As New HyperLink
                    currentControl.Controls.Add(hl)
                    currentControl = hl
                    hl.NavigateUrl = Me.Url
                    hl.CssClass = Me.CssClass
                End If

                If (ActionItem.StackedIconName <> IconName.None) Then
                    Dim stackP As New HtmlControls.HtmlGenericControl("p")
                    currentControl.Controls.Add(stackP)
                    currentControl = stackP
                    Dim containerCssClass As String = "fa-stack" & GetCssClass(IconName.None, ActionItem.StackContainerOptions, False)
                    stackP.Attributes.Add("class", containerCssClass)
                End If

                If (ActionItem.IconName <> IconName.None) Then
                    Dim iconLabel As New Label
                    currentControl.Controls.Add(iconLabel)
                    iconLabel.CssClass = Me.GetCssClass(ActionItem.IconName, ActionItem.IconOptions, ActionItem.StackedIconName <> IconName.None)
                End If


                If (ActionItem.StackedIconName <> IconName.None) Then
                    Dim stackIcon As New Label

                    currentControl.Controls.Add(stackIcon)
                    stackIcon.CssClass = Me.GetCssClass(ActionItem.StackedIconName, ActionItem.StackedIconOptions, True)
                    currentControl = currentControl.Parent
                End If

                If (Not String.IsNullOrEmpty(Me.Text)) Then
                    Dim objTextLabel As New Label()
                    currentControl.Controls.Add(objTextLabel)
                    objTextLabel.Text = Me.Text
                    'objTextLabel.CssClass = Me.CssClass
                    objTextLabel.CssClass = "actionText"
                    objTextLabel.Attributes.Add("resourcekey", Me.ResourceKey)
                End If




                ResourcesUtils.registerStylesheet(Me.Page, "font-awesome", "//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css", False)
            End If



        End Sub

        Public Function GetCssClass(objIconName As IconName, objOptions As IconOptions, stacked As Boolean) As String
            Dim toReturn As New StringBuilder()

            If objIconName <> IconName.None Then
                toReturn.AppendFormat("fa {0}", objIconName)
            End If

            If (objOptions And IconOptions.Rotate90) = IconOptions.Rotate90 Then
                toReturn.Append(" fa-rotate-90")
            ElseIf (objOptions And IconOptions.Rotate180) = IconOptions.Rotate180 Then
                toReturn.Append(" fa-rotate-180")
            ElseIf (objOptions And IconOptions.Rotate270) = IconOptions.Rotate180 Then
                toReturn.Append(" fa-rotate-270")
            ElseIf (objOptions And IconOptions.FlipHorizontal) = IconOptions.FlipHorizontal Then
                toReturn.Append(" fa-flip-horizontal")
            ElseIf (objOptions And IconOptions.FlipVertical) = IconOptions.FlipVertical Then
                toReturn.Append(" fa-flip-vertical")
            End If

            If (objOptions And IconOptions.Border) = IconOptions.Border Then
                toReturn.Append("  fa-border")
            End If

            If (objOptions And IconOptions.FixedWidth) = IconOptions.FixedWidth Then
                toReturn.Append("  fa-fw")
            End If

            If (objOptions And IconOptions.Spin) = IconOptions.Spin Then
                toReturn.Append("  fa-spin")
            End If

            If stacked Then
                If (objOptions And IconOptions.Stack2X) = IconOptions.Stack2X Then
                    toReturn.Append(" fa-stack-2x")
                Else
                    toReturn.Append(" fa-stack-1x")
                End If
            Else
                If (objOptions And IconOptions.Large) = IconOptions.Large Then
                    toReturn.Append(" fa-lg")
                ElseIf (objOptions And IconOptions.x2) = IconOptions.x2 Then
                    toReturn.Append(" fa-2x")
                ElseIf (objOptions And IconOptions.x3) = IconOptions.x3 Then
                    toReturn.Append(" fa-3x")
                ElseIf (objOptions And IconOptions.x4) = IconOptions.x4 Then
                    toReturn.Append(" fa-4x")
                ElseIf (objOptions And IconOptions.x5) = IconOptions.x5 Then
                    toReturn.Append(" fa-5x")
                End If
            End If


            Return toReturn.ToString()
        End Function



        Protected Overrides Sub Render(writer As HtmlTextWriter)
            MyBase.RenderChildren(writer)
        End Sub


    End Class

End Namespace
