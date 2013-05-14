Imports Aricie.DNN.UI.Controls
Namespace UI.WebControls


    Public Class PropertyEditorUserControlBase
        Inherits AricieUserControlBase


        Private _value As Object

        Public Property Value As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
            End Set
        End Property

        Public Property TrialMode As UserControlEditControl.TrialModeInformation = Nothing

    End Class
End Namespace
