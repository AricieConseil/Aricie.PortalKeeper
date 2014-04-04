Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization

Namespace ComponentModel

    <Serializable()> _
    <DefaultProperty("Name")> _
    Public Class NamedEntity

        Private _Name As String = ""

        Private _Decription As CData = ""



        Public Sub New()

        End Sub

        Public Sub New(ByVal name As String, ByVal description As String)
            Me._Name = name
            Me._Decription = description
        End Sub

        <ExtendedCategory("")> _
                    <Required(True)> _
            <MainCategory()> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <Width(300)> _
        Public Overridable Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        '<Required(True)> _
        <ExtendedCategory("")> _
            <Width(400)> _
            <LineCount(5)> _
            <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        Public Overridable Property Decription() As CData
            Get
                Return _Decription
            End Get
            Set(ByVal value As CData)
                _Decription = value
            End Set
        End Property

    End Class

    Public Interface IEnabled

        Property Enabled() As Boolean


    End Interface


    <Serializable()> _
    <DefaultProperty("FriendlyName")> _
    Public Class NamedConfig
        Inherits NamedEntity
        Implements IEnabled


        Private _Enabled As Boolean = True


        Public Sub New()

        End Sub

        Public Sub New(ByVal name As String, ByVal description As String)
            MyBase.New(name, description)
        End Sub

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property FriendlyName As String
            Get
                Dim strEnable As String = "Enabled"
                If Not Me.Enabled Then
                    strEnable = "Disabled"
                End If
                Return String.Format("{0} {2} {1}", Me.Name, strEnable, Aricie.DNN.ComponentModel.UIConstants.TITLE_SEPERATOR)
            End Get
        End Property


        <ExtendedCategory("")> _
        Public Overridable Property Enabled() As Boolean Implements IEnabled.Enabled
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        Public Sub Disable()
            Me._Enabled = False
        End Sub

    End Class
End Namespace