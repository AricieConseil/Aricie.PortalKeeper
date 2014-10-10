Imports System.Reflection
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.Services
Imports System.ComponentModel

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


        Private Shared _MemberActions As New Dictionary(Of MemberInfo, ActionButtonInfo)


        Public Shared Function FromMember(objMember As MemberInfo) As ActionButtonInfo
            Dim toReturn As ActionButtonInfo = Nothing
            If Not _MemberActions.TryGetValue(objMember, toReturn) Then
                Dim custAttr = ReflectionHelper.GetCustomAttributes(objMember)
                If Not custAttr.Any(Function(objAttribute) (TypeOf objAttribute Is BrowsableAttribute) AndAlso DirectCast(objAttribute, BrowsableAttribute).Browsable = False) Then
                    Dim attrs = custAttr.Where(Function(objAttribute) TypeOf objAttribute Is ActionButtonAttribute)
                    If attrs.Any Then
                        toReturn = FromAttribute(DirectCast(attrs(0), ActionButtonAttribute))
                    End If
                End If
                SyncLock _MemberActions
                    _MemberActions(objMember) = toReturn
                End SyncLock
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


