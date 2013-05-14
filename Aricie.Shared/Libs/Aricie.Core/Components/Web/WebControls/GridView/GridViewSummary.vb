

Namespace Web.UI.Controls.GridViewGrouping
    Public Class GridViewSummary
        ' Methods
        Private Sub New(ByVal col As String, ByVal grp As GridViewGroup)
            Me._column = col
            Me._group = grp
            Me._value = Nothing
            Me._quantity = 0
            Me._automatic = True
            Me._treatNullAsZero = False
        End Sub

        Public Sub New(ByVal col As String, ByVal op As SummaryOperation, ByVal grp As GridViewGroup)
            Me.New(col, String.Empty, op, grp)
        End Sub

        Public Sub New(ByVal col As String, ByVal op As CustomSummaryOperation, ByVal getResult As SummaryResultMethod, _
                        ByVal grp As GridViewGroup)
            Me.New(col, String.Empty, op, getResult, grp)
        End Sub

        Public Sub New(ByVal col As String, ByVal formatString As String, ByVal op As SummaryOperation, _
                        ByVal grp As GridViewGroup)
            Me.New(col, grp)
            Me._formatString = formatString
            Me._operation = op
            Me._customOperation = Nothing
            Me._getSummaryMethod = Nothing
        End Sub

        Public Sub New(ByVal col As String, ByVal formatString As String, ByVal op As CustomSummaryOperation, _
                        ByVal getResult As SummaryResultMethod, ByVal grp As GridViewGroup)
            Me.New(col, grp)
            Me._formatString = formatString
            Me._operation = SummaryOperation.Custom
            Me._customOperation = op
            Me._getSummaryMethod = getResult
        End Sub

        Public Sub AddValue(ByVal newValue As Object)
            Me._quantity += 1
            If ((Me._operation = SummaryOperation.Sum) OrElse (Me._operation = SummaryOperation.Avg)) Then
                If (Me._value Is Nothing) Then
                    Me._value = newValue
                Else
                    Me._value = Me.PerformSum(Me._value, newValue)
                End If
            ElseIf (Not Me._customOperation Is Nothing) Then
                If (Not Me._group Is Nothing) Then
                    Me._customOperation.Invoke(Me._column, Me._group.Name, newValue)
                Else
                    Me._customOperation.Invoke(Me._column, Nothing, newValue)
                End If
            End If
        End Sub

        Public Sub Calculate()
            If (Me._operation = SummaryOperation.Avg) Then
                Me._value = Me.PerformDiv(Me._value, Me._quantity)
            End If
            If (Me._operation = SummaryOperation.Count) Then
                Me._value = Me._quantity
            ElseIf ((Me._operation = SummaryOperation.Custom) AndAlso (Not Me._getSummaryMethod Is Nothing)) Then
                Me._value = Me._getSummaryMethod.Invoke(Me._column, Nothing)
            End If
        End Sub

        Private Function PerformDiv(ByVal a As Object, ByVal b As Integer) As Object
            Dim zero As Object = 0
            If (a Is Nothing) Then
                Return IIf(Me._treatNullAsZero, zero, Nothing)
            End If
            If (b <> 0) Then
                Select Case a.GetType.FullName
                    Case "System.Int16"
                        Return (Convert.ToInt16(a) / b)
                    Case "System.Int32"
                        Return (Convert.ToInt32(a) / b)
                    Case "System.Int64"
                        Return (Convert.ToInt64(a) / CLng(b))
                    Case "System.UInt16"
                        Return (Convert.ToUInt16(a) / b)
                    Case "System.UInt32"
                        Return CLng((CULng(Convert.ToUInt32(a)) / CLng(b)))
                    Case "System.Single"
                        Return (Convert.ToSingle(a) / CSng(b))
                    Case "System.Double"
                        Return (Convert.ToDouble(a) / CDbl(b))
                    Case "System.Decimal"
                        Return (Convert.ToDecimal(a) / b)
                    Case "System.Byte"
                        Return (Convert.ToByte(a) / b)
                End Select
            End If
            Return Nothing
        End Function

        Private Function PerformSum(ByVal a As Object, ByVal b As Object) As Object
            If (a Is Nothing) Then
                If Not Me._treatNullAsZero Then
                    Return Nothing
                End If
                a = 0
            End If
            If (b Is Nothing) Then
                If Not Me._treatNullAsZero Then
                    Return Nothing
                End If
                b = 0
            End If
            Select Case a.GetType.FullName
                Case "System.Int16"
                    Return (Convert.ToInt16(a) + Convert.ToInt16(b))
                Case "System.Int32"
                    Return (Convert.ToInt32(a) + Convert.ToInt32(b))
                Case "System.Int64"
                    Return (Convert.ToInt64(a) + Convert.ToInt64(b))
                Case "System.UInt16"
                    Return (Convert.ToUInt16(a) + Convert.ToUInt16(b))
                Case "System.UInt32"
                    Return (Convert.ToUInt32(a) + Convert.ToUInt32(b))
                Case "System.UInt64"
                    Return (Convert.ToUInt64(a) + Convert.ToUInt64(b))
                Case "System.Single"
                    Return (Convert.ToSingle(a) + Convert.ToSingle(b))
                Case "System.Double"
                    Return (Convert.ToDouble(a) + Convert.ToDouble(b))
                Case "System.Decimal"
                    Return (Convert.ToDecimal(a) + Convert.ToDecimal(b))
                Case "System.Byte"
                    Return (Convert.ToByte(a) + Convert.ToByte(b))
                Case "System.String"
                    Return (a.ToString & b.ToString)
            End Select
            Return Nothing
        End Function

        Public Sub Reset()
            Me._quantity = 0
            Me._value = Nothing
        End Sub

        Friend Sub SetGroup(ByVal g As GridViewGroup)
            Me._group = g
        End Sub

        Public Function Validate() As Boolean
            If (Me._operation = SummaryOperation.Custom) Then
                Return ((Not Me._customOperation Is Nothing) AndAlso (Not Me._getSummaryMethod Is Nothing))
            End If
            Return ((Me._customOperation Is Nothing) AndAlso (Me._getSummaryMethod Is Nothing))
        End Function


        ' Properties
        Public Property Automatic() As Boolean
            Get
                Return Me._automatic
            End Get
            Set(ByVal value As Boolean)
                Me._automatic = value
            End Set
        End Property

        Public ReadOnly Property Column() As String
            Get
                Return Me._column
            End Get
        End Property

        Public ReadOnly Property CustomOperation() As CustomSummaryOperation
            Get
                Return Me._customOperation
            End Get
        End Property

        Public Property FormatString() As String
            Get
                Return Me._formatString
            End Get
            Set(ByVal value As String)
                Me._formatString = value
            End Set
        End Property

        Public ReadOnly Property GetSummaryMethod() As SummaryResultMethod
            Get
                Return Me._getSummaryMethod
            End Get
        End Property

        Public ReadOnly Property Group() As GridViewGroup
            Get
                Return Me._group
            End Get
        End Property

        Public ReadOnly Property Operation() As SummaryOperation
            Get
                Return Me._operation
            End Get
        End Property

        Public ReadOnly Property Quantity() As Integer
            Get
                Return Me._quantity
            End Get
        End Property

        Public Property TreatNullAsZero() As Boolean
            Get
                Return Me._treatNullAsZero
            End Get
            Set(ByVal value As Boolean)
                Me._treatNullAsZero = value
            End Set
        End Property

        Public ReadOnly Property Value() As Object
            Get
                Return Me._value
            End Get
        End Property


        ' Fields
        Private _automatic As Boolean
        Private _column As String
        Private _customOperation As CustomSummaryOperation
        Private _formatString As String
        Private _getSummaryMethod As SummaryResultMethod
        Private _group As GridViewGroup
        Private _operation As SummaryOperation
        Private _quantity As Integer
        Private _treatNullAsZero As Boolean
        Private _value As Object
    End Class
End Namespace


