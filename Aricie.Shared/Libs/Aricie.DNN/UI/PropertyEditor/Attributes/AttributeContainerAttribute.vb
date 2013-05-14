

Namespace UI.Attributes

    <AttributeUsage(AttributeTargets.Property)> _
    Public Class AttributeContainerAttribute
        Inherits Attribute

        Private _AttributeProviderType As Type
        Private _ProviderAttributes As List(Of Attribute)
        Private _Attributes As New List(Of Attribute)


        Protected Sub New()

        End Sub

        Public Sub New(ByVal attributeProviderType As Type)

            If attributeProviderType IsNot Nothing Then
                Me._AttributeProviderType = attributeProviderType
            End If

        End Sub

        Public Function GetAttributes(ByVal objValueType As Type) As IList(Of Attribute)
            Dim toReturn As New List(Of Attribute)
            toReturn.AddRange(Me._Attributes)
            toReturn.AddRange(ProviderAttributes(objValueType))
            Return toReturn
        End Function

        Public ReadOnly Property ProviderAttributes(ByVal objValueType As Type) As List(Of Attribute)
            Get
                If _ProviderAttributes Is Nothing Then
                    _ProviderAttributes = New List(Of Attribute)
                    If _AttributeProviderType IsNot Nothing Then
                        If _AttributeProviderType.GetInterface("IAttributesProvider") IsNot Nothing Then
                            Dim attributeProvider As IAttributesProvider = DirectCast(Activator.CreateInstance(_AttributeProviderType), IAttributesProvider)

                            _ProviderAttributes.AddRange(attributeProvider.GetAttributes)
                        ElseIf _AttributeProviderType.GetInterface("IDynamicAttributesProvider") IsNot Nothing Then
                            Dim attributeProvider As IDynamicAttributesProvider = DirectCast(Activator.CreateInstance(_AttributeProviderType), IDynamicAttributesProvider)

                            _ProviderAttributes.AddRange(attributeProvider.GetAttributes(objValueType))
                        End If

                    End If
                End If
                Return _ProviderAttributes
            End Get
        End Property

        Protected Sub AddAttribute(ByVal attr As Attribute)
            Me._Attributes.Add(attr)
        End Sub


    End Class



End Namespace
