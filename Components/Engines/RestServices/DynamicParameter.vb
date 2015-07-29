
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Cog, IconOptions.Normal)> _
    Public Class DynamicParameter
        Inherits NamedIdentifierEntity

        Public Property DynamicAttributes As New ParameterAttributes()

        Public Property DotNetType As New DotNetType()

        <Browsable(False)> _
        Public ReadOnly Property HasType As Boolean
            Get
                Return DotNetType.GetDotNetType() IsNot Nothing
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property ParameterType As String
            Get
                Return DotNetType.TypeName
            End Get
        End Property

        Public Property IsOptional As Boolean



    End Class


End Namespace