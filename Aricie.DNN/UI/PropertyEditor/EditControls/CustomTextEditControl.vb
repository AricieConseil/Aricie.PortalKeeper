Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Web.UI
Imports DotNetNuke.Common.Utilities
Imports System.Globalization
Imports System.Web

Namespace UI.WebControls.EditControls



    Public Class WriteAndReadCustomTextEditControl
        Inherits CustomTextEditControl

        Protected Overrides Sub RenderViewMode(writer As HtmlTextWriter)
            RenderMode(writer, True)
        End Sub


    End Class
    Public Class CustomTextEditControl
        Inherits TextEditControl

        Public Property MaxLength As Integer = Null.NullInteger
        Public Property WidthPx As Integer = Null.NullInteger
        Public Property LineCount As Integer = 1
        Public Property Size As Integer = Null.NullInteger


        Protected Overrides Sub OnAttributesChanged()
            MyBase.OnAttributesChanged()
            If (Not MyBase.CustomAttributes Is Nothing) Then
                For Each attribute As Attribute In MyBase.CustomAttributes
                    If TypeOf attribute Is MaxLengthAttribute Then
                        Me.MaxLength = DirectCast(attribute, MaxLengthAttribute).Length
                    ElseIf TypeOf attribute Is WidthAttribute Then
                        Me.WidthPx = DirectCast(attribute, WidthAttribute).Width
                    ElseIf TypeOf attribute Is LineCountAttribute Then
                        Me.LineCount = DirectCast(attribute, LineCountAttribute).LineCount
                    ElseIf TypeOf attribute Is SizeAttribute Then
                        Me.Size = DirectCast(attribute, SizeAttribute).Size
                    End If
                Next
            End If
        End Sub



        ' Methods
        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            Me.RenderMode(writer, False)
        End Sub

        Protected Overridable Sub RenderMode(ByVal writer As HtmlTextWriter, makeReadonly As Boolean)
            MyBase.ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            If makeReadonly Then
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "true")
            End If
            If (Size > Null.NullInteger) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Size, _
                                     Size.ToString(CultureInfo.InvariantCulture))
            End If
            If (MaxLength > Null.NullInteger) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, _
                                     MaxLength.ToString(CultureInfo.InvariantCulture))
            End If
            If (WidthPx > Null.NullInteger) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Style, ("width: " & WidthPx & "px"))
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            If (LineCount = 1) Then
                'writer.AddAttribute(HtmlTextWriterAttribute.Value, HttpUtility.HtmlEncode(Me.StringValue))
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Me.StringValue)
                writer.RenderBeginTag(HtmlTextWriterTag.Input)
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Rows, LineCount.ToString(CultureInfo.InvariantCulture))
                writer.RenderBeginTag(HtmlTextWriterTag.Textarea)
                writer.Write(HtmlEncode(Me.StringValue))

            End If
            writer.RenderEndTag()
        End Sub




    End Class
End Namespace