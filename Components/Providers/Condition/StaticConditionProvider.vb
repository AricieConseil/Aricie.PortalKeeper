Imports Aricie.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
     <System.ComponentModel.DisplayName("Static Condition")> _
    <Description("Simply matches according to the static value configured")> _
    Public Class StaticConditionProvider(Of TEngineEvents As IConvertible)
        Inherits ConditionProvider(Of TEngineEvents)



        Private _Value As Boolean = True


        <ExtendedCategory("Specifics")> _
        Public Property Value() As Boolean
            Get
                Return _Value
            End Get
            Set(ByVal value As Boolean)
                _Value = value
            End Set
        End Property



        Public Overrides Function Match(ByVal context As PortalKeeperContext(Of TEngineEvents)) As Boolean
            Return _Value

        End Function


    End Class
End Namespace