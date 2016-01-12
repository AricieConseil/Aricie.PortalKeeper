Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Security.Cryptography


Namespace Security
End Namespace


Namespace Services.Files

   

    
    Public Class SmartFileInfo
        Implements IKeyPathFormatter

        <ExtendedCategory("Processing")> _
        Public Property Sign As Boolean

        <ConditionalVisible("Sign", False, True)> _
        <ExtendedCategory("Processing")> _
        Public Property EnforceSignature As Boolean

        <ExtendedCategory("Processing")> _
        Public Property Compress As Boolean

        <ExtendedCategory("Processing")> _
        Public Property Encrypt As Boolean

        <ConditionalVisible("Encrypt", False, True)> _
       <ExtendedCategory("Processing")> _
        Public Property EnforceEncryption As Boolean

        <ExtendedCategory("Keys")> _
        Public Property Encryption As New EncryptionInfo



        '<XmlIgnore()> _
        '<ExtendedCategory("UserBots")> _
        '<ConditionalVisible("EnableUserBots", False, True)> _
        '<Width(450)> _
        '<Editor(GetType(CustomTextEditControl), GetType(EditControl))>
        'Public Property EncryptionKey() As String
        '    Get
        '        Return _EncryptionKey
        '    End Get
        '    Set(ByVal value As String)
        '        _EncryptionKey = value
        '    End Set
        'End Property


        Public Function CheckSmartFile(objSmartFile As SmartFile) As Boolean
            Return (objSmartFile.Encrypted OrElse (Not Me.Encrypt) OrElse (Not Me.EnforceEncryption)) _
                AndAlso (objSmartFile.Signed OrElse (Not Me.Sign) OrElse (Not Me.EnforceSignature))
        End Function

        <ExtendedCategory("Storage")> _
        Public Property PathFormat As String = "{Application}/{Entity}/{Field}/{UserName}.xml"
        <ExtendedCategory("Storage")> _
        Public Property GrantUserView As Boolean
        <ExtendedCategory("Storage")> _
        Public Property GrantUserEdit As Boolean


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