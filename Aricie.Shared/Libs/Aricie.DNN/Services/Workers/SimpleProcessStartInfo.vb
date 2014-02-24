Imports Aricie.DNN.Services.Files
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Services.Workers
    Public Class SimpleProcessStartInfo

        Public Property ProgramFileName As New FilePathInfo()

        Public Property Arguments As New SimpleOrExpression(Of String)("")


        Public Property UseCredentials As Boolean

        <ConditionalVisible("UseCredentials", False, True)>
        Public Property Credentials As New LoginInfo()

        <ConditionalVisible("UseCredentials", False, True)>
        Public Property Domain As String = ""


        Public Function GetProcessStartInfo(owner As Object, lookup As IContextLookup) As ProcessStartInfo
            Dim strProgramPath As String = ProgramFileName.GetMapPath(owner, lookup)

            Dim toReturn As New ProcessStartInfo(strProgramPath, Arguments.GetValue(owner, lookup))
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
            toReturn.WorkingDirectory = Environment.CurrentDirectory

            Return toReturn
        End Function

    End Class
End NameSpace