Imports System.Xml
Imports Aricie.DNN.Services
Imports System.ComponentModel

Namespace Configuration

  
    ''' <summary>
    ''' This is the base element for an Xml based installable component
    ''' </summary>
    ''' <remarks>Uses xml merge files</remarks>
    <Serializable()> _
    Public MustInherit Class XmlConfigElementInfo
        Implements IConfigElementInfo

        Public MustOverride Overloads Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean


        Public MustOverride Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

        Public Overloads Function IsInstalled() As Boolean Implements IConfigElementInfo.IsInstalled
            Return IsInstalled(NukeHelper.WebConfigDocument)
        End Function


        Public Sub ProcessConfig(ByVal actionType As ConfigActionType) Implements IConfigElementInfo.ProcessConfig
            Throw New NotImplementedException("The config installer is supposed to intercept those and use the AddConfigNodes method instead")
        End Sub

    End Class
End Namespace


