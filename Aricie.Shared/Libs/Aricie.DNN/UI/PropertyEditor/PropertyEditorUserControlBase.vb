Imports Aricie.DNN.UI.Controls
Imports DotNetNuke.UI.WebControls

Namespace UI.WebControls


    Public Class PropertyEditorUserControlBase
        Inherits AricieUserControlBase

        Public Property EditControl As UserControlEditControl


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
