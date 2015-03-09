Imports System.Xml.Serialization

Namespace Configuration

    ''' <summary>
    ''' AppSettings Add merge node
    ''' </summary>
    Public Class AppSettingAddInfo
        Inherits AddInfoBase

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal key As String, ByVal value As String)
            Me.Attributes("key") = key
            Me.Attributes("value") = value
        End Sub

    End Class
End Namespace