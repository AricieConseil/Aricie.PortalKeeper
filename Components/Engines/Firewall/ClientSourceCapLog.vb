Namespace Aricie.DNN.Modules.PortalKeeper

    
    Public Structure ClientSourceCapLog




        Public Sub New(ByVal lastRequestTime As DateTime, ByVal nbRequests As Integer, ByVal capWindow As DateTime, longTotalBytes As Long)

            Me.FirstRequestTime = lastRequestTime
            Me.NbRequests = nbRequests
            Me.CapWindow = capWindow
            Me.TotalBytes = longTotalBytes
            'SyncLock capWindowLock
            '    If lastRequestTime > capWindow.Add(rateSpan) Then
            '        capWindow = lastRequestTime
            '    End If

            'End SyncLock
        End Sub

        Public CapWindow As DateTime
        Public FirstRequestTime As DateTime
        Public NbRequests As Integer
        Public TotalBytes As Long

    End Structure
End Namespace