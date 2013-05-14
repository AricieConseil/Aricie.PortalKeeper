Imports System.Reflection

Namespace UI.WebControls
    'Public Class AssemblySelectorDropDownList
    '    Inherits DropDownList

    '    Private _assemblies As Assembly()

    '    Public Event AssemblyChanged(ByVal sender As Object, ByVal assembly As Assembly)

    '    Private Sub AssemblySelectorDropDownList_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    '        Me._assemblies = System.AppDomain.CurrentDomain.GetAssemblies()

    '        Me.DataTextField = "FullName"
    '        Me.DataValueField = "FullName"
    '        Me.DataSource = Me._assemblies
    '        Me.DataBind()

    '    End Sub


    '    Private Sub AssemblySelectorDropDownList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


    '    End Sub

    '    Private Sub AssemblySelectorDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SelectedIndexChanged
    '        RaiseEvent AssemblyChanged(sender, Me._assemblies(Me.SelectedIndex))
    '    End Sub

    '    Public ReadOnly Property Assembly() As Assembly
    '        Get
    '            If Me.SelectedIndex <> -1 Then
    '                Return Me._assemblies(Me.SelectedIndex)
    '            Else
    '                Return Nothing
    '            End If
    '        End Get
    '    End Property
    'End Class

    Public Class AssemblySelectorDropDownList
        Inherits SelectorControl(Of Assembly)


        Public Overrides Function GetEntitiesG() As IList(Of Assembly)
            Return AppDomain.CurrentDomain.GetAssemblies()
        End Function

        Private Sub AssemblySelectorDropDownList_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            If Me.DataTextField = "" Then
                Me.DataTextField = "FullName"
            End If
            If Me.DataValueField = "" Then
                Me.DataValueField = "FullName"
            End If
        End Sub
    End Class
End Namespace
