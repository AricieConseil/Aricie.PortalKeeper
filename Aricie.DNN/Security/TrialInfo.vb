Imports System.Xml.Serialization
Imports DotNetNuke.Security
Imports System.Globalization

Namespace Security.Trial
    ''' <summary>
    ''' Class giving information about the trial status
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class TrialInfo
        Private Shared _Separator As Char = "$"c
        Private _ExpirationDate As Date = Date.MinValue
        Private _Key As String = String.Empty
        Private _Value As String = String.Empty
        Private _Decrypted As Boolean = False


        ''' <summary>
        ''' Expiration date for the trial version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property ExpirationDate() As Date
            Get
                Return Me._ExpirationDate
            End Get
            Set(ByVal value As Date)
                Me._ExpirationDate = value
            End Set
        End Property

        ''' <summary>
        ''' Key for the trial version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property Key() As String
            Get
                Return Me._Key
            End Get
            Set(ByVal value As String)
                Me._Key = value
            End Set
        End Property

        ''' <summary>
        ''' Is the trail version decrypted
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property IsDecrypted() As Boolean
            Get
                Return Me._Decrypted
            End Get
            Set(ByVal value As Boolean)
                Me._Decrypted = value
            End Set
        End Property

        ''' <summary>
        ''' Value of the trial version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value() As String
            Get
                Return Me._Value
            End Get
            Set(ByVal value As String)
                Me._Value = value
            End Set
        End Property

        ''' <summary>
        ''' Encryption 
        ''' </summary>
        ''' <param name="encryptionKey"></param>
        ''' <remarks></remarks>
        Public Sub Encrypt(ByVal encryptionKey As String)
            Dim ps As New PortalSecurity
            Me._Value = _
                ps.Encrypt(encryptionKey, ExpirationDate.ToString(CultureInfo.InvariantCulture) & _Separator & Key)
        End Sub

        ''' <summary>
        ''' Decryption
        ''' </summary>
        ''' <param name="encryptionKey"></param>
        ''' <remarks></remarks>
        Public Sub Decrypt(ByVal encryptionKey As String)
            Dim ps As New PortalSecurity
            Dim decrypted As String() = ps.Decrypt(encryptionKey, Me._Value).Split(_Separator)
            If decrypted.Length = 2 Then
                Me.ExpirationDate = Date.Parse(decrypted(0), CultureInfo.InvariantCulture)
                Me.Key = decrypted(1)
                Me._Decrypted = True
            End If
        End Sub
    End Class
End Namespace