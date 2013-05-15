Imports Aricie.DNN.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    <Serializable()> _
    Public Class ProbeRanking
        Inherits NamedEntity


        Private _Items As New SimpleList(Of ProbeInstance)
        Public Property Items() As SimpleList(Of ProbeInstance)
            Get
                Return _Items
            End Get
            Set(ByVal value As SimpleList(Of ProbeInstance))
                _Items = value
            End Set
        End Property


    End Class
End Namespace