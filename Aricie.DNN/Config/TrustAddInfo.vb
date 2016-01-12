Imports System.Xml.Serialization

Namespace Configuration
    ''<XmlType("trust")> _
    ''' <summary>
    ''' Custom Error Add merge node
    ''' </summary>
    <XmlRoot("trust")> _
    Public Class TrustAddInfo
        Inherits AddInfoBase

        Public Sub New()

        End Sub

        Public Sub New(ByVal objTrust As TrustInfo)
            Me.Attributes("level") = objTrust.Level

            If Not String.IsNullOrEmpty(objTrust.OriginUrl) Then
                Me.Attributes("originUrl") = objTrust.OriginUrl
            End If
        End Sub


    End Class
End Namespace