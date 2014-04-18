Imports System.Xml.Serialization
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.Globalization
Imports Aricie.DNN.Services
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Services.Localization

Namespace Aricie.DNN.Modules.PortalKeeper

    <XmlRoot("profiledefinition", IsNullable:=False)> _
        <Serializable()> _
    Public Class GeneralPropertyDefinition

#Region "Constructors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal portalId As Integer)
            Me.PortalId = portalId
        End Sub

#End Region

#Region "Public Properties"

        Public Function ToDNNProfileDefinition() As ProfilePropertyDefinition
            Dim toReturn As New ProfilePropertyDefinition
            toReturn.DataType = Me.DataType
            toReturn.DefaultValue = Me.DefaultValue
            toReturn.Length = Me.Length
            toReturn.ModuleDefId = Me.ModuleDefId
            toReturn.PortalId = Me.PortalId
            toReturn.PropertyCategory = Me.PropertyCategory
            toReturn.PropertyDefinitionId = Me.PropertyDefinitionId
            toReturn.PropertyName = Me.PropertyName
            toReturn.PropertyValue = Me.PropertyValue
            toReturn.Required = Me.Required
            toReturn.ValidationExpression = Me.ValidationExpression
            toReturn.Visibility = Me.Visibility
            toReturn.Visible = Me.Visible

            Return toReturn
        End Function

        Public Shared Function FromDNNProfileDefinition(source As ProfilePropertyDefinition) As GeneralPropertyDefinition
            Dim toReturn As New GeneralPropertyDefinition
            toReturn.DataType = source.DataType
            toReturn.DefaultValue = source.DefaultValue
            toReturn.Length = source.Length
            toReturn.ModuleDefId = source.ModuleDefId
            toReturn.PortalId = source.PortalId
            toReturn.PropertyCategory = source.PropertyCategory
            toReturn.PropertyDefinitionId = source.PropertyDefinitionId
            toReturn.PropertyName = source.PropertyName
            toReturn.PropertyValue = source.PropertyValue
            toReturn.Required = source.Required
            toReturn.ValidationExpression = source.ValidationExpression
            toReturn.Visibility = source.Visibility
            toReturn.Visible = source.Visible
            Return toReturn
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Data Type of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Editor("DotNetNuke.UI.WebControls.DNNListEditControl, DotNetNuke", GetType(EditControl)), _
            List("DataType", "", ListBoundField.Id, ListBoundField.Value), _
            IsReadOnly(True), Required(True), SortOrder(1)> _
        Public Property DataType() As Integer = Null.NullInteger

        <SortOrder(4)> _
        Public Property DefaultValue() As String = String.Empty

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Length of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(3)> <XmlElement("length")> _
        Public Property Length() As Integer = 0


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleDefId
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property ModuleDefId() As Integer = Null.NullInteger

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PortalId
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property PortalId() As Integer = 0

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Category of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Required(True), SortOrder(2)> <XmlElement("propertycategory")> _
        Public Property PropertyCategory() As String = String.Empty

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Id of the GeneralPropertyDefinition
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property PropertyDefinitionId() As Integer = Null.NullInteger

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Name of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Required(True), SortOrder(0), RegularExpressionValidator("^[a-zA-Z0-9._%\-+']+$")> <XmlElement("propertyname")> _
        Public Property PropertyName() As String = String.Empty

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Value of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property PropertyValue() As String


        <Browsable(False)> _
        Public ReadOnly Property TypedValue As Object
            Get
                Dim listEntry As ListEntryInfo = New ListController().GetListEntryInfo(Me._DataType)
                If listEntry IsNot Nothing AndAlso listEntry.ListName = "DataType" Then
                    Select Case listEntry.Value
                        Case "Integer"
                            Return Convert.ToInt32(Me._PropertyValue)
                        Case "TrueFalse"
                            Return Convert.ToBoolean(Me._PropertyValue)
                        Case "Date", "DateTime"
                            Return DateTime.Parse(Me._PropertyValue, CultureInfo.InvariantCulture, DateTimeStyles.None)
                        Case "Locale"
                            Return Localization.GetSupportedLocales.Item(Me._PropertyValue)
                        Case "Page"
                            Return TabController.GetTab(Convert.ToInt32(Me._PropertyValue), NukeHelper.PortalId, False)
                        Case Else
                            Return Me._PropertyValue
                    End Select
                End If
                Return Me._PropertyValue
            End Get
        End Property


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is required
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(6)> _
        Public Property Required() As Boolean = False

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a Validation Expression (RegEx) for the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(5)> _
        Public Property ValidationExpression() As String = String.Empty


        ' ''' -----------------------------------------------------------------------------
        ' ''' <summary>
        ' ''' Gets and sets the View Order of the Property
        ' ''' </summary>
        ' ''' <history>
        ' '''     [cnurse]	01/31/2006	created
        ' ''' </history>
        ' ''' -----------------------------------------------------------------------------
        '<IsReadOnly(True), SortOrder(8)> _
        'Public Property ViewOrder() As Integer = 0

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is visible
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(7)> _
        Public Property Visible() As Boolean

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Default Visibility of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [sbwalker]	06/28/2010	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(9)> _
        Public Property DefaultVisibility() As UserVisibilityMode = UserVisibilityMode.AdminOnly


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is visible
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property Visibility() As UserVisibilityMode = UserVisibilityMode.AdminOnly


#End Region

#Region "Public Methods"

        Public Function Clone() As GeneralPropertyDefinition
            Dim objClone As New GeneralPropertyDefinition(Me.PortalId)
            objClone.DataType = Me.DataType
            objClone.DefaultValue = Me.DefaultValue
            objClone.Length = Me.Length
            objClone.ModuleDefId = Me.ModuleDefId
            objClone.PropertyCategory = Me.PropertyCategory
            objClone.PropertyDefinitionId = Me.PropertyDefinitionId
            objClone.PropertyName = Me.PropertyName
            objClone.PropertyValue = Me.PropertyValue
            objClone.Required = Me.Required
            objClone.ValidationExpression = Me.ValidationExpression
            'objClone.ViewOrder = Me.ViewOrder
            objClone.Visibility = Me.Visibility
            objClone.DefaultVisibility = Me.DefaultVisibility
            objClone.Visible = Me.Visible

            Return objClone
        End Function

#End Region

    End Class
End Namespace