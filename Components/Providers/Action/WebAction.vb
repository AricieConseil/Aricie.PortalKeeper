Imports System.ComponentModel
Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports System.Security.Cryptography
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports DotNetNuke.Services.Exceptions
Imports Aricie.DNN.Services.Flee
Imports System.Globalization
'Imports Jayrock.Json
'Imports Jayrock.JsonRpc
Imports System.Net
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Globe, IconOptions.Normal)> _
    <Serializable()> _
       <DisplayName("Web Action")> _
       <Description("Performs a web call to a web address")> _
    Public Class WebAction(Of TEngineEvents As IConvertible)
        Inherits OutputAction(Of TEngineEvents)

        Private _WebRequest As New WebRequestInfo(Of TEngineEvents)


        Private _InputParameters As New Variables(Of String)
        Private _HeaderParameters As New Variables(Of String)

        Private _IncludePostData As Boolean
        Private _PostDataVar As String = "postData"


        <ExtendedCategory("WebRequest")> _
        Public Property WebRequest() As WebRequestInfo(Of TEngineEvents)
            Get
                Return _WebRequest
            End Get
            Set(ByVal value As WebRequestInfo(Of TEngineEvents))
                _WebRequest = value
            End Set
        End Property


        <ExtendedCategory("Parameters")> _
            Public Property InputParameters() As Variables(Of String)
            Get
                Return _InputParameters
            End Get
            Set(ByVal value As Variables(Of String))
                _InputParameters = value
            End Set
        End Property

        <ExtendedCategory("Parameters")> _
          Public Property HeaderParameters() As Variables(Of String)
            Get
                Return _HeaderParameters
            End Get
            Set(ByVal value As Variables(Of String))
                _HeaderParameters = value
            End Set
        End Property

      
        <ExtendedCategory("Parameters")> _
        Public Property IncludePostData() As Boolean
            Get
                Return _IncludePostData
            End Get
            Set(ByVal value As Boolean)
                _IncludePostData = value
            End Set
        End Property

        <ExtendedCategory("Parameters")> _
        <ConditionalVisible("IncludePostData", False, True)> _
        Public Property PostDataVar() As String
            Get
                Return _PostDataVar
            End Get
            Set(ByVal value As String)
                _PostDataVar = value
            End Set
        End Property



        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
            Dim params As SerializableDictionary(Of String, String) = Me._InputParameters.EvaluateGeneric(actionContext, actionContext)
            Dim headersLookup As IContextLookup = actionContext
            If Me._IncludePostData Then
                Dim postData As String = Me._WebRequest.GetPostData(params)
                headersLookup = New SimpleContextLookup(actionContext)
                headersLookup.Items(Me._PostDataVar) = postData
            End If
            Dim headers As SerializableDictionary(Of String, String) = Me._HeaderParameters.EvaluateGeneric(actionContext, headersLookup)

            Return Me._WebRequest.Run(actionContext, params, headers)

        End Function


    End Class




End Namespace
