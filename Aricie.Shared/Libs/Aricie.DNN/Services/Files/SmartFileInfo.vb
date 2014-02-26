Imports Aricie.DNN.ComponentModel

Namespace Services.Files
    <Serializable()> _
    Public Class SmartFileInfo
        Implements IKeyPathFormatter
        Public Overridable Property Sign As Boolean
        Public Overridable Property Compress As Boolean
        Public Overridable Property Encrypt As Boolean
        Public Overridable Property PathFormat As String = "{Application}/{Entity}/{Field}/{UserName}.xml"
        Public Overridable Property GrantUserView As Boolean
        Public Overridable Property GrantUserEdit As Boolean


        Public Function GetFolderPath(key As EntityKey) As String
            Return System.IO.Path.GetDirectoryName(GetPath(key)).Replace("\", "/")
        End Function

        Private Shared _ProcessedFormats As New Dictionary(Of String, String)

        Private Function GetProcessedFormt() As String
            Dim toReturn As String = Nothing
            If Not _ProcessedFormats.TryGetValue(Me.PathFormat, toReturn) Then
                toReturn = Me.PathFormat.Replace("Application", "0").Replace("Entity", "1").Replace("Field", "2").Replace("UserName", "3")
                SyncLock _ProcessedFormats
                    _ProcessedFormats(PathFormat) = toReturn
                End SyncLock
            End If
            Return toReturn
        End Function

        Public Function GetPath(key As EntityKey) As String Implements IKeyPathFormatter.GetPath
            Return String.Format(GetProcessedFormt(), key.Application, key.Entity, key.Field, key.UserName).Replace("//", "/"c).Replace("..", "."c)
        End Function

    End Class
End Namespace