Imports System.Xml.Serialization

Namespace Configuration
    '<XmlRoot("add")> _
    
    Public Class CustomAddInfo
        Inherits AddInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal elementName As String)
            Me._ElementName = elementName
        End Sub


        Private _ElementName As String = ""


        Public Property ElementName() As String
            Get
                Return _ElementName
            End Get
            Set(ByVal value As String)
                _ElementName = value
            End Set
        End Property



    End Class
End NameSpace