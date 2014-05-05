Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports System.IO
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Framework

Namespace Aricie.DNN.Modules.PortalKeeper





    <Serializable()> _
    Public Class FirewallSettings
        Inherits RuleEngineSettings(Of RequestEvent)




#Region "Private members"




        Private _RecoveryParam As String

        Private _RestartParam As String


        Private _RequestScope As RequestScope = RequestScope.DNNPageOnly

        Private _IgnoredExtensions As String = "jpg,jpeg,png,axd,js,css,gif,bmp"

        Private _IgnoredExtensionList As List(Of String)


        Friend cacheKey As String = Guid.NewGuid.ToString
#End Region

#Region "cTors"


        Public Sub New()
        End Sub


#End Region


#Region "Public Properties"

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Mode As RuleEngineMode
            Get
                Return RuleEngineMode.Rules
            End Get
            Set(value As RuleEngineMode)
                'do nothing
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <Browsable(False)> _
         <XmlIgnore()> _
        Public Overrides Property Decription As CData
            Get
                Return MyBase.Decription
            End Get
            Set(value As CData)
                MyBase.Decription = value
            End Set
        End Property


        <ExtendedCategory("")> _
        <Width(450)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RecoveryParam() As String
            Get
                If String.IsNullOrEmpty(_RecoveryParam) Then
                    _RecoveryParam = HttpUtility.UrlEncode(UserController.GeneratePassword(6))
                End If
                Return _RecoveryParam
            End Get
            Set(ByVal value As String)
                _RecoveryParam = value
            End Set
        End Property


        <ExtendedCategory("")> _
        <Width(450)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property RestartParam() As String
            Get
                If String.IsNullOrEmpty(_RestartParam) Then
                    _RestartParam = HttpUtility.UrlEncode(UserController.GeneratePassword(6))
                End If
                Return _RestartParam
            End Get
            Set(ByVal value As String)
                _RestartParam = value
            End Set
        End Property




        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("ShowTechnicalSettings", False, True, True)> _
        <SortOrder(1000)> _
        Public Property RequestScope() As RequestScope
            Get
                Return _RequestScope
            End Get
            Set(ByVal value As RequestScope)
                _RequestScope = value
            End Set
        End Property

        <ExtendedCategory("TechnicalSettings")> _
        <ConditionalVisible("ShowTechnicalSettings", False, True, True)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <LineCount(2)> _
        <Width(300)> _
        <SortOrder(1000)> _
        Public Property IgnoredExtensions() As String
            Get
                Return _IgnoredExtensions
            End Get
            Set(ByVal value As String)
                _IgnoredExtensions = value
            End Set
        End Property








        'comfort property
        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property IgnoredExtensionList() As List(Of String)
            Get
                If _IgnoredExtensionList Is Nothing Then
                    _IgnoredExtensionList = New List(Of String)
                    Dim strList As String() = Me._IgnoredExtensions.Trim.Trim(","c).Split(","c)
                    For Each strExt As String In strList
                        If Not String.IsNullOrEmpty(strExt) AndAlso Not _IgnoredExtensionList.Contains(strExt) Then
                            _IgnoredExtensionList.Add(strExt)
                        End If
                    Next
                End If
                Return Me._IgnoredExtensionList
            End Get
        End Property


#End Region


        Public Function RequestIsInScope(ByVal context As HttpContext) As Boolean
            If context.CurrentHandler IsNot Nothing Then
                Select Case Me.RequestScope
                    Case RequestScope.DNNPageOnly
                        Return TypeOf context.CurrentHandler Is CDefault
                    Case RequestScope.PagesOnly
                        Return TypeOf context.CurrentHandler Is Page
                End Select
            End If
            Dim fileExtension As String = DirectCast(context.Items("Extension"), String)
            If String.IsNullOrEmpty(fileExtension) Then
                fileExtension = Path.GetExtension(context.Request.FilePath)
                If Not String.IsNullOrEmpty(fileExtension) Then
                    fileExtension = fileExtension.TrimStart("."c)
                    context.Items("Extension") = fileExtension
                End If
            End If
            If Me.IgnoredExtensionList.Contains(fileExtension) Then
                Return False
            End If
            Return True
        End Function












    End Class
End Namespace
