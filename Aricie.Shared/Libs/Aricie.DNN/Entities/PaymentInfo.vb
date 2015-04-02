Imports System.Xml.Serialization
Imports System.Web
Imports System.Web.HttpContext
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities
Imports DotNetNuke.Entities.Users


Namespace Entities


    <Serializable()> _
    Public Class PaymentInfo

        Private _Amount As Integer
        <IsReadOnly(True)> _
        Public Property Amount() As Integer
            Get
                Return Me._Amount
            End Get
            Set(value As Integer)
                Me._Amount = value
            End Set
        End Property

        Private _TransectionHash As String = ""
        <IsReadOnly(True)> _
        Public Property TransectionHash() As String
            Get
                Return Me._TransectionHash
            End Get
            Set(value As String)
                Me._TransectionHash = value
            End Set
        End Property

        Private _Confirmation As Integer
        <IsReadOnly(True)> _
        Public Property Confirmation() As Integer
            Get
                Return Me._Confirmation
            End Get
            Set(value As Integer)
                Me._Confirmation = value
            End Set
        End Property

        Private _DestinationAddresse As String = ""
        <Browsable(False)> _
        Public Property DestinationAddresse() As String
            Get
                Return Me._DestinationAddresse
            End Get
            Set(value As String)
                Me._DestinationAddresse = value
            End Set
        End Property

        Public Property InputAddresse As String = ""

        Private _Enable As Boolean
        <Browsable(False)> _
        <XmlIgnore()> _
        Public Property Enable() As Boolean
            Get
                Return Me._Enable
            End Get
            Set(value As Boolean)
                Me._Enable = value
            End Set
        End Property

        Public Property InputTransection As String = ""

        Private _PaymentDate As Date
        <IsReadOnly(True)> _
        Public Property PaymentDate() As Date
            Get
                Return Me._PaymentDate
            End Get
            Set(value As Date)
                Me._PaymentDate = value
            End Set
        End Property

        Private _UserInfo As UserInfo
        <Browsable(False)> _
        Public Property Userinfo() As UserInfo
            Get
                Return Me._UserInfo
            End Get
            Set(value As UserInfo)
                Me._UserInfo = value
            End Set
        End Property

        Public Property PID As Integer = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.PortalId

        Public Sub New()

        End Sub

        Public Sub New(context As HttpContext, request As HttpRequest)
            Amount = Integer.Parse(request.QueryString("vaule"))
            InputAddresse = request.QueryString("input_address")
            TransectionHash = request.QueryString("transection_hash")
            DestinationAddresse = request.QueryString("destination_address")
            Confirmation = Integer.Parse(request.QueryString("confirmations"))
            InputTransection = request.QueryString("input_transaction_hash")

            Userinfo = DotNetNuke.Entities.Users.UserController.GetUser(PID, Integer.Parse(context.Request.QueryString("userid")), False)
        End Sub

    End Class
End Namespace