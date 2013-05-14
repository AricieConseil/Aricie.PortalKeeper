Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace UI.Attributes


    Public Class InnerEditorAttribute
        Inherits AttributeContainerAttribute

        Protected Sub New()

        End Sub

        Public Sub New(ByVal attributeProviderType As Type)
            MyBase.New(attributeProviderType)
        End Sub

        Public Sub New(ByVal editorTypeName As String, ByVal attributeProviderType As Type)
            Me.New(attributeProviderType)
            Me.AddAttribute(New EditorAttribute(editorTypeName, GetType(EditControl)))
        End Sub

        Public Sub New(ByVal editorType As Type, ByVal attributeProviderType As Type)
            Me.New(editorType.AssemblyQualifiedName, attributeProviderType)
        End Sub

        Public Sub New(ByVal editorTypeName As String)
            MyBase.New()
            Me.AddAttribute(New EditorAttribute(editorTypeName, GetType(EditControl)))
        End Sub

    End Class




End Namespace
