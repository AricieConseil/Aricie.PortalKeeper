Imports Aricie.Services
Imports System.ComponentModel

Namespace ComponentModel
    ''' <summary>
    ''' Wrapper class to serialize and rebuild a delegate. Supports invocation list combination
    ''' </summary>
    <Serializable()> _
    Public Class DelegateInfo(Of T)


        Private _TypeName As String = ""
        Private _MethodName As String = ""
        Private _InvocationList As New List(Of DelegateInfo(Of T))

        Public Sub New()
        End Sub

        Public Sub New(ByVal typeName As String, ByVal methodName As String)
            Me._TypeName = typeName
            Me._MethodName = methodName
        End Sub

        Public Sub New(ByVal objDelegate As [Delegate])
            Dim invList() As [Delegate] = objDelegate.GetInvocationList
            Select Case invList.Length
                Case 0
                    Throw New ArgumentException("Empty Delegate", "objDelegate")
                Case 1
                    Me._TypeName = ReflectionHelper.GetSafeTypeName(invList(0).Method.DeclaringType)
                    Me._MethodName = invList(0).Method.Name
                Case Else
                    For Each subDelegate As [Delegate] In invList
                        Me._InvocationList.Add(New DelegateInfo(Of T)(subDelegate))
                    Next
            End Select
        End Sub

        Public Overridable Property TypeName() As String
            Get
                Return _TypeName
            End Get
            Set(ByVal value As String)
                _TypeName = value
            End Set
        End Property



        Public Overridable Property MethodName() As String
            Get
                Return _MethodName
            End Get
            Set(ByVal value As String)
                _MethodName = value
            End Set
        End Property

        Public Function GetDelegate() As [Delegate]
            'Return ReflectionHelper.CreateDelegate(Of T)(Me)
            If Me.InvocationList.Count = 0 Then
                Return ReflectionHelper.CreateDelegate(Of T)(Me.TypeName, Me.MethodName)
            Else
                Dim tempList As New List(Of [Delegate])(Me.InvocationList.Count)
                For Each subDelegate As DelegateInfo(Of T) In Me.InvocationList
                    tempList.Add(subDelegate.GetDelegate())
                Next
                Return [Delegate].Combine(tempList.ToArray())
            End If

        End Function

        <Browsable(False)> _
        Public Property InvocationList() As List(Of DelegateInfo(Of T))
            Get
                Return _InvocationList
            End Get
            Set(ByVal value As List(Of DelegateInfo(Of T)))
                _InvocationList = value
            End Set
        End Property

    End Class
End Namespace