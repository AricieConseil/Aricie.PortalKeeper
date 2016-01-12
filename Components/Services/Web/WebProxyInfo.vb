Imports System.Net
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Road, IconOptions.Normal)>
    <DefaultProperty("Address")>
    Public Class WebProxyInfo


        Private _Address As String = String.Empty

        Private _UseCredentials As Boolean

        Private _Credentials As New LoginInfo



        Private _Proxy As WebProxy

        Private _Disabled As Boolean
        Private _AvailableNb As Integer

        Private _Lag As TimeSpan
        Private _LastTestDate As DateTime = Date.MinValue


        Public Sub New()

        End Sub

        Public Sub New(ByVal strAddress As String)
            Me._Address = strAddress
        End Sub


        Public ReadOnly Property Available() As Boolean
            Get
                Return AvailableNb > 0 AndAlso Not _Disabled
            End Get
        End Property

        <Required(True)>
        <Editor(GetType(CustomTextEditControl), GetType(EditControl)),
            LineCount(2), Width(500)>
        Public Property Address() As String
            Get
                Return _Address
            End Get
            Set(ByVal value As String)
                _Address = value.ToLower.Trim
                Me._Proxy = Nothing
            End Set
        End Property


        Public Property VirtualProxy As Boolean



        Public Property UseCredentials() As Boolean
            Get
                Return _UseCredentials
            End Get
            Set(ByVal value As Boolean)
                _UseCredentials = value
            End Set
        End Property


        <ConditionalVisible("UseCredentials", False, True)>
        Public Property Credentials() As LoginInfo
            Get
                Return _Credentials
            End Get
            Set(ByVal value As LoginInfo)
                _Credentials = value
            End Set
        End Property



        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal value As Boolean)
                _Disabled = value
            End Set
        End Property


        <IsReadOnly(True)>
        Public Property LastTestDate() As DateTime
            Get
                Return _LastTestDate
            End Get
            Set(ByVal value As DateTime)
                _LastTestDate = value
            End Set
        End Property

        <IsReadOnly(True)>
        Public Property Lag() As STimeSpan
            Get
                Return New STimeSpan(_Lag)
            End Get
            Set(ByVal value As STimeSpan)
                _Lag = value.Value
            End Set
        End Property



        <Browsable(False)>
        <XmlIgnore()>
        Public ReadOnly Property Proxy() As WebProxy
            Get
                If _Proxy Is Nothing AndAlso Not String.IsNullOrEmpty(Me._Address) Then
                    _Proxy = New WebProxy(Me._Address)
                End If
                Return _Proxy
            End Get
        End Property

        <IsReadOnly(True)>
        Public Property AvailableNb() As Integer
            Get
                Return _AvailableNb
            End Get
            Set(ByVal value As Integer)
                _AvailableNb = value
            End Set
        End Property

        <IsReadOnly(True)>
        Public Property CheckEnqueued As Boolean
            Get
                Return _CheckEnqueued
            End Get
            Set(value As Boolean)
                _CheckEnqueued = value
            End Set
        End Property

        Private _CheckEnqueued As Boolean



        Private Shared availableLock As New Object

        Public Function NeedsChecking(ByVal maxAge As TimeSpan) As Boolean
            Dim oldFactor As Long
            If AvailableNb < 0 Then
                Math.DivRem(Now.Ticks, 2 * AvailableNb, oldFactor)
            End If

            If oldFactor = 0 Then
                Return Now.Subtract(LastTestDate) > maxAge
            End If
            Return False
        End Function

        Public Sub CheckAvailibility(ByVal timeout As TimeSpan, ByVal testUri As String, responseTestPredicate As Predicate(Of String))
            SyncLock availableLock
                If (Not Me._Disabled) AndAlso (Not String.IsNullOrEmpty(Me._Address)) Then
                    Dim objClient As WebClient = CompressionEnabledWebClient.GetWebClient(WebMethod.Get.ToString, timeout)
                    objClient.Proxy = Me.Proxy
                    Dim startTest As DateTime = Now
                    Try
                        Dim testString As String = objClient.DownloadString(testUri)
                        _Lag = Now.Subtract(startTest)
                        If _Lag < timeout Then
                            If responseTestPredicate.Invoke(testString) Then
                                AvailableNb = Math.Min(1, AvailableNb \ 2 + 1)
                            Else
                                AvailableNb -= 3
                            End If
                        Else
                            AvailableNb -= 1
                        End If
                    Catch ex As Exception
                        AvailableNb -= 1
                        _Lag = TimeSpan.MaxValue
                    End Try
                End If
                _LastTestDate = Now
            End SyncLock
        End Sub

    End Class
End Namespace