Imports System.Globalization

Namespace Security.Trial
    ''' <summary>
    ''' Information about trial configuration
    ''' </summary>
    ''' <remarks></remarks>
    <CLSCompliant(True), Serializable()> _
    Public Class TrialConfigInfo

        Private _IsTrial As Boolean = True
        Private _ModuleName As String = String.Empty
        Private _EncryptionKey As String = String.Empty
        Private _Limitation As TrialLimitation
        Private _Duration As Integer = 30
        Private _NbMaxInstances As Integer = Integer.MaxValue

        Public Sub New()

        End Sub


        Public Sub New(ByVal isTrial As Boolean, ByVal moduleName As String, ByVal limitations As TrialLimitation, _
                       ByVal encryptionKey As String, ByVal duration As Integer)
            Me.New(isTrial, moduleName, limitations, encryptionKey, duration, 0)
        End Sub


        Public Sub New(ByVal isTrial As Boolean, ByVal moduleName As String, ByVal limitations As TrialLimitation, _
                       ByVal encryptionKey As String, ByVal duration As Integer, ByVal nbMaxInstances As Integer)
            Me._IsTrial = isTrial
            Me._ModuleName = moduleName
            Me._Limitation = limitations
            Me._EncryptionKey = encryptionKey
            Me._Duration = duration
            Me._NbMaxInstances = nbMaxInstances

        End Sub


        Public Property IsTrial() As Boolean
            Get
                Return Me._IsTrial
            End Get
            Set(ByVal value As Boolean)
                Me._IsTrial = value
            End Set
        End Property

        Public Property ModuleName() As String
            Get
                Return _ModuleName
            End Get
            Set(ByVal value As String)
                _ModuleName = value
            End Set
        End Property

        Public Property EncryptionKey() As String
            Get
                Return _EncryptionKey
            End Get
            Set(ByVal value As String)
                _EncryptionKey = value
            End Set
        End Property

        Public Property Limitation() As TrialLimitation
            Get
                Return Me._Limitation
            End Get
            Set(ByVal value As TrialLimitation)
                Me._Limitation = value
            End Set
        End Property

        Public Property Duration() As Integer
            Get
                Return Me._Duration
            End Get
            Set(ByVal value As Integer)
                Me._Duration = value
            End Set
        End Property


        Public Property NbMaxInstances() As Integer
            Get
                Return Me._NbMaxInstances
            End Get
            Set(ByVal value As Integer)
                Me._NbMaxInstances = value
            End Set
        End Property


        Public Overrides Function GetHashCode() As Integer
            Return (Me.ModuleName & Me.Limitation.ToString & Me.EncryptionKey & Me.NbMaxInstances.ToString(CultureInfo.InvariantCulture)).GetHashCode
        End Function

        Public Shared Empty As New TrialConfigInfo(False, "", TrialLimitation.Expiration, "", 30)

    End Class
End Namespace