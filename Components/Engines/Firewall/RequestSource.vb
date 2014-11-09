Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports System.Net
Imports Aricie.Text

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class RequestSource

        Public Sub New()

        End Sub

        Public Sub New(ByVal requestSourceType As RequestSourceType)
            Me._SourceType = requestSourceType
        End Sub

        Private _SourceType As RequestSourceType
        Private _ExpressionFilter As New ExpressionFilterInfo(-1, False, EncodeProcessing.None, DefaultTransforms.None)

        <NonSerialized()> _
        Private _Escaper As StringEscaper

        <ExtendedCategory("")> _
        <MainCategory()> _
        <AutoPostBack()> _
        Public Property SourceType() As RequestSourceType
            Get
                Return _SourceType
            End Get
            Set(ByVal value As RequestSourceType)
                _SourceType = value
            End Set
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("")> _
        <LabelMode(LabelMode.Top)> _
       Public ReadOnly Property CurrentValue() As String
            Get
                Return Me.GenerateKey(PortalKeeperContext(Of RequestEvent).Instance)
            End Get
        End Property

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
           <Category("FilterSettings")> _
       Public Property ExpressionFilter() As ExpressionFilterInfo
            Get
                Return _ExpressionFilter
            End Get
            Set(ByVal value As ExpressionFilterInfo)
                _ExpressionFilter = value
            End Set
        End Property



        'Private ReadOnly Property Escaper() As StringEscaper
        '    Get
        '        If _Escaper Is Nothing Then
        '            _Escaper = New StringEscaper(Me._ExpressionFilter)
        '        End If
        '        Return _Escaper
        '    End Get
        'End Property

        Public Function GenerateKey(Of TEngineEvent As IConvertible)(ByVal context As PortalKeeperContext(Of TEngineEvent)) As String
            Dim toReturn As String = ""
            Select Case Me.SourceType
                Case RequestSourceType.Country
                    toReturn = context.DnnContext.CountryName
                Case RequestSourceType.IPAddress
                    toReturn = context.DnnContext.Request.UserHostAddress
                Case RequestSourceType.XForwardedIP
                    Dim objIp As IPAddress = context.DnnContext.IPAddress
                    If objIp IsNot Nothing Then
                        toReturn = objIp.ToString
                    Else
                        toReturn = context.DnnContext.Request.UserHostAddress
                    End If
                Case RequestSourceType.Session
                    Dim curSession As HttpSessionState = context.DnnContext.Session
                    If curSession IsNot Nothing Then
                        If curSession.IsNewSession AndAlso curSession("dumbData") Is Nothing Then
                            'this is to force the sessionid to be saved
                            curSession("dumbData") = True
                        End If
                        toReturn = curSession.SessionID
                    Else
                        toReturn = context.DnnContext.Request.UserHostAddress
                    End If
                Case RequestSourceType.UrlPath
                    toReturn = context.DnnContext.Request.Url.AbsolutePath
                Case RequestSourceType.Url
                    toReturn = context.DnnContext.Request.RawUrl
                Case Else
                    toReturn = String.Empty
            End Select
            Return ExpressionFilter.Process(toReturn)
        End Function

    End Class


    <Flags()> _
    Public Enum RequestSourceType
        Any = 0
        Country = 1
        IPAddress = 2
        Session = 4
        UrlPath = 8
        Url = 16
        XForwardedIP = 32
    End Enum


End Namespace