Namespace Aricie.DNN.Modules.PortalKeeper

    <Serializable()> _
    Public Structure ClientSourceCapLog




        Public Sub New(ByVal lastRequestTime As DateTime, ByVal nbRequests As Integer, ByVal capWindow As DateTime, totalBytes As Integer)

            Me.LastRequestTime = lastRequestTime
            Me.NbRequests = nbRequests
            Me.CapWindow = capWindow
            'SyncLock capWindowLock
            '    If lastRequestTime > capWindow.Add(rateSpan) Then
            '        capWindow = lastRequestTime
            '    End If

            'End SyncLock
        End Sub

        Public CapWindow As DateTime
        Public LastRequestTime As DateTime
        Public NbRequests As Integer
        Public TotalBytes As Integer

    End Structure
End Namespace