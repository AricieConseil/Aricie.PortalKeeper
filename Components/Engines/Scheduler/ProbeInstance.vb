Imports System.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <DefaultProperty("Header")> _
    Public Class ProbeInstance
        Implements IComparable(Of ProbeInstance)


        Private _Header As String = String.Empty
        Private _Value As IComparable
        Private _Parameters As SerializableDictionary(Of String, Object)
        Private _User As String = "Anonymous"

        Public Sub New()

        End Sub

        Public Sub New(header As String, value As IComparable, params As SerializableDictionary(Of String, Object))
            'If Not String.IsNullOrEmpty(userName) Then
            '    Me._UserName = userName
            'End If

            Me._Header = header
            Me._Value = value
            Me._Parameters = params
        End Sub

        <IsReadOnly(True)> _
        Public Property Header() As String
            Get
                Return _Header
            End Get
            Set(value As String)
                _Header = value
            End Set
        End Property



        <Browsable(False)> _
            <IsReadOnly(True)> _
        Public Property Value() As IComparable
            Get
                Return _Value
            End Get
            Set(value As IComparable)
                _Value = value
            End Set
        End Property



        Public Property User() As String
            Get
                Return _User
            End Get
            Set(ByVal value As String)
                _User = value
            End Set
        End Property


        <IsReadOnly(True)> _
        <CollectionEditor(True, False, False, True, 5, CollectionDisplayStyle.Accordion, False)> _
        Public Property Parameters() As SerializableDictionary(Of String, Object)
            Get
                Return _Parameters
            End Get
            Set(value As SerializableDictionary(Of String, Object))
                _Parameters = value
            End Set
        End Property




        Public Function CompareTo(other As ProbeInstance) As Integer Implements IComparable(Of ProbeInstance).CompareTo
            Return Me._Value.CompareTo(other._Value)
        End Function



    End Class
End Namespace