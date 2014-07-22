Imports System.Web.UI
Namespace UI.WebControls


    Public Class ResourcesUtils
        Public Shared Function getWebResourceUrl(ByVal page As Page, ByVal assemblyRef As Object, ByVal resourceName As String) As String
            Dim res As String = Nothing
            If (page IsNot Nothing) AndAlso (page.ClientScript IsNot Nothing) AndAlso (assemblyRef IsNot Nothing) Then
                Dim objType As System.Type
                If (TypeOf assemblyRef Is System.Type) Then
                    objType = DirectCast(assemblyRef, System.Type)
                Else
                    objType = assemblyRef.GetType
                End If
                res = page.ClientScript.GetWebResourceUrl(objType, resourceName)
            End If
            Return res
        End Function

        Public Shared Sub registerStylesheet(ByVal page As Page, ByVal cssKey As String, ByVal url As String, ByVal beforeOthers As Boolean)
            If (page IsNot Nothing) AndAlso (page.Header IsNot Nothing) Then
                Dim cid As String = String.Format("css_{0}", cssKey)
                If page.Header.FindControl(cid) Is Nothing Then
                    Dim sm As ScriptManager = ScriptManager.GetCurrent(page)
                    If sm.IsInAsyncPostBack Then
                        '"if (jQuery("link#css_font-awesome").length==0)"
                        Dim scriptBlock As String = String.Format("if (jQuery('link#{0}').length==0) {{ var fileref = document.createElement('link');" _
                                                                                                             & "fileref.setAttribute('rel', 'stylesheet'); " _
                                                                                                             & "fileref.setAttribute('type', 'text/css'); " _
                                                                                                             & "fileref.setAttribute('href', '{1}'); " _
                                                                                                             & "document.getElementsByTagName('head')[0].appendChild(fileref);}}", cid, page.ResolveUrl(url))
                        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), cid, scriptBlock, True)
                        'DotNetNuke.UI.Utilities.ClientAPI.RegisterClientScriptBlock(page, cid,)

                    Else
                        Dim lnk As New HtmlControls.HtmlLink
                        lnk.ID = cid
                        lnk.Href = page.ResolveUrl(url)
                        lnk.Attributes.Add("type", "text/css")
                        lnk.Attributes.Add("rel", "stylesheet")
                        lnk.Attributes.Add("class", "Aricie_stylesheet")
                        If beforeOthers Then
                            page.Header.Controls.AddAt(0, lnk)
                        Else
                            page.Header.Controls.Add(lnk)
                        End If
                    End If
                End If
            End If
        End Sub
    End Class
End Namespace
