
Namespace UI.Attributes

    Public Interface IAttributesProvider

        Function GetAttributes() As IEnumerable(Of Attribute)


    End Interface

    Public Interface IDynamicAttributesProvider

        Function GetAttributes(ByVal valueType As Type) As IEnumerable(Of Attribute)


    End Interface


    '<AttributeUsage(AttributeTargets.Property)> _
    'Public MustInherit Class InnerAttributesAttribute
    '    Inherits Attribute

    '    'Private _keyAttributes As List(Of Attribute)
    '    'Private _valueAttributes As List(Of Attribute)
    '    Private _InnerAttributes As New Dictionary(Of String, List(Of Attribute))
    '    Private _attributesBuilt As Boolean = False

    '    'Public Sub New()
    '    '    _keyAttributes = New List(Of Attribute)
    '    '    _valueAttributes = New List(Of Attribute)
    '    '    _attributesBuilt = False
    '    'End Sub

    '    'Public ReadOnly Property InnerAttributes() As Attribute()
    '    '    Get

    '    '        Return Me.InnerAttributes("")
    '    '    End Get
    '    'End Property

    '    'Public ReadOnly Property InnerAttributes(ByVal key As String) As Attribute()
    '    '    Get

    '    '        Dim toReturn As Attribute()
    '    '        If Not _attributesBuilt Then
    '    '            Me.InitAttributes()
    '    '        End If
    '    '        If _InnerAttributes.ContainsKey(key) Then
    '    '            toReturn = Me._InnerAttributes(key).ToArray
    '    '        Else
    '    '            toReturn = New Attribute() {}

    '    '        End If
    '    '        Return toReturn
    '    '    End Get
    '    'End Property


    '    Public ReadOnly Property InnerAttributes() As Dictionary(Of String, List(Of Attribute))
    '        Get


    '            If Not _attributesBuilt Then
    '                Me.InitAttributes()
    '            End If

    '            Return Me._InnerAttributes
    '        End Get
    '    End Property


    '        'Public ReadOnly Property InnerKeyAttributes() As Attribute()
    '        '    Get

    '        '        If Not _attributesBuilt Then
    '        '            BuildAttributes()
    '        '        End If

    '        '        Return _keyAttributes.ToArray()
    '        '    End Get
    '        'End Property

    '        'Public ReadOnly Property InnerValueAttributes() As Attribute()
    '        '    Get

    '        '        If Not _attributesBuilt Then
    '        '            BuildAttributes()
    '        '        End If

    '        '        Return _valueAttributes.ToArray()
    '        '    End Get
    '    'End Property

    '    Protected Sub AddInnerAttribute(ByVal attr As Attribute)
    '        Me.AddInnerAttribute("", attr)
    '    End Sub

    '    Protected Sub AddInnerAttribute(ByVal key As String, ByVal attr As Attribute)
    '        If Not Me._InnerAttributes.ContainsKey(key) Then
    '            Me._InnerAttributes(key) = New List(Of Attribute)
    '        End If
    '        Me._InnerAttributes(key).Add(attr)
    '    End Sub

    '    'Protected Sub AddKeyAttribute(ByVal attr As Attribute)
    '    '    _attributesBuilt = True

    '    '    _keyAttributes.Add(attr)
    '    'End Sub

    '    'Protected Sub AddValueAttribute(ByVal attr As Attribute)
    '    '    _attributesBuilt = True

    '    '    _valueAttributes.Add(attr)
    '    'End Sub

    '    Private Sub InitAttributes()
    '        Me.BuildAttributes()
    '        Me._attributesBuilt = True
    '    End Sub

    '    Protected MustOverride Sub BuildAttributes()

    'End Class

End Namespace
