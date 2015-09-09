
Imports System.ComponentModel

Namespace ComponentModel
    Public MustInherit Class DynamicAttributes
        Inherits ReflectedProviderContainer(Of Attribute)

        <Browsable(False)> _
        Public Overridable ReadOnly Property BaseType As Type
            Get
                Return GetType(Attribute)
            End Get
        End Property

        <Browsable(False)> _
        Public Overridable ReadOnly Property AttributeUsage As AttributeTargets
            Get
                Return AttributeTargets.All
            End Get
        End Property

        <Browsable(False)> _
        Public Overridable ReadOnly Property ExcludedUsage As AttributeTargets
            Get
                Return 0
            End Get
        End Property


        Public Overrides Function GetProviderIdsByNames() As IEnumerable(Of KeyValuePair(Of String, String))
            Dim toReturn As New SortedDictionary(Of String, String)
            For Each objAttributeType As Type In Me.GetCandidateAttributes()
                If BaseType.IsAssignableFrom(objAttributeType) Then
                    Dim usageOk As Boolean = True
                    For Each objUsage As AttributeUsageAttribute In objAttributeType.GetCustomAttributes(GetType(AttributeUsageAttribute), True)
                        If (objUsage.ValidOn And AttributeUsage) = 0 OrElse (objUsage.ValidOn And ExcludedUsage) <> 0 Then
                            usageOk = False
                        Else
                            usageOk = True
                        End If
                    Next
                    If usageOk Then
                        toReturn(objAttributeType.Name.Replace("Attribute", "").Replace("Surrogate", "")) = objAttributeType.AssemblyQualifiedName
                    End If
                End If
            Next
            Return toReturn
        End Function


        Public MustOverride Function GetCandidateAttributes() As IEnumerable(Of Type)

        Private _ItemsSurrogated As List(Of Attribute)

        <Browsable(False)> _
        Public ReadOnly Property ItemsSurrogated As IEnumerable(Of Attribute)
            Get
                If _ItemsSurrogated Is Nothing Then
                    _ItemsSurrogated = New List(Of Attribute)(Me.Items.Count)
                    For Each objAttr In Me.Items

                        If TypeOf objAttr Is IAttributeSurrogate Then
                            Dim surrog As IAttributeSurrogate = DirectCast(objAttr, IAttributeSurrogate)
                            _ItemsSurrogated.Add(surrog.GetAttribute())
                        Else
                            _ItemsSurrogated.Add(objAttr)
                        End If


                    Next
                End If
                Return _ItemsSurrogated
            End Get
        End Property


    End Class
End Namespace