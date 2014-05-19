Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.Net
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Globe, IconOptions.Normal)> _
    <Serializable()> _
    Public Class RestService
        Inherits NamedConfig



        Public Property ResourceType As New DotNetType

        Public Property AtUri As String = "/MyResource"

        <CollectionEditor(DisplayStyle:=CollectionDisplayStyle.List, EnableExport:=False)> _
        Public Property AlternateUris As New List(Of String)

        Public Property RestHandlerType As RestHandlerType

        <ConditionalVisible("RestHandlerType", False, True, RestHandlerType.CustomHandler)> _
        Public Property HandlerType As New DotNetType

        <ConditionalVisible("RestHandlerType", False, True, RestHandlerType.DynamicHandler)> _
        Public Property DynamicMethods() As New List(Of DynamicRestMethod)


        Public Property AsJsonDataContract As Boolean = True

        Public Property AsXmlDataContract As Boolean = True

        Public Property AsXmlSerializer As Boolean = True


    End Class
End Namespace