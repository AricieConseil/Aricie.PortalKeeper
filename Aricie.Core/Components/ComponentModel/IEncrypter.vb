Imports System.Xml

Namespace ComponentModel

    Public Interface IEncrypter
        Function Encrypt(ByVal payload As Byte(), ByRef salt As Byte()) As Byte()
        Function Decrypt(ByVal payload As Byte(), ByVal salt As Byte()) As Byte()
        Sub Sign(ByRef doc As XmlDocument, ParamArray paths As String())
        Function Verify(ByVal signedDoc As XmlDocument) As Boolean
    End Interface



End Namespace