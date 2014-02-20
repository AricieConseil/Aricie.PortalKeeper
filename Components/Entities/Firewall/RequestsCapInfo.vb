Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Xml.Serialization
Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.Services.Flee
Imports DotNetNuke.Framework
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Tachometer, IconOptions.Normal)> _
    <Serializable()> _
    Public Class RequestsCapInfo
        Inherits NamedEntity


        'Private _CountStartTime As DateTime = Now
        'Private _RecordLock As New Object
        <NonSerialized()> _
        Private _LockDico As New ReaderWriterLock()
        Private _Record As New Dictionary(Of String, ClientSourceCapLog)

        Private _Enabled As Boolean = True
        Private _CappedRequestScope As RequestScope
        Private _RequestSource As New RequestSource(RequestSourceType.IPAddress)

        Private _Duration As New STimeSpan(TimeSpan.FromSeconds(60))
        Private _RateSpan As TimeSpan = TimeSpan.Zero
        Private _MaxNbRequest As Integer = 100
        Private _MaxNbNewSources As Integer = 0


        Public Sub BeginRead()
            _LockDico.AcquireReaderLock(Aricie.Constants.LockTimeOut)
        End Sub

        Public Sub EndRead()
            _LockDico.ReleaseReaderLock()
        End Sub

        Private Sub BeginWrite()
            _LockDico.AcquireWriterLock(Aricie.Constants.LockTimeOut)
        End Sub

        Private Sub EndWrite()
            _LockDico.ReleaseWriterLock()
        End Sub



        Public Sub New()

        End Sub

        Public Sub New(ByVal requestSource As RequestSource, ByVal requestScope As RequestScope, ByVal maxNbRequest As Integer, ByVal duration As TimeSpan)
            Me._RequestSource = requestSource
            Me._CappedRequestScope = requestScope
            Me._MaxNbRequest = maxNbRequest
            Me._Duration = New STimeSpan(duration)
        End Sub


        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property


        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
        Public Property RequestSource() As RequestSource
            Get
                Return _RequestSource
            End Get
            Set(ByVal value As RequestSource)
                _RequestSource = value
            End Set
        End Property

        Public Property CappedRequestScope() As RequestScope
            Get
                Return _CappedRequestScope
            End Get
            Set(ByVal value As RequestScope)
                _CappedRequestScope = value
            End Set
        End Property

        <ConditionalVisible("CappedRequestScope", False, True, RequestScope.CustomCondition)> _
        Public Property RequestScopeCondition As New SimpleExpression(Of Boolean)("DnnContext.Request.Url.AbsoluteUri.Contains(""pagename.aspx"")")


        Public ReadOnly Property Rate() As Decimal
            Get
                If TimeSpan.Zero.Equals(Me._Duration.Value) Then
                    Return 0
                End If
                Return CDec(Math.Round(_MaxNbRequest * TimeSpan.FromSeconds(1).Ticks / Me._Duration.Value.Ticks, 2))
            End Get
        End Property

        Public Property MaxNbRequest() As Integer
            Get
                Return _MaxNbRequest
            End Get
            Set(ByVal value As Integer)
                _MaxNbRequest = value
            End Set
        End Property

        Public Property MaxNbNewSources() As Integer
            Get
                Return _MaxNbNewSources
            End Get
            Set(ByVal value As Integer)
                _MaxNbNewSources = value
            End Set
        End Property


        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
           <LabelMode(LabelMode.Top)> _
        Public Property Duration() As STimeSpan
            Get
                Return _Duration
            End Get
            Set(ByVal value As STimeSpan)
                _Duration = value
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property RateSpan() As TimeSpan
            Get
                If _RateSpan = TimeSpan.Zero Then
                    _RateSpan = TimeSpan.FromTicks(CInt(TimeSpan.FromSeconds(1).Ticks / Math.Abs(Me.Rate)))
                End If
                Return _RateSpan
            End Get
        End Property



        Private _CurrentCapWindow As DateTime = Now
        Private _LockCapWindow As New Object
        Private _CurrentWindowCount As Integer
        Private _BannedWindows As New Dictionary(Of DateTime, Boolean)

        Private Function IsInScope(context As PortalKeeperContext(Of RequestEvent)) As Boolean
            Select Case Me._CappedRequestScope
                Case RequestScope.Any
                    Return True
                Case RequestScope.PagesOnly
                    Return TypeOf context.DnnContext.HttpContext.CurrentHandler Is Page
                Case RequestScope.DNNPageOnly
                    Return TypeOf context.DnnContext.HttpContext.CurrentHandler Is CDefault
                Case RequestScope.CustomCondition
                    Return Me.RequestScopeCondition.Evaluate(context, context)
            End Select
        End Function


        Public Function IsValid(ByVal context As PortalKeeperContext(Of RequestEvent), ByRef clue As Object, ByRef key As String) As Boolean
            If Not Me._Enabled OrElse Not Me.IsInScope(context) Then
                Return True
            End If
            Dim dateNow As DateTime = Now
            If dateNow > _CurrentCapWindow.Add(Me.Duration) Then
                SyncLock _LockCapWindow
                    If dateNow > _CurrentCapWindow.Add(Me.Duration) Then
                        _CurrentCapWindow = dateNow
                        _CurrentWindowCount = 0
                    End If
                End SyncLock
            End If
            Dim currentLog As ClientSourceCapLog
            key = Me._RequestSource.GenerateKey(context)
            clue = Me._RequestSource
            BeginRead()
            Dim exists As Boolean = Me._Record.TryGetValue(key, currentLog)
            EndRead()
            Dim toReturn As Boolean
            If Not exists Then
                currentLog = New ClientSourceCapLog(dateNow, 1, Me._CurrentCapWindow)
                Interlocked.Increment(_CurrentWindowCount)
                If Me._MaxNbNewSources > 0 AndAlso _CurrentWindowCount > Me._MaxNbNewSources Then
                    SyncLock _LockCapWindow
                        If _CurrentWindowCount > Me._MaxNbNewSources Then
                            _BannedWindows(_CurrentCapWindow) = True
                        End If
                    End SyncLock
                End If
                toReturn = True
            Else
                If dateNow.Subtract(currentLog.LastRequestTime) > Duration.Value Then
                    toReturn = True
                    Dim nb As Integer = Math.Max(1, currentLog.NbRequests - 1)
                    currentLog = New ClientSourceCapLog(dateNow.Subtract(Duration.Value).Add(RateSpan), nb, currentLog.CapWindow)
                    Thread.Sleep(Convert.ToInt32(Math.Pow(100, nb / Me._MaxNbRequest)))
                Else
                    If currentLog.NbRequests < Me._MaxNbRequest Then
                        currentLog = New ClientSourceCapLog(currentLog.LastRequestTime, currentLog.NbRequests + 1, currentLog.CapWindow)
                        toReturn = True
                    Else
                        If currentLog.LastRequestTime < dateNow.Subtract(RateSpan) Then
                            currentLog = New ClientSourceCapLog(currentLog.LastRequestTime.Add(RateSpan), currentLog.NbRequests, currentLog.CapWindow)
                        End If
                    End If
                End If
            End If
            If toReturn AndAlso _BannedWindows.ContainsKey(currentLog.CapWindow) Then
                toReturn = False
            End If
            'currentFlag = New KeyValuePair(Of DateTime, Integer)(currentFlag.Key.Add(TimeSpan.FromSeconds(1)), currentFlag.Value - CInt(Math.Ceiling(Me.Rate)))


            'SyncLock _RecordLock
            BeginWrite()
            Me._Record(key) = currentLog
            EndWrite()
            'End SyncLock
            Return toReturn
        End Function





    End Class
End Namespace