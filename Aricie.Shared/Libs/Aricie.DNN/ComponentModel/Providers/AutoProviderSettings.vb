Imports Aricie.Services
Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace ComponentModel


    
   Public Class AutoProviderSettings
        Inherits ProviderSettings
        Implements IProvider

        Protected _ProviderConfig As IProviderConfig

        Public Sub SetConfig(ByVal config As IProviderConfig) Implements IProvider.SetConfig
            Me._ProviderConfig = config
        End Sub

    End Class


    
    Public Class AutoProviderSettings(Of TConfig As IProviderConfig, TSettings As {IProviderSettings, AutoProviderSettings(Of TConfig, TSettings)})
        Inherits AutoProviderSettings
        Implements IProvider(Of TConfig, TSettings)


        <Browsable(False)> _
        <XmlIgnore()> _
        Public Overridable Property Config() As TConfig Implements IProvider(Of TConfig).Config
            Get
                Return DirectCast(Me._ProviderConfig, TConfig)
            End Get
            Set(ByVal value As TConfig)
                Me._ProviderConfig = value
            End Set
        End Property

        Public Function GetNewProviderSettings() As TSettings Implements IProvider(Of TConfig, TSettings).GetNewProviderSettings
            Return DirectCast(ReflectionHelper.CreateObject(Me.GetType), TSettings)
        End Function




    End Class


End Namespace