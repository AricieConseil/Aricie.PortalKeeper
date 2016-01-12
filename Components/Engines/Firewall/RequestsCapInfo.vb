Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.UI.WebControls
Imports System.Threading
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Flee
Imports DotNetNuke.Framework
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Services
Imports System.Globalization
Imports System.IO
Imports Aricie.DNN.ComponentModel
Imports Aricie.IO

Namespace Aricie.DNN.Modules.PortalKeeper


    <ActionButton(IconName.ExclamationTriangle, IconOptions.Normal)> _
    <DefaultProperty("FriendlyName")> _
    Public Class RequestsCapInfo
        Implements IEnabled



        '<NonSerialized()> _
        Private ReadOnly _LockDico As New ReaderWriterLock()
        Private ReadOnly _Record As New Dictionary(Of String, ClientSourceCapLog)

        Private _Enabled As Boolean = True
        Private _CappedRequestScope As RequestScope
        Private _RequestSource As New RequestSource(RequestSourceType.IPAddress)

        Private _Duration As New STimeSpan(TimeSpan.FromSeconds(60))
        Private _RequestRateSpan As TimeSpan = TimeSpan.Zero
        Private _MaxNbRequest As Integer = 100
        Private _MaxNbNewSources As Integer = 0


        Public Sub BeginRead()
            _LockDico.AcquireReaderLock(Constants.LockTimeOut)
        End Sub

        Public Sub EndRead()
            _LockDico.ReleaseReaderLock()
        End Sub

        Private Sub BeginWrite()
            _LockDico.AcquireWriterLock(Constants.LockTimeOut)
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

        <Browsable(False)> _
        Public ReadOnly Property FriendlyName() As String
            Get
                Return String.Format("Max {1} Requests ({2}) / sec{0} per {3} over {4}{0}Max{5} KB{0}Max {6} new sources", _
                                     UIConstants.TITLE_SEPERATOR, Me.Rate.ToString(CultureInfo.InvariantCulture), Me.CappedRequestScope.ToString(), _
                                     Me.RequestSource.SourceType.ToString(), Me.Duration.ToString(), _
                                     Me.MaxTotalKBs.ToString(CultureInfo.InvariantCulture), Me.MaxNbNewSources.ToString(CultureInfo.InvariantCulture))
            End Get
        End Property


        Public Property Enabled() As Boolean Implements IEnabled.Enabled
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        Public Property Decription() As New CData


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

        Public Property MaxNbNewSources() As Integer
            Get
                Return _MaxNbNewSources
            End Get
            Set(ByVal value As Integer)
                _MaxNbNewSources = value
            End Set
        End Property

        Public Property MaxTotalKBs As Integer = 100000


        Public Property MeasureResponseSize As Boolean




        <Browsable(False)> _
        Public ReadOnly Property RequestRateSpan() As TimeSpan
            Get
                If _RequestRateSpan = TimeSpan.Zero Then
                    _RequestRateSpan = TimeSpan.FromTicks(CInt(TimeSpan.FromSeconds(1).Ticks / Math.Abs(Me.Rate)))
                End If
                Return _RequestRateSpan
            End Get
        End Property




        Private _CurrentCapWindow As DateTime = PerformanceLogger.Now
        Private ReadOnly _LockCapWindow As New Object
        Private _CurrentWindowCount As Integer
        Private ReadOnly _BannedWindows As New Dictionary(Of DateTime, Boolean)

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
            Dim dateNow As DateTime = PerformanceLogger.Now
            If dateNow > _CurrentCapWindow.Add(Me.Duration) Then
                SyncLock _LockCapWindow
                    If dateNow > _CurrentCapWindow.Add(Me.Duration) Then
                        _CurrentCapWindow = dateNow
                        _CurrentWindowCount = 0
                    End If
                End SyncLock
            End If
            Dim currentHttpContext As HttpContext = DnnContext.Current.HttpContext


            Dim currentLog As ClientSourceCapLog
            key = Me._RequestSource.GenerateKey(context)
            clue = Me._RequestSource

            BeginRead()
            Dim exists As Boolean = Me._Record.TryGetValue(key, currentLog)
            EndRead()
            Dim toReturn As Boolean

            Dim requestLength As Integer
            If currentHttpContext IsNot Nothing Then
                requestLength = currentHttpContext.Request.ContentLength
                If Me.MeasureResponseSize Then
                    Dim nonByRefKey As String = key
                    Dim measuringFilter As New ResponseFilterStream(currentHttpContext.Response.Filter, currentHttpContext, ResponseFilterType.SignalLengths)
                    currentHttpContext.Response.Filter = measuringFilter
                    AddHandler measuringFilter.SignalLengths, Sub(sender As Object, e As ChangedEventArgs(Of Long))
                                                                  Dim responseLength As Long = e.NewValue
                                                                  Dim responseCurrentLog As ClientSourceCapLog
                                                                  BeginRead()
                                                                  Dim responseExists As Boolean = Me._Record.TryGetValue(nonByRefKey, responseCurrentLog)
                                                                  EndRead()
                                                                  If responseExists Then
                                                                      responseCurrentLog = New ClientSourceCapLog(responseCurrentLog.FirstRequestTime, _
                                                                                                             responseCurrentLog.NbRequests, _
                                                                                                             responseCurrentLog.CapWindow, _
                                                                                                             responseCurrentLog.TotalBytes + responseLength)
                                                                      BeginWrite()
                                                                      Me._Record(nonByRefKey) = responseCurrentLog
                                                                      EndWrite()
                                                                  End If
                                                              End Sub

                End If
            End If

            If Not exists Then
                currentLog = New ClientSourceCapLog(dateNow, 1, Me._CurrentCapWindow, requestLength)
                Interlocked.Increment(_CurrentWindowCount)
                If Me._MaxNbNewSources > 0 AndAlso _CurrentWindowCount > Me._MaxNbNewSources Then
                    SyncLock _BannedWindows
                        _BannedWindows(_CurrentCapWindow) = True
                    End SyncLock
                End If
                toReturn = True
            Else
                Dim localWindowSpan As TimeSpan = dateNow.Subtract(currentLog.FirstRequestTime)
                If localWindowSpan > Duration.Value Then
                    'the current individual window has reached duration, reset window 
                    toReturn = True

                    currentLog = New ClientSourceCapLog(dateNow, 1, currentLog.CapWindow, requestLength)
                Else
                    'the current individual window is still below duration, keep counting within existing window
                    Dim totalBytes As Long = currentLog.TotalBytes + requestLength
                    If currentLog.NbRequests < Me._MaxNbRequest AndAlso (Me.MaxTotalKBs = 0 OrElse totalBytes < Me.MaxTotalKBs * 1000) Then
                        'the request is below cap, increment existing counter without changing reference window
                        currentLog = New ClientSourceCapLog(currentLog.FirstRequestTime, currentLog.NbRequests + 1, currentLog.CapWindow, totalBytes)
                        toReturn = True
                    Else

                        'the request reached limit, slide window with ratespan and same request number, leaving the opportunity to recover
                        Dim nb As Integer = Math.Max(1, currentLog.NbRequests - 1)
                        currentLog = New ClientSourceCapLog(currentLog.FirstRequestTime.Add(RequestRateSpan), nb, currentLog.CapWindow, totalBytes * (nb - 1) \ nb)
                    End If
                End If
            End If
            If toReturn AndAlso Me._MaxNbNewSources > 0 AndAlso _BannedWindows.ContainsKey(currentLog.CapWindow) Then
                toReturn = False
            End If


            BeginWrite()
            Me._Record(key) = currentLog
            EndWrite()
            Return toReturn
        End Function



    End Class
End Namespace