Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Web
Imports System.Web.SessionState

Namespace UI
    ''' <summary>
    ''' Http handler that serves images
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ImageHandler
        Implements IHttpHandler
        Implements IRequiresSessionState

        ''' <summary>
        ''' Indicates whether the handler is reusable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Request handling
        ''' </summary>
        ''' <param name="context"></param>
        ''' <remarks>The client can pass parameters in the url: width and height</remarks>
        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            Try
                Dim width As Integer = 500
                Dim heigth As Integer = 500
                Dim strtemp As String = context.Request.QueryString("width")
                If Not String.IsNullOrEmpty(strtemp) Then
                    width = Integer.Parse(strtemp)
                End If
                strtemp = context.Request.QueryString("heigth")
                If Not String.IsNullOrEmpty(strtemp) Then
                    heigth = Integer.Parse(strtemp)
                End If
                Dim bitmap As Bitmap = Me.GetImage(context, width, heigth)
                context.Response.ContentType = "image/jpeg"

                Dim imageEncoders As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders
                Dim encoderParams As New EncoderParameters
                encoderParams.Param(0) = New EncoderParameter(Encoder.Quality, 90)
                bitmap.Save(context.Response.OutputStream, imageEncoders(1), encoderParams)

                'bitmap.Save(context.Response.OutputStream, ImageFormat.Jpeg)

                bitmap.Dispose()
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.LogException(ex)
            End Try

        End Sub


        Public MustOverride Function GetImage(ByVal context As HttpContext, ByVal width As Integer, ByVal height As Integer) As Bitmap



    End Class
End Namespace