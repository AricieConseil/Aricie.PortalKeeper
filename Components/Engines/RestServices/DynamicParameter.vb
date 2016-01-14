
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls
Imports Aricie.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.Entities
Imports Aricie.DNN.Services.Flee

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Cog, IconOptions.Normal)> _
     <DefaultProperty("FriendlyName")> _
    Public Class DynamicParameter
        Inherits NamedIdentifierEntity


        Public Property DynamicAttributes As New ParameterAttributes()

        Public Property EditableType As New DotNetType()

        <Browsable(False)> _
        Public ReadOnly Property HasType As Boolean
            Get
                Return EditableType.GetDotNetType() IsNot Nothing
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property ParameterType As String
            Get
                Return EditableType.TypeName
            End Get
        End Property

        Public Property IsOptional As Boolean

        <ConditionalVisible("IsOptional")>
        Public property DefaultValue As new EnabledFeature(Of AnonymousGeneralVariableInfo)
        
        Public Overrides Function GetFriendlyDetails() As String
            Return String.Format("{1} {0} {2}", UIConstants.TITLE_SEPERATOR, MyBase.GetFriendlyDetails(), Me.ParameterType)
        End Function


    End Class


End Namespace