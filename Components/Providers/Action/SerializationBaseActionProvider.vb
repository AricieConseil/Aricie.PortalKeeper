
Imports Aricie.DNN.Services
Imports System.IO
Imports System.IO.Compression
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services
Imports System.Xml
Imports System.Globalization
Imports Newtonsoft.Json

Namespace Aricie.DNN.Modules.PortalKeeper

    

    <Serializable()> _
    Public MustInherit Class SerializationBaseActionProvider(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)


       


        <ExtendedCategory("Serialization")> _
        Public Property SerializationType() As SerializationType
       

        <ExtendedCategory("Serialization")> _
        Public Property UseCompression() As Boolean

        <ConditionalVisible("SerializationType", False, True, SerializationType.FileHelpers)> _
        <ExtendedCategory("Serialization")> _
        Public Property FileHelpersSettings As New FileHelpersSettings

       

       






    End Class


End Namespace