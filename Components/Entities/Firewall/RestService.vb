Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class RestService
        Inherits NamedConfig



        Public Property ResourceType As New DotNetType

        Public Property AtUri As String = "/MyResource"


        Public Property AlternateUris As New List(Of String)


        Public Property HandlerType As New DotNetType

        Public Property AsJsonDataContract As Boolean = True

        Public Property AsXmlDataContract As Boolean = True

        Public Property AsXmlSerializer As Boolean = True


    End Class
End Namespace