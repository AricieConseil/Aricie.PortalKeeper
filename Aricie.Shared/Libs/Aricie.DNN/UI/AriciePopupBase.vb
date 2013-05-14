Imports DotNetNuke.Common
Imports DotNetNuke.Framework
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports DotNetNuke.Common.Utilities
Imports System.IO

Namespace UI.Controls
    ''' <summary>
    ''' Permet d'avoir un popup avec les fonctionalités de localisation de DNN, Skin.AddPageMessage et les feuilles de styles de DNN et du skin par défaut
    ''' </summary>
    ''' <remarks>Pour utiliser cette classe, il suffit d'en hériter votre aspx</remarks>
    Public Class AriciePopupBase
        Inherits CDefault

        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

            ' création du placeholder pour les CSS
            Dim phCss As New PlaceHolder()
            phCss.ID = "CSS"
            Me.Header.Controls.Add(phCss)

            ' ou permettre d'utiliser la fonction Skin.AddPageMessage
            Dim divContentPane As New HtmlGenericControl("div")
            divContentPane.ID = glbDefaultPane
            Me.Controls.AddAt(0, divContentPane)

            ' add CSS links
            ManageStyleSheets(False)
        End Sub

        Private Sub ManageStyleSheets(ByVal PortalCSS As Boolean)

            ' initialize reference paths to load the cascading style sheets
            Dim ID As String

            Dim objCSSCache As Hashtable = CType(DataCache.GetCache("CSS"), Hashtable)
            If objCSSCache Is Nothing Then
                objCSSCache = New Hashtable
            End If

            If PortalCSS = False Then
                ' default style sheet ( required )
                ID = CreateValidID(HostPath)
                AddStyleSheet(ID, HostPath & "default.css")

                ' skin package style sheet
                ID = CreateValidID(PortalSettings.ActiveTab.SkinPath)
                If objCSSCache.ContainsKey(ID) = False Then
                    If File.Exists(Server.MapPath(PortalSettings.ActiveTab.SkinPath) & "skin.css") Then
                        objCSSCache(ID) = PortalSettings.ActiveTab.SkinPath & "skin.css"
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not PerformanceSetting = PerformanceSettings.NoCaching Then
                        DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If

                ' skin file style sheet
                ID = CreateValidID(Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css"))
                If objCSSCache.ContainsKey(ID) = False Then
                    If File.Exists(Server.MapPath(Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css"))) Then
                        objCSSCache(ID) = Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css")
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not PerformanceSetting = PerformanceSettings.NoCaching Then
                        DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If
            Else
                ' portal style sheet
                ID = CreateValidID(PortalSettings.HomeDirectory)
                AddStyleSheet(ID, PortalSettings.HomeDirectory & "portal.css")
            End If
        End Sub

        Public Function CreateValidID(ByVal strID As String) As String

            Dim strBadCharacters As String = " ./-\"

            Dim intIndex As Integer
            For intIndex = 0 To strBadCharacters.Length - 1
                strID = strID.Replace(strBadCharacters.Substring(intIndex, 1), "_")
            Next

            Return strID

        End Function
    End Class
End Namespace
