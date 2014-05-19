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


    Private Function GetStringPostBackRefrence() As String
        Dim options As New PostBackOptions(_ParentControl, _ParentControl.ID)
        Return Me._ParentControl.Page.ClientScript.GetPostBackEventReference(options, True)
    End Function

    Private Function AddPostBack(value As String) As String
        If Not String.IsNullOrEmpty(value) Then
            value &= ";"c
        End If
        Dim onClick As String = GetStringPostBackRefrence()
        value &= onClick
        Return value
    End Function

    Public Overrides Sub RenderBeginTag(ByVal objTagKey As System.Web.UI.HtmlTextWriterTag)
        If Me._AutoPostBack Then
            Select Case objTagKey
                Case HtmlTextWriterTag.Input, HtmlTextWriterTag.Select
                    Select Case objTagKey
                        Case HtmlTextWriterTag.Input
                            If Not Me.IsAttributeDefined(HtmlTextWriterAttribute.Onclick) Then
                                Dim onClick As String = GetStringPostBackRefrence()
                                Me.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick)
                            End If
                        Case HtmlTextWriterTag.Select
                            If Not Me.IsAttributeDefined(HtmlTextWriterAttribute.Onchange) Then
                                Dim onClick As String = GetStringPostBackRefrence()
                                Me.AddAttribute(HtmlTextWriterAttribute.Onchange, onClick)
                            End If
                    End Select
            End Select
        End If
        MyBase.RenderBeginTag(objTagKey)
    End Sub

    Public Overrides Sub AddAttribute(ByVal key As System.Web.UI.HtmlTextWriterAttribute, ByVal value As String)
        If Me._PasswordMode AndAlso key = HtmlTextWriterAttribute.Type AndAlso value = "text" Then
            MyBase.AddAttribute(HtmlTextWriterAttribute.Type, "password")
        Else
            If _AutoPostBack AndAlso (key = HtmlTextWriterAttribute.Onclick OrElse key = HtmlTextWriterAttribute.Onchange) Then
                value = AddPostBack(value)
            End If
            MyBase.AddAttribute(key, value)
        End If
    End Sub


    Public Overrides Sub AddAttribute(name As String, value As String)
        If _AutoPostBack AndAlso name.ToLower() = "onclick" Then
           value = AddPostBack(value)
        End If
        MyBase.AddAttribute(name, value)
    End Sub



End Class