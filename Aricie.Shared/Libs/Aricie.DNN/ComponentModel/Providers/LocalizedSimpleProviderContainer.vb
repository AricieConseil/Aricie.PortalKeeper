Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes

Namespace ComponentModel
    Public MustInherit Class LocalizedSimpleProviderContainer(Of T)
        Inherits SimpleProviderContainer(Of T)

        <Selector("Text", "Value", False, False, "", "", True, False, True)> _
        Public Overrides Property Items As SerializableList(Of T)
            Get
                Return MyBase.Items
            End Get
            Set(value As SerializableList(Of T))
                MyBase.Items = value
            End Set
        End Property


    End Class
End Namespace