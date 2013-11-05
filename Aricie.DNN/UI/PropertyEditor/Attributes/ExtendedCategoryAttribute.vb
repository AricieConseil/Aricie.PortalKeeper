Imports Aricie.DNN.ComponentModel

Namespace UI.Attributes

    Public Class ExtendedCategoryAttribute
        Inherits Attribute



        Public Sub New(ByVal tabName As String)
            Me._ExtendedCategory = New ExtendedCategory(tabName)
            
        End Sub

        Public Sub New(ByVal sectionName As String, ByVal column As Integer)
            Me._ExtendedCategory = New ExtendedCategory(sectionName, column)
        End Sub

        Public Sub New(ByVal tabName As String, ByVal sectionName As String)
            Me._ExtendedCategory = New ExtendedCategory(tabName, sectionName)
        End Sub
        Public Sub New(ByVal tabName As String, ByVal sectionName As String, ByVal column As Integer)
            Me._ExtendedCategory = New ExtendedCategory(tabName, sectionName, column)
        End Sub


        Public Property ExtendedCategory As ExtendedCategory


    End Class

End Namespace
