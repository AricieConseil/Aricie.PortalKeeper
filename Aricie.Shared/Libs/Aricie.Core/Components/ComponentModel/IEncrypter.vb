Imports System.Xml

Namespace ComponentModel

    Public Interface IEncrypter
        Function Encrypt(ByVal payload As String, ByRef salt As Byte()) As String
        Function Decrypt(ByVal payload As String, ByVal salt As Byte()) As String
        Sub Sign(doc As XmlDocument, ParamArray paths As String())
        Function Verify(signedDoc As XmlDocument) As Boolean
    End Interface



End Namespace