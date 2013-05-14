Imports System.Xml
Imports System.ComponentModel
Imports Aricie.DNN.Configuration
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Errors

    ''' <summary>
    ''' Custom error holder class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class CustomErrorInfo
        Inherits XmlConfigElementInfo

        ''' <summary>
        ''' Status code
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property StatusCode() As Integer

        ''' <summary>
        ''' Url to which the custom error will redirect
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        Public Property Redirect() As new ControlUrlInfo
         

        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

        End Sub

        ''' <summary>
        ''' Checks whether the entry is present in the web.config file
        ''' </summary>
        ''' <param name="xmlConfig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Overrides Function IsInstalled(ByVal xmlConfig As System.Xml.XmlDocument) As Boolean
            Dim xpath As String = NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors/error[@statusCode='" & Me.StatusCode.ToString & "' and @redirect='" & Me.Redirect.UrlPath & "']"
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode(xpath)
            Return moduleNode IsNot Nothing
        End Function
    End Class
End Namespace


