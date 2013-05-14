Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    ''' <summary>
    ''' Flee import class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class FleeImportInfo

        Public Sub New()

        End Sub

        Public Sub New(ByVal objType As Type, ByVal customNamespace As String)
            Me._DotNetType.TypeName = ReflectionHelper.GetSafeTypeName(objType)
            Me._CustomNamespace = customNamespace
        End Sub

        ''' <summary>
        ''' Imported type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        Public Property DotNetType() As New DotNetType

        ''' <summary>
        ''' Destination namespace
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CustomNamespace() As String = String.Empty


    End Class
End Namespace