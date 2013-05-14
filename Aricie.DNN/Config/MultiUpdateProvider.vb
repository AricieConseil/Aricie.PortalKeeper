Namespace Configuration
    ''' <summary>
    ''' Compund Update provider for modular usage
    ''' </summary>
    Public Class MultiUpdateProvider
        Implements IUpdateProvider

        Private _SourceProviders As IUpdateProvider()


        Public Sub New(ByVal ParamArray sourceProviders() As IUpdateProvider)
            Me._SourceProviders = sourceProviders
        End Sub


        Public Function GetConfigElements() As System.Collections.Generic.List(Of IConfigElementInfo) Implements IUpdateProvider.GetConfigElements
            Dim toReturn As New List(Of IConfigElementInfo)
            If Me._SourceProviders IsNot Nothing Then
                For Each prov As IUpdateProvider In _SourceProviders
                    toReturn.AddRange(prov.GetConfigElements)
                Next
            End If
            Return toReturn
        End Function
    End Class
End Namespace