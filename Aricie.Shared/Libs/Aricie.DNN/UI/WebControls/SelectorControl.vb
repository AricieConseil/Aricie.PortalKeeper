Imports System.Web.UI.WebControls
Imports Aricie.ComponentModel
Imports System.Web.UI
Imports Aricie.Web.UI
Imports Aricie.DNN.Services
Imports DotNetNuke.Services.Localization

Namespace UI.WebControls
    Public MustInherit Class SelectorControl
        Inherits DropDownList

        Public Event SelectedItemChanged As EventHandler(Of ChangedEventArgs)

        Private _ExclusiveSelector As Boolean

        Private _ExclusiveScopeControlId As String
        Private _ExclusiveScopeControl As Control

        Private _AllItemsScopeId As String

        Private _ExcludedScopeId As String

        Private _InsertNullItem As Boolean

        Private _NullItemText As String = "NullSelect"

        Private _NullItemValue As String = ""

        Private _LocalizeNull As Boolean

        Private _LocalizeItems As Boolean


        Private _BoundItems As IList
        Private _PreviousSelectedObject As Object


#Region "Public Properties"


        Public Property ExclusiveSelector() As Boolean 'Implements 'ISelectorControl.ExclusiveSelector
            Get
                Return _ExclusiveSelector
            End Get
            Set(ByVal value As Boolean)
                _ExclusiveSelector = value
            End Set
        End Property

        <IDReferenceProperty(GetType(Control))> _
        Public Property ExclusiveScopeControlId() As String 'Implements 'ISelectorControl.ExclusiveScopeControlId
            Get
                If Me._ExclusiveScopeControlId = "" Then
                    Me._ExclusiveScopeControlId = Me.ID
                End If
                Return Me._ExclusiveScopeControlId
            End Get
            Set(ByVal value As String)
                Me._ExclusiveScopeControlId = value
            End Set
        End Property

        Public Property ExclusiveScopeControl() As Control 'Implements 'ISelectorControl.ExclusiveScopeControl
            Get
                If Me._ExclusiveScopeControl Is Nothing Then
					Me._ExclusiveScopeControl = Web.UI.ControlHelper.FindControl(Me, Me.ExclusiveScopeControlId)
                End If
                Return _ExclusiveScopeControl
            End Get
            Set(ByVal value As Control)
                _ExclusiveScopeControl = value
            End Set
        End Property

        Public Property AllItemsScopeId() As String

            Get
                If Me._AllItemsScopeId = "" Then
                    Me._AllItemsScopeId = "Aricie.Selector-" & Me.GetType().Name
                End If
                Return Me._AllItemsScopeId
            End Get
            Set(ByVal value As String)
                Me._AllItemsScopeId = value
            End Set
        End Property

        Public ReadOnly Property ExcludedScopeId() As String
            Get
                If Me._ExcludedScopeId = "" Then
                    Me._ExcludedScopeId = Me.AllItemsScopeId & "-Excluded-" & Me.ExclusiveScopeControl.ClientID
                End If
                Return Me._ExcludedScopeId
            End Get
        End Property


        Public Overridable ReadOnly Property AllItems() As IList
            Get
                If Not Me.Context.Items.Contains(AllItemsScopeId) Then
                    Me.Context.Items(AllItemsScopeId) = Me.GetEntities
                End If
                Return DirectCast(Me.Context.Items(AllItemsScopeId), IList)
            End Get
        End Property

        Public ReadOnly Property AvailableItems() As IList
            Get
                Dim toReturn As IList = AllItems
                If Me.ExclusiveSelector Then
                    toReturn = New ArrayList(Me.AllItems)
                    For Each objSelector As SelectorControl In Me.SelectorsInScope
                        If objSelector IsNot Me AndAlso objSelector.SelectedObject IsNot Nothing Then
                            toReturn.Remove(objSelector.SelectedObject)
                        End If
                    Next
                End If
                Return toReturn
            End Get
        End Property


        Public Property SelectedObject() As Object
            Get
                If Me.SelectedIndex >= GetNullBias() Then
                    Return Me.BoundItems(Me.SelectedIndex - GetNullBias())
                End If
                Return Nothing
            End Get
            Set(ByVal value As Object)
                'first get a unified instance of value for equality comparison
                Dim unifiedValue As Object
                If Me.UnifyInstances AndAlso value IsNot Nothing Then
                    unifiedValue = GetUnifiedInstance(Me.AllItems, Me.DataValueField, value)
                Else
                    unifiedValue = value
                End If

                'save previous value for further event notification.
                Me._PreviousSelectedObject = Me.SelectedObject

                If value IsNot Nothing Then

                    Me.SelectedIndex = Me.BoundItems.IndexOf(unifiedValue) + Me.GetNullBias
                Else
                    Me.SelectedIndex = Me.GetNullBias - 1

                End If
            End Set
        End Property

        Public Property BoundItems() As IList
            Get
                If Me.ExclusiveSelector Then
                    If Me._BoundItems Is Nothing Then
                        Me._BoundItems = New ArrayList
                        For Each idx As Integer In Me.BoundIndices
                            Me._BoundItems.Add(Me.AllItems(idx))
                        Next
                    End If

                    Return Me._BoundItems
                End If
                Return Me.AllItems
            End Get
            Set(ByVal value As IList)
                Me._BoundItems = value
                Me.BoundIndices.Clear()
                For Each item As Object In Me._BoundItems
                    Me.BoundIndices.Add(Me.AllItems.IndexOf(item))
                Next
            End Set
        End Property

        Public ReadOnly Property PreviousSelectedObject() As Object
            Get
                Return Me._PreviousSelectedObject
            End Get
        End Property


        Public Property NullItemText() As String 'Implements 'ISelectorControl.NullItemText
            Get
                Return _NullItemText
            End Get
            Set(ByVal value As String)
                _NullItemText = value
            End Set
        End Property

        Public Property NullItemValue() As String 'Implements 'ISelectorControl.NullItemValue
            Get
                Return _NullItemValue
            End Get
            Set(ByVal value As String)
                _NullItemValue = value
            End Set
        End Property

        Public Property InsertNullItem() As Boolean 'Implements 'ISelectorControl.InsertNullItem
            Get
                Return _InsertNullItem
            End Get
            Set(ByVal value As Boolean)
                _InsertNullItem = value
            End Set
        End Property

        Public Property LocalizeNull() As Boolean 'Implements ISelectorControl.LocalizeNull
            Get
                Return _LocalizeNull
            End Get
            Set(ByVal value As Boolean)
                _LocalizeNull = value
            End Set
        End Property


        Public Property LocalizeItems() As Boolean 'Implements ISelectorControl.LocalizeItems
            Get
                Return _LocalizeItems
            End Get
            Set(ByVal value As Boolean)
                _LocalizeItems = value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Dim toReturn As String = CStr(Me.ViewState("LocalResourceFile"))
                If String.IsNullOrEmpty(toReturn) Then
                    toReturn = GetModuleSharedResourceFile(GetParentModuleBase(Me).ModuleConfiguration.ModuleDefID)
                    Me.ViewState("LocalResourceFile") = toReturn
                End If
                Return toReturn
            End Get
            Set(ByVal value As String)
                Me.ViewState("LocalResourceFile") = value
            End Set
        End Property

