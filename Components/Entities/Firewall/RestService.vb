Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Class DynamicRestHandlerConfig

        Public EntityType As New DotNetType



    End Class


    Public Enum RestServiceType
        CustomHandler
        GenericHandler
    End Enum

    <Serializable()> _
    Public Class RestService
        Inherits NamedConfig



        Public Property ResourceType As New DotNetType

        Public Property AtUri As String = "/MyResource"


        Public Property AlternateUris As New List(Of String)

        Public Property ServiceType As RestServiceType

        <ConditionalVisible("ServiceType", False, True, RestServiceType.CustomHandler)> _
        Public Property HandlerType As New DotNetType




        Public Property AsJsonDataContract As Boolean = True

        Public Property AsXmlDataContract As Boolean = True

        Public Property AsXmlSerializer As Boolean = True


    End Class
End Namespace