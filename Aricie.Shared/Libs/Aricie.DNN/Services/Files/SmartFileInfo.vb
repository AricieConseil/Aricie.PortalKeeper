Imports Aricie.DNN.ComponentModel

Namespace Services.Files
    <Serializable()> _
    Public Class SmartFileInfo
        Implements IKeyPathFormatter
        Public Overridable Property Sign As Boolean
        Public Overridable Property Compress As Boolean
        Public Overridable Property Encrypt As Boolean
        Public Overridable Property PathFormat As String = "{0}/{1}/{2}/{3}"
        Public Overridable Property GrantUserView As Boolean
        Public Overridable Property GrantUserEdit As Boolean

        Public Function GetPath(key As EntityKey) As String Implements IKeyPathFormatter.GetPath
            Return String.Format(Me.PathFormat, key.Application, key.Entity, key.Field, key.UserName).Replace("//", "/"c).Replace("..", "."c) & ".Config"
        End Function

    End Class
End Namespace