Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace Services.Flee

    ''' <summary>
    ''' Generics-compatible List of flee actions holder class
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    
    Public Class ObjectActions(Of TObjectType)
        Inherits ObjectActionsBase

        ''' <summary>
        ''' Returns the generic type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetInitialTypes() As System.Collections.Generic.IList(Of ComponentModel.DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(GetType(TObjectType)))
            Return toReturn
        End Function


    End Class

    ''' <summary>
    ''' List of flee actions holder class
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Class ObjectActions
        Inherits ObjectActionsBase

        ''' <summary>
        ''' Gets or sets whther to display the available types
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Category("")> _
        Public Property ShowAvailableTypes() As Boolean
        

        ''' <summary>
        ''' Gets or sets List of types from the base class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MainCategory()> _
        <Category("")> _
            <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 5, CollectionDisplayStyle.List)> _
            <LabelMode(LabelMode.Top)> _
            <ConditionalVisible("ShowAvailableTypes", False, True)> _
        Public Property ExpressionTypes() As List(Of DotNetType)
            Get
                Return _ExpressionTypes
            End Get
            Set(ByVal value As List(Of DotNetType))
                _ExpressionTypes = value
            End Set
        End Property

        ''' <summary>
        ''' Returns a list of types containing string type and boolean type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetInitialTypes() As IList(Of DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(GetType(String)))
            toReturn.Add(New DotNetType(GetType(Boolean)))
            Return toReturn
        End Function

    End Class
End Namespace