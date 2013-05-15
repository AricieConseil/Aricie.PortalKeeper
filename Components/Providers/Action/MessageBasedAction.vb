Imports Aricie.DNN.Services.Filtering
Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public MustInherit Class MessageBasedAction(Of TEngineEvents As IConvertible)
        Inherits AsyncEnabledActionProvider(Of TEngineEvents)

        'Private _EnventMessage As String = "Alert from PortalKeeper generated with condition [Condition:Name] matching request [Context:CurrentIpAddress] and user [Context:DnnContext:User:Username]"
        Private _Message As String = ""
        Private _EnableTokenReplace As Boolean
        Private _AdditionalTokenSource As New TokenSourceInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal enableTokenReplace As Boolean)

        End Sub

        <ExtendedCategory("MessageSettings")> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(10)> _
            <Width(400)> _
        Public Property Message() As String
            Get
                Return _Message
            End Get
            Set(ByVal value As String)
                _Message = value
            End Set
        End Property

        <ExtendedCategory("MessageSettings")> _
        Public Property EnableTokenReplace() As Boolean
            Get
                Return _EnableTokenReplace
            End Get
            Set(ByVal value As Boolean)
                _EnableTokenReplace = value
            End Set
        End Property

        <ExtendedCategory("MessageSettings")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property AdditionalTokenSource() As TokenSourceInfo
            Get
                Return _AdditionalTokenSource
            End Get
            Set(ByVal value As TokenSourceInfo)
                _AdditionalTokenSource = value
            End Set
        End Property

        Protected Overrides Function GetAdvancedTokenReplace(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As Services.AdvancedTokenReplace
            Dim toReturn As AdvancedTokenReplace = MyBase.GetAdvancedTokenReplace(actionContext)
            Me._AdditionalTokenSource.SetTokens(toReturn)
            Return toReturn
        End Function

        Protected Function GetMessage(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As String

            Return Me.GetMessage(actionContext, Me._Message)

        End Function

        Protected Function GetMessage(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal template As String) As String

            If Me._EnableTokenReplace Then
                Return Me.GetAdvancedTokenReplace(actionContext).ReplaceAllTokens(template)
            Else
                Return template
            End If


        End Function


    End Class
End Namespace