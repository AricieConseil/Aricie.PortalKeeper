Imports System.Web.UI

Namespace Web.UI.Controls.GridViewGrouping
    Public Class GridViewGroup
        ' Methods
        Public Sub New(ByVal cols As String(), ByVal [auto] As Boolean, ByVal hideGroupColumns As Boolean)
            Me.New(cols, [auto], hideGroupColumns, False)
        End Sub

        Public Sub New(ByVal cols As String(), ByVal [auto] As Boolean, ByVal hideGroupColumns As Boolean, _
                        ByVal generateAllCellsOnSummaryRow As Boolean)
            Me.New(cols, False, [auto], hideGroupColumns, generateAllCellsOnSummaryRow)
        End Sub

        Public Sub New(ByVal cols As String(), ByVal isSuppressGroup As Boolean, ByVal [auto] As Boolean, _
                        ByVal hideGroupColumns As Boolean, ByVal generateAllCellsOnSummaryRow As Boolean)
            Me.mSummaries = New GridViewSummaryList
            Me._actualValues = Nothing
            Me._quantity = 0
            Me._columns = cols
            Me._isSuppressGroup = isSuppressGroup
            Me._automatic = [auto]
            Me._hideGroupColumns = hideGroupColumns
            Me._generateAllCellsOnSummaryRow = generateAllCellsOnSummaryRow
        End Sub

        Public Sub AddSummary(ByVal s As GridViewSummary)
            If Me.ContainsSummary(s) Then
                Throw New Exception("Summary already exists in this group.")
            End If
            If Not s.Validate Then
                Throw New Exception("Invalid summary.")
            End If
            s.SetGroup(Me)
            Me.mSummaries.Add(s)
        End Sub

        Public Sub AddValueToSummaries(ByVal dataitem As Object)
            Me._quantity += 1
            Dim s As GridViewSummary
            For Each s In Me.mSummaries
                s.AddValue(DataBinder.Eval(dataitem, s.Column))
            Next
        End Sub

        Public Sub CalculateSummaries()
            Dim s As GridViewSummary
            For Each s In Me.mSummaries
                s.Calculate()
            Next
        End Sub

        Public Function ContainsSummary(ByVal s As GridViewSummary) As Boolean
            Return Me.mSummaries.Contains(s)
        End Function

        Public Sub Reset()
            Me._quantity = 0
            Dim s As GridViewSummary
            For Each s In Me.mSummaries
                s.Reset()
            Next
        End Sub

        Friend Sub SetActualValues(ByVal values As Object())
            Me._actualValues = values
        End Sub


        ' Properties
        Public ReadOnly Property ActualValues() As Object()
            Get
                Return Me._actualValues
            End Get
        End Property

        Public Property Automatic() As Boolean
            Get
                Return Me._automatic
            End Get
            Set(ByVal value As Boolean)
                Me._automatic = value
            End Set
        End Property

        Public ReadOnly Property Columns() As String()
            Get
                Return Me._columns
            End Get
        End Property

        Public Property GenerateAllCellsOnSummaryRow() As Boolean
            Get
                Return Me._generateAllCellsOnSummaryRow
            End Get
            Set(ByVal value As Boolean)
                Me._generateAllCellsOnSummaryRow = value
            End Set
        End Property

        Public Property HideGroupColumns() As Boolean
            Get
                Return Me._hideGroupColumns
            End Get
            Set(ByVal value As Boolean)
                Me._hideGroupColumns = value
            End Set
        End Property

        Public ReadOnly Property IsSuppressGroup() As Boolean
            Get
                Return Me._isSuppressGroup
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return String.Join("+", Me._columns)
            End Get
        End Property

        Public ReadOnly Property Quantity() As Integer
            Get
                Return Me._quantity
            End Get
        End Property

        Public ReadOnly Property Summaries() As GridViewSummaryList
            Get
                Return Me.mSummaries
            End Get
        End Property


        ' Fields
        Private _actualValues As Object()
        Private _automatic As Boolean
        Private _columns As String()
        Private _generateAllCellsOnSummaryRow As Boolean
        Private _hideGroupColumns As Boolean
        Private _isSuppressGroup As Boolean
        Private _quantity As Integer
        Private mSummaries As GridViewSummaryList
    End Class
End Namespace


