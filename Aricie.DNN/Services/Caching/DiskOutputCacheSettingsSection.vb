Imports System
Imports System.Configuration

Namespace Services.Caching
    Public Class DiskOutputCacheSettingsSection
        Inherits ConfigurationSection
        ' Properties
        <ConfigurationProperty("cachedUrls")> _
        Public ReadOnly Property CachedUrls() As CachedUrlsCollection
            Get
                Return DirectCast(MyBase.Item("cachedUrls"), CachedUrlsCollection)
            End Get
        End Property

        <ConfigurationProperty("fileRemovalDelay", DefaultValue:="00:00:15", IsKey:=False, IsRequired:=False)> _
        Public Property FileRemovalDelay() As TimeSpan
            Get
                Return DirectCast(MyBase.Item("fileRemovalDelay"), TimeSpan)
            End Get
            Set(ByVal value As TimeSpan)
                MyBase.Item("fileRemovalDelay") = value
            End Set
        End Property

        <ConfigurationProperty("fileScavangingDelay", DefaultValue:="00:10:00", IsKey:=False, IsRequired:=False)> _
        Public Property FileScavangingDelay() As TimeSpan
            Get
                Return DirectCast(MyBase.Item("fileScavangingDelay"), TimeSpan)
            End Get
            Set(ByVal value As TimeSpan)
                MyBase.Item("fileScavangingDelay") = value
            End Set
        End Property

        <ConfigurationProperty("fileValidationDelay", DefaultValue:="00:00:05", IsKey:=False, IsRequired:=False)> _
        Public Property FileValidationDelay() As TimeSpan
            Get
                Return DirectCast(MyBase.Item("fileValidationDelay"), TimeSpan)
            End Get
            Set(ByVal value As TimeSpan)
                MyBase.Item("fileValidationDelay") = value
            End Set
        End Property

        <ConfigurationProperty("location", DefaultValue:="", IsKey:=False, IsRequired:=False)> _
        Public Property Location() As String
            Get
                Return CStr(MyBase.Item("location"))
            End Get
            Set(ByVal value As String)
                MyBase.Item("location") = value
            End Set
        End Property

        <ConfigurationProperty("varyByLimitPerUrl", DefaultValue:=&H100, IsKey:=False, IsRequired:=False)> _
        Public Property VaryByLimitPerUrl() As Integer
            Get
                Return CInt(MyBase.Item("varyByLimitPerUrl"))
            End Get
            Set(ByVal value As Integer)
                MyBase.Item("varyByLimitPerUrl") = value
            End Set
        End Property

    End Class
End Namespace

