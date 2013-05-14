Namespace Security.Trial
    ''' <summary>
    ''' Type of display that will be shortcircuited by the trial
    ''' </summary>
    ''' <remarks></remarks>
    <Flags()> _
    Public Enum TrialPropertyMode
        Hide = 2
        Disable = 4
        NoDelete = 8
        NoAdd = 16
    End Enum
End Namespace