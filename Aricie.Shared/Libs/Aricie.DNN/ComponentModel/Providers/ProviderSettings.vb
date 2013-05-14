Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace ComponentModel
    <Serializable()> _
    Public Class ProviderSettings
        Inherits NamedConfig
        Implements IProviderSettings

        Protected _ProviderName As String = ""

        <ExtendedCategory("")> _
            <IsReadOnly(True)> _
        Public Overridable Property ProviderName() As String Implements IProviderSettings.ProviderName
            Get
                Return _ProviderName
            End Get
            Set(ByVal value As String)
                _ProviderName = value
            End Set
        End Property



    End Class
End Namespace