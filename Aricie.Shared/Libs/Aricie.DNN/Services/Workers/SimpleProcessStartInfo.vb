Imports Aricie.DNN.Services.Files
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Services.Workers

    Public  Enum  WorkingDirectoryMode
        ProgramDirectory
        CurrentDirectory
        CustomDirectory
    End Enum

    Public Class SimpleProcessStartInfo

        Public Property ProgramFileName As New FilePathInfo()

        Public Property CommandArguments As New EnabledFeature(Of SimpleOrExpression(Of String))(new SimpleOrExpression(Of String)(""))
        'Public Property Arguments As New SimpleOrExpression(Of String)("")


        Public Property UseCredentials As Boolean

        <ConditionalVisible("UseCredentials", False, True)>
        Public Property Credentials As New LoginInfo()

        <ConditionalVisible("UseCredentials", False, True)>
        Public Property Domain As String = ""

        Public  Property WorkingDirectoryMode As WorkingDirectoryMode

        <ConditionalVisible("WorkingDirectoryMode", False, True, WorkingDirectoryMode.CustomDirectory)> _
        Public  Property CustomWorkingDirectory() As New PathInfo()


        Public Function GetProcessStartInfo(owner As Object, lookup As IContextLookup) As ProcessStartInfo
            Dim strProgramPath As String = ProgramFileName.GetMapPath(owner, lookup)

            dim args as String = ""
            if CommandArguments.Enabled
                args = CommandArguments.Entity.GetValue(owner, lookup)
            End If
            Dim toReturn As New ProcessStartInfo(strProgramPath, args)
            If UseCredentials Then
                toReturn.UserName = Credentials.UserName
                toReturn.Password = Credentials.GetPasswordAsSecureString()
                If Not String.IsNullOrEmpty(Domain) Then
                    toReturn.Domain = Domain
                End If
            End If
            toReturn.UseShellExecute = False
            toReturn.RedirectStandardInput = True
            toReturn.RedirectStandardOutput = True
            toReturn.CreateNoWindow = True
            Select Case WorkingDirectoryMode
                Case WorkingDirectoryMode.CurrentDirectory
                    toReturn.WorkingDirectory = Environment.CurrentDirectory
                Case WorkingDirectoryMode.ProgramDirectory
                    Dim programDir as String = System.IO.Path.GetDirectoryName( strProgramPath)
                    toReturn.WorkingDirectory = programDir
                Case WorkingDirectoryMode.ProgramDirectory
                    Dim customDir as String = CustomWorkingDirectory.GetMapPath(owner, lookup)
                    toReturn.WorkingDirectory = customDir
            End Select
            Return toReturn
        End Function

    End Class
End NameSpace