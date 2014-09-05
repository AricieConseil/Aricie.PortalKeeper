Namespace Web
    <Flags()> _
    Public Enum HttpVerb
        Unknown = 0
        [Get] = 1
        Post = 2
        Put = 4
        Delete = 8
        Head = 16
        Options = 32
        Trace = 64
    End Enum
End NameSpace