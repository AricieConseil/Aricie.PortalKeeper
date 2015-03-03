
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace ComponentModel
    <Serializable()> _
    Public Class ModuleProviderConfig
        Inherits ProviderConfig


        Private _ModuleName As String = ""

        <Required(True)> _
            <ExtendedCategory("")> _
            <MainCategory()> _
        Public Property ModuleName() As String
            Get
                Return _ModuleName
            End Get
            Set(ByVal value As String)
                _ModuleName = value
            End Set
        End Property



    End Class
End Namespace