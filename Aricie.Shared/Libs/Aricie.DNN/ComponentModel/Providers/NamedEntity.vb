Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization

Namespace ComponentModel
    <Serializable()> _
    <DefaultProperty("FriendlyName")> _
    Public Class NamedEntity



        Public Sub New()
        End Sub

        Public Sub New(ByVal strName As String, ByVal strDescription As String)
            Me._Name = strName
            Me._Decription = strDescription
        End Sub

        <SortOrder(0)> _
        <XmlAttribute("name")> _
        <Required(True)> _
        <Width(300)> _
        Public Overridable Property Name() As String = ""

        <XmlElement("Name")> _
        <Browsable(False)> _
        Public Property OldName As String
            Get
                Return Nothing
            End Get
            Set(value As String)
                Name = value
            End Set
        End Property


        <Browsable(False)> _
       <XmlIgnore()> _
        Public ReadOnly Property FriendlyName As String
            Get
                Dim details As String = GetFriendlyDetails()
                If details.IsNullOrEmpty() Then
                    Return Me.Name
                Else
                    Return String.Format("{0} {1} {2}", Me.Name, UIConstants.TITLE_SEPERATOR, details)
                End If
            End Get
        End Property


        <SortOrder(1)> _
         Public Overridable Property Decription() As CData = ""

        Public Overridable Function GetFriendlyDetails() As String
            Return ""
        End Function

    End Class

    Public Class NamedIdentifierEntity
        Inherits NamedEntity


        Public Sub New()
        End Sub

        Public Sub New(ByVal strName As String, ByVal strDescription As String)
            MyBase.New(strName, strDescription)
        End Sub

        <XmlAttribute("name")> _
        <RegularExpressionValidator(Constants.Content.RegularNameValidator)> _
        <Required(True)> _
        <Width(300)> _
        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

    End Class


End NameSpace