Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.ComponentModel

Namespace ComponentModel
    Public Interface IEnabled

        Property Enabled() As Boolean


    End Interface


    <Serializable()> _
    <DefaultProperty("FriendlyName")> _
    Public Class NamedConfig
        Inherits NamedEntity
        Implements IEnabled



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
                Return String.Format("{0} {2} {1}", Me.Name, strEnable, UIConstants.TITLE_SEPERATOR)
            End Get
        End Property

        Public Overridable Property Enabled() As Boolean = True Implements IEnabled.Enabled


        Public Sub Disable()
            Me._Enabled = False
        End Sub

    End Class
End Namespace