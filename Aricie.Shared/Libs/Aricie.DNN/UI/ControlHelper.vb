Imports DotNetNuke.UI.UserControls
Imports System.Web.UI.WebControls

Namespace UI

	Public Module ControlHelper

		Public Sub DisableDualList(ByRef dualList As DualListControl)

			SetDualListBehaviour(dualList, False)

		End Sub

		Public Sub EnableDualList(ByRef dualList As DualListControl)

			SetDualListBehaviour(dualList, True)

		End Sub

		Private Sub SetDualListBehaviour(ByRef duallist As DualListControl, isEnabled As Boolean)

			If duallist.FindControl("cmdAdd") IsNot Nothing Then
				CType(duallist.FindControl("cmdAdd"), LinkButton).Enabled = isEnabled
			End If
			If duallist.FindControl("cmdRemove") IsNot Nothing Then
				CType(duallist.FindControl("cmdRemove"), LinkButton).Enabled = isEnabled
			End If
			If duallist.FindControl("cmdAddAll") IsNot Nothing Then
				CType(duallist.FindControl("cmdAddAll"), LinkButton).Enabled = isEnabled
			End If
			If duallist.FindControl("cmdRemoveAll") IsNot Nothing Then
				CType(duallist.FindControl("cmdRemoveAll"), LinkButton).Enabled = isEnabled
			End If
			If duallist.FindControl("lstAvailable") IsNot Nothing Then
				CType(duallist.FindControl("lstAvailable"), ListBox).Enabled = isEnabled
			End If
			If duallist.FindControl("lstAssigned") IsNot Nothing Then
				CType(duallist.FindControl("lstAssigned"), ListBox).Enabled = isEnabled
			End If

		End Sub

		Public Sub RebindDualList(ByRef duallist As DualListControl, available As ArrayList, assigned As ArrayList)

            Dim lstAvailable As ListBox = Nothing
            Dim lstAssigned As ListBox = Nothing

			If duallist.FindControl("lstAvailable") IsNot Nothing Then
				lstAvailable = CType(duallist.FindControl("lstAvailable"), ListBox)
			End If
			If duallist.FindControl("lstAssigned") IsNot Nothing Then
				lstAssigned = CType(duallist.FindControl("lstAssigned"), ListBox)
			End If

			If (lstAvailable IsNot Nothing And lstAssigned IsNot Nothing) Then

				lstAvailable.DataSource = available
				lstAvailable.DataBind()
				SortDualList(lstAssigned)

				lstAssigned.DataSource = assigned
				lstAssigned.DataBind()
				SortDualList(lstAssigned)

			End If

		End Sub

		Private Sub SortDualList(ctlListBox As ListBox)

			Dim arrListItems As New ArrayList()

			For Each objListItem As ListItem In ctlListBox.Items
				arrListItems.Add(objListItem)
			Next

			arrListItems.Sort(New ListItemComparer())
			ctlListBox.Items.Clear()

			For Each objListItem As ListItem In arrListItems
				ctlListBox.Items.Add(objListItem)
			Next

		End Sub

	End Module

End Namespace