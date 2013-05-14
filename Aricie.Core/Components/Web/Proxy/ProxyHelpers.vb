Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Net
Imports System.Threading
Imports System.IO


Namespace Web.Proxy

    Public Module HttpHelper



        Public Function CreateScalableHttpWebRequest(ByVal url As String) As HttpWebRequest
            Dim request As HttpWebRequest = TryCast(WebRequest.Create(url), HttpWebRequest)
            request.Headers.Add("Accept-Encoding", "gzip")
            request.AutomaticDecompression = DecompressionMethods.GZip
            request.MaximumAutomaticRedirections = 2
            request.ReadWriteTimeout = 5000
            request.Timeout = 3000
            request.Accept = "*/*"
            request.Headers.Add("Accept-Language", "en-US")
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.6) Gecko/20070725 Firefox/2.0.0.6"

            Return request
        End Function

        Public Sub CacheResponse(ByVal context As HttpContext, ByVal durationInMinutes As Integer)
            Dim duration As TimeSpan = TimeSpan.FromMinutes(durationInMinutes)

            context.Response.Cache.SetCacheability(HttpCacheability.[Public])
            context.Response.Cache.SetExpires(DateTime.Now.Add(duration))
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate")
            context.Response.Cache.SetMaxAge(duration)
        End Sub
        Public Sub DoNotCacheResponse(ByVal context As HttpContext)
            context.Response.Cache.SetNoServerCaching()
            context.Response.Cache.SetNoStore()
            context.Response.Cache.SetMaxAge(TimeSpan.Zero)
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate")
            context.Response.Cache.SetExpires(DateTime.Now.AddYears(-1))
        End Sub
    End Module

    Public Class CachedContent
        Public ContentType As String
        Public ContentEncoding As String
        Public ContentLength As String
        Public Content As MemoryStream
    End Class

    Public Class AsyncState
        Public Context As HttpContext
        Public Url As String
        Public CacheDuration As Integer
        Public Request As HttpWebRequest
    End Class

    Public Class SyncResult
        Implements IAsyncResult
        Public Content As CachedContent
        Public Context As HttpContext


        Private ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
            Get
                Return New Object()
            End Get
        End Property

        Private ReadOnly Property AsyncWaitHandle() As WaitHandle Implements IAsyncResult.AsyncWaitHandle
            Get
                Return New ManualResetEvent(True)
            End Get
        End Property

        Private ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
            Get
                Return True
            End Get
        End Property

        Private ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
            Get
                Return True
            End Get
        End Property
    End Class


    Public Module Log

        Private logStream As StreamWriter
        Private lockObject As New Object()
        Public Sub WriteLine(ByVal msg As String)
            If logStream Is Nothing Then
                SyncLock lockObject
                    If logStream Is Nothing Then
                        logStream = File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"))
                    End If
                End SyncLock
            End If
            logStream.WriteLine(msg)
            'logStream.Flush();

            '
            '            lock (lockObject)
            '            {
            '                File.AppendAllText(
            '                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"),
            '                    msg
            '                );
            '            }
            '            

        End Sub
    End Module

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

            Log.WriteLine((duration.TotalMilliseconds & vbTab) + _Message & vbLf)
        End Sub
    End Class

End Namespace
