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

                    'htmlToAdd.AppendFormat("<a href='{0}'>", ActionItem.Url)
                End If

                If (ActionItem.StackedIconName <> IconName.None) Then
                    Dim stackP As New HtmlControls.HtmlGenericControl("p")
                    currentControl.Controls.Add(stackP)
                    currentControl = stackP
                    stackP.Attributes.Add("class", "fa-stack")
                    'htmlToAdd.AppendFormat("<p class='fa-stack fa-lg'>")
                End If

                If (ActionItem.IconName <> IconName.None) Then
                    Dim iconLabel As New Label
                    currentControl.Controls.Add(iconLabel)
                    iconLabel.CssClass = Me.GetCssClass()
                End If

                
                'If Not String.IsNullOrEmpty(Me.CssClass) Then
                '    iconLabel.CssClass = Me.CssClass & " " & iconLabel.CssClass
                'End If

                If (ActionItem.StackedIconName <> IconName.None) Then
                    Dim stackIcon As New Label

                    currentControl.Controls.Add(stackIcon)
                    stackIcon.CssClass = String.Format("fa fa-stack-2x {0}", IconActionInfo.Icons(ActionItem.StackedIconName))
                    currentControl = currentControl.Parent
                End If

                'If (Not String.IsNullOrEmpty(Me.Url)) Then
                '    currentControl = currentControl.Parent
                '    Dim hl As New HyperLink
                '    currentControl.Controls.Add(hl)
                '    currentControl = hl
                '    hl.NavigateUrl = Me.Url
                '    hl.CssClass = Me.CssClass
                'End If

                If (Not String.IsNullOrEmpty(Me.Text)) Then
                    Dim objTextLabel As New Label()
                    currentControl.Controls.Add(objTextLabel)
                    objTextLabel.Text = Me.Text
                    'objTextLabel.CssClass = Me.CssClass
                    objTextLabel.Attributes.Add("resourcekey", Me.ResourceKey)
                End If



                'If (ActionItem.StackedIconName <> IconName.None) Then
                '    htmlToAdd.AppendFormat("<span class='fa fa-stack-2x {0}'></span></p>", IconActionInfo.Icons(ActionItem.StackedIconName))
                'End If


                'If (Not String.IsNullOrEmpty(ActionItem.Url)) Then
                '    htmlToAdd.Append("</a>")
                'End If

                'myLiteral.Text = htmlToAdd.ToString()
                'Else
                '    myLit.Text = String.Format("<a href='{0}'><span class='fa {1}'></span>{2}</a>", Url, IconName, Text)
            End If
            ResourcesUtils.registerStylesheet(Me.Page, "font-awesome", "//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css", False)


        End Sub

        Public Function GetCssClass() As String
            Dim toReturn As New StringBuilder()
            toReturn.AppendFormat("fa {0}", IconActionInfo.Icons(ActionItem.IconName))
            If (ActionItem.StackedIconName <> IconName.None) Then
                toReturn.Append(" fa-stack-1x")
            End If

            If ActionItem.Border Then
                toReturn.Append("  fa-border")
            End If
            If ActionItem.FixWidth Then
                toReturn.Append(" fa-fw")
            End If
            If ActionItem.FlipAndRotate <> IconActionInfo.Rotate.Normal Then
                If (ActionItem.FlipAndRotate And IconActionInfo.Rotate.Rotate90) = IconActionInfo.Rotate.Rotate90 Then
                    toReturn.Append(" fa-rotate-90")
                End If
                If (ActionItem.FlipAndRotate And IconActionInfo.Rotate.Rotate180) = IconActionInfo.Rotate.Rotate180 Then
                    toReturn.Append(" fa-rotate-180")
                End If
                If (ActionItem.FlipAndRotate And IconActionInfo.Rotate.Rotate270) = IconActionInfo.Rotate.Rotate270 Then
                    toReturn.Append(" fa-rotate-270")
                End If
                If (ActionItem.FlipAndRotate And IconActionInfo.Rotate.FlipHorizontal) = IconActionInfo.Rotate.FlipHorizontal Then
                    toReturn.Append(" fa-flip-horizontal")
                End If
                If (ActionItem.FlipAndRotate And IconActionInfo.Rotate.FlipVertical) = IconActionInfo.Rotate.FlipVertical Then
                    toReturn.Append(" fa-flip-vertical")
                End If

            End If
            If ActionItem.ZoomLevel <> IconActionInfo.Zoom.Normal Then
                If ActionItem.ZoomLevel = IconActionInfo.Zoom.Large Then
                    toReturn.Append(" fa-lg")
                End If
                If ActionItem.ZoomLevel = IconActionInfo.Zoom.x2 Then
                    toReturn.Append(" fa-2x")
                End If
                If ActionItem.ZoomLevel = IconActionInfo.Zoom.x3 Then
                    toReturn.Append(" fa-3x")
                End If
                If ActionItem.ZoomLevel = IconActionInfo.Zoom.x4 Then
                    toReturn.Append(" fa-4x")
                End If
                If ActionItem.ZoomLevel = IconActionInfo.Zoom.x5 Then
                    toReturn.Append(" fa-5x")
                End If
            End If
            If ActionItem.Spinning Then
                toReturn.Append(" fa-spin")
            End If

            'If ActionItem.StackStatus <> IconActionEntity.Stack.Normal Then
            '    If (ActionItem.StackStatus And IconActionEntity.Stack.Stack1x) = IconActionEntity.Stack.Stack1x Then
            '        htmlToAdd.Append(" fa-stack-1x ")
            '    End If
            '    If (ActionItem.StackStatus And IconActionEntity.Stack.Stack2x) = IconActionEntity.Stack.Stack2x Then
            '        htmlToAdd.Append(" fa-stack-2x ")
            '    End If
            '    If (ActionItem.StackStatus And IconActionEntity.Stack.InverseColor) = IconActionEntity.Stack.InverseColor Then
            '        htmlToAdd.Append(" fa-inverse ")
            '    End If
            '   End If

            Return toReturn.ToString()
        End Function

        Protected Overrides Sub Render(writer As HtmlTextWriter)
            MyBase.RenderChildren(writer)
        End Sub


    End Class

End Namespace
