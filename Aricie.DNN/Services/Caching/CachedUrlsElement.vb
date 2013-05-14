Imports System
Imports System.Configuration

Namespace Services.Caching
    Public Class CachedUrlsElement
        Inherits ConfigurationElement
        ' Properties
        <ConfigurationProperty("duration", DefaultValue:="00:00:00", IsKey:=False, IsRequired:=False)> _
        Public Property Duration() As TimeSpan
            Get
                Return DirectCast(MyBase.Item("duration"), TimeSpan)
            End Get
            Set(ByVal value As TimeSpan)
                MyBase.Item("duration") = value
            End Set
        End Property

        <ConfigurationProperty("emptyPathInfoOnly", DefaultValue:=False, IsKey:=False, IsRequired:=False)> _
        Public Property EmptyPathInfoOnly() As Boolean
            Get
                Return CBool(MyBase.Item("emptyPathInfoOnly"))
            End Get
            Set(ByVal value As Boolean)
                MyBase.Item("emptyPathInfoOnly") = value
            End Set
        End Property

        <ConfigurationProperty("emptyQueryStringOnly", DefaultValue:=False, IsKey:=False, IsRequired:=False)> _
        Public Property EmptyQueryStringOnly() As Boolean
            Get
                Return CBool(MyBase.Item("emptyQueryStringOnly"))
            End Get
            Set(ByVal value As Boolean)
                MyBase.Item("emptyQueryStringOnly") = value
            End Set
        End Property

        <ConfigurationProperty("path", DefaultValue:="", IsKey:=True, IsRequired:=True)> _
        Public Property Path() As String
            Get
                Return CStr(MyBase.Item("path"))
            End Get
            Set(ByVal value As String)
                MyBase.Item("path") = value
            End Set
        End Property

        <ConfigurationProperty("serveFromMemory", DefaultValue:=False, IsKey:=False, IsRequired:=False)> _
        Public Property ServeFromMemory() As Boolean
            Get
                Return CBool(MyBase.Item("serveFromMemory"))
            End Get
            Set(ByVal value As Boolean)
                MyBase.Item("serveFromMemory") = value
            End Set
        End Property

        <ConfigurationProperty("varyBy", DefaultValue:="", IsKey:=False, IsRequired:=False)> _
        Public Property VaryBy() As String
            Get
                Return CStr(MyBase.Item("varyBy"))
            End Get
            Set(ByVal value As String)
                MyBase.Item("varyBy") = value
            End Set
        End Property

        <ConfigurationProperty("verbs", DefaultValue:="GET", IsKey:=False, IsRequired:=False)> _
        Public Property Verbs() As String
            Get
                Return CStr(MyBase.Item("verbs"))
            End Get
            Set(ByVal value As String)
                MyBase.Item("verbs") = value
            End Set
        End Property

    End Class
End Namespace

