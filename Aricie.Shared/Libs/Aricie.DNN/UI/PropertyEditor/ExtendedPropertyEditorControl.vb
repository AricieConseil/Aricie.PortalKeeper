Imports DotNetNuke.UI.WebControls
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Reflection

Namespace UI.WebControls
    Public Class ExtendedPropertyEditorControl
        Inherits PropertyEditorControl

        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public ReadOnly Property PropertiesFields() As List(Of Field)
            Get
                If (Me.ViewState.Item("PropertiesFields") Is Nothing) Then
                    Me.ViewState.Item("PropertiesFields") = New List(Of Field)
                End If
                Return DirectCast(Me.ViewState.Item("PropertiesFields"), List(Of Field))
            End Get
        End Property

        Protected Overrides Sub CreateEditor()
            If (Not Me.DataSource Is Nothing) Then
                Dim tbl As New Table
                tbl.ID = "tbl"

                Dim list2 As New List(Of Field)

                For Each fd As Field In Me.PropertiesFields

                    If String.IsNullOrEmpty(fd.FieldName) Then
                        Throw New ArgumentException("Le champ FieldName doit etre renseigné.")
                    End If

                    list2.Add(fd)
                Next

                If MyBase.AutoGenerate Then

                    For Each efi As Object In Me.UnderlyingDataSource

                        Dim ef As PropertyInfo = CType(efi, PropertyInfo)
                        Dim finded As Boolean = False
                        'Remplacement de fonction anonyme : list2.Exists(Function(f) f.FieldName.ToLower = ef.Name.ToLowerInvariant) 
                        For Each myField As Field In list2
                            If myField.FieldName.ToLower = ef.Name.ToLowerInvariant Then
                                finded = True
                            End If
                        Next
                        If Not finded Then
                            Dim fd As New Field
                            fd.FieldName = ef.Name
                            fd.LabelMode = LabelMode.Left
                            list2.Add(fd)
                        End If
                    Next
                End If

                For Each field As Field In list2
                    Me.AddEditorRow(tbl, field)
                Next
                Me.Controls.Add(tbl)
            End If
        End Sub

        Protected Overrides Sub AddEditorRow(ByRef tbl As Table, ByVal obj As Object)
            Dim field As Field = CType(obj, Field)

            If TypeOf field Is CollectionField Then
                MyBase.AddEditorRow(tbl, field.FieldName, _
                                     New CollectionEditorInfoAdapter(Me.DataSource, field.FieldName, _
                                                                      DirectCast(field, CollectionField)))
            Else
                If String.IsNullOrEmpty(field.Type) Then
                    MyBase.AddEditorRow(tbl, field.FieldName, _
                                         New StandardEditorInfoAdapter(Me.DataSource, field.FieldName))
                Else
                    MyBase.AddEditorRow(tbl, field.FieldName, _
                                         New FieldEditorInfoAdapter(Me.DataSource, field.FieldName, field))
                End If
            End If
        End Sub
    End Class
End Namespace
