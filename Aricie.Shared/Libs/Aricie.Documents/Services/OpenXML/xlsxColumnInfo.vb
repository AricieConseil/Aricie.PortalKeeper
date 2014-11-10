Imports DocumentFormat.OpenXml.Spreadsheet

Namespace Services.OpenXML

    Public Class xlsxColumnInfo

        Public Property Name() As String
        Public Property FriendlyName() As String
        Public Property Format() As System.Type
        Public Property Width() As Double = 0
        'Public Property Autosize As Boolean = False

    End Class

End Namespace
