Imports Aricie.DNN.ComponentModel
Imports Aricie.Services

Namespace Services.Flee

   
    ''' <summary>
    ''' List of flee actions holder class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class ObjectActionsBase
        Inherits ProviderHost(Of DotNetType(Of ObjectAction), ObjectAction, IGenericizer(Of ObjectAction))


        Protected _ExpressionTypes As New List(Of DotNetType)
        Private Shared ReadOnly _GenerictypeLessTypeSetProp As Type = GetType(SetObjectProperty(Of ))
        Private Shared ReadOnly _GenerictypeLessTypeCallMethod As Type = GetType(CallObjectMethod(Of ))
        Private Shared ReadOnly _GenerictypeLessTypeAddHandler As Type = GetType(AddEventHandler(Of ))

        ''' <summary>
        ''' Run flee actions
        ''' </summary>
        ''' <param name="owner"></param>
        ''' <param name="globalVars"></param>
        ''' <remarks></remarks>
        Public Sub Run(ByVal owner As Object, ByVal globalVars As IContextLookup)
            For Each objAction As ObjectAction In Me.Instances
                If objAction.Enabled Then
                    objAction.Run(owner, globalVars)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Returns collection of object actions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetAvailableProviders() As System.Collections.Generic.IDictionary(Of String, DotNetType(Of ObjectAction))
            Dim toReturn As New Dictionary(Of String, DotNetType(Of ObjectAction))
            toReturn.Add(ReflectionHelper.GetSimpleTypeName(GetType(GeneralObjectAction)), New DotNetType(Of ObjectAction)(GetType(GeneralObjectAction)))
            If Me._ExpressionTypes.Count = 0 Then
                Me._ExpressionTypes.AddRange(GetInitialTypes())
            End If
            For Each simpleDotNetType As DotNetType In Me._ExpressionTypes
                Dim simpleType As Type = simpleDotNetType.GetDotNetType
                If simpleType IsNot Nothing Then
                    Dim toAdd As New DotNetType(Of ObjectAction)(_GenerictypeLessTypeSetProp, simpleDotNetType)
                    toReturn.Add(toAdd.Name, toAdd)
                   
                    toAdd = New DotNetType(Of ObjectAction)(_GenerictypeLessTypeCallMethod, simpleDotNetType)
                    toReturn.Add(toAdd.Name, toAdd)
                    toAdd = New DotNetType(Of ObjectAction)(_GenerictypeLessTypeAddHandler, simpleDotNetType)
                    toReturn.Add(toAdd.Name, toAdd)
                End If
            Next

            Return toReturn
        End Function

        ''' <summary>
        ''' Returns a list containing the type of Object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetInitialTypes() As IList(Of DotNetType)
            Dim toReturn As New List(Of DotNetType)
            toReturn.Add(New DotNetType(GetType(Object)))
            Return toReturn
        End Function

    End Class
End Namespace