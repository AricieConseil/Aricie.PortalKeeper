Imports System.Reflection
Imports Aricie.Services

Namespace Services


    ''' <summary>
    ''' Property explorer for the ATR, to go from string representation of properties to code representation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PropertyExplorer

        Public Const LocalizedKey As String = "Localized"
        Public Shared ArrayGetMethod As MethodInfo = GetType(Array).GetMethod("GetValue", New Type() {GetType(Integer)})

        Private _Separator As Char

        Public Sub New(ByVal expression As String, ByVal initialValue As Object)
            Me.New(expression, initialValue, ":"c)
        End Sub

        Public Sub New(ByVal expression As String, ByVal initialValue As Object, ByVal separator As Char)
            Me._Separator = separator
            Me._Params = New List(Of String)(expression.Split(_Separator))
            Me._CurrentValue = initialValue
        End Sub

        ''' <summary>
        ''' Gets if the Property Explorer is done digging down in the properties
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsCompleted() As Boolean
            Get
                Return Me._CurrentIndex >= Me.Params.Count
            End Get
        End Property

        ''' <summary>
        ''' List of parameters to dig down into
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Params() As List(Of String)

        ''' <summary>
        ''' Current status for the exploration value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property CurrentValue() As Object

        ''' <summary>
        ''' Current index of the exploration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CurrentIndex() As Integer = 0
           
        ''' <summary>
        ''' Gets or sets whether the property access is localized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsLocalized() As Boolean = False


        ''' <summary>
        ''' Gets the current parameter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentParam() As String
            Get
                If Me.CurrentIndex < Me.Params.Count Then
                    Return Me.Params(CurrentIndex)
                End If
                Return ""
            End Get
        End Property


        Private _CurrentMember As MemberInfo

        ''' <summary>
        ''' Gets the current member information
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentMember() As MemberInfo
            Get
                Return _CurrentMember
            End Get
        End Property

        ''' <summary>
        ''' Gets the remaining tokens as a list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TokenQueue() As String
            Get
                If CurrentIndex < Me.Params.Count Then
                    Return String.Join(_Separator, Me.Params.GetRange(CurrentIndex, Me.Params.Count - CurrentIndex).ToArray())
                End If
                Return ""
            End Get
        End Property

        ''' <summary>
        ''' Calculates deep property value
        ''' </summary>
        ''' <param name="deepAccess"></param>
        ''' <remarks></remarks>
        Public Sub GetDeepPropertyValue(ByVal deepAccess As DeepObjectPropertyAccess)

            Dim dpa As DeepObjectPropertyAccess = Nothing
            While Me.CurrentIndex < Me.Params.Count AndAlso Not Me.IsCompleted
                Dim currentParam As String = Me.CurrentParam
                Me.GetPropertyValue()
                If Me._CurrentValue IsNot Nothing Then
                    If Not String.IsNullOrEmpty(Me.TokenQueue) Then

                        'dealing with sub properties
                        If Not deepAccess.PropertiesDeepAccess.TryGetValue(currentParam, dpa) Then
                            dpa = deepAccess.GetDeepPropertyAccess(currentParam, Me)
                        End If
                       
                        Me.GetDeepPropertyValue(dpa)
                    End If
                End If
            End While

        End Sub

        ''' <summary>
        ''' Calculates property value
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub GetPropertyValue()
            Try
                If Me.CurrentParam = LocalizedKey Then
                    Me.IsLocalized = True
                    Me.CurrentIndex += 1
                Else
                    Dim objType As Type = Me.CurrentValue.GetType
                    _CurrentMember = ReflectionHelper.GetMember(objType, Me.CurrentParam)
                    If Not _CurrentMember Is Nothing Then
                        Me.CurrentIndex += 1
                    Else

                        Dim defaultMembers() As MemberInfo = objType.GetDefaultMembers
                        For Each tempMember As MemberInfo In defaultMembers

                            Select Case tempMember.MemberType
                                Case MemberTypes.Field, MemberTypes.Property, MemberTypes.Method
                                    _CurrentMember = tempMember
                                    Exit For
                            End Select
                        Next
                        If _CurrentMember Is Nothing And TypeOf Me.CurrentValue Is Array Then
                            _CurrentMember = ArrayGetMethod
                        End If
                    End If

                    If _CurrentMember IsNot Nothing Then

                        Select Case _CurrentMember.MemberType
                            Case MemberTypes.Property
                                Dim propInfo As PropertyInfo = DirectCast(_CurrentMember, PropertyInfo)

                                Dim objIndexParameters() As ParameterInfo = propInfo.GetIndexParameters()
                                Dim paramvalues As Object() = ReflectionHelper.BuildParameters(objIndexParameters, _
                                                        Me.Params.GetRange(Me.CurrentIndex, Me.Params.Count - Me.CurrentIndex))
                                If paramvalues IsNot Nothing Then
                                    Me.CurrentIndex += paramvalues.Length
                                End If
                                Me._CurrentValue = propInfo.GetValue(Me.CurrentValue, paramvalues)
                            Case MemberTypes.Field
                                Dim fInfo As FieldInfo = DirectCast(_CurrentMember, FieldInfo)
                                Me._CurrentValue = fInfo.GetValue(Me.CurrentValue)
                            Case MemberTypes.Method
                                Dim mInfo As MethodInfo = DirectCast(_CurrentMember, MethodInfo)
                                Dim objIndexParameters() As ParameterInfo = mInfo.GetParameters()
                                Dim paramvalues As Object() = ReflectionHelper.BuildParameters(objIndexParameters, _
                                                        Me.Params.GetRange(Me.CurrentIndex, Me.Params.Count - Me.CurrentIndex))
                                If paramvalues IsNot Nothing Then
                                    Me.CurrentIndex += paramvalues.Length
                                End If

                                Me._CurrentValue = mInfo.Invoke(Me.CurrentValue, paramvalues)
                            Case Else
                                Throw New ArgumentException(String.Format("wrong member type in token replace, {0} is not allowed", _CurrentMember.Name))
                        End Select
                    Else
                        Me.CurrentValue = Nothing
                    End If

                    If Me.CurrentParam = LocalizedKey Then
                        Me.IsLocalized = True
                        Me.CurrentIndex += 1
                    End If
                End If
            Catch ex As Exception
                Dim message As String = String.Format("Token Replace exception with current member {0}.{1}, and current value {2} ", Me.CurrentMember.DeclaringType.Name, Me.CurrentMember.Name.ToString(), Me.CurrentValue.ToString())
                Throw New ApplicationException(message, ex)
            End Try

        End Sub

    End Class
End Namespace
