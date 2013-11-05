Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel

Namespace Entities

    ''' <summary>
    ''' Composite Entity with a code expression to be evaluated for a conditional behaviour
    ''' </summary>
    <Serializable()> _
    Public Class ConditionalEntity(Of T As {New})
        Inherits NamedEntity



#Region "Private members"

        Private _Condition As String = ""

        Private _Value As New T





#End Region

#Region "cTors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal condition As String, ByVal value As T)
            Me._Condition = condition
            Me._Value = value
        End Sub


#End Region


#Region "Public Properties"

        <Required(True)> _
           <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
           <LineCount(7)> _
           <Width(500)> _
           <LabelMode(LabelMode.Top)> _
        Public Property Condition() As String
            Get
                Return Me._Condition
            End Get
            Set(ByVal value As String)
                Me._Condition = value
            End Set
        End Property

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property Value() As T
            Get
                Return Me._Value
            End Get
            Set(ByVal value As T)
                Me._Value = value
            End Set
        End Property


#End Region


    End Class

End Namespace
