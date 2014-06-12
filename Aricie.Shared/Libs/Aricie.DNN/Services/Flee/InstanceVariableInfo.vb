Imports Aricie.DNN.UI.Attributes
Imports Aricie.Services
Imports DotNetNuke.UI.WebControls

Namespace Services.Flee
    ''' <summary>
    ''' Flee instance variable
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class InstanceVariableInfo(Of TResult)
        Inherits VariableInfo(Of TResult)

        ''' <summary>
        ''' Return Off as the default instance mode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetDefaultInstanceMode() As InstanceMode
            Return InstanceMode.Off
        End Function


        ''' <summary>
        ''' gets or sets base InstanceMode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Evaluation")> _
        <SortOrder(101)> _
        Public Overrides Property InstanceMode() As InstanceMode
            Get
                Return MyBase.InstanceMode
            End Get
            Set(ByVal value As InstanceMode)
                MyBase.InstanceMode = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets base _EvaluationMode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Evaluation")> _
        <SortOrder(102)> _
        Public Overrides Property EvaluationMode() As VarEvaluationMode
            Get
                Return MyBase.EvaluationMode
            End Get
            Set(ByVal value As VarEvaluationMode)
                MyBase.EvaluationMode = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets base scope
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Evaluation")> _
        <SortOrder(103)> _
        Public Overrides Property Scope() As VariableScope
            Get
                Return MyBase.Scope
            End Get
            Set(ByVal value As VariableScope)
                MyBase.Scope = value
            End Set
        End Property



    End Class
End Namespace