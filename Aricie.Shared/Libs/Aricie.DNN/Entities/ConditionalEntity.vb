Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee

Namespace Entities

    ''' <summary>
    ''' Composite Entity with a code expression to be evaluated for a conditional behaviour
    ''' </summary>
    
    Public Class ConditionalEntity(Of T As {New})
        Inherits NamedConfig



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

        'todo: remove that obsolete property
        <Browsable(False)> _
        Public Property Condition() As String
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)
                If Not value.IsNullOrEmpty() Then
                    Me.ConditionExpression.Expression = value
                End If
            End Set
        End Property

        Public Property ConditionExpression As New FleeExpressionInfo(Of Boolean)


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
