Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Ciloci.Flee
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper
    <ActionButton(IconName.Magic, IconOptions.Normal)> _
    <Serializable()> _
        <DisplayName("Define Variables Action Provider")> _
        <Description("This provider allows to declare and instanciate a set of variables, which will be stored in the context ""Item"" dictionary")> _
    Public Class DefineVarsActionProvider(Of TEngineEvents As IConvertible)
        Inherits CacheableAction(Of TEngineEvents)


        Private _Variables As New Variables

        Private _GetFromHistory As Boolean


        <ExtendedCategory("Variables")> _
        Public Property GetFromHistory As Boolean
            Get
                Return _GetFromHistory
            End Get
            Set(value As Boolean)
                _GetFromHistory = value
            End Set
        End Property

        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
            <ExtendedCategory("Variables")> _
            <MainCategory()> _
        Public Property Variables() As Variables
            Get
                Return _Variables
            End Get
            Set(ByVal value As Variables)
                _Variables = value
            End Set
        End Property




        Public Overrides Function BuildResult(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean) As Object
            If Me.DebuggerBreak Then
                Me.CallDebuggerBreak()
            End If
            Dim toReturn As SerializableDictionary(Of String, Object) = Me._Variables.EvaluateVariables(actionContext, actionContext)
            If Me._GetFromHistory Then
                Dim newValue As Object = Nothing
                For Each objKey As String In New List(Of String)(toReturn.Keys)
                    If actionContext.Items.TryGetValue("Last" & objKey, newValue) Then
                        toReturn(objKey) = newValue
                    End If
                Next
            End If
            Return toReturn
        End Function

        Public Overrides Function RunFromObject(ByVal actionContext As PortalKeeperContext(Of TEngineEvents), ByVal async As Boolean, ByVal cachedObject As Object) As Boolean
            Dim vars As Dictionary(Of String, Object) = DirectCast(cachedObject, Dictionary(Of String, Object))
            For Each objPair As KeyValuePair(Of String, Object) In vars
                'If TypeOf objPair.Value Is IDynamicExpression Then
                '    actionContext.Items(objPair.Key) = DirectCast(objPair.Value, IDynamicExpression).Evaluate()
                'Else
                '    actionContext.Items(objPair.Key) = objPair.Value
                'End If
                actionContext.Items(objPair.Key) = objPair.Value
            Next
            Return True
        End Function


        Public Overrides Sub AddVariables(currentProvider As IExpressionVarsProvider, ByRef existingVars As IDictionary(Of String, Type))
            Me.Variables.AddVariables(currentProvider, existingVars)
            MyBase.AddVariables(currentProvider, existingVars)
        End Sub

    End Class
End Namespace