Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports Aricie.Collections
Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Diagnostics
Imports DotNetNuke.Entities.Profile
Imports System.Xml.Serialization
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Users
Imports Aricie.Services

Namespace Aricie.DNN.Modules.PortalKeeper


    <Serializable()> _
    Public Class UserBotInfo
        Inherits NamedConfig

        Private _NoOverride As Boolean
        Private _AnonymousRanking As Boolean

        Private _UserParameters As New SimpleList(Of UserVariableInfo)

        Private _PropertyDefinitions As New Dictionary(Of String, GeneralPropertyDefinition)
        Private _Entities As New Dictionary(Of String, Object)
        Private _Rankings As List(Of ProbeRanking)


        Public Property NoOverride() As Boolean
            Get
                Return _NoOverride
            End Get
            Set(ByVal value As Boolean)
                _NoOverride = value
            End Set
        End Property

        Public Property AnonymousRanking() As Boolean
            Get
                Return _AnonymousRanking
            End Get
            Set(ByVal value As Boolean)
                _AnonymousRanking = value
            End Set
        End Property

        <Browsable(False)> _
        Public Property UserParameters() As SimpleList(Of UserVariableInfo)
            Get
                Return _UserParameters
            End Get
            Set(ByVal value As SimpleList(Of UserVariableInfo))
                _UserParameters = value
            End Set
        End Property

       
        '<Obsolete("Use XmlParameters")> _
        <Browsable(False)> _
        Public Property Parameters() As SerializableList(Of Object)
            Get
                ' pas de get pour accomoder le property editor
                Return Nothing
            End Get
            Set(ByVal value As SerializableList(Of Object))
                If value IsNot Nothing Then
                    Me.Clear()
                    For i As Integer = 0 To Math.Min(value.Count - 1, Me.UserParameters.Instances.Count - 1)
                        Dim userParameter As UserVariableInfo = Me.UserParameters.Instances(i)
                        Select Case userParameter.Mode
                            Case UserParameterMode.PropertyDefinition
                                Me._PropertyDefinitions(userParameter.Name) = DirectCast(value(i), GeneralPropertyDefinition)
                            Case Else
                                Me._Entities(userParameter.Name) = value(i)
                        End Select
                    Next
                End If
            End Set
        End Property


        <Browsable(False)> _
        Public Property XmlParameters() As SerializableDictionary(Of String, Object)
            Get
                Dim toReturn As New SerializableDictionary(Of String, Object)
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    Select Case userParameter.Mode
                        Case UserParameterMode.PropertyDefinition
                            Dim propDef As GeneralPropertyDefinition = Nothing
                            If Not Me._PropertyDefinitions.TryGetValue(userParameter.Name, propDef) Then
                                'Throw New Exception(String.Format("Missing UserParameter {0}", userParameter.Name))
                            Else
                                toReturn(userParameter.Name) = propDef
                            End If
                        Case Else
                            Dim item As Object = Nothing
                            If Not Me._Entities.TryGetValue(userParameter.Name, item) Then
                                'Throw New Exception(String.Format("Missing UserParameter {0}", userParameter.Name))
                            Else
                                toReturn(userParameter.Name) = item
                            End If
                    End Select
                Next
                Return toReturn
            End Get
            Set(ByVal value As SerializableDictionary(Of String, Object))
                If value IsNot Nothing AndAlso value.Count > 0 Then
                    For i As Integer = 0 To Me.UserParameters.Instances.Count - 1
                        Dim userParameter As UserVariableInfo = Me.UserParameters.Instances(i)
                        Dim objValue As Object = Nothing
                        If value.TryGetValue(userParameter.Name, objValue) Then
                            Select Case userParameter.Mode
                                Case UserParameterMode.PropertyDefinition
                                    Me._PropertyDefinitions(userParameter.Name) = DirectCast(objValue, GeneralPropertyDefinition)
                                Case Else
                                    Me._Entities(userParameter.Name) = objValue
                            End Select
                        End If
                    Next
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property HasPropertyDefinition() As Boolean
            Get
                Return Me.PropertyDefinitions.Count > 0
            End Get
        End Property

        <XmlIgnore()> _
        <ExtendedCategory("Configuration")> _
            <ConditionalVisible("HasPropertyDefinition", False, False)> _
            <Editor(GetType(ProfileEditorEditControl), GetType(EditControl))> _
            <LabelMode(LabelMode.Top)> _
        Public Property PropertyDefinitions() As ProfilePropertyDefinitionCollection
            Get
                Dim toReturn As New ProfilePropertyDefinitionCollection
                For Each galDef As GeneralPropertyDefinition In Me._PropertyDefinitions.Values
                    toReturn.Add(galDef.ToDNNProfileDefinition)
                Next
                Return toReturn
            End Get
            Set(ByVal value As ProfilePropertyDefinitionCollection)
                Me._PropertyDefinitions = New Dictionary(Of String, GeneralPropertyDefinition)
                Dim index As Integer = 0
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    If userParameter.Mode = UserParameterMode.PropertyDefinition Then
                        If index < value.Count Then
                            'Me._PropertyDefinitions(userParameter.Name) = DirectCast(value(index), GeneralPropertyDefinition)
                            Me._PropertyDefinitions(userParameter.Name) = GeneralPropertyDefinition.FromDNNProfileDefinition(value(index))
                            index += 1
                        End If
                    End If
                Next
            End Set
        End Property

        '  <XmlIgnore()> _
        '<ExtendedCategory("Configuration")> _
        '    <ConditionalVisible("HasPropertyDefinition", False, False)> _
        '    <Editor(GetType(ProfileEditorEditControl), GetType(EditControl))> _
        '    <LabelMode(LabelMode.Top)> _
        '  Public Property PropertyDefinitions() As List(Of GeneralPropertyDefinition)
        '      Get
        '          Return New List(Of GeneralPropertyDefinition)(Me._PropertyDefinitions.Values)
        '      End Get
        '      Set(ByVal value As List(Of GeneralPropertyDefinition))
        '          Me._PropertyDefinitions = New Dictionary(Of String, GeneralPropertyDefinition)
        '          Dim index As Integer = 0
        '          For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
        '              If userParameter.Mode = UserParameterMode.PropertyDefinition Then
        '                  If index < value.Count Then
        '                      Me._PropertyDefinitions(userParameter.Name) = DirectCast(value(index), GeneralPropertyDefinition)
        '                      index += 1
        '                  End If
        '              End If
        '          Next
        '      End Set
        '  End Property



        <Browsable(False)> _
        Public ReadOnly Property HasEntities() As Boolean
            Get
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    If userParameter.Mode = UserParameterMode.ReflectedEditor AndAlso Not userParameter.IsReadOnly Then
                        Return True
                    End If
                Next
                Return False
            End Get
        End Property

        <Browsable(False)> _
        Public ReadOnly Property HasReadonlyEntities() As Boolean
            Get
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    If userParameter.Mode = UserParameterMode.ReflectedEditor AndAlso userParameter.IsReadOnly Then
                        Return True
                    End If
                Next
                Return False
            End Get
        End Property

        <XmlIgnore()> _
        <OnDemand(False)> _
        <ExtendedCategory("Configuration")> _
            <ConditionalVisible("HasEntities", False, False)> _
            <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(True, False, False, False, 11, CollectionDisplayStyle.Accordion, False)> _
            <LabelMode(LabelMode.Top)> _
        Public Property Entities() As SerializableList(Of Object)
            Get
                Dim toReturn As New SerializableList(Of Object)
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    If userParameter.Mode = UserParameterMode.ReflectedEditor AndAlso Not userParameter.IsReadOnly Then
                        toReturn.Add(Me._Entities(userParameter.Name))
                    End If
                Next
                Return toReturn
            End Get
            Set(ByVal value As SerializableList(Of Object))
                ' on ne fait rien mais on laisse la propriété en read write pour le property editor
            End Set
        End Property

        <XmlIgnore()> _
        <OnDemand(False)> _
       <ExtendedCategory("Configuration")> _
           <ConditionalVisible("HasReadonlyEntities", False, False)> _
           <Editor(GetType(ListEditControl), GetType(EditControl))> _
           <CollectionEditor(True, False, False, False, 11, CollectionDisplayStyle.Accordion, False)> _
           <LabelMode(LabelMode.Top)> _
        Public ReadOnly Property ReadonlyEntities() As SerializableList(Of Object)
            Get
                Dim toReturn As New SerializableList(Of Object)
                For Each userParameter As UserVariableInfo In Me._UserParameters.Instances
                    If userParameter.Mode = UserParameterMode.ReflectedEditor AndAlso userParameter.IsReadOnly Then
                        toReturn.Add(Me._Entities(userParameter.Name))
                    End If
                Next
                Return toReturn
            End Get
        End Property




        <IsReadOnly(True)> _
        <XmlIgnore()> _
        <OnDemand(False)> _
       <ExtendedCategory("Configuration")> _
           <CollectionEditor(True, False, False, False, 11, CollectionDisplayStyle.Accordion, False)> _
           <LabelMode(LabelMode.Top)> _
        Public Property ComputedEntities() As SerializableDictionary(Of String, Object)
      



        Private _BotHistory As New WebBotHistory

        <LabelMode(LabelMode.Top), IsReadOnly(True)> _
        <ExtendedCategory("History")> _
        Public Property BotHistory() As WebBotHistory
            Get
                Return _BotHistory
            End Get
            Set(ByVal value As WebBotHistory)
                _BotHistory = value
            End Set
        End Property



        <XmlIgnore()> _
        <OnDemand(False)> _
        <LabelMode(LabelMode.Top), IsReadOnly(True)> _
       <ExtendedCategory("Rankings")> _
        Public Property Rankings() As List(Of ProbeRanking)
            Get
                Return _Rankings
            End Get
            Set(ByVal value As List(Of ProbeRanking))
                _Rankings = value
            End Set
        End Property



        Private Sub Clear()
            Me._PropertyDefinitions = New Dictionary(Of String, GeneralPropertyDefinition)
            Me._Entities = New Dictionary(Of String, Object)
        End Sub

        Public Sub SetPropertyDefinition(ByVal name As String, ByVal def As GeneralPropertyDefinition)
            Me._PropertyDefinitions(name) = def
        End Sub
        Public Sub SetEntity(ByVal name As String, ByVal obj As Object)
            Me._Entities(name) = obj
        End Sub

        Public Sub RevertReadonlyParameters(originalVersion As UserBotInfo)
            For Each objVar As UserVariableInfo In Me.UserParameters.Instances
                If objVar.IsReadOnly Then
                    Select Case objVar.Mode
                        Case UserParameterMode.PropertyDefinition
                            Dim propDef As GeneralPropertyDefinition = Nothing
                            If originalVersion._PropertyDefinitions.TryGetValue(objVar.Name, propDef) Then
                                Me.SetPropertyDefinition(objVar.Name, propDef)
                            End If
                        Case UserParameterMode.ReflectedEditor
                            Dim entity As Object = Nothing
                            If originalVersion._Entities.TryGetValue(objVar.Name, entity) Then
                                Me.SetEntity(objVar.Name, entity)
                            End If
                    End Select
                End If
            Next
        End Sub

        Public Function GetParameterValues(ByVal objUser As UserInfo) As IDictionary(Of String, Object)
            Dim toReturn As IDictionary(Of String, Object) = Me.GetParameterValues
            toReturn("User") = objUser
            Return toReturn
        End Function

        Public Function GetParameterValues() As IDictionary(Of String, Object)
            Dim toReturn As New SerializableDictionary(Of String, Object)
            For Each objPair As KeyValuePair(Of String, GeneralPropertyDefinition) In Me._PropertyDefinitions
                toReturn(objPair.Key) = objPair.Value.TypedValue
            Next
            For Each objPair As KeyValuePair(Of String, Object) In Me._Entities
                toReturn(objPair.Key) = objPair.Value
            Next

            toReturn("UserBot") = Me
            Return toReturn
        End Function

    End Class
End Namespace