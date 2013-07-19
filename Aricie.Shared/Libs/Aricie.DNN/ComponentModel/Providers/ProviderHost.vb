Imports Aricie.DNN.UI.WebControls.EditControls
Imports System.ComponentModel
Imports Aricie.DNN.UI.Attributes
Imports Aricie.ComponentModel
Imports Aricie.Collections
Imports DotNetNuke.UI.WebControls
Imports Aricie.Services

Namespace ComponentModel



    <Serializable()> _
    Public MustInherit Class ProviderHost(Of TConfig As IProviderConfig(Of TProvider), TSettings, TProvider As IProvider)
        Implements IProviderContainer
        Implements ISelector(Of TConfig)




        Private _Instances As New SerializableList(Of TSettings)

        <ExtendedCategory("")> _
        <MainCategory()> _
        <Editor(GetType(ListEditControl), GetType(EditControl))> _
            <CollectionEditor(False, False, True, True, 11, CollectionDisplayStyle.Accordion, False)> _
            <ProvidersSelector()> _
            <LabelMode(LabelMode.Top)> _
            <TrialLimited(Security.Trial.TrialPropertyMode.NoAdd Or Security.Trial.TrialPropertyMode.NoDelete)> _
        Public Property Instances() As SerializableList(Of TSettings)
            Get
                Return _Instances
            End Get
            Set(ByVal value As SerializableList(Of TSettings))
                _Instances = value
                Me.OnCollectionChange()
            End Set
        End Property

        Protected Overridable Sub OnCollectionChange()

        End Sub


        Public MustOverride Function GetAvailableProviders() As System.Collections.Generic.IDictionary(Of String, TConfig)


        Private _AvailableProviders As IDictionary(Of String, TConfig)

        <Browsable(False)> _
        Public ReadOnly Property AvailableProviders As IDictionary(Of String, TConfig)
            Get
                If _AvailableProviders Is Nothing Then
                    _AvailableProviders = GetAvailableProviders()
                End If
                Return _AvailableProviders
            End Get
        End Property



        Public Function GetSelector(ByVal propertyName As String) As System.Collections.IList Implements ISelector.GetSelector
            Return DirectCast(GetSelectorG(propertyName), IList)
        End Function


        Public Function GetSelectorG(ByVal propertyName As String) As System.Collections.Generic.IList(Of TConfig) Implements ISelector(Of TConfig).GetSelectorG
            Select Case propertyName
                Case "Instances"
                    Return New List(Of TConfig)(Me.AvailableProviders.Values)
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Function GetNewItem(ByVal collectionPropertyName As String, ByVal providerName As String) As Object Implements UI.WebControls.EditControls.IProviderContainer.GetNewItem
            Select Case collectionPropertyName
                Case "Instances"
                    Dim toReturn As TSettings
                    Dim objProvider As TProvider = Me.AvailableProviders(providerName).GetTypedProvider
                    If TypeOf objProvider Is IProvider(Of TConfig, TSettings) Then
                        toReturn = DirectCast(objProvider, IProvider(Of TConfig, TSettings)).GetNewProviderSettings
                    Else
                        toReturn = ReflectionHelper.CreateObject(Of TSettings)()
                    End If
                    If toReturn IsNot Nothing AndAlso TypeOf toReturn Is IProviderSettings Then
                        DirectCast(toReturn, IProviderSettings).ProviderName = providerName
                    End If

                    Return toReturn
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class
End Namespace
