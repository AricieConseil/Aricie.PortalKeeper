Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Cog, IconOptions.Normal)> _
    <DefaultProperty("FriendlyName")> _
    <Serializable()> _
    Public Class UserParameterWrapper

        Public Sub New()

        End Sub

        Private _Entities As Dictionary(Of String, Object)
        Private _Key As String

        Public Sub New(objTitle As String, objDescription As String, objEntities As Dictionary(Of String, Object), objKey As String)
            Me.Title = objTitle
            Me.Description = objDescription
            Me._Entities = objEntities
            Me._Key = objKey
            'Me.Instance = objInstance
        End Sub

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                Dim toreturn As String = Me.Title
                If Instance IsNot Nothing Then
                    Dim strFriendlyName As String = ReflectionHelper.GetFriendlyName(Instance)
                    If strFriendlyName.Length < 50 AndAlso Not strFriendlyName.StartsWith(ReflectionHelper.GetSimpleTypeName(Instance.GetType)) Then
                        toreturn &= UIConstants.TITLE_SEPERATOR & strFriendlyName
                    End If
                End If
                Return toreturn
            End Get
        End Property


        <IsReadOnly(True)> _
        Public Property Title As String

        <IsReadOnly(True)> _
        Public Property Description As String


        Public Property Instance As Object
            Get
                Return _Entities(_Key)
            End Get
            Set(value As Object)
                _Entities(_Key) = value
            End Set
        End Property

    End Class
End Namespace