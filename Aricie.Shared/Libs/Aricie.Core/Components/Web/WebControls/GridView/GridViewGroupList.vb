Imports System.Reflection

Namespace Web.UI.Controls.GridViewGrouping
    <DefaultMember("Item")> _
    Public Class GridViewGroupList
        Inherits List(Of GridViewGroup)
        ' Methods
        Public Function FindGroupByName(ByVal name As String) As GridViewGroup
            Dim g As GridViewGroup
            For Each g In Me
                If (g.Name.ToLower = name.ToLower) Then
                    Return g
                End If
            Next
            Return Nothing
        End Function


        ' Properties
        Default Public Overloads ReadOnly Property Item(ByVal name As String) As GridViewGroup
            Get
                Return Me.FindGroupByName(name)
            End Get
        End Property
    End Class
End Namespace


