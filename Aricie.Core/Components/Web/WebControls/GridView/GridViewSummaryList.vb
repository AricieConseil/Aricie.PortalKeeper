Imports System.Reflection


Namespace Web.UI.Controls.GridViewGrouping
    <DefaultMember("Item")> _
    Public Class GridViewSummaryList
        Inherits List(Of GridViewSummary)
        ' Methods
        Public Function FindSummaryByColumn(ByVal columnName As String) As GridViewSummary
            Dim s As GridViewSummary
            For Each s In Me
                If (s.Column.ToLower = columnName.ToLower) Then
                    Return s
                End If
            Next
            Return Nothing
        End Function


        ' Properties
        Default Public Overloads ReadOnly Property Item(ByVal name As String) As GridViewSummary
            Get
                Return Me.FindSummaryByColumn(name)
            End Get
        End Property
    End Class
End Namespace


