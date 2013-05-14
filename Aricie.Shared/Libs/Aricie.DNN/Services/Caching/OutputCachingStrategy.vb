Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Web
Imports System.Text

Namespace Services.Caching
    <Serializable()> _
    Public Class OutputCachingStrategy

        Private _Enabled As Boolean = True
        Private _Mode As OutputCacheMode = OutputCacheMode.Cache
        Private _Duration As New STimeSpan(TimeSpan.FromMinutes(30))
        Private _EmptyPathInfoOnly As Boolean
        Private _EmptyQueryStringOnly As Boolean
        Private _VaryByMode As VaryByMode = Caching.VaryByMode.AllButList
        Private _VaryByList As List(Of String)
        Private _VaryBy As String = ""
        Private _VaryByBrowser As Boolean = True
        Private _Verbs As String = "get"
        Private _VerbsList As List(Of String)



        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        Public Property Mode() As OutputCacheMode
            Get
                Return _Mode
            End Get
            Set(ByVal value As OutputCacheMode)
                _Mode = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property VerbsList() As List(Of String)
            Get
                If _VerbsList Is Nothing Then
                    SyncLock _Verbs
                        If _VerbsList Is Nothing Then
                            _VerbsList = Common.ParseStringList(_Verbs)
                        End If
                    End SyncLock
                End If
                Return _VerbsList
            End Get
        End Property

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(2), Width(400), LabelMode(LabelMode.Top)> _
        Public Property Verbs() As String
            Get
                Return _Verbs
            End Get
            Set(ByVal value As String)
                _Verbs = value
                _VerbsList = Nothing
            End Set
        End Property

        Public Property EmptyPathInfoOnly() As Boolean
            Get
                Return _EmptyPathInfoOnly
            End Get
            Set(ByVal value As Boolean)
                _EmptyPathInfoOnly = value
            End Set
        End Property



        Public Property EmptyQueryStringOnly() As Boolean
            Get
                Return _EmptyQueryStringOnly
            End Get
            Set(ByVal value As Boolean)
                _EmptyQueryStringOnly = value
            End Set
        End Property


        Public Property Duration() As STimeSpan
            Get
                Return _Duration
            End Get
            Set(ByVal value As STimeSpan)
                _Duration = value
            End Set
        End Property


        Public Property VaryByMode() As VaryByMode
            Get
                Return _VaryByMode
            End Get
            Set(ByVal value As VaryByMode)
                _VaryByMode = value
            End Set
        End Property

        <Browsable(False)> _
              Public ReadOnly Property VaryByList() As List(Of String)
            Get
                If _VaryByList Is Nothing Then
                    SyncLock _VaryBy
                        If _VaryByList Is Nothing Then
                            _VaryByList = Common.ParseStringList(_VaryBy)
                        End If
                    End SyncLock
                End If
                Return _VaryByList
            End Get
        End Property

        <Editor(GetType(CustomTextEditControl), GetType(EditControl))> _
            <LineCount(3), Width(400), LabelMode(LabelMode.Top)> _
        Public Property VaryBy() As String
            Get
                Return _VaryBy
            End Get
            Set(ByVal value As String)
                _VaryBy = value
                _VaryByList = Nothing
            End Set
        End Property

        Public Property VaryByBrowser() As Boolean
            Get
                Return _VaryByBrowser
            End Get
            Set(ByVal value As Boolean)
                _VaryByBrowser = value
            End Set
        End Property


        Public Function CalculateVaryByKey(ByVal request As HttpRequest) As String
            If Me.VaryByList.Count = 0 Then
                Return String.Empty
            End If
            If ((Me.VaryByList.Count = 1) AndAlso (Me.VaryByList(0) = "*")) Then
                Return request.QueryString.ToString
            End If
            Dim builder As New StringBuilder
            Dim i As Integer
            For i = 0 To Me.VaryByList.Count - 1
                builder.Append(request.QueryString.Item(Me.VaryByList(i)))
                builder.Append("-"c)
            Next i
            Return HttpUtility.UrlEncodeUnicode(builder.ToString)
        End Function



        Private Class VaryByListAttributes
            Implements IAttributesProvider

            Public Function GetAttributes() As System.Collections.Generic.IEnumerable(Of System.Attribute) Implements IAttributesProvider.GetAttributes
                Dim toReturn As New List(Of Attribute)
                toReturn.Add(New WidthAttribute(300))
                Return toReturn
            End Function
        End Class

    End Class
End Namespace