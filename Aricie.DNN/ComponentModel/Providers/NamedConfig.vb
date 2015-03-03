Imports System.ComponentModel
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
                Dim details As String = GetFriendlyDetails()
                If details.IsNullOrEmpty() Then
                    Return String.Format("{0} {2} {1}", Me.Name, strEnable, UIConstants.TITLE_SEPERATOR)
                Else
                    Return String.Format("{1} {0} {2} {0} {3}", UIConstants.TITLE_SEPERATOR, Me.Name, strEnable, details)
                End If
            End Get
        End Property

        Public Overridable Property Enabled() As Boolean = True Implements IEnabled.Enabled


        Public Overridable Function GetFriendlyDetails() As String
            Return ""
        End Function

        Public Sub Disable()
            Me._Enabled = False
        End Sub

    End Class
End Namespace