Imports Aricie.DNN.ComponentModel
Imports Aricie.Collections
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports System.Web.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    <Serializable()>
    Public Class ExpressionBuilder
        Implements ISelector

        Public Sub New()

        End Sub

        Public Sub New(vars As IDictionary(Of String, DotNetType))
            For Each objVar In vars
                _AvailableVariables(objVar.Key) = objVar.Value
            Next
        End Sub


        Private ReadOnly _AvailableVariables As New SerializableDictionary(Of String, DotNetType)


        <XmlIgnore()> _
        <AutoPostBack()> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <Selector("Text", "Value", False, True, "<-- Select a Variable -->", "", False, True)> _
        Public Property SelectedVariable As String = ""

        <XmlIgnore()> _
        <AutoPostBack()> _
        <Selector("Text", "Value", False, True, "<-- Select a Member -->", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("SelectedVariable", True, True, "")> _
        Public Property SelectedMember As String = ""


        <Browsable(False)> _
        Public ReadOnly Property SelectedMemberInfo As MemberInfo
            Get
                Dim selectedVarType As Type = _AvailableVariables(SelectedVariable).GetDotNetType()
                Dim objMember As MemberInfo = Nothing
                If Not ReflectionHelper.GetReturnSubMembers(selectedVarType).TryGetValue(SelectedMember, objMember) Then
                    Throw New ApplicationException(String.Format("Selected Member {0} Not found in available sub members from Variable {1}", SelectedMember, SelectedVariable))
                End If
                Return objMember
            End Get
        End Property

        <ConditionalVisible("SelectedVariable", True, True, "")> _
     <ConditionalVisible("SelectedMember", True, True, "")> _
        Public ReadOnly Property SelectedMemberSignature As String
            Get
                Return ReflectionHelper.GetMemberSignature(SelectedMemberInfo, False)
            End Get
        End Property


        <XmlIgnore()> _
        <AutoPostBack()> _
        <Selector("Text", "Value", False, True, "<-- Select a SubMember -->", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
         <ConditionalVisible("SelectedVariable", True, True, "")> _
        <ConditionalVisible("SelectedMember", True, True, "")> _
        Public Property SelectedSubMember As String = ""



        Public Function GetInsertString() As String
            Dim toReturn As String = Me.SelectedVariable
            If Not SelectedMember.IsNullOrEmpty() Then
                toReturn &= "."c & Me.SelectedMember
            End If
            If Not SelectedSubMember.IsNullOrEmpty() Then
                toReturn &= "."c & Me.SelectedSubMember
            End If
            Return toReturn
        End Function

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "SelectedVariable"
                    Return (_AvailableVariables.Keys).Select(Function(objString) New ListItem(objString)).ToList()
                Case "SelectedMember"
                    Dim selectedVarType As Type = _AvailableVariables(SelectedVariable).GetDotNetType()
                    Return ReflectionHelper.GetReturnSubMembers(selectedVarType).Select(Function(objMemberPair) New ListItem(objMemberPair.Key)).ToList()
                Case "SelectedSubMember"
                    Dim selectedVarType As Type = _AvailableVariables(SelectedVariable).GetDotNetType()
                    Dim objMember As MemberInfo = SelectedMemberInfo
                    Dim objMemberType As Type = ReflectionHelper.GetMemberReturnType(objMember, True)
                    Return ReflectionHelper.GetReturnSubMembers(objMemberType).Select(Function(objMemberPair) New ListItem(objMemberPair.Key)).ToList()
            End Select
            Return Nothing
        End Function


    End Class
End NameSpace