Imports Aricie.DNN.Services.Workers
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.Services.Filtering
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum PickMethod
        Sequential
        Random
    End Enum


    <ActionButton(IconName.Compass, IconOptions.Normal)>
    Public Class WebProxyPool
        Implements IDisposable



        Private _Checked As New Dictionary(Of String, Boolean)
        Private _CheckTaskQueueInfo As New TaskQueueInfo(1, True, TimeSpan.Zero, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10))
        Private _CheckQueue As TaskQueue(Of WebProxyInfo)
        Private _CheckPeriod As New STimeSpan(TimeSpan.FromMinutes(5))
        Private _TimeOut As New STimeSpan(TimeSpan.FromSeconds(5))
        Private _Proxies As New SimpleList(Of WebProxyInfo)

        Private _TestProxies As Boolean
        Private _TestUri As String = ""
        Private _TestXPath As String = ""
        Private _MinNbProxies As Integer = 5
        Private _MaxNbProxies As Integer = 30
        'Private _Proxies As New SerializableDictionary(Of String, WebProxyInfo)






        <Browsable(False)>
        <XmlIgnore()>
        Public ReadOnly Property CheckQueue() As TaskQueue(Of WebProxyInfo)
            Get
                If _CheckQueue Is Nothing Then
                    _CheckQueue = New TaskQueue(Of WebProxyInfo)(AddressOf Me.CheckProxy, Me._CheckTaskQueueInfo)
                End If
                Return _CheckQueue
            End Get
        End Property

        <ExtendedCategory("Proxies")>
        Public Property PickMethod As PickMethod = PickMethod.Random


        <ExtendedCategory("Proxies")>
        <LabelMode(LabelMode.Top)>
        Public Property Proxies() As SimpleList(Of WebProxyInfo)
            Get
                Return _Proxies
            End Get
            Set(ByVal value As SimpleList(Of WebProxyInfo))
                _Proxies = value
            End Set
        End Property



        <ExtendedCategory("Size")>
        Public Property MinNbProxies() As Integer
            Get
                Return _MinNbProxies
            End Get
            Set(ByVal value As Integer)
                _MinNbProxies = value
            End Set
        End Property


        <ExtendedCategory("Size")>
        Public Property MaxNbProxies() As Integer
            Get
                Return _MaxNbProxies
            End Get
            Set(ByVal value As Integer)
                _MaxNbProxies = value
            End Set
        End Property




        <ExtendedCategory("Verification")>
        Public Property TestProxies() As Boolean
            Get
                Return _TestProxies
            End Get
            Set(ByVal value As Boolean)
                _TestProxies = value
            End Set
        End Property



        <Required(True)>
        <Editor(GetType(CustomTextEditControl), GetType(EditControl))>
        <Width(500)>
        <ConditionalVisible("TestProxies", False, True)>
        <ExtendedCategory("Verification")>
        Public Property TestUri() As String
            Get
                Return _TestUri
            End Get
            Set(ByVal value As String)
                _TestUri = value
            End Set
        End Property



        <Editor(GetType(CustomTextEditControl), GetType(EditControl))>
        <Width(500)>
        <ConditionalVisible("TestProxies", False, True)>
        <ExtendedCategory("Verification")>
        Public Property TestXPath() As String
            Get
                Return _TestXPath
            End Get
            Set(ByVal value As String)
                _TestXPath = value
            End Set
        End Property

        <ConditionalVisible("TestProxies", False, True)>
        <ExtendedCategory("Verification")>
        Public Property CheckPeriod() As STimeSpan
            Get
                Return _CheckPeriod
            End Get
            Set(ByVal value As STimeSpan)
                _CheckPeriod = value
            End Set
        End Property

        <ConditionalVisible("TestProxies", False, True)>
        <ExtendedCategory("Verification")>
        Public Property TimeOut() As STimeSpan
            Get
                Return _TimeOut
            End Get
            Set(ByVal value As STimeSpan)
                _TimeOut = value
            End Set
        End Property

        <ConditionalVisible("TestProxies", False, True)>
        <ExtendedCategory("Verification")>
        Public Property CheckTaskQueueInfo() As TaskQueueInfo
            Get
                Return _CheckTaskQueueInfo
            End Get
            Set(ByVal value As TaskQueueInfo)
                _CheckTaskQueueInfo = value
            End Set
        End Property



        Public Sub Add(ByVal webProxy As WebProxyInfo)
            If Me._Proxies.Instances.Count < Me._MaxNbProxies Then
                If webProxy IsNot Nothing Then
                    If Me._TestProxies Then
                        If Not _Checked.ContainsKey(webProxy.Address) Then
                            SyncLock Me._Checked
                                If Not _Checked.ContainsKey(webProxy.Address) Then
                                    Dim skipProxy As Boolean
                                    If Me._Proxies.Instances.Count < Me._MinNbProxies Then
                                        Me.CheckProxy(webProxy)
                                        skipProxy = Not webProxy.Available
                                    Else
                                        Me._Checked(webProxy.Address) = True
                                        webProxy.CheckEnqueued = True
                                        Me.CheckQueue.EnqueueTask(webProxy)
                                    End If
                                    If Not skipProxy Then
                                        SyncLock _Proxies
                                            Me._Proxies.Instances.Add(webProxy)
                                        End SyncLock
                                    End If
                                End If
                            End SyncLock
                        End If
                    Else
                        SyncLock _Proxies
                            Me._Proxies.Instances.Add(webProxy)
                        End SyncLock
                    End If
                End If
            End If
        End Sub



        <Browsable(False)>
        <XmlIgnore()>
        Public ReadOnly Property AvailableProxies() As List(Of WebProxyInfo)
            Get
                Dim toReturn As List(Of WebProxyInfo)
                If Me._TestProxies Then
                    toReturn = New List(Of WebProxyInfo)
                    For Each objProxy As WebProxyInfo In New List(Of WebProxyInfo)(Me._Proxies.Instances)
                        If Not objProxy.CheckEnqueued Then
                            If objProxy.Available Then
                                toReturn.Add(objProxy)
                            End If
                            If objProxy.NeedsChecking(Me._CheckPeriod.Value) Then
                                objProxy.CheckEnqueued() = True
                                Me.CheckQueue.EnqueueTask(objProxy)
                            End If
                        End If
                    Next
                Else
                    toReturn = Me._Proxies.Instances
                End If
                Return toReturn
            End Get
        End Property



        Private randomPicker As New Random
        Private seqLock As New Object
        Private nextSequentialIndex As Integer = 0

        Public Function GetOne() As WebProxyInfo
            Dim avProxies As List(Of WebProxyInfo) = AvailableProxies
            If avProxies.Count > 0 Then
                Dim idx As Integer
                Select Case PickMethod
                    Case PickMethod.Random
                        idx = randomPicker.Next(0, avProxies.Count - 1)
                    Case PickMethod.Sequential
                        SyncLock seqLock
                            If nextSequentialIndex > avProxies.Count - 1 Then
                                nextSequentialIndex = 0
                            End If
                            idx = nextSequentialIndex
                            nextSequentialIndex += 1
                        End SyncLock
                End Select
                Return avProxies(idx)
            End If
            Return Nothing
        End Function

        Public Sub Clean()
            SyncLock Me._Proxies
                Dim clearList As New List(Of WebProxyInfo)(Me._Proxies.Instances)
                For Each objProxy As WebProxyInfo In clearList
                    If Not objProxy.Available Then
                        objProxy.Disabled = True
                        Me._Proxies.Instances.Remove(objProxy)
                    End If
                Next
            End SyncLock
        End Sub

        Private Sub CheckProxy(ByVal objProxy As WebProxyInfo)
            objProxy.CheckAvailibility(Me._TimeOut, Me._TestUri, AddressOf Me.CheckResponse)
            objProxy.CheckEnqueued = False
        End Sub


        Private _XPathInfo As XPathInfo

        Private ReadOnly Property XPathInfo As XPathInfo
            Get
                If _XPathInfo Is Nothing Then
                    _XPathInfo = New XPathInfo(Me._TestXPath, True, True)
                End If
                Return _XPathInfo
            End Get
        End Property



        Private Function CheckResponse(ByVal response As String) As Boolean
            If (Not String.IsNullOrEmpty(response)) Then
                If Me._TestXPath <> "" Then
                    Return Not String.IsNullOrEmpty(DirectCast(Me.XPathInfo.DoSelect(response), String))
                Else
                    Return True
                End If
            End If
            Return False
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Me._CheckQueue.Terminate()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace