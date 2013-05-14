Imports System.Web.UI

Public Class FieldEditorHtmlWriter
    Inherits HtmlTextWriter

    Private _ParentControl As Control
    Private _AutoPostBack As Boolean
    Private _PasswordMode As Boolean


    Public Sub New(ByVal parentControl As Control, ByVal writer As HtmlTextWriter, ByVal autoPostback As Boolean, ByVal passwordMode As Boolean)
        MyBase.New(writer)
        Me._ParentControl = parentControl
        Me._AutoPostBack = autoPostback
        Me._PasswordMode = passwordMode
    End Sub

    Public Overrides Sub RenderBeginTag(ByVal tagKey As System.Web.UI.HtmlTextWriterTag)
        If Me._AutoPostBack Then
            Select Case tagKey
                Case HtmlTextWriterTag.Input, HtmlTextWriterTag.Select
                    Dim options As New PostBackOptions(_ParentControl, _ParentControl.ID)
                    Dim onClick As String = Me._ParentControl.Page.ClientScript.GetPostBackEventReference(options, True)
                    Select Case tagKey
                        Case HtmlTextWriterTag.Input
                            Me.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick)
                        Case HtmlTextWriterTag.Select
                            Me.AddAttribute(HtmlTextWriterAttribute.Onchange, onClick)
                    End Select
            End Select
        End If
        MyBase.RenderBeginTag(tagKey)
    End Sub

    Public Overrides Sub AddAttribute(ByVal key As System.Web.UI.HtmlTextWriterAttribute, ByVal value As String)
        If Me._PasswordMode AndAlso key = HtmlTextWriterAttribute.Type AndAlso value = "text" Then
            MyBase.AddAttribute(HtmlTextWriterAttribute.Type, "password")
        Else
            MyBase.AddAttribute(key, value)
        End If
    End Sub

End Class