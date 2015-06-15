Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace ComponentModel
    <Serializable()> _
    Public Class LocationSettings
        Implements IEquatable(Of LocationSettings)


        Private _UserFile As Boolean
        Private _UserFileName As String
        Private _BackupsNb As Integer = 4



        Public Sub New()

        End Sub

        Public Sub New(ByVal userFile As Boolean, ByVal userFileName As String)
            Me._UserFile = userFile
            Me._UserFileName = userFileName
        End Sub

        <AutoPostBack> _
        Public Property UserFile() As Boolean
            Get
                Return _UserFile
            End Get
            Set(ByVal value As Boolean)
                _UserFile = value
            End Set
        End Property

        <ConditionalVisible("UserFile", False, True)> _
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
        <Width(300)> _
        Public Property UserFileName() As String
            Get
                Return _UserFileName
            End Get
            Set(ByVal value As String)
                _UserFileName = value
            End Set
        End Property

        Public Property BackupsNb() As Integer
            Get
                Return _BackupsNb
            End Get
            Set(ByVal value As Integer)
                _BackupsNb = value
            End Set
        End Property

        Public Property UseSingleton As Boolean = True

        <ConditionalVisible("UseSingleton", True)> _
        Public Property UseCache As Boolean = True


        Public Shared CoreFile As New LocationSettings(False, "")


        Public Overloads Function Equals(other As LocationSettings) As Boolean Implements System.IEquatable(Of LocationSettings).Equals
            Return Me._UserFile = other._UserFile AndAlso Me._UserFileName = other._UserFileName AndAlso Me._BackupsNb = other._BackupsNb
        End Function
    End Class
End Namespace