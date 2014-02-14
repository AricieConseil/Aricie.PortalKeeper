Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace Services.Filtering
    <Serializable()> _
    Public Class TokenizedTextInfo

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <LineCount(10)> _
        <Width(400)> _
        Public Overridable Property Text() As String = ""

        Public Overridable Property EnableTokenReplace() As Boolean

        Public Overridable Property AdditionalTokenSource() As TokenSourceInfo = New TokenSourceInfo

        Public Overloads Function GetText() As String
            Return GetText(Nothing)
        End Function


        Public Overloads Function GetText(atr As AdvancedTokenReplace) As String

            Return GetText(atr, Text)

        End Function

        Public Overloads Function GetText(atr As AdvancedTokenReplace, template As String) As String

            If Me.EnableTokenReplace Then
                PrepareTokenReplace(atr)
                Return atr.ReplaceAllTokens(template)
            Else
                Return template
            End If

        End Function


        Public Sub PrepareTokenReplace(atr As AdvancedTokenReplace)
            If Me.EnableTokenReplace Then
                If atr Is Nothing Then
                    atr = New AdvancedTokenReplace()
                End If
                If Not atr.IsSet(Me.AdditionalTokenSource) Then
                    Me.AdditionalTokenSource.SetTokens(atr)
                End If
            End If
        End Sub


    End Class
End NameSpace