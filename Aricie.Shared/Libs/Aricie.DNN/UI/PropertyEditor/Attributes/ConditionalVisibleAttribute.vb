Imports System.Reflection
Imports Aricie.DNN.ComponentModel

Namespace UI.Attributes


    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Method, AllowMultiple:=True)> _
    Public Class ConditionalVisibleAttribute
        Inherits Attribute

        Public Property Value As ConditionalVisibleInfo





        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean)
            Me.Value = New ConditionalVisibleInfo(masterPropertyName, negate, enforcePostBack)
        End Sub
        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean, ByVal ParamArray objMatchingValues() As Object)
            Me.Value = New ConditionalVisibleInfo(masterPropertyName, negate, enforcePostBack, objMatchingValues)
        End Sub

        Public Sub New(ByVal masterPropertyName As String, ByVal negate As Boolean, ByVal enforcePostBack As Boolean, ByVal matchingPredicate As Predicate(Of Object))
            Me.Value = New ConditionalVisibleInfo(masterPropertyName, negate, enforcePostBack, matchingPredicate)
        End Sub

        Public Sub New(ByVal enforcePostBack As Boolean, ByVal masterPropertyName As String, ByVal masterNegate As Boolean, ByVal masterValue As Object, secondaryPropertyName As String, secondaryNegate As Boolean, secondaryValue As Object)
            Me.Value = New ConditionalVisibleInfo(enforcePostBack, masterPropertyName, masterNegate, masterValue, secondaryPropertyName, secondaryNegate, secondaryValue)
        End Sub





    End Class






End Namespace
