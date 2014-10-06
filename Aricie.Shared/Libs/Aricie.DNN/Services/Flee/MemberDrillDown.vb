Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports System.Reflection
Imports System.Web.UI.WebControls
Imports Aricie.Services

Namespace Services.Flee
    <Serializable()> _
    Public Class MemberDrillDown
        Inherits FleeExpressionBuilderBase
        Implements ISelector

        Private _SelectedSubMember As MemberDrillDown
        Private _SelectedMember As String = ""
        Private _SelectSubMember As Boolean

        Public Sub New()
        End Sub

        Public Sub New(objParentType As Type)
            Me.ParentType = New DotNetType(objParentType)
        End Sub
        Public Sub New(objParentType As DotNetType)
            Me.ParentType = objParentType
        End Sub


        <Browsable(False)> _
        Public Property ParentType As DotNetType


        <AutoPostBack()> _
        <Selector("Text", "Value", False, True, "<- Select a sub member ->", "", False, True)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        Public Property SelectedMember As String
            Get
                Return _SelectedMember
            End Get
            Set(value As String)
                If _SelectedMember <> value Then
                    _SelectedMember = value
                    _SelectedSubMember = Nothing
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property SelectedMemberInfo As MemberInfo
            Get
                'Dim selectedVarType As Type = _AvailableVariables(SelectedVariable).GetDotNetType()

                Dim objMember As MemberInfo = Nothing
                If Not SelectedMember.IsNullOrEmpty() AndAlso Not ReflectionHelper.GetReturnSubMembers(ParentType.GetDotNetType()).TryGetValue(SelectedMember, objMember) Then
                    Throw New ApplicationException(String.Format("Selected Member {0} Not found in available sub members from Variable Type {1}", SelectedMember, Me.ParentType.TypeName))
                End If
                Return objMember
            End Get
        End Property

        '<ConditionalVisible("SelectedMember", True, True, "")> _
        'Public ReadOnly Property SelectedMemberSignature As String
        '    Get
        '        Return ReflectionHelper.GetMemberSignature(SelectedMemberInfo, False)
        '    End Get
        'End Property

        '<ConditionalVisible("SelectedMember", True, True, "")>
        'Public Property SelectSubMember As Boolean
        '    Get
        '        Return _SelectSubMember
        '    End Get
        '    Set(value As Boolean)
        '        If Not value Then
        '            Me.SelectedSubMember = Nothing
        '        End If
        '        _SelectSubMember = value
        '    End Set
        'End Property

        '<ConditionalVisible("SelectSubMember", False, True)> _
        <ConditionalVisible("SelectedMember", True, True, "")> _
        Public Property SelectedSubMember As MemberDrillDown
            Get
                If Not SelectedMember.IsNullOrEmpty Then
                    If _SelectedSubMember Is Nothing Then
                        Dim objMemberType As Type = ReflectionHelper.GetMemberReturnType(SelectedMemberInfo, True)
                        _SelectedSubMember = New MemberDrillDown(objMemberType)
                    End If
                    Return _SelectedSubMember
                End If
                Return Nothing
            End Get
            Set(value As MemberDrillDown)
                _SelectedSubMember = value
            End Set
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property InsertString As String
            Get
                Dim toReturn As String = ""
                If Not SelectedMember.IsNullOrEmpty() Then
                    toReturn &= "."c & Me.SelectedMember & SelectedSubMember.InsertString
                End If
                Return toReturn
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property ResultingMemberInfo As MemberInfo
            Get
                If SelectedSubMember IsNot Nothing Then
                    Return SelectedSubMember.ResultingMemberInfo
                End If
                Return SelectedMemberInfo
            End Get
        End Property

        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "SelectedMember"
                    Dim selectedVarType As Type = ParentType.GetDotNetType()
                    Return ReflectionHelper.GetReturnSubMembers(selectedVarType).Select(Function(objMemberPair) New ListItem(objMemberPair.Key & " ( " & ReflectionHelper.GetMemberSignature(objMemberPair.Value, False) & " ) ", objMemberPair.Key)).ToList()
                    'Case "SelectedSubMember"
                    '    Dim objMember As MemberInfo = SelectedMemberInfo
                    '    Dim objMemberType As Type = ReflectionHelper.GetMemberReturnType(objMember, True)
                    '    Return ReflectionHelper.GetReturnSubMembers(objMemberType).Select(Function(objMemberPair) New ListItem(objMemberPair.Key)).ToList()
            End Select
            Return Nothing
        End Function
    End Class
End NameSpace