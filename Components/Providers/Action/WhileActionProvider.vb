
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.Security.Trial

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum WhileType
        WhileDo
        DoUntil
    End Enum
    <ActionButton(IconName.Repeat, IconOptions.Normal)> _
   <Serializable()> _
   <DisplayName("While Action Provider")> _
       <Description("This provider allows to loop running a sub bot while a boolean condition evaluates to true.")> _
    Public Class WhileActionProvider(Of TEngineEvents As IConvertible)
        Inherits MultipleActionProvider(Of TEngineEvents)


        Private _WhileCondition As New KeeperCondition(Of TEngineEvents)
        Private _WhileType As WhileType
        Private _MaxIterationNb As Integer = 0

        <ExtendedCategory("WhileAction")> _
       <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
               <LabelMode(LabelMode.Top)> _
               <TrialLimited(TrialPropertyMode.NoAdd Or TrialPropertyMode.NoDelete)> _
        Public Property WhileCondition() As KeeperCondition(Of TEngineEvents)
            Get
                Return _WhileCondition
            End Get
            Set(ByVal value As KeeperCondition(Of TEngineEvents))
                _WhileCondition = value
            End Set
        End Property


        <ExtendedCategory("WhileAction")> _
        Public Property WhileType() As WhileType
            Get
                Return _WhileType
            End Get
            Set(ByVal value As WhileType)
                _WhileType = value
            End Set
        End Property


        <ExtendedCategory("WhileAction")> _
        Public Property MaxIterationNb() As Integer
            Get
                Return _MaxIterationNb
            End Get
            Set(ByVal value As Integer)
                _MaxIterationNb = value
            End Set
        End Property



        Protected Overloads Overrides Function Run(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal aSync As Boolean) As Boolean
            If Me.DebuggerBreak Then
                Common.CallDebuggerBreak()
            End If
            Dim toReturn As Boolean = True
            Dim iterationNb As Integer = 0
            Select Case Me._WhileType
                Case WhileType.WhileDo
                    While Me._WhileCondition.Match(actionContext) AndAlso (_MaxIterationNb <= 0 OrElse iterationNb < _MaxIterationNb)
                        iterationNb += 1
                        toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                        If (Not toReturn) AndAlso Me.StopOnFailure Then
                            Exit While
                        End If
                    End While
                Case WhileType.DoUntil
                    Do
                        iterationNb += 1
                        toReturn = MyBase.Run(actionContext, aSync) AndAlso toReturn
                        If (Not toReturn) AndAlso Me.StopOnFailure Then
                            Exit Do
                        End If
                    Loop Until (Not Me._WhileCondition.Match(actionContext)) OrElse (_MaxIterationNb > 0 AndAlso iterationNb = _MaxIterationNb)
            End Select
            Return toReturn
        End Function

    End Class




End Namespace