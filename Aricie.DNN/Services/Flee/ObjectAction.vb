Imports Aricie.DNN.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls

Namespace Services.Flee
    ''' <summary>
    ''' Base class for flee actions
    ''' </summary>
    ''' <remarks></remarks>
    <ActionButton(IconName.Wrench, IconOptions.Normal)> _
    <Serializable()> _
    Public MustInherit Class ObjectAction
        Implements IEnabled

        Public Property Enabled As Boolean = True Implements IEnabled.Enabled

        <ExtendedCategory("Instance")> _
        Public Property LockTarget As Boolean = True

        ''' <summary>
        ''' Returns the name of the Object object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ObjectType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(Object))
            End Get
        End Property

        Public MustOverride Function Run(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object

        Public MustOverride Function GetOutputType() As Type




    End Class


    ''' <summary>
    ''' Type of flee actions
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ObjectActionType
        CallProcedure
        SetProperty
    End Enum


    ''' <summary>
    ''' Generic version of the ObjectAction
    ''' </summary>
    ''' <typeparam name="TObjectType"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public MustInherit Class ObjectAction(Of TObjectType)
        Inherits ObjectAction

        ''' <summary>
        ''' Returns the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("")> _
        Public Overrides ReadOnly Property ObjectType() As String
            Get
                Return ReflectionHelper.GetSafeTypeName(GetType(TObjectType))
            End Get
        End Property

        ''' <summary>
        ''' Instance of the generic type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Instance")> _
        Public Property Instance() As New SimpleExpression(Of TObjectType)

        ''' <summary>
        ''' Parameters for the object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Parameters")> _
        Public Overridable Property Parameters() As New Variables

    End Class




End Namespace