

Namespace UI.Attributes



    Public MustInherit Class InnerPropertyEditorAttribute
        Inherits InnerEditorAttribute

        Private _PropertyName As String

        Public ReadOnly Property PropertyName() As String
            Get
                Return Me._PropertyName
            End Get
        End Property


        Public Sub New(ByVal propertyName As String, ByVal attributeProviderType As Type)
            MyBase.New(attributeProviderType)
            Me._PropertyName = propertyName
        End Sub

        Public Sub New(ByVal propertyName As String, ByVal editorTypeName As String)
            MyBase.New(editorTypeName)
            Me._PropertyName = propertyName
        End Sub

        Public Sub New(ByVal propertyName As String, ByVal editorTypeName As String, ByVal attributeProviderType As Type)
            MyBase.New(editorTypeName, attributeProviderType)
            Me._PropertyName = propertyName
        End Sub

        Public Sub New(ByVal propertyName As String, ByVal editorType As Type, ByVal attributeProviderType As Type)
            Me.New(propertyName, editorType.FullName, attributeProviderType)
        End Sub

    End Class



End Namespace
