Imports System.Web.UI.WebControls
Imports System.Web.UI

Namespace UI.WebControls
    <ParseChildren(True)> _
    Public Class IconActionControl
        Inherits System.Web.UI.WebControls.WebControl

        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property ActionItem() As IconActionEntity



        Protected Overrides Sub OnPreRender(e As System.EventArgs)
            MyBase.OnPreRender(e)
            Dim myLit As New Literal()
            Dim htmlToAdd As New System.Text.StringBuilder()
            
            If (Not Me.ActionItem Is Nothing) Then
                If (Not String.IsNullOrEmpty(ActionItem.Url)) Then
                    htmlToAdd.AppendFormat("<a href='{0}'>", ActionItem.Url)
                End If
                If (ActionItem.StackedIconName <> IconActionEntity.IconsName.None) Then
                    htmlToAdd.AppendFormat("<p class='fa-stack fa-lg'>")
                End If
                
                htmlToAdd.AppendFormat("<span class='fa {0} ", IconActionEntity.Icons(ActionItem.IconName))
                If (ActionItem.StackedIconName <> IconActionEntity.IconsName.None) Then
                    htmlToAdd.Append(" fa-stack-1x ")
                End If

                If ActionItem.Border Then
                    htmlToAdd.Append("  fa-border ")
                End If
                If ActionItem.FixWidth Then
                    htmlToAdd.Append(" fa-fw ")
                End If
                If ActionItem.FlipAndRotate <> IconActionEntity.Rotate.Normal Then
                    If (ActionItem.FlipAndRotate And IconActionEntity.Rotate.Rotate90) = IconActionEntity.Rotate.Rotate90 Then
                        htmlToAdd.Append(" fa-rotate-90 ")
                    End If
                    If (ActionItem.FlipAndRotate And IconActionEntity.Rotate.Rotate180) = IconActionEntity.Rotate.Rotate180 Then
                        htmlToAdd.Append(" fa-rotate-180 ")
                    End If
                    If (ActionItem.FlipAndRotate And IconActionEntity.Rotate.Rotate270) = IconActionEntity.Rotate.Rotate270 Then
                        htmlToAdd.Append(" fa-rotate-270 ")
                    End If
                    If (ActionItem.FlipAndRotate And IconActionEntity.Rotate.FlipHorizontal) = IconActionEntity.Rotate.FlipHorizontal Then
                        htmlToAdd.Append(" fa-flip-horizontal ")
                    End If
                    If (ActionItem.FlipAndRotate And IconActionEntity.Rotate.FlipVertical) = IconActionEntity.Rotate.FlipVertical Then
                        htmlToAdd.Append(" fa-flip-vertical ")
                    End If

                End If
                If ActionItem.ZoomLevel <> IconActionEntity.Zoom.Normal Then
                    If ActionItem.ZoomLevel = IconActionEntity.Zoom.Large Then
                        htmlToAdd.Append("fa-lg")
                    End If
                    If ActionItem.ZoomLevel = IconActionEntity.Zoom.x2 Then
                        htmlToAdd.Append("fa-2x")
                    End If
                    If ActionItem.ZoomLevel = IconActionEntity.Zoom.x3 Then
                        htmlToAdd.Append("fa-3x")
                    End If
                    If ActionItem.ZoomLevel = IconActionEntity.Zoom.x4 Then
                        htmlToAdd.Append("fa-4x")
                    End If
                    If ActionItem.ZoomLevel = IconActionEntity.Zoom.x5 Then
                        htmlToAdd.Append("fa-5x")
                    End If
                End If
                If ActionItem.Spinning Then
                    htmlToAdd.Append(" fa-spin ")
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
                htmlToAdd.AppendFormat("'></span>{0}", ActionItem.Text)
                If (ActionItem.StackedIconName <> IconActionEntity.IconsName.None) Then
                    htmlToAdd.AppendFormat("<span class='fa fa-stack-2x {0}'></span></p>", IconActionEntity.Icons(ActionItem.StackedIconName))
                End If


                If (Not String.IsNullOrEmpty(ActionItem.Url)) Then
                    htmlToAdd.Append("</a>")
                End If
                myLit.Text = htmlToAdd.ToString
                'Else
                '    myLit.Text = String.Format("<a href='{0}'><span class='fa {1}'></span>{2}</a>", Url, IconName, Text)
            End If
            ResourcesUtils.registerStylesheet(Me.Page, "font-awesome", "//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css", False)
            Me.Controls.Add(myLit)

        End Sub
    End Class

End Namespace
