Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Key, IconOptions.Normal)> _
  <Serializable()> _
      <DisplayName("Membership Condition")> _
      <Description("Matches according to the user membership")> _
    Public Class MembershipCondition
        Inherits MembershipCondition(Of RequestEvent)

    End Class


    <ActionButton(IconName.Key, IconOptions.Normal)> _
   <Serializable()> _
       <DisplayName("Membership Condition")> _
       <Description("Matches according to the user membership")> _
    Public Class MembershipCondition(Of TEngineEvents As IConvertible)
        Inherits ConditionProvider(Of TEngineEvents)

        Private _MatchSuperUsers As Boolean

        Private _MatchRoles As String = ""

        Private _MatchUserIds As String = ""

        <ExtendedCategory("Specifics")> _
        Public Property MatchSuperUsers() As Boolean
            Get
                Return _MatchSuperUsers
            End Get
            Set(ByVal value As Boolean)
                _MatchSuperUsers = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(2)> _
            <Width(200)> _
        Public Property MatchRoles() As String
            Get
                Return _MatchRoles
            End Get
            Set(ByVal value As String)
                _MatchRoles = value
            End Set
        End Property

        <ExtendedCategory("Specifics")> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(2)> _
            <Width(200)> _
        Public Property MatchUserIds() As String
            Get
                Return _MatchUserIds
            End Get
            Set(ByVal value As String)
                _MatchUserIds = value
            End Set
        End Property




        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            If Me.DebuggerBreak Then
                CallDebuggerBreak()
            End If
            Dim objUser As UserInfo = context.DnnContext.User
            context.Items("ClientUser") = objUser
            If Me._MatchSuperUsers AndAlso objUser.IsSuperUser Then
                Return True
            End If
            If Me._MatchRoles <> "" AndAlso objUser.IsInRole(Me._MatchRoles) AndAlso Not objUser.IsSuperUser Then
                Return True
            End If
            If Me._MatchUserIds <> "" Then
                Dim userIds As String() = Me._MatchUserIds.Split(";"c)
                Dim userId As Integer
                For Each strUserId As String In userIds
                    userId = Integer.Parse(strUserId)
                    If userId = objUser.UserID Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

    End Class
End Namespace