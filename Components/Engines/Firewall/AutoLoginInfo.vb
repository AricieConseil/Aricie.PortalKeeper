
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports System.Xml.Serialization



Namespace Aricie.DNN.Modules.PortalKeeper

   
    
    Public Class AutoLoginInfo

        Public Sub New()

        End Sub

        Public Enum AutoLoginModeType
            Manual
            TicketAuth
        End Enum


        Private _AutoLoginUserName As String = String.Empty
        Private _AutoLoginMode As AutoLoginModeType = AutoLoginModeType.Manual
        Private _AutoLoginPassword As String = String.Empty
        Private _EncryptionKey As String = String.Empty
        Private _TicketAuthParamName As String = String.Empty

        Public Property AutoLoginMode() As AutoLoginModeType
            Get
                Return _AutoLoginMode
            End Get
            Set(ByVal value As AutoLoginModeType)
                _AutoLoginMode = value
            End Set
        End Property

        <ConditionalVisible("AutoLoginMode", False, True, AutoLoginModeType.Manual)> _
        Public Property AutoLoginUserName() As String
            Get
                Return _AutoLoginUserName.Trim
            End Get
            Set(ByVal value As String)
                _AutoLoginUserName = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property AutoLoginPassword() As String
            Get
                Return _AutoLoginPassword
            End Get
            Set(ByVal value As String)
                _AutoLoginPassword = value
            End Set
        End Property

        <ConditionalVisible("AutoLoginMode", False, True, AutoLoginModeType.Manual)> _
        <XmlIgnore()> _
        Public Property EditAutoLoginPassword() As String
            Get

                Dim toReturn As String = ""
                For Each objChar As Char In _AutoLoginPassword.Trim
                    toReturn &= "x"
                Next
                Return toReturn
            End Get
            Set(ByVal value As String)
                If value.Replace("x", "").Length > 0 Then
                    _AutoLoginPassword = value
                End If
            End Set
        End Property

        <ConditionalVisible("AutoLoginMode", False, True, AutoLoginModeType.TicketAuth)> _
        Public Property TicketAuthParamName() As String
            Get
                Return _TicketAuthParamName
            End Get
            Set(ByVal value As String)
                _TicketAuthParamName = value
            End Set
        End Property

        <ConditionalVisible("AutoLoginMode", False, True, AutoLoginModeType.TicketAuth)> _
        Public Property EncryptionKey() As String
            Get
                Return _EncryptionKey
            End Get
            Set(ByVal value As String)
                _EncryptionKey = value
            End Set
        End Property

    

      

    End Class

End Namespace