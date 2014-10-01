Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace Services.Filtering
    <Serializable()> _
    Public Class TokenizedTextInfo

        <Obsolete("Use Template property")> _
        <Browsable(False)> _
        Public Property Text() As String
            Get
                Return Nothing
            End Get
            Set(value As String)
                Me.Template.Value = value
            End Set
        End Property

        Public Property Template As New CData("")


        Public Property EnableTokenReplace() As Boolean

        <ConditionalVisible("EnableTokenReplace", False, True)> _
        Public Property AddNewTokenSources As Boolean

        <ConditionalVisible("AddNewTokenSources", False, True)> _
        <ConditionalVisible("EnableTokenReplace", False, True)> _
        Public Property AdditionalTokenSource() As TokenSourceInfo = New TokenSourceInfo()

        Public Overloads Function GetText() As String
            Return GetText(Nothing)
        End Function


        Public Overloads Function GetText(atr As AdvancedTokenReplace) As String

            Return GetText(atr, Template)

        End Function

        Public Overloads Function GetText(atr As AdvancedTokenReplace, strtemplate As String) As String

            Return GetText(atr, strtemplate, Nothing, Nothing)

        End Function

        Public Overloads Function GetText(atr As AdvancedTokenReplace, owner As Object, globalVars As IContextLookup) As String

            Return GetText(atr, Template, owner, globalVars)

        End Function

        Public Overloads Function GetText(atr As AdvancedTokenReplace, strtemplate As String, owner As Object, globalVars As IContextLookup) As String

            If Me.EnableTokenReplace Then
                PrepareTokenReplace(atr, owner, globalVars)
                Return atr.ReplaceAllTokens(strtemplate)
            Else
                Return strtemplate
            End If

        End Function

        Public Sub PrepareTokenReplace(ByRef atr As AdvancedTokenReplace)
            PrepareTokenReplace(atr, Nothing, Nothing)
        End Sub

        Public Sub PrepareTokenReplace(ByRef atr As AdvancedTokenReplace, owner As Object, globalVars As IContextLookup)
            If Me.EnableTokenReplace Then
                If atr Is Nothing Then
                    atr = New AdvancedTokenReplace()
                End If
                If Not atr.IsSet(Me.AdditionalTokenSource) Then
                    Me.AdditionalTokenSource.SetTokens(atr, owner, globalVars)
                End If
            End If
        End Sub


    End Class
End NameSpace