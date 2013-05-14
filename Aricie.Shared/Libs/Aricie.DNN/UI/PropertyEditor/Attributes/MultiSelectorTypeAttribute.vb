
Namespace UI.Attributes

    Public Class MultiSelectorTypeAttribute
        Inherits Attribute

#Region "Constructor"

        Public Sub New(ByVal multiSelectorType As String)

            _multiSelectorType = multiSelectorType

        End Sub

        Public Sub New(ByVal multiSelectorType As String, ByVal exclusiveSelector As Boolean, ByVal invertExclusion As Boolean, ByVal exclusiveScopeControlId As String)

            _multiSelectorType = multiSelectorType
            _exclusiveSelector = exclusiveSelector
            _invertExclusion = invertExclusion
            _exclusiveScopeControlId = exclusiveScopeControlId

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property MultiSelectorType() As String
            Get
                Return _multiSelectorType
            End Get
        End Property

        Public ReadOnly Property ExclusiveSelector() As Boolean
            Get
                Return _exclusiveSelector
            End Get
        End Property

        Public ReadOnly Property InvertExclusion() As Boolean
            Get
                Return _invertExclusion
            End Get
        End Property

        Public ReadOnly Property ExclusiveScopeControlId() As String
            Get
                Return _exclusiveScopeControlId
            End Get
        End Property

#End Region

#Region "Fields"

        Private _multiSelectorType As String
        Private _exclusiveSelector As Boolean
        Private _invertExclusion As Boolean
        Private _exclusiveScopeControlId As String

#End Region

    End Class

End Namespace