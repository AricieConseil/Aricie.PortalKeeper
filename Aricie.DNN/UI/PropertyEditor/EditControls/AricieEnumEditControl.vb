Imports DotNetNuke.UI.WebControls
Imports System.Web.UI
Imports System.Globalization

Namespace UI.WebControls.EditControls
    Public Class AricieEnumEditControl
        Inherits EnumEditControl

        Protected ReadOnly EnumType As Type

        Public Sub New(objEnumType As Type)
            MyBase.New(objEnumType.AssemblyQualifiedName)
            EnumType = objEnumType
        End Sub
        Protected Overrides Sub RenderViewMode(writer As HtmlTextWriter)
            'Dim propValue As Int32 = Convert.ToInt32(Value)
            Dim propValue As Object = DirectCast(Value, IConvertible).ToType([Enum].GetUnderlyingType(EnumType), CultureInfo.InvariantCulture)
            Dim enumValue As String = [Enum].Format(EnumType, propValue, "G")

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(enumValue)
            writer.RenderEndTag()
        End Sub

    End Class
End NameSpace