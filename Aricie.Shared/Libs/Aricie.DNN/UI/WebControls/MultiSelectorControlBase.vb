Imports System.Web.UI.WebControls
Imports Aricie.ComponentModel
Imports System.Web.UI
Imports Aricie.Web.UI
Imports Aricie.Collections
Imports System.Reflection
Imports Aricie.Services

Namespace UI.WebControls

    Public MustInherit Class MultiSelectorControlBase
        Inherits CheckBoxList

        Public Event SelectedItemsChanged As EventHandler(Of ChangedEventArgs)

#Region "Private members"

        Private _ExclusiveSelector As Boolean
        Private _ExclusiveScopeControlId As String
        Private _AllItemsScopeId As String
        Private _ExcludedScopeId As String

        Private _AvailableItems As IList
        Private _ExcludedItems As IList
        Private _PreviousSelectedObjects As IList
        Private _SelectedObjects As IList

        Private _InvertExclusion As Boolean

#End Region

#Region "Public Properties"

        Public Property ExclusiveSelector() As Boolean
            Get
                Return _ExclusiveSelector
            End Get
            Set(ByVal value As Boolean)
                _ExclusiveSelector = value
            End Set
        End Property

        <IDReferenceProperty(GetType(Control))> _
        Public Property ExclusiveScopeControlId() As String
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

        Public ReadOnly Property AllItemsScopeId() As String
            Get
                If Me._AllItemsScopeId = "" Then
                    Me._AllItemsScopeId = "Aricie.Selector-" & Me.GetType().Name
                End If
                Return Me._AllItemsScopeId
            End Get
        End Property

        Public ReadOnly Property ExcludedScopeId() As String
            Get
                If Me._ExcludedScopeId = "" Then
					Dim ctrl As Control = Web.UI.ControlHelper.FindControl(Me, Me.ExclusiveScopeControlId)
                    If ctrl IsNot Nothing Then
                        Me._ExcludedScopeId = Me.AllItemsScopeId & "-Excluded-" & ctrl.ClientID
                    Else
						ctrl = Web.UI.ControlHelper.FindControlRecursive(Me.NamingContainer.NamingContainer, Me.ExclusiveScopeControlId)
                        Me._ExcludedScopeId = Me.AllItemsScopeId & "-Excluded-" & ctrl.ClientID
                    End If
                End If
                Return Me._ExcludedScopeId
            End Get
        End Property

        Public ReadOnly Property AllItems() As IList
            Get
                If Not Me.Context.Items.Contains(AllItemsScopeId) Then
                    Me.Context.Items(AllItemsScopeId) = Me.GetEntities
                End If
                Return DirectCast(Me.Context.Items(AllItemsScopeId), IList)
            End Get
        End Property

        Public ReadOnly Property AvailableItems() As IList
            Get
                If Me._AvailableItems Is Nothing Then
                    If Me.ExclusiveSelector AndAlso Me.ExcludedItems.Count > 0 Then
                        Me._AvailableItems = New ArrayList(Me.AllItems)
                        For Each item As Object In Me.ExcludedItems
                            If Me._SelectedObjects Is Nothing OrElse Not Me._SelectedObjects.Contains(item) Then
                                Me._AvailableItems.Remove(item)
                            End If
                        Next
                    Else
                        Me._AvailableItems = Me.AllItems
                    End If

                End If
                Return Me._AvailableItems
            End Get
        End Property

        Public Property SelectedObjects() As IEnumerable
            Get
                Return Me._SelectedObjects
            End Get
            Set(ByVal value As IEnumerable)

                Dim origValue As IList
                If Me.UnifyInstances AndAlso value IsNot Nothing Then
                    origValue = GetUnifiedInstances(Me.AllItems, Me.DataValueField, DirectCast(value, IList))
                Else
                    origValue = DirectCast(value, IList)
                End If

                If MatchLists(origValue, Me._SelectedObjects) Then
                    Exit Property
                End If
                Me._PreviousSelectedObjects = Me._SelectedObjects
                If Me._SelectedObjects IsNot Nothing Then
                    If Me.ExclusiveSelector Then
                        Me.UnRegisterSelectedItems(Me._SelectedObjects)
                    End If
                End If

                If origValue IsNot Nothing Then
                    If Me.ExclusiveSelector Then
                        Me.RegisterSelectedItems(DirectCast(value, IList))
                    End If
                Else
                    Me.ClearSelection()
                End If

                Me._SelectedObjects = origValue

            End Set
        End Property

        Public ReadOnly Property PreviousSelectedObjects() As IList
            Get
                Return Me._PreviousSelectedObjects
            End Get
        End Property

        Public ReadOnly Property GlobalSelectedIndexes() As IList(Of Integer)
            Get
                Dim toReturn As New List(Of Integer)
                For i As Integer = 0 To Me.Items.Count - 1
                    If Me.Items(i).Selected Then
                        toReturn.Add(Me.GlobalFromLocalIndex(i))
                    End If
                Next
                Return toReturn
            End Get
        End Property

        Public Property SelectedValues() As IList(Of String)
            Get
                Dim toReturn As New List(Of String)
                If Me.SelectedObjects IsNot Nothing Then
                    For Each obj As Object In Me.SelectedObjects
                        toReturn.Add(Me.GetValue(obj))
                    Next
                End If
                Return toReturn
            End Get
            Set(ByVal value As IList(Of String))
                'Dim objs As ArrayList = Me.GetObjects(value)
                Me.SelectedObjects = Me.GetObjects(value) 'objs
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Return CStr(Me.ViewState("LocalResourceFile"))
            End Get
            Set(ByVal value As String)
                Me.ViewState("LocalResourceFile") = value
            End Set
        End Property

        Public Property InvertExclusion() As Boolean
            Get
                Return _InvertExclusion
            End Get
            Set(ByVal value As Boolean)
                _InvertExclusion = value
            End Set
        End Property

        Public Overridable ReadOnly Property UnifyInstances() As Boolean
            Get
                Return True
            End Get
        End Property

#End Region

#Region "Public methods"

        Public Overrides Sub DataBind()
            If Me.DataSource Is Nothing Then
                Me.DataSource = Me.AllItems
            End If
            MyBase.DataBind()
        End Sub

        Public Shared Function MatchLists(ByVal list1 As IList, ByVal list2 As IList) As Boolean
            If list1 Is Nothing And list2 Is Nothing Then
                Return True
            End If
            If list1 IsNot Nothing AndAlso list2 IsNot Nothing AndAlso list1.Count = list2.Count Then
                For i As Integer = 0 To list1.Count - 1
                    If Not list1(i).Equals(list2(i)) Then
                        Return False
                    End If
                Next
                Return True
            End If
            Return False
        End Function

#End Region

#Region "Event Handlers"

        Private Sub SelectorControl_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            If Not Me.Page.IsPostBack Then
                DataBind()
            End If
        End Sub

        Protected Overrides Sub OnPagePreLoad(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.OnPagePreLoad(sender, e)
            Me.HandleSelection()
        End Sub

        Protected Overrides Sub RaisePostDataChangedEvent()

            MyBase.RaisePostDataChangedEvent()
            Me.HandleSelection()
        End Sub

        Public Overrides Property SelectedValue() As String
            Get
                Return MyBase.SelectedValue
            End Get
            Set(ByVal value As String)
                MyBase.SelectedValue = value
                SelectorControl_SelectedIndexChanged(Me, EventArgs.Empty)
            End Set
        End Property

        Private Sub SelectorControl_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles Me.SelectedIndexChanged
            HandleSelection()
            Me.OnChanged(Me._PreviousSelectedObjects)

        End Sub

        Private Sub SelectorControl_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender

            Dim selectedObjs As ArrayList
            If Me.SelectedObjects IsNot Nothing Then
                selectedObjs = New ArrayList(DirectCast(Me.SelectedObjects, IList))
            Else
                selectedObjs = New ArrayList
            End If

            Me.Items.Clear()
            Me.DataSource = Me.AvailableItems
            Me.DataBind()
            For i As Integer = 0 To Me.AvailableItems.Count - 1
                Me.GlobalFromLocalIndex(i) = Me.AllItems.IndexOf(Me.AvailableItems(i))
            Next
            If selectedObjs.Count > 0 Then
                For Each item As Object In selectedObjs
                    Me.Items(Me.AvailableItems.IndexOf(item)).Selected = True
                Next
            End If

            'If Attributes.

        End Sub

#End Region

#Region "Private Methods and Properties"

        Private ReadOnly Property ExcludedItems() As IList
            Get
                If Me._ExcludedItems Is Nothing Then
                    If Me.ExclusiveSelector Then
                        If Not Me.Context.Items.Contains(Me.ExcludedScopeId) Then
                            Me.Context.Items(Me.ExcludedScopeId) = New ArrayList
                        End If
                        Dim tempReturn As IList
                        If Me.InvertExclusion Then
                            tempReturn = GetComplementList(Me.AllItems, DirectCast(Me.Context.Items(Me.ExcludedScopeId), IList), Me.DataValueField)
                        Else
                            tempReturn = DirectCast(Me.Context.Items(Me.ExcludedScopeId), IList)
                        End If
                        If Me._SelectedObjects IsNot Nothing Then
                            Me._ExcludedItems = New ArrayList(tempReturn)
                            For Each item As Object In Me._SelectedObjects
                                If _ExcludedItems.Contains(item) Then
                                    _ExcludedItems.Remove(item)
                                End If
                            Next
                        Else
                            _ExcludedItems = tempReturn
                        End If
                    Else
                        Me._ExcludedItems = New ArrayList
                    End If
                End If
                Return Me._ExcludedItems
            End Get
        End Property

        Private Property GlobalFromLocalIndex(ByVal localIndex As Integer) As Integer
            Get
                Dim toReturn As Integer = localIndex
                If Me.ExclusiveSelector Then
                    If Me.ViewState("GlobalFromLocalIndex") Is Nothing Then
                        Me.ViewState("GlobalFromLocalIndex") = New SerializableDictionary(Of Integer, Integer)
                    End If
                    Dim _
                        dico As Dictionary(Of Integer, Integer) = _
                            DirectCast(Me.ViewState("GlobalFromLocalIndex"),  _
                            SerializableDictionary(Of Integer, Integer))
                    If dico.ContainsKey(localIndex) Then
                        toReturn = dico(localIndex)
                    End If
                End If

                Return toReturn
            End Get
            Set(ByVal value As Integer)
                If Me.ViewState("GlobalFromLocalIndex") Is Nothing Then
                    Me.ViewState("GlobalFromLocalIndex") = New SerializableDictionary(Of Integer, Integer)
                End If
                Dim _
                    dico As Dictionary(Of Integer, Integer) = _
                        DirectCast(Me.ViewState("GlobalFromLocalIndex"), SerializableDictionary(Of Integer, Integer))
                dico(localIndex) = value
            End Set
        End Property

        Public Sub HandleSelection()
            Dim selObjs As New ArrayList
            For Each index As Integer In Me.GlobalSelectedIndexes
                selObjs.Add(Me.AllItems(index))
            Next
            Me.SelectedObjects = selObjs
        End Sub

        Private Sub RegisterSelectedItems(ByVal items As IList)
            If Not Me.Context.Items.Contains(Me.ExcludedScopeId) Then
                Me.Context.Items(Me.ExcludedScopeId) = New ArrayList
            End If
            Dim allExcludedItems As ArrayList = DirectCast(Me.Context.Items(Me.ExcludedScopeId), ArrayList)
            allExcludedItems.AddRange(items)
        End Sub

        Private Sub UnRegisterSelectedItems(ByVal items As IList)

            If Not Me.Context.Items.Contains(Me.ExcludedScopeId) Then
                Me.Context.Items(Me.ExcludedScopeId) = New ArrayList
            End If
            Dim allExcludedItems As ArrayList = DirectCast(Me.Context.Items(Me.ExcludedScopeId), ArrayList)
            For Each item As Object In items
                allExcludedItems.Remove(item)
            Next

        End Sub

        Private Sub OnChanged(ByVal oldValues As IList)
            RaiseEvent SelectedItemsChanged(Me, New ChangedEventArgs(oldValues, Me.SelectedObjects))
        End Sub

#End Region

#Region "Abstract methods"

        Public MustOverride Function GetEntities() As IEnumerable
        Public MustOverride Function GetValue(ByVal fromObject As Object) As String
        Public MustOverride Function GetObject(ByVal value As String) As Object
        Public MustOverride Function GetObjects(ByVal values As IList(Of String)) As IEnumerable

#End Region

    End Class

End Namespace
