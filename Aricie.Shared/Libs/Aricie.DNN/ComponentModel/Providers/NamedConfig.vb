Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.ComponentModel

Namespace ComponentModel
    Public Interface IEnabled

        Property Enabled() As Boolean


    End Interface


    
    Public Class NamedConfig
        Inherits NamedEntity
        Implements IEnabled



        Public Sub New()

        End Sub

        Public Sub New(ByVal name As String, ByVal description As String)
            MyBase.New(name, description)
        End Sub

        Public Overridable Property Enabled() As Boolean = True Implements IEnabled.Enabled


        Public Overrides Function GetFriendlyDetails() As String
            Dim strEnable As String = "Enabled"
            If Not Me.Enabled Then
                strEnable = "Disabled"
            End If
            Return strEnable
        End Function

        Public Sub Disable()
            Me._Enabled = False
        End Sub

    End Class
End Namespace