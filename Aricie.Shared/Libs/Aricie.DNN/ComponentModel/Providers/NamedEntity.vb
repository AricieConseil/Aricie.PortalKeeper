Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace ComponentModel
    <Serializable()> _
    <DefaultProperty("Name")> _
    Public Class NamedEntity



        Public Sub New()
        End Sub

        Public Sub New(ByVal strName As String, ByVal strDescription As String)
            Me._Name = strName
            Me._Decription = strDescription
        End Sub

        <Required(True)> _
        <Width(300)> _
        Public Overridable Property Name() As String = ""
           

        Public Overridable Property Decription() As CData = ""
         

    End Class

    Public Class NamedIdentifierEntity
        Inherits NamedEntity


        Public Sub New()
        End Sub

        Public Sub New(ByVal strName As String, ByVal strDescription As String)
            MyBase.New(strName, strDescription)
        End Sub

        <SortOrder(0)> _
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