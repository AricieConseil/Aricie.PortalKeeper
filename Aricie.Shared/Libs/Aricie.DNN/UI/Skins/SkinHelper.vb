Imports DotNetNuke.Entities.Portals
Imports System.IO
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Common.Globals

Namespace UI.Skin

    Public Module SkinHelper

        Public Function GetDefaultPath(ps As PortalSettings) As String
            Dim RawSkinPath As String = GetDefaultSkin(ps)
            If Not String.IsNullOrEmpty(RawSkinPath) Then
                Dim strSkinPath = RawSkinPath.Substring(3, RawSkinPath.LastIndexOf("/") - 2)
                Dim strPortalFolderSkin As String = String.Empty
                If RawSkinPath.StartsWith("[G]") Then
                    strPortalFolderSkin = HostMapPath & strSkinPath
                ElseIf RawSkinPath.StartsWith("[L]") Then
                    strPortalFolderSkin = ps.HomeDirectoryMapPath & strSkinPath
                End If
                Return strPortalFolderSkin
            End If
            Return Nothing
        End Function

        Public Function GetDefaultSkin(ps As PortalSettings) As String
            Dim SkinPathRetriever As IPortalSkinPathRetriever = Nothing
            If Services.NukeHelper.DnnVersion.Major >= 5 Then
                SkinPathRetriever = New DNN5PortalSkinRetriever
            Else
                SkinPathRetriever = New DNN4PortalSkinRetriever()
            End If

            If (SkinPathRetriever IsNot Nothing) Then
                Return SkinPathRetriever.GetPath(ps)
            End If

            Return Nothing
        End Function

        Public Function GetSkinFilesList(ByVal portalId As Integer, ByVal portalSettings As PortalSettings) As ArrayList
           
            Dim toReturn As New ArrayList

            Dim strPortalFolderSkin As String = GetDefaultPath(portalSettings)
            If (Not String.IsNullOrEmpty(strPortalFolderSkin)) Then
              
                If Directory.Exists(strPortalFolderSkin) Then
                    Dim arrFiles As String() = Directory.GetFiles(strPortalFolderSkin, "*.ascx")
                    For Each _file As String In arrFiles
                        toReturn.Add(Path.GetFileNameWithoutExtension(_file))
                    Next
                End If
            End If

            Return toReturn
        End Function

        Interface IPortalSkinPathRetriever
            Function GetPath(ps As PortalSettings) As String
        End Interface

        Class DNN4PortalSkinRetriever
            Implements IPortalSkinPathRetriever

            Public Function GetPath(ps As DotNetNuke.Entities.Portals.PortalSettings) As String Implements IPortalSkinPathRetriever.GetPath
                Dim portalSkin As SkinInfo = ps.PortalSkin
                If portalSkin IsNot Nothing Then
                    Return portalSkin.SkinSrc
                End If
                Return Nothing
            End Function
        End Class

        Class DNN5PortalSkinRetriever
            Implements IPortalSkinPathRetriever

            Public Function GetPath(ps As DotNetNuke.Entities.Portals.PortalSettings) As String Implements IPortalSkinPathRetriever.GetPath
                Return DirectCast(Aricie.Services.ReflectionHelper.GetProperty(ps.GetType, "DefaultPortalSkin", ps), String)
            End Function
        End Class

    End Module

End Namespace