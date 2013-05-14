
Imports System.Xml.Serialization

Namespace Entities
    ''' <summary>
    ''' Simple entity class for a secure Rest service
    ''' </summary>
    <Serializable()> _
    Public Class FileUploadInfo

        Public Sub New()

        End Sub

        Public Sub New(objlogin As LoginInfo, objFileName As String, content As Byte())
            Me.Login = objlogin
            Me._FileName = objFileName
            Me._FileContent = content
        End Sub

        '<xmlignore()> _
        Public Property Login As New LoginInfo()

        Public Property FileName As String = ""

        Public Property FileContent As Byte()

    End Class
End Namespace