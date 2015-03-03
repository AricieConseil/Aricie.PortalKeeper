
Imports Aricie.DNN.Services
Imports DotNetNuke.Common.Globals
Imports System.IO

Namespace UI.WebControls
    Public Class HostContainerSelector
        Inherits SelectorControl(Of KeyValuePair(Of String, String))


        Public Overrides Function GetEntitiesG() As IList(Of KeyValuePair(Of String, String))
            Dim toReturn As New List(Of KeyValuePair(Of String, String))

            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strFile As String
            Dim arrFiles As String()
            Dim strLastFolder As String = ""
            Dim strSeparator As String = "----------------------------------------"

            strRoot = HostMapPath & "Containers"
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    If Not strFolder.EndsWith(glbHostSkinFolder) Then
                        arrFiles = Directory.GetFiles(strFolder, "*.ascx")
                        For Each strFile In arrFiles
                            strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                            If strLastFolder <> strFolder Then
                                If strLastFolder <> "" Then
                                    toReturn.Add(New KeyValuePair(Of String, String)(strSeparator, ""))
                                End If
                                strLastFolder = strFolder
                            End If
                            toReturn.Add(New KeyValuePair(Of String, String)(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[G]Containers" & "/" & strFolder & "/" & Path.GetFileName(strFile)))
                        Next
                    End If
                Next
            End If
            Return toReturn
        End Function

        Private Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
            If strSkinFolder.ToLower = "_default" Then
                ' host folder
                Return strSkinFile
            Else ' portal folder
                Select Case strSkinFile.ToLower
                    Case "skin", "container", "default"
                        Return strSkinFolder
                    Case Else
                        Return strSkinFolder & " - " & strSkinFile
                End Select
            End If
        End Function
    End Class


    Public Class PortalContainerSelector
        Inherits SelectorControl(Of KeyValuePair(Of String, String))


        Public Overrides Function GetEntitiesG() As IList(Of KeyValuePair(Of String, String))
            Dim toReturn As New List(Of KeyValuePair(Of String, String))

            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strFile As String
            Dim arrFiles As String()
            Dim strLastFolder As String = String.Empty
            Dim strSeparator As String = "----------------------------------------"
            strRoot = NukeHelper.PortalSettings.HomeDirectoryMapPath & "Containers"
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    arrFiles = Directory.GetFiles(strFolder, "*.ascx")
                    For Each strFile In arrFiles
                        strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                        If strLastFolder <> strFolder Then
                            If strLastFolder <> "" Then
                                toReturn.Add(New KeyValuePair(Of String, String)(strSeparator, ""))
                            End If
                            strLastFolder = strFolder
                        End If
                        toReturn.Add(New KeyValuePair(Of String, String)(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[L]Containers/" & strFolder & "/" & Path.GetFileName(strFile)))
                    Next
                Next
            End If
            Return toReturn
        End Function

        Private Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
            If strSkinFolder.ToLower = "_default" Then
                ' host folder
                Return strSkinFile
            Else ' portal folder
                Select Case strSkinFile.ToLower
                    Case "skin", "container", "default"
                        Return strSkinFolder
                    Case Else
                        Return strSkinFolder & " - " & strSkinFile
                End Select
            End If
        End Function
    End Class
End Namespace
