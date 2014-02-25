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


        Public Sub New()
        End Sub

        Public Sub New(ByVal enableTokenReplace As Boolean)
            Me.TokenizedText.EnableTokenReplace = enableTokenReplace
        End Sub

        <ExtendedCategory("MessageSettings")> _
        Public Overridable Property TokenizedText As New TokenizedTextInfo()




        'todo, make obsolete after migration of old parameters
        '<Obsolete("use Tokenized sub entity instead")> _
        <Browsable(False)>
        Public Property Message() As String
            Get
                Return Me.TokenizedText.Text
            End Get
            Set(value As String)
                Me.TokenizedText.Text = value
            End Set
        End Property



        <Browsable(False)>
        Public Property EnableTokenReplace() As Boolean
            Get
                Return Me.TokenizedText.EnableTokenReplace
            End Get
            Set(value As Boolean)
                Me.TokenizedText.EnableTokenReplace = value
            End Set
        End Property

        <Browsable(False)>
        Public Property AdditionalTokenSource() As TokenSourceInfo
            Get
                Return Me.TokenizedText.AdditionalTokenSource
            End Get
            Set(value As TokenSourceInfo)
                Me.TokenizedText.AdditionalTokenSource = value
            End Set
        End Property


        Protected Overrides Function GetAdvancedTokenReplace(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As AdvancedTokenReplace
            Dim toReturn As AdvancedTokenReplace = MyBase.GetAdvancedTokenReplace(actionContext)
            Me.TokenizedText.AdditionalTokenSource.SetTokens(toReturn)
            Return toReturn
        End Function

        Protected Function GetMessage(ByVal actionContext As PortalKeeperContext(Of TEngineEvents)) As String

            Return Me.GetMessage(actionContext, Me.TokenizedText.Text)

        End Function

        Protected Function GetMessage(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal template As String) As String
            If Me.TokenizedText.EnableTokenReplace Then
                Return Me.TokenizedText.GetText(Me.GetAdvancedTokenReplace(actionContext), template)
            Else
                Return template
            End If
        End Function


    End Class
End Namespace