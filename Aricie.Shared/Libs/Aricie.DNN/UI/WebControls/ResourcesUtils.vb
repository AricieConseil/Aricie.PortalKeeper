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
        End Sub
    End Class
End Namespace
