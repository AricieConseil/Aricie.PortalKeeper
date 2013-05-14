Imports System.Xml.Serialization
Imports Aricie.Services

Namespace Configuration

    ''' <summary>
    ''' Providers Add merge node
    ''' </summary>
    ''' <XmlInclude(GetType(CustomErrorAddInfo))> _
    '''    <XmlInclude(GetType(CustomErrorsAddInfo))> _
    <XmlRoot("add")> _
    <Serializable()> _
    Public Class ProviderAddInfo
        Inherits AddInfo

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String, ByVal type As Type, ByVal objAttributes As Dictionary(Of String, String))
            Me.Attributes("name") = name
            Me.Attributes("type") = ReflectionHelper.GetSafeTypeName(type)
            If objAttributes IsNot Nothing Then
                For Each attr As String In objAttributes.Keys
                    Me.Attributes(attr) = objAttributes(attr)
                Next
            End If
        End Sub

    End Class
End Namespace