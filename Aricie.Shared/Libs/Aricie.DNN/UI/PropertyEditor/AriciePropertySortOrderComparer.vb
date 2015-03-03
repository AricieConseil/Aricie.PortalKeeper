Imports System.Reflection
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace UI.WebControls
    Public Class AriciePropertySortOrderComparer
        Implements IComparer

        Private _initialPositions As New Dictionary(Of PropertyInfo, Integer)


        Public Sub New(ByVal initialArray As PropertyInfo())
            For i As Integer = 0 To initialArray.Length - 1
                _initialPositions(initialArray(i)) = i
            Next
        End Sub

        Private Shared _SortOrders As New Dictionary(Of PropertyInfo, Integer)



        Private Function GetOrder(ByVal objprop As PropertyInfo) As Integer
            'Dim order As Nullable(Of Integer) = Aricie.Services.CacheHelper.GetGlobal(Of Nullable(Of Integer))("sortOrder", objprop.DeclaringType.Name, objprop.Name)
            Dim order As Integer
            If Not _SortOrders.TryGetValue(objprop, order) Then
                order = 100
                Dim customAttributes = ReflectionHelper.GetCustomAttributes(objprop).Where(Function(objAttribute) TypeOf objAttribute Is SortOrderAttribute)
                If (customAttributes.Any) Then
                    order = DirectCast(customAttributes(0), SortOrderAttribute).Order
                End If
                SyncLock (_SortOrders)
                    _SortOrders(objprop) = order
                End SyncLock
                'Aricie.Services.CacheHelper.SetGlobal(Of Nullable(Of Integer))(order, "sortOrder", objprop.DeclaringType.Name, objprop.Name)
            End If
            Return order
        End Function

        ' Methods
        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            If (Not TypeOf x Is PropertyInfo OrElse Not TypeOf y Is PropertyInfo) Then
                Throw New ArgumentException("Object is not of type PropertyInfo")
            End If
            'Dim order As Integer = 100
            'Dim order2 As Integer = 100
            'Dim info As PropertyInfo = DirectCast(x, PropertyInfo)
            'Dim info2 As PropertyInfo = DirectCast(y, PropertyInfo)
            'Dim customAttributes As Object()
            'customAttributes = info.GetCustomAttributes(GetType(SortOrderAttribute), True)
            'If (customAttributes.Length > 0) Then
            '    order = DirectCast(customAttributes(0), SortOrderAttribute).Order
            'End If
            'customAttributes = info2.GetCustomAttributes(GetType(SortOrderAttribute), True)
            'If (customAttributes.Length > 0) Then
            '    order2 = DirectCast(customAttributes(0), SortOrderAttribute).Order
            'End If
            Dim info As PropertyInfo = DirectCast(x, PropertyInfo)
            Dim info2 As PropertyInfo = DirectCast(y, PropertyInfo)
           
            Dim order As Integer = GetOrder(info)
            Dim order2 As Integer = GetOrder(info2)
            If order <> order2 Then
                'customAttributes = info.GetCustomAttributes(GetType(CategoryAttribute), True)
                'Dim cat1 As String = ""
                'Dim cat2 As String = ""
                'If (customAttributes.Length > 0) Then
                '    cat1 = DirectCast(customAttributes(0), CategoryAttribute).Category
                'End If
                'customAttributes = info2.GetCustomAttributes(GetType(CategoryAttribute), True)
                'If (customAttributes.Length > 0) Then
                '    cat2 = DirectCast(customAttributes(0), CategoryAttribute).Category
                'End If
                'If cat1 = cat2 Then
                Return order - order2
                'End If
            End If

            Dim type1 As Type = info.DeclaringType
            Dim type2 As Type = info2.DeclaringType
            If type1 IsNot type2 Then
                Dim baseType1 As Type = info.GetAccessors.First.GetBaseDefinition.DeclaringType
                Dim baseType2 As Type = info2.GetAccessors.First.GetBaseDefinition.DeclaringType
                If baseType1.IsSubclassOf(baseType2) Then
                    Return 1
                ElseIf baseType2.IsSubclassOf(baseType1) Then
                    Return -1
                ElseIf type1.IsSubclassOf(type2) Then
                    Return 1
                Else
                    Return -1
                End If
            End If
            Return _initialPositions(info) - _initialPositions(info2)
        End Function


    End Class
End Namespace