Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Entities
    ''' <summary>
    ''' General Entity class to hold common metadata
    ''' </summary>
    <ActionButton(IconName.Tags, IconOptions.Normal)> _
    Public Class MetaDataInfo

#Region "Private members"

        Private _title As String = String.Empty
        Private _metaTitle As String = String.Empty
        Private _description As String = String.Empty
        Private _keyWords As String = String.Empty
        Private _OverrideKeywords As Boolean
        Private _Category As String = String.Empty
        Private _logo As String = String.Empty
        Private _frequency As Integer
        Private _priority As Decimal
        Private _rating As Integer
        Private _permanent As Boolean
        Private _startDate As Date
        Private _endDate As Date

#End Region

#Region "Public Properties"

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Property MetaTitle() As String
            Get
                Return _metaTitle
            End Get
            Set(ByVal value As String)
                _metaTitle = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property KeyWords() As String
            Get
                Return _keyWords
            End Get
            Set(ByVal value As String)
                _keyWords = value
            End Set
        End Property

        Public Property OverrideKeywords() As Boolean
            Get
                Return _OverrideKeywords
            End Get
            Set(ByVal value As Boolean)
                _OverrideKeywords = value
            End Set
        End Property

        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal value As String)
                _Category = value
            End Set
        End Property

        Public Property Frequency() As Integer
            Get
                Return _frequency
            End Get
            Set(ByVal value As Integer)
                _frequency = value
            End Set
        End Property

        <Editor("Aricie.DNN.UI.WebControls.EditControls.DecimalEditControl, Aricie.DNN", GetType(EditControl))> _
        Public Property Priority() As Decimal
            Get
                Return _priority
            End Get
            Set(ByVal value As Decimal)
                _priority = value
            End Set
        End Property

        ''' <summary>
        ''' Le fileId DNN du logo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Editor("Aricie.DNN.UI.WebControls.EditControls.UploadEditControl, Aricie.DNN", GetType(EditControl)), _
            FileExtensions("jpg,gif,png")> _
        Public Property Logo() As String
            Get
                Return _logo
            End Get
            Set(ByVal value As String)
                _logo = value
            End Set
        End Property

        Public Property Rating() As Integer
            Get
                Return _rating
            End Get
            Set(ByVal value As Integer)
                _rating = value
            End Set
        End Property

        Public Property Permant() As Boolean
            Get
                Return _permanent
            End Get
            Set(ByVal value As Boolean)
                _permanent = value
            End Set
        End Property

        <Editor("DotNetNuke.UI.WebControls.DateEditControl, DotNetNuke", GetType(EditControl))> _
        Public Property StartDate() As Date
            Get
                Return _startDate
            End Get
            Set(ByVal value As Date)
                _startDate = value
            End Set
        End Property

        <Editor("DotNetNuke.UI.WebControls.DateEditControl, DotNetNuke", GetType(EditControl))> _
        Public Property EndDate() As Date
            Get
                Return _endDate
            End Get
            Set(ByVal value As Date)
                _endDate = value
            End Set
        End Property

#End Region


    End Class

End Namespace
