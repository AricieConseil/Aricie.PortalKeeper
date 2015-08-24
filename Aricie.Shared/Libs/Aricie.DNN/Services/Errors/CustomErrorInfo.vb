Imports System.Xml
Imports System.ComponentModel
Imports System.Net
Imports Aricie.DNN.Configuration
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Globalization
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Services.Errors

    ''' <summary>
    ''' Custom error holder class
    ''' </summary>
    ''' <remarks></remarks>
    <ActionButton(IconName.ExclamationCircle, IconOptions.Normal)> _
   <DefaultProperty("FriendlyName")> _
    Public Class CustomErrorInfo
        Inherits XmlConfigElementInfo


        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                Return CInt(Me.Status).ToString() & UIConstants.TITLE_SEPERATOR & Me.Redirect.UrlPath
            End Get
        End Property


        'todo: remove/obsolete that property when not needed for xmlserialization anymore
        '<Obsolete()> _
        <Browsable(False)> _
        Public Property StatusCode() As Integer
            Get
                Return CInt(Status)
            End Get
            Set(value As Integer)
                Status = CType(value, HttpStatusCode)
            End Set
        End Property


        ''' <summary>
        ''' Status code
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status As HttpStatusCode = HttpStatusCode.NotFound

        ''' <summary>
        ''' Url to which the custom error will redirect
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        Public Property Redirect() As New ControlUrlInfo


        Public Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

        End Sub

        ''' <summary>
        ''' Checks whether the entry is present in the web.config file
        ''' </summary>
        ''' <param name="xmlConfig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Overrides Function IsInstalled(ByVal xmlConfig As System.Xml.XmlDocument) As Boolean
            Dim xpath As String = NukeHelper.DefineWebServerElementPath("system.web") & "/customErrors/error[@statusCode='" & CInt(Me.Status).ToString(CultureInfo.InvariantCulture) & "' and @redirect='" & Me.Redirect.UrlPath & "']"
            Dim moduleNode As XmlNode = xmlConfig.SelectSingleNode(xpath)
            Return moduleNode IsNot Nothing
        End Function
    End Class
End Namespace


