Imports System.Web.UI

<Assembly: WebResource("Aricie.DNN.AutoCompleteTextBox.js", "text/javascript")> 

Namespace UI.WebControls
    <ParseChildren(True), PersistChildren(True)> _
     <ValidationPropertyAttribute("Text")> _
    Public Class AutoCompleteTextBox
        Inherits System.Web.UI.WebControls.Panel
        Implements INamingContainer, IScriptControl


        Private _urlWS As String
        ''' <summary>
        ''' URL Du webService à interroger. Ce dernier doit renvoyer un objet json sous la forme key/value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UrlWS() As String
            Get
                Return _urlWS
            End Get
            Set(ByVal value As String)
                _urlWS = value
            End Set
        End Property
        Private _additionalSelectFunction As String
        Public Property AdditionalSelectFunction() As String
            Get
                Return _additionalSelectFunction
            End Get
            Set(ByVal value As String)
                _additionalSelectFunction = value
            End Set
        End Property
        Private _additionalFunctionForWSResponse As String

        Public Property AdditionalFunctionForWSResponse() As String
            Get
                Return _AdditionalFunctionForWSResponse
            End Get
            Set(ByVal value As String)
                _additionalFunctionForWSResponse = value
            End Set
        End Property
        Private _additionalOnClickFunction As String
        Public Property AdditionalOnClickFunction() As String
            Get
                Return _additionalOnClickFunction
            End Get
            Set(ByVal value As String)
                _additionalOnClickFunction = value
            End Set
        End Property

        Public Property Value As String

            Get
                Return DirectCast(Me.FindControl("Hf"), System.Web.UI.WebControls.HiddenField).Value
            End Get
            Set(value As String)
                DirectCast(Me.FindControl("Hf"), System.Web.UI.WebControls.HiddenField).Value = value
            End Set
        End Property


        Public Property Text() As String
            Get
                Return DirectCast(Me.FindControl("Tb"), System.Web.UI.WebControls.TextBox).Text
            End Get
            Set(ByVal value As String)
                DirectCast(Me.FindControl("Tb"), System.Web.UI.WebControls.TextBox).Text = value
            End Set
        End Property


        Private _UrljQueryUIJS As String = "https://code.jquery.com/ui/1.9.0/jquery-ui.js"
        Public Property UrljQueryUIJS() As String
            Get
                Return _UrljQueryUIJS
            End Get
            Set(ByVal value As String)
                _UrljQueryUIJS = value
            End Set
        End Property

        Private _UrljQueryUICSS As String = "https://code.jquery.com/ui/1.9.0/themes/base/jquery-ui.css"
        Public Property UrljQueryUICSS() As String
            Get
                Return _UrljQueryUICSS
            End Get
            Set(ByVal value As String)
                _UrljQueryUICSS = value
            End Set
        End Property
        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            Dim myTb As New System.Web.UI.WebControls.TextBox
            Dim myHf As New System.Web.UI.WebControls.HiddenField
            myTb.ID = "Tb"
            myHf.ID = "Hf"
            myTb.Text = EmptyText
            Me.Controls.Add(myTb)
            Me.Controls.Add(myHf)
        End Sub
        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim toReturn As New List(Of ScriptDescriptor)
            Dim myScriptD As New ScriptControlDescriptor("Aricie.DNN.AutoCompleteTextBox", Me.ClientID)
            myScriptD.AddProperty("ClientId", Me.ClientID)
            myScriptD.AddProperty("TbClientId", Me.FindControl("Tb").ClientID)
            myScriptD.AddProperty("HfClientId", Me.FindControl("Hf").ClientID)
            myScriptD.AddProperty("UrlWS", String.Format("http://{0}/{1}", System.Web.HttpContext.Current.Request.Url.Host, ResolveClientUrl(Me.UrlWS)))
            myScriptD.AddProperty("AdditionalFunctionForWSResponse", Me.AdditionalFunctionForWSResponse)
            myScriptD.AddProperty("AdditionalSelectFunction", Me.AdditionalSelectFunction)
            myScriptD.AddProperty("AdditionalOnClickFunction", Me.AdditionalOnClickFunction)
            myScriptD.AddProperty("EmptyText", Me.EmptyText)
            myScriptD.AddProperty("AdditionalParam", Me.AdditionalParam)
            toReturn.Add(myScriptD)
            Return toReturn
        End Function

        Private _EmptyText As String
        Public Property EmptyText() As String
            Get
                Return _EmptyText
            End Get
            Set(ByVal value As String)
                _EmptyText = value
            End Set
        End Property
        Private _AdditionalParam As Integer
        Public Property AdditionalParam() As Integer
            Get
                Return _AdditionalParam
            End Get
            Set(ByVal value As Integer)
                _AdditionalParam = value
            End Set
        End Property


        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim toReturn As New List(Of ScriptReference)
            toReturn.Add(New ScriptReference("Aricie.DNN.AutoCompleteTextBox.js", GetType(AutoCompleteTextBox).Assembly.FullName))

            '  toReturn.Add(New ScriptReference(UrljQueryUIJS))

            Return toReturn
        End Function

        Private Sub AutoCompleteTextBox_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptControl(Me)

            End If
            ResourcesUtils.registerStylesheet(Page, "jQueryUI", UrljQueryUICSS, False)

        End Sub
        Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
            If Not Me.DesignMode Then
                ScriptManager.GetCurrent(Me.Page).RegisterScriptDescriptors(Me)
            End If
            MyBase.Render(writer)

        End Sub
    End Class
End Namespace