#End Region


#Region "Private properties"

        Protected Overridable ReadOnly Property UnifyInstances() As Boolean
            Get
                Return True
            End Get
        End Property


        Private ReadOnly Property BoundIndices() As List(Of Integer)
            Get
                If Me.ViewState("BoundIndices") Is Nothing Then
                    Me.ViewState("BoundIndices") = New List(Of Integer)
                End If
                Return DirectCast(Me.ViewState("BoundIndices"), List(Of Integer))
            End Get
        End Property


        Private ReadOnly Property SelectorsInScope() As HashSet(Of SelectorControl)
            Get
                If Not Me.Context.Items.Contains(Me.ExcludedScopeId) Then
                    Me.Context.Items(Me.ExcludedScopeId) = New HashSet(Of SelectorControl)
                End If
                Return DirectCast(Me.Context.Items(Me.ExcludedScopeId), HashSet(Of SelectorControl))
            End Get
        End Property


#End Region




#Region "Event Handlers"


        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            MyBase.LoadViewState(savedState)
            Me._PreviousSelectedObject = Me.SelectedObject

        End Sub


        Private Sub SelectorControl_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles Me.SelectedIndexChanged
            Me.OnChanged(Me._PreviousSelectedObject)

        End Sub


        Private Sub SelectorControl_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender



            If Me.ExclusiveSelector Then

                Dim selObject As Object = Me.SelectedObject
                Me.SelectedIndex = -1
                Me.DataBind()

                Me.SelectedObject = selObject

            End If

        End Sub

        Public Sub OnSelectorUnload(ByVal sender As Object, ByVal e As EventArgs)
            Me.OnUnload(e)
        End Sub
        Protected Overrides Sub OnUnload(ByVal e As EventArgs)
            'if the control is removed, update the exclusion list
            If Me.ExclusiveSelector Then
                If Me.SelectorsInScope.Contains(Me) Then
                    Me.SelectorsInScope.Remove(Me)
                End If
            End If
            MyBase.OnUnload(e)
        End Sub

#End Region

#Region "Private methods"


        Protected Function GetNullBias() As Integer
            Return Convert.ToInt32(Me.InsertNullItem)
        End Function

        Private Sub ManageInsertNull()
            If Me.InsertNullItem Then
                Dim nullItem As New ListItem(Me.NullItemText, Me.NullItemValue)
                If Me.LocalizeNull Then
                    DoLocalizeNull(nullItem)
                End If
                Me.Items.Insert(0, nullItem)

            End If
        End Sub

        Private Function IsNullSelected() As Boolean
            Return Me.SelectedIndex < Me.GetNullBias()
        End Function

        Private Sub OnChanged(ByVal oldValue As Object)

            RaiseEvent SelectedItemChanged(Me, New ChangedEventArgs(oldValue, Me.SelectedObject))
        End Sub

        Protected Sub DoLocalizeItems()
            Dim typeName As String = Me.GetType.Name & "."
            Dim localText As String
            For i As Integer = 0 To Me.Items.Count - 1
                localText = Localization.GetString(typeName & Me.Items(i).Text, Me.LocalResourceFile)
                If Not String.IsNullOrEmpty(localText) Then
                    Me.Items(i).Text = localText
                End If
            Next
        End Sub

        Protected Sub DoLocalizeNull(ByVal nullItem As ListItem)
            Dim localText As String

            localText = Localization.GetString(Me.GetType.Name & ".NullSelect", Me.LocalResourceFile)
            If Not String.IsNullOrEmpty(localText) Then
                nullItem.Text = localText
            End If

        End Sub



#End Region

#Region "overrides"


        Public Overrides Sub DataBind()
            Try
                Me.InitSelector()
                Me.Items.Clear()

                Dim objDataSource As IList = Me.AvailableItems
                Me.DataSource = objDataSource

                If Me.ExclusiveSelector Then
                    Me.SelectorsInScope.Add(Me)
                    Me.BoundItems = objDataSource
                End If
                MyBase.DataBind()
                If Me.LocalizeItems Then
                    Me.DoLocalizeItems()
                End If
                ManageInsertNull()
            Catch ex As Exception
                If Me.Parent IsNot Nothing Then
                    DotNetNuke.Services.Exceptions.ProcessModuleLoadException(Me.Parent, ex)
                Else
                    Throw
                End If
            End Try
            
        End Sub


#End Region

#Region "overridable"


        Public Overridable Sub InitSelector()
          
        End Sub



#End Region


#Region "Abstract methods"

        Public MustOverride Function GetEntities() As IEnumerable 'Implements ISelectorControl.GetEntities


#End Region

    End Class


    Public MustInherit Class SelectorControl(Of T)
        Inherits SelectorControl

        Public Property SelectedObjectG() As T
            Get
                If (Me.SelectedObject Is Nothing) Then
                    Return Nothing
                End If
                Return DirectCast(Me.SelectedObject, T)
            End Get
            Set(ByVal value As T)
                Me.SelectedObject = value
            End Set
        End Property

        Public ReadOnly Property AllItemsG() As IList(Of T)
            Get
                Return DirectCast(Me.AllItems, IList(Of T))
            End Get
        End Property


        Public Overrides Function GetEntities() As IEnumerable
            Return Me.GetEntitiesG
        End Function

        Public Overrides Sub InitSelector()

        End Sub

        Public MustOverride Function GetEntitiesG() As IList(Of T) 'Implements ISelectorControl(Of T).GetEntitiesG

    End Class
End Namespace
