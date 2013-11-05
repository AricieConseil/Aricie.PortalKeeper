Imports System.Reflection
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes

Namespace ComponentModel

    <Serializable> _
    Public Class ExtendedCategory

        Public Sub New()
        End Sub

        Public Sub New(ByVal tabName As String)
            Me._TabName = tabName
            Me._SectionName = String.Empty
            Me._Column = 0
        End Sub

        Public Sub New(ByVal sectionName As String, ByVal column As Integer)
            Me._TabName = String.Empty
            Me._SectionName = sectionName
            Me._Column = column
        End Sub

        Public Sub New(ByVal tabName As String, ByVal sectionName As String)
            Me._TabName = tabName
            Me._SectionName = sectionName
            Me._Column = 0
        End Sub
        Public Sub New(ByVal tabName As String, ByVal sectionName As String, ByVal column As Integer)
            Me._TabName = tabName
            Me._SectionName = sectionName
            Me._Column = column
        End Sub


        Public Property TabName As String = ""
        Public Property SectionName As String = ""
        Public Property Column As Integer = 0
        Public Property Prefix As String



        Public Shared Function FromMember(member As MemberInfo) As ExtendedCategory
            Dim toReturn As ExtendedCategory
            Dim customAttributes As Object() = member.GetCustomAttributes(GetType(CategoryAttribute), True)
            If customAttributes.Length > 0 Then
                Dim categoryAttr As CategoryAttribute = DirectCast(customAttributes(0), CategoryAttribute)
                toReturn = New ExtendedCategory
                toReturn.SectionName = categoryAttr.Category
            Else
                customAttributes = member.GetCustomAttributes(GetType(ExtendedCategoryAttribute), True)
                If customAttributes.Length > 0 Then
                    toReturn = DirectCast(customAttributes(0), ExtendedCategoryAttribute).ExtendedCategory
                Else
                    toReturn = New ExtendedCategory
                End If
            End If
            toReturn.Prefix = GetResourcePrefix(member)
            Return toReturn
        End Function


        Private Shared Function GetResourcePrefix(member As MemberInfo) As String
            Dim toReturn As String = ""
            If TypeOf member Is PropertyInfo Then
                toReturn = DirectCast(member, PropertyInfo).GetAccessors().First.GetBaseDefinition.DeclaringType.Name
            ElseIf TypeOf member Is MethodInfo Then
                toReturn = DirectCast(member, MethodInfo).GetBaseDefinition.DeclaringType.Name
            Else
                toReturn = member.DeclaringType.Name
            End If
            Return toReturn.Replace("`"c, "")
        End Function


    End Class

End Namespace


