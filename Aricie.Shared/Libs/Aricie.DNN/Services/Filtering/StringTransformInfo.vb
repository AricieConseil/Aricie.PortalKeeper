Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Filtering

    ''' <summary>
    ''' Type of string filtering
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum StringFilterType
        CharsReplace
        StringReplace
        RegexReplace
    End Enum

    ''' <summary>
    ''' Class for string transformation
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    <DefaultProperty("FriendlyName")> _
    Public Class StringTransformInfo

        Public Sub New()
        End Sub

        Public Sub New(ByVal filterType As StringFilterType, ByVal sourcevalue As String, ByVal replacevalue As String)
            Me.New()
            Me._FilterType = filterType
            Me._SourceValue = sourcevalue
            Me._ReplaceValue = replacevalue
        End Sub

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName As String
            Get
                Return String.Format("{1} {0} {2} {0} {3}", Aricie.ComponentModel.UIConstants.TITLE_SEPERATOR, Me.FilterType.ToString(), SourceValue, ReplaceValue)
            End Get
        End Property

        ''' <summary>
        ''' Type of filter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <SortOrder(0)> _
        Public Property FilterType() As StringFilterType = StringFilterType.CharsReplace

        ''' <summary>
        ''' Source value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>The source value will change depending on what filter type is used</remarks>
        <SortOrder(1)> _
         <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
            LineCount(2), Width(400)> _
        Public Property SourceValue() As String = String.Empty

        ''' <summary>
        ''' Replace value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>The replace value will change depending on what filter type is used, most natbly for char replacement</remarks>
        <SortOrder(2)> _
         <Editor(GetType(CustomTextEditControl), GetType(EditControl)), _
            LineCount(2), Width(400)> _
        Public Property ReplaceValue() As String = String.Empty

        ''' <summary>
        ''' Equality
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim objToCompare As StringTransformInfo = CType(obj, StringTransformInfo)
            If objToCompare Is Nothing Then
                Return False
            End If
            Return objToCompare.FilterType = Me._FilterType _
                AndAlso objToCompare.SourceValue = Me._SourceValue _
                AndAlso objToCompare._ReplaceValue = Me._ReplaceValue

        End Function


    End Class


End Namespace
