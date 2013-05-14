Imports System.Web.UI.WebControls
Imports System.Web.UI

<Assembly: System.Web.UI.WebResource("Aricie.DNN.HtmlJsSlider.js", "text/javascript")> 
'<Assembly: System.Web.UI.WebResource("Aricie.DNN.jquery-ui.js", "text/javascript")> 

Namespace UI.WebControls
    Public Class HtmlJsSlider
        Inherits Panel
        Implements System.Web.UI.IScriptControl, INamingContainer



        'Inherits System.Web.UI.ScriptControl
        Private _minValue As HiddenField
        Private _maxValue As HiddenField
        Private _labelValue As Label
        Private _sliderContainer As Panel

        ' Private _initMin As Integer = 0
        ' Private _initMax As Integer = 300

        Public Sub New()
            MyBase.New()
            'On crée très les contrôles dès le new pour qu'ils participent à toutes les étapes de l'initialisation.
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()


            _minValue = New HiddenField
            _maxValue = New HiddenField
            _labelValue = New Label
            _sliderContainer = New Panel

            _minValue.ID = "minValue"
            _maxValue.ID = "maxValue"
            _labelValue.ID = "lbVal"
            _sliderContainer.ID = "plSlider"
            With Me
                .Controls.Add(_minValue)
                .Controls.Add(_maxValue)
                .Controls.Add(_labelValue)
                .Controls.Add(_sliderContainer)
            End With

            MyBase.CreateChildControls()
        End Sub

        Protected Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors

            Dim toReturn As New List(Of System.Web.UI.ScriptDescriptor)
            Dim descriptor As New ScriptControlDescriptor("Aricie.DNN.HtmlJsSlider", Me.ClientID)
            descriptor.AddProperty("minValueId", Me._minValue.ClientID)
            descriptor.AddProperty("maxValueId", Me._maxValue.ClientID)
            descriptor.AddProperty("lbValId", Me._labelValue.ClientID)
            descriptor.AddProperty("plSliderId", Me._sliderContainer.ClientID)
            descriptor.AddProperty("useRange", Me.UseRange)
            descriptor.AddProperty("initMin", Me.InitMin)
            descriptor.AddProperty("initMax", Me.InitMax)
            descriptor.AddProperty("min", Me.Min)
            descriptor.AddProperty("max", Me.Max)
            '   descriptor.AddProperty("urlJqueryUI", Page.ClientScript.GetWebResourceUrl(Me.GetType, "Aricie.DNN.jquery-ui.js"))
            toReturn.Add(descriptor)
            Return toReturn
        End Function

        Protected Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim toReturn As New List(Of System.Web.UI.ScriptReference)
            toReturn.Add(New ScriptReference("https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.13/jquery-ui.js"))
            toReturn.Add(New ScriptReference("Aricie.DNN.HtmlJsSlider.js", Me.GetType().Assembly.FullName))
            Return toReturn
        End Function


        Private Sub HtmlJsSlider_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim defaultCSS As String = "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.13/themes/base/jquery-ui.css"
            ResourcesUtils.registerStylesheet(Page, "jQueryUIBase", defaultCSS, False)
        End Sub

        Public Property Min() As Double
            Get
                Dim toReturn As Double = 0
                If Not String.IsNullOrEmpty(_minValue.Value) Then
                    toReturn = CDbl(_minValue.Value)
                End If
                Return toReturn
            End Get
            Set(ByVal value As Double)
                _minValue.Value = value.ToString
            End Set
        End Property

        Public Property Max() As Double
            Get
                Dim toReturn As Double = 0
                If Not String.IsNullOrEmpty(_maxValue.Value) Then
                    toReturn = CDbl(_maxValue.Value)
                End If
                Return toReturn
            End Get
            Set(ByVal value As Double)
                _maxValue.Value = value.ToString
            End Set
        End Property

        Public Property InitMin() As Double
            Get
                Dim toReturn As Double = 0
                If Not ViewState("InitMin") Is Nothing Then
                    toReturn = CDbl(ViewState("InitMin"))
                End If
                Return toReturn '_initMin
            End Get
            Set(ByVal value As Double)
                '_initMin = value
                ViewState("InitMin") = value
            End Set
        End Property

        Public Property InitMax() As Double
            Get
                Dim toReturn As Double = 300
                If Not ViewState("InitMax") Is Nothing Then
                    toReturn = CDbl(ViewState("InitMax"))
                End If
                Return toReturn 'Return _initMax
            End Get
            Set(ByVal value As Double)
                '_initMax = value
                ViewState("InitMax") = value
            End Set
        End Property



        Public Property UseRange() As Boolean
            Get
                Dim toReturn As Boolean = True
                If Not ViewState("UseRange") Is Nothing Then
                    toReturn = CBool(ViewState("UseRange"))
                End If
                Return toReturn
            End Get
            Set(ByVal value As Boolean)
                ViewState("UseRange") = value
            End Set
        End Property

        Public Property ShowLabel() As Boolean
            Get
                Return Me._labelValue.Visible
            End Get
            Set(ByVal value As Boolean)
                Me._labelValue.Visible = value
            End Set
        End Property

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptDescriptors(Me)
            End If
            MyBase.Render(writer)
        End Sub


        Private Sub HtmlInputDate_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptControl(Me)
            End If
        End Sub
        
    End Class
End Namespace
