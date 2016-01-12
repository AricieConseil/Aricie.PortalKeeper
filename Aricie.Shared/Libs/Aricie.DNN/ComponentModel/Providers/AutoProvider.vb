
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace ComponentModel
    
    Public MustInherit Class AutoProvider(Of TConfig As {IProviderConfig, NamedConfig}, TSettings As {AutoProviderSettings(Of TConfig, TSettings)}, TProvider As {IProvider(Of TConfig)})
        Inherits AutoProviderSettings(Of TConfig, TSettings)
        Implements IAutoProvider(Of TConfig, TProvider)



        Private _ProviderInstance As TProvider




        Protected MustOverride Function CreateProvider() As TProvider 'Implements IProviderSettings(Of TProvider).GetProvider

        <ExtendedCategory("")> _
        <IsReadOnly(True)> _
        Public Overrides Property ProviderName() As String
            Get
                Dim toReturn As String = MyBase._ProviderName
                If String.IsNullOrEmpty(toReturn) Then
                    toReturn = ReflectionHelper.GetDisplayName(Me.GetType)
                    MyBase._ProviderName = toReturn
                End If
                Return toReturn
            End Get
            Set(ByVal value As String)
                MyBase.ProviderName = value
            End Set
        End Property

        Public Function GetProvider() As TProvider Implements IProviderSettings(Of TProvider).GetProvider
            If _ProviderInstance Is Nothing Then
                _ProviderInstance = Me.CreateProvider
                '_ProviderInstance.Config = Me.Config
            End If
            Return _ProviderInstance
        End Function

       
    End Class
End Namespace