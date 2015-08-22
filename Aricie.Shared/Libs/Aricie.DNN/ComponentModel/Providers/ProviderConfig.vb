Imports Aricie.Services
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace ComponentModel
    <Serializable()> _
    Public Class ProviderConfig(Of T As IProvider)
        Inherits ProviderConfig
        Implements IProviderConfig(Of T)

        Public Sub New()
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String, ByVal type As Type)
            MyBase.New(name, description, type)
        End Sub

        Public Sub New(ByVal objType As Type)
            MyBase.New(objType)
        End Sub

        Public Overridable Function GetTypedProvider() As T Implements IProviderConfig(Of T).GetTypedProvider
            Dim toReturn As T = DirectCast(Me.GetProvider, T)
            If toReturn IsNot Nothing Then
                toReturn.SetConfig(Me)
            End If
            Return toReturn
        End Function
    End Class


    <Serializable()> _
    Public Class ProviderConfig
        Inherits NamedConfig
        Implements IProviderConfig

        Public Sub New()
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String, ByVal type As Type)
            MyBase.New(name, description)
            Me._TypeName = ReflectionHelper.GetSafeTypeName(type)
        End Sub

        Public Sub New(ByVal objType As Type)
            Me.New(ReflectionHelper.GetDisplayName(objType), ReflectionHelper.GetDescription(objType), objType)
        End Sub


        Private _TypeName As String = ""

        <ExtendedCategory("")> _
        <Required(True)> _
        <Width(500)> _
        <LineCount(2)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Property TypeName() As String
            Get
                Return _TypeName
            End Get
            Set(ByVal value As String)
                _TypeName = value
            End Set
        End Property

        <Browsable(False)> _
       <XmlIgnore()> _
       Public ReadOnly Property ProviderType() As Type
            Get
                If Not String.IsNullOrEmpty(Me._TypeName) Then
                    Return ReflectionHelper.CreateType(Me._TypeName)
                End If
                Return Nothing
            End Get
        End Property

        Public Overridable Function GetProvider() As Object Implements IProviderConfig.GetProvider
            Dim provType As Type = Me.ProviderType
            If provType IsNot Nothing Then
                Return ReflectionHelper.CreateObject(Me.ProviderType.AssemblyQualifiedName)
            End If
            Return Nothing
        End Function


        'todo: move to reflection helper
        

    End Class
End Namespace