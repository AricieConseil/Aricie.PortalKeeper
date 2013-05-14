Namespace ComponentModel
    <Flags()> _
    Public Enum EntityServices
        None = 0
        Viewable = 1
        Controllable = 2
        Portable = 4
        Indexable = 8
        Localizable = 16
        Versionable = 32
        Commentable = 64
        Categorizable = 128
        Tagable = 256
        Votable = 512
        Relatable = 1024
        Searchable = 2048
        Convertible = 4096
    End Enum
End Namespace