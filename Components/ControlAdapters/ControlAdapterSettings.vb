Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Aricie.DNN.Services
Imports Aricie.Services
Imports Aricie.DNN.UI.WebControls.EditControls
Imports Aricie.Collections
Imports Aricie.DNN.UI.WebControls

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum DynamicHandlerType
        Typed
        UnTyped
    End Enum

    '<DefaultProperty("FriendlyName")> _
    <Serializable()> _
    Public Class ControlAdapterSettings
        Inherits NamedConfig
        Implements IProviderContainer


        '<Browsable(False)> _
        'Public ReadOnly Property FriendlyName As String
        '    Get
        '        If ResolvedAdaptedControlType IsNot Nothing Then
        '            Return ResolvedAdaptedControlType.Name
        '        End If
        '        Return "Select Adapted Control Type"
        '    End Get
        'End Property

        Public Property AdaptedMode As AdaptedControlMode

        <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Path)> _
        Public Property AdaptedControlPath As String = ""

        <ConditionalVisible("AdaptedMode", False, True, AdaptedControlMode.Type)> _
        Public Property AdaptedControlType As New DotNetType()


        Private _ResolvedAdaptedType As Type

        <Browsable(False)> _
        Public ReadOnly Property ResolvedAdaptedControlType As Type
            Get
                If _ResolvedAdaptedType Is Nothing Then
                    Try
                        Select Case Me.AdaptedMode
                            Case AdaptedControlMode.Type
                                _ResolvedAdaptedType = Me.AdaptedControlType.GetDotNetType()
                            Case AdaptedControlMode.Path
                                If Not String.IsNullOrEmpty(Me.AdaptedControlPath) AndAlso DnnContext.Current.Page IsNot Nothing Then
                                    Dim tempUserControl As Control = DnnContext.Current.Page.LoadControl(Me.AdaptedControlPath)
                                    _ResolvedAdaptedType = tempUserControl.GetType()
                                End If
                        End Select
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return _ResolvedAdaptedType
            End Get
        End Property


        Private _ResolvedAdapterType As Type

        <Browsable(False)> _
        Public ReadOnly Property ResolvedAdapterControlType As Type
            Get
                If _ResolvedAdapterType Is Nothing Then
                    Try
                        Select Case Me.AdapterMode
                            Case AdapterControlMode.Type
                                _ResolvedAdapterType = Me.AdapterControlType.GetDotNetType()
                            Case AdapterControlMode.DynamicAdapter
                                _ResolvedAdapterType = DynamicControlAdapter.GetGenericType(Me.ResolvedAdaptedControlType)
                        End Select
                    Catch ex As Exception
                        ExceptionHelper.LogException(ex)
                    End Try
                End If
                Return _ResolvedAdapterType
            End Get
        End Property


        Public Property AdapterMode As AdapterControlMode

        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.Type)> _
        Public Property AdapterControlType As New DotNetType()

        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.DynamicAdapter)> _
        Public Property HandlersRegistrationStep As HandlersRegistrationStep = HandlersRegistrationStep.OnInit

        <ProvidersSelector("Text", "Value")> _
        <ConditionalVisible("AdapterMode", False, True, AdapterControlMode.DynamicAdapter)> _
        Public Property DynamicHandlers As New SerializableList(Of DynamicHandlerSettings)


        Private _DynamicHandlersDictionary As Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)

        <XmlIgnore()> _
        <Browsable(False)> _
        Public ReadOnly Property DynamicHandlersDictionary As Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)
            Get
                If _DynamicHandlersDictionary Is Nothing Then
                    Dim newDico As New Dictionary(Of DynamicHandlerStep, DynamicHandlerSettings)
                    For Each objSettings As DynamicHandlerSettings In Me.DynamicHandlers
                        newDico(objSettings.GetStep()) = objSettings
                    Next
                    Me._DynamicHandlersDictionary = newDico
                End If
                Return _DynamicHandlersDictionary
            End Get
        End Property

        <ActionButton(IconName.Anchor, IconOptions.Normal)> _
        Public Sub UpgradeDynamicHandlers(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            Dim genericDynamicHandlerType As Type = Me.GetGenericDynamicHandlerType()
            SyncLock Me
                Dim tempList As New SerializableList(Of DynamicHandlerSettings)
                For Each objDynamicHandler As DynamicHandlerSettings In Me.DynamicHandlers

                    Dim objHandlerType As Type = objDynamicHandler.GetType()

                    If objHandlerType Is GetType(DynamicHandlerSettings) Then
                        tempList.Add(DirectCast(Activator.CreateInstance(genericDynamicHandlerType, objDynamicHandler), DynamicHandlerSettings))
                    Else
                        tempList.Add(objDynamicHandler)
                    End If
                Next
                Me.DynamicHandlers = tempList
            End SyncLock

        End Sub

        <ActionButton(IconName.Suitcase, IconOptions.Normal)> _
        Public Sub DowngradeDynamicHandlers(ape As Aricie.DNN.UI.WebControls.AriciePropertyEditorControl)
            SyncLock Me
                Dim tempList As New SerializableList(Of DynamicHandlerSettings)
                For Each objDynamicHandler As DynamicHandlerSettings In Me.DynamicHandlers
                    Dim objHandlerType As Type = objDynamicHandler.GetType()
                    If objHandlerType IsNot GetType(DynamicHandlerSettings) Then
                        tempList.Add(objDynamicHandler.Downgrade())
                    Else
                        tempList.Add(objDynamicHandler)
                    End If
                Next
                Me.DynamicHandlers = tempList
            End SyncLock
        End Sub


        Public Function GetSelector(propertyName As String) As IList Implements ISelector.GetSelector
            Select Case propertyName
                Case "DynamicHandlers"
                    Dim toReturn As New ListItemCollection
                    toReturn.Add(New ListItem("Typed Handler", "Typed"))
                    toReturn.Add(New ListItem("Portable Handler", "Portable"))
                    Return toReturn
            End Select

        End Function


        Public Function GetNewItem(collectionPropertyName As String, providerName As String) As Object Implements IProviderContainer.GetNewItem
            If collectionPropertyName = "DynamicHandlers" Then
                If providerName = "Portable" Then
                    Return New DynamicHandlerSettings()
                Else
                    Return ReflectionHelper.CreateObject(GetGenericDynamicHandlerType())
                End If
            End If
            Return Nothing
        End Function

        'Private _GenericDynamicHandlerType As Type

        Public Function GetGenericDynamicHandlerType() As Type
            'If _GenericDynamicHandlerType Is Nothing Then
            Return GetType(DynamicHandlerSettings(Of )).MakeGenericType(Me.ResolvedAdaptedControlType)
            'End If
            'Return _GenericDynamicHandlerType
        End Function


    End Class
End Namespace