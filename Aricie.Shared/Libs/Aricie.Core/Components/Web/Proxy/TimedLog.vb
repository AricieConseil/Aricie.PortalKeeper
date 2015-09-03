Namespace Web.Proxy
    Public Class TimedLog
        Implements IDisposable
        Private _Message As String
        Private _Start As DateTime

        Public Sub New(ByVal msg As String)
            Me._Message = msg
            Me._Start = DateTime.Now
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dim [end] As DateTime = DateTime.Now
            Dim duration As TimeSpan = [end] - Me._Start
            '
            '            Log.WriteLine(this._Start.Minute + ":" + this._Start.Second + ":" + this._Start.Millisecond
            '                + "/" + end.Minute + ":" + end.Second + ":" + this._Start.Millisecond
            '                    + "\t" + duration.TotalMilliseconds
            '                    + "\t" + _Message + "\n");
            '            

            Log.WriteLine((duration.FormatTimeSpan() & vbTab) + _Message & vbLf)
        End Sub
    End Class



End NameSpace