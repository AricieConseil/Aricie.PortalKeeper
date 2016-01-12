Imports System.ComponentModel
Imports Aricie.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.DNN.UI.Attributes
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    <ActionButton(IconName.Ban, IconOptions.Normal)> _
    Public Class DenialOfServiceSettings

        Private _Enable As Boolean = True
        Private _DoSAction As New KeeperAction(Of RequestEvent)

        <ExtendedCategory("")> _
            <MainCategory()> _
        Public Property Enabled() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        <ExtendedCategory("")> _
            <Editor(GetType(PropertyEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property DoSAction() As KeeperAction(Of RequestEvent)
            Get
                Return Me._DoSAction
            End Get
            Set(ByVal value As KeeperAction(Of RequestEvent))
                Me._DoSAction = value
            End Set
        End Property

        <Category("BlackList")> _
            <LabelMode(LabelMode.Top)> _
        Public ReadOnly Property DosBlackList() As String
            Get
                Dim toReturn As New StringBuilder
                DosEnabledConditionProvider(Of RequestEvent).BeginReadProviders()
                Dim tempProvs As New List(Of DosEnabledConditionProvider(Of RequestEvent))(DosEnabledConditionProvider(Of RequestEvent).DosProviders)
                DosEnabledConditionProvider(Of RequestEvent).EndReadProviders()
                If tempProvs.Count > 0 Then
                    For Each dosConditionSettings As DosEnabledConditionProvider(Of RequestEvent) In tempProvs
                        toReturn.Append(dosConditionSettings.GetCurrentBans(300))
                    Next
                Else
                    toReturn.Append("Black List is Empty")
                End If
                Return toReturn.ToString
            End Get
        End Property


    End Class
End Namespace