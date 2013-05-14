Imports System.Xml.Serialization

Namespace Configuration

    ''' <summary>
    ''' Custom Error Add merge node
    ''' </summary>
    <XmlRoot("trust")> _
    <Serializable()> _
    Public Class TrustAddInfo
        Inherits AddInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal objTrust As TrustInfo)
            Me.Attributes("level") = objTrust.Level

            If Not String.IsNullOrEmpty(objTrust.OriginUrl) Then
                Me.Attributes("originUrl") = objTrust.OriginUrl
                'Else
                '    Me.Attributes("originUrl") = ".*"
            End If
        End Sub


    End Class
End Namespace