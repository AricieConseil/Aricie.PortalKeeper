Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Web
Imports System.Web.Compilation

Namespace Services.Caching

    Public MustInherit Class OutputTrackerBase

        Protected _capturingEvent As ManualResetEvent

        Protected Path As String
        Protected Strategy As OutputCachingStrategy
        Protected CachedOutput As CachedOutput


        Protected VaryById As String = ""
        Private _VaryByTrackers As Dictionary(Of String, OutputTrackerBase)

        Protected _parent As OutputTrackerBase

        Private _triedToLoadCachedResponse As Integer
        Private _capturingResponse As HttpResponse
        Protected CapturingFilter As CaptureStream
        Protected NextResponseValidationTime As DateTime


        Public Sub New(ByVal path As String, ByVal objStrategy As OutputCachingStrategy)
            Me.Path = path
            Me.Strategy = objStrategy
            Me._capturingEvent = New ManualResetEvent(True)
        End Sub

        Protected Sub New(ByVal parent As OutputTrackerBase, ByVal varyById As String)
            Me.New(parent.Path, parent.Strategy)
            Me._parent = parent
            Me.VaryById = varyById
        End Sub

        Public Overridable Function FindTrackerForRequest(ByVal request As HttpRequest) As OutputTrackerBase
            Dim toReturn As OutputTrackerBase = Nothing
            If (String.Compare(Me.Path, request.FilePath, StringComparison.OrdinalIgnoreCase) = 0) Then
                For i = 0 To Me.Strategy.VerbsList.Count - 1
                    If (String.Compare(Me.Strategy.VerbsList(i), request.HttpMethod, StringComparison.OrdinalIgnoreCase) = 0) Then
                        If (Not Me.Strategy.EmptyPathInfoOnly OrElse String.IsNullOrEmpty(request.PathInfo)) Then
                            If (Not Me.Strategy.EmptyQueryStringOnly OrElse (request.QueryString.Count > 0)) Then
                                If (Me.Strategy.VaryByList.Count = 0) Then
                                    toReturn = Me
                                Else
                                    Dim key As String = Me.Strategy.CalculateVaryByKey(request)
                                    If _VaryByTrackers Is Nothing Then
                                        _VaryByTrackers = New Dictionary(Of String, OutputTrackerBase)
                                    End If
                                    If (Not Me._VaryByTrackers.TryGetValue(key, toReturn) AndAlso (Me._VaryByTrackers.Count < OutputCacheManager.VaryByLimit)) Then
                                        SyncLock Me._VaryByTrackers
                                            If (Not Me._VaryByTrackers.TryGetValue(key, toReturn) AndAlso (Me._VaryByTrackers.Count < OutputCacheManager.VaryByLimit)) Then
                                                toReturn = New FileOutputTracker(Me, key)
                                                Me._VaryByTrackers.Add(key, toReturn)
                                            End If
                                        End SyncLock
                                    End If
                                End If
                            End If
                        End If
                        Exit For
                    End If
                Next i
            End If
            Return toReturn
        End Function

        Public Function TrySendResponseOrStartResponseCapture(ByVal response As HttpResponse) As Boolean
            Dim outputToSend As CachedOutput = Nothing
            Do While True
                SyncLock Me
                    If Not Thread.VolatileRead(Me._triedToLoadCachedResponse) = 1 Then
                        Me.TryLoadCachedResponse()
                        Thread.VolatileWrite(Me._triedToLoadCachedResponse, 1)
                    End If
                    Me.ValidateLoadedCachedResponse()
                    outputToSend = Me.CachedOutput
                    If outputToSend IsNot Nothing Then
                        Exit Do
                    End If
                    If (Me._capturingResponse Is Nothing) Then

                        Me.CapturingFilter = Me.LoadCaptureFilter(response)
                        response.Filter = Me.CapturingFilter
                        Me._capturingEvent.Reset()
                        Me._capturingResponse = response
                        Exit Do
                    End If
                End SyncLock
                Me._capturingEvent.WaitOne()
            Loop
            If outputToSend IsNot Nothing Then
                If Not String.IsNullOrEmpty(outputToSend.ResponseFileName) Then
                    Try
                        response.TransmitFile(outputToSend.ResponseFileName)
                    Catch
                        Me.InvalidateCachedResponse()
                        Throw
                    End Try
                    Return True
                ElseIf (Not outputToSend.ResponseBytes Is Nothing) Then
                    response.OutputStream.Write(outputToSend.ResponseBytes, 0, outputToSend.ResponseBytes.Length)
                    Return True
                End If
            End If
            Return False
        End Function

        Public Sub FinishCaptureAndSaveResponse(ByVal response As HttpResponse)
            If (Not Me._capturingResponse Is response) Then
                Throw New InvalidOperationException("Attempt to complete response capture occured on wrong HttpResponse")
            End If
            Me.FinishOrCancelCapture(response, False)
        End Sub

        Public Sub CancelCapture(ByVal response As HttpResponse)
            If ((Not Me._capturingResponse Is Nothing) AndAlso (Not Me._capturingResponse Is response)) Then
                Throw New InvalidOperationException("Attempt to cancel response capture occured on wrong HttpResponse")
            End If
            Me.FinishOrCancelCapture(response, True)
        End Sub

#Region "Abstract Methods"


        Protected MustOverride Sub TryLoadCachedResponse()

        Protected MustOverride Sub FinishCapture(ByVal response As HttpResponse)

        Protected MustOverride Function LoadCaptureFilter(ByVal response As HttpResponse) As CaptureStream

#End Region


        Protected Overridable Sub InvalidateCachedResponse()
            Me.CachedOutput = Nothing
        End Sub

        Public Shared Function CalculateHandlerHash(ByVal path As String) As Long
            Dim num As Long = 0
            Try
                Dim compiledType As Type = BuildManager.GetCompiledType(path)
                If (Not compiledType Is Nothing) Then
                    Dim assemblyQualifiedName As String = compiledType.AssemblyQualifiedName
                    Dim fullyQualifiedName As String = compiledType.Module.FullyQualifiedName
                    num = CLng(((assemblyQualifiedName.GetHashCode And &HFFFFFFFF) + ((fullyQualifiedName.GetHashCode And &HFFFFFFFF) << &H1D)))
                End If
            Catch
            End Try
            Return num
        End Function


#Region "Private methods"

        Private Sub ValidateLoadedCachedResponse()

            Dim tempResponse As CachedOutput = Me.CachedOutput
            If tempResponse IsNot Nothing Then
                Dim utcNow As DateTime = DateTime.UtcNow
                Dim invalidate As Boolean = (tempResponse.ResponseExpiry <= utcNow)
                If Not invalidate AndAlso utcNow > Me.NextResponseValidationTime Then
                    invalidate = (tempResponse.ResponseHash <> OutputTrackerBase.CalculateHandlerHash(Me.Path))
                    Me.NextResponseValidationTime = (utcNow + OutputCacheManager.FileValidationDelay)
                End If
                If invalidate Then
                    Me.InvalidateCachedResponse()
                End If
            End If

        End Sub


        Private Sub FinishOrCancelCapture(ByVal response As HttpResponse, ByVal cancel As Boolean)
            If (Me._capturingResponse Is response) Then
                If (Me.CapturingFilter Is Nothing) Then
                    Throw New InvalidOperationException("Response capturing filter is missing.")
                End If
                SyncLock Me
                    Try
                        Me.CapturingFilter.StopFiltering(cancel)
                        If Not cancel Then
                            Me.FinishCapture(response)
                        End If
                    Finally
                        Me.CapturingFilter = Nothing
                        Me._capturingResponse = Nothing
                        Me._capturingEvent.Set()
                    End Try
                End SyncLock
            End If
        End Sub


#End Region

    End Class
End Namespace

