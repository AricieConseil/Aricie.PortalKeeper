Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports System.Xml.Serialization

Namespace Entities
    ''' <summary>
    ''' Entity class for common secure API credentials authentication systems
    ''' </summary>
    ''' <remarks>The last authentication date helps managing locks.</remarks>
    <Serializable()> _
    <DefaultProperty("Key")> _
    Public Class APICredentials

        Private _Key As String = ""
        Private _Secret As String = ""
        Private _LastAuthenticationFailure As DateTime = DateTime.MinValue
        Private _Disabled As Boolean

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <Width(500)> _
        <DotNetNuke.UI.WebControls.MaxLength(256)> _
        Public Property Key() As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                If Me._Disabled AndAlso Not String.IsNullOrEmpty(_Key) AndAlso value <> _Key Then
                    Me._Disabled = False
                End If
                _Key = value.Trim
            End Set
        End Property


        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Required(True)> _
        <LineCount(2)> _
        <Width(500)> _
        <DotNetNuke.UI.WebControls.MaxLength(256)> _
        Public Property Secret() As String
            Get
                Return _Secret
            End Get
            Set(ByVal value As String)
                If Me._Disabled AndAlso Not String.IsNullOrEmpty(_Secret) AndAlso value <> _Secret Then
                    Me._Disabled = False
                End If
                _Secret = value.Trim
            End Set
        End Property



        <IsReadOnly(True)> _
        Public Property LastAuthenticationFailure() As DateTime
            Get
                Return _LastAuthenticationFailure
            End Get
            Set(ByVal value As DateTime)
                _LastAuthenticationFailure = value
            End Set
        End Property



        <IsReadOnly(True)> _
        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    If Now.Subtract(_LastAuthenticationFailure) < TimeSpan.FromHours(1) Then
                        _Disabled = True
                    End If
                    _LastAuthenticationFailure = Now
                Else
                    _Disabled = False
                End If
            End Set
        End Property

        <Browsable(False)> _
        <XmlIgnore()> _
        Public ReadOnly Property Enabled() As Boolean
            Get
                Return Not _Disabled AndAlso Not String.IsNullOrEmpty(Me._Key) AndAlso Not String.IsNullOrEmpty(Me._Secret) AndAlso Not Me._Secret.Contains(" "c)
            End Get
        End Property

    End Class
End NameSpace