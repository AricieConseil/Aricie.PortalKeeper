Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports DotNetNuke.UI.WebControls

Namespace Services.Flee

    ''' <summary>
    ''' Class to use flee constructors
    ''' </summary>
    ''' <typeparam name="TResult"></typeparam>
    ''' <remarks></remarks>
    <DisplayName("Constructor")> _
    <Serializable()> _
    Public Class CtorVariableInfo(Of TResult)
        Inherits InstanceVariableInfo(Of TResult)

        ''' <summary>
        ''' Parameters that will be evaluated for the constructor of TResult
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExtendedCategory("Definition")> _
        <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
        <LabelMode(LabelMode.Top)> _
        Public Property Parameters() As New Variables

        ''' <summary>
        ''' Evaluate Constructor and returns a TResult
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function EvaluateOnce(ByVal owner As Object, ByVal globalVars As IContextLookup) As Object
            Dim args As New List(Of Object)
            For Each objParam As KeyValuePair(Of String, Object) In Me._Parameters.EvaluateVariables(owner, globalVars)
                args.Add(objParam.Value)
            Next
            Return System.Activator.CreateInstance(GetType(TResult), args.ToArray)
        End Function
    End Class
End Namespace