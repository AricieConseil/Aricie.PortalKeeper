Imports System.Web.UI


'===============================================================================
'HISTORY:
'-------------------------------------------------------------------------------
' 13/04/2011 - [JBB] - Création
' 19/04/2011 - [JBB] - Gestion du format de la date
'-------------------------------------------------------------------------------
' Infos Surround : 
' $Header$
' $Log$



<Assembly: system.Web.UI.WebResource("Aricie.DNN.Aricie.Web.css", "text/css", PerformSubstitution:=True)> 
<Assembly: system.Web.UI.WebResource("Aricie.DNN.HtmlInputDate.js", "text/javascript")> 
<Assembly: System.Web.UI.WebResource("Aricie.DNN.jquery.tools.min.js", "text/javascript")> 

Namespace UI.WebControls


    Public Class HtmlInputDate
        Inherits System.Web.UI.WebControls.TextBox

        'Inherits HtmlControls.HtmlInputText
        Implements IScriptControl


        Private _min As Integer?
        Private _max As Integer?
        Private _slaveDateCtId As String
        Private _format As String = "dd/mm/yyyy"
        Private _selectors As Boolean
        Private _yearRange As String = "[-5,5]"

        Public Sub New()
            MyBase.New()

        End Sub

        'Public Property CssClass() As String
        '    Get
        '        Return Me.GetAttribute("class")
        '    End Get
        '    Set(ByVal value As String)
        '        Me.SetAttribute("class", value)
        '    End Set
        'End Property

        ''' <summary>
        ''' Generate the trigger (a link) to open the calendar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.ComponentModel.DefaultValue(False)> _
        Public Property Trigger() As Boolean
            Get
                Dim toReturn As Boolean = False
                Dim strTrigger As String = Me.Attributes("trigger")
                If Not String.IsNullOrEmpty(strTrigger) Then
                    toReturn = CBool(strTrigger)
                End If
                Return toReturn
            End Get
            Set(ByVal value As Boolean)
                Me.Attributes.Remove("trigger")
                Me.Attributes.Add("trigger", value.ToString)
            End Set
        End Property

        ''' <summary>
        ''' NbDays from today (+ or - x days) for the min value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Min() As Integer?
            Get
                Return _min
            End Get
            Set(ByVal value As Integer?)
                _min = value
            End Set
        End Property

        ''' <summary>
        ''' NbDays from today (+ or - x days) for the max value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Max() As Integer?
            Get
                Return _max
            End Get
            Set(ByVal value As Integer?)
                _max = value
            End Set
        End Property

        Public Property DateValue() As Date
            Get
                Dim toReturn As Date = Nothing
                'If Not Date.TryParseExact(Me.GetAttribute("dateValue"), "dd-mm-yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, toReturn) Then
                If Not Date.TryParseExact(Me.Text, "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, toReturn) Then
                    toReturn = Nothing
                End If
                'If toReturn = Nothing Then
                '    toReturn = CDate(Me.ViewState("DateValue"))
                'Else
                '    Me.ViewState("DateValue") = Value
                'End If
                Return toReturn
            End Get
            Set(ByVal value As Date)
                Me.Text = value.ToString("dd/MM/yyyy")
                'Me.Attributes.Remove("data-value")
                'Me.Attributes.Add("data-value", Me.Value)
                ' Me.ViewState("DateValue") = value
            End Set
        End Property

        Public Property SlaveDateCtId() As String
            Get
                Return _slaveDateCtId
            End Get
            Set(ByVal value As String)
                _slaveDateCtId = value
            End Set
        End Property



        Public Property Format() As String
            Get
                Return _format
            End Get
            Set(ByVal value As String)
                _format = value
            End Set
        End Property


        Public Property Selectors() As Boolean
            Get
                Return _selectors
            End Get
            Set(ByVal value As Boolean)
                _selectors = value
            End Set
        End Property

        Public Property YearRange() As String
            Get
                Return _yearRange
            End Get
            Set(ByVal value As String)
                _yearRange = value
            End Set
        End Property



        Private _language As String = "fr"
        ''' <summary>
        ''' define the language of the calendar (fr by default, en is available)
        ''' </summary>
        ''' <value>en/fr</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Language() As String
            Get
                Return _language
            End Get
            Set(ByVal value As String)
                _language = value
            End Set
        End Property

        Private _urlCDNJqueryTools As String = "http://cdn.jquerytools.org/1.2.5/form/jquery.tools.min.js"
        Public Property UrlCDNJqueryTools As String
            Get
                Return _urlCDNJqueryTools
            End Get
            Set(ByVal value As String)
                _urlCDNJqueryTools = value
            End Set
        End Property




        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim res As New List(Of ScriptDescriptor)
            Dim descriptor As New ScriptControlDescriptor("Aricie.DNN.HtmlInputDate", Me.ClientID)
            ' Dim scriptRef As New ScriptReference("DT.Web.jquery.tools.min.js", Me.GetType().Assembly.FullName)

            Dim currentDate As Date = DateValue
            descriptor.AddProperty("clientId", Me.ClientID)
            descriptor.AddProperty("trigger", Me.Trigger)
            descriptor.AddProperty("min", Me.Min)
            descriptor.AddProperty("max", Me.Max)
            descriptor.AddProperty("selectors", Me.Selectors)
            descriptor.AddProperty("yearRange", Me.YearRange)
            descriptor.AddProperty("language", Me.Language)
            descriptor.AddProperty("format", Me.Format)
            descriptor.AddProperty("currentDate", Me.DateValue) '.ToString("dd/MM/yyyy"))
            descriptor.AddProperty("urljQueryTools", Page.ClientScript.GetWebResourceUrl(GetType(HtmlInputDate), "Aricie.DNN.jquery.tools.min.js")) 'scriptRef.Path)
            descriptor.AddProperty("urlCDNjQueryTools", Me.UrlCDNJqueryTools)
            Me.Text = ""
            'descriptor.AddProperty("format", "dd/mm/yyyy")
            'If Not currentDate = Nothing Then
            '    descriptor.AddProperty("valueYear", currentDate.Year)
            '    descriptor.AddProperty("valueMonth", currentDate.Month)
            '    descriptor.AddProperty("valueDay", currentDate.Day)
            'End If
            If Not String.IsNullOrEmpty(Me.SlaveDateCtId) Then
                Dim ct As Control = Me.NamingContainer.FindControl(Me.SlaveDateCtId)
                If Not ct Is Nothing Then
                    descriptor.AddProperty("linkedDateId", ct.ClientID)
                End If

            End If
            res.Add(descriptor)
            Return res
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim toReturn As New List(Of ScriptReference)
            Dim scriptRef As New ScriptReference("Aricie.DNN.HtmlInputDate.js", GetType(HtmlInputDate).Assembly.FullName)
            toReturn.Add(scriptRef)
            Return toReturn
        End Function

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptDescriptors(Me)
            End If
            MyBase.Render(writer)
        End Sub


        Private Sub HtmlInputDate_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim url As String = ResourcesUtils.getWebResourceUrl(Page, GetType(HtmlInputDate), "Aricie.DNN.Aricie.Web.css")
            If Not String.IsNullOrEmpty(url) Then
                ResourcesUtils.registerStylesheet(Page, "Aricie.Web", url, False)
            End If

            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptControl(Me)
            End If
        End Sub

      
    End Class
End Namespace