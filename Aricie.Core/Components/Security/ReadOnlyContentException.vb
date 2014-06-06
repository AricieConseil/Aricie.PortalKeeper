Imports System.Runtime.Serialization

Namespace Security
    <Serializable> _
    Friend Class ReadOnlyContentException
        Inherits Exception
        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
        End Sub

        ' A constructor is needed for serialization when an 
        ' exception propagates from a remoting server to the client.  
        Protected Sub New(info As SerializationInfo, context As StreamingContext)
        End Sub
    End Class
End NameSpace