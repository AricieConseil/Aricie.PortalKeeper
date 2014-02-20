Imports System.Reflection
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace ComponentModel

    <Serializable> _
    Public Class ActionButtonInfo

        Public Sub New()

        End Sub

        Public Property Method As MethodInfo
        Public Property ExtendedCategory As ExtendedCategory
        Public Property Mode As ActionButtonMode = ActionButtonMode.CommandButton
        Public Property IconAction As New IconActionInfo
        Public Property IconPath As String = ""
        Public Property AlertKey As String = ""
        Public Property ConditionalVisibles As List(Of ConditionalVisibleInfo)

        Public Shared Function FromMethod(objMethod As MethodInfo) As ActionButtonInfo
            Dim toReturn As ActionButtonInfo = FromMember(objMethod)
            If toReturn IsNot Nothing Then
                toReturn.Method = objMethod
                toReturn.ExtendedCategory = ExtendedCategory.FromMember(objMethod)
                toReturn.ConditionalVisibles = ConditionalVisibleInfo.FromMember(objMethod)
            End If
            Return toReturn
        End Function

        Public Shared Function FromMember(objMember As MemberInfo) As ActionButtonInfo
            Dim toReturn As ActionButtonInfo = Nothing
            Dim attrs As Attribute() = DirectCast(objMember.GetCustomAttributes(GetType(ActionButtonAttribute), True), Attribute())
            If attrs.Length > 0 Then

                toReturn = FromAttribute(DirectCast(attrs(0), ActionButtonAttribute))

            End If
            Return toReturn
        End Function

        Public Shared Function FromAttribute(objAttribute As ActionButtonAttribute) As ActionButtonInfo
            Dim toReturn As New ActionButtonInfo
            toReturn.Mode = objAttribute.Mode
            toReturn.IconAction = objAttribute.IconAction
            toReturn.IconPath = objAttribute.IconPath
            toReturn.AlertKey = objAttribute.AlertKey
            'toReturn.ExtendedCategory = ExtendedCategory.FromMember(objMethod)
            'toReturn.ConditionalVisibles = ConditionalVisibleInfo.FromMember(objMethod)
            Return toReturn
        End Function

    End Class
End Namespace


