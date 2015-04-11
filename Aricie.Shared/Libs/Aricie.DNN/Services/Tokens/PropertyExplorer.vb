Imports System.Reflection
Imports Aricie.Services
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Linq

Namespace Services


    ''' <summary>
    ''' Property explorer for the ATR, to go from string representation of properties to code representation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PropertyExplorer


        Private Shared ReadOnly ExpressionSplitter As New Regex(Constants.Content.ExpressionSplitCapture, RegexOptions.Compiled)
        Private Shared ReadOnly ExpressionSegmentSplitter As New Regex(Constants.Content.ExpressionSegmentCapture, RegexOptions.Compiled)


        Public Shared Function SplitExpression(exp As String) As IEnumerable(Of String)
            Dim groupsCaptures As CaptureCollection = ExpressionSplitter.Match(exp).Groups(1).Captures
            Return (From objCapture As Capture In groupsCaptures.Cast(Of Capture)() Select objCapture.Value).ToList()
        End Function

        Public Shared Function ExpressionToTokens(expression As String) As String
            Dim toReturn As New StringBuilder()
            Dim segments As IEnumerable(Of String) = SplitExpression(expression)
            For Each segment As String In SplitExpression(expression)
                toReturn.Append(":")
                Dim segmentSplit As Match = ExpressionSegmentSplitter.Match(segment)
                toReturn.Append(segmentSplit.Groups(1).Captures(0).Value)
                If segmentSplit.Groups(2).Captures.Count > 0 Then
                    toReturn.Append(":"c)
                    toReturn.Append(segmentSplit.Groups(2).Captures(0).Value)
                End If
            Next
            Return toReturn.ToString().TrimStart(":"c)
            'Return expression.Replace("."c, ":"c).Replace("['", ":").Replace("[""", ":").Replace("["c, ":").Replace("']", "").Replace("""]", "").Replace("]"c, "")
        End Function

        Public Const LocalizedKey As String = "Localized"
        Public Shared ReadOnly ArrayGetMethod As MethodInfo = GetType(Array).GetMethod("GetValue", New Type() {GetType(Integer)})

        Public Property LevelAccess As TokenLevelAccess = TokenLevelAccess.All

        Private ReadOnly _Separator As Char

        Public Sub New(ByVal expression As String, ByVal initialValue As Object, levelAccess As TokenLevelAccess)
            Me.New(expression, initialValue, ":"c)
        End Sub

        Public Sub New(ByVal expression As String, ByVal initialValue As Object, ByVal separator As Char)
            Me._Separator = separator
            If expression Is Nothing Then
                expression = ""
            End If
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


        'Private _CurrentMember As MemberInfo
        Private _CurrentMemberStack As New List(Of MemberInfo)

        ''' <summary>
        ''' Gets the current member information
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentMember() As MemberInfo
            Get
                If CurrentMemberStack.Count > 0 Then
                    Return CurrentMemberStack(CurrentMemberStack.Count - 1)
                End If
                Return Nothing
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

        Public Property CurrentMemberStack As List(Of MemberInfo)
            Get
                Return _CurrentMemberStack
            End Get
            Set(value As List(Of MemberInfo))
                _CurrentMemberStack = value
            End Set
        End Property

        ''' <summary>
        ''' Calculates deep property value
        ''' </summary>
        ''' <param name="deepAccess"></param>
        ''' <remarks></remarks>
        Public Sub GetDeepPropertyValue(ByVal deepAccess As DeepObjectPropertyAccess)

            Dim dpa As DeepObjectPropertyAccess = Nothing
            While Me.CurrentIndex < Me.Params.Count AndAlso Not Me.IsCompleted
                'Dim currentParam As String = Me.CurrentParam
                Me.GetPropertyValue()
                If Me._CurrentValue IsNot Nothing Then
                    If Not String.IsNullOrEmpty(Me.TokenQueue) Then

                        'dealing with sub properties
                        If Not deepAccess.PropertiesDeepAccess.TryGetValue(CurrentParam, dpa) Then
                            dpa = deepAccess.GetDeepPropertyAccess(CurrentParam, Me)
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
                    If Me.CurrentValue IsNot Nothing Then
                        Dim objType As Type = Me.CurrentValue.GetType
                        Dim objCurrentMember As MemberInfo = ReflectionHelper.GetMember(objType, Me.CurrentParam)
                        If objCurrentMember IsNot Nothing Then
                            Me.CurrentIndex += 1
                        Else
                            Dim defaultMembers() As MemberInfo = objType.GetDefaultMembers
                            For Each tempMember As MemberInfo In defaultMembers
                                Select Case tempMember.MemberType
                                    Case MemberTypes.Field, MemberTypes.Property, MemberTypes.Method
                                        objCurrentMember = tempMember
                                        Exit For
                                End Select
                            Next
                            If objCurrentMember Is Nothing AndAlso TypeOf Me.CurrentValue Is Array Then
                                objCurrentMember = ArrayGetMethod
                            End If
                        End If
                        If objCurrentMember IsNot Nothing Then

                            Me.CurrentMemberStack.Add(objCurrentMember)

                            If LevelAccess = TokenLevelAccess.PropertiesOnly AndAlso objCurrentMember.MemberType <> MemberTypes.Property Then
                                Throw New ArgumentException(String.Format("Only properties allowed in token replace, {0} is not allowed", objCurrentMember.Name))
                            End If

                            Select Case objCurrentMember.MemberType
                                Case MemberTypes.Property
                                    Dim propInfo As PropertyInfo = DirectCast(objCurrentMember, PropertyInfo)

                                    Dim objIndexParameters() As ParameterInfo = propInfo.GetIndexParameters()
                                    Dim paramvalues As Object() = ReflectionHelper.BuildParameters(objIndexParameters, _
                                                            Me.Params.GetRange(Me.CurrentIndex, Me.Params.Count - Me.CurrentIndex))
                                    If paramvalues IsNot Nothing Then
                                        Me.CurrentIndex += paramvalues.Length
                                    End If
                                    Try
                                        Me._CurrentValue = propInfo.GetValue(Me.CurrentValue, paramvalues)
                                    Catch ex As Exception
                                        Dim message As New StringBuilder()
                                        message.AppendLine("Evaluation Failure: ")
                                        message.Append("Parameters: ")
                                        For i As Integer = LBound(objIndexParameters) To UBound(objIndexParameters) Step 1
                                            Dim objParam As ParameterInfo = objIndexParameters(i)
                                            message.AppendLine(String.Format("{0} = {1} ;", objParam.Name, paramvalues(i)))
                                        Next
                                        Throw New ApplicationException(message.ToString(), ex)
                                    End Try
                                Case MemberTypes.Field
                                    Dim fInfo As FieldInfo = DirectCast(objCurrentMember, FieldInfo)
                                    Me._CurrentValue = fInfo.GetValue(Me.CurrentValue)
                                Case MemberTypes.Method
                                    Dim mInfo As MethodInfo = DirectCast(objCurrentMember, MethodInfo)
                                    Dim objIndexParameters() As ParameterInfo = mInfo.GetParameters()
                                    Dim paramvalues As Object() = ReflectionHelper.BuildParameters(objIndexParameters, _
                                                            Me.Params.GetRange(Me.CurrentIndex, Me.Params.Count - Me.CurrentIndex))
                                    If paramvalues IsNot Nothing Then
                                        Me.CurrentIndex += paramvalues.Length
                                    End If

                                    Me._CurrentValue = mInfo.Invoke(Me.CurrentValue, paramvalues)
                                Case Else
                                    Throw New ArgumentException(String.Format("wrong member type in token replace, {0} is not allowed", objCurrentMember.Name))
                            End Select
                        Else
                            Me.CurrentValue = Nothing
                        End If
                    Else
                        Me.CurrentIndex += 1
                    End If

                    If Me.CurrentParam = LocalizedKey Then
                        Me.IsLocalized = True
                        Me.CurrentIndex += 1
                    End If
                End If
            Catch ex As Exception
                Dim strMemberName As String = ""
                If Me.CurrentMember IsNot Nothing Then
                    strMemberName = String.Format("{0}.{1}", ReflectionHelper.GetSimpleTypeName(Me.CurrentMember.DeclaringType), Me.CurrentMember.Name.ToString())
                End If
                Dim strValue As String = ""
                If Me.CurrentValue IsNot Nothing Then
                    strValue = Me.CurrentValue.ToString
                End If
                Dim message As String = String.Format("Token Replace exception with current member {0}, and current value {1} ", strMemberName, strValue)
                Throw New ApplicationException(message, ex)
            End Try

        End Sub

    End Class
End Namespace
