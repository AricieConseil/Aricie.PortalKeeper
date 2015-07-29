Imports System.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class ControllerAttributes
        Inherits DynamicWebAPIAttributes

        <Browsable(False)> _
        Public Overrides ReadOnly Property AttributeUsage As AttributeTargets
            Get
                Return AttributeTargets.Class
            End Get
        End Property

        <Browsable(False)> _
        Public Overrides ReadOnly Property ExcludedUsage As AttributeTargets
            Get
                Return AttributeTargets.Property Or AttributeTargets.Parameter
            End Get
        End Property

    End Class


    Public Class ActionAttributes
        Inherits DynamicWebAPIAttributes

        <Browsable(False)> _
        Public Overrides ReadOnly Property AttributeUsage As AttributeTargets
            Get
                Return AttributeTargets.Method
            End Get
        End Property

    End Class


    Public Class ParameterAttributes
        Inherits DynamicWebAPIAttributes

        <Browsable(False)> _
        Public Overrides ReadOnly Property AttributeUsage As AttributeTargets
            Get
                Return AttributeTargets.Parameter
            End Get
        End Property

    End Class
End NameSpace