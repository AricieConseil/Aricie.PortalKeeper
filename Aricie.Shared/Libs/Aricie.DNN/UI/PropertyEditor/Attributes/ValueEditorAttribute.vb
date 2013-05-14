Namespace UI.Attributes

    Public Class ValueEditorAttribute
        Inherits InnerPropertyEditorAttribute

        Private Const PropName As String = "Value"

        Public Sub New(ByVal editorTypeName As String)
            MyBase.New(PropName, editorTypeName)
        End Sub

        Public Sub New(ByVal editorType As Type)
            MyBase.New(PropName, editorType.FullName)
        End Sub

        Public Sub New(ByVal editorTypeName As String, ByVal attributeProviderType As Type)
            MyBase.New(PropName, editorTypeName, attributeProviderType)
        End Sub

        Public Sub New(ByVal editorType As Type, ByVal attributeProviderType As Type)
            MyBase.New(PropName, editorType.FullName, attributeProviderType)
        End Sub

    End Class

End Namespace
