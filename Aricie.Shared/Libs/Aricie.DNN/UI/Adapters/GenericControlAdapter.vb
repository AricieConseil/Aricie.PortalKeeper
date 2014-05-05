Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.Adapters
Imports System.Web.UI
Imports System.Drawing
Imports System.Text
Imports System.IO
Imports System.Web.UI.Adapters

Namespace UI.Adapters


    ''' <summary>
    ''' Provides custom HTML rendering for a <see cref="WebControl"/>.
    ''' </summary>
    ''' <typeparam name="T">A <see cref="WebControl"/> type.</typeparam>
    ''' <remarks>
    ''' Unlike typical controls related to rendering web markup, the HtmlRenderer does not
    ''' output directly to the current request, even though an <see cref="HtmlTextWriter"/>
    ''' is provided to many methods. Instead, this class is responsible for returning the HTML output
    ''' as a string. 
    ''' 
    ''' This is done to improve testability. Output from HtmlRenderer objects
    ''' can be tested programmatically, and the custom control adapters rely on these renderers
    ''' for basic markup.
    ''' </remarks>
    Public MustInherit Class HtmlRenderer(Of T As WebControl)

        Private _control As T

        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the class.
        ''' </summary>
        ''' <param name="control">The control this renderer generates HTML for.</param>
        Public Sub New(control As T)
            _control = control
        End Sub

        ''' <summary>
        ''' Gets or sets the control this renderer generates HTML for.
        ''' </summary>
        Public Property Control() As T
            Get
                Return _control
            End Get
            Set(value As T)
                _control = value
            End Set
        End Property

        ''' <summary>
        ''' Renders the beginning HTML code for a control. Intended to be called by a control adapter's
        ''' RenderBeginTag method.
        ''' </summary>
        ''' <returns>The generated HTML.</returns>
        Public MustOverride Function RenderBeginTag() As String

        ''' <summary>
        ''' Renders the inner HTML code for a control. Intended to be called by a control adapter's
        ''' RenderContents method.
        ''' </summary>
        ''' <returns>The generated HTML.</returns>
        Public MustOverride Function RenderContents() As String

        ''' <summary>
        ''' Renders the ending HTML code for a control. Intended to be called by a control adapter's
        ''' RenderEndTag method.
        ''' </summary>
        ''' <returns>The generated HTML.</returns>
        Public MustOverride Function RenderEndTag() As String

        ''' <summary>
        ''' Adds the default properties and attributes for a given control to a given attribute collection.
        ''' </summary>
        ''' <param name="control">The web control.</param>
        ''' <param name="attributes">The collection of attributes.</param>
        ''' <returns>The updated collection.</returns>
        Public Shared Function AddDefautAttributesToCollection(control As WebControl, attributes As AttributeCollection) As AttributeCollection
            If attributes Is Nothing Then
                Throw New ArgumentNullException("attributes")
            End If

            Dim styles As CssStyleCollection = attributes.CssStyle

            If control.BackColor <> Color.Empty Then
                styles("background-color") = ColorTranslator.ToHtml(control.BackColor)
            End If
            If control.BorderColor <> Color.Empty Then
                styles("border-color") = ColorTranslator.ToHtml(control.BorderColor)
            End If
            If control.BorderStyle <> BorderStyle.NotSet Then
                styles("border-style") = control.BorderStyle.ToString().ToLowerInvariant()
            End If
            If Not control.BorderWidth.IsEmpty Then
                styles("border-width") = control.BorderWidth.ToString()
            End If
            If control.ForeColor <> Color.Empty Then
                styles("color") = ColorTranslator.ToHtml(control.ForeColor)
            End If
            If Not control.Height.IsEmpty Then
                styles("height") = control.Height.ToString()
            End If
            If Not control.Width.IsEmpty Then
                styles("width") = control.Width.ToString()
            End If

            For Each key As String In control.Attributes.Keys
                If Not [String].Equals(key, "style", StringComparison.InvariantCultureIgnoreCase) Then
                    attributes.Add(key, control.Attributes(key))
                End If
            Next
            Return attributes
        End Function

        ''' <summary>
        ''' Writes a collection of attributes to the output stream.
        ''' </summary>
        ''' <param name="writer">The output stream to write to.</param>
        ''' <param name="attributes">The collection of attributes.</param>
        Public Shared Sub WriteAttributes(writer As HtmlTextWriter, attributes As AttributeCollection)
            If attributes.Count > 0 Then
                For Each key As String In attributes.Keys
                    writer.WriteAttribute(key, attributes(key))
                Next
            End If
        End Sub

        ''' <summary>
        ''' Writes a collection of styles to the output stream.
        ''' </summary>
        ''' <param name="writer">The output stream to write to.</param>
        ''' <param name="styles">The collection of styles.</param>
        Public Shared Sub WriteStyles(writer As HtmlTextWriter, styles As CssStyleCollection)
            If styles.Count > 0 Then
                writer.Write(" style=""")
                For Each key As String In styles.Keys
                    writer.WriteStyleAttribute(key, styles(key), True)
                Next
                writer.Write("""")
            End If
        End Sub

        ''' <summary>
        ''' Concatenates a series of CSS class names into a markup-friendly class list.
        ''' </summary>
        ''' <param name="classes">The CSS classes to concatenate.</param>
        ''' <returns>A markup-friendly class list.</returns>
        ''' <remarks>
        ''' Duplicates are permitted and would be included in the output. Empty strings are skipped.
        ''' </remarks>
        Public Shared Function ConcatenateCssClasses(ParamArray classes As String()) As String
            Dim sb As New StringBuilder()
            For Each cl As String In classes
                If [String].IsNullOrEmpty(cl) Then
                    Continue For
                End If

                If sb.Length > 0 Then
                    sb.Append(" "c)
                End If

                sb.Append(cl)
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Gets the HTML Name attribute value, such as for form fields, from a given ID attribute value.
        ''' </summary>
        ''' <param name="clientID">The ID attribute value.</param>
        ''' <returns>The corresponding Name attribute value.</returns>
        Public Shared Function GetNameFromClientID(clientID As String) As String
            Return clientID.Replace("_"c, "$"c)
        End Function

   


        ''' <summary>
        ''' Returns a new <see cref="HtmlTextWriter"/> for use in rendering output.
        ''' </summary>
        ''' <returns>A new <see cref="HtmlTextWriter"/>.</returns>
        Public Shared Function CreateHtmlTextWriter() As HtmlTextWriter
            Return New HtmlTextWriter(New StringWriter())
        End Function

        ''' <summary>
        ''' Generates HTML from a control template.
        ''' </summary>
        ''' <param name="template">The template.</param>
        ''' <returns>The generated HTML.</returns>
        Public Shared Function GenerateHtmlFromTemplate(template As ITemplate) As String
            Dim writer As HtmlTextWriter = CreateHtmlTextWriter()

            Dim ph As New PlaceHolder()
            template.InstantiateIn(ph)
            ph.RenderControl(writer)

            Return writer.InnerWriter.ToString()
        End Function
    End Class


    ''' <summary>
    ''' Provides an abstract base class for our custom control adapters.
    ''' </summary>
    ''' <typeparam name="T">A type of <see cref="WebControl"/> that this adapter will service.</typeparam>
    Public MustInherit Class ControlAdapterBase(Of T As Control)
        Inherits ControlAdapter

        ''' <summary>
        ''' Returns a strongly-typed instance of the <see cref="WebControlAdapter.Control"/> property.
        ''' </summary>
        Public ReadOnly Property AdaptedControl() As T
            Get
                Return TryCast(Me.Control, T)
            End Get
        End Property


    End Class





    ''' <summary>
    ''' Provides an abstract base class for our custom control adapters.
    ''' </summary>
    ''' <typeparam name="T">A type of <see cref="WebControl"/> that this adapter will service.</typeparam>
    Public MustInherit Class WebControlAdapterBase(Of T As WebControl)
        Inherits WebControlAdapter
        ''' <summary>
        ''' Gets or sets the <see cref="HtmlRenderer"/> used by this adapter.
        ''' </summary>
        Public Property HtmlRenderer() As HtmlRenderer(Of T)
            Get
                Return _HtmlRenderer
            End Get
            Set(value As HtmlRenderer(Of T))
                _HtmlRenderer = value
            End Set
        End Property
        Private _HtmlRenderer As HtmlRenderer(Of T)

        ''' <summary>
        ''' Returns a strongly-typed instance of the <see cref="WebControlAdapter.Control"/> property.
        ''' </summary>
        Public ReadOnly Property AdaptedControl() As T
            Get
                Return TryCast(Me.Control, T)
            End Get
        End Property

        ''' <summary>
        ''' Abstract class that returns the appropriate <see cref="HtmlRenderer"/> to use for this class.
        ''' </summary>
        ''' <returns>An <see cref="HtmlRenderer"/>.</returns>
        Protected Overridable Function CreateHtmlRenderer() As HtmlRenderer(Of T)
            Return Nothing
        End Function

        ''' <summary>
        ''' Initializes the control adapter prior to rendering markup.
        ''' The default implementation creates the <see cref="HtmlRenderer"/> to use by the
        ''' adapter by calling <see cref="CreateHtmlRenderer"/> if the renderer has not
        ''' already been provided.
        ''' </summary>
        ''' <param name="e">The event arguments.</param>
        Protected Overrides Sub OnPreRender(e As EventArgs)
            If HtmlRenderer Is Nothing Then
                HtmlRenderer = CreateHtmlRenderer()
            End If
        End Sub

        ''' <summary>
        ''' Writes the beginning HTML tag to the output stream.
        ''' The default implementation writes the HtmlRenderer's RenderBeginTag
        ''' output to the output stream.
        ''' </summary>
        ''' <param name="writer">The output stream.</param>
        Protected Overrides Sub RenderBeginTag(writer As HtmlTextWriter)
            If HtmlRenderer IsNot Nothing Then
                writer.Write(HtmlRenderer.RenderBeginTag())
            Else
                MyBase.RenderBeginTag(writer)
            End If
        End Sub

        ''' <summary>
        ''' Writes the HTML contents to the output stream.
        ''' The default implementation writes the HtmlRenderer's RenderContents
        ''' output to the output stream.
        ''' </summary>
        ''' <param name="writer">The output stream.</param>
        Protected Overrides Sub RenderContents(writer As HtmlTextWriter)
            If HtmlRenderer IsNot Nothing Then
                writer.Write(HtmlRenderer.RenderContents())
            Else
                MyBase.RenderBeginTag(writer)
            End If
        End Sub

        ''' <summary>
        ''' Writes the beginning HTML tag to the output stream.
        ''' The default implementation writes the HtmlRenderer's RenderEndTag
        ''' output to the output stream.
        ''' </summary>
        ''' <param name="writer">The output stream.</param>
        Protected Overrides Sub RenderEndTag(writer As HtmlTextWriter)
            If HtmlRenderer IsNot Nothing Then
                writer.Write(HtmlRenderer.RenderEndTag())
            Else
                MyBase.RenderBeginTag(writer)
            End If
        End Sub





    End Class

End Namespace

