Imports System.Xml
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes

Namespace Configuration

    ''' <summary>
    ''' Base class for configuration elements with a type attribute
    ''' </summary>
    
    Public MustInherit Class TypedXmlConfigElementInfo
        Inherits XmlNamedConfigElementInfo


        Private _Type As Type

        Public Sub New()

        End Sub



        Public Sub New(ByVal name As String, ByVal objType As Type)
            MyBase.New(name)
            Me.Type = objType
        End Sub

        <Browsable(False)> _
         Public ReadOnly Property HasType As Boolean
            Get
                Return Type IsNot Nothing
            End Get
        End Property

        <ConditionalVisible("HasType", False, True)> _
         Public Overrides ReadOnly Property Installed As Boolean
            Get
                Return MyBase.Installed
            End Get
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overridable Property Type() As Type
            Get
                Return _Type
            End Get
            Set(ByVal value As Type)
                _Type = value
            End Set
        End Property

        <ConditionalVisible("HasType", False, True)> _
        Public Overrides Sub Install(pe As UI.WebControls.AriciePropertyEditorControl)
            MyBase.Install(pe)
        End Sub

        <ConditionalVisible("HasType", False, True)> _
        Public Overrides Sub Update(pe As UI.WebControls.AriciePropertyEditorControl)
            MyBase.Update(pe)
        End Sub

        <ConditionalVisible("HasType", False, True)> _
        Public Overrides Sub Uninstall(pe As UI.WebControls.AriciePropertyEditorControl)
            MyBase.Uninstall(pe)
        End Sub

        Public MustOverride Overrides Function IsInstalled(ByVal xmlConfig As XmlDocument) As Boolean

        Public MustOverride Overrides Sub AddConfigNodes(ByRef targetNodes As NodesInfo, ByVal actionType As ConfigActionType)

    End Class


End Namespace